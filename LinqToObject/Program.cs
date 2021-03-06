﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static LinqToObject.Lambda2;

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

            foreach (var item in source)
            {
                Console.WriteLine(item);
            }
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

            void OutputQueryResults(IEnumerable<string> query, string message)
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

                    Console.WriteLine($"Exam #{column + 1}: Average:{average:##.##} " +
                        $"Hig Score:{max} Low Score {min}");
                }
            }
        }

        public void IntroToLINQ()
        {
            List<int> numbers1 = new List<int>() { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            List<int> numbers2 = new List<int>() { 15, 14, 11, 13, 19, 18, 16, 17, 12, 10 };

            //8ms -9
            var numQuery =
                from num in numbers1
                where (num % 2) == 0
                orderby num
                select num;
            //fast 5ms -6
            IEnumerable<int> numQuery2 = numbers1
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

            var q4 = words
                .OrderBy(w => w.Length);

            var q5 = from word in words
                     let w = word.Split(' ')
                     orderby word.Length
                     select word;

            var q6 = sentence
                .Split(' ')
                .OrderBy(w => w.Length);

            var q11 = sentence
                .Split(' ')
                .Where(w => w.Length == 3);

            string[] planets1 = { "Mercury", "Venus", "Earth", "Jupiter" };
            string[] planets2 = { "Mercury", "Earth", "Mars", "Jupiter" };

            var q7 = planets1//հատումը վերցրած առաջինտ
                .Except(planets2);

            var q8 = planets1//երկուսի ընդհանուրը
                .Intersect(planets2);

            var q9 = planets1//երկուսի ընդհանուրը
               .Intersect(planets2);

            var q10 = planets1//միավորումը երկուսի
                .Union(planets2);

            foreach (var item in q11)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

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
                .GroupBy(w => w.Length,
                         w => w.ToUpper())
                .Select(g => new { Length = g.Key, Words = g })
                .OrderBy(o => o.Length);

            //foreach (var obj in query2)//1
            //{
            //    Console.WriteLine("Words of length {0}:", obj.Length);
            //    foreach (string word in obj.Words)
            //        Console.WriteLine(word);
            //}
        }

        class Market
        {
            public string Name { get; set; }
            public string[] Items { get; set; }
        }
        public void Markets()
        {
            List<Market> markets = new List<Market>
            {
                new Market { Name = "Emily's", Items = new string[] { "kiwi", "cheery", "banana" } },
                new Market { Name = "Kim's", Items = new string[] { "melon", "mango", "olive" } },
                new Market { Name = "Adam's", Items = new string[] { "kiwi", "apple", "orange" } },
            };

            IEnumerable<string> names = markets
                .Where(w => w.Items.All(item => item.Length == 5))
                .Select(m => m.Name);

            IEnumerable<string> names1 = from market in markets
                                         where market.Items.All(item => item.Length == 5)
                                         select market.Name;

            IEnumerable<string> names2 = markets
              .Where(w => w.Items.Any(item => item.StartsWith("o")))
              .Select(m => m.Name);

            IEnumerable<string> names3 = markets
              .Where(w => w.Items.Contains("kiwi"))
              .Select(m => m.Name);

            foreach (var item in names3)
            {
                Console.WriteLine(item);
            }
        }

        public void SelectVsSelectMany()
        {
            List<string> phrases = new List<string>() { "an apple a day", "the quick brown fox" };

            //var q1 = phrases
            //.OrderBy(w => w.Split(' '));

            var q2 = from phrase in phrases
                     from word in phrase.Split(' ')
                     select word;

            foreach (var item in q2)
            {
                Console.WriteLine(item);
            }
        }

        class Bouquet
        {
            public List<string> Flowers { get; set; }
        }

        public void SelectVsSelectMany2()
        {
            List<Bouquet> bouquets = new List<Bouquet>()
            {
                new Bouquet { Flowers = new List<string> { "sunflower", "daisy", "daffodil", "larkspur" }},
                new Bouquet{ Flowers = new List<string> { "tulip", "rose", "orchid" }},
                new Bouquet{ Flowers = new List<string> { "gladiolis", "lily", "snapdragon", "aster", "protea" }},
                new Bouquet{ Flowers = new List<string> { "larkspur", "lilac", "iris", "dahlia" }}
            };

            Stopwatch sw1 = Stopwatch.StartNew();
            IEnumerable<List<string>> q1 = bouquets
                .Select(bq => bq.Flowers);

            foreach (IEnumerable<String> item in q1)
            {
                foreach (var str in item)
                    Console.WriteLine(str);
            }
            Console.WriteLine(sw1.Elapsed);

            Console.WriteLine();

            Stopwatch sw2 = Stopwatch.StartNew();//fast
            IEnumerable<string> q2 = bouquets
                .SelectMany(bq => bq.Flowers);

            foreach (var item in q2)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine(sw2.Elapsed);
        }

        class Product
        {
            public string Name { get; set; }
            public int CategoryId { get; set; }
        }

        class Category
        {
            public int Id { get; set; }
            public string CategoryName { get; set; }
        }

        public void JoinOperations()
        {
            List<Product> products = new List<Product>
            {
                new Product { Name = "Cola", CategoryId = 0 },
                new Product { Name = "Tea", CategoryId = 0 },
                new Product { Name = "Apple", CategoryId = 1 },
                new Product { Name = "Kiwi", CategoryId = 1 },
                new Product { Name = "Carrot", CategoryId = 2 },
            };

            List<Category> categories = new List<Category>
            {
                new Category { Id = 0, CategoryName = "Beverage" },
                new Category { Id = 1, CategoryName = "Fruit" },
                new Category { Id = 2, CategoryName = "Vegetable" }
            };

            var q1 = from product in products
                     join category in categories
                                   on product.CategoryId
                                   equals category.Id
                     select new { product.Name, category.CategoryName };

            foreach (var item in q1)
            {
                //Console.WriteLine($"{item.Name} - {item.CategoryName}");
            }

            var q2 = from category in categories
                     join product in products
                                  on category.Id
                                  equals product.CategoryId
                                  into productGroup
                     select productGroup;

            foreach (IEnumerable<Product> productGroup in q2)
            {
                Console.WriteLine("Group");
                foreach (Product product in productGroup)
                {
                    Console.WriteLine($"{product.Name,9}");
                }
            }
        }

        public void GroupingData()
        {
            List<int> numbers = new List<int>() { 35, 44, 200, 84, 3987, 4, 199, 329, 446, 208 };

            IEnumerable<IGrouping<int, int>> query = from number in numbers
                                                     group number by number % 2;

            foreach (var group in query)
            {
                Console.WriteLine(group.Key == 0 ? "\nEven numbers:" : "\nOdd numbers");
                foreach (var item in group)
                {
                    Console.WriteLine(item);
                }
            }
        }

        class Plant
        {
            public string Name { get; set; }
        }

        class CarnivorousPlant : Plant
        {
            public string TrapType { get; set; }
        }

        public void ConvertingDataTypes()
        {
            Plant[] plants = new Plant[]
            {
                new CarnivorousPlant { Name = "Venus Fly Trap", TrapType = "Snap Trap" },
                new CarnivorousPlant { Name = "Pitcher Plant", TrapType = "Pitfall Trap" },
                new CarnivorousPlant { Name = "Sundew", TrapType = "Flypaper Trap" },
                new CarnivorousPlant { Name = "Waterwheel Plant", TrapType = "Snap Trap" }
            };

            var q1 = from CarnivorousPlant cPlant in plants
                     where cPlant.TrapType == "Snap Trap"
                     select cPlant;

            foreach (Plant plant in q1)
            {
                Console.WriteLine(plant.Name);
            }
        }

        public void ReflectionHowTo()
        {
            Assembly assembly = Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken= b77a5c561934e089");
            var pubTypesQuery = from type in assembly.GetTypes()
                                where type.IsPublic
                                from method in type.GetMethods()
                                where method.ReturnType.IsArray == true
                                || (method.ReturnType.GetInterface(
                                    typeof(System.Collections.Generic.IEnumerable<>).FullName) != null
                                    && method.ReturnType.FullName != "System.String")
                                group method.ToString() by type.ToString();

            foreach (var groupOfMethods in pubTypesQuery)
            {
                Console.WriteLine("Type: {0}", groupOfMethods.Key);
                foreach (var method in groupOfMethods)
                {
                    Console.WriteLine("  {0}", method);
                }
            }
        }

        public void GroupDemo()
        {
            //string[] source = text.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

            string[] websites1 = { "NmaeA.com NmaeB.am NmaeG.ru NmaeD.em" +
                    " NmaeA.bz NmaeE.it NmaeZ.az NmaeE.org NmaeT.tv" +
                    " NmaeA.com NmaeB.com NmaeG.com NmaeD.com" +
                    " NmaeA.com NmaeE.it NmaeZ.az NmaeE.org NmaeT.tv"};
            //string[] source = websites1.Split(new char[] {' '});

            var q1 = from web in websites1
                     from word in web.Split(' ')
                     group word by word.Substring(word.LastIndexOf("."));

            var q5 = websites1
                .OrderBy(x => x.Split(' '));
            //.GroupBy(x => x.Substring(x.LastIndexOf(".")));

            foreach (var item in q5)
            {
                Console.WriteLine(item);
            }

            var q3 = from web in websites1
                     from word in web.Split(' ')
                     group word by word.Substring(word.LastIndexOf("."))
                     into w
                     where w.Count() > 1
                     select w;

            foreach (var items in q3)
            {
                //Console.WriteLine($"Web sites grouped by {items.Key}");
                foreach (var item in items)
                {
                    //Console.WriteLine($"{item}");
                }
            }

            string[] websites2 = { "NmaeA.com, NmaeB.com, NmaeG.com, NmaeD.com," +
                    " NmaeA.com, NmaeE.it, NmaeZ.az, NmaeE.org, NmaeT.tv"};

            var q2 = from web in websites2
                     from word in web.Split(", ")
                     group word by word.Substring(word.LastIndexOf("."));
            //select word;



            foreach (var item in q2)
            {
                //Console.WriteLine(item.Key);
            }
            string[] websites = { "NmaeA.com", "NmaeB.am", "NmaeG.cm", "NmaeD.com", "NmaeA.com",
                "NmaeE.com", "NmaeZ.tv", "NmaeE.com", "NmaeT.com" };

            var q = from website in websites
                    group website by website.Substring(website.LastIndexOf('.'));

            var q4 = websites
                .GroupBy(x => x.Substring(x.LastIndexOf(".")));

            foreach (var item in q4)
            {
                Console.WriteLine(item.Key);
            }

        }
    }
    public class Lambda
    {
        class Student1
        {
            public int StudentID { get; set; }
            public string StudentName { get; set; }
            public int Age { get; set; }
        }
        public void SortingOperators()
        {
            IList<Student1> studentList = new List<Student1>()
            {
                new Student1() { StudentID = 1, StudentName = "John", Age = 18 } ,
                new Student1() { StudentID = 2, StudentName = "Steve",  Age = 15 } ,
                new Student1() { StudentID = 3, StudentName = "Bill",  Age = 25 } ,
                new Student1() { StudentID = 4, StudentName = "Ram" , Age = 20 } ,
                new Student1() { StudentID = 5, StudentName = "Ron" , Age = 19 },
                new Student1() { StudentID = 6, StudentName = "Ram" , Age = 18 }
            };

            IList mixedList = new ArrayList();
            mixedList.Add(0);
            mixedList.Add("One");
            mixedList.Add("Two");
            mixedList.Add(3);
            mixedList.Add(new Student1() { StudentID = 1, StudentName = "Bill" });

            var thenBy = studentList.OrderBy(s => s.StudentName).ThenBy(s => s.Age);
            var q1 = studentList.OrderBy(s => s.StudentName);
            var q2 = studentList.OrderByDescending(s => s.StudentName);
            var q3 = studentList.OrderBy(s => s.StudentName).ThenBy(s => s.Age);
            var q4 = studentList.OrderBy(s => s.StudentName).ThenByDescending(s => s.Age);
            var q5 = studentList.GroupBy(s => s.Age);
            var q6 = studentList.ToLookup(s => s.Age);
            var q7 = studentList.OrderBy(s => s.StudentName).Reverse();



            foreach (var item in q7)
            {
                Console.WriteLine($"StudentName: {item.StudentName}, Age: {item.Age}");
            }

            foreach (var item in q6)
            {
                Console.WriteLine("Age Group: {0}", item.Key); //Each group has a key 

                foreach (Student1 s in item) // Each group has inner collection
                    Console.WriteLine("Student Name: {0}", s.StudentName);
            }

            var query = mixedList.OfType<String>();

            foreach (var item in query)
            {
                Console.WriteLine(item);
            }
        }

        public class Student : IComparable<Student>
        {
            public int StudentID { get; set; }
            public string StudentName { get; set; }
            public int Age { get; set; }
            public int StandardID { get; set; }

            public int CompareTo(Student other)
            {
                //if (this.StudentName.Length >= other.StudentName.Length)
                //    return 1;
                //return 0;

                //if (this.StudentID >= other.StudentID)
                //    return 1;
                //return 0;

                if (this.StandardID >= other.StandardID)
                    return 1;
                return 0;

            }
        }
        public class StudentComparer : IEqualityComparer<Student>
        {
            public bool Equals(Student x, Student y)
            {
                if (x.StudentID == y.StudentID &&
                         x.StudentName.ToLower() == y.StudentName.ToLower())
                    return true;
                return false;
            }

            public int GetHashCode([DisallowNull] Student obj)
            {
                return obj.GetHashCode();
            }
        }

        public class Standard
        {
            public int StandardID { get; set; }
            public string StandardName { get; set; }
        }




        public void JGJ()
        {
            IList<Student> studentList = new List<Student>()
            {
                    new Student() { StudentID = 1, StudentName = "John", Age = 13, StandardID =1 },
                    new Student() { StudentID = 2, StudentName = "Moin", Age = 21, StandardID =1 },
                    new Student() { StudentID = 3, StudentName = "Bill", Age = 18, StandardID =2 },
                    new Student() { StudentID = 4, StudentName = "Ram", Age = 20, StandardID =2 },
                    new Student() { StudentID = 5, StudentName = "Ron", Age = 15 }
            };

            IList<Standard> standardList = new List<Standard>()
            {
                    new Standard(){ StandardID = 1, StandardName="Standard 1"},
                    new Standard(){ StandardID = 2, StandardName="Standard 2"},
                    new Standard(){ StandardID = 3, StandardName="Standard 3"}
            };


            var innerJoinResult = studentList.Join(// outer sequence 
                  standardList,  // inner sequence 
                  student => student.StandardID,    // outerKeySelector
                  standard => standard.StandardID,  // innerKeySelector
                  (student, standard) => new  // result selector
                  {
                      StudentName = student.StudentName,
                      StandardName = standard.StandardName
                  });

            //foreach (var item in innerJoinResult)
            //{
            //    Console.WriteLine($"{item.StudentName}-{item.StandardName}");
            //}

            Console.WriteLine();

            var groupJoin = standardList.GroupJoin(
                studentList,
                std => std.StandardID,
                s => s.StandardID,
                (std, studentGroup) => new
                {
                    Students = studentGroup,
                    StandarFulldName = std.StandardName
                });

            //foreach (var item in groupJoin)
            //{
            //    Console.WriteLine(item.StandarFulldName);
            //    foreach (var stud in item.Students)
            //        Console.WriteLine(stud.StudentName);
            //}

            var selectResult = studentList.Select(s => new
            {
                Name = s.StudentName,
                Age = s.StudentID
            });

            //foreach (var item in selectResult)
            //    Console.WriteLine($"Student Name: {item.Name}, ID: {item.Age}" );

            bool areAllStudentsTeenAger = studentList.All(s => s.StudentID > 1 && s.StudentID < 4);
            //Console.WriteLine(areAllStudentsTeenAger);

            bool isAnyStudentTeenAger = studentList.Any(s => s.StudentID > 1 && s.StudentID < 4);
            //Console.WriteLine(isAnyStudentTeenAger);

            IList<int> intList = new List<int>() { 1, 2, 3, 4, 5 };
            bool result = intList.Contains(10);
            //Console.WriteLine(result);

            Student student = new Student() { StudentID = 4, StudentName = "Ram", StandardID = 2 };
            bool res = studentList.Contains(student, new StudentComparer());
            //Console.WriteLine(res);

            string commaSeparatedStudentNames = studentList.Aggregate<Student, string>(
                "Student name: ",
                (s1, s2) => s1 += s2.StudentName + ", ");
            Console.WriteLine(commaSeparatedStudentNames);

            int sumOfStudentsAge = studentList.Aggregate<Student, int>(
                0,
                (i1, i2) => i1 += i2.Age);
            Console.WriteLine(sumOfStudentsAge);

            string commaSeparatedStudentNames2 = studentList.Aggregate<Student, string, string>(
                "Student name: ",
                (str, s) => str += s.StudentName + ", ",
                str => str.Substring(0, str.Length - 2));
            Console.WriteLine(commaSeparatedStudentNames2);

            var avgAge = studentList.Average(s => s.Age);
            Console.WriteLine($"Average Age of Student: {avgAge}");

            var totalStudents = studentList.Count();
            Console.WriteLine($"Total Students: {totalStudents}");
            var longCount = studentList.LongCount();
            Console.WriteLine(longCount);


            var adultStudents = studentList.Count(s => s.Age >= 18);
            Console.WriteLine($"Number of Adult Students: {adultStudents}");

            var studentWithLongName = studentList.Max();
            Console.WriteLine($"Student ID: {studentWithLongName.StudentID}, Student Name: {studentWithLongName.StudentName}");

            var min = studentList.Min(s => s.Age);
            Console.WriteLine($"Min : {min}");

            var sum = studentList.Sum(s => s.StudentName.Length);
            Console.WriteLine($"Sum age {sum}");

            var oldest = studentList.Max(s => s.Age);
            Console.WriteLine($"Oldest Student Age: {oldest}");

            var elementAt = studentList.ElementAt(3);
            Console.WriteLine($"Element at 0. Name : {elementAt.StudentName}, Age : {elementAt.Age}");

            var elementAtOrDefault = studentList.ElementAtOrDefault(4);
            Console.WriteLine($"Element At Or Default {elementAtOrDefault.StudentName}");

            var first = studentList.First();
            Console.WriteLine($"First name: {first.StudentName}");

            var first2 = studentList.First(s => s.StudentName == "Ron");
            Console.WriteLine($"First name: {first2.StudentName}");

            var firstOrDefault = studentList.FirstOrDefault(s => s.Age >= 18);
            Console.WriteLine($"First name: {firstOrDefault.StudentName}");

            var firstOrDefault2 = studentList.FirstOrDefault(s => s.StudentName.Contains("R"));
            Console.WriteLine($"First name: {firstOrDefault2.StudentName}");

            var last = studentList.Last();
            Console.WriteLine($"Last name {last.StudentName}");

            var last2 = studentList.Last(s => s.Age >= 18);
            Console.WriteLine($"Last name {last2.StudentName}");

            var lastOrDefault = studentList.LastOrDefault(s => s.StudentName.Contains("R"));
            Console.WriteLine($"Last name: {lastOrDefault.StudentName}");

            //var single = studentList.Single();
            //Console.WriteLine(single.StudentName);

            var single1 = studentList.Single(s => s.Age == 18);
            Console.WriteLine($"Single : {single1.StudentName}");

            var singleOrDefault = studentList.SingleOrDefault(s => s.StudentID == 2);
            Console.WriteLine($"Single Or Default : {singleOrDefault.StudentName}");

            Student std = new Student() { StudentID = 1, StudentName = "Bill" };
            IList<Student> studentList1 = new List<Student>() { std };
            IList<Student> studentList2 = new List<Student>() { std };
            bool isEqual = studentList1.SequenceEqual(studentList2); // returns true
            Console.WriteLine($"Sequence Equal : {isEqual}");

            Student std1 = new Student() { StudentID = 1, StudentName = "Bill" };
            Student std2 = new Student() { StudentID = 1, StudentName = "Bill" };
            IList<Student> studentList3 = new List<Student>() { std1 };
            IList<Student> studentList4 = new List<Student>() { std2 };
            isEqual = studentList3.SequenceEqual(studentList4);
            Console.WriteLine($"Sequence Equal : {isEqual}");
            isEqual = studentList3.SequenceEqual(studentList4, new StudentComparer());
            Console.WriteLine($"Sequence Equal : {isEqual}");

            IList<string> collection1 = new List<string>() { "One", "Two", "Three" };
            IList<string> collection2 = new List<string>() { "Five", "Six" };

            var collection3 = collection1.Concat(collection2);

            foreach (string str in collection3)
                Console.WriteLine(str);


            IList<Student> emptyStudentList = new List<Student>();

            var newStudentList1 = emptyStudentList.DefaultIfEmpty();
            Console.WriteLine($"Count: {emptyStudentList.Count()} ");
            Console.WriteLine($"Student ID: {newStudentList1.ElementAt(0)} ");

            var newStudentList2 = studentList.DefaultIfEmpty(new Student()
            {
                StudentID = 0,
                StudentName = ""
            });
            Console.WriteLine($"Count: {newStudentList2.Count()}");
            Console.WriteLine($"Student ID: {newStudentList2.ElementAt(0).StudentID}");

            Console.WriteLine();

            var emptyCollection1 = Enumerable.Empty<string>();
            var emptyCollection2 = Enumerable.Empty<Student>();

            Console.WriteLine($"Count : {emptyCollection1.Count()}");
            Console.WriteLine($"Type : {emptyCollection1.GetType().Name}");

            Console.WriteLine($"Count : {emptyCollection2.Count()}");
            Console.WriteLine($"Type : {emptyCollection2.GetType().Name}");

            var intCollectionRange = Enumerable.Range(10, 11);
            Console.WriteLine($"Total Count : {intCollectionRange.Count()}");

            for (int i = 0; i < intCollectionRange.Count(); i++)
                Console.WriteLine($"Value at index {i} : {intCollectionRange.ElementAt(i)}");

            Console.WriteLine();

            var intCollectionRepeat = Enumerable.Repeat<int>(10, 10);
            Console.WriteLine("Total Count: {0} ", intCollectionRepeat.Count());

            for (int i = 0; i < intCollectionRepeat.Count(); i++)
                Console.WriteLine("Value at index {0} : {1}", i, intCollectionRepeat.ElementAt(i));


            //The Except() method requires two collections. It returns
            //a new collection with elements from the first collection
            //which do not exist in the second collection(parameter collection). 
            IList<string> strList1 = new List<string>() { "One", "Two", "Three", "Four", "Five" };
            IList<string> strList2 = new List<string>() { "Four", "Five", "Six", "Seven", "Eight" };

            var resultExcept = strList1.Except(strList2);

            foreach (string str in resultExcept)
                Console.WriteLine(str);
        }
    }

    class Lambda2
    {
        public class Student2
        {
            public int StudentID { get; set; }
            public string StudentName { get; set; }
            public int Age { get; set; }
        }

        class StudentComparer2 : IEqualityComparer<Student2>
        {
            public bool Equals(Student2 x, Student2 y)
            {
                if (x.StudentID == y.StudentID && x.StudentName.ToLower() == y.StudentName.ToLower())

                    return true;

                return false;
            }

            public int GetHashCode(Student2 obj)
            {
                return obj.StudentID.GetHashCode();
            }
        }
        public void Met()
        {
            IList<Student2> studentList1 = new List<Student2>()
            {
                new Student2() { StudentID = 1, StudentName = "Steve", Age = 18 } ,
                new Student2() { StudentID = 2, StudentName = "John",  Age = 15 } ,
                new Student2() { StudentID = 3, StudentName = "Bill",  Age = 25 } ,
                new Student2() { StudentID = 5, StudentName = "Ron" , Age = 19 }
            };

            IList<Student2> studentList2 = new List<Student2>()
            {
                new Student2() { StudentID = 3, StudentName = "Bill",  Age = 25 } ,
                new Student2() { StudentID = 5, StudentName = "Ron" , Age = 19 } ,
                new Student2() { StudentID = 6, StudentName = "Ram" , Age = 29 }
            };

            var resultedCol = studentList1.Except(studentList2, new StudentComparer2());
            //foreach (Student2 std in resultedCol)
                //Console.WriteLine(std.StudentName);

            //Console.WriteLine();

            var resultIntersect = studentList1.Intersect(studentList2, new StudentComparer2());
            //foreach (var item in resultIntersect)
                //Console.WriteLine(item.StudentName);

            //Console.WriteLine();

            var resultUnion = studentList1.Union(studentList2, new StudentComparer2());
            //foreach (var item in resultUnion)
                //Console.WriteLine(item.StudentName);

            //Console.WriteLine();

            var resultSkip = resultUnion.Skip(2);
            //foreach (var item in resultSkip)
            //    Console.WriteLine(item.StudentName);
            
            //Console.WriteLine();

            var resultSkipLast = studentList1.SkipLast(2);
            //foreach (var item in resultSkipLast)
            //    Console.WriteLine(item.StudentName);

            //Console.WriteLine();
            
            //բաց է թողնում պայմանին բավարարող բոլոր դեպքերը, 
            //բավարարելու դեպքում անտեսում պայմանը տպելով բոլոր մնացած դեպքերը
            var resultSkipWhile = studentList1.SkipWhile(s => s.StudentName.Length == 4);
            //foreach (var item in resultSkipWhile)
            //    Console.WriteLine(item.StudentName);

            //Console.WriteLine();

            var resultTake = studentList1.Take(4);
            foreach (var item in resultTake)
                Console.WriteLine(item.StudentName);

            Console.WriteLine();

            //վերցնում է պայմանին բավարարող բոլոր դեպքերը, 
            //չբավարարելու դեպքում անտեսում բոլոր մնացած դեպքերը
            var resultTakeWhile = studentList1.TakeWhile(s => s.StudentName.Length > 3);
            foreach (var item in resultTakeWhile)
                Console.WriteLine(item.StudentName);

           
        }

        public void Met2()
        {
            Student2[] studentArray = 
            {
                new Student2() { StudentID = 1, StudentName = "John", Age = 18 } ,
                new Student2() { StudentID = 2, StudentName = "Steve",  Age = 21 } ,
                new Student2() { StudentID = 3, StudentName = "Bill",  Age = 25 } ,
                new Student2() { StudentID = 4, StudentName = "Ram" , Age = 20 } ,
                new Student2() { StudentID = 5, StudentName = "Ron" , Age = 31 } ,
            };

            ReportTypeProperties(studentArray);

            //ReportTypeProperties(studentArray.AsEnumerable());
            //ReportTypeProperties(studentArray.AsQueryable());

            //ReportTypeProperties(studentArray.Cast<Student2>());
            //ReportTypeProperties(studentArray.OfType<string>());

            ReportTypeProperties(studentArray.ToArray<Student2>());
            ReportTypeProperties(studentArray.ToList<Student2>());
            ReportTypeProperties(studentArray.ToLookup(s => s.Age));

            //ReportTypeProperties(studentArray.AsParallel());
            //ReportTypeProperties(studentArray.AsMemory());

            IDictionary<int, Student2> dictionary =
                studentArray.ToDictionary<Student2, int>(s => s.StudentID);

            foreach (var item in dictionary.Keys)
                Console.WriteLine($"Key: {item}, Value: {(dictionary[item] as Student2).StudentName}");
            
        }

        private void ReportTypeProperties<T>(T obj)
        {
            Console.WriteLine($"Compile-time type: {typeof(T).Name}");
            Console.WriteLine($"Actual type: {obj.GetType().Name}");
            Console.WriteLine();
        }
    }
    class ExpressioninLINQ
    {
        public class Student
        {
            public int StudentID { get; set; }
            public string StudentName { get; set; }
            public int Age { get; set; }
        }
        public void Met()
        {
            Expression<Func<Student, bool>> TeenAgerExpr = s => s.Age > 12 && s.Age < 20;

            Func<Student, bool> isTeenAger = TeenAgerExpr.Compile();

            bool result = isTeenAger(new Student() { StudentID = 1, StudentName = "Steve", Age = 20 });
            Console.WriteLine(result);
        }

        public void ExpressionTree()
        {
            //ParameterExpression parameterExpression = Expression.Parameter(typeof(Student), "s");

            //MemberExpression memberExpression = Expression.Property(parameterExpression, "Age");

            //ConstantExpression constant = Expression.Constant(18, typeof(int));

            //BinaryExpression body = Expression.GreaterThanOrEqual(memberExpression, constant);

            //var ExpressionTree = Expression.Lambda<Func<Student, bool>>(body, new[] { parameterExpression });

            //Console.WriteLine($"Expression Tree: {ExpressionTree}");

            //Console.WriteLine($"Expression Tree Body: {ExpressionTree.Body}");

            //Console.WriteLine($"Number of Parameters in Expression Tree: {ExpressionTree.Parameters.Count}");

            //Console.WriteLine($"Parameters in Expression Tree: {ExpressionTree.Parameters[0]}");

            Console.WriteLine();

            Expression<Func<Student, bool>> isTeenAgerExpr = s => s.Age > 12 && s.Age < 20;
            
            Console.WriteLine($"Expression: {isTeenAgerExpr}");
            Console.WriteLine($"Expression Type: {isTeenAgerExpr.NodeType}");

            var parameters = isTeenAgerExpr.Parameters;

            foreach (var item in parameters)
            {
                Console.WriteLine($"Parameter Name: {item.Name}");
                Console.WriteLine($"Parameter Type: {item.Type.Name}");
            }

            var bodyExpr = isTeenAgerExpr.Body as BinaryExpression;

            Console.WriteLine($"Left side body expression: {bodyExpr.Left}");
            Console.WriteLine($"Binary Expression Type: {bodyExpr.NodeType}");
            Console.WriteLine($"Right side body expression: {bodyExpr.Right}");
            Console.WriteLine($"Return Type: {isTeenAgerExpr.ReturnType}");

        }

        public void AsParall()
        {
            int[] data = new int[100000000];
            for (int i = 0; i < data.Length; i++)
                data[i] = i;
            data[1000] = -1;
            data[14000] = -2;
            data[15000] = -3;
            data[676000] = -4;
            data[8024540] = -5;
            data[9906000] = -6;
            data[99080000] = -7;

            Stopwatch sw1 = Stopwatch.StartNew();
            var qp = data.Where(s => s < 0)
                         .AsParallel();

            foreach (var item in qp)
                Console.WriteLine(item + " ");
            Console.WriteLine(sw1.Elapsed);

            Console.WriteLine();///////////////////////

            Stopwatch sw2 = Stopwatch.StartNew();
            var qp2 = data.Where(s => s < 0)
                          .AsParallel()
                          .AsOrdered();

            foreach (var item in qp2)
                Console.WriteLine(item + " ");
            Console.WriteLine(sw2.Elapsed);

            Console.WriteLine();///////////////////////

            Stopwatch sw3 = Stopwatch.StartNew();
            var qp3 = data.Where(s => s < 0)
                          .AsParallel()
                          .WithDegreeOfParallelism(6);
            foreach (var item in qp3)
                Console.WriteLine(item + " ");
            Console.WriteLine(sw3.Elapsed);

            Console.WriteLine();//////////////////////

            CancellationTokenSource source = new CancellationTokenSource();
            var qp4 = data.Where(s => s < 0)
                          .AsParallel()
                          .WithCancellation(source.Token);

            Task task = Task.Factory.StartNew(() =>
            {
                source.Cancel();
            });

            try
            {
                foreach (var item in qp4)
                    Console.WriteLine(item + " ");             
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            task.Wait();
            task.Dispose();
            source.Dispose();
                
            
        }
    }

    

        class Program
    {
        static void Main(string[] args)
        {
            new ExpressioninLINQ().AsParall();
        }
    }
}

