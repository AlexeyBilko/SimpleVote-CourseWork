using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleVote.UI.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="NameRequired")]
        [RegularExpression("^((?!^Username$)[a-zA-Zа-яА-Я '])+$", ErrorMessage ="InvalidUserName")]
        [MinLength(2,ErrorMessage ="UserNameTooShort")]
        [MaxLength(20,ErrorMessage ="UserNameIsLargest")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="EmailRequired")]
        [EmailAddress(ErrorMessage="IncorrectEmail")]
        public string Email { get; set; }

        [Required(ErrorMessage ="PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPasswordRequired")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="ComparePasswords")]
        public string ConfirmPassword { get; set; }

    }
}
