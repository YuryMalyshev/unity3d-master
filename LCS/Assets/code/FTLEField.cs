using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FTLEField
{
	public readonly double[,] field;

	public FTLEField(string filename)
	{
		byte[] bytes = File.ReadAllBytes(filename);
		int index = 0;
		int rows = BitConverter.ToInt32(bytes, index);
		Debug.Log(rows);
		index += sizeof(int);
		int cols = BitConverter.ToInt32(bytes, index);
		Debug.Log(cols);
		index += sizeof(int);
		int r = 0;
		int c = 0;
		field = new double[rows, cols];
		double min = double.PositiveInfinity;
		double max = double.NegativeInfinity;
		for (int i = index; i < bytes.Length; i += sizeof(double))
		{
			try
			{
				double val = BitConverter.ToDouble(bytes, i);
				if (val < min)
					min = val;
				if (val > max)
					max = val;
				field[r, c] = BitConverter.ToDouble(bytes, i);
			}
			catch
			{
				field[r, c] = 0;
			}
			c++;
			if(c == cols)
			{
				c = 0;
				r++;
			}
		}
		Debug.Log("Min: " + min + " Max: " + max);
	}
}


