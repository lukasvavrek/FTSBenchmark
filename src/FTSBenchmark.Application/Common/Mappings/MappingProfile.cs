using System.Reflection;
using AutoMapper;

namespace FTSBenchmark.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var typeSelector = new Func<Type, bool>(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>));
        
        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(typeSelector))
            .ToList();

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod("Mapping") ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");
            methodInfo?.Invoke(instance, new object[] { this });
        }
    }
}