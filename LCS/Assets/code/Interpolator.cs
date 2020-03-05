using System;
using System.Collections.Generic;
using UnityEngine;

public static class Interpolator
{
	public static double Interpolate(double[] pos, List<Seed> dataset, double radius)
	{
		double inverseDistSum = 0;
		int power = 3;

		double FTLE = double.NaN;
		double closestDist = double.PositiveInfinity;
		double closestValue = double.NaN;

		double test1 = pos[2];
		double test2 = dataset[0].pos[2];

		foreach(Seed s in dataset)
		{
			double d = Math.Sqrt(Math.Pow(pos[0] - s.pos[0], 2) + Math.Pow(pos[1] - s.pos[1], 2) + Math.Pow(pos[2] - s.pos[2], 2));
			if (d == 0)
			{
				return s.FTLE;
			}

			if(d < closestDist)
			{
				closestDist = d;
				closestValue = s.FTLE;
			}

			if(d < radius)
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

		if(double.IsNaN(FTLE))
		{
			Debug.LogWarning("Unable to interpolate, returning closest value at distance " + closestDist);
			return closestValue;
		}

		FTLE /= inverseDistSum;
		return FTLE;
	}
}
