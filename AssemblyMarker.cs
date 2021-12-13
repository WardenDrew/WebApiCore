using System.Reflection;

namespace WebApiCore
{
	public static class AssemblyMarker
	{
		public static Assembly Assembly
			=> typeof(AssemblyMarker).Assembly;
	}
}
