using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace AdvectionCalculationsGUI.src
{
	/// <summary>
	/// DataSet stores all the voxels, which store data points.
	/// There can be multiple independent DataSets.
	/// </summary>
	public class InputDataSet
	{

		public List<Voxel<Point>> Voxels { get; private set; }
		private static readonly CultureInfo culture = new CultureInfo("");
		public readonly double voxelSize;
		/// <summary>
		/// Parses the file and generates Voxels and Points in the process
		/// </summary>
		/// <param name="filepath"> Directory path to the file</param>
		/// <param name="filename"> Name of the fle</param>
		/// <param name="voxelSize"> Size of a cubic Voxel</param>
		public InputDataSet(string filepath, double voxelSize, BackgroundWorker worker)
		{
			culture.NumberFormat.NumberDecimalDigits = 2;
			culture.NumberFormat.NumberDecimalSeparator = ".";
			culture.NumberFormat.NumberGroupSeparator = ",";

			this.voxelSize = voxelSize;
			Voxels = new List<Voxel<Point>>();
			int total;
			using (StreamReader r = new StreamReader(filepath))
			{
				total = 0;
				while (r.ReadLine() != null) { total++; }
			}
			StreamReader reader = new StreamReader(filepath);
			int count = total;
			worker.ReportProgress(0);
			while (!reader.EndOfStream)
			{
				Point p = ParseLine(reader.ReadLine());
				bool added = false;
				foreach (Voxel<Point> v in Voxels)
				{
					if (v.AddPoint(p))
					{
						added = true;
						break;
					}
				}
				if (!added)
				{
					Voxel<Point> v;
					if (Voxels.Count == 0)
					{
						v = new Voxel<Point>(p.Pos.X - voxelSize / 2, p.Pos.Y - voxelSize / 2, p.Pos.Z - voxelSize / 2,
												p.Pos.X + voxelSize / 2, p.Pos.Y + voxelSize / 2, p.Pos.Z + voxelSize / 2);
					}
					else
					{
						double x = Math.Floor((p.Pos.X + Voxels[0].vertices[3].X) / voxelSize) * voxelSize - Voxels[0].vertices[3].X;
						double y = Math.Floor((p.Pos.Y + Voxels[0].vertices[3].Y) / voxelSize) * voxelSize - Voxels[0].vertices[3].Y;
						double z = Math.Floor((p.Pos.Z + Voxels[0].vertices[3].Z) / voxelSize) * voxelSize - Voxels[0].vertices[3].Z;
						if (p.Pos.Z > 0)
						{
							Debug.WriteLine("");
						}
						v = new Voxel<Point>(x, y, z, x + voxelSize, y + voxelSize, z + voxelSize);
					}
					v.ForceAddPoint(p);
					Voxels.Add(v);
				}
				count--;
				if(count % 100 == 0)
				{
					worker.ReportProgress(((total - count) * 1000) / total);
				}
			}
			worker.ReportProgress(1000);
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
			for (int i = 0; i < n.Length; i++)
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
			foreach (Voxel<Point> v in Voxels)
			{
				points.AddRange(v.Points);
			}
			return points;
		}

		public Point GetPoint(Vector3 pos, bool simple)
		{
			if (simple)
			{
				return Interpolator<Point>.NNInterpolatePoint(pos, GetPoints());
			}
			else
			{
				return GetPoint(pos);
			}
		}

		public Point GetPoint(Vector3 pos)
		{
			List<Voxel<Point>> local = new List< Voxel<Point>>();
			foreach (Voxel<Point> v in Voxels)
			{
				if (v.IsPointInside(pos))
				{
					local.Add(v);
					break;
				}
			}
			if (local.Count == 0)
				return null;
				//throw new Exception("Point is outside of known boundary!");

			foreach (Voxel<Point> v in Voxels)
			{
				if (!v.Equals(local[0]) && v.IsNeighborOf(local[0]))
				{
					local.Add(v);
				}
			}

			return Interpolator<Point>.NNInterpolatePoint(pos, GetPoints());
		}
	}
}
