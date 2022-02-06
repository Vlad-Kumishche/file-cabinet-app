using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    public class CustomCashSavingsValidator : IRecordValidator
    {
        /// <summary>
        /// Checks if the cash savings is within a specified range.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the cash savings does not match the specified range.</exception>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            decimal cashSavings = recordToValidate.CashSavings;
            const decimal minCashSavings = 100M;
            const decimal maxCashSavings = 100_000_000M;
            if (cashSavings < minCashSavings || cashSavings > maxCashSavings)
            {
                throw new ArgumentException($"The {nameof(cashSavings)} is not within the allowed range.", nameof(recordToValidate));
            }
        }
    }
}
