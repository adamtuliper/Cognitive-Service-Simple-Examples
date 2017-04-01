using CognitiveFunctions;
using System;

namespace CognitiveDemos
{
    class Program
    {
        static void Main(string[] args)
        {
            //1 - using LUIS
            //var task1 = LUISMakerShowClient.ParseUserInput("What can you tell me about an arduino?");
            //task1.Wait();

            //2 - Tuple, calling text analytics
            var task2 = TextAnalytics.ProcessLanguage();
            task2.Wait();
            Console.WriteLine(task2.Result.keyPhrases);

            //3 - Face API with SDK (this is .net core so copied classes manually into my project)
            BlurFaces.Process();

            //4 - 
            TopicDetection.MakeRequest();
            
        }
    }
}