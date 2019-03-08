using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BomRepo.BRXXXXX.DL
{
    public class Entity
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedByUsername { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string NamePattern { get; set; }
    }

    public class EntityValidator : AbstractValidator<Entity> {
        public EntityValidator() {
            RuleFor(e => e.CreatedByUsername).NotNull().NotEmpty().WithMessage("CreatedByUsername field cannot be null or empty.");
            RuleFor(e => e.Name).NotNull().NotEmpty().WithMessage("Name field cannot be null or empty");
            RuleFor(e => e.NamePattern).NotNull().NotEmpty().WithMessage("NamePattern cannot be null or empty");
        }
    }
}
