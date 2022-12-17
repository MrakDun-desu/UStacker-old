using System;

namespace UStacker.Common.Attributes
{
    public class MinRestraintAttribute : Attribute
    {
        public MinRestraintAttribute(double value, bool useForValidation)
        {
            Value = value;
            UseForValidation = useForValidation;
        }

        public double Value { get; }
        public bool UseForValidation { get; }
    }
}