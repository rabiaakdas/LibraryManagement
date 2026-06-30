using FluentValidation;
using LibraryManagement.Web.Areas.Admin.Models;

namespace LibraryManagement.Web.Validators
{
    public class GenreValidator : AbstractValidator<AdminGenreFormViewModel>
    {
        public GenreValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Kategori adı boş olamaz.")
                .MinimumLength(2).WithMessage("Kategori adı en az 2 karakter olmalıdır.")
                .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");
        }
    }
}