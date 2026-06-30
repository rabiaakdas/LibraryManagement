using FluentValidation;
using LibraryManagement.Web.Models;

namespace LibraryManagement.Web.Validators
{
    public class UserRegisterValidator : AbstractValidator<RegisterViewModel>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
                .MaximumLength(30).WithMessage("Kullanıcı adı en fazla 30 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                .Matches("[A-Z]").WithMessage("Şifre en az 1 büyük harf içermelidir.")
                .Matches("[a-z]").WithMessage("Şifre en az 1 küçük harf içermelidir.")
                .Matches("[0-9]").WithMessage("Şifre en az 1 rakam içermelidir.");
        }
    }
}