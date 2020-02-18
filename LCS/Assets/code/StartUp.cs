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
	public bool StartAdvection = false;
	public int steps = 300;
	public double dt = 0.03;
	public double radius = 0.001;

	private Advection adv;
	// Start is called before the first frame update
	void Start()
	{
		Thread t = new Thread(LoadData);
		t.Start(new List<object> { "./Assets/data/", "doublegyro", 0.5 });
	}

	// Update is called once per frame	
	void Update()
	{
		if(StartAdvection)
		{
			if(ds != null)
			{
				StartAdvection = false;
				adv = new Advection(ds);
				Thread t = new Thread(adv.Start);
				t.Start(new List<double> { radius, steps, dt }); //TODO
			}
		}
	}

	/// <summary>
	/// Threaded function since loading of data from a file might take some time. See parameter description for more details.
	/// </summary>
	/// <param name="FPathFNameVSize">List[object] {string filepath, string filename, double voxelSize}</param>
	private void LoadData(object FPathFNameVSize)
	{
		List<object> pathNnameNsize = (List<object>)FPathFNameVSize;
		ds = new DataSet((string)pathNnameNsize[0], (string)pathNnameNsize[1], (double)pathNnameNsize[2]);
	}

	/*void CreatePoints(DataSet ds)
	{
		double maxv = double.NegativeInfinity;
		double minv = double.PositiveInfinity;
		foreach (Point p in ds.GetPoints())
		{
			double vel = Math.Sqrt(Math.Pow(p.vel[0], 2) + Math.Pow(p.vel[1], 2));
			if (vel > maxv)
				maxv = vel;
			if (vel < minv)
				minv = vel;
		}
		foreach (Point p in ds.GetPoints())
		{
			GameObject go = Instantiate(PointPrefab);
			go.transform.position = new Vector3((float)p.pos[0], (float)p.pos[1], (float)p.pos[2]);
			double vel = Math.Sqrt(Math.Pow(p.vel[0],2) + Math.Pow(p.vel[1], 2));
			go.GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.green, (float)(vel/ (maxv - minv)));
			//Debug.Log(vel);
		}
	}*/
}
