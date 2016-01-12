# Challenge

Ignoring students younger than 25 years old, what's the average number of students in each class?

Answer: Based on the data provided the number of classes are 11, and ignoring students younger than 25 years old there's an average of **20 students** per class. See the following screenshot:

![image](https://cloud.githubusercontent.com/assets/904058/12219752/8bcdd604-b705-11e5-89c7-b6b8d89dd3db.png)


---

This is a small console application written in C# that retrieves the json data from the provided endpoint, parses it and calculates the average number of students per class as stated above.

The source code is found on `Challenge/ApiClient.cs` and it's also shown below:

```cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace Challenge {
    public class ApiClient {
        public void Run() {
            Console.WriteLine("Fetching rooms and student data...");

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
```

---

Helper class:

```
public class Class {
        public string Note { get; set; }
        public string Room { get; set; }
        public IEnumerable<Student> Students { get; set; }
        public string Next { get; set; }
        public int FilteredStudentsCount { get { return Students.Where(s => s.Age >= 25).Count(); } }
    }
```
