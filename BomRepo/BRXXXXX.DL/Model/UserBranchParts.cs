using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using FluentValidation;

namespace BomRepo.BRXXXXX.DL
{
    public partial class UserBranchPart
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserBranchId { get; set; }
        public int EntityId { get; set; }
        public string Name { get; set; }
    }

    public class UserBranchPartValidator : AbstractValidator<UserBranchPart> {
        public UserBranchPartValidator() {
            RuleFor(e => e.UserBranchId).NotNull();
            RuleFor(e => e.EntityId).NotNull();
            RuleFor(e => e.Name).NotNull().NotEmpty();
            RuleFor(e => e.UserBranchId).NotNull().NotEmpty();
        }
    }
}
