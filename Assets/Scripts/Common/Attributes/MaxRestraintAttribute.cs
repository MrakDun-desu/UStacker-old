using System;

namespace UStacker.Common.Attributes
{
    public class MaxRestraintAttribute : Attribute
    {
        public MaxRestraintAttribute(double value, bool useForValidation)
        {
            Value = value;
            UseForValidation = useForValidation;
        }

        public double Value { get; }
        public bool UseForValidation { get; }
    }
}