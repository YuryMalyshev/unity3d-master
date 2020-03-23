using System;
using System.Collections.Generic;
using UnityEngine;

public class Seed_DEPRECATED : IEquatable<Seed_DEPRECATED>
{
	private readonly List<Point_DEPRECATED> points;
	public int Step { get; }
	private bool inUse = false;
	public double FTLE { get; set; }
	public Vector3 pos { get => points[0].pos;}

	public Seed_DEPRECATED(List<Point_DEPRECATED> points, int Step)
	{
		this.points = points;
		this.Step = Step;
	}
	/// <summary>
	/// Test
	/// </summary>
	/// <param name="pos">[3]</param>
	public Seed_DEPRECATED(double[] pos)
	{
		points = new List<Point_DEPRECATED>();
		points.Add(new Point_DEPRECATED(pos, new double[]{0,0,0}));
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

	public bool Equals(Seed_DEPRECATED other)
	{
		return points[0].Equals(other.points[0]);
	}
}
