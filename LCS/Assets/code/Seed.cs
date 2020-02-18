using System;
using System.Collections.Generic;

public class Seed
{
	private List<Point> points = new List<Point>();
	public Point CenterPoint { get; private set; }
	private readonly double radius;

	public Seed(Point center, double radius, DataSet ds)
	{
		CenterPoint = center;
		this.radius = radius;

		points.Add(center.Clone());
		points.Add(ds.GetPoint(new double[3] { center.pos[0], center.pos[1] + radius, center.pos[2] })); //UP
		points.Add(ds.GetPoint(new double[3] { center.pos[0], center.pos[1] - radius, center.pos[2] })); //DOWN
		points.Add(ds.GetPoint(new double[3] { center.pos[0] - radius, center.pos[1], center.pos[2] })); //LEFT
		points.Add(ds.GetPoint(new double[3] { center.pos[0] + radius, center.pos[1], center.pos[2] })); //RIGHT
	}

	public double[] getCurPos()
	{
		return points[0].pos;
	}

	public double Calculate(DataSet ds, double dt)
	{
		for(int i = 0; i < points.Count; i++)
		{
			double[] newPos = new double[3];
			newPos[0] = points[i].pos[0] + points[i].vel[1] * dt;
			newPos[1] = points[i].pos[1] + points[i].vel[0] * dt;
			newPos[2] = points[i].pos[2] + points[i].vel[2] * dt;
			points[i] = Interpolator.Interpolate(newPos, ds.GetPoints(), ds.voxelSize);
			
		}
		double A = (points[4].pos[0] - points[3].pos[0]) / 2 * radius;
		double B = (points[1].pos[0] - points[2].pos[0]) / 2 * radius;
		double C = (points[4].pos[1] - points[3].pos[1]) / 2 * radius;
		double D = (points[1].pos[1] - points[2].pos[1]) / 2 * radius;
		double eigh = (A * A) + (B * B) - (A * C) - (B * D); //eigenvalue of {{A, B}, {C, D}} * {{A, C}, {B, D}}
		return eigh;
	}
}
