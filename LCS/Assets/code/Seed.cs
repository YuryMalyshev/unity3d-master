using System;
using System.Collections.Generic;
using UnityEngine;

public class Seed : IEquatable<Seed>
{
	private readonly List<Point> points;
	public int Step { get; }
	private bool inUse = false;
	public double FTLE { get; set; }
	public Vector3 pos { get => points[0].pos;}

	public Seed(List<Point> points, int Step)
	{
		this.points = points;
		this.Step = Step;
	}
	/// <summary>
	/// Test
	/// </summary>
	/// <param name="pos">[3]</param>
	public Seed(double[] pos)
	{
		points = new List<Point>();
		points.Add(new Point(pos, new double[]{0,0,0}));
	}

	public void ResetUse()
	{
		inUse = false;
	}

	public void Use()
	{
		inUse = true;
	}

	public bool IsInUse()
	{
		return inUse;
	}

	public bool Equals(Seed other)
	{
		return points[0].Equals(other.points[0]);
	}
}
