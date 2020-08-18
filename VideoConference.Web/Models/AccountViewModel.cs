using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoConference.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Username is required")]
        [AlphaNumeric(ErrorMessage ="Username can only contain letters and numbers")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [BeginWIthAlphaNumeric(ErrorMessage = "Name must begin with alphabeth or number")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name ="Department")]
        public List<SelectListItem> Departments { get; set; }

        [Display(Name ="User type")]
        public List<SelectListItem> UserTypes { get; set; }
    }

    public class EditUserViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [BeginWIthAlphaNumeric(ErrorMessage = "Name must begin with alphabeth or number")]
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class AlphaNumeric : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string strValue = value as string;

            if (strValue.Length < 1)
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));

            if (!Regex.IsMatch(strValue, "^[a-zA-Z0-9]*$"))
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }

}
