using System;
using System.Text.RegularExpressions;

namespace Infrastructure.Validation.ValidationRules
{
    public class RegexRule : ValidationRule
    {
        public Regex Regex { get; private set; }

        public RegexRule(string pattern, string errorMessage)
            : base(errorMessage)
        {
            this.Regex = new Regex(pattern, RegexOptions.Compiled);
            this.ErrorMessage = errorMessage ?? "格式错误";
        }

        public override bool IsValid(object value)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString()) || this.Regex.IsMatch(value.ToString()))
            {
                return true;
            }

            return false;
        }
    }
}
