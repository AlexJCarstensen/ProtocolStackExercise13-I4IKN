using System;
using Linklaget;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
	/// <summary>
	/// Transport.
	/// </summary>
	public class Transport
	{
		/// <summary>
		/// The link.
		/// </summary>
		private Link link;
		/// <summary>
		/// The 1' complements checksum.
		/// </summary>
		private Checksum checksum;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
		private byte seqNo;
		/// <summary>
		/// The old_seq no.
		/// </summary>
		private byte old_seqNo;
		/// <summary>
		/// The error count.
		/// </summary>
		private int errorCount;
		/// <summary>
		/// The DEFAULT_SEQNO.
		/// </summary>
		private const int DEFAULT_SEQNO = 2;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private bool receiveAck()
		{
			byte[] buf = new byte[(int)TransSize.ACKSIZE];
			int size = link.receive(ref buf);
			if (size != (int)TransSize.ACKSIZE) return false;
			if(!checksum.checkChecksum(buf, (int)TransSize.ACKSIZE) ||
					buf[(int)TransCHKSUM.SEQNO] != seqNo ||
					buf[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				return false;

            seqNo = (byte)((Convert.ToInt32(buffer[(int)TransCHKSUM.SEQNO]) + 1) % 2);

            return true;
		}

		/// <summary>
		/// Sends the ack.
		/// </summary>
		/// <param name='ackType'>
		/// Ack type.
		/// </param>
		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
					(ackType ? buffer [(int)TransCHKSUM.SEQNO] : (Convert.ToInt32(buffer[(int)TransCHKSUM.SEQNO]) + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);

			link.send(ackBuf, (int)TransSize.ACKSIZE);
		}

		/// <summary>
		/// Send the specified buffer and size.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send(byte[] buf, int size)
		{
			// TO DO Your own code
            Array.Clear(buffer,0 , buffer.Length);
			do {
				for(int i = 0; i < size; i++)
				{
					buffer[i+4] = buf[i];
				}

				buffer[(int)TransCHKSUM.SEQNO] = seqNo;
				buffer[(int)TransCHKSUM.TYPE] = (byte)TransType.DATA;

				checksum.calcChecksum(ref buffer, size + 4);

				link.send (buffer, size + 4);

			} while(!receiveAck());
		}

		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			// TO DO Your own code
			while (true) {
				int recievedSize = link.receive (ref buffer);
				while(!checksum.checkChecksum(buffer, recievedSize)){

					sendAck(false);

					recievedSize = link.receive (ref buffer);
					Console.WriteLine (this.GetType().Name + ": checksum error. Error count = " + errorCount++);
				}

				sendAck (true);

				if (seqNo == buffer [(int)TransCHKSUM.SEQNO]) {
					seqNo = (byte)((Convert.ToInt32(buffer[(int)TransCHKSUM.SEQNO]) + 1) % 2);

					for (int i = 0; i < recievedSize; i++) {
						buf [i] = buffer [i + 4];
					}
					return recievedSize - 4;
				}
			}
		}
	}
}

