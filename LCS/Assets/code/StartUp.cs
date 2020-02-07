using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour
{
	public GameObject PointPrefab;
	// Start is called before the first frame update
	void Start()
	{
		DataSet ds = new DataSet("Assets/data/", "doublegyro", 0.5);
		CreatePoints(ds);
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log("Game objects: " + GameObject.FindGameObjectsWithTag("Point").Length);
	}

	void CreatePoints(DataSet ds)
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

		/*for (int i = 0; i < points.Count; i++)
		{
			DataEntry de = setList[i];
			GameObject p = Instantiate(PointPrefab);
			p.transform.position = new Vector3((float)de.position[0], (float)de.position[1], (float)de.position[2]);
			if (start.Equals(end))
			{
				p.GetComponent<Renderer>().material.color = start;
			}
			else
			{
				p.GetComponent<Renderer>().material.color = Color.Lerp(start, end, (float)i / (float)(setList.Count));
			}
		}*/
	}
}
