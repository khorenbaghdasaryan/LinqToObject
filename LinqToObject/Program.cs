using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
            string[] names1 = System.IO.File.ReadAllLines(@"C:/Users/khore/Desktop/names1.txt");
            string[] names2 = System.IO.File.ReadAllLines(@"C:/Users/khore/Desktop/names2.txt");

            IEnumerable<string> differenceQuery =
                names1.Except(names2);

            Console.Write("The following lines are in names1.txt but not names2.txt ");

            foreach (var item in differenceQuery)
            {
                Console.WriteLine(item);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            new Q().CompareLists();
        }
    }
}
