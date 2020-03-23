using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class StreamLine_DEPRECATED
{
	private List<Seed_DEPRECATED> points;

	public StreamLine_DEPRECATED(string filedir, string filename)
	{
		// read data
		byte[] bytes = File.ReadAllBytes(filedir + filename);
		//Debug.Log(filename + " : " + bytes.Length + " bytes");

		// add seeds

		int npoints = 5;
		int nparam = 6;
		int pointSize = nparam * sizeof(double);
		int pointsSize = npoints * pointSize;
		int lineSize = pointsSize + sizeof(double);

		points = new List<Seed_DEPRECATED>(bytes.Length / lineSize);
		double maxFTLE = double.NegativeInfinity;
		for (int i = 0; i < bytes.Length; i += lineSize)
		{
			try
			{
				int step = i / lineSize;
				List<Point_DEPRECATED> temp_points = new List<Point_DEPRECATED>(npoints);
				for (int j = i; j < i + pointsSize; j += pointSize)
				{
					double[] pos = new double[nparam / 2];
					double[] vel = new double[nparam / 2];
					for (int k = 0; k < nparam / 2; k++)
					{
						pos[k] = BitConverter.ToDouble(bytes, j + sizeof(double) * k);
						vel[k] = BitConverter.ToDouble(bytes, j + sizeof(double) * (k + nparam / 2));
					}
					temp_points.Add(new Point_DEPRECATED(pos, vel));
				}
				double FTLE = BitConverter.ToDouble(bytes, i + pointsSize);
				maxFTLE = Math.Max(maxFTLE, FTLE);
				points.Add(new Seed_DEPRECATED(temp_points, step));
			}
			catch
			{
				Debug.LogError("Error in " + filename);
			}
		}
		foreach(Seed_DEPRECATED s in points)
		{
			s.FTLE = maxFTLE;
		}
		//Debug.Log("MaxFTLE = " + maxFTLE);
	}

	public List<Seed_DEPRECATED> GetPoints()
	{
		return points;
	}
}
