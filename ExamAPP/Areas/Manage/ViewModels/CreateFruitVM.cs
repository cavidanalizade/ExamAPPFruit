using FluentValidation;

namespace ExamAPP.Areas.Manage.ViewModels
{
    public class CreateFruitVM
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public IFormFile Image { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public class CreateFruitVmValidator : AbstractValidator<CreateFruitVM>
    {
        public CreateFruitVmValidator()
        {
            RuleFor(x => x.Title).NotNull().WithMessage("Title bos ola bilmez");
            RuleFor(x => x.Title).MinimumLength(3).WithMessage("Title 3 den kicik ola bilmez");
            RuleFor(x => x.Title).MaximumLength(25).WithMessage("Title 25 den boyuk ola bilmez");

            RuleFor(x => x.Subtitle).NotNull().WithMessage("Subtitle bos ola bilmez");
            RuleFor(x => x.Subtitle).MinimumLength(3).WithMessage("Subtitle 3 den kicik ola bilmez");
            RuleFor(x => x.Subtitle).MaximumLength(25).WithMessage("Subtitle 25 den boyuk ola bilmez");

            RuleFor(x => x.Image).NotNull().WithMessage("image bos ola bilmez");


        }
    }
}
