using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace AdvectionCalculationsGUI.src
{
	public class Voxel<T> where T : Point
	{
		public List<T> Points { get; private set; }

		// 4 5 | 0 1
		// 6 7 | 2 3
		public readonly Vector3[] vertices;

		private void Initialize(Vector3 minCorner, Vector3 maxCorner)
		{
			Points = new List<T>();

			vertices[3] = minCorner;
			vertices[4] = maxCorner;

			vertices[0] = new Vector3(vertices[4].X, vertices[4].Y, vertices[3].Z);
			vertices[1] = new Vector3(vertices[3].X, vertices[4].Y, vertices[3].Z);
			vertices[2] = new Vector3(vertices[4].X, vertices[3].Y, vertices[3].Z);

			vertices[5] = new Vector3(vertices[3].X, vertices[4].Y, vertices[4].Z);
			vertices[6] = new Vector3(vertices[4].X, vertices[3].Y, vertices[4].Z);
			vertices[7] = new Vector3(vertices[3].X, vertices[3].Y, vertices[4].Z);
		}

		public Voxel(Vector3[] vertices)
		{
			Points = new List<T>();
			this.vertices = new Vector3[vertices.Length];
			vertices.CopyTo(this.vertices, 0);
		}

		public Voxel(Vector3 minCorner, Vector3 maxCorner)
		{
			vertices = new Vector3[8];
			Initialize(minCorner, maxCorner);
		}

		public Voxel(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
		{
			vertices = new Vector3[8];
			Vector3 minCorner = new Vector3((float)minX, (float)minY, (float)minZ);
			Vector3 maxCorner = new Vector3((float)maxX, (float)maxY, (float)maxZ);
			Initialize(minCorner, maxCorner);
		}

		/// <summary>
		/// If the point belongs to the Voxel, adds it. Returns True on success.
		/// </summary>
		/// <param name="p"></param>
		/// <returns>True if the point was added</returns>
		public bool AddPoint(T p)
		{
			if (IsPointInside(p))
			{
				Points.Add(p);
				return true;
			}
			return false;
		}

		public void ForceAddPoint(T p)
		{
			Points.Add(p);
		}

		/// <summary>
		/// Check if the point is inside of the Voxel
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool IsPointInside(T p)
		{
			return IsPointInside(p.Pos);
		}

		public bool IsPointInside(Vector3 Pos)
		{
			bool val = (Pos.X < vertices[4].X && Pos.X >= vertices[3].X) &&
						  (Pos.Y < vertices[4].Y && Pos.Y >= vertices[3].Y) &&
						  (Pos.Z < vertices[4].Z && Pos.Z >= vertices[3].Z);
			if (Pos.Z > 0)
			{
				//Debug.WriteLine(Pos);
			}
			return val;
		}

		public bool IsNeighborOf(Voxel<T> voxel)
		{
			foreach(Vector3 v1 in vertices)
			{
				foreach(Vector3 v2 in voxel.vertices)
				{
					if(v1.Equals(v2))
					{
						return true;
					}
				}
			}
			return false;
		}

		public int Size()
		{
			return Points.Count;
		}

		public double GetVoxelDimension()
		{
			return Math.Abs(vertices[0].X - vertices[1].X);
		}
	}
}
