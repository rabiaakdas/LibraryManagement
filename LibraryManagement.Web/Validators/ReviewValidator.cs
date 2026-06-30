using FluentValidation;
using LibraryManagement.Web.Models;

namespace LibraryManagement.Web.Validators
{
    public class ReviewValidator : AbstractValidator<BookReviewCreateViewModel>
    {
        public ReviewValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Puan 1 ile 5 arasında olmalıdır.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Yorum boş olamaz.")
                .MaximumLength(500).WithMessage("Yorum en fazla 500 karakter olabilir.");
        }
    }
}