using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BomRepo.BRXXXXX.DL
{
    public class PartPlacement
    {
        public int ParentPartId { get; set; }
        public int ChildPartId { get; set; }
        public int Qty { get; set; }
    }

    public class PartPlacementValidator : AbstractValidator<PartPlacement> {
        public PartPlacementValidator() {
            RuleFor(e => e.ParentPartId).NotNull();
            RuleFor(e => e.ChildPartId).NotNull();
            RuleFor(e => e.Qty).NotNull().GreaterThan(0);
        }
    }
}
