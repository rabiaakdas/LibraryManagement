using FluentValidation;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Validators
{
    public class CouponValidator : AbstractValidator<Coupon>
    {
        public CouponValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Kupon kodu zorunludur.")
                .MaximumLength(30).WithMessage("Kupon kodu en fazla 30 karakter olabilir.");

            RuleFor(x => x.DiscountType)
                .Must(x => x == "Percentage" || x == "FixedAmount")
                .WithMessage("Geçerli bir indirim türü seçmelisiniz.");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("İndirim değeri 0'dan büyük olmalıdır.");

            RuleFor(x => x.DiscountValue)
                .InclusiveBetween(1, 100)
                .When(x => x.DiscountType == "Percentage")
                .WithMessage("Yüzde indirim 1 ile 100 arasında olmalıdır.");

            RuleFor(x => x.MinimumOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum sipariş tutarı negatif olamaz.");

            RuleFor(x => x.UsageLimit)
                .GreaterThanOrEqualTo(0)
                .When(x => x.UsageLimit.HasValue)
                .WithMessage("Kullanım limiti negatif olamaz.");

            RuleFor(x => x.ExpirationDate)
                .GreaterThanOrEqualTo(System.DateTime.Today)
                .When(x => x.ExpirationDate.HasValue)
                .WithMessage("Son kullanma tarihi geçmiş tarih olamaz.");
        }
    }
}
