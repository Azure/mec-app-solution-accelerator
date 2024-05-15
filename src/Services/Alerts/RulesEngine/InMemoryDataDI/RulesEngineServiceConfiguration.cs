using MediatR;
using RulesEngine.Configuration;
using System.Data;
using System.Reflection;

namespace RulesEngine.InMemoryDataDI
{
    public static class RulesEngineServiceConfiguration
    {
        public static void AddRulesEngineConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var rulesConfig = configuration.GetSection("AlertsClassesConfig").Get<AlertsClassesConfig>();

            AddRulesCommandsDictionary(services);
            AddRulesConfigurationDictionary(services, rulesConfig);
        }

        private static void AddRulesConfigurationDictionary(IServiceCollection services, AlertsClassesConfig configuration)
        {
            var alertsConfigByClass = new Dictionary<string, IEnumerable<AlertsConfig>>();
            foreach (var classConfig in configuration.ClassesConfig)
            {
                var alertsConfigs = new List<AlertsConfig>();
                foreach (var alertName in classConfig.Alerts)
                {
                    alertsConfigs.Add(configuration.AlertsConfig.First(alertConfig => alertConfig.AlertName == alertName));
                }
                alertsConfigByClass.Add(classConfig.Name, alertsConfigs);
            }

            services.AddSingleton(alertsConfigByClass);
        }

        private static void AddRulesCommandsDictionary(IServiceCollection services)
        {
            var commandsType = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                               from type in assembly.GetTypes()
                               where typeof(IRequest<bool>).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract
                               select type;

            var types = new Dictionary<string, Type>();
            foreach (var commandType in commandsType)
            {
                var tag = commandType.GetCustomAttribute<RuleTagAttribute>();
                if (tag != null)
                {
                    types.Add(tag.Name, commandType);
                }
            }

            services.AddSingleton(types);
        }
    }
}
