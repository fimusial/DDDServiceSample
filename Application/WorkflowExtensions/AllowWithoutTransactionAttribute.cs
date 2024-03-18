using System;

namespace Application;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AllowWithoutTransactionAttribute : Attribute
{
}