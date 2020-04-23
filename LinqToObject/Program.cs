﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LinqToObject
{
    class Q
    {
        string text = @"Historically, the world of data and the world of objects" +
          @" have not been well integrated. Programmers work in C# or Visual Basic" +
          @" and also in SQL or XQuery. On the one side are concepts such as classes," +
          @" objects, fields, inheritance, and .NET Framework APIs. On the other side" +
          @" are tables, columns, rows, nodes, and separate languages for dealing with" +
          @" them. Data types often require translation between the two worlds; there are" +
          @" different standard functions. Because the object world has no notion of query, a" +
          @" query can only be represented as a string without compile-time type checking or" +
          @" IntelliSense support in the IDE. Transferring data from SQL tables or XML trees to" +
          @" objects in memory is often tedious and error-prone.";

        string aString = "ABCDE99F-J7.4-12-89A";


        public void CountWords()
        {
            string searchTerm = "data";

            string[] source = text.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

            var matchQuery = from word in source
                             where word.ToLowerInvariant() == searchTerm.ToLowerInvariant()
                             select word;

            int wordCount = matchQuery.Count();

            Console.WriteLine($"{wordCount} occurrences(s) of the search term  {searchTerm}");
        }

        public void FindSentences()
        {
            string[] sentences = text.Split(new char[] { '.', '?', '!' });

            string[] wordsToMatch = { "Historically", "data", "integrated" };

            var sentenceQuery = from sentence in sentences
                                let w = sentence.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' },
                                                        StringSplitOptions.RemoveEmptyEntries)
                                where w.Distinct().Intersect(wordsToMatch).Count() == wordsToMatch.Count()
                                select sentence;

            foreach (string item in sentenceQuery)
            {
                Console.WriteLine(item);
            }
        }

        public void QueryAString()
        {
            IEnumerable<char> stringQuery =
                from ch in aString
                where Char.IsDigit(ch)
                select ch;

            foreach (char item in stringQuery)
            {
                Console.Write($"{item}  ");
            }

            int count = stringQuery.Count();
            Console.WriteLine($"count = {count}");

            IEnumerable<char> stringQuery2 = aString.TakeWhile(c => c != '.');//տպում է մինչև նշված char ը
            foreach (char item in stringQuery2)
            {
                Console.Write(item);
            }
        }

        string startFolder = @"C:\Program Files (x86)\Microsoft Visual Studio\";
        public void QueryWithRegEx()
        {
            IEnumerable<System.IO.FileInfo> fileList = GetFiles(startFolder);

            System.Text.RegularExpressions.Regex searchTerm =
                new System.Text.RegularExpressions.Regex(@"Visual (Basic|C#|C\+\+|Studio)");

            var queryMatchingFiles =
                from file
                in fileList
                where file.Extension == ".htm"
                let fileText = System.IO.File.ReadAllText(file.FullName)
                let matches = searchTerm.Matches(fileText)
                where matches.Count > 0
                select new
                {
                    name = file.FullName,
                    matchedValues = from System.Text.RegularExpressions.Match match
                                    in matches
                                    select match.Value
                };

            Console.WriteLine($"The term \"{searchTerm.ToString()}\" was found in:");

            foreach (var item in queryMatchingFiles)
            {
                string s = item.name.Substring(startFolder.Length - 1);
                Console.WriteLine(s);

                foreach (var item2 in item.matchedValues)
                {
                    Console.WriteLine($"  {item2}");
                }
            }

            IEnumerable<System.IO.FileInfo> GetFiles(string path)
            {
                if (!System.IO.Directory.Exists(path))
                    throw new System.IO.DirectoryNotFoundException();

                string[] fileNames = null;
                List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();

                fileNames = System.IO.Directory.GetFiles(path, "*.*",
                    System.IO.SearchOption.AllDirectories);
                foreach (string name in fileNames)
                {
                    files.Add(new System.IO.FileInfo(name));
                }
                return files;
            }
        }

        public void CompareLists()
        {
            string[] names1 = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/names1.txt");
            string[] names2 = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/names2.txt");

            IEnumerable<string> differenceQuery =
                names1.Except(names2);

            Console.WriteLine("The following lines are in names1.txt but not names2.txt ");

            foreach (var item in differenceQuery)
            {
                Console.WriteLine(item);
            }
        }

        public void SortLines()
        {
            string[] scores = File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/scores.txt");

            // Change this to any value from 0 to 4.  
            int sortField = 4;

            Console.WriteLine($"Sorted highest to lowest by field [{sortField}]");

            foreach (var item in RunQuery(scores, sortField))
            {
                Console.WriteLine(item);
            }

            IEnumerable<string> RunQuery(IEnumerable<string> source, int num)
            {
                var scoreQuery = from line in source
                                 let fields = line.Split(',')
                                 orderby fields[num] descending//ascending
                                 select line;

                return scoreQuery;

            }
        }

        public void SortData()
        {
            string[] scores = File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/spreadsheet1.txt");

            IEnumerable<string> query =
                from line in scores
                let x = line.Split(',')
                orderby x[1]
                select $"{x[2]} {x[1]} {x[0]}";

            File.WriteAllLines(@"C:/Users/khore/source/repos/LinqToObject/spreadsheet2.txt", query.ToArray());

            Console.WriteLine("Spreadsheet2.csv written to disk. Press any key to exit");

        }

        public void MergeStrings()
        {
            string[] fileA = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/names1.txt");
            string[] fileB = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/names2.txt");

            IEnumerable<string> concatQuery =
                fileA.Concat(fileB).OrderBy(s => s);

            OutputQueryResults(concatQuery, "Simple concatenate and sort. Duplicates are preserved:");

            IEnumerable<string> uniqueNamesQuery =
                fileA.Union(fileB).OrderBy(s => s);

            OutputQueryResults(uniqueNamesQuery, "Union removes duplicate names:");

            IEnumerable<string> commonNamesQuery =
                fileA.Intersect(fileB);
            OutputQueryResults(commonNamesQuery, "Merge based on intersect:");

            string nameMatch = "Garcia";

            IEnumerable<string> tempQuery1 =
                from name1 in fileA
                let n1 = name1.Split(',')
                where n1[0] == nameMatch
                select name1;

            IEnumerable<string> tempQuery2 =
               from name2 in fileB
               let n2 = name2.Split(',')
               where n2[0] == nameMatch
               select name2;

            IEnumerable<string> nameMatchQuery =
                tempQuery1.Concat(tempQuery2)
                .OrderBy(s => s);
            OutputQueryResults(nameMatchQuery, $"Concat based on partial name match \"{nameMatch}\":");

            void OutputQueryResults(IEnumerable<string> query, string message)
            {
                Console.WriteLine(System.Environment.NewLine + message);
                foreach (var item in query)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine($"{query.Count()} total name in list");
            }
        }

        public class Student
        {
            public int ID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<int> ExamScores { get; set; }
        }

        public void PopulateCollection()
        {
            string[] names = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/spreadsheet1.txt");
            string[] scores = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/scores.txt");

            IEnumerable<Student> queryNameScores =
                from nameLine in names
                let splitName = nameLine.Split(',')
                from scoreLine in scores
                let splitScoreLine = scoreLine.Split(',')
                where Convert.ToInt32(splitName[2]) == Convert.ToInt32(splitScoreLine[0])
                select new Student()
                {
                    FirstName = splitName[0],
                    LastName = splitName[1],
                    ID = Convert.ToInt32(splitName[2]),
                    ExamScores = (from scoreAsText in splitScoreLine.Skip(1)
                                  select Convert.ToInt32(scoreAsText))
                                  .ToList()
                };

            List<Student> students = queryNameScores.ToList();

            foreach (var item in students)
            {
                Console.WriteLine($"The average score of {item.FirstName} {item.LastName} is {item.ExamScores.Average()} ");
            }
        }

        public void SplitWithGroups()
        {
            string[] fileA = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/names1.txt");
            string[] fileB = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/names2.txt");

            var mergeQuery = fileA.Union(fileB);

            var groupQuery = from name in mergeQuery
                             let n = name.Split(',')
                             group name by n[0][0] into g
                             orderby g.Key
                             select g;

            foreach (var item in groupQuery)
            {
                string fileName = @"C:/Users/khore/source/repos/LinqToObject/testFile_" + item.Key + ".txt";

                Console.WriteLine(item.Key);

                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(fileName))
                {
                    foreach (var item2 in item)
                    {
                        streamWriter.WriteLine(item2);

                        Console.WriteLine($"{item2}");
                    }
                }
            }
        }

        public void JoinStrings()
        {
            string[] names = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/spreadsheet1.txt");
            string[] scores = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/scores.txt");

            IEnumerable<string> scoreQuery1 =
                from name in names
                let nameFields = name.Split(',')
                from id in scores
                let scoreFields = id.Split(',')
                where Convert.ToInt32(nameFields[2]) == Convert.ToInt32(scoreFields[0])
                select $"{nameFields[0]}, {scoreFields[1]}, {scoreFields[2]}, {scoreFields[3]} {scoreFields[4]}";

            OutputQueryResults(scoreQuery1, "Merge two spreadsheets");

            void OutputQueryResults(IEnumerable<string> query,  string message)
            {
                Console.WriteLine(Environment.NewLine + message);

                foreach (string item in query)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine($"{query.Count()} total names in list");
            }
        }

        public void SumColumns()
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:/Users/khore/source/repos/LinqToObject/scores.txt");

            int exam = 3;
            SingleColumn(lines, exam + 1);
            Console.WriteLine();
            MultiColumns(lines);

            void SingleColumn(IEnumerable<string> strs, int examNum)
            {
                Console.WriteLine("Single Column Query: ");

                var columnQuery =
                    from line in strs
                    let elements = line.Split(',')
                    select Convert.ToInt32(elements[examNum]);

                var results = columnQuery.ToList();

                double average = results.Average();
                int max = results.Max();
                int min = results.Min();

                Console.WriteLine($"Exam #{examNum}: Average:{average:##.##} " +
                    $"Hig Score:{max} Low Score {min}");
            }

            void MultiColumns(IEnumerable<string> strs)
            {
                Console.WriteLine("Multi Column Query:");

                IEnumerable<IEnumerable<int>> multiColumns =
                    from line in strs
                    let element = line.Split(',')
                    let scores = element.Skip(1)
                    select (from str in scores
                            select Convert.ToInt32(str));

                var results = multiColumns.ToList();
                int columnCount = results[0].Count();

                for (int column = 0; column < columnCount; column++)
                {
                    var results2 = from row in results
                                   select row.ElementAt(column);

                    double average = results2.Average();
                    int max = results2.Max();
                    int min = results2.Min();

                    Console.WriteLine($"Exam #{column+1}: Average:{average:##.##} " +
                        $"Hig Score:{max} Low Score {min}");
                }
            }
        }

        public void IntroToLINQ()
        {
            int[] numbers = new int[10] { 5, 10, 8, 3, 6, 12, 6, 7, 8, 9 };
            //8ms -9
            var numQuery =
                from num in numbers
                where (num % 2) == 0
                orderby num
                select num;
            //fast 5ms -6
            IEnumerable<int> numQuery2 = numbers
                .Where(num => num % 2 == 0)
                .OrderBy(n => n);

            foreach (int item in numQuery)
            {
                Console.Write($"{item}  ");
            }
            Console.WriteLine();
            foreach (int item in numQuery2)
            {
                Console.Write($"{item}  ");
            }
        }

        class Students
        {
            public string First { get; set; }
            public string Last { get; set; }
            public int ID { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public List<int> Scores;
        }

        class Teachers
        {
            public string First { get; set; }
            public string Last { get; set; }
            public int ID { get; set; }
            public string City { get; set; }
        }
        public void DataTransformations()
        {
            List<Students> students = new List<Students>()
            {
                new Students 
                { 
                    First="Svetlana",
                    Last="Omelchenko",
                    ID=111,
                    Street="123 Main Street",
                    City="Seattle",
                    Scores= new List<int> { 97, 92, 81, 60 } 
                },
                new Students 
                { 
                    First="Claire",
                    Last="O’Donnell",
                    ID=112,
                    Street="124 Main Street",
                    City="Redmond",
                    Scores= new List<int> { 75, 84, 91, 39 } 
                },
                new Students 
                { 
                    First="Sven",
                    Last="Mortensen",
                    ID=113,
                    Street="125 Main Street",
                    City="Lake City",
                    Scores= new List<int> { 88, 94, 65, 91 } 
                },
            };
            List<Teachers> teachers = new List<Teachers>()
            {
                new Teachers { First="Ann", Last="Beebe", ID=945, City="Seattle" },
                new Teachers { First="Alex", Last="Robinson", ID=956, City="Redmond" },
                new Teachers { First="Michiyo", Last="Sato", ID=972, City="Tacoma" }
            };

            var peopleInSeattle = (
                from student in students
                where student.City == "Seattle"
                select student.Last)
                .Concat(
                from teacher in teachers
                where teacher.City == "Seattle"
                select teacher.Last);

            Console.WriteLine("The following students and teachers live in Seattle:");

            foreach (var person in peopleInSeattle)
            {
                Console.WriteLine(person);
            }

            var studentsToXML = new XElement
                ("Root",
                from student in students
                let scores = string.Join(",", student.Scores)
                select new XElement("student",
                           new XElement("First", student.First),
                           new XElement("Last", student.Last),
                           new XElement("Scores", scores)
                           )
                );

            Console.WriteLine(studentsToXML);
        }

        public void FormatQuery()
        {
            double[] radii = { 1, 2, 3 };

            IEnumerable<string> query =
                from rad in radii
                select $"Area = {rad * rad * Math.PI:F2}";

            foreach (string item in query)
            {
                Console.WriteLine(item);
            }
        }

        public void ToUp()
        {
            string sentence = "the quick brown fox jumps over the lazy dog";

            string[] words = sentence.Split(' ');

            var q3 = from word in words
                     orderby word.Length
                     select word;

            foreach (var item in q3)
            {

            }

            //11ms
            var query1 = from word in words
                        group word.ToUpper()
                        by word.Length
                        into gr
                        orderby gr.Key
                        select new
                        {
                            Length = gr.Key,
                            Words = gr
                        };
            //8ms
            var query2 = words
                .GroupBy(w => w.Length, w => w.ToUpper())
                .Select(g => new { Length = g.Key, Words = g })
                .OrderBy(o => o.Length);

            foreach (var obj in query2)//1
            {
                Console.WriteLine("Words of length {0}:", obj.Length);
                foreach (string word in obj.Words)
                    Console.WriteLine(word);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            new Q().ToUp();
        }
    }
}
