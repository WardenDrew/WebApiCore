using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApiCore.Models;

namespace WebApiCore.Extensions
{
	public static class IServiceCollectionExtensions
	{
		public static IServiceCollection AddWebApiCore(this IServiceCollection services)
		{
			return services.AddWebApiCore(options =>
            {
				options = new WebApiCoreOptions();
            });
		}

		public static IServiceCollection AddWebApiCore(this IServiceCollection services, Action<WebApiCoreOptions> options)
		{
			// Build options here to get properties needed for service configuration
			WebApiCoreOptions webApiCoreOptions = new();
			options.Invoke(webApiCoreOptions);
			
			// Add WebApiCoreOptions to IOptions services
			services.Configure(options);

			// Configure MediatR if assemblies were provided
			if (webApiCoreOptions.MediatRAssembliesToScan.Count > 0)
            {
				if (webApiCoreOptions.MediatRServiceConfiguration is not null)
                {
					services.AddMediatR(webApiCoreOptions.MediatRServiceConfiguration, webApiCoreOptions.MediatRAssembliesToScan.ToArray());
                }
				else
                {
					services.AddMediatR(webApiCoreOptions.MediatRAssembliesToScan.ToArray());
                }
            }

			// Add our assembly to the list to scan for services if its not already there
			if (!webApiCoreOptions.ServiceScanningAssembliesToScan.Contains(WebApiCore.AssemblyMarker.Assembly))
			{
				webApiCoreOptions.ServiceScanningAssembliesToScan.Add(WebApiCore.AssemblyMarker.Assembly);
			}

			// Scan assemblies for services
			services.ScanAssemblies(webApiCoreOptions.ServiceScanningAssembliesToScan.ToArray());
			
			return services;
		}

		public static IServiceCollection ScanAssemblies(this IServiceCollection services, params Assembly[] assemblies)
		{
			Dictionary<Type, Type> scopedServices = LocateTypes(typeof(IServiceScanningScopedImplementation), assemblies);
			Dictionary<Type, Type> transientServices = LocateTypes(typeof(IServiceScanningTransientImplementation), assemblies);
			Dictionary<Type, Type> singletonServices = LocateTypes(typeof(IServiceScanningSingletonImplementation), assemblies);

			foreach (KeyValuePair<Type, Type> scoped in scopedServices)
			{
				services.AddScoped(scoped.Key, scoped.Value);
			}

			foreach (KeyValuePair<Type, Type> transient in transientServices)
			{
				services.AddTransient(transient.Key, transient.Value);
			}

			foreach (KeyValuePair<Type, Type> singleton in singletonServices)
			{
				services.AddTransient(singleton.Key, singleton.Value);
			}

			return services;
		}

		public static IServiceCollection ScanAssemblies(this IServiceCollection services, params Type[] assemblyMarkers)
		{
			List<Assembly> assemblies = new();

			foreach (Type assemblyMarker in assemblyMarkers)
			{
				if (!assemblies.Contains(assemblyMarker.Assembly))
				{
					assemblies.Add(assemblyMarker.Assembly);
				}
			}

			return ScanAssemblies(services, assemblies.ToArray());
		}

		private static Dictionary<Type, Type> LocateTypes(Type implementationInterface, params Assembly[] assemblies)
		{
			Dictionary<Type, Type> result = new();

			foreach (Assembly assembly in assemblies)
			{
				IEnumerable<TypeInfo> implementationTypes = assembly.DefinedTypes.Where(x =>
					implementationInterface.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

				foreach (TypeInfo implementationType in implementationTypes)
				{
					// Locate the service interface

					List<Type> serviceTypes = implementationType.GetInterfaces().Where(x =>
						x != typeof(IServiceScanningServiceInterface) &&
						typeof(IServiceScanningServiceInterface).IsAssignableFrom(x) &&
						x.IsInterface).ToList();

					if (serviceTypes.Count > 1)
					{
						throw new AmbiguousMatchException($"Service implementation \"{implementationType.FullName}\" inherits multiple interfaces that are inheriting from {nameof(IServiceScanningServiceInterface)}. Cannot determine which interface is the service Interface.");
					}

					Type? serviceType = serviceTypes.FirstOrDefault();

					if (serviceType is null)
					{
						throw new NoMatchException($"Service implementation \"{implementationType.FullName}\" does not inherit an interface that inherits {nameof(IServiceScanningServiceInterface)}.");
					}

					result.Add(serviceType, implementationType);
				}
			}

			return result;
		}
	}
}
