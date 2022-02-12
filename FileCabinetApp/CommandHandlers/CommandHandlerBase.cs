using System.Globalization;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// CommandHandlerBase.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// Select add sign.
        /// </summary>
        protected const string SelectAll = "*";

        private ICommandHandler? nextHandler;
        private string? commandName;

        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        /// <value>
        /// Command name.
        /// </value>
        protected string? CommandName { get => this.commandName; set => this.commandName = value; }

        /// <inheritdoc/>
        public void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (string.Equals(appCommandRequest.Command, this.CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.Command(appCommandRequest.Parameters);
            }
            else
            {
                if (this.nextHandler is not null)
                {
                    this.nextHandler.Handle(appCommandRequest);
                }
                else
                {
                    PrintMissedCommandInfo(appCommandRequest.Command);
                }
            }
        }

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
        }

        /// <summary>
        /// Parses sourceString into paramsArray.
        /// </summary>
        /// <param name="count">Number of parameters to parse.</param>
        /// <param name="sourceString">Source string.</param>
        /// <param name="parameterExplanations">Parameter explanations.</param>
        /// <param name="paramsArray">Array of params.</param>
        /// <returns>Is parsing was successful.</returns>
        protected static bool GetParameters(int count, string sourceString, string[] parameterExplanations, out string[] paramsArray)
        {
            paramsArray = sourceString.Split(' ', count, StringSplitOptions.RemoveEmptyEntries);
            foreach (var parameter in paramsArray)
            {
                if (parameter is null)
                {
                    Console.Write($"{count} parameters required.");
                    int i = 0;
                    foreach (var explanation in parameterExplanations)
                    {
                        Console.Write($" <param{++i}> - {explanation}.");
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reades user induput and returns converted and validated data.
        /// </summary>
        /// <typeparam name="T">Final data format.</typeparam>
        /// <param name="converter">The converter.</param>
        /// <param name="validator">The validator.</param>
        /// <returns>Converted and validated data.</returns>
        protected static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input ?? string.Empty);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        /// <summary>
        /// Converter for string type.
        /// </summary>
        /// <param name="stringToConvert">source string.</param>
        /// <returns>Result of conversion.</returns>
        protected static Tuple<bool, string, string> StringConverter(string stringToConvert)
        {
            return Tuple.Create(true, string.Empty, stringToConvert);
        }

        /// <summary>
        /// Converter for DateTime type.
        /// </summary>
        /// <param name="stringToConvert">source string.</param>
        /// <returns>Result of conversion.</returns>
        protected static Tuple<bool, string, DateTime> DateConverter(string stringToConvert)
        {
            const string requiredDateFormat = "MM/DD/YYYY";
            DateTime birthday = DateTime.MinValue;
            if (!string.IsNullOrEmpty(stringToConvert) && stringToConvert.Length == requiredDateFormat.Length)
            {
                string toParse = stringToConvert[3..6] + stringToConvert[..3] + stringToConvert[6..];
                if (DateTime.TryParse(toParse, out birthday))
                {
                    return Tuple.Create(true, string.Empty, birthday);
                }
            }

            return Tuple.Create(false, $"{stringToConvert} is not a date", birthday);
        }

        /// <summary>
        /// Converter for short type.
        /// </summary>
        /// <param name="stringToConvert">source string.</param>
        /// <returns>Result of conversion.</returns>
        protected static Tuple<bool, string, short> ShortConverter(string stringToConvert)
        {
            if (short.TryParse(stringToConvert, out var number))
            {
                return Tuple.Create(true, string.Empty, number);
            }

            return Tuple.Create(false, $"{stringToConvert} is not short number", number);
        }

        /// <summary>
        /// Converter for decimal type.
        /// </summary>
        /// <param name="stringToConvert">source string.</param>
        /// <returns>Result of conversion.</returns>
        protected static Tuple<bool, string, decimal> DecimalConverter(string stringToConvert)
        {
            if (decimal.TryParse(stringToConvert, out var number))
            {
                return Tuple.Create(true, string.Empty, number);
            }

            return Tuple.Create(false, $"{stringToConvert} is not decimal number", number);
        }

        /// <summary>
        /// Converter for char type.
        /// </summary>
        /// <param name="stringToConvert">source string.</param>
        /// <returns>Result of conversion.</returns>
        protected static Tuple<bool, string, char> CharConverter(string stringToConvert)
        {
            if (char.TryParse(stringToConvert, out var c))
            {
                return Tuple.Create(true, string.Empty, c);
            }

            return Tuple.Create(false, $"{stringToConvert} is not character", c);
        }

        /// <summary>
        /// Validator for name.
        /// </summary>
        /// <param name="nameToValidate">The name.</param>
        /// <returns>Result of validation.</returns>
        protected static Tuple<bool, string> NameValidator(string nameToValidate)
        {
            int minLength;
            int maxLength;
            if (Program.CurrentValidationRules == Program.CustomValidationRules)
            {
                minLength = 4;
                maxLength = 20;
            }
            else
            {
                minLength = 2;
                maxLength = 60;
            }

            if (string.IsNullOrEmpty(nameToValidate) || nameToValidate.Length < minLength || nameToValidate.Length > maxLength)
            {
                return Tuple.Create(false, $"Length of \"{nameToValidate}\" does not meet the requirements. Min. length = {minLength}, max. Length = {maxLength}");
            }

            foreach (char c in nameToValidate)
            {
                var validationResult = LetterValidator(c);
                if (!validationResult.Item1)
                {
                    return Tuple.Create(false, validationResult.Item2);
                }
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Validator for date.
        /// </summary>
        /// <param name="dateToValidate">The date.</param>
        /// <returns>Result of validation.</returns>
        protected static Tuple<bool, string> DateValidator(DateTime dateToValidate)
        {
            DateTime minDate;
            DateTime maxDate;
            if (Program.CurrentValidationRules == Program.CustomValidationRules)
            {
                minDate = new DateTime(1940, 1, 1);
                const int ageOfMajority = 18;
                maxDate = DateTime.Now.AddYears(-ageOfMajority);
            }
            else
            {
                minDate = new DateTime(1950, 1, 1);
                maxDate = DateTime.Now;
            }

            if (dateToValidate < minDate || dateToValidate >= maxDate)
            {
                return Tuple.Create(false, $"Invalid date. Min date: {minDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}, max date: {maxDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}.");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Validator for height.
        /// </summary>
        /// <param name="heightToValidate">The height.</param>
        /// <returns>Result of validation.</returns>
        protected static Tuple<bool, string> HeightValidator(short heightToValidate)
        {
            short minHeight;
            short maxHeight;
            if (Program.CurrentValidationRules == Program.CustomValidationRules)
            {
                minHeight = 120;
                maxHeight = 250;
            }
            else
            {
                minHeight = 40;
                maxHeight = 300;
            }

            if (heightToValidate < minHeight || heightToValidate > maxHeight)
            {
                return Tuple.Create(false, $"The height is not within the allowed range. Min. value = {minHeight}, max. value = {maxHeight}");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Validator for cash savings.
        /// </summary>
        /// <param name="heightToValidate">The cash savings.</param>
        /// <returns>Result of validation.</returns>
        protected static Tuple<bool, string> CashSavingsValidator(decimal heightToValidate)
        {
            decimal minCashSavings;
            decimal maxCashSavings;
            if (Program.CurrentValidationRules == Program.CustomValidationRules)
            {
                minCashSavings = 100M;
                maxCashSavings = 100_000_000M;
            }
            else
            {
                minCashSavings = 0M;
                maxCashSavings = 10_000_000M;
            }

            if (heightToValidate < minCashSavings || heightToValidate > maxCashSavings)
            {
                return Tuple.Create(false, $"The cash savings is not within the allowed range. Min. value = {minCashSavings}, max. value = {maxCashSavings}");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Validator for letter.
        /// </summary>
        /// <param name="c">The letter.</param>
        /// <returns>Result of validation.</returns>
        protected static Tuple<bool, string> LetterValidator(char c)
        {
            if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
            {
                return Tuple.Create(false, $"\"{c}\" is not an English letter");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Gets substrings from <paramref name="inputString"/>.
        /// </summary>
        /// <param name="inputString">String.</param>
        /// <returns>Substrings.</returns>
        protected static List<string> GetSubstrings(string inputString) => inputString.Split(new char[] { '(', ',', ')' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

        /// <summary>
        /// Gets search options and logical operator between them.
        /// </summary>
        /// <param name="searchOptions">A set of parameters to search a record.</param>
        /// <param name="logicalOperator">Logical operator.</param>
        /// <returns>Separated options.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="searchOptions"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when using the "AND" and "OR" operators at the same time.</exception>
        protected static List<KeyValuePair<string, string>> GetKeyValuePairsOfSearchOptions(string searchOptions, out string logicalOperator)
        {
            if (string.IsNullOrEmpty(searchOptions))
            {
                throw new ArgumentNullException(nameof(searchOptions));
            }

            if (searchOptions.Trim() == SelectAll)
            {
                logicalOperator = string.Empty;
                return new () { new (SelectAll, SelectAll) };
            }

            var keyValuePairs = new List<KeyValuePair<string, string>>();
            var separatedByAnd = Regex.Split(searchOptions, " and ", RegexOptions.IgnoreCase);
            var separatedByOr = Regex.Split(searchOptions, " or ", RegexOptions.IgnoreCase);

            var byAndIterator = separatedByAnd.GetEnumerator();
            byAndIterator.MoveNext();
            var byOrIterator = separatedByOr.GetEnumerator();
            byOrIterator.MoveNext();

            var firstSubstringByAnd = byAndIterator.Current;
            var firstSubstringByOr = byOrIterator.Current;

            string[] separatedSearchOptions;
            if (!firstSubstringByAnd.Equals(searchOptions) &&
                !firstSubstringByOr.Equals(searchOptions) &&
                !separatedByAnd.SequenceEqual(separatedByOr))
            {
                throw new ArgumentException("Either 'AND' or 'OR' is allowed, but not both.");
            }
            else if (!firstSubstringByAnd.Equals(searchOptions))
            {
                logicalOperator = "and";
                separatedSearchOptions = separatedByAnd;
            }
            else
            {
                logicalOperator = "or";
                separatedSearchOptions = separatedByOr;
            }

            var optionRegex = new Regex(@"(.*)=(.*)");
            foreach (var option in separatedSearchOptions)
            {
                keyValuePairs.Add(GetPairOfParameters(optionRegex, option));
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Gets key value pair of parameters.
        /// </summary>
        /// <param name="regex">Used regex.</param>
        /// <param name="pairOfParams">Source pair.</param>
        /// <returns>Separated pair.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="pairOfParams"/> is invalid.</exception>
        protected static KeyValuePair<string, string> GetPairOfParameters(Regex regex, string pairOfParams)
        {
            const int keyIndex = 1;
            const int valueIndex = 2;
            if (regex.IsMatch(pairOfParams))
            {
                var match = regex.Match(pairOfParams);
                var key = match.Groups[keyIndex].Value.Trim(' ').ToLowerInvariant();
                var value = Regex.Match(match.Groups[valueIndex].Value, @"'(.*?)'").Groups[1].Value.Trim(' ');
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"In pair '{pairOfParams}' value must be in single quotes.");
                }

                return new KeyValuePair<string, string>(key, value);
            }

            throw new ArgumentException($"Pair of parameters '{pairOfParams}' is incorrect.");
        }

        /// <summary>
        /// The command itself.
        /// </summary>
        /// <param name="parameters">Parameters of the command.</param>
        protected abstract void Command(string parameters);

        /// <summary>
        /// Prints message about unknown command.
        /// </summary>
        /// <param name="command">Unknown command.</param>
        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command. See help.");
            Console.WriteLine();

            var similarCommands = new List<string>();
            const int sensingDistance = 3;
            foreach (var supportedCommand in HelpCommandHandler.GetListOfCommands())
            {
                if (GetLevenshteinDistance(command, supportedCommand) < sensingDistance)
                {
                    similarCommands.Add(supportedCommand);
                }
            }

            if (similarCommands.Count > 0)
            {
                Console.Write("The most similar command");
                if (similarCommands.Count == 1)
                {
                    Console.WriteLine(" is");
                }
                else
                {
                    Console.WriteLine("s are");
                }

                foreach (var similarCommand in similarCommands)
                {
                    Console.WriteLine($"\t{similarCommand}");
                }

                Console.WriteLine();
            }

            return;
        }

        private static int GetMinInt(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;

        private static int GetLevenshteinDistance(string firstString, int firstLength, string secondString, int secondLength)
        {
            if (firstLength == 0)
            {
                return secondLength;
            }

            if (secondLength == 0)
            {
                return firstLength;
            }

            var substitutionCost = 0;
            if (firstString[firstLength - 1] != secondString[secondLength - 1])
            {
                substitutionCost = 1;
            }

            var deletion = GetLevenshteinDistance(firstString, firstLength - 1, secondString, secondLength) + 1;
            var insertion = GetLevenshteinDistance(firstString, firstLength, secondString, secondLength - 1) + 1;
            var substitution = GetLevenshteinDistance(firstString, firstLength - 1, secondString, secondLength - 1) + substitutionCost;

            return GetMinInt(deletion, insertion, substitution);
        }

        private static int GetLevenshteinDistance(string firstString, string secondString) => GetLevenshteinDistance(firstString, firstString.Length, secondString, secondString.Length);
    }
}