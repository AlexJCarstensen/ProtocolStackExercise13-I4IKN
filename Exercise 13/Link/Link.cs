using System;
using System.IO.Ports;

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
			// Create a new SerialPort object with default settings.
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);

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
            char[] startChar = { Convert.ToChar(DELIMITER) };

			serialPort.Write(DELIMITER.ToString());
			/*if(serialPort.WriteBufferSize != 1){
				Console.WriteLine (this.GetType().Name + ": Serial port failed to write " + startChar.ToString());
				return;
			}*/


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
			if(serialPort.WriteBufferSize != bufferCounter){
				Console.WriteLine (this.GetType().Name + ": Serial port failed to write " + buffer.ToString());
				//return;
			}

			serialPort.Write (startChar, 0, 1);
			if(serialPort.WriteBufferSize != 1){
				Console.WriteLine (this.GetType().Name + ": Serial port failed to write " + startChar.ToString());
				//return;
			}

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
            byte[] startChar = new byte[1];
            byte[] tempData = new byte[buf.Length * 2 + 6];

            do
            {
				serialPort.Read(startChar, 0, 1);
			} while(Convert.ToChar(startChar[0]) != 'A');

			
			do{
				serialPort.Read(tempData, tempDataCounter, 1);
				tempDataCounter++;
			}while(Convert.ToChar(tempData[tempDataCounter - 1]) != 'A');

			tempDataCounter--;

		    for (int i = 0; i < tempDataCounter; i++) if (tempData[i] != 'B') realDataCounter++;

		    tempDataCounter = 0;
		    for (int i = 0; i < realDataCounter; i++)
		    {
		        if (tempData[tempDataCounter] == 'B' && tempData[tempDataCounter + 1] == 'C')
		        {
		            buf[i] = Convert.ToByte('A');
		            tempDataCounter += 2;
		        } else if (tempData[tempDataCounter] == 'B' && tempData[tempDataCounter + 1] == 'D')
		        {
		            buf[i] = Convert.ToByte('B');
		            tempDataCounter += 2;
		        }
		        else
		        {
		            buf[i] = tempData[tempDataCounter];
		            realDataCounter++;
		        }
		    }
            Console.WriteLine("From: " + this.GetType().Name + " recieved " + buf.ToString());
		    return realDataCounter;
		}
	}
}
