using System;
using System.IO;
using System.Text;

namespace replace_in_file
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 4 || (args.Length - 1) % 3 != 0)
            {
                ShowHelp();
                return 1;
            }

            var filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"The file at path {filePath} does not exist"); ;
                return 1;
            }

            var content = File.ReadAllText(filePath);
            var sb = new StringBuilder(content);
            var items = new ProcessingSet[(args.Length - 1) / 3];
            var counter = 0;
            for (int i = 1; i < args.Length - 2; i += 3)
            {
                var separator = args[i];
                if (separator != "-set")
                {
                    Console.WriteLine("You should separate your placeholder and value pairs using -set\n type replace-in-file /? for the syntax.");
                    return 1;
                }

                var placeHolder = args[i + 1];
                var vallue = args[i + 2];
                var item = new ProcessingSet { PlaceHolder = placeHolder, Value = vallue };
                items[counter] = item;
                counter++;
            }

            for (int i = 0; i < items.Length; ++i)
                sb = sb.Replace(items[i].PlaceHolder, items[i].Value);

            File.WriteAllText(filePath, sb.ToString());
            return 0;
        }

        static void ShowHelp()
        {
            Write("The correct syntax is:\n", ConsoleColor.Red);
            Write("replace-in-file ");
            Write("\"file-path\" ", ConsoleColor.White);
            Write("-set ");
            Write("placeholer1 value1 ", ConsoleColor.Cyan);
            Write("-set ");
            Write("\"place holer with space\" \"value with space\" ", ConsoleColor.Cyan);
            Write("-set ");
            Write("...", ConsoleColor.Cyan);
        }

        static void Write(string text, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
    }

    struct ProcessingSet
    {

        public string PlaceHolder { get; set; }
        public string Value { get; set; }
    }
}