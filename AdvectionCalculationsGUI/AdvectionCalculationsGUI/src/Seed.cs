using Accord.Math;
using Accord.Math.Decompositions;
using System;
using System.Collections.Generic;
using System.Linq;

public class Seed
{
	private List<Point> points = new List<Point>();
	private readonly double radius;
	public double FTLE { get; set; }
	public int Step { get; }
	public Seed(Point center, float radius, InputDataSet ds)
	{
		this.radius = radius;
		FTLE = double.NegativeInfinity;
		points.Add(center.Clone());
		points.Add(ds.GetPoint(new System.Numerics.Vector3( center.Pos.X, center.Pos.Y + radius, center.Pos.Z ))); //UP
		points.Add(ds.GetPoint(new System.Numerics.Vector3(center.Pos.X, center.Pos.Y - radius, center.Pos.Z ))); //DOWN
		points.Add(ds.GetPoint(new System.Numerics.Vector3( center.Pos.X - radius, center.Pos.Y, center.Pos.Z ))); //LEFT
		points.Add(ds.GetPoint(new System.Numerics.Vector3(center.Pos.X + radius, center.Pos.Y, center.Pos.Z ))); //RIGHT
	}

	public Seed(List<Point> points, double radius)
	{
		this.radius = radius;
		this.points = points;
	}

	public List<Point> getPoints()
	{
		return points;
	}

	public System.Numerics.Vector3 getCurPos()
	{
		return points[0].Pos;
	}

	public double Calculate(InputDataSet ds, double dt)
	{
		for (int i = 0; i < points.Count; i++)
		{
			if (points[i] != null)
			{
				System.Numerics.Vector3 newPos = new System.Numerics.Vector3();
				newPos.X = points[i].Pos.X + points[i].Vel.Y * (float)dt;
				newPos.Y = points[i].Pos.Y + points[i].Vel.X * (float)dt;
				newPos.Z = points[i].Pos.Z + points[i].Vel.Z * (float)dt;
				try
				{
					points[i] = Interpolator.InterpolatePoint(newPos, ds.GetPoints(), ds.voxelSize);
				}
				catch (Exception)
				{
					points[i] = null;
				}
			}
			if(points[i] == null)
			{
				//Console.WriteLine("[WARNING] point " + i + " in null");
			}
		}
		double A, B, C, D;
		if (points[1] == null || points[2] == null)
		{
			B = 0;
			D = 0;
		}
		else
		{
			B = (points[1].Pos.X - points[2].Pos.X) / 2 * radius;
			D = (points[1].Pos.Y - points[2].Pos.Y) / 2 * radius;
		}
		if (points[3] == null || points[4] == null)
		{
			A = 0;
			C = 0;
		}
		else
		{
			A = (points[4].Pos.X - points[3].Pos.X) / 2 * radius;
			C = (points[4].Pos.Y - points[3].Pos.Y) / 2 * radius;
		}

		double[,] phi = new double[2,2]{ { A, B }, { C, D } };

		double[,] phiT = Matrix.Transpose(phi);
		double[,] phiTphi = Matrix.Dot(phiT, phi);
		EigenvalueDecomposition eig = new EigenvalueDecomposition(phiTphi, true, true);
		double[] eigh = eig.RealEigenvalues;
		return Matrix.Max(eigh);
	}
	
	public byte[] Serialize()
	{
		byte[] temp = new byte[0];
		foreach (Point p in points)
		{
			if (p == null)
			{
				for (int i = 0; i < 6; i++)
				{
					temp = temp.Concat(BitConverter.GetBytes(0)).ToArray();
				}
			}
			else
			{
				temp = temp.Concat(BitConverter.GetBytes(p.Pos.X)).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(p.Pos.Y)).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(p.Pos.Z)).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(p.Vel.X)).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(p.Vel.Y)).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(p.Vel.Z)).ToArray();
			}
		}
		return temp;
	}

	public Seed Clone()
	{
		List<Point> temp = new List<Point>(points.Count);
		foreach(Point p in points)
		{
			temp.Add(p.Clone());
		}
		Seed s = new Seed(temp, radius);
		s.FTLE = FTLE;
		return s;
	}
}
