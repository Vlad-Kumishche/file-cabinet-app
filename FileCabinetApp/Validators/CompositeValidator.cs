using FileCabinetApp.Data;

namespace FileCabinetApp.Validators
{
    public abstract class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        protected CompositeValidator(List<IRecordValidator> validators)
        {
            this.validators = validators;
        }

        public void ValidateParameters(RecordArgs recordToValidate)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(recordToValidate);
            }
        }
    }
}
