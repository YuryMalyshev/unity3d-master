using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdvectionCalculationsGUI.src
{
	public static class Interpolator<T> where T : Point
	{
		public static T NNInterpolatePoint(Vector3 pos, List<T> dataset)
		{
			double FTLE = 0;
			Vector3 vel = Vector3.Zero;

			double minDist = double.PositiveInfinity;
			foreach (T p in dataset)
			{
				double d = Vector3.Distance(pos, p.Pos);
				if (d == 0)
				{
					return (T)p.Clone();
				}

				if (d < minDist)
				{
					minDist = d;
					vel = p.Vel;
					if (typeof(T) == typeof(SeedPoint))
					{
						FTLE = ((SeedPoint)(Point)p).FTLE;
					}
				}
			}
			if (typeof(T) == typeof(SeedPoint))
			{
				return (T)(object)new SeedPoint(pos, vel, FTLE);
			}
			else 
				return (T)new Point(pos, vel);
		}

		public static T NNInterpolatePoint(Vector3 pos, List<Voxel<T>> voxels)
		{
			return null;
		}

		public static T IWDInterpolatePoint(Vector3 pos, List<T> dataset)
		{
			return null;
		}

		public static T IWDInterpolatePoint(Vector3 pos, List<Voxel<T>> voxels)
		{
			return null;
		}
	}
}
