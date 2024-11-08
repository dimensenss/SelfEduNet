using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.ViewModels
{
    public class AccountLoginViewModel
    {
        [Required(ErrorMessage = "Необхідно ввести email")]
        [Display(Name = "Введіть email")]
        [DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Необхідно ввести пароль")]
        [Display(Name = "Введіть пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
