using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class StartUp : MonoBehaviour
{
	public GameObject PointPrefab;
	private DataSet ds = null;
	Dictionary<double[], double> points;
	Painter painter;
	public bool loaddata = false;
	public string datafolder = "D:/UnityWS/AdvectionCalc/AdvectionCalc/bin/Debug/output/";

	public bool debug = true;
	public string debugline = "*";

	public bool draw = false;
	public double resolution = 0.01;
	public double MAX_OVERRIDE = double.NaN;
	public double MIN_OVERRIDE = double.NaN;

	// Start is called before the first frame update
	void Start()
	{
		painter = GameObject.Find("Painter").GetComponent<Painter>();
		/*ds = new DataSet("./Assets/inputdata/");
		Thread t = new Thread(createPoints);
		t.Start();*/
	}

	// Update is called once per frame	
	void Update()
	{
		if(loaddata)
		{
			ds = new DataSet(datafolder);
			loaddata = false;
		}
		if(draw)
		{
			Thread t = new Thread(createPoints);
			t.Start();
			draw = false;
		}
	}

	private void createPoints()
	{
		Thread.Sleep(2000);
		if (debug)
		{
			if (debugline.Equals("*") || debugline.Equals("all"))
			{
				List<Seed> seeds = ds.GetPoints();
				Debug.Log("Seed count: " + seeds.Count);
				foreach (Seed s in seeds)
				{
					//Debug.Log("Pos: " + s.pos[0] + " " + s.pos[1]);
					painter.UpdateObject(new List<object> { s.pos, s.pos[0] + " " + s.pos[1], Color.white, 0.01 });
				}
			}
			else
			{
				try
				{
					List<Seed> seeds = ds.GetLine(int.Parse(debugline));
					foreach (Seed s in seeds)
					{
						//Debug.Log("Pos: " + s.pos[0] + " " + s.pos[1]);
						painter.UpdateObject(new List<object> { s.pos, s.pos[0] + " " + s.pos[1], Color.white, 0.01 });
					}
				}
				catch
				{
					Debug.Log(debugline + " is not a number or '*'");
				}
			}
		}
		else
		{
			List<Seed> seeds = ds.GetPoints();
			Debug.Log("Seed count: " + seeds.Count);
			double MAX = double.NegativeInfinity;
			double MIN = double.PositiveInfinity;
			foreach (Seed s in seeds)
			{
				MAX = Math.Max(MAX, s.FTLE);
				MIN = Math.Min(MIN, s.FTLE);
			}

			if(!double.IsNaN(MAX_OVERRIDE))
			{
				Debug.Log("MAX [" + MAX + "] was overriden with " + MAX_OVERRIDE);
				MAX = MAX_OVERRIDE;
			}
			if (!double.IsNaN(MIN_OVERRIDE))
			{
				Debug.Log("MIN [" + MIN + "] was overriden with " + MIN_OVERRIDE);
				MIN = MIN_OVERRIDE;
			}

			double Diff = MAX - MIN;
			Debug.Log("MAX: " + MAX + "  MIN: " + MIN);

			double dimX = 2;
			double dimY = 1;
			



			points = new Dictionary<double[], double>();
			for (double i = 0; i < dimX; i = Math.Round(i + resolution, 4))
			{
				for (double j = 0; j < dimY; j = Math.Round(j + resolution, 4))
				{
					double[] pos = new double[3] { i, j, 0 };
					Thread t = new Thread(CalculatePoint);
					t.Start(new List<object> { pos, MIN, MAX, seeds });
				}
			}
		}

	}

	private void CalculatePoint(object pos_min_max_seeds)
	{
		List<object> param = (List<object>)pos_min_max_seeds;
		double[] pos = (double[])param[0];
		double MIN = (double)param[1];
		double MAX = (double)param[2];
		List<Seed> seeds = (List<Seed>)param[3];
		double Diff = MAX - MIN;
		
		double FTLE = Interpolator.Interpolate(pos, seeds, 0.5);
		points.Add(pos, FTLE);

		double val = (FTLE - MIN) / Diff;
		Color c;
		if (val < 0.5)
		{
			c = Color.Lerp(Color.blue, Color.green, (float)(val * 2));
		}
		else
		{
			c = Color.Lerp(Color.green, Color.red, (float)(val - 0.5) * 2);
		}
		painter.UpdateObject(new List<object> { pos, pos[0] + " " + pos[1], c, resolution });
	}
}
