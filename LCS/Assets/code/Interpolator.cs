using System;
using System.Collections.Generic;

public static class Interpolator
{
	public static Point Interpolate(double[] pos, List<Point> dataset, double size)
	{
		double inverseDistSum = 0;
		double[] vel = new double[3];
		for (int i = 0; i < vel.Length; i++)
			vel[i] = double.NaN;

		foreach(Point p in dataset)
		{
			double d = Math.Sqrt(Math.Pow(pos[0] - p.pos[0], 2) + Math.Pow(pos[1] - p.pos[1], 2) + Math.Pow(pos[2] - p.pos[2], 2));
			if (d == 0)
			{
				return new Point(p.pos, p.vel);
			}

			if (d < size)
			{
				int power = 3;
				double w = 1 / (Math.Pow(d, power));
				inverseDistSum += w;
				double[] pVel = p.vel;
				for (int i = 0; i < pVel.Length; i++)
				{
					if (double.IsNaN(vel[i]))
					{
						vel[i] = 0;
					}
					vel[i] += w * pVel[i];
				}
			}
		}

		if (double.IsNaN(vel[0]) || Double.IsNaN(vel[1]) || Double.IsNaN(vel[2]))
		{
			throw new Exception("The point is too far away from known points");
		}

		for (int i = 0; i < vel.Length; i++)
		{
			vel[i] /= inverseDistSum;
		}
		double[] newPos = new double[3];
		pos.CopyTo(newPos, 0);
		return new Point(newPos, vel);
	}

	public static Point Interpolate(double[] pos, List<Voxel> voxels, double size)
	{
		double inverseDistSum = 0;
		double[] vel = new double[3];
		for (int i = 0; i < vel.Length; i++)
			vel[i] = double.NaN;

		foreach (Voxel v in voxels)
		{
			foreach (Point p in v.GetPoints())
			{
				double d = Math.Sqrt(Math.Pow(pos[0] - p.pos[0], 2) + Math.Pow(pos[1] - p.pos[1], 2) + Math.Pow(pos[2] - p.pos[2], 2));
				if (d == 0)
				{
					return new Point(p.pos, p.vel);
				}

				if (d < size)
				{
					int power = 3;
					double w = 1 / (Math.Pow(d, power));
					inverseDistSum += w;
					double[] pVel = p.vel;
					for (int i = 0; i < pVel.Length; i++)
					{
						if (double.IsNaN(vel[i]))
						{
							vel[i] = 0;
						}
						vel[i] += w * pVel[i];
					}
				}
			}
		}

		if (double.IsNaN(vel[0]) || Double.IsNaN(vel[1]) || Double.IsNaN(vel[2]))
		{
			throw new Exception("The point is too far away from known points");
		}

		for (int i = 0; i < vel.Length; i++)
		{
			vel[i] /= inverseDistSum;
		}
		return new Point(pos, vel);
	}

	public static double[] OneStep(Point startPoint, double dt)
	{
		double[] newPos = new double[3];
		newPos[0] = startPoint.pos[0] + startPoint.vel[1] * dt;
		newPos[1] = startPoint.pos[1] + startPoint.vel[0] * dt;
		newPos[2] = startPoint.pos[2] + startPoint.vel[2] * dt;
		return newPos;
	}
}
