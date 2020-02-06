using System;
using System.Collections.Generic;

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
		return (p.pos[0] < boundary[3] && p.pos[0] >= boundary[0]) &&
					(p.pos[1] < boundary[4] && p.pos[1] >= boundary[1]) &&
					(p.pos[2] < boundary[5] && p.pos[2] >= boundary[2]);
	}

	public int Size()
	{
		return points.Count;
	}
}
