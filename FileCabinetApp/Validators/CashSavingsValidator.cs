using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    public class CashSavingsValidator : IRecordValidator
    {
        private decimal minCashSavings;
        private decimal maxCashSavings;

        public CashSavingsValidator(decimal minCashSavings, decimal maxCashSavings)
        {
            this.minCashSavings = minCashSavings;
            this.maxCashSavings = maxCashSavings;
        }

        /// <summary>
        /// Checks if the cash savings is within a specified range.
        /// </summary>
        /// <param name="recordToValidate">Record to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the cash savings does not match the specified range.</exception>
        public void ValidateParameters(RecordArgs recordToValidate)
        {
            decimal cashSavings = recordToValidate.CashSavings;
            if (cashSavings < this.minCashSavings || cashSavings > this.maxCashSavings)
            {
                throw new ArgumentException($"The {nameof(cashSavings)} is not within the allowed range.", nameof(recordToValidate));
            }
        }
    }
}
