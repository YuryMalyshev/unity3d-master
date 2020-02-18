using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Advection
{
	public DataSet DS {get; private set;}
	private readonly Painter painter;
	private ManualResetEvent mre = new ManualResetEvent(false);
	private double dt = 0.05;
	/// <summary>
	/// Step, (ID, Value)
	/// </summary>
	private Dictionary<int, Dictionary<int, double>> ftleout;
	/// <summary>
	/// ID, Seed
	/// </summary>
	private Dictionary<int, Seed> seeds;
	private readonly object listLock = new object();

	private int totalExpectedSteps;
	private int stepsMade;

	public Advection(DataSet DS)
	{
		this.DS = DS;
		painter = GameObject.Find("Painter").GetComponent<Painter>();
	}

	
	private Semaphore semaphore;

	public void Start(object RadiusStepsDT)
	{
		Debug.Log("Print 'TEST': ");
		painter.test();

		/*int iter = 0;
		foreach(Point p in DS.GetPoints())
		{
			painter.UpdateObject(new List<object> { new double[] { p.pos[0], p.pos[1], 0.1 }, iter + "_ini", Color.white });
			iter++;
		}*/


		List<double> param = (List<double>)RadiusStepsDT;
		double radius = param[0];
		int steps = (int)param[1];
		dt = param[2];

		seeds = new Dictionary<int, Seed>();
		List<Thread> threads = new List<Thread>();

		ftleout = new Dictionary<int, Dictionary<int, double>>(steps);

		int fraction = 5;
		int pauseCount = 20;
		semaphore = new Semaphore(pauseCount, pauseCount);
		totalExpectedSteps = 0;
		stepsMade = 0;
		Debug.Log("Total points: " + DS.GetPoints().Count);
		for(int ID = 0; ID < DS.GetPoints().Count; ID++)
		{
			Point p = DS.GetPoints()[ID];
			if ((int)Math.Round(p.pos[0] * 100) % fraction == 0 && (int)Math.Round(p.pos[1] * 100) % fraction == 0)
			{
				try
				{
					Seed s = new Seed(p, radius, DS);
					seeds.Add(ID, s);
					Thread worker = new Thread(CalculateSingleSeed);
					threads.Add(worker);
					worker.Start(new List<object> { ID, s, steps });
					ID++;
					totalExpectedSteps += steps;
					if (ID % pauseCount == 0)
					{
						//Debug.Log("Added " + ID + "/" + totalRough + " seeds!");
						//Thread.Sleep(pauseCount*steps*3);
					}
				}
				catch (Exception e)
				{
					//Debug.Log(e.Message);
				}
			}
		}

		int count = 0;
		foreach (Thread t in threads)
		{
			//Debug.Log("Tying to Join!");
			t.Join();
			count++;
			if(count % pauseCount == 0)
			{
				Debug.Log("Joined! " + count + "/" + threads.Count);
			}
			//Debug.Log("Joined! " + count + "/" + threads.Count);
		}
		Debug.Log("All threads are done!");
		Dictionary<int, double> ID_MaxValue = new Dictionary<int, double>(ftleout.Count);
		lock (listLock)
		{
			foreach (int step in ftleout.Keys)
			{
				Dictionary<int, double> ID_Value = ftleout[step];
				foreach (int id in ID_Value.Keys)
				{
					double val = ID_Value[id];
					if(ID_MaxValue.ContainsKey(id))
					{
						if(ID_MaxValue[id] < val)
						{
							ID_MaxValue[id] = val;
						}
					}
					else
					{
						ID_MaxValue.Add(id, val);
					}
				}
			}
			
		}
		

		double MAX = double.NegativeInfinity;
		double MIN = double.PositiveInfinity;
		double Diff;
		foreach(double val in ID_MaxValue.Values)
		{
			if (val < MIN)
				MIN = val;
			if (val > MAX)
				MAX = val;
		}
		Diff = MAX - MIN;

		Debug.Log(" Min: " + MIN + " Max: " + MAX);

		foreach (int id in ID_MaxValue.Keys)
		{
			double val = (ID_MaxValue[id] - MIN) / Diff;
			Color c = Color.Lerp(Color.green, Color.red, (float)val);
			/*if (val < 0.5)
			{
				c = Color.Lerp(Color.blue, Color.green, (float)(val * 2));
			}
			else
			{
				c = Color.Lerp(Color.green, Color.red, (float)val);
			}*/
			
			painter.UpdateObject(new List<object> { seeds[id].CenterPoint.pos, "" + id, c });
		}
		


		//Start(seeds, steps, dt);
	}

	/*public void Start(List<Seed> seeds, int maxSteps, double dt)
	{
		if(DS == null) //TODO: try with Reset(), probably not needed because DS is needed to create a seed
		{
			Debug.Log("Waiting for the data to load...");
			mre.WaitOne();
		}
		this.dt = dt;

		Debug.Log("Starting calculations...");
		List<Thread> threads = new List<Thread>(seeds.Count);
		ftleout = new List<double>(maxSteps*seeds.Count);
		foreach (Seed s in seeds)
		{
			threads.Add(StartSingle(s, maxSteps));
		}
		Thread t = new Thread(WaitForFTLE);
		t.Start(threads);
	}*/

	/*private Thread StartSingle(Seed seed, int maxSteps)
	{
		Thread worker = new Thread(CalculateSingleSeed);
		worker.Start(new Tuple<Seed, int>(seed, maxSteps));
		return worker;
	}*/

	/// <summary>
	/// Threaded function for calculating N-steps for a seed. See parameter description for more details.
	/// </summary>
	/// <param name="seedAndSteps"> Tuple[Seed, int] {Seed StartSeed, int Steps}  </param>
	private void CalculateSingleSeed(object IDseedAndSteps) //thread with parameters "Seed" and "Int"
	{
		List<object> param = (List<object>)IDseedAndSteps;
		int ID = (int)param[0];
		Seed seed = (Seed)param[1];
		int maxSteps = (int)param[2];

		int userUpdateInterval = maxSteps / 100;
		userUpdateInterval = userUpdateInterval <= 0 ? 1 : userUpdateInterval;

		for (int i = 0; i < maxSteps; i++)
		{
			semaphore.WaitOne();
			double ftleoutsingle = -1 * Math.Log(seed.Calculate(DS, dt));// / Math.Abs(dt * i);
			if(double.IsInfinity(ftleoutsingle))
			{
				//Debug.Log("INF");
				ftleoutsingle = 0;
			}
			else
			{
				//Debug.Log("       NOT INF: " + ftleoutsingle);
			}
			lock (listLock)
			{
				Dictionary<int, double> step;
				if (ftleout.TryGetValue(i, out step)) {}
				else
				{
					step = new Dictionary<int, double>(seeds.Count);
					if(i % userUpdateInterval == 0)
						Debug.Log("Added new step " + i + "/" + maxSteps);
					ftleout.Add(i, step);
				}
				step.Add(ID, ftleoutsingle);
				stepsMade++;
			}

			if (stepsMade % maxSteps == 0)
			{
				Debug.Log("~~~~~Steps: " + stepsMade + "/" + totalExpectedSteps);
			}

			semaphore.Release();

			//799
			if(ID == 5295 || ID == 14795)
			{
				painter.UpdateObject(new List<object> { new double[] { seed.getCurPos()[0], seed.getCurPos()[1], 0.05 }, ID + "_" + i, Color.grey });
			}
		}

	}

	/*private void WaitForFTLE(object threadsList)
	{
		List<Thread> threads = (List<Thread>)threadsList;
		foreach(Thread t in threads)
		{
			t.Join();
		}
		Debug.Log("All threads are done!");
		double minFTLE = double.PositiveInfinity;
		double maxFTLE = double.NegativeInfinity;
		foreach(double f in ftleout)
		{
			if (f < minFTLE)
				minFTLE = f;
			if (f > maxFTLE)
				maxFTLE = f;
		}
	}*/
}
