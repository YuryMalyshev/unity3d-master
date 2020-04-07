using Accord.Math.Decompositions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace AdvectionCalculationsGUI.src
{
	public class Seed
	{
		private enum Direction
		{
			North, South, East, West, Up, Down
		}
		private Point center;
		private readonly Dictionary<Direction, Point> pseudoparticles;
		private readonly float radius;
		private double FTLE;
		private readonly InputDataSet ids;
		public Seed(Point center, float radius, InputDataSet ids)
		{
			this.radius = radius;
			FTLE = double.NegativeInfinity;
			this.center = center;
			this.ids = ids;
			pseudoparticles = new Dictionary<Direction, Point>(6)
			{
				{ Direction.North, ids.GetPoint(new System.Numerics.Vector3(center.Pos.X, center.Pos.Y + radius, center.Pos.Z)) },
				{ Direction.South, ids.GetPoint(new System.Numerics.Vector3(center.Pos.X, center.Pos.Y - radius, center.Pos.Z)) },
				{ Direction.West,  ids.GetPoint(new System.Numerics.Vector3(center.Pos.X - radius, center.Pos.Y, center.Pos.Z)) },
				{ Direction.East,  ids.GetPoint(new System.Numerics.Vector3(center.Pos.X + radius, center.Pos.Y, center.Pos.Z)) },
				{ Direction.Up,    ids.GetPoint(new System.Numerics.Vector3(center.Pos.X, center.Pos.Y, center.Pos.Z + radius)) },
				{ Direction.Down,  ids.GetPoint(new System.Numerics.Vector3(center.Pos.X, center.Pos.Y, center.Pos.Z - radius)) }
			};
		}


		public void Calculate(float dt)
		{
			if (center == null)
				return;
			Vector3 newPos = GetNewPos(center, dt);
			try
			{
				center = Interpolator<Point>.IWDInterpolatePoint(newPos, Interpolator<Point>.SelectVoxels(newPos, ids.Voxels), ids.voxelSize);
			}
			catch
			{
				center = null;
				return;
			}

			Direction[] keys = new Direction[pseudoparticles.Count];
			pseudoparticles.Keys.CopyTo(keys, 0);
			foreach (Direction d in keys)
			{
				if (pseudoparticles[d] != null)
				{
					newPos = GetNewPos(pseudoparticles[d], dt);
					try
					{
						pseudoparticles[d] = Interpolator<Point>.IWDInterpolatePoint(newPos, Interpolator<Point>.SelectVoxels(newPos, ids.Voxels), ids.voxelSize);
					}
					catch
					{
						Debug.WriteLine("Unable to interpolate particle " + d + " in seed " + center);
						pseudoparticles[d] = null;
					}
				}
			}
			
			double A, B, C, D, E, F, G, H, I;
			if (pseudoparticles[Direction.West] == null || pseudoparticles[Direction.East] == null)
			{
				A = 0;
				D = 0;
				G = 0;
			}
			else
			{
				A = (pseudoparticles[Direction.East].Pos.X - pseudoparticles[Direction.West].Pos.X) / 2 * radius;
				D = (pseudoparticles[Direction.East].Pos.Y - pseudoparticles[Direction.West].Pos.Y) / 2 * radius;
				G = (pseudoparticles[Direction.East].Pos.Z - pseudoparticles[Direction.West].Pos.Z) / 2 * radius;
			}
			if (pseudoparticles[Direction.North] == null || pseudoparticles[Direction.South] == null)
			{
				B = 0;
				E = 0;
				H = 0;
			}
			else
			{
				B = (pseudoparticles[Direction.North].Pos.X - pseudoparticles[Direction.South].Pos.X) / 2 * radius;
				E = (pseudoparticles[Direction.North].Pos.Y - pseudoparticles[Direction.South].Pos.Y) / 2 * radius;
				H = (pseudoparticles[Direction.North].Pos.Z - pseudoparticles[Direction.South].Pos.Z) / 2 * radius;
			}
			if (pseudoparticles[Direction.Up] == null || pseudoparticles[Direction.Down] == null)
			{
				C = 0;
				F = 0;
				I = 0;
			}
			else
			{
				C = (pseudoparticles[Direction.Up].Pos.X - pseudoparticles[Direction.Down].Pos.X) / 2 * radius;
				F = (pseudoparticles[Direction.Up].Pos.Y - pseudoparticles[Direction.Down].Pos.Y) / 2 * radius;
				I = (pseudoparticles[Direction.Up].Pos.Z - pseudoparticles[Direction.Down].Pos.Z) / 2 * radius;
			}

			double[,] phi = new double[3, 3] { { A, B, C }, { D, E, F },  { G, H, I } };

			double[,] phiT = Accord.Math.Matrix.Transpose(phi);
			double[,] phiTphi = Accord.Math.Matrix.Dot(phiT, phi);
			try
			{
				EigenvalueDecomposition eig = new EigenvalueDecomposition(phiTphi, true, true);
				double[] eigh = eig.RealEigenvalues;
				FTLE = Math.Log(Accord.Math.Matrix.Max(eigh));
				if (double.IsInfinity(FTLE))
				{
					FTLE = double.NaN;
				}
			}
			catch
			{
				FTLE = double.NaN;
			}
		}

		public Seed Clone()
		{
			return new Seed((Point)center.Clone(), radius, ids);
		}

		public SeedPoint Simplify()
		{
			if(center == null)
				return null;
			return new SeedPoint(center, FTLE);
		}

		private Vector3 GetNewPos(Point p, float dt)
		{
			//return new Vector3(p.Pos.X + p.Vel.Y * dt, p.Pos.Y + p.Vel.X * dt, p.Pos.Z + p.Vel.Z * dt); // FOR GYRO
			return new Vector3(p.Pos.X + p.Vel.Y * dt, p.Pos.Y + p.Vel.Z * dt, p.Pos.Z + p.Vel.X * dt); // FOR GUMA
		}
	}
}