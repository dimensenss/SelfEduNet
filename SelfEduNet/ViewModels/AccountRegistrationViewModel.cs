using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.ViewModels
{
    public class AccountRegistrationViewModel
    {
        [Required(ErrorMessage = "Необхідно ввести email")]
        [Display(Name = "Введіть email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Необхідно ввести ім'я")]
        [Display(Name = "Введіть ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Необхідно ввести прізвище")]
        [Display(Name = "Введіть прізвище")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Необхідно ввести пароль")]
        [Display(Name = "Введіть пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Необхідно підтвердити пароль")]
        [Display(Name = "Підтвердіть пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; }
       
    }
}
