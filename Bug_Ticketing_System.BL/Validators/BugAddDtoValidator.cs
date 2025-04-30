using Bug_Ticketing_System.BL.Dtos;
using FluentValidation;

namespace Bug_Ticketing_System.BL
{
    public class BugAddDtoValidator : AbstractValidator<BugAddDto>
    {
        public BugAddDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Priority).IsEnumName(typeof(PriorityLevel));
            RuleFor(x => x.ProjectId).NotEmpty();


        }
    }
}
