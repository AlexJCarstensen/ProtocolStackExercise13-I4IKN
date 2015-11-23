using System;
using System.IO.Ports;
using Library;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE)
		{
			serialPort = new SerialPort("/dev/ttyS0",115200,Parity.None,8,StopBits.One);

			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)+2];
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
            // TO DO Your own code
            Array.Clear(buffer, 0, buffer.Length);

			serialPort.Write(new[] {DELIMITER}, 0, 1);


			int bufferCounter = 0;

			for (int i = 0; i < size; i++) {
				switch (Convert.ToChar(buf [i])) {
				case 'A':
					buffer [bufferCounter] = Convert.ToByte('B');
					bufferCounter++;
					buffer [bufferCounter] = Convert.ToByte('C');
					bufferCounter++;
					break;
				case 'B':
					buffer [bufferCounter] = Convert.ToByte('B');
					bufferCounter++;
					buffer [bufferCounter] = Convert.ToByte('D');
					bufferCounter++;
					break;
				default:
					buffer [bufferCounter] = buf [i];
					bufferCounter++;
					break;
				}
			}
			serialPort.Write (buffer, 0, bufferCounter);
			serialPort.Write (new [] {DELIMITER}, 0, 1);

		}

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public int receive (ref byte[] buf)
		{
            // TO DO Your own code
            int tempDataCounter = 0;
		    int realDataCounter = 0;

            byte[] tempData = new byte[buf.Length * 2 + 6];

            while (serialPort.ReadChar() != DELIMITER) {}

            do
			{
			    tempData[tempDataCounter] = (byte)serialPort.ReadByte();
				tempDataCounter++;
			}while(tempData[tempDataCounter - 1] != DELIMITER);

			tempDataCounter--;

		    for (int i = 0; i < tempDataCounter; i++) if (tempData[i] != 'B') realDataCounter++;

		    tempDataCounter = 0;
		    for (int i = 0; i < realDataCounter; i++)
		    {
		        if (tempData[tempDataCounter] == 'B' && tempData[tempDataCounter + 1] == 'C')
		        {
		            buf[i] = Convert.ToByte(DELIMITER);
		            tempDataCounter += 2;
		        } else if (tempData[tempDataCounter] == 'B' && tempData[tempDataCounter + 1] == 'D')
		        {
		            buf[i] = Convert.ToByte('B');
		            tempDataCounter += 2;
		        }
		        else
		        {
		            buf[i] = tempData[tempDataCounter];
		            tempDataCounter++;
		        }
		    }
		    return realDataCounter;
		}
	}
}
