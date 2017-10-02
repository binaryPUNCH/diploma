﻿using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Diploma.BLL.Interfaces.Services;
using Diploma.Common.Properties;
using Diploma.Framework.Validations;
using Diploma.ViewModels;
using FluentValidation;

namespace Diploma.Validators
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        private const int MaximumUsernameCharCount = 30;
        
        private const int MinimumUsernameCharCount = 5;

        private static readonly Regex UsernameCharCheck = new Regex("^[a-zA-Z0-9_.-]*$", RegexOptions.Compiled);

        private readonly IUserService _userService;

        public RegisterViewModelValidator(IUserService userService)
        {
            _userService = userService;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.FirstName).NotEmpty().WithMessage(x => Resources.Registration_FirstName_Can_Not_Be_Empty);

            RuleFor(x => x.LastName).NotEmpty().WithMessage(x => Resources.Registration_LastName_Can_Not_Be_Empty);

            RuleFor(x => x.Username).NotEmpty().WithMessage(x => Resources.Registration_Username_Can_Not_Be_Empty)
                .Length(MinimumUsernameCharCount, MaximumUsernameCharCount)
                .WithMessage(x => string.Format(Resources.Registration_Username_Invalid_Length, MinimumUsernameCharCount, MaximumUsernameCharCount))
                .Matches(UsernameCharCheck).WithMessage(x => Resources.Registration_Username_Contains_Invalid_Characters).MustAsync(BeUniqueUserName)
                .WithMessage(x => Resources.Registration_Username_Already_Taken);

            RuleFor(x => x.BirthDate).BirthDate().WithMessage(x => Resources.Registration_BirthDate_Must_Be_Be_Valid_Age);

            RuleFor(x => x.Password).NotEmpty().WithMessage(x => Resources.Registration_Password_Can_Not_Be_Empty);

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(x => Resources.Registration_ConfirmPassword_Can_Not_Be_Empty)
                .Equal(customer => customer.Password).WithMessage(x => Resources.Registration_ConfirmPassword_Not_Match_Password);
        }

        private async Task<bool> BeUniqueUserName(string username, CancellationToken cancellationToken)
        {
            var result = await _userService.IsUsernameUniqueAsync(username, cancellationToken).ConfigureAwait(false);

            return result.Result;
        }
    }
}
