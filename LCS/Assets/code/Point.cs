using System;
using System.Collections.Generic;
using UnityEngine;

public class Point : IEquatable<Point>, IComparer<Point>, IComparable<Point>
{
	public readonly Vector3 pos;
	public readonly double[] vel;

	public Point(double x, double y, double z, double vx, double vy, double vz)
	{
		Vector3 pos = new Vector3((float)x, (float)y, (float)z);
		this.pos = pos;
		double[] vel = { vx, vy, vz };
		this.vel = vel;
	}
	public Point(double[] pos, double[] vel)
	{
		this.pos = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
		this.vel = vel;
	}

	/// <summary>
	/// Compare point positions
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Equals(Point other)
	{
		return pos.Equals(other.pos);
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
		return "[" + pos.x + ", " + pos.y + ", " + pos.z + "]" +
		"[" + vel[0] + ", " + vel[1] + ", " + vel[2] + "]";
	}

	public Point Clone()
	{
		return new Point(pos.x, pos.y, pos.z, vel[0], vel[1], vel[2]);
	}
}
