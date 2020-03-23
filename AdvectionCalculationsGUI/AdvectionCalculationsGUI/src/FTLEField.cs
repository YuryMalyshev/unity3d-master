﻿using System;
using System.Collections.Generic;
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
		private List<Square<SeedPoint>> squares;
		private object squaresLock = new object();
		public FTLEField(InputDataSet ids, int resolution)
		{
			foreach(Voxel<Point> v in ids.Voxels)
			{
				voxels.Add(new OrderedVoxel<SeedPoint>(v.vertices, resolution));
			}
			squares = new List<Square<SeedPoint>>();
		}

		List<SeedPoint> bulkList = null;
		readonly object bulkListLock = new object();
		public List<Thread> PrepareField(ManualResetEvent notifier, StreamLines sls)
		{
			List<StreamLine> streamLines = sls.GetStreamLines();
			if (streamLines.Count > 0)
				bulkList = new List<SeedPoint>(streamLines.Count * streamLines[0].Points.Count);
			else
			{
				Debug.WriteLine("[WARNING] No streamsLines were created previously");
				bulkList = new List<SeedPoint>();
			}
			List<Thread> threads = new List<Thread>(streamLines.Count);
			foreach(StreamLine sl in streamLines)
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
			foreach(SeedPoint sp in sl.Points)
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
			notifier.Set();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="notifier"></param>
		/// <param name="resolution"> amount of points on an edge of a voxel</param>
		/// <returns></returns>
		public List<Thread> Start(ManualResetEvent notifier, int resolution)
		{
			if(bulkList == null)
			{
				throw new Exception("StreamLines not yet loaded. Call PrepareField(...) first!");
			}
			List<Thread> threads = new List<Thread>(voxels.Count);
			foreach(OrderedVoxel<SeedPoint> v in voxels)
			{
				Thread t = new Thread(PopulateVoxel);
				t.Start(new List<object> { v, notifier, resolution });
				threads.Add(t);
			}
			return threads;
		}

		private void PopulateVoxel(object voxel_norifier_resolution)
		{
			List<object> param = (List<object>)voxel_norifier_resolution;
			OrderedVoxel<SeedPoint> voxel = (OrderedVoxel<SeedPoint>)param[0];
			ManualResetEvent notifier = (ManualResetEvent)param[1];
			int resolution = (int)param[2];
			float step = (float)(voxel.GetVoxelDimension() / resolution);
			for (int x = 0; x < resolution; x++)
			{
				for(int y = 0; y < resolution; y ++)
				{
					for (int z = 0; z < resolution; z ++)
					{
						SeedPoint sp = Interpolator<SeedPoint>.NNInterpolatePoint(
							new Vector3((x * step) + voxel.vertices[3].X,
											(y * step) + voxel.vertices[3].Y, 
											(z * step) + voxel.vertices[3].Z), 
							bulkList);
						voxel.AddPoint(sp);
						voxel.AddPointAt(sp, x, y, z);
					}
				}
			}
			//TODO: create voxel polygons
			List<SeedPoint> vertices = new List<SeedPoint>(8);
			List<Square<SeedPoint>> voxelSquares = new List<Square<SeedPoint>>();
			for (int x = 0; x < resolution; x++)
			{
				for (int y = 0; y < resolution; y++)
				{
					for (int z = 0; z < resolution; z++)
					{
						vertices.Clear();
						vertices.Add(voxel.GetPointAt(x  , y  , z  ));
						vertices.Add(voxel.GetPointAt(x+1, y  , z  ));
						vertices.Add(voxel.GetPointAt(x  , y+1, z  ));
						vertices.Add(voxel.GetPointAt(x+1, y+1, z  ));
						vertices.Add(voxel.GetPointAt(x  , y  , z+1));
						vertices.Add(voxel.GetPointAt(x+1, y  , z+1));
						vertices.Add(voxel.GetPointAt(x  , y+1, z+1));
						vertices.Add(voxel.GetPointAt(x+1, y+1, z+1));

						voxelSquares.Add(new Square<SeedPoint>(vertices[0], vertices[2], vertices[4], vertices[6]));
						voxelSquares.Add(new Square<SeedPoint>(vertices[1], vertices[3], vertices[5], vertices[7]));

						voxelSquares.Add(new Square<SeedPoint>(vertices[0], vertices[1], vertices[4], vertices[5]));
						voxelSquares.Add(new Square<SeedPoint>(vertices[2], vertices[3], vertices[6], vertices[7]));

						voxelSquares.Add(new Square<SeedPoint>(vertices[0], vertices[1], vertices[2], vertices[3]));
						voxelSquares.Add(new Square<SeedPoint>(vertices[4], vertices[5], vertices[6], vertices[7]));
					}
				}
			}
			lock(squaresLock)
			{
				foreach(Square<SeedPoint> s in voxelSquares)
				{
					if (s.IsComplete())
						squares.Add(s);
				}
			}
			voxelSquares.Clear();
			notifier.Set();
		}

		public void Serialize(string path)
		{
			List<SeedPoint> points = new List<SeedPoint>(voxels.Count);
			byte[] temp = new byte[0];
			foreach (Voxel<SeedPoint> v in voxels)
			{
				foreach(SeedPoint s in v.Points)
				{
					temp = temp.Concat(BitConverter.GetBytes(points.Count)).ToArray();
					temp = temp.Concat(s.Serialize()).ToArray();
					points.Add(s);
				}
			}
			using (FileStream fileStream = new FileStream(path + "/FTLEField.dat", FileMode.Create))
			{
				fileStream.Write(temp, 0, temp.Length);
			}
			temp = new byte[0];
			foreach (Square<SeedPoint> s in squares)
			{
				temp = temp.Concat(BitConverter.GetBytes(points.IndexOf(s.A))).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(points.IndexOf(s.B))).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(points.IndexOf(s.C))).ToArray();
				temp = temp.Concat(BitConverter.GetBytes(points.IndexOf(s.D))).ToArray();
			}
			using (FileStream fileStream = new FileStream(path + "/Squares.dat", FileMode.Create))
			{
				fileStream.Write(temp, 0, temp.Length);
			}
		}

	}
}