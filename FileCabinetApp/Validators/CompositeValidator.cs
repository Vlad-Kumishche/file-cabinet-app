using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Composite validator.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">List of validators.</param>
        public CompositeValidator(List<IRecordValidator> validators)
        {
            this.validators = validators;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="recordToValidate">The parameters.</param>
        public void ValidateParameters(RecordParameters recordToValidate)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(recordToValidate);
            }
        }
    }
}
