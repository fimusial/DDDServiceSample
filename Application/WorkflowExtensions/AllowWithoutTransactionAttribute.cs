using System;

namespace Application;

[AttributeUsage(AttributeTargets.Method)]
public class AllowWithoutTransactionAttribute : Attribute
{
}