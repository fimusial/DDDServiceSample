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
        if (Target == null)
        {
            throw new InvalidOperationException($"{nameof(UnitOfWorkSafeguard<TInterface>)} requires {nameof(Target)} to be set");
        }

        if (UnitOfWork == null)
        {
            throw new InvalidOperationException($"{nameof(UnitOfWorkSafeguard<TInterface>)} requires {nameof(UnitOfWork)} to be set");
        }

        if (!UnitOfWork.HasOngoingTransaction)
        {
            if (Target.GetType().GetMethod(targetMethod!.Name)!.GetCustomAttribute<AllowWithoutTransactionAttribute>() == null)
            {
                throw new InvalidOperationException($"operation {typeof(TInterface).Name}.{targetMethod.Name} requires {nameof(UnitOfWork.HasOngoingTransaction)} to be true");
            }
        }

        return targetMethod!.Invoke(Target, args);
    }

    public static TInterface CreateProxy(TInterface target, IUnitOfWork unitOfWork)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (unitOfWork == null)
        {
            throw new ArgumentNullException(nameof(unitOfWork));
        }

        var proxy = Create<TInterface, UnitOfWorkSafeguard<TInterface>>() as UnitOfWorkSafeguard<TInterface>;

        if (proxy == null)
        {
            throw new InvalidOperationException($"could not create a proxy: {nameof(UnitOfWorkSafeguard<TInterface>)}");
        }

        proxy.Target = target;
        proxy.UnitOfWork = unitOfWork;

        return (proxy as TInterface)!;
    }
}