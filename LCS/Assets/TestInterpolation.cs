using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Assets.code;
using System.Collections.Generic;

public class TestInterpolation : MonoBehaviour
{
	public GameObject point;
	// Use this for initialization
	void Start()
	{
		int pointSize = sizeof(float) * 6;
		byte[] subset = new byte[pointSize];
		byte[] bytes = File.ReadAllBytes("D:/UnityWS/unity3d-master/AdvectionCalculationsGUI/AdvectionCalculationsGUI/bin/Debug/NN.dat");
		List<Point> points = new List<Point>(bytes.Length / pointSize);
		float minVel = float.PositiveInfinity;
		float maxVel = float.NegativeInfinity;
		for (int i = 0; i < bytes.Length; i += pointSize)
		{
			Array.Copy(bytes, i, subset, 0, subset.Length);
			Point p = Point.DeSerialize(subset);
			points.Add(p);
			if (p.Vel.Length() < minVel)
			{
				minVel = p.Vel.Length();
			}
			if (p.Vel.Length() > maxVel)
			{
				maxVel = p.Vel.Length();
			}
		}
		float velRange = maxVel - minVel;
		Color c;
		foreach(Point p in points)
		{
			float num = (float)((p.Vel.Length() - minVel) / velRange);
			if (num > 0.5)
			{
				c = Color.Lerp(Color.green, Color.red, (num - 0.5f) * 2);
			}
			else
			{
				c = Color.Lerp(Color.blue, Color.green, num * 2);
			}
			GameObject o = Instantiate(point);
			o.transform.position = new Vector3(p.Pos.X, p.Pos.Y, 0);
			o.GetComponent<Renderer>().material.color = c;
		}

		
		pointSize = sizeof(float) * 6;
		subset = new byte[pointSize];
		bytes = File.ReadAllBytes("D:/UnityWS/unity3d-master/AdvectionCalculationsGUI/AdvectionCalculationsGUI/bin/Debug/IDW.dat");
		points = new List<Point>(bytes.Length / pointSize);
		minVel = float.PositiveInfinity;
		maxVel = float.NegativeInfinity;
		for (int i = 0; i < bytes.Length; i += pointSize)
		{
			Array.Copy(bytes, i, subset, 0, subset.Length);
			Point p = Point.DeSerialize(subset);
			points.Add(p);
			if (p.Vel.Length() < minVel)
			{
				minVel = p.Vel.Length();
			}
			if (p.Vel.Length() > maxVel)
			{
				maxVel = p.Vel.Length();
			}
		}
		velRange = maxVel - minVel;
		foreach (Point p in points)
		{
			float num = (float)((p.Vel.Length() - minVel) / velRange);
			if (num > 0.5)
			{
				c = Color.Lerp(Color.green, Color.red, (num - 0.5f) * 2);
			}
			else
			{
				c = Color.Lerp(Color.blue, Color.green, num * 2);
			}
			GameObject o = Instantiate(point);
			o.transform.position = new Vector3(p.Pos.X+6, p.Pos.Y, 0);
			o.GetComponent<Renderer>().material.color = c;
		}





		pointSize = sizeof(float) * 6;
		subset = new byte[pointSize];
		bytes = File.ReadAllBytes("D:/UnityWS/unity3d-master/AdvectionCalculationsGUI/AdvectionCalculationsGUI/bin/Debug/ORIG.dat");
		points = new List<Point>(bytes.Length / pointSize);
		minVel = float.PositiveInfinity;
		maxVel = float.NegativeInfinity;
		for (int i = 0; i < bytes.Length; i += pointSize)
		{
			Array.Copy(bytes, i, subset, 0, subset.Length);
			Point p = Point.DeSerialize(subset);
			points.Add(p);
			if (p.Vel.Length() < minVel)
			{
				minVel = p.Vel.Length();
			}
			if (p.Vel.Length() > maxVel)
			{
				maxVel = p.Vel.Length();
			}
		}
		velRange = maxVel - minVel;
		foreach (Point p in points)
		{
			float num = (float)((p.Vel.Length() - minVel) / velRange);
			if (num > 0.5)
			{
				c = Color.Lerp(Color.green, Color.red, (num - 0.5f) * 2);
			}
			else
			{
				c = Color.Lerp(Color.blue, Color.green, num * 2);
			}
			GameObject o = Instantiate(point);
			o.transform.position = new Vector3(p.Pos.X+12, p.Pos.Y, 0);
			o.GetComponent<Renderer>().material.color = c;
		}

		pointSize = sizeof(float) * 6;
		subset = new byte[pointSize];
		bytes = File.ReadAllBytes("D:/UnityWS/unity3d-master/AdvectionCalculationsGUI/AdvectionCalculationsGUI/bin/Debug/ORIG_ROUGH.dat");
		points = new List<Point>(bytes.Length / pointSize);
		minVel = float.PositiveInfinity;
		maxVel = float.NegativeInfinity;
		for (int i = 0; i < bytes.Length; i += pointSize)
		{
			Array.Copy(bytes, i, subset, 0, subset.Length);
			Point p = Point.DeSerialize(subset);
			points.Add(p);
			if (p.Vel.Length() < minVel)
			{
				minVel = p.Vel.Length();
			}
			if (p.Vel.Length() > maxVel)
			{
				maxVel = p.Vel.Length();
			}
		}
		velRange = maxVel - minVel;
		foreach (Point p in points)
		{
			float num = (float)((p.Vel.Length() - minVel) / velRange);
			if (num > 0.5)
			{
				c = Color.Lerp(Color.green, Color.red, (num - 0.5f) * 2);
			}
			else
			{
				c = Color.Lerp(Color.blue, Color.green, num * 2);
			}
			GameObject o = Instantiate(point);
			o.transform.position = new Vector3(p.Pos.X - 6, p.Pos.Y, 0);
			o.GetComponent<Renderer>().material.color = c;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
