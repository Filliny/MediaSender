using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MediaSender.Helpers
{
    public class EmailRegexValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            Match match = regex.Match(value.ToString());
            if (match == null || match == Match.Empty)
            {
                return new ValidationResult(false, "Please enter valid email");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }


    public class TelegramRegexValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^@([\w-]+)$");
            Match match = regex.Match(value.ToString());
            if (match == null || match == Match.Empty)
            {
                return new ValidationResult(false, "Please enter valid contact");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }

    public class ResponseRegexValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^([\d]){5}$");
            Match match = regex.Match(value.ToString());
            if (match == null || match == Match.Empty)
            {
                return new ValidationResult(false, "Please enter valid 6 digit code");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }


    public class CheckBoxValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^@([\w-]+)$");
            Match match = regex.Match(value.ToString());
            if (match == null || match == Match.Empty)
            {
                return new ValidationResult(false, "Please enter valid contact");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
