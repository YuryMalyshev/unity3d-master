using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour
{
	public GameObject PointPrefab;
	private DataSet ds;
	// Start is called before the first frame update
	void Start()
	{
		ds = new DataSet("Assets/data/", "doublegyro", 0.5);
		CreatePoints(ds);
		double[] pos = { 0.5, 0.1, 0.000000 };
		Point p;
		p = Interpolator.Interpolate(pos, ds.GetPoints(), ds.voxelSize);
		Debug.Log(p);
		startPoints.Add(p);
		pos[1] = 0.2;
		p = Interpolator.Interpolate(pos, ds.GetPoints(), ds.voxelSize);
		Debug.Log(p);
		startPoints.Add(p);
		pos[1] = 0.35;
		p = Interpolator.Interpolate(pos, ds.GetPoints(), ds.voxelSize);
		Debug.Log(p);
		startPoints.Add(p);
		pos[1] = 0.55;
		p = Interpolator.Interpolate(pos, ds.GetPoints(), ds.voxelSize);
		Debug.Log(p);
		startPoints.Add(p);
		pos[1] = 0.6;
		p = Interpolator.Interpolate(pos, ds.GetPoints(), ds.voxelSize);
		Debug.Log(p);
		startPoints.Add(p);
		Debug.Log(startPoints.Count);
		foreach(Point pr in startPoints)
		{
			Debug.Log(pr);
		}
	}

	// Update is called once per frame
	int step = 0;
	List<Point> startPoints = new List<Point>();
	double dt = 0.05;
	void Update()
	{
		if(step < 2000)
		{
			step++;
			for(int i = 0; i < startPoints.Count; i++)
			{
				startPoints[i] = Interpolator.Interpolate(Interpolator.OneStep(startPoints[i], dt), ds.GetPoints(), ds.voxelSize);
				//Debug.Log(startPoints[i]);
				GameObject go = Instantiate(PointPrefab);
				go.transform.position = new Vector3((float)startPoints[i].pos[0], (float)startPoints[i].pos[1], (float)(startPoints[i].pos[2] - 0.1));
				double vel = Math.Sqrt(Math.Pow(startPoints[i].vel[0], 2) + Math.Pow(startPoints[i].vel[1], 2));
				go.GetComponent<Renderer>().material.color = Color.blue;
			}
		}
	}

	double maxv = double.NegativeInfinity;
	double minv = double.PositiveInfinity;

	void CreatePoints(DataSet ds)
	{
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
	}
}
