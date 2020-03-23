using Accord.Math.Decompositions;
using System;
using System.Collections.Generic;
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
		private readonly double radius;
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
				center = Interpolator<Point>.NNInterpolatePoint(newPos, ids.GetPoints());
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
						pseudoparticles[d] = Interpolator<Point>.NNInterpolatePoint(newPos, ids.GetPoints());
					}
					catch
					{
						pseudoparticles[d] = null;
					}
				}
			}
			
			double A, B, C, D, E, F;
			if (pseudoparticles[Direction.North] == null || pseudoparticles[Direction.South] == null)
			{
				B = 0;
				D = 0;
			}
			else
			{
				B = (pseudoparticles[Direction.North].Pos.X - pseudoparticles[Direction.South].Pos.X) / 2 * radius;
				D = (pseudoparticles[Direction.North].Pos.Y - pseudoparticles[Direction.South].Pos.Y) / 2 * radius;
			}
			if (pseudoparticles[Direction.West] == null || pseudoparticles[Direction.East] == null)
			{
				A = 0;
				C = 0;
			}
			else
			{
				A = (pseudoparticles[Direction.East].Pos.X - pseudoparticles[Direction.West].Pos.X) / 2 * radius;
				C = (pseudoparticles[Direction.East].Pos.Y - pseudoparticles[Direction.West].Pos.Y) / 2 * radius;
			}
			if (pseudoparticles[Direction.Up] == null || pseudoparticles[Direction.Down] == null)
			{
				E = 0;
				F = 0;
			}
			else
			{
				E = (pseudoparticles[Direction.Up].Pos.X - pseudoparticles[Direction.Down].Pos.X) / 2 * radius;
				F = (pseudoparticles[Direction.Up].Pos.Y - pseudoparticles[Direction.Down].Pos.Y) / 2 * radius;
			}

			double[,] phi = new double[2, 2] { { A, B }, { C, D } };

			double[,] phiT = Accord.Math.Matrix.Transpose(phi);
			double[,] phiTphi = Accord.Math.Matrix.Dot(phiT, phi);
			EigenvalueDecomposition eig = new EigenvalueDecomposition(phiTphi, true, true);
			double[] eigh = eig.RealEigenvalues;
			FTLE = Math.Log(Accord.Math.Matrix.Max(eigh));
			if(double.IsInfinity(FTLE))
			{
				FTLE = 0;
			}
		}

		public SeedPoint Simplify()
		{
			return new SeedPoint(center, FTLE);
		}

		private Vector3 GetNewPos(Point p, float dt)
		{
			return new Vector3(p.Pos.X + p.Vel.Y * dt, p.Pos.Y + p.Vel.X * dt, p.Pos.Z + p.Vel.Z * dt);
		}
	}
}