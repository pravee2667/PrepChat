using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.BotBuilderSamples.Bots;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class Request
    {
        private static readonly HttpClient client = new HttpClient();

        public  async void requestbody(string actualAns,string userAns)

        {
            //    var values = new Dictionary<string, string>
            //{
            //{ "actualAns", "hello" },
            //{ "userAns", "world" }
            //};

            //    var content = new FormUrlEncodedContent(values);

            //    var response = await client.PostAsync("https://preptalk-gi.bfmdev1.com/evaluate_chat", content);

            //    var responseString = await response.Content.ReadAsStringAsync();

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost/yourwebapi");

            using (var httpClient = new HttpClient())
            {
                var uri = new Uri("https://preptalk-gi.bfmdev1.com/evaluate_chat");

                HttpContent stringContent1 = new StringContent(actualAns);
                HttpContent stringContent2 = new StringContent(userAns);
                using (var formData = new MultipartFormDataContent())
                {
                    //add content to form data
                    formData.Add(stringContent1, "actualAns");

                    //add config to form data
                    formData.Add(stringContent2, "userAns");

                    var response = httpClient.PostAsync(uri, formData).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                         dynamic dynamicObject = JsonConvert.DeserializeObject(jsonString);

                        double value=dynamicObject.softscore;
                        double value1 = dynamicObject.techscore;

                        DBAccess db = new DBAccess();
                        int sessionid=db.selectSessionID();

                        db.insertTicket(sessionid, value, value1);
                        //var content = JsonConvert.DeserializeObject<actualAns>(jsonString);
                        //var config = JsonConvert.DeserializeObject<Config>(jsonString);
                    }

                   // return response.ToString();
                    //response.Wait();

                    //if (!response.Result.IsSuccessStatusCode)
                    //{
                    //    //error handling code goes here
                    //}
                }
            }



        }









    }


}
