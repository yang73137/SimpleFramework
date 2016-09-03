using System;

namespace Infrastructure.Validation.ValidationRules
{
    public class CustomRule : ValidationRule
    {
        public Func<object, bool> ValidateFunc { get; private set; }

        public CustomRule(Func<object, bool> validateFunc, string errorMessage)
            : base(errorMessage)
        {
            if (validateFunc == null)
            {
                throw new ArgumentNullException("validateFunc");
            }

            this.ValidateFunc = validateFunc;
            this.ErrorMessage = errorMessage ?? "无效";
        }

        public override bool IsValid(object value)
        {
            return this.ValidateFunc(value);
        }
    }
}
