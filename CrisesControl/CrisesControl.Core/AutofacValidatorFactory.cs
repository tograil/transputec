using System;
using Autofac;
using FluentValidation;

namespace CrisesControl.Core
{
    public class AutofacValidatorFactory : ValidatorFactoryBase
    {
        private readonly IComponentContext _context;

        public AutofacValidatorFactory(IComponentContext context)
        {
            _context = context;
        }

        public override IValidator? CreateInstance(Type validatorType)
        {
            if (_context.TryResolve(validatorType, out var instance))
            {
                var validator = instance as IValidator;
                return validator;
            }

            return null;
        }
    }
}