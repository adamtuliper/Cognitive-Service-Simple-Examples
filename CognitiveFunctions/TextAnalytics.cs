using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveFunctions
{
    public class TextAnalytics
    {
        /// <summary>
        /// Azure portal URL.
        /// </summary>
        private const string BaseUrl = "https://westus.api.cognitive.microsoft.com/";

        /// <summary>
        /// Your account key goes here. This is for the text analytics service.
        /// Use your own key. This is a demo key and will expire shortly.
        /// As of now, you can get keys here (though that may change location) and they can take 10 mins or so to become active
        /// https://www.microsoft.com/cognitive-services/en-us/subscriptions?displayClass=subscription-free-trials
        /// </summary>
        private const string AccountKey = "1b389f9abbcd4e5a98f83b466f5bd3b2 ";

        /// <summary>
        /// Maximum number of languages to return in language detection API.
        /// </summary>
        private const int NumLanguages = 1;


        public async static Task<(string keyPhrases, string language, string sentiment)> ProcessLanguage()
        {
            var result = await MakeRequests();

            return result;

        }

        //private static LanguageRequest PopulateDocuments()
        //{
        //    LanguageRequest requestText = new Microsoft.ProjectOxford.Text.Language.LanguageRequest();
        //    requestText.Documents.Add(
        //        new Microsoft.ProjectOxford.Text.Core.Document()
        //        { Id = "One", Text = "The quick brown fox jumped over the hedge" });
        //    requestText.Documents.Add(
        //        new Microsoft.ProjectOxford.Text.Core.Document()
        //        { Id = "Two", Text = "March is a green month" });
        //    requestText.Documents.Add(
        //        new Microsoft.ProjectOxford.Text.Core.Document()
        //        { Id = "Three", Text = "When I press enter the program crashes" });
        //    requestText.Documents.Add(
        //        new Microsoft.ProjectOxford.Text.Core.Document()
        //        { Id = "4", Text = "Pressing return - the program crashes" });
        //    requestText.Documents.Add(
        //        new Microsoft.ProjectOxford.Text.Core.Document()
        //        { Id = "5", Text = "Los siento, no hablo Enspanol" });

        //    return requestText;
        //}

        private static async Task<(string keyPhrases, string language, string sentiment)> MakeRequests()
        {

            //Note usage of default. You can't use 'new' with a tuple type
            var responses = default((string keyPhrases, string languages, string sentiment));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AccountKey);
                //still shows only with Content-Type: application/json
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Request body. Insert your text data here in JSON format.

                //string req = JsonConvert.SerializeObject(requestDocs);
                string json = "{\"documents\":[" +
                    "{\"id\":\"1\",\"text\":\"Internet Explorer 6 in Mac OS X\"}," +
                    "{\"id\":\"2\",\"text\":\"hello foo world\"}," +
                    "{\"id\":\"3\",\"text\":\"I absolutely love politics.\"}," +
                    "{\"id\":\"4\",\"text\":\"I absolutely love politics. Seriously!\"}," +
                    "{\"id\":\"five\",\"text\":\"hello my world\"},]}";

                byte[] byteData = Encoding.UTF8.GetBytes(json);

                // Detect key phrases
                var uri = "text/analytics/v2.0/keyPhrases";
                var response = await CallEndpoint(client, uri, byteData);
                Console.WriteLine("\nDetect key phrases response:\n" + response);
                responses.keyPhrases = response;

                // Detect language
                uri = "text/analytics/v2.0/languages?numberOfLanguagesToDetect=1";
                response = await CallEndpoint(client, uri, byteData);
                Console.WriteLine("\nDetect language response:\n" + response);
                responses.languages = response;

                // Detect sentiment
                uri = "text/analytics/v2.0/sentiment";
                response = await CallEndpoint(client, uri, byteData);
                Console.WriteLine("\nDetect sentiment response:\n" + response);
                responses.sentiment = response;


                //Return tuple
                return responses;
            }
        }

        static async Task<String> CallEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

    }
}
