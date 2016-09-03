using System;

namespace Infrastructure.Validation.ValidationRules
{
    public class RangeRule : ValidationRule
    {
        public IComparable Minimum { get; private set; }

        public IComparable Maximum { get; private set; }

        public RangeRule(IComparable minimum, IComparable maximum, string errorMessage)
            : base(errorMessage)
        {
            if (minimum == null)
            {
                throw new ArgumentNullException("minimum");
            }

            if (maximum == null)
            {
                throw new ArgumentNullException("maximum");
            }

            if (maximum.CompareTo(minimum) < 0)
            {
                throw new ArgumentException("maximum is smaller than minimum");
            }

            this.Minimum = minimum;
            this.Maximum = maximum;
            this.ErrorMessage = errorMessage ?? String.Format("有效范围{0}-{1}", minimum.ToString(), maximum.ToString());
        }

        public override bool IsValid(object value)
        {
            var comparable = value as IComparable;

            if (comparable == null)
            {
                return true;
            }

            return comparable.CompareTo(Minimum) >= 0 && comparable.CompareTo(Maximum) <= 0;
        }
    }
}
