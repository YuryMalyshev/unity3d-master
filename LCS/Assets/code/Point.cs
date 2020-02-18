using System;
using System.Collections.Generic;

public class Point : IEquatable<Point>, IComparer<Point>, IComparable<Point>
{
	public readonly double[] pos;
	public readonly double[] vel;

	public Point(double x, double y, double z, double vx, double vy, double vz)
	{
		double[] pos = { x, y, z };
		this.pos = pos;
		double[] vel = { vx, vy, vz };
		this.vel = vel;
	}
	public Point(double[] pos, double[] vel)
	{
		this.pos = pos;
		this.vel = vel;
	}

	/// <summary>
	/// Compare point positions
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Equals(Point other)
	{
		return this.pos.Equals(other.pos);
	}

	public int Compare(Point x, Point y)
	{
		//TODO: maybe should change implementation
		return x.pos[2].CompareTo(y.pos[2]);
	}

	public int CompareTo(Point other)
	{
		return this.Compare(this, other);
	}

	public override string ToString()
	{
		return "[" + pos[0] + ", " + pos[1] + ", " + pos[2] + "]" +
		"[" + vel[0] + ", " + vel[1] + ", " + vel[2] + "]";
	}

	public Point Clone()
	{
		return new Point(pos[0], pos[1], pos[2], vel[0], vel[1], vel[2]);
	}
}
