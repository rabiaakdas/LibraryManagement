using FluentValidation;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Models.Api;

namespace LibraryManagement.Web.Validators
{
    public class BookValidator : AbstractValidator<BookUpsertDto>
    {
        public BookValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Kitap adı boş olamaz.")
                .MinimumLength(2).WithMessage("Kitap adı en az 2 karakter olmalıdır.")
                .MaximumLength(200).WithMessage("Kitap adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Yazar boş olamaz.")
                .MinimumLength(2).WithMessage("Yazar en az 2 karakter olmalıdır.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz.");

            RuleFor(x => x.PageCount)
                .GreaterThan(0).WithMessage("Sayfa sayısı 0'dan büyük olmalıdır.");
        }
    }

    public class AdminBookFormViewModelValidator : AbstractValidator<AdminBookFormViewModel>
    {
        public AdminBookFormViewModelValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Kitap adı boş olamaz.")
                .MinimumLength(2).WithMessage("Kitap adı en az 2 karakter olmalıdır.")
                .MaximumLength(200).WithMessage("Kitap adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Yazar boş olamaz.")
                .MinimumLength(2).WithMessage("Yazar en az 2 karakter olmalıdır.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz.");

            RuleFor(x => x.PageCount)
                .GreaterThan(0).WithMessage("Sayfa sayısı 0'dan büyük olmalıdır.");
        }
    }
}