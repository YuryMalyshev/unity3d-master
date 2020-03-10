using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AdvectionCalc
{
	class Program
	{
		static void Main_1(string[] args)
		{
			
			int lineSize = sizeof(int) + 30 * sizeof(double);
			Console.WriteLine(lineSize);
			Random rand = new Random();

			List<object> checkList = new List<object>();

			for (int j = 0; j < 2; j++)
			{
				byte[] bytes_temp = new byte[0];
				int ID = j*256;
				checkList.Add(ID);
				bytes_temp = bytes_temp.Concat(BitConverter.GetBytes(ID)).ToArray();
				for (int i = 0; i < 30; i++)
				{
					double num = rand.NextDouble();
					Console.WriteLine(num);
					checkList.Add(num);
					bytes_temp = bytes_temp.Concat(BitConverter.GetBytes(num)).ToArray();
				}

				using (FileStream
				fileStream = new FileStream("byteStream", FileMode.Append))
				{
					for (int k = 0; k < bytes_temp.Length; k++)
					{
						fileStream.WriteByte(bytes_temp[k]);
					}
				}
			}

			Thread.Sleep(5000);
			List<object> checkedList = new List<object>();

			byte[] bytes;
			using (FileStream
				fileStream = new FileStream("byteStream", FileMode.Open))
			{
				// Set the stream position to the beginning of the file.
				fileStream.Seek(0, SeekOrigin.Begin);

				// Read the data.
				bytes = new byte[fileStream.Length];
				for (int i = 0; i < fileStream.Length; i++)
				{
					bytes[i] = (byte)fileStream.ReadByte();
				}
			}


			for (int j = 0; j < bytes.Length; j += lineSize)
			{
				int ConvertedID = BitConverter.ToInt32(bytes, j);
				checkedList.Add(ConvertedID);
				Console.WriteLine(ConvertedID);
				for (int i = j+sizeof(int); i < j + lineSize; i += sizeof(double))
				{
					double num = BitConverter.ToDouble(bytes, i);
					checkedList.Add(num);
					Console.WriteLine(num);
				}
			}

			if(Enumerable.SequenceEqual(checkList, checkedList))
			{
				Console.WriteLine("Congrats!");
			}
			else
			{
				Console.WriteLine("Not equal!");
			}

			Thread.Sleep(5000);
		}
	}
}
