using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace AdvectionCalculationsGUI.src
{
	public class FTLEField
	{
		private List<OrderedVoxel<SeedPoint>> voxels = new List<OrderedVoxel<SeedPoint>>();
		private OrderedVoxel<SeedPoint>[,,] voxelsMatrix;
		private List<Square<SeedPoint>> squares;
		public FTLEField(InputDataSet ids, int resolution)
		{
			Vector3 min = new Vector3(float.PositiveInfinity);
			Vector3 max = new Vector3(float.NegativeInfinity);
			foreach (Voxel<Point> v in ids.Voxels)
			{
				if (v.vertices[3].X < min.X)
					min.X = v.vertices[3].X;
				if (v.vertices[3].Y < min.Y)
					min.Y = v.vertices[3].Y;
				if (v.vertices[3].Z < min.Z)
					min.Z = v.vertices[3].Z;

				if (v.vertices[4].X > max.X)
					max.X = v.vertices[4].X;
				if (v.vertices[4].Y > max.Y)
					max.Y = v.vertices[4].Y;
				if (v.vertices[4].Z > max.Z)
					max.Z = v.vertices[4].Z;

				voxels.Add(new OrderedVoxel<SeedPoint>(v.vertices, resolution));
			}
			float size = (float)voxels.First().GetVoxelDimension();
			Vector3 matrixSize = Vector3.Divide(Vector3.Subtract(max, min), size);
			voxelsMatrix = new OrderedVoxel<SeedPoint>[(int)matrixSize.X, (int)matrixSize.Y, (int)matrixSize.Z];
			foreach (OrderedVoxel<SeedPoint> v in voxels)
			{
				int x = (int)Math.Round((v.vertices[3].X - min.X) / size);
				int y = (int)Math.Round((v.vertices[3].Y - min.Y) / size);
				int z = (int)Math.Round((v.vertices[3].Z - min.Z) / size);
				voxelsMatrix[x, y, z] = v;
			}
			Debug.WriteLine("Min " + min + " Max " + max);
			squares = new List<Square<SeedPoint>>();
		}

		List<SeedPoint> bulkList = null;
		readonly object bulkListLock = new object();
		private Semaphore semaphore;
		public List<Thread> PrepareField(ManualResetEvent notifier, StreamLines sls, int maxThreads)
		{
			List<StreamLine> streamLines = sls.GetStreamLines();
			if (streamLines.Count > 0)
				bulkList = new List<SeedPoint>(streamLines.Count * streamLines[0].Points.Count);
			else
			{
				Debug.WriteLine("[WARNING] No streamsLines were created previously");
				bulkList = new List<SeedPoint>();
			}
			double minFTLE = double.PositiveInfinity;
			double maxFTLE = double.NegativeInfinity;
			foreach(StreamLine sl in streamLines)
			{
				foreach(SeedPoint sp in sl.Points)
				{
					if(!double.IsNaN(sp.FTLE))
					{
						if (sp.FTLE < minFTLE)
							minFTLE = sp.FTLE;
						if (sp.FTLE > maxFTLE)
							maxFTLE = sp.FTLE;
					}
				}
			}
			Debug.WriteLine("MinFTLE: " + minFTLE + "; MaxFTLE: " + maxFTLE);
			foreach (StreamLine sl in streamLines)
			{
				foreach (SeedPoint sp in sl.Points)
				{
					if (double.IsNaN(sp.FTLE))
					{
						sp.FTLE = maxFTLE;
					}
				}
			}
			List<Thread> threads = new List<Thread>(streamLines.Count);
			semaphore = new Semaphore(maxThreads, maxThreads);
			foreach (StreamLine sl in streamLines)
			{
				Thread t = new Thread(AnalyzeLine);
				t.Start(new List<object> { sl, notifier });
				threads.Add(t);
			}
			return threads;
		}

		private void AnalyzeLine(object SteamLine_notifier)
		{
			List<object> param = (List<object>)SteamLine_notifier;
			StreamLine sl = (StreamLine)param[0];
			ManualResetEvent notifier = (ManualResetEvent)param[1];

			double maxFTLE = double.NegativeInfinity;
			semaphore.WaitOne();
			foreach (SeedPoint sp in sl.Points)
			{
				if(sp.FTLE > maxFTLE)
				{
					maxFTLE = sp.FTLE;
				}
			}
			foreach (SeedPoint sp in sl.Points)
			{
				SeedPoint newSp = new SeedPoint(sp, maxFTLE);
				lock (bulkListLock)
				{
					bulkList.Add(newSp);
				}
			}
			semaphore.Release();
			notifier.Set();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="notifier"></param>
		/// <param name="resolution"> amount of points on an edge of a voxel</param>
		/// <returns></returns>
		public List<Thread> Start(ManualResetEvent notifier, int resolution, int maxThreads)
		{
			if(bulkList == null)
			{
				throw new Exception("StreamLines not yet loaded. Call PrepareField(...) first!");
			}
			semaphore = new Semaphore(maxThreads, maxThreads);
			List<Thread> threads = new List<Thread>(voxels.Count);
			Debug.WriteLine("Count of voxels " + voxels.Count);
			foreach(OrderedVoxel<SeedPoint> v in voxels)
			{
				Thread t = new Thread(PopulateVoxel);
				t.Start(new List<object> { v, notifier, resolution });
				threads.Add(t);
			}
			return threads;
		}

		private void PopulateVoxel(object voxel_notifier_resolution)
		{
			List<object> param = (List<object>)voxel_notifier_resolution;
			OrderedVoxel<SeedPoint> voxel = (OrderedVoxel<SeedPoint>)param[0];
			ManualResetEvent notifier = (ManualResetEvent)param[1];
			int resolution = (int)param[2];
			double diameter = voxel.GetVoxelDimension();
			float step = (float)(diameter / resolution);
			
			for (int x = 0; x < resolution; x++)
			{
				for(int y = 0; y < resolution; y ++)
				{
					semaphore.WaitOne();
					for (int z = 0; z < resolution; z ++)
					{
						Vector3 pos = new Vector3((x * step) + voxel.vertices[3].X,
												(y * step) + voxel.vertices[3].Y,
												(z * step) + voxel.vertices[3].Z);
						SeedPoint sp = Interpolator<SeedPoint>.IWDInterpolatePoint(pos, bulkList, diameter);
						if (sp == null)
						{
							sp = Interpolator<SeedPoint>.NNInterpolatePoint(pos, bulkList);
						}
						voxel.AddPoint(sp);
						voxel.AddPointAt(sp, x, y, z);
					}
					semaphore.Release();
				}
			}
			notifier.Set();
		}

		public void CreateSquares(int resolution, BackgroundWorker worker)
		{
			Debug.WriteLine("Creating squares");
			int total = voxelsMatrix.Length;
			int count = total;
			for (int vx = 0; vx < voxelsMatrix.GetLength(0); vx++)
			{
				for(int vy = 0; vy < voxelsMatrix.GetLength(1); vy++)
				{
					for(int vz = 0; vz < voxelsMatrix.GetLength(2); vz++)
					{
						if (voxelsMatrix[vx, vy, vz] != null)
							FillVertex(resolution, vx, vy, vz);
						count--;
						worker.ReportProgress((total - count) * 1000 / total);
					}
				}
			}
		}

		private void FillVertex(int resolution, int vx, int vy, int vz)
		{
			List<SeedPoint> vertices = new List<SeedPoint>(8);
			for (int x = 0; x < resolution; x++)
			{
				for (int y = 0; y < resolution; y++)
				{
					for (int z = 0; z < resolution; z++)
					{
						vertices.Clear();
						vertices.Add(AddVertex(resolution, vx, vy, vz, x, y, z));
						vertices.Add(AddVertex(resolution, vx, vy, vz, x, y + 1, z));
						vertices.Add(AddVertex(resolution, vx, vy, vz, x + 1, y + 1, z));
						vertices.Add(AddVertex(resolution, vx, vy, vz, x + 1, y, z));
						vertices.Add(AddVertex(resolution, vx, vy, vz, x, y, z + 1));
						vertices.Add(AddVertex(resolution, vx, vy, vz, x, y + 1, z + 1));
						vertices.Add(AddVertex(resolution, vx, vy, vz, x + 1, y + 1, z + 1));
						vertices.Add(AddVertex(resolution, vx, vy, vz, x + 1, y, z + 1));

						Square<SeedPoint> sq;

						sq = new Square<SeedPoint>(vertices[1], vertices[2], vertices[0], vertices[5]);
						if (sq.IsComplete()) squares.Add(sq);
						sq = new Square<SeedPoint>(vertices[3], vertices[0], vertices[2], vertices[7]);
						if (sq.IsComplete()) squares.Add(sq);

						sq = new Square<SeedPoint>(vertices[4], vertices[5], vertices[7], vertices[0]);
						if (sq.IsComplete()) squares.Add(sq);
						sq = new Square<SeedPoint>(vertices[6], vertices[7], vertices[5], vertices[1]);
						if (sq.IsComplete()) squares.Add(sq);
					}
				}
			}
		}

		private SeedPoint AddVertex(int resolution, int vx, int vy, int vz, int x, int y, int z)
		{
			if (voxelsMatrix[vx, vy, vz].TryGetPointAt(x, y, z, out SeedPoint v))
			{
				return v;
			}
			else
			{
				if (x != 0 && (x %= resolution) == 0) // outer edge
				{
					vx++;
					if (vx >= voxelsMatrix.GetLength(0))
						return null;
				}
				if (y != 0 && (y %= resolution) == 0)
				{
					vy++;
					if (vy >= voxelsMatrix.GetLength(1))
						return null;
				}
				if (z != 0 && (z %= resolution) == 0)
				{
					vz++;
					if (vz >= voxelsMatrix.GetLength(2))
						return null;
				}
				if(voxelsMatrix[vx, vy, vz] != null)
				{
					if (voxelsMatrix[vx, vy, vz].TryGetPointAt(x, y, z, out v))
						return v;
				}
			}
			return null;
		}

		byte[] serialized;
		public void Serialize(string path, BackgroundWorker worker)
		{
			Debug.WriteLine("Serializing...");
			worker.ReportProgress(0);
			int total = voxels.Count;
			int count = total;
			List<SeedPoint> points = new List<SeedPoint>(voxels.Count);
			foreach (OrderedVoxel<SeedPoint> v in voxels)
			{
				foreach(SeedPoint s in v.Points)
				{
					points.Add(s);
				}
				count--;
				worker.ReportProgress((total - count) * 250 / total);
			}

			total = points.Count;
			serialized = new byte[total * (sizeof(float) * 6 + sizeof(double) + sizeof(int))];
			for (int i = 0; i < total; i++)
			{
				byte[] temp = new byte[0];
				temp = temp.Concat(BitConverter.GetBytes(i)).ToArray();
				temp = temp.Concat(points[i].Serialize()).ToArray();
				worker.ReportProgress((i * 750 / total)+250);
				Array.Copy(temp, 0, serialized, i * temp.Length, temp.Length);
			}
			using (FileStream fileStream = new FileStream(path + "/FTLEField.dat", FileMode.Create))
			{
				fileStream.Write(serialized, 0, serialized.Length);
			}

			serialized = new byte[squares.Count*4*sizeof(int)];
			worker.ReportProgress(0);
			total = squares.Count;
			count = total;
			int index = 0;
			foreach (Square<SeedPoint> s in squares)
			{
				byte[] temp = new byte[0];
				temp = temp.Concat(BitConverter.GetBytes(points.IndexOf(s.A))).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(points.IndexOf(s.B))).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(points.IndexOf(s.C))).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(points.IndexOf(s.D))).ToArray();
				Array.Copy(temp, 0, serialized, index, temp.Length);
				index += (4 * sizeof(int));
				count--;
				worker.ReportProgress((total - count) * 1000 / total);
			}
			using (FileStream fileStream = new FileStream(path + "/Squares.dat", FileMode.Create))
			{
				fileStream.Write(serialized, 0, serialized.Length);
			}
		}
	}
}