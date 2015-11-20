using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using System.IO.Ports;
using System.Runtime.Remoting.Channels;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
            SerialPort serialPort = new SerialPort("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);

            if (!serialPort.IsOpen)
                serialPort.Open();
            byte[] bytes = new byte[1000];
            Console.WriteLine("Serial port opened...");
            Console.WriteLine(serialPort.Read(bytes, 0, 1));

            Console.WriteLine("Write a string to terminate");
            var input = Console.ReadLine();
            if (serialPort.IsOpen)
                serialPort.Close();

            // TO DO Your own code
            /*byte[] fileName = new byte[BUFSIZE];
			Transport transport = new Transport(BUFSIZE);

            Console.WriteLine("Waiting for client...");
		    if(transport.receive(ref fileName) < 1) return;

            Console.WriteLine("Server recieved " + LIB.GetString(fileName) + " from client.");
		    long fileSize = LIB.check_File_Exists(LIB.GetString(fileName));

            SendFile(LIB.GetString(fileName), fileSize, transport);*/
        }

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void SendFile(String fileName, long fileSize, Transport transport)
		{
            // TO DO Your own code
            byte[] file = new byte[BUFSIZE];
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            BinaryReader binReader = new BinaryReader(fileStream);

            bool run = true;
            while (run)
            {
                file = binReader.ReadBytes(BUFSIZE);
                transport.send(file, file.Length);
                fileSize -= BUFSIZE;
                if (fileSize < BUFSIZE)
                {
                    file = binReader.ReadBytes((int)fileSize);
                    transport.send(file, file.Length);
                    run = false;
                }


            }
            binReader.Close();
            fileStream.Close();
        }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			new file_server();
		}
	}
}
