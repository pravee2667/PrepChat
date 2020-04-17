using System;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class DBdeta
    {
        DataTable dtResult;
        public void insertTicket(int sessionid, double softscore,double techscore)
        {

            try
            {
                //context.ConversationData.TryGetValue<string>("channel",out channel);

                //string channel = "Milestones";
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                var cs = "Server=tcp:idcdbserver.database.windows.net,1433;Initial Catalog=IDCDB;Persist Security Info=False;User ID=idclogin;Password=Idc@login;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("insert into dbo.prepchat_score(session_id,soft_score,tech_score) values("+sessionid+","+ softscore + ","+ techscore + ")");
                    // Trace.TraceInformation("IN DB Access" + channel);
                    String sql = sb.ToString();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        dtResult = new DataTable();
                        dtResult.Load(command.ExecuteReader());
                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }


        }

    }
}
