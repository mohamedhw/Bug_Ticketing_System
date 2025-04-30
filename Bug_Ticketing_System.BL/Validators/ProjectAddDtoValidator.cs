
using Bug_Ticketing_System.BL.Dtos;
using Bug_Ticketing_System.DAL.UnitOfWork;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Bug_Ticketing_System.BL
{
    public class ProjectAddDtoValidator : AbstractValidator<ProjectAddDto>
    {
        private readonly IConfiguration configuration;
        private IUnitOfWork _unitOfWork;

        public ProjectAddDtoValidator(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            RuleFor(d => d.ProjectName)
                .NotEmpty()
                .WithMessage("Project name is required.");

            RuleFor(d => d.Description)
                .NotEmpty()
                .WithMessage("Project description is required.");


            

        }
    }
}
