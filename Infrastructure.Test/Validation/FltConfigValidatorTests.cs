using System;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Validation;
using Infrastructure.Validation.ValidationRules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.Test.Validation
{
    [TestClass()]
    public class FltConfigValidatorTests
    {
        public class DataAnnotationEntity
        {
            [Range(0, Int32.MaxValue, ErrorMessage = "ID不能小于0")]
            public int ID { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
            [RegularExpression(@"^[a-zA-Z]{3}$", ErrorMessage = "城市三字码格式错误")]
            public string City { get; set; }

            [Range(1, 99, ErrorMessage = "年龄范围1-99")]
            public int Age { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
            [Range(typeof(DateTime), "1995-01-01", "2000-12-31", ErrorMessage = "日期范围错误")]
            public DateTime? Date { get; set; }

            public static DataAnnotationEntity Create()
            {
                return new DataAnnotationEntity
                {
                    ID = 1,
                    City = "SHA",
                    Age = 50,
                    Date = new DateTime(1998, 05, 02),
                };
            }
        }

        public class RuleEntity
        {
            public int ID { get; set; }

            public string City { get; set; }

            public int Age { get; set; }

            public DateTime? Date { get; set; }

            public static RuleEntity Create()
            {
                return new RuleEntity
                {
                    ID = 1,
                    City = "SHA",
                    Age = 50,
                    Date = new DateTime(1998, 05, 02),
                };
            }
        }

        [TestMethod()]
        public void DataAnnotationTest()
        {
            var idRule = new CustomRule(obj => Convert.ToInt32(obj) >= 0, "ID不能小于0");
            var requiredRule = FltConfigValidationRules.NotNullOrEmpty;
            var cityRule = FltConfigValidationRules.City;
            var ageRule = new RangeRule(1, 99, "年龄范围1-99");
            var dateRule = new RangeRule(new DateTime(1995, 1, 1), new DateTime(2000, 12, 31), "日期范围错误");

            var validator = new FltConfigValidator<DataAnnotationEntity>();
            DataAnnotationEntity entity = null;

            #region null

            var result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);

            #endregion

            #region ok

            entity = DataAnnotationEntity.Create();
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            #endregion

            #region ID

            entity = DataAnnotationEntity.Create();
            entity.ID = -1;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("ID", result.MemberName);
            Assert.AreEqual(idRule.ErrorMessage, result.ErrorMessage);

            entity = DataAnnotationEntity.Create();
            entity.ID = 0;
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            #endregion

            #region City

            entity = DataAnnotationEntity.Create();
            entity.City = null;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("City", result.MemberName);
            Assert.AreEqual(requiredRule.ErrorMessage, result.ErrorMessage);

            entity = DataAnnotationEntity.Create();
            entity.City = String.Empty;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("City", result.MemberName);
            Assert.AreEqual(requiredRule.ErrorMessage, result.ErrorMessage);

            entity = DataAnnotationEntity.Create();
            entity.City = "   ";
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("City", result.MemberName);
            Assert.AreEqual(requiredRule.ErrorMessage, result.ErrorMessage);

            entity = DataAnnotationEntity.Create();
            entity.City = "ABC1";
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("City", result.MemberName);
            Assert.AreEqual(cityRule.ErrorMessage, result.ErrorMessage);

            #endregion

            #region Age

            entity = DataAnnotationEntity.Create();
            entity.Age = Convert.ToInt32(ageRule.Minimum) - 1;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Age", result.MemberName);
            Assert.AreEqual(ageRule.ErrorMessage, result.ErrorMessage);

            entity = DataAnnotationEntity.Create();
            entity.Age = Convert.ToInt32(ageRule.Minimum);
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            entity = DataAnnotationEntity.Create();
            entity.Age = Convert.ToInt32(ageRule.Maximum);
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            entity = DataAnnotationEntity.Create();
            entity.Age = Convert.ToInt32(ageRule.Maximum) + 1;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Age", result.MemberName);
            Assert.AreEqual(ageRule.ErrorMessage, result.ErrorMessage);

            #endregion

            #region Date

            entity = DataAnnotationEntity.Create();
            entity.Date = null;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Date", result.MemberName);
            Assert.AreEqual(requiredRule.ErrorMessage, result.ErrorMessage);

            entity = DataAnnotationEntity.Create();
            entity.Date = Convert.ToDateTime(dateRule.Minimum).AddMinutes(-1);
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Date", result.MemberName);
            Assert.AreEqual(dateRule.ErrorMessage, result.ErrorMessage);

            entity = DataAnnotationEntity.Create();
            entity.Date = Convert.ToDateTime(dateRule.Minimum);
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            entity = DataAnnotationEntity.Create();
            entity.Date = Convert.ToDateTime(dateRule.Maximum);
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            entity = DataAnnotationEntity.Create();
            entity.Date = Convert.ToDateTime(dateRule.Maximum).AddMinutes(1);
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Date", result.MemberName);
            Assert.AreEqual(dateRule.ErrorMessage, result.ErrorMessage);

            #endregion
        }

        [TestMethod()]
        public void RuleTest()
        {
            var validator = new FltConfigValidator<RuleEntity>();

            var idRule = new CustomRule(obj => Convert.ToInt32(obj) >= 0, "ID不能小于0");
            validator.Rules(p => p.ID).AddRule(idRule);

            var requiredRule = FltConfigValidationRules.NotNullOrEmpty;
            var cityRule = FltConfigValidationRules.City;
            validator.Rules(p => p.City)
                     .AddRule(requiredRule)
                     .AddRule(cityRule);

            var ageRule = new RangeRule(1, 99, "年龄范围1-99");
            validator.Rules(p => p.Age).AddRule(ageRule);

            var dateRule = new RangeRule(new DateTime(1995, 1, 1), new DateTime(2000, 12, 31), "日期范围错误");
            validator.Rules(p => p.Date).AddRule(dateRule);

            RuleEntity entity = null;

            #region null

            var result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);

            #endregion

            #region ok

            entity = RuleEntity.Create();
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            #endregion

            #region ID

            entity = RuleEntity.Create();
            entity.ID = -1;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("ID", result.MemberName);
            Assert.AreEqual(idRule.ErrorMessage, result.ErrorMessage);

            entity = RuleEntity.Create();
            entity.ID = 0;
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            #endregion

            #region City

            entity = RuleEntity.Create();
            entity.City = null;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("City", result.MemberName);
            Assert.AreEqual(requiredRule.ErrorMessage, result.ErrorMessage);

            entity = RuleEntity.Create();
            entity.City = String.Empty;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("City", result.MemberName);
            Assert.AreEqual(requiredRule.ErrorMessage, result.ErrorMessage);

            entity = RuleEntity.Create();
            entity.City = "   ";
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("City", result.MemberName);
            Assert.AreEqual(cityRule.ErrorMessage, result.ErrorMessage);

            entity = RuleEntity.Create();
            entity.City = "ABC1";
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("City", result.MemberName);
            Assert.AreEqual(cityRule.ErrorMessage, result.ErrorMessage);

            #endregion

            #region Age

            entity = RuleEntity.Create();
            entity.Age = Convert.ToInt32(ageRule.Minimum) - 1;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Age", result.MemberName);
            Assert.AreEqual(ageRule.ErrorMessage, result.ErrorMessage);

            entity = RuleEntity.Create();
            entity.Age = Convert.ToInt32(ageRule.Minimum);
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            entity = RuleEntity.Create();
            entity.Age = Convert.ToInt32(ageRule.Maximum);
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            entity = RuleEntity.Create();
            entity.Age = Convert.ToInt32(ageRule.Maximum) + 1;
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Age", result.MemberName);
            Assert.AreEqual(ageRule.ErrorMessage, result.ErrorMessage);

            #endregion

            #region Date

            entity = RuleEntity.Create();
            entity.Date = null;
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            entity = RuleEntity.Create();
            entity.Date = Convert.ToDateTime(dateRule.Minimum).AddMinutes(-1);
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Date", result.MemberName);
            Assert.AreEqual(dateRule.ErrorMessage, result.ErrorMessage);

            entity = RuleEntity.Create();
            entity.Date = Convert.ToDateTime(dateRule.Minimum);
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            entity = RuleEntity.Create();
            entity.Date = Convert.ToDateTime(dateRule.Maximum);
            result = validator.Validate(entity);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(String.IsNullOrWhiteSpace(result.ErrorMessage));

            entity = RuleEntity.Create();
            entity.Date = Convert.ToDateTime(dateRule.Maximum).AddMinutes(1);
            result = validator.Validate(entity);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Date", result.MemberName);
            Assert.AreEqual(dateRule.ErrorMessage, result.ErrorMessage);

            #endregion
        }
    }
}
