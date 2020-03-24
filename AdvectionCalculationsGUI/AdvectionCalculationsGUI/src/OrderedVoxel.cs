using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdvectionCalculationsGUI.src
{
	public class OrderedVoxel<T> : Voxel<T> where T : Point
	{
		private readonly T[,,] matrix;
		public OrderedVoxel(Vector3[] vertices, int resolution) : base(vertices)
		{
			matrix = new T[resolution, resolution, resolution];
		}

		public void AddPointAt(T sp, int x, int y, int z)
		{
			matrix[x, y, z] = sp;
		}

		public bool TryGetPointAt(int x, int y, int z, out T point)
		{
			if (x >= matrix.GetLength(0) || y >= matrix.GetLength(1) || z >= matrix.GetLength(2))
			{
				point = null;
				return false;
			}
			else
			{
				point = matrix[x, y, z];
				return true;
			}
		}
	}
}
