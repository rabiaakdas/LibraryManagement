using FluentValidation;
using LibraryManagement.Web.Areas.Admin.Models;

namespace LibraryManagement.Web.Validators
{
    public class OrderValidator : AbstractValidator<AdminOrderStatusViewModel>
    {
        public OrderValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Sipariş durumu seçilmelidir.");

            RuleFor(x => x.CargoCompany)
                .MaximumLength(100).WithMessage("Kargo şirketi en fazla 100 karakter olabilir.");

            RuleFor(x => x.TrackingNumber)
                .MaximumLength(50).WithMessage("Takip numarası en fazla 50 karakter olabilir.");

            When(x => x.Status == "Shipped", () =>
            {
                RuleFor(x => x.CargoCompany)
                    .NotEmpty().WithMessage("Kargoya verilen siparişlerde kargo şirketi zorunludur.");

                RuleFor(x => x.TrackingNumber)
                    .NotEmpty().WithMessage("Kargoya verilen siparişlerde takip numarası zorunludur.");
            });
        }
    }
}
