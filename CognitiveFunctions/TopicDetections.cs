using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CognitiveFunctions
{
    public class TopicDetection
    {
        /// <summary>
        /// Azure portal URL.
        /// </summary>
        private const string BaseUrl = "https://westus.api.cognitive.microsoft.com/";

        /// <summary>
        /// Your account key goes here.
        /// </summary>
        private const string AccountKey = "8139a8831c13424cae1cf26a9f8efe98";

        /// <summary>
        /// Path to file with JSON inputs.
        /// </summary>
        private const string InputFile = "<path to your input file>";

        /// <summary>
        /// How many minutes to poll before timeout.
        /// </summary>
        private const int TimeoutMinutes = 20;

        /// <summary>
        /// How many seconds to wait between subsequent polls.
        /// </summary>
        private const int PollPeriodSec = 60;

        public static async void MakeRequest()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AccountKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Request body.
                var byteData = File.ReadAllBytes(InputFile);

                // For list of optional request parameters, please visit the documentation pages.
                var uri = "text/analytics/v2.0/topics";

                // Start topic detection and get the URL we need to poll for results.
                uri = await CallTopicEndpoint(client, uri, byteData);
                Console.WriteLine("\nTopic detection started. You can poll for results here:\n" + uri);

                // Poll the service until the job has completed.
                var sw = new Stopwatch();
                sw.Start();
                while (sw.Elapsed.TotalMinutes < TimeoutMinutes)
                {
                    var result = await GetTopicResult(client, uri);
                    if (result.IndexOf("\"status\":\"Succeeded\"", StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                        Console.WriteLine("\nTopic detection succeeded! Result:\n" + result);
                        break;
                    }
                    Console.WriteLine('\n' + result);
                    Thread.Sleep(TimeSpan.FromSeconds(PollPeriodSec));
                }
            }
        }

        static async Task<string> CallTopicEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(uri, content);
                // Return URL containing OperationID to poll from.
                return response.Headers.GetValues("Operation-Location").First();
            }
        }

        static async Task<string> GetTopicResult(HttpClient client, string uri)
        {
            var response = await client.GetAsync(uri);
            return await response.Content.ReadAsStringAsync();
        }
    }
}

