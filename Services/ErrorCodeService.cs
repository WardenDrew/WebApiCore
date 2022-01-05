using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiCore.Models;

namespace WebApiCore.Services
{
	public interface IErrorCodeService : IServiceScanningServiceInterface
	{
		public List<IErrorCode> ScanErrorCodeProviders(params Assembly[] assemblies);
		public List<IErrorCode> ScanErrorCodeProviders(params Type[] assemblyMarkers);
	}

	public class ErrorCodeService : IErrorCodeService, IServiceScanningScopedImplementation
	{
		public List<IErrorCode> ScanErrorCodeProviders(params Assembly[] assemblies)
		{
			List<IErrorCode> result = new();

			foreach (Assembly assembly in assemblies)
			{
				IEnumerable<TypeInfo> errorCodeProviderTypes = assembly.DefinedTypes.Where(x =>
					typeof(IErrorCodeProvider).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

				foreach (TypeInfo providerType in errorCodeProviderTypes)
				{
					if (providerType.ContainsGenericParameters)
					{
						throw new InvalidOperationException($"Error Code provider classes must not be contained inside a generic class as generic classes cannot be instantiated through reflection: {providerType.FullName}");
					}
					
					List<object?> fieldValues = providerType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
						.Where(x => typeof(IErrorCode).IsAssignableFrom(x.FieldType))
						.Where(x => x.IsInitOnly)
						.Select(x => x.GetValue(null))
						.ToList();

					result.AddRange(fieldValues
						.Where(x => x is not null) // filter null field values
						.Select(x => x as IErrorCode) // cast to IErrorCode, returns null if cast failed
						.Where(x => x is not null) // filter failed casts
						.Select(x => x!)); // make the compiler happy that we've filtered null values already
				}
			}

			return result;
		}

		public List<IErrorCode> ScanErrorCodeProviders(params Type[] assemblyMarkers)
		{
			List<Assembly> assemblies = new();

			foreach (Type assemblyMarker in assemblyMarkers)
			{
				if (!assemblies.Contains(assemblyMarker.Assembly))
				{
					assemblies.Add(assemblyMarker.Assembly);
				}
			}

			return ScanErrorCodeProviders(assemblies.ToArray());
		}
	}
}
