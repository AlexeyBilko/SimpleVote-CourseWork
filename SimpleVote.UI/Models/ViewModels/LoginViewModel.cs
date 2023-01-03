using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleVote.UI.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="EmailRequired")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
