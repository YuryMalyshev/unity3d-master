using System;
using System.Collections.Generic;
using System.Numerics;

public static class Interpolator
{
	public static Point InterpolatePoint(Vector3 pos, List<Point> dataset, double size)
	{
		double inverseDistSum = 0;
		Vector3 vel = new Vector3(float.NaN, float.NaN, float.NaN);

		foreach(Point p in dataset)
		{
			double d = Math.Sqrt(Math.Pow(pos.X - p.Pos.X, 2) + Math.Pow(pos.Y - p.Pos.Y, 2) + Math.Pow(pos.Z - p.Pos.Z, 2));
			if (d == 0)
			{
				return new Point(p.Pos, p.Vel);
			}

			if (d < size)
			{
				int power = 3;
				double w = 1 / (Math.Pow(d, power));
				inverseDistSum += w;
				if (float.IsNaN(vel.X))
					vel.X = 0;
				vel.X += (float)w * vel.X;

				if (float.IsNaN(vel.Y))
					vel.Y = 0;
				vel.Y += (float)w * vel.Y;

				if (float.IsNaN(vel.Z))
					vel.Z = 0;
				vel.Z += (float)w * vel.Z;
			}
		}

		if (float.IsNaN(vel.X) || float.IsNaN(vel.Y) || float.IsNaN(vel.Z))
		{
			throw new Exception("The point is too far away from known points");
		}

		vel.X /= (float)inverseDistSum;
		vel.X /= (float)inverseDistSum;
		vel.X /= (float)inverseDistSum;

		return new Point(pos, vel);
	}

	public static Point InterpolatePoint(Vector3 pos, List<Voxel> voxels, double size)
	{
		double inverseDistSum = 0;
		Vector3 vel = new Vector3(float.NaN, float.NaN, float.NaN);

		foreach (Voxel v in voxels)
		{
			foreach (Point p in v.GetPoints())
			{
				double d = Math.Sqrt(Math.Pow(pos.X - p.Pos.X, 2) + Math.Pow(pos.Y - p.Pos.Y, 2) + Math.Pow(pos.Z - p.Pos.Z, 2));
				if (d == 0)
				{
					return new Point(p.Pos, p.Vel);
				}

				if (d < size)
				{
					int power = 3;
					double w = 1 / (Math.Pow(d, power));
					inverseDistSum += w;
					if (float.IsNaN(vel.X))
						vel.X = 0;
					vel.X += (float)w * vel.X;

					if (float.IsNaN(vel.Y))
						vel.Y = 0;
					vel.Y += (float)w * vel.Y;

					if (float.IsNaN(vel.Z))
						vel.Z = 0;
					vel.Z += (float)w * vel.Z;
				}
			}
		}

		if (float.IsNaN(vel.X) || float.IsNaN(vel.Y) || float.IsNaN(vel.Z))
		{
			throw new Exception("The point is too far away from known points");
		}

		vel.X /= (float)inverseDistSum;
		vel.X /= (float)inverseDistSum;
		vel.X /= (float)inverseDistSum;

		return new Point(pos, vel);
	}

	public static double[] OneStep(Point startPoint, double dt)
	{
		double[] newPos = new double[3];
		newPos[0] = startPoint.Pos.X + startPoint.Vel.X * dt;
		newPos[1] = startPoint.Pos.Y + startPoint.Vel.Y * dt;
		newPos[2] = startPoint.Pos.Z + startPoint.Vel.Z * dt;
		return newPos;
	}

	public static double InterpolateFTLE(double[] pos, List<Seed> dataset, double radius)
	{
		double inverseDistSum = 0;
		int power = 3;

		double FTLE = double.NaN;
		double closestDist = double.PositiveInfinity;
		double closestValue = double.NaN;

		foreach (Seed s in dataset)
		{
			double d = Math.Sqrt(Math.Pow(pos[0] - s.getCurPos().X, 2) + Math.Pow(pos[1] - s.getCurPos().Y, 2) + Math.Pow(pos[2] - s.getCurPos().Z, 2));
			if (d == 0)
			{
				return s.FTLE;
			}

			if (d < closestDist)
			{
				closestDist = d;
				closestValue = s.FTLE;
			}

			if (d < radius)
			{
				if (double.IsNaN(FTLE))
				{
					FTLE = 0;
				}
				double w = 1 / (Math.Pow(d, power));
				inverseDistSum += w;
				FTLE += w * s.FTLE;
			}
		}

		if (double.IsNaN(FTLE))
		{
			Console.WriteLine("Unable to interpolate, returning closest value at distance " + closestDist);
			return closestValue;
		}

		FTLE /= inverseDistSum;
		return FTLE;
	}

}
