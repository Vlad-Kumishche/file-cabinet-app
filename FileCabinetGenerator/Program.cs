using System;
using System.Collections.Generic;

namespace FileCabinetGenerator
{
    class Program
    {
        private static string OutputType = "csv";
        private static string OutputFileName = "records." + OutputType;
        private static int RecordsAmount = 250;
        private static int StartId = 15;

        private static readonly Dictionary<string, SetRule> ChangeSettings = new Dictionary<string, SetRule>
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

        static void Main(string[] args)
        {
            ParseCommandLineParams(args);

            Console.WriteLine($"{nameof(OutputType)} = {OutputType}");
            Console.WriteLine($"{nameof(OutputFileName)} = {OutputFileName}");
            Console.WriteLine($"{nameof(RecordsAmount)} = {RecordsAmount}");
            Console.WriteLine($"{nameof(StartId)} = {StartId}");
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
                    if (ChangeSettings.ContainsKey(operation.ToLower()))
                    {
                        changeRule = ChangeSettings[operation.ToLower()];
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

                if (ChangeSettings.ContainsKey(operation.ToLower()))
                {
                    changeRule = ChangeSettings[operation.ToLower()];
                    parsedSetting = 2;
                }
            }

            if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(parameter) && changeRule != null)
            {
                changeRule.Invoke(parameter.ToLower());
            }

            return parsedSetting;
        }

        private static void SetOutputType(string outputType)
        {
            if (outputType == "csv" || outputType == "xml")
            {
                OutputType = outputType;
            }
            else
            {
                Console.WriteLine("Invalid output type. CSV or XML are allowed.");
            }
        }

        private static void SetOutput(string fileName)
        {
            if (OutputFileName == null)
            {
                Console.WriteLine("Set output type before output filename.");
                return;
            }

            if (fileName.EndsWith("." + OutputType))
            {
                OutputFileName = fileName;
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
                    RecordsAmount = amount;
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
                    StartId = amount;
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