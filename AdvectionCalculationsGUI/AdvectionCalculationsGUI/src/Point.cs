using System;
using System.Collections.Generic;
using System.Numerics;

public class Point : IEquatable<Point>, IComparer<Point>, IComparable<Point>
{
	public Vector3 Pos { get; private set; }
	public Vector3 Vel { get; private set; }

	public Point(double x, double y, double z, double vx, double vy, double vz)
	{
		this.Pos = new Vector3((float)x, (float)y, (float)z);
		this.Vel = new Vector3((float)vx, (float)vy, (float)vz);
	}
	public Point(double[] pos, double[] vel)
	{
		this.Pos = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
		this.Vel = new Vector3((float)vel[0], (float)vel[1], (float)vel[2]);
	}

	public Point(Vector3 Pos, Vector3 Vel)
	{
		this.Pos = Pos;
		this.Vel = Vel;
	}

	public double DistanceTo(Point another)
	{
		return Math.Sqrt(Math.Pow(Pos.X - another.Pos.X, 2) + Math.Pow(Pos.Y - another.Pos.Y, 2) + Math.Pow(Pos.Z - another.Pos.Z, 2));
	}

	/// <summary>
	/// Compare point positions
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Equals(Point other)
	{
		return this.Pos.Equals(other.Pos);
	}

	public int Compare(Point x, Point y)
	{
		//TODO: maybe should change implementation
		return x.Pos.Z.CompareTo(y.Pos.Z);
	}

	public int CompareTo(Point other)
	{
		return this.Compare(this, other);
	}

	public override string ToString()
	{
		return "{[" + Pos.X + ", " + Pos.Y + ", " + Pos.Z + "]" +
		"[" + Vel.X + ", " + Vel.Y + ", " + Vel.Z + "]}";
	}

	public Point Clone()
	{
		return new Point(Pos, Vel);
	}
}
