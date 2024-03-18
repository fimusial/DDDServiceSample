using System;
using System.Reflection;

namespace Application;

public class UnitOfWorkSafeguard<TInterface> : DispatchProxy
    where TInterface : class
{
    private TInterface Target { get; set; } = null!;
    private IUnitOfWork UnitOfWork { get; set; } = null!;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (Target is null)
        {
            throw new InvalidOperationException($"{nameof(UnitOfWorkSafeguard<TInterface>)} requires {nameof(Target)} to be set");
        }

        if (UnitOfWork is null)
        {
            throw new InvalidOperationException($"{nameof(UnitOfWorkSafeguard<TInterface>)} requires {nameof(UnitOfWork)} to be set");
        }

        if (!UnitOfWork.HasOngoingTransaction)
        {
            if (Target.GetType().GetMethod(targetMethod!.Name)!.GetCustomAttribute<AllowWithoutTransactionAttribute>() is null)
            {
                throw new InvalidOperationException($"operation {typeof(TInterface).Name}.{targetMethod.Name} requires {nameof(UnitOfWork.HasOngoingTransaction)} to be true");
            }
        }

        return targetMethod!.Invoke(Target, args);
    }

    public static TInterface CreateProxy(TInterface target, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        var proxy = Create<TInterface, UnitOfWorkSafeguard<TInterface>>() as UnitOfWorkSafeguard<TInterface>;

        if (proxy is null)
        {
            throw new InvalidOperationException($"could not create a proxy: {nameof(UnitOfWorkSafeguard<TInterface>)}");
        }

        proxy.Target = target;
        proxy.UnitOfWork = unitOfWork;

        return (proxy as TInterface)!;
    }
}