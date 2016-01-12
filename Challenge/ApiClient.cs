using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace Challenge {
    public class ApiClient {
        public void Run() {
            Console.WriteLine("Fetching rooms and student data...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string baseUri = ConfigurationManager.AppSettings["baseUri"];

            var client = new RestClient(baseUri);
            var response = client.Execute(CreateRequest());

            var classContent = JsonConvert.DeserializeObject<ClassData>(response.Content);

            Console.WriteLine("Retrieving {0} classes...", classContent.Classes.Length);
            Console.WriteLine();

            var classes = new List<Class>();

            foreach (var uri in classContent.Classes) {
                classes.Add(GetClass(uri));
            }

            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Finished fetching rooms and student data. Time elapsed: {0} seconds.", stopwatch.Elapsed.Seconds);
            Console.WriteLine();

            foreach (var classData in classes) {
                Console.WriteLine("Room: {0}, Number of students: {1}, Number of students 25 y/o and older: {2}"
                    , classData.Room
                    , classData.Students.Count()
                    , classData.FilteredStudentsCount);
            }
           
            var average = classes.Select(c => c.FilteredStudentsCount).Average();

            Console.WriteLine();
            Console.WriteLine("Average number of students in each class (ignoring students younger than 25 years old): {0}", average);
        }

        public Class GetClass(string uri) {
            Console.WriteLine("Retrieving class data from '{0}'...", uri);

            var client = new RestClient(uri);
            var response = client.Execute(CreateRequest());

            var content = JsonConvert.DeserializeObject<Class>(response.Content);

            var students = content.Students.ToList();

            if (!String.IsNullOrWhiteSpace(content.Next)) { // Get the rest of students
                var classData = GetClass(content.Next);
                var nextStudents = classData.Students.ToList();
                students.AddRange(nextStudents);
                content.Students = students;
            }

            return content;
        }

        private RestRequest CreateRequest() {
            var request = new RestRequest(Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();

            return request;
        }
    }
}
