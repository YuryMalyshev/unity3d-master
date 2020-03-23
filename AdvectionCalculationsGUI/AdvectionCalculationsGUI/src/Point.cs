using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Point : IEquatable<Point>, IComparer<Point>, IComparable<Point>, ICloneable
{
	public Vector3 Pos { get; private set; }
	public Vector3 Vel { get; private set; }

	public Point(double x, double y, double z, double vx, double vy, double vz)
	{
		this.Pos = new Vector3((float)x, (float)y, (float)z);
		this.Vel = new Vector3((float)vx, (float)vy, (float)vz);
	}

	public Point(float x, float y, float z, float vx, float vy, float vz)
	{
		this.Pos = new Vector3(x, y, z);
		this.Vel = new Vector3(vx, vy, vz);
	}

	public Point(double[] pos, double[] vel)
	{
		this.Pos = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
		this.Vel = new Vector3((float)vel[0], (float)vel[1], (float)vel[2]);
	}

	public byte[] Serialize()
	{
		byte[] temp = new byte[0];
		temp = temp.Concat(BitConverter.GetBytes(Pos.X)).ToArray();
		temp = temp.Concat(BitConverter.GetBytes(Pos.Y)).ToArray();
		temp = temp.Concat(BitConverter.GetBytes(Pos.Z)).ToArray();
		temp = temp.Concat(BitConverter.GetBytes(Vel.X)).ToArray();
		temp = temp.Concat(BitConverter.GetBytes(Vel.Y)).ToArray();
		temp = temp.Concat(BitConverter.GetBytes(Vel.Z)).ToArray();
		return temp;
	}

	public static Point DeSerialize(byte[] subset)
	{
		Vector3 Pos = new Vector3();
		Vector3 Vel = new Vector3();
		int index = 0;
		Pos.X = BitConverter.ToSingle(subset, index); index += sizeof(float);
		Pos.Y = BitConverter.ToSingle(subset, index); index += sizeof(float);
		Pos.Z = BitConverter.ToSingle(subset, index); index += sizeof(float);
		Vel.X = BitConverter.ToSingle(subset, index); index += sizeof(float);
		Vel.Y = BitConverter.ToSingle(subset, index); index += sizeof(float);
		Vel.Z = BitConverter.ToSingle(subset, index);
		return new Point(Pos, Vel);
	}

	public Point(Point p)
	{
		Pos = p.Pos;
		Vel = p.Vel;
	}

	public Point(Vector3 Pos, Vector3 Vel)
	{
		this.Pos = Pos;
		this.Vel = Vel;
	}

	public double DistanceTo(Point another)
	{
		return Vector3.Distance(Pos, another.Pos);
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

	public virtual object Clone()
	{
		return new Point(Pos, Vel);
	}
}
