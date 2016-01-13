using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;
using RestSharp;

namespace Challenge {
    class Program {
        static void Main(string[] args) {
            Run();
            Console.ReadLine();
        }

        static void Run() {
            Console.WriteLine("Fetching rooms and student data...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string baseUri = ConfigurationManager.AppSettings["baseUri"];

            var client = new RestClient(baseUri);
            var response = client.Execute(CreateRequest());
            var classContent = JsonConvert.DeserializeObject<ClassData>(response.Content);

            Console.WriteLine("Retrieving {0} classes...", classContent.Classes.Length);
            Console.WriteLine();

            var average = classContent.Classes
                .AsParallel()
                .Aggregate(0, GetClass) / classContent.Classes.Length;

            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Finished fetching rooms and student data. Time elapsed: {0} seconds.", stopwatch.Elapsed.Seconds);

            Console.WriteLine();
            Console.WriteLine("Average number of students in each class (ignoring students younger than 25 years old): {0}", average);
        }

        static int GetClass(int acc, string uri) {
            Console.WriteLine("Retrieving class data from '{0}'...", uri);

            var client = new RestClient(uri);
            var response = client.Execute(CreateRequest());
            var content = JsonConvert.DeserializeObject<Class>(response.Content);

            int count = content.Students.Where(s => s.Age >= 25).Count();

            Console.WriteLine("Room: {0}, Number of students: {1}, Number of students 25 y/o and older: {2}"
                    , content.Room
                    , content.Students.Count()
                    , count);

            return String.IsNullOrWhiteSpace(content.Next) 
                ? acc + count 
                : GetClass(acc + count, content.Next);
        }

        static RestRequest CreateRequest() {
            var request = new RestRequest(Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();

            return request;
        }
    }
}
