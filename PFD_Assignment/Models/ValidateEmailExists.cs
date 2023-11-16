using PFD_Assignment.DAL;
using System.ComponentModel.DataAnnotations;

namespace PFD_Assignment.Models
{
    public class ValidateEmailExists : ValidationAttribute
    {
        private MemberDAL memberContext = new MemberDAL();
        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            // Get the email value to validate
            string email = Convert.ToString(value);
            Member member = (Member)validationContext.ObjectInstance;
            if (memberContext.IsEmailExist(email))
                // validation failed
                return new ValidationResult
                ("Email address already exists!");
            else
                // validation passed 
                return ValidationResult.Success;
        }
    }
}
