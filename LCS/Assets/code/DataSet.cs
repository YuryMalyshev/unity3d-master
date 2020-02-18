using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>
/// DataSet stores all the voxels, which store data points.
/// There can be multiple independent DataSets.
/// </summary>
public class DataSet
{
	private List<Voxel> voxels = new List<Voxel>();
	private static readonly CultureInfo culture = new CultureInfo("");

	public readonly double voxelSize;
	/// <summary>
	/// Parses the file and generates Voxels and Points in the process
	/// </summary>
	/// <param name="filepath"> Directory path to the file</param>
	/// <param name="filename"> Name of the fle</param>
	/// <param name="voxelSize"> Size of a cubic Voxel</param>
	public DataSet(string filepath, string filename, double voxelSize)
	{
		culture.NumberFormat.NumberDecimalDigits = 2;
		culture.NumberFormat.NumberDecimalSeparator = ".";
		culture.NumberFormat.NumberGroupSeparator = ",";

		this.voxelSize = voxelSize;

		string path = filepath+filename;
		Debug.Log(path);
		StreamReader reader = new StreamReader(path);
		while (!reader.EndOfStream)
		{
			Point p = ParseLine(reader.ReadLine());
			bool added = false;
			foreach(Voxel v in voxels)
			{
				if(v.AddPoint(p))
				{
					added = true;
					break;
				}
			}
			if(!added)
			{
				Voxel v;
				if (voxels.Count == 0)
				{
					v = new Voxel(p.pos[0] - voxelSize / 2, p.pos[1] - voxelSize / 2, p.pos[2] - voxelSize / 2,
											p.pos[0] + voxelSize / 2, p.pos[1] + voxelSize / 2, p.pos[2] + voxelSize / 2);
					//Debug.Log("Created Voxel at pos " + v.boundary[0] + " " + v.boundary[1] + " " + v.boundary[2]);
					//Debug.Log("For point " + p.pos[0] + " " + p.pos[1]);
				}
				else
				{
					double x = Math.Ceiling(p.pos[0] / voxelSize) * voxelSize + voxels[0].boundary[0];
					double y = Math.Ceiling(p.pos[1] / voxelSize) * voxelSize + voxels[0].boundary[1];
					double z = Math.Ceiling(p.pos[2] / voxelSize) * voxelSize + voxels[0].boundary[2];
					v = new Voxel(x, y, z , x + voxelSize, y + voxelSize, z + voxelSize);
					//Debug.Log("Created Voxel at pos " + x + " " + y + " " + z);
					//Debug.Log("For point " + p.pos[0] + " " + p.pos[1]);
				}
				v.AddPoint(p);
				voxels.Add(v);
			}
		}
		reader.Close();
		reader.Dispose();
	}

	/// <summary>
	/// Parses a single line into a Point
	/// </summary>
	/// <param name="line"></param>
	/// <returns></returns>
	private Point ParseLine(string line)
	{
		string[] n = line.Split(' ');
		if (n.Length < 6) 
			throw new ArgumentNullException();
		double[] v = new double[n.Length];
		for(int i = 0; i < n.Length; i++)
		{
			v[i] = ParseDouble(n[i]);
		}
		return new Point(v[0], v[1], v[2], v[3], v[4], v[5]);
	}

	private double ParseDouble(string num)
	{
		return double.Parse(num, NumberStyles.Number | NumberStyles.AllowExponent, culture);
	}

	public List<Point> GetPoints()
	{
		List<Point> points = new List<Point>();
		foreach(Voxel v in voxels)
		{
			points.AddRange(v.GetPoints());
		}
		return points;
	}

	public Point GetPoint(double[] pos, bool simple)
	{
		if(simple)
		{
			return Interpolator.Interpolate(pos, GetPoints(), voxelSize);
		}
		else
		{
			return GetPoint(pos);
		}
	}
	public Point GetPoint(double[] pos)
	{
		List<Voxel> local = new List<Voxel>();
		foreach (Voxel v in voxels)
		{
			if (v.IsPointInside(pos))
			{
				local.Add(v);
				break;
			}
		}
		if (local.Count == 0)
			throw new Exception("Point is outside of known boundary!");

		foreach (Voxel v in voxels)
		{
			if (!v.Equals(local[0]) && v.IsNeighbor(local[0].boundary))
			{
				local.Add(v);
			}
		}

		Point p = Interpolator.Interpolate(pos, local, voxelSize);
		return p;
		/*Voxel origin = null;
	   foreach(Voxel v in voxels)
		{
			if(v.IsPointInside(pos))
			{
				origin = v;
				break;
			}
		}
		if (origin == null)
			throw new Exception("Point is outside of known boundary!");
		List<Point> dataset = new List<Point>();
		dataset.AddRange(origin.GetPoints());
		foreach(Voxel v in voxels)
		{
			if (!v.Equals(origin) && v.IsNeighbor(origin.boundary))
			{
				dataset.AddRange(v.GetPoints());
			}
		}
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		Point p = Interpolator.Interpolate(pos, GetPoints(), voxelSize);
		sw.Stop();
		Debug.Log("Elapsed Time is {" + sw.ElapsedMilliseconds + "} ms");
		//Point p = Interpolator.Interpolate(pos, dataset, voxelSize);
		return p;*/
	}
}
