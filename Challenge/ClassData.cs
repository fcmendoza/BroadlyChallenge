using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge {
    public class ClassData {
        public string[] Classes { get; set; }
        public string Note { get; set; }
    }

    public class Class {
        public string Note { get; set; }
        public string Room { get; set; }
        public IEnumerable<Student> Students { get; set; }
        public string Next { get; set; }
        public int FilteredStudentsCount { get { return Students.Where(s => s.Age >= 25).Count(); } }
    }

    public class Student {
        public string Name { get; set; }
        public int Age { get; set; }
        public string ID { get; set; }
    }
}
