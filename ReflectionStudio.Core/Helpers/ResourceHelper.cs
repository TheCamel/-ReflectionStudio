using System;
using System.Reflection;
using System.IO;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Core.Helpers
{
	/// <summary>
	/// Description résumée de ResReader.
	/// </summary>
	public class ResourceHelper
	{
		#region lecture de ressources

		/// <summary>
		/// Load the resource specified in the query string and return it
		/// as the HTTP response.
		/// </summary>
		/// <param name="hc">The context object for the request</param>
		public string ReadResource(string strAssembly, string strResName)
		{
			Stream s = null;
			StreamReader sr = null;
			Assembly asm = null;
			string result = string.Empty;

			asm = this.RetreiveResAssembly(ref strResName, strAssembly);

			try
			{
				sr = new StreamReader(asm.GetManifestResourceStream(strResName));
				result = sr.ReadToEnd();
			}
			catch
			{
				Tracer.Error("ResourceReader", "Error retreiving resource {0} in assembly {1}", strResName, strAssembly);
			}
			finally
			{
				if (sr != null)
					sr.Close();
				if (s != null)
					s.Close();
			}

			return result;
		}

		public Stream ReadResourceAsStream(string strAssembly, string strResName)
		{
			Stream s = null;
			Assembly asm = this.RetreiveResAssembly(ref strResName, strAssembly);

			try
			{
				s = asm.GetManifestResourceStream(strResName);
			}
			catch
			{
				Tracer.Error("ResourceReader", "Error retreiving resource {0} in assembly {1}", strResName, strAssembly);
			}

			return s;
		}
		#endregion

		#region trouver l'assembly contenant la ressouce demandée
		/// <summary>
		/// Retourne l'assembly qui contient la ressource cherchée
		/// </summary>
		/// <param name="ResName">Le nom de la resource</param>
		/// <param name="AssemblyName">Le nom de l'assembly </param>
		private Assembly RetreiveResAssembly(ref string ResName, string AssemblyName)
		{
			Assembly asm = null;

			try
			{
				// Get the resource from this assembly or another?
				if (AssemblyName == null)
				{
					asm = Assembly.GetExecutingAssembly();
				}
				else
				{
					Assembly[] asmList = AppDomain.CurrentDomain.GetAssemblies();

					foreach (Assembly a in asmList)
						if (a.GetName().Name == AssemblyName)
						{
							asm = a;
							break;
						}

					if (asm == null)
						throw new ArgumentOutOfRangeException("Assembly",
							AssemblyName, "Assembly not found");

					// Now get the resources listed in the assembly manifest
					// and look for the filename.  Note the fact that it is
					// matched on the filename and not necessarily the path
					// within the assembly.  This may restricts you to using
					// a filename only once, but it also prevents the problem
					// that the VB.NET compiler has where it doesn't seem to
					// output folder names on resources.

					foreach (string strResource in asm.GetManifestResourceNames())
						if (strResource.EndsWith(ResName))
						{
							ResName = strResource;
							break;
						}
				}
			}
			catch (Exception)
			{
				Tracer.Error("ResourceReader", "Error retreiving the assembly containing resource {0} ", ResName);
			}

			return asm;
		}
		#endregion

		#region lecture d'une ressource binaire quelconque
		/// <summary>
		/// Read a binary resource such as an image in from the stream.  Since
		/// the size of the resource cannot be obtained, it is read in a block
		/// at a time until the end of the stream is reached.
		/// </summary>
		/// <param name="sIn">The stream from which to read the resource</param>
		/// <param name="byBuffer">An out parameter to receive the resource bytes</param>
		/// <param name="nLen">An out parameter to receive the resource length</param>
		private void ReadBinaryResource(Stream sIn, out Byte[] byBuffer, out int nLen)
		{
			byte[] byTemp;
			int nBytesRead;

			byBuffer = new Byte[1024];
			nLen = 0;

			do
			{
				if (byBuffer.Length < nLen + 1024)
				{
					byTemp = new Byte[byBuffer.Length * 2];
					Array.Copy(byBuffer, byTemp, byBuffer.Length);
					byBuffer = byTemp;
				}

				nBytesRead = sIn.Read(byBuffer, nLen, 1024);
				nLen += nBytesRead;

			} while (nBytesRead == 1024);
		}
		#endregion
	}
}
