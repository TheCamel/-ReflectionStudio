using System;
using System.Security.Permissions;
using System.Security;
using System.Runtime.InteropServices;

namespace Calibrate.Core
{
	public class LocalTimer
	{
		// Fields
		private long elapsed;
		private readonly long Frequency;
		private readonly bool IsHighResolution;
		private bool isRunning;
		private long startTimeStamp;
		private readonly double tickFrequency;
		private const long TicksPerMillisecond = 0x2710L;
		private const long TicksPerSecond = 0x989680L;

		#region ----------------SINGLETON----------------
		public static readonly LocalTimer Instance = new LocalTimer();

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private LocalTimer()
		{
			this.elapsed = 0L;
			this.isRunning = false;
			this.startTimeStamp = 0L;

			if (!NativeMethods.QueryPerformanceFrequency(out Frequency))
			{
				IsHighResolution = false;
				Frequency = 0x989680L;
				tickFrequency = 1.0;
			}
			else
			{
				IsHighResolution = true;
				tickFrequency = 10000000.0;
				tickFrequency /= (double)Frequency;
			}

			this.Start();
		}
		#endregion
		

		private long GetElapsedDateTimeTicks()
		{
			long rawElapsedTicks = this.GetRawElapsedTicks();
			if (IsHighResolution)
			{
				double num2 = rawElapsedTicks;
				num2 *= tickFrequency;
				return (long) num2;
			}
			return rawElapsedTicks;
		}

		private long GetRawElapsedTicks()
		{
			long elapsed = this.elapsed;
			if (this.isRunning)
			{
				long num3 = GetTimestamp() - this.startTimeStamp;
				elapsed += num3;
			}
			return elapsed;
		}

		public long GetTimestamp()
		{
			if (IsHighResolution)
			{
				long num = 0L;
				NativeMethods.QueryPerformanceCounter(out num);
				return num;
			}
			return DateTime.UtcNow.Ticks;
		}

		public void Start()
		{
			if (!this.isRunning)
			{
				this.startTimeStamp = GetTimestamp();
				this.isRunning = true;
			}
		}

		public void Stop()
		{
			if (this.isRunning)
			{
				long num2 = GetTimestamp() - this.startTimeStamp;
				this.elapsed += num2;
				this.isRunning = false;
			}
		}

		// Properties
		public TimeSpan Elapsed
		{
			get
			{
				return new TimeSpan(this.GetElapsedDateTimeTicks());
			}
		}

		public long ElapsedMilliseconds
		{
			get
			{
				return (this.GetElapsedDateTimeTicks() / 0x2710L);
			}
		}

		public long ElapsedTicks
		{
			get
			{
				return this.GetRawElapsedTicks();
			}
		}

		public bool IsRunning
		{
			get
			{
				return this.isRunning;
			}
		}

		[SuppressUnmanagedCodeSecurity, HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
		internal class NativeMethods
		{
			[DllImport("kernel32.dll")]
			public static extern bool QueryPerformanceCounter(out long value);
			[DllImport("kernel32.dll")]
			public static extern bool QueryPerformanceFrequency(out long value);
		}
	}
}
