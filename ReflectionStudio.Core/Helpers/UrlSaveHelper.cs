using System;
using System.IO;
using System.Net;
using System.Text;
using ReflectionStudio.Core.Events;

namespace ReflectionStudio.Core.Helpers
{
	/// <summary>
	/// UrlSaveHelper allow to save the response of any HTTP GET in a synchrone way. For async, use AsyncUrlSaveHelper
	/// </summary>
	public class UrlSaveHelper
	{
		private string _webUrl;
		private string _destinationFile;

		/// <summary>
		/// Constructor, init the context
		/// </summary>
		/// <param name="url"></param>
		/// <param name="destinationFile"></param>
		public UrlSaveHelper(string url, string destinationFile)
		{
			_webUrl = url;
			_destinationFile = destinationFile;
		}

		/// <summary>
		/// Save the response of any HTTP GET in a synchrone way
		/// </summary>
		/// <returns></returns>
		public bool SaveUrlContent()
		{
			try
			{
				Tracer.Verbose("UrlSaveHelper.SaveUrlContent", "Try to save resource {0} into {1}", _webUrl, _destinationFile);

				// used to build entire input
				StringBuilder sb = new StringBuilder();

				// used on each read operation
				byte[] buf = new byte[8192];

				// prepare the web page we will be asking for
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_webUrl);

				// execute the request
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				// we will read data via the response stream
				Stream resStream = response.GetResponseStream();

				string tempString = null;
				int count = 0;

				do
				{
					// fill the buffer with data
					count = resStream.Read(buf, 0, buf.Length);

					// make sure we read some data
					if (count != 0)
					{
						// translate from bytes to ASCII text
						tempString = Encoding.ASCII.GetString(buf, 0, count);

						// continue building the string
						sb.Append(tempString);
					}
				}
				while (count > 0); // any more data to read?

				//sb.Remove(0, 3);
				File.WriteAllText(_destinationFile, sb.ToString());
			}
			catch (Exception all)
			{
				Tracer.Error("UrlSaveHelper.SaveUrlContent", all);
				Tracer.Verbose("UrlSaveHelper.SaveUrlContent", "Error retreiving resource {0} to save it into {1}", _webUrl, _destinationFile);
				return false;
			}

			Tracer.Verbose("UrlSaveHelper.SaveUrlContent", "Resource {0} saved it into {1}", _webUrl, _destinationFile);
			return true;
		}
	}
}
