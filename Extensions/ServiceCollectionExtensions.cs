using System.Reflection;

namespace _40Let.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers every concrete "*Service" class against its matching interface
    /// (e.g. BotUserService -> IBotUserService) found in the given assembly.
    /// </summary>
    public static IServiceCollection AddServicesByConvention(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var serviceTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.Name.EndsWith("Service"));

        foreach (var implementation in serviceTypes)
        {
            // "get the interface by type": find the interface this class implements
            // whose name is "I" + the class name.
            var serviceInterface = implementation.GetInterfaces()
                .FirstOrDefault(i => i.Name == "I" + implementation.Name);

            if (serviceInterface is null)
                continue;

            services.Add(new ServiceDescriptor(serviceInterface, implementation, lifetime));
        }

        return services;
    }
}
