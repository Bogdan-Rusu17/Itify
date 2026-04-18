using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MobyLabWebProgramming.Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyConfigurationsFromAssemblies(this ModelBuilder modelBuilder, Assembly[] assemblies, IServiceProvider? serviceProvider = null)
    {
        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        serviceProvider ??= new ServiceCollection().BuildServiceProvider();
        
        var configurations = assemblies.SelectMany(e => e.GetTypes()).Where(e => e is { IsClass: true, IsAbstract: false, IsGenericType: false })
            .Where(e => e.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
            .ToList();

        var applyEntityConfigurationMethod = typeof(ModelBuilder)
            .GetMethods()
            .Single(e => e is { Name: nameof(ModelBuilder.ApplyConfiguration), ContainsGenericParameters: true }
                         && e.GetParameters().FirstOrDefault()?.ParameterType.GetGenericTypeDefinition()
                         == typeof(IEntityTypeConfiguration<>));

        foreach (var configuration in configurations)
        {
            var instance = ActivatorUtilities.CreateInstance(serviceProvider, configuration);

            var applyMethod = applyEntityConfigurationMethod.MakeGenericMethod(configuration.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                .GetGenericArguments().First());

            applyMethod.Invoke(modelBuilder, [instance]);
        }
    }
}