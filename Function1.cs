using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using System.Numerics;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string prompt = req.Query["prompt"];
            string context = req.Query["context"];

            string finalPrompt = "";

            
            if(prompt != null)
            {
                if (context != null)
                    finalPrompt += "generate multiple multiline feedback of 5 lines using keywords "+prompt+", with positive sentiment in the context of "+context;
                else
                {
                    finalPrompt += "generate multiple multiline feedback of 5 lines using keywords "+prompt+", with positive sentiment";
                }
            }
            else
            {
                return null;
            }
            // create an instance of the HttpClient class

            var client = new HttpClient();

            // specify the API endpoint you want to call

            var url = "https://api.openai.com/v1/completions";

            // create a dictionary to hold the request headers

            var headers = new Dictionary<string, string>();

            // add the headers to the dictionary

            //headers.Add("Authorization", "Bearer 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");
            //headers.Add("Authorization", "Bearer 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");
            headers.Add("Authorization", "Bearer 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");

            var body = new Dictionary<string, dynamic>
            {
                {"model", "text-davinci-003"},

                { "prompt", finalPrompt},

                { "max_tokens", 2048},

                { "temperature", 0},

                { "top_p", 1},

                { "n", 1},

                { "stream", false},

                { "logprobs", null}

                //{ "stop", "\n"}
            };


            var jsonData = JsonConvert.SerializeObject(body);
            var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // create a new HttpRequestMessage object with the specified method, url, and body
            Console.WriteLine(contentData);

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                 Content = contentData
            };


            // add the headers to the request

            foreach (var header in headers)

            {
                request.Headers.Add(header.Key, header.Value);
            }

            // make the API call using the SendAsync method of the HttpClient

            var response = await client.SendAsync(request);

            // check the status code of the response to make sure the call was successful

            if (response.IsSuccessStatusCode)

            {
                // if the call was successful, read the response content

                var content = await response.Content.ReadAsStringAsync();
                //string requestBody = await new StreamReader(content).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(content);

                string responseText = data.choices[0].text;
                string[] splitText = responseText.Split("\n");
                var filtered = splitText.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                string[] finalResponse = filtered.Select(x => x.Remove(0,2)).ToArray();

                return new OkObjectResult(finalResponse);

            }
            return null;
        }
    }
}
