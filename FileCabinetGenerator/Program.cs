using System.Globalization;
using System.Text;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Class provides user interaction.
    /// </summary>
    public static class Program
    {
        private static string outputType = "csv";
        private static string outputFileName = "records." + outputType;
        private static int recordsAmount = 250;
        private static int startId = 15;

        private static IRecordGenerator generator = new DefaultRecordGenerator();

        private static Dictionary<string, SetRule> changeSettings = new Dictionary<string, SetRule>
        {
            ["--output-type"] = new SetRule(SetOutputType),
            ["-t"] = new SetRule(SetOutputType),
            ["--output"] = new SetRule(SetOutput),
            ["-o"] = new SetRule(SetOutput),
            ["--records-amount"] = new SetRule(SetRecordsAmount),
            ["-a"] = new SetRule(SetRecordsAmount),
            ["--start-id"] = new SetRule(SetStartId),
            ["-i"] = new SetRule(SetStartId),
        };

        private delegate void SetRule(string args);

        /// <summary>
        /// Processes user input and calls the appropriate functions.
        /// </summary>
        /// <param name="args">Сommand line arguments.</param>
        public static void Main(string[] args)
        {
            ParseCommandLineParams(args);

            Console.WriteLine($"{nameof(outputType)} = {outputType}");
            Console.WriteLine($"{nameof(outputFileName)} = {outputFileName}");
            Console.WriteLine($"{nameof(recordsAmount)} = {recordsAmount}");
            Console.WriteLine($"{nameof(startId)} = {startId}");

            Export();
        }

        private static void Export()
        {
            var file = new FileInfo(outputFileName);
            try
            {
                string messageToUser;
                var snapshot = new FileCabinetServiceSnapshot(generator.GenerateRecords(startId, recordsAmount));
                switch (outputType)
                {
                    case "csv":
                        var csvWriter = new StreamWriter(outputFileName, false, Encoding.UTF8);
                        snapshot.SaveToCsv(csvWriter);
                        csvWriter.Close();
                        messageToUser = $"All records are exported to file {outputFileName}";
                        break;

                    case "xml":
                        /*XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.IndentChars = "\t";
                        settings.OmitXmlDeclaration = true;

                        var xmlWriter = XmlWriter.Create(outputFileName, settings);
                        snapshot.SaveToXml(xmlWriter);
                        xmlWriter.Close();
                        messageToUser = $"All records are exported to file {outputFileName}";
                        break;*/

                    default:
                        messageToUser = $"<param1> - unsuppurted file format.";
                        break;
                }

                Console.WriteLine(messageToUser);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"Export failed: can't open file {outputFileName}");
            }
        }

        private static void ParseCommandLineParams(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                string[] currentArgs = new string[2];

                for (int i = 1; i <= args.Length;)
                {
                    currentArgs[0] = args[i - 1];
                    if (i != args.Length)
                    {
                        currentArgs[1] = args[i];
                    }

                    int numberOfParsedParameters = ParseParams(currentArgs);
                    if (numberOfParsedParameters == 0)
                    {
                        break;
                    }

                    i += numberOfParsedParameters;
                }
            }
        }

        private static int ParseParams(string[] args)
        {
            string operation = string.Empty;
            string parameter = string.Empty;
            SetRule? changeRule = null;
            int parsedSetting = 0;

            if (args[0].StartsWith("--", StringComparison.InvariantCulture))
            {
                int index = args[0].IndexOf("=", StringComparison.InvariantCulture);
                if (index != -1)
                {
                    operation = args[0].Substring(0, index);
                    parameter = args[0].Substring(index + 1);
                    if (changeSettings.ContainsKey(operation.ToLower(CultureInfo.InvariantCulture)))
                    {
                        changeRule = changeSettings[operation.ToLower(CultureInfo.InvariantCulture)];
                        parsedSetting = 1;
                    }
                }
            }
            else if (args[0].StartsWith("-", StringComparison.InvariantCulture))
            {
                operation = args[0];
                if (args[1] != null)
                {
                    parameter = args[1];
                }

                if (changeSettings.ContainsKey(operation.ToLower(CultureInfo.InvariantCulture)))
                {
                    changeRule = changeSettings[operation.ToLower(CultureInfo.InvariantCulture)];
                    parsedSetting = 2;
                }
            }

            if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(parameter) && changeRule != null)
            {
                changeRule.Invoke(parameter.ToLower(CultureInfo.InvariantCulture));
            }

            return parsedSetting;
        }

        private static void SetOutputType(string outputType)
        {
            if (outputType == "csv" || outputType == "xml")
            {
                Program.outputType = outputType;
            }
            else
            {
                Console.WriteLine("Invalid output type. CSV or XML are allowed.");
            }
        }

        private static void SetOutput(string fileName)
        {
            if (outputFileName == null)
            {
                Console.WriteLine("Set output type before output filename.");
                return;
            }

            if (fileName.EndsWith("." + outputType, StringComparison.InvariantCultureIgnoreCase))
            {
                outputFileName = fileName;
            }
            else
            {
                Console.WriteLine("Invalid file name.");
            }
        }

        private static void SetRecordsAmount(string recordsAmount)
        {
            if (int.TryParse(recordsAmount, out int amount))
            {
                if (amount > 0)
                {
                    Program.recordsAmount = amount;
                }
                else
                {
                    Console.WriteLine("Invalid amount of records. Should be more than 0.");
                }
            }
            else
            {
                Console.WriteLine("Invalid amount of records.");
            }
        }

        private static void SetStartId(string startId)
        {
            if (int.TryParse(startId, out int amount))
            {
                if (amount >= 0)
                {
                    Program.startId = amount;
                }
                else
                {
                    Console.WriteLine("Invalid start id. Should be non-negative.");
                }
            }
            else
            {
                Console.WriteLine("Invalid start id.");
            }
        }
    }
}