using System;
using System.Globalization;
using System.Text;

namespace ReflectionStudio.Core.Helpers
{
	internal class ByteHelper
	{
		/// <summary>
		/// Convert an array of byte to a hex-string
		/// </summary>
		/// <param name="input">array to convert</param>
		/// <returns>resulting hex-string</returns>
		public static string ByteToHexString(Byte[] input)
		{
			if (input != null)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < input.Length; i++)
				{
					sb.Append(input[i].ToString("x2"));
				}
				return sb.ToString();
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Convert a hex-string to an array of byte
		/// </summary>
		/// <param name="input">hex-string to convert</param>
		/// <returns>resulting array</returns>
		public static byte[] HexStringToByte(string input)
		{
			byte[] result = new byte[input.Length / 2];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = Byte.Parse(input.Substring(i * 2, 2), NumberStyles.HexNumber);
			}
			return result;
		}
	}
}
