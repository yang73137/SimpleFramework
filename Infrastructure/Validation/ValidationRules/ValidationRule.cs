namespace Infrastructure.Validation.ValidationRules
{
    public abstract class ValidationRule
    {
        public string ErrorMessage { get; protected set; }

        protected ValidationRule(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        public abstract bool IsValid(object value);
    }
}
