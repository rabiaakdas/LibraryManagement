using FluentValidation;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Models.Api.Auth;

namespace LibraryManagement.Web.Validators
{
    public class UserLoginValidator : AbstractValidator<LoginViewModel>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.");
        }
    }

    public class ApiLoginRequestDtoValidator : AbstractValidator<ApiLoginRequestDto>
    {
        public ApiLoginRequestDtoValidator()
        {
            RuleFor(x => x.UserNameOrEmail)
                .NotEmpty().WithMessage("Kullanıcı adı veya e-posta boş olamaz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.");
        }
    }
}