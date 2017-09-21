using CognitiveFunctions;
using System;

namespace CognitiveDemos
{
    class Program
    {
        static void Main(string[] args)
        {
            //2 - Tuple, calling text analytics
            //var task2 = TextAnalytics.ProcessLanguage();
            //task2.Wait();
            //Console.WriteLine(task2.Result.keyPhrases);

            //3 - Face API with SDK (this is .net core so copied classes manually into my project)
            BlurFaces.Process();

            //4 - 
            //TopicDetection.MakeRequest();

            Console.ReadLine();
            
        }
    }
}