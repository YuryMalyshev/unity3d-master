using AdvectionCalc.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

public class Advection_DEPRECATED
{
	public InputDataSet_DEPRECATED DS {get; private set;}
	public FTLEDataSet_DEPRECATED FDS { get; private set; }
	//private double dt = 0.05;
	private List<Seed_DEPRECATED> seeds;

	public Advection_DEPRECATED(InputDataSet_DEPRECATED DS, FTLEDataSet_DEPRECATED FDS)
	{
		this.DS = DS;
		this.FDS = FDS;
	}

	
	private Semaphore semaphore;
	private object fdslock = new object();
	private ManualResetEvent notifier;
	private string folder;

	public List<Thread> Start(float radius, int steps, double dt, List<Point> entryPoints, int maxThreads, string folder, ManualResetEvent notifier)//object Radius_Steps_DT_EntryPoints_NThreads)
	{
		this.folder = folder;
		this.notifier = notifier;

		List<Thread> threads = new List<Thread>(maxThreads);

		seeds = new List<Seed_DEPRECATED>();

		semaphore = new Semaphore(maxThreads, maxThreads);
		Console.WriteLine("Total points: " + entryPoints.Count);

		

		for (int i = 0; i < entryPoints.Count; i++)
		{
			Point p = entryPoints[i];
			Thread t = new Thread(CalculateSeed);
			t.Start(new List<object> { i, p, steps, dt, radius });
			threads.Add(t);
		}
		return threads;
	}

	private void CalculateSeed(object ID_Point_Steps_dt_radius)
	{
		List<object> param = (List<object>)ID_Point_Steps_dt_radius;
		int ID = (int)param[0];
		Point entry = (Point)param[1];
		int steps = (int)param[2];
		double dt = (double)param[3];
		float radius = (float)param[4];

		Seed_DEPRECATED seed;
		int startstep = 0;
		byte[] output = new byte[0];

		if (File.Exists(folder + "/ID_" + ID + ".dat"))
		{
			int npoints = 5;
			int nparam = 6;
			int pointSize = nparam * sizeof(double);
			int pointsSize = npoints * pointSize;
			int lineSize = pointsSize + sizeof(double);

			byte[] data = File.ReadAllBytes(folder + "/ID_" + ID + ".dat");

			int lastStepStart = data.Length - lineSize;
			startstep = data.Length / lineSize;

			List<Point> points = new List<Point>(npoints);
			for (int j = lastStepStart; j < lastStepStart + pointsSize; j += pointSize)
			{
				double[] pos = new double[nparam / 2];
				double[] vel = new double[nparam / 2];
				for (int k = 0; k < nparam / 2; k++)
				{
					pos[k] = BitConverter.ToDouble(data, j + sizeof(double) * k);
					vel[k] = BitConverter.ToDouble(data, j + sizeof(double) * (k + nparam / 2));
				}
				points.Add(new Point(pos, vel));
			}

			seed = new Seed_DEPRECATED(points, radius);
			Console.WriteLine("File exists. Start with step " + startstep);
		}
		else
		{
			seed = new Seed_DEPRECATED(entry, radius, DS);
		}

		double maxFTLE = double.NegativeInfinity;
		for (int i = startstep; i < steps; i++)
		{
			semaphore.WaitOne();
			double FTLE = Math.Log(seed.Calculate(DS, dt));

			if (double.IsInfinity(FTLE))
			{
				Console.WriteLine("[WARNING] FTLE is INF for " + ID + " step " + i);
				FTLE = 0;
			}

			if (FTLE > maxFTLE)
			{
				maxFTLE = FTLE;
				seed.FTLE = maxFTLE;
			}

			output = output.Concat(seed.Serialize()).ToArray();
			output = output.Concat(BitConverter.GetBytes(FTLE)).ToArray();
			semaphore.Release();
			if ((100 * (float)(i - startstep) / (steps - startstep)) % 25 == 0)
				Console.WriteLine(ID + "\t:\t" + (100 * (i - startstep) / (steps - startstep)) + " % Done");

			lock (fdslock)
			{
				FDS.AddPoint(seed.Clone(), ID);
			}
		}

		
		using (FileStream fileStream = new FileStream(folder  + "/ID_" + ID + ".dat", FileMode.Append))
		{
			fileStream.Write(output, 0, output.Length);
		}

		notifier.Set();
	}
}
