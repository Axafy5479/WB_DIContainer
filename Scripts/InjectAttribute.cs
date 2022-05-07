using System;

namespace WB.DI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public class InjectAttribute : Attribute{}
}
