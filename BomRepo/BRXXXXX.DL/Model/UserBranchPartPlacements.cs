using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace BomRepo.BRXXXXX.DL
{
    public partial class UserBranchPartPlacement
    {
        public int UserBranchId { get; set; }
        public int ParentUserBranchPartId { get; set; }
        public int ChildUserBranchPartId { get; set; }
        public int Qty { get; set; }
    }

    public class UserBranchPartPlacementValidator : AbstractValidator<UserBranchPartPlacement> {
        public UserBranchPartPlacementValidator() {
            RuleFor(e => e.UserBranchId).NotNull();
            RuleFor(e => e.ParentUserBranchPartId).NotNull();
            RuleFor(e => e.ChildUserBranchPartId).NotNull();
            RuleFor(e => e.Qty).NotNull().GreaterThan(0);
        }
    }
}
