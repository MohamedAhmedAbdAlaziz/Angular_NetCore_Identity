using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15,MinimumLength =3 , ErrorMessage ="FirstName must be at Least(2) ,and maximum{1} characters")]
         public string FirstName {get;set;}
        [Required]
        [StringLength(15,MinimumLength =3 , ErrorMessage ="LastName must be at Least(2) ,and maximum{1} characters")]

         public string  LastName { get; set; }
         [Required]
 [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
   ErrorMessage = "Email must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 6 characters")]
           public string  Email { get; set; }
         [Required]
          [StringLength(15,MinimumLength =6 , ErrorMessage ="Password must be at Least(2) ,and maximum{1} characters")]
  
         public string  Password { get; set; }
    }
}