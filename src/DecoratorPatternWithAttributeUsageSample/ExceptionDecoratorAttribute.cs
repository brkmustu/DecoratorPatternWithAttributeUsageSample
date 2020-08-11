using System;

namespace DecoratorPatternWithAttributeUsageSample
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ExceptionDecoratorAttribute : Attribute
    {
    }
}
