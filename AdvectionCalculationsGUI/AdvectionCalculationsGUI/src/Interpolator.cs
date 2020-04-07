using System;
using System.Collections.Generic;
using System.Diagnostics;
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
				if (d < minDist)
				{
					if (d == 0)
					{
						return (T)p.Clone();
					}
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
			double FTLE = 0;
			Vector3 vel = Vector3.Zero;
			List<T> dataset = new List<T>();
			foreach(Voxel<T> v in voxels)
			{
				dataset.AddRange(v.Points);
			}

			double minDist = double.PositiveInfinity;
			foreach (T p in dataset)
			{
				double d = Vector3.Distance(pos, p.Pos);
				if (d < minDist)
				{
					if (d == 0)
					{
						return (T)p.Clone();
					}
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

		public static T IWDInterpolatePoint(Vector3 pos, List<T> dataset, double diameter)
		{
			bool found = false;
			double inverseDistSum = 0;
			int power = 3;
			double FTLE = 0;
			Vector3 vel = Vector3.Zero;
			foreach (T p in dataset)
			{
				double d = Vector3.Distance(pos, p.Pos);
				if (d == 0)
				{
					return (T)p.Clone();
				}

				if (d < diameter/2)
				{
					double w = 1 / (Math.Pow(d, power));
					inverseDistSum += w;
					vel += Vector3.Multiply(p.Vel, (float)w);
					if (typeof(T) == typeof(SeedPoint))
					{
						FTLE += ((SeedPoint)(Point)p).FTLE * w;
					}
					found = true;
				}
			}

			if (!found)
			{
				return null;
			}

			// TODO
			if (typeof(T) == typeof(SeedPoint))
			{
				return (T)(object)new SeedPoint(pos, Vector3.Divide(vel, (float)inverseDistSum), FTLE/inverseDistSum);
			}
			else
				return (T)new Point(pos, Vector3.Divide(vel, (float)inverseDistSum));
		}

		public static T IWDInterpolatePoint(Vector3 pos, List<Voxel<T>> voxels, double diameter)
		{
			List<T> dataset = new List<T>();
			bool found = false;
			foreach (Voxel<T> v in voxels)
			{
				dataset.AddRange(v.Points);
			}
			double inverseDistSum = 0;
			int power = 3;
			double FTLE = 0;
			Vector3 vel = Vector3.Zero;
			foreach (T p in dataset)
			{
				double d = Vector3.Distance(pos, p.Pos);
				if (d == 0)
				{
					return (T)p.Clone();
				}

				if (d < diameter / 2)
				{
					double w = 1 / (Math.Pow(d, power));
					inverseDistSum += w;
					vel += Vector3.Multiply(p.Vel, (float)w);
					if (typeof(T) == typeof(SeedPoint))
					{
						FTLE += ((SeedPoint)(Point)p).FTLE * w;
					}
					found = true;
				}
			}

			if(!found)
			{
				return null;
			}
			// TODO
			if (typeof(T) == typeof(SeedPoint))
			{
				return (T)(object)new SeedPoint(pos, Vector3.Divide(vel, (float)inverseDistSum), FTLE / inverseDistSum);
			}
			else
				return (T)new Point(pos, Vector3.Divide(vel, (float)inverseDistSum));
		}
	
		public static List<Voxel<T>> SelectVoxels(Vector3 pos, List<Voxel<T>> all)
		{
			List<Voxel<T>> voxels = new List<Voxel<T>>(27);
			Voxel<T> center = null;
			foreach (Voxel<T> v in all)
			{
				if (v.IsPointInside(pos))
				{
					center = v;
					voxels.Add(center);
					break;
				}
			}
			if(center == null)
			{
				//Debug.WriteLine("CENTER IS NULL!");
				return voxels;
			}
			foreach (Voxel<T> v in all)
			{
				if (center != v && v.IsNeighborOf(center))
				{
					voxels.Add(v);
				}
			}
			return voxels;
		}
	}

}
