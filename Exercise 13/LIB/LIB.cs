using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Library
{
	public class LIB
	{
		private LIB ()
		{
		}

		/// <summary>
		/// Extracts the name of the file.
		/// </summary>
		/// <returns>
		/// The filename only.
		/// </returns>
		/// <param name='fileName'>
		/// Filename with path.
		/// </param>
		public static String extractFileName(String fileName)
    	{
    		return (fileName.LastIndexOf('/')==0 ? fileName : fileName.Substring(fileName.LastIndexOf('/')+1));
    	}

		/// <summary>
		/// Check_s the file_ exists.
		/// </summary>
		/// <returns>
		/// The filesize.
		/// </returns>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		public static long check_File_Exists (String fileName)
		{
			if (File.Exists (fileName))
				return (new FileInfo(fileName)).Length;

			return 0;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetByteArray(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}

