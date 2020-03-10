using AdvectionCalc.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

public class Advection
{
	public InputDataSet DS {get; private set;}
	public FTLEDataSet FDS { get; private set; }
	private double dt = 0.05;
	private List<Seed> seeds;

	public Advection(InputDataSet DS, FTLEDataSet FDS)
	{
		this.DS = DS;
		this.FDS = FDS;
	}

	
	private Semaphore semaphore;
	private object fdslock = new object();

	public void Start(object Radius_Steps_DT_EntryPoints_NThreads)
	{
		List<object> param = (List<object>)Radius_Steps_DT_EntryPoints_NThreads;
		float radius = (float)param[0];
		int steps = (int)param[1];
		dt = (double)param[2];
		List<Point> entryPoints = (List<Point>)param[3];
		int threadCount = (int)param[4];

		List<Thread> threads = new List<Thread>(threadCount);

		seeds = new List<Seed>();

		semaphore = new Semaphore(threadCount, threadCount);
		Console.WriteLine("Total points: " + entryPoints.Count);

		

		for (int i = 0; i < entryPoints.Count; i++)
		{
			Point p = entryPoints[i];
			Thread t = new Thread(CalculateSeed);
			t.Start(new List<object> { i, p, steps, dt, radius });
			threads.Add(t);
		}

		foreach(Thread t in threads)
		{
			t.Join();
		}

		Console.WriteLine("All Done!");
	}

	private void CalculateSeed(object ID_Point_Steps_dt_radius)
	{
		List<object> param = (List<object>)ID_Point_Steps_dt_radius;
		int ID = (int)param[0];
		Point entry = (Point)param[1];
		int steps = (int)param[2];
		double dt = (double)param[3];
		float radius = (float)param[4];

		Seed seed;
		int startstep = 0;
		byte[] output = new byte[0];

		if (File.Exists("./output/ID_" + ID + ".dat"))
		{
			int npoints = 5;
			int nparam = 6;
			int pointSize = nparam * sizeof(double);
			int pointsSize = npoints * pointSize;
			int lineSize = pointsSize + sizeof(double);

			byte[] data = File.ReadAllBytes("./output/ID_" + ID + ".dat");

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

			seed = new Seed(points, radius);
			Console.WriteLine("File exists. Start with step " + startstep);
		}
		else
		{
			seed = new Seed(entry, radius, DS);
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

		
		using (FileStream fileStream = new FileStream("./output/ID_" + ID + ".dat", FileMode.Append))
		{
			fileStream.Write(output, 0, output.Length);
		}

	}
}
