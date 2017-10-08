﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using FluentValidation;
using FluentValidation.Results;
using JetBrains.Annotations;

namespace Diploma.Core.Framework.Validations
{
    public abstract class ValidatableScreen<TProperty, TValidator> : Screen, INotifyDataErrorInfo
        where TProperty : ValidatableScreen<TProperty, TValidator>
        where TValidator : class, IValidator<TProperty>
    {
        private readonly TProperty _target;

        private readonly TValidator _validator;

        private ValidationResult _validationResult;

        protected ValidatableScreen([NotNull] TValidator validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _target = (TProperty)this;
            Validate();
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _validationResult.Errors.Count > 0;

        public IEnumerable GetErrors(string propertyName)
        {
            return _validationResult.Errors.Where(x => x.PropertyName == propertyName)
                .Select(x => x.ErrorMessage);
        }

        public bool Validate()
        {
            _validationResult = _validator.Validate(_target);

            foreach (var error in _validationResult.Errors)
            {
                OnUIThread(() => RaiseErrorsChanged(error.PropertyName));
            }

            NotifyOfPropertyChange(nameof(HasErrors));
            return !HasErrors;
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            handler?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
