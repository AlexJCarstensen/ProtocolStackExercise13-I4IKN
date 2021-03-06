using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using System.IO.Ports;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		const int BUFSIZE = 1000;


		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(String[] args)
	    {
            // TO DO Your own code
            Transport transport = new Transport(BUFSIZE);
		    string file = @"/home/ikn/Desktop/test.png";
            byte[] fileName = LIB.GetByteArray(file);
            transport.send(fileName, fileName.Length);

		    receiveFile(LIB.extractFileName(file), transport);
        }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (string fileName, Transport transport)
		{
            // TO DO Your own code
		    byte[] recieveSize = new byte[100];
		    int recievedSizeLength = transport.receive(ref recieveSize);
            long fileSize = long.Parse(LIB.GetString(recieveSize));

			if (fileSize < 1) {
				Console.WriteLine ("File not found on server");
				return;
			}

            Console.WriteLine(fileSize);
            string filePath = @"/home/ikn/Desktop/" + fileName;


            FileStream fileStream = new FileStream(filePath, FileMode.Append);
            BinaryWriter binWriter = new BinaryWriter(fileStream);

            bool run = true;

            while (run)
            {
                long size;
                if (fileSize < BUFSIZE)
                    size = fileSize;
                else
                    size = BUFSIZE;

                fileSize -= size;


                byte[] byteFile = new byte[size];

                transport.receive(ref byteFile);

                binWriter.Write(byteFile);


                if (fileSize < 1)
                {
                    run = false;
                }
            }
            binWriter.Close();
            fileStream.Close();
        }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
			new file_client(args);
		}
	}
}
