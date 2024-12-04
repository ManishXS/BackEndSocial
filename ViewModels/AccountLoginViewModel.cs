
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.ViewModels
{
    public class AccountLoginViewModel
    {

        [StringLength(60, MinimumLength = 3)]
        [BindProperty]
        [Required]
        public string Username { get; set; }
    }
}
