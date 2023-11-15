using PFD_Assignment.DAL;
using System.ComponentModel.DataAnnotations;

namespace PFD_Assignment.Models
{
    public class ValidateUserExists : ValidationAttribute
    {
        private MemberDAL memberContext = new MemberDAL();
        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            // Get the username value to validate
            string user = Convert.ToString(value);
            // Casting the validation context to the "Member" model class
            Member member = (Member)validationContext.ObjectInstance;
            // Get the member Id from the member instance
            int memberID = member.MemberId;
            if (memberContext.IfUserExist(user/*, memberID*/))
                // validation failed
                return new ValidationResult
                ("Username is already in use! Please choose a different one.");
            else
                // validation passed 
                return ValidationResult.Success;
        }
    }
}
