using FluentValidation;
using LibraryManagement.Web.Models;

namespace LibraryManagement.Web.Validators
{
    public class CheckoutValidator : AbstractValidator<CheckoutViewModel>
    {
        public CheckoutValidator()
        {
            RuleFor(x => x.AddressId)
                .NotEmpty()
                .WithMessage("Teslimat adresi seçmelisiniz.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("Ödeme yöntemi seçmelisiniz.")
                .Must(method => method == "Kapıda ödeme" ||
                                method == "Kredi kartı simülasyonu" ||
                                method == "Havale/EFT")
                .WithMessage("Geçerli bir ödeme yöntemi seçmelisiniz.");

            When(x => x.PaymentMethod == "Kredi kartı simülasyonu", () =>
            {
                RuleFor(x => x.CardHolderName)
                    .NotEmpty()
                    .WithMessage("Kart sahibi zorunludur.");

                RuleFor(x => x.CardNumber)
                    .NotEmpty()
                    .WithMessage("Kart numarası zorunludur.")
                    .Matches(@"^\d{16}$")
                    .WithMessage("Kart numarası 16 haneli olmalıdır.");

                RuleFor(x => x.ExpirationDate)
                    .NotEmpty()
                    .WithMessage("Son kullanma tarihi zorunludur.")
                    .Matches(@"^(0[1-9]|1[0-2])\/\d{2}$")
                    .WithMessage("Son kullanma tarihi AA/YY formatında olmalıdır.");

                RuleFor(x => x.Cvv)
                    .NotEmpty()
                    .WithMessage("CVV zorunludur.")
                    .Matches(@"^\d{3,4}$")
                    .WithMessage("CVV 3 veya 4 haneli olmalıdır.");
            });
        }
    }
}
