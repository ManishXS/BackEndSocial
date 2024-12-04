using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.ViewModels
{
    public class AccountRegisterViewModel
    {

        [StringLength(60, MinimumLength = 3)]
        [BindProperty]
        [Required]
        public string Username { get; set; }

        public string Message { get; set; }

    }
}
