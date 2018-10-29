using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace replace_in_file
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var replacements = ExtractReplacements(args);

                foreach (var item in replacements)
                {
                    Populate(item);
                }
            }
            catch (Exception ex)
            {
                Write(ex.Message, ConsoleColor.Red);
                ShowHelp();
                return 1;
            }

            return 0;
        }

        static IEnumerable<Replacement> ExtractReplacements(string[] args)
        {
            if (args[0] == "-m")
                return ExtractMappedReplacements(args[1]);

            return new Replacement[1] { ExtractReplacementFromArgs(args) };
        }

        private static Replacement ExtractReplacementFromArgs(string[] args)
        {
            if (args.Length < 4 || (args.Length - 1) % 3 != 0)
            {
                throw new Exception("Invalid arguments!");
            }

            var filePath = args[0];
            if (!File.Exists(filePath))
            {
                throw new Exception($"The file at path {filePath} does not exist");
            }

            var items = new ProcessingSet[(args.Length - 1) / 3];
            var counter = 0;
            for (var i = 1; i < args.Length - 2; i += 3)
            {
                var separator = args[i];
                if (separator != "-set")
                {
                    throw new Exception("You should separate your placeholder and value pairs using -set\n type replace-in-file /? for the syntax.");
                }

                var placeHolder = args[i + 1];
                var vallue = args[i + 2];
                var item = new ProcessingSet { PlaceHolder = placeHolder, Value = vallue };
                items[counter] = item;
                counter++;
            }

            return new Replacement { File = filePath, Sets = items };
        }
        private static IEnumerable<Replacement> ExtractMappedReplacements(string mappingfilePath)
        {
            if (string.IsNullOrEmpty(mappingfilePath))
                mappingfilePath = ".\\replace-in-file.yaml";

            if (!File.Exists(mappingfilePath))
            {
                throw new Exception($"The file at path {mappingfilePath} does not exist");
            }

            using (var file = File.OpenText(mappingfilePath))
            {
                var infoPairs = new Deserializer().Deserialize<Dictionary<string, Dictionary<string, string>>>(file);

                return from item in infoPairs
                       let fileName = item.Key
                       let sets = from pair in item.Value
                                  select new ProcessingSet { PlaceHolder = pair.Key, Value = pair.Value }
                       select new Replacement { File = fileName, Sets = sets.ToArray() };
            }
        }

        static void Populate(Replacement info)
        {
            var content = File.ReadAllText(info.File);
            var sb = new StringBuilder(content);
            foreach (var pair in info.Sets)
            {
                sb = sb.Replace(pair.PlaceHolder, pair.Value);
            }
            File.WriteAllText(info.File, sb.ToString());
        }

        static void ShowHelp()
        {
            WriteLine("The correct syntax is:\n", ConsoleColor.Red);
            Write("replace-in-file ");
            Write("\"file-path\" ", ConsoleColor.White);
            Write("-set ");
            Write("placeholer1 value1 ", ConsoleColor.Cyan);
            Write("-set ");
            Write("\"placeholer with space\" \"value with space\" ", ConsoleColor.Cyan);
            Write("-set ");
            Write("...", ConsoleColor.Cyan);
            WriteLine("Or", ConsoleColor.Red);
            Write("-m ");
            Write("the path to the yaml file containing the mappings as below:", ConsoleColor.Cyan);
            WriteLine("\"filepath\"");
            WriteLine("  \"KEY1\" : \"VALUE1\" ");
            WriteLine("  \"KEY2\" : \"VALUE2\" ");
        }

        static void Write(string text, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
        static void WriteLine(string text, ConsoleColor color = ConsoleColor.Yellow) =>
            Write(Environment.NewLine + text, color);
    }

    struct Replacement
    {
        public string File { get; set; }
        public ProcessingSet[] Sets { get; set; }
    }

    struct ProcessingSet
    {

        public string PlaceHolder { get; set; }
        public string Value { get; set; }
    }
}