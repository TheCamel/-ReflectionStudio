using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace ReflectionStudio.Spy.Timer
{
	[SuppressUnmanagedCodeSecurity, HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	internal class NativeMethods
	{
		[DllImport("kernel32.dll")]
		public static extern bool QueryPerformanceCounter(out long value);
		[DllImport("kernel32.dll")]
		public static extern bool QueryPerformanceFrequency(out long value);
	}
}
