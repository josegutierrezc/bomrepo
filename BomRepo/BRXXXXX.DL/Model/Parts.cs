using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BomRepo.BRXXXXX.DL
{
    public class Part
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedByUsername { get; set; }
        public int ProjectId { get; set; }
        public int PartDefinitionId { get; set; }
        public string Name { get; set; }
    }

    public class PartValidator : AbstractValidator<Part> {
        public PartValidator() {
            RuleFor(e => e.CreatedByUsername).NotNull().NotEmpty();
            RuleFor(e => e.ProjectId).NotNull();
            RuleFor(e => e.PartDefinitionId).NotNull();
            RuleFor(e => e.Name).NotNull().NotEmpty();
        }
    }
}
