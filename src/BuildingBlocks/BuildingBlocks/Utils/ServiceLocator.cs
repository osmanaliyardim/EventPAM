using Microsoft.Extensions.DependencyInjection;

namespace EventPAM.BuildingBlocks.Utils;

public class ServiceLocator
{
    private IServiceProvider _currentServiceProvider;
    private static IServiceProvider _serviceProvider = default!;

    public ServiceLocator(IServiceProvider currentServiceProvider)
    {
        _currentServiceProvider = currentServiceProvider;
    }

    public static ServiceLocator Current
    {
        get
        {
            return new ServiceLocator(_serviceProvider);
        }
    }

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object GetInstance(Type serviceType)
    {
        return _currentServiceProvider.GetService(serviceType)!;
    }

    public TService GetInstance<TService>()
    {
        return _currentServiceProvider.GetService<TService>()!;
    }
}
