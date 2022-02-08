using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Gets information about validation rules from json configuration file.
    /// </summary>
    public class ValidationRulesConfigurationReader
    {
        private readonly IConfiguration config;
        private readonly string validationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRulesConfigurationReader"/> class.
        /// </summary>
        /// <param name="validationType">Validation type.</param>
        public ValidationRulesConfigurationReader(string validationType)
        {
            if (string.IsNullOrEmpty(validationType))
            {
                throw new ArgumentNullException(nameof(validationType));
            }

            if (validationType.Equals("custom", StringComparison.OrdinalIgnoreCase))
            {
                this.validationType = "custom";
            }
            else
            {
                this.validationType = "default";
            }

            this.config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("validation-rules.json")
                .Build();
        }

        /// <summary>
        /// Returns the criteria for checking the first name.
        /// </summary>
        /// <returns>A tuple of minimum and maximum first name lengths.</returns>
        public (int, int) ReadFirstNameValidationCriteria()
        {
            var firstNameSection = this.config.GetSection(this.validationType).GetSection("firstName");
            var minLenght = int.Parse(firstNameSection.GetSection("min").Value, CultureInfo.InvariantCulture);
            var maxLenght = int.Parse(firstNameSection.GetSection("max").Value, CultureInfo.InvariantCulture);
            return (minLenght, maxLenght);
        }

        /// <summary>
        /// Returns the criteria for checking the last name.
        /// </summary>
        /// <returns>A tuple of minimum and maximum last name lengths.</returns>
        public (int, int) ReadLastNameValidationCriteria()
        {
            var lastNameSection = this.config.GetSection(this.validationType).GetSection("lastName");
            var minLenght = int.Parse(lastNameSection.GetSection("min").Value, CultureInfo.InvariantCulture);
            var maxLenght = int.Parse(lastNameSection.GetSection("max").Value, CultureInfo.InvariantCulture);
            return (minLenght, maxLenght);
        }

        /// <summary>
        /// Returns the criteria for checking the date of birth.
        /// </summary>
        /// <returns>A tuple of the minimum and maximum birthday dates.</returns>
        public (DateTime, DateTime) ReadDateOfBirthValidationCriteria()
        {
            var dateOfBirthSection = this.config.GetSection(this.validationType).GetSection("dateOfBirth");
            var from = DateTime.Parse(dateOfBirthSection.GetSection("from").Value, CultureInfo.InvariantCulture);
            var to = DateTime.Parse(dateOfBirthSection.GetSection("to").Value, CultureInfo.InvariantCulture);
            return (from, to);
        }

        /// <summary>
        /// Returns the criteria for checking the height.
        /// </summary>
        /// <returns>A tuple of the minimum and maximum height.</returns>
        public (short, short) ReadHeightValidationCriteria()
        {
            var lastNameSection = this.config.GetSection(this.validationType).GetSection("height");
            var minHeight = short.Parse(lastNameSection.GetSection("min").Value, CultureInfo.InvariantCulture);
            var maxHeight = short.Parse(lastNameSection.GetSection("max").Value, CultureInfo.InvariantCulture);
            return (minHeight, maxHeight);
        }

        /// <summary>
        /// Returns the criteria for checking the cash savings.
        /// </summary>
        /// <returns>A tuple of the minimum and maximum cash savings amount.</returns>
        public (decimal, decimal) ReadCashSavingsValidationCriteria()
        {
            var lastNameSection = this.config.GetSection(this.validationType).GetSection("cashSavings");
            var minCashSavings = decimal.Parse(lastNameSection.GetSection("min").Value, CultureInfo.InvariantCulture);
            var maxCashSavings = decimal.Parse(lastNameSection.GetSection("max").Value, CultureInfo.InvariantCulture);
            return (minCashSavings, maxCashSavings);
        }
    }
}