using System;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class DBAccess
    {
        DataTable dtResult;
        public int selectSessionID()
        {
            SqlDataReader myRead = null;
            int flaa = 0;
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
                    sb.Append("select session_id from dbo.prepchat_score where qid =(select max(qid) from dbo.prepchat_score)");
                    // Trace.TraceInformation("IN DB Access" + channel);
                    String sql = sb.ToString();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        dtResult = new DataTable();
                        myRead = command.ExecuteReader();
                        while (myRead.Read())
                        {
                            flaa = (int)myRead["session_id"];

                        }
                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return flaa;

        }


       
        public void insertTicket(int sessionid, double softscore, double techscore)
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
                    sb.Append("insert into dbo.prepchat_score(session_id,soft_score,tech_score) values(" + sessionid + "," + softscore + "," + techscore + ")");
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


        public int avgScore(int sessionID)
        {
            SqlDataReader myRead = null;
            int flaa = 0;
            int flaag = 0;
            int count = 0;
            var result = new Result();
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
                    sb.Append("select avg(soft_score) as avg_soft from dbo.prepchat_score where session_id=" + sessionID + "");
                    // Trace.TraceInformation("IN DB Access" + channel);
                   // sb.Append("select * from dbo.prepchat_score where session_id=26");
                    String sql = sb.ToString();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                         object count1 = command.ExecuteScalar();
                        //dtResult = new DataTable();
                        //dtResult.Load(command.ExecuteReader());
                        //myRead = command.ExecuteReader();
                        //while (myRead.Read())
                        //{
                        count=Convert.ToInt32(count1);


                        //    result.value = (int)myRead[0];
                        //    result.value1 = (int)myRead[2];
                        //}

                       // int count1 = (int)count;
                        
                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return count;
        }


        public int avgtechScore(int sessionID)
        {
            SqlDataReader myRead = null;
            int flaa = 0;
            int flaag = 0;
            int count = 0;
            var result = new Result();
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
                    sb.Append("select avg(tech_score) as avg_soft from dbo.prepchat_score where session_id="+ sessionID + "");
                    // Trace.TraceInformation("IN DB Access" + channel);
                    // sb.Append("select * from dbo.prepchat_score where session_id=26");
                    String sql = sb.ToString();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        object count1 = command.ExecuteScalar();
                        //dtResult = new DataTable();
                        //dtResult.Load(command.ExecuteReader());
                        //myRead = command.ExecuteReader();
                        //while (myRead.Read())
                        //{
                        count = Convert.ToInt32(count1);


                        //    result.value = (int)myRead[0];
                        //    result.value1 = (int)myRead[2];
                        //}

                        // int count1 = (int)count;

                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return count;
        }


    }



   

}
