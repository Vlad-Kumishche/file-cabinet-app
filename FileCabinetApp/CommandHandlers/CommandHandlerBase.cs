using System.Globalization;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// CommandHandlerBase.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
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
        /// The command itself.
        /// </summary>
        /// <param name="parameters">Parameters of the command.</param>
        protected abstract void Command(string parameters);

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

        protected static Tuple<bool, string, string> StringConverter(string stringToConvert)
        {
            return Tuple.Create(true, string.Empty, stringToConvert);
        }

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

        protected static Tuple<bool, string, short> ShortConverter(string stringToConvert)
        {
            if (short.TryParse(stringToConvert, out var number))
            {
                return Tuple.Create(true, string.Empty, number);
            }

            return Tuple.Create(false, $"{stringToConvert} is not short number", number);
        }

        protected static Tuple<bool, string, decimal> DecimalConverter(string stringToConvert)
        {
            if (decimal.TryParse(stringToConvert, out var number))
            {
                return Tuple.Create(true, string.Empty, number);
            }

            return Tuple.Create(false, $"{stringToConvert} is not decimal number", number);
        }

        protected static Tuple<bool, string, char> CharConverter(string stringToConvert)
        {
            if (char.TryParse(stringToConvert, out var c))
            {
                return Tuple.Create(true, string.Empty, c);
            }

            return Tuple.Create(false, $"{stringToConvert} is not character", c);
        }

        protected static Tuple<bool, string> NameValidator(string nameToValidate)
        {
            int minLength;
            int maxLength;
            if (Program.validator is DefaultValidator)
            {
                minLength = 2;
                maxLength = 60;
            }
            else
            {
                minLength = 4;
                maxLength = 20;
            }

            if (string.IsNullOrEmpty(nameToValidate) || nameToValidate.Length < minLength || nameToValidate.Length > maxLength)
            {
                return Tuple.Create(false, $"Length of \"{nameToValidate}\" does not meet the requirements. Min. length = {minLength}, max. lenght = {maxLength}");
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

        protected static Tuple<bool, string> DateValidator(DateTime dateToValidate)
        {
            DateTime minDate;
            DateTime maxDate;
            if (Program.validator is DefaultValidator)
            {
                minDate = new DateTime(1950, 1, 1);
                maxDate = DateTime.Now;
            }
            else
            {
                minDate = new DateTime(1940, 1, 1);
                const int ageOfMajority = 18;
                maxDate = DateTime.Now.AddYears(-ageOfMajority);
            }

            if (dateToValidate < minDate || dateToValidate >= maxDate)
            {
                return Tuple.Create(false, $"Invalid date. Min date: {minDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}, max date: {maxDate.ToString("MM / dd / yyyy", CultureInfo.InvariantCulture)}.");
            }

            return Tuple.Create(true, string.Empty);
        }

        protected static Tuple<bool, string> HeightValidator(short heightToValidate)
        {
            short minHeight;
            short maxHeight;
            if (Program.validator is DefaultValidator)
            {
                minHeight = 40;
                maxHeight = 300;
            }
            else
            {
                minHeight = 120;
                maxHeight = 250;
            }

            if (heightToValidate < minHeight || heightToValidate > maxHeight)
            {
                return Tuple.Create(false, $"The height is not within the allowed range. Min. value = {minHeight}, max. value = {maxHeight}");
            }

            return Tuple.Create(true, string.Empty);
        }

        protected static Tuple<bool, string> CashSavingsValidator(decimal heightToValidate)
        {
            decimal minCashSavings;
            decimal maxCashSavings;
            if (Program.validator is DefaultValidator)
            {
                minCashSavings = 0M;
                maxCashSavings = 10_000_000M;
            }
            else
            {
                minCashSavings = 100M;
                maxCashSavings = 100_000_000M;
            }

            if (heightToValidate < minCashSavings || heightToValidate > maxCashSavings)
            {
                return Tuple.Create(false, $"The cash savings is not within the allowed range. Min. value = {minCashSavings}, max. value = {maxCashSavings}");
            }

            return Tuple.Create(true, string.Empty);
        }

        protected static Tuple<bool, string> LetterValidator(char c)
        {
            if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
            {
                return Tuple.Create(false, $"\"{c}\" is not an English letter");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// Prints message about unknown command.
        /// </summary>
        /// <param name="command">Unknown command.</param>
        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}