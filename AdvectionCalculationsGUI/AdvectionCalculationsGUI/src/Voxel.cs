using System;
using System.Collections.Generic;
using System.Numerics;

public class Voxel
{
	private List<Point> points = new List<Point>();
	public readonly double[] boundary;
	/// <summary>
	/// 
	/// </summary>
	/// <param name="xo">X origin</param>
	/// <param name="yo">Y origin</param>
	/// <param name="zo">Z origin</param>
	/// <param name="xm">X maximum</param>
	/// <param name="ym">Y maximum</param>
	/// <param name="zm">X maximum</param>
	public Voxel(double xo, double yo, double zo, double xm, double ym, double zm)
	{
		double[] boundary = { xo, yo, zo, xm, ym, zm };
		this.boundary = boundary;
	}

	public List<Point> GetPoints()
	{
		return points;
	}

	/// <summary>
	/// If the point belongs to the Voxel, adds it. Returns True on success.
	/// </summary>
	/// <param name="p"></param>
	/// <returns>True if the point was added</returns>
	public bool AddPoint(Point p)
	{
		if (IsPointInside(p))
		{
			points.Add(p);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Check if the point is inside of the Voxel
	/// </summary>
	/// <param name="p"></param>
	/// <returns></returns>
	public bool IsPointInside(Point p)
	{
		return IsPointInside(p.Pos);
	}

	public bool IsPointInside(Vector3 Pos)
	{
		return (Pos.X < boundary[3] && Pos.X >= boundary[0]) &&
					(Pos.Y < boundary[4] && Pos.Y >= boundary[1]) &&
					(Pos.Z < boundary[5] && Pos.Z >= boundary[2]);
	}

	public bool IsNeighbor(double[] boundary)
	{
		if (boundary.Length != this.boundary.Length)
			throw new Exception("Not the boundary!");

		for(int i = 0; i < this.boundary.Length; i++)
		{
			for(int j = 0; j < boundary.Length; j++)
			{
				if (this.boundary[i] == boundary[j])
					return true;
			}
		}
		return false;
	}

	public int Size()
	{
		return points.Count;
	}
}
