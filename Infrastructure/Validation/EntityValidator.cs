using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Infrastructure.Descriptor;
using Infrastructure.Validation.ValidationRules;

namespace Infrastructure.Validation
{
    #region FltConfigValidator

    public class FltConfigValidator<TEntity> where TEntity : class
    {
        protected Dictionary<string, FltConfigValidationRuleCollection> RuleDict = new Dictionary<string, FltConfigValidationRuleCollection>();

        protected EntityDescriptor<TEntity> EntityDescriptor = EntityDescriptor<TEntity>.Instance;

        public FltConfigValidator()
        {
            this.AddRulesFromDataAnnotations();
        }

        public virtual FltConfigValidationResult Validate(TEntity entity)
        {
            if (entity == null)
            {
                return FltConfigValidationResult.Null;
            }

            var propertities = EntityDescriptor.PropertyDescriptors;
            foreach (var property in propertities)
            {
                FltConfigValidationRuleCollection ruleCollection;
                if (RuleDict.TryGetValue(property.PropertyName, out ruleCollection) && ruleCollection != null)
                {
                    foreach (var rule in ruleCollection)
                    {
                        var value = property.GetValue(entity);
                        var isValid = rule.IsValid(value);
                        if (!isValid)
                        {
                            return new FltConfigValidationResult { MemberName = property.PropertyName, ErrorMessage = rule.ErrorMessage, IsValid = false };
                        }
                    }
                }
            }

            return FltConfigValidationResult.Success;
        }

        public FltConfigValidationRuleCollection Rules<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("Expression or Rule is null");
            }

            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("Expression is not MemberExpression");
            }

            var propertyName = memberExpression.Member.Name;

            if (!RuleDict.ContainsKey(propertyName))
            {
                RuleDict[propertyName] = new FltConfigValidationRuleCollection();
            }

            return RuleDict[propertyName];
        }

        protected void AddRule(string propertyName, ValidationRule rule)
        {
            if (String.IsNullOrWhiteSpace(propertyName) || rule == null)
            {
                return;
            }

            if (!RuleDict.ContainsKey(propertyName))
            {
                RuleDict[propertyName] = new FltConfigValidationRuleCollection();
            }

            RuleDict[propertyName].Add(rule);
        }

        private void AddRulesFromDataAnnotations()
        {
            var propertities = EntityDescriptor.PropertyDescriptors;

            foreach (var property in propertities)
            {
                var validationAttributes = property.GetAttributes<ValidationAttribute>();

                foreach (var validationAttribute in validationAttributes)
                {
                    this.AddRule(property.PropertyName, new CustomRule(validationAttribute.IsValid, validationAttribute.ErrorMessage));
                }
            }
        }
    }

    public class FltConfigValidationRuleCollection : List<ValidationRule>
    {
        public FltConfigValidationRuleCollection AddRule(ValidationRule rule)
        {
            this.Add(rule);
            return this;
        }
    }

    #endregion

    #region FltConfigValidationRule

    public static class FltConfigValidationRules
    {
        public static readonly CustomRule NotNull = new CustomRule(obj => obj != null, "必填");

        public static readonly CustomRule NotNullOrEmpty = new CustomRule(obj =>
        {
            if (obj is String)
            {
                return !String.IsNullOrEmpty(obj as String);
            }

            return obj != null;

        }, "必填");

        public static readonly RegexRule City = new RegexRule(@"^[a-zA-Z]{3}$", "城市三字码格式错误");

        public static readonly RegexRule Airline = new RegexRule(@"^[a-zA-Z0-9]{2}$", "航司二字码格式错误");

        public static readonly RegexRule Airlines = new RegexRule(@"^[a-zA-Z0-9]{2}(,[a-zA-Z0-9]{2})*$", "航司二字码格式错误");

        public static readonly RegexRule Flight = new RegexRule(@"^[a-zA-Z0-9]{2}\d{3,4}[a-zA-Z]?$", "航班格式错误");
    }

    #endregion

    #region FltConfigValidationResult

    public class FltConfigValidationResult
    {
        public static readonly FltConfigValidationResult Success = new FltConfigValidationResult { IsValid = true, ErrorMessage = String.Empty };

        public static readonly FltConfigValidationResult Null = new FltConfigValidationResult { IsValid = false, ErrorMessage = "不能为null" };

        public static readonly FltConfigValidationResult Required = new FltConfigValidationResult { IsValid = false, ErrorMessage = "必填" };

        public bool IsValid { get; set; }

        public string MemberName { get; set; }

        public string ErrorMessage { get; set; }
    }

    #endregion
}