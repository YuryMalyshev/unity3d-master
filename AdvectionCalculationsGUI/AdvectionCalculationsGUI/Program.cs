using AdvectionCalculationsGUI.src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvectionCalculationsGUI
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new GUI());
			//InterpolationTest();
			//InterpolationComparison();
		}

		private static void InterpolationComparison()
		{
			string filepath = "D:/Users/yrmal/Desktop/doublegyro";
			double voxelSize = 0.25;
			InputDataSet ids = new InputDataSet(filepath, voxelSize, null);
			double xMax = 2;
			double yMax = 1;
			double step = 0.01;

			Point[,] matrix = new Point[(int)(xMax / step), (int)(yMax / step)];

			for(int x = 0; x < xMax / step; x++)
			{
				for(int y = 0; y < yMax / step; y++)
				{
					matrix[x, y] = Interpolator<Point>.NNInterpolatePoint(new Vector3((float)(x *step), (float)(y*step), 0), ids.GetPoints());
				}
			}
			int pointSize = sizeof(float) * 6;
			byte[] serialized = new byte[matrix.Length * pointSize];
			int i = 0;
			foreach (Point p in matrix)
			{
				if (p != null)
					Array.Copy(p.Serialize(), 0, serialized, i * pointSize, pointSize);
				i++;
			}
			using (FileStream fileStream = new FileStream("./NN.dat", FileMode.Create))
			{
				fileStream.Write(serialized, 0, serialized.Length);
			}




			for (int x = 0; x < xMax / step; x++)
			{
				for (int y = 0; y < yMax / step; y++)
				{
					matrix[x, y] = ids.GetPoint(new Vector3((float)(x * step), (float)(y * step), 0));
				}
			}
			serialized = new byte[matrix.Length * pointSize];
			i = 0;
			foreach (Point p in matrix)
			{
				if(p != null)
					Array.Copy(p.Serialize(), 0, serialized, i * pointSize, pointSize);
				i++;
			}
			using (FileStream fileStream = new FileStream("./IDW.dat", FileMode.Create))
			{
				fileStream.Write(serialized, 0, serialized.Length);
			}

		}

		private static void InterpolationTest()
		{
			string filepath = "D:/Users/yrmal/Desktop/doublegyro";
			double voxelSize = 0.1;
			InputDataSet ids = new InputDataSet(filepath, voxelSize, null);
			Debug.WriteLine("voxels: " + ids.Voxels.Count() + " points/voxel " + ids.GetPoints().Count() / ids.Voxels.Count());
			Vector3 pos = new Vector3(0.390000f, 0.880000f, 1.000000f);
			Point orig = new Point(pos, new Vector3(-0.003917f, -0.027478f, 0.000000f));
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Point p = null;
			for (int i = 0; i < 1000; i++)
			{
				p = Interpolator<Point>.NNInterpolatePoint(pos, ids.GetPoints());
			}
			sw.Stop();
			Debug.WriteLine("Nearest Neighbour: Time spent " + sw.ElapsedMilliseconds + " ms => " + (sw.ElapsedMilliseconds / 1000f) + " ms/interpolation");
			Debug.WriteLine(p.Vel + " vs " + orig.Vel);
			Vector3 error = (p.Vel - orig.Vel);
			float totalError = Math.Abs(error.X) + Math.Abs(error.Y) + Math.Abs(error.Z);
			Debug.WriteLine("Error: " + error + " Total: " + (1000 * totalError));

			sw.Restart();
			p = null;
			for (int i = 0; i < 1000; i++)
			{
				p = Interpolator<Point>.IWDInterpolatePoint(pos, ids.GetPoints(), voxelSize);
			}
			sw.Stop();
			Debug.WriteLine("IWD: Time spent " + sw.ElapsedMilliseconds + " ms => " + (sw.ElapsedMilliseconds / 1000f) + " ms/interpolation");
			Debug.WriteLine(p.Vel + " vs " + orig.Vel);
			error = (p.Vel - orig.Vel);
			totalError = Math.Abs(error.X) + Math.Abs(error.Y) + Math.Abs(error.Z);
			Debug.WriteLine("Error: " + error + " Total: " + (1000 * totalError));

			sw.Restart();
			p = null;
			for (int i = 0; i < 1000; i++)
			{
				List<Voxel<Point>> voxels = new List<Voxel<Point>>(27);
				Voxel<Point> center = null;
				foreach (Voxel<Point> v in ids.Voxels)
				{
					if (v.IsPointInside(pos))
					{
						center = v;
						voxels.Add(center);
						break;
					}
				}
				foreach (Voxel<Point> v in ids.Voxels)
				{
					if (center != v && v.IsNeighborOf(center))
					{
						voxels.Add(v);
					}
				}
				p = Interpolator<Point>.NNInterpolatePoint(pos, voxels);
			}
			sw.Stop();
			Debug.WriteLine("NN /w voxel: Time spent " + sw.ElapsedMilliseconds + " ms => " + (sw.ElapsedMilliseconds / 1000f) + " ms/interpolation");
			Debug.WriteLine(p.Vel + " vs " + orig.Vel);
			error = (p.Vel - orig.Vel);
			totalError = Math.Abs(error.X) + Math.Abs(error.Y) + Math.Abs(error.Z);
			Debug.WriteLine("Error: " + error + " Total: " + (1000 * totalError));

			sw.Restart();
			p = null;
			for (int i = 0; i < 1000; i++)
			{
				List<Voxel<Point>> voxels = new List<Voxel<Point>>(27);
				Voxel<Point> center = null;
				foreach (Voxel<Point> v in ids.Voxels)
				{
					if (v.IsPointInside(pos))
					{
						center = v;
						voxels.Add(center);
						break;
					}
				}
				foreach (Voxel<Point> v in ids.Voxels)
				{
					if (center != v && v.IsNeighborOf(center))
					{
						voxels.Add(v);
					}
				}
				p = Interpolator<Point>.IWDInterpolatePoint(pos, voxels, voxelSize);
			}
			sw.Stop();
			Debug.WriteLine("IWD /w voxel: Time spent " + sw.ElapsedMilliseconds + " ms => " + (sw.ElapsedMilliseconds / 1000f) + " ms/interpolation");
			Debug.WriteLine(p.Vel + " vs " + orig.Vel);
			error = (p.Vel - orig.Vel);
			totalError = Math.Abs(error.X) + Math.Abs(error.Y) + Math.Abs(error.Z);
			Debug.WriteLine("Error: " + error + " Total: " + (1000 * totalError));
		}
	}
}
