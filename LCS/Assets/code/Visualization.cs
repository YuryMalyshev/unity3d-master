﻿using Assets.code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

class Visualization : MonoBehaviour
{
	public Shader shader;
	//private List<Square<SeedPoint>> squares;
	private List<SquareSimplified> squares;
	int[] meshtriangles;
	//private Dictionary<int, SeedPoint> points;
	private List<SeedPoint> points;

	public double AbsLevelMin = double.PositiveInfinity;
	public double AbsLevelMax = double.NegativeInfinity;
	public double levelRange;
	public bool newtask = false;
	private Surface surface;

	public GameObject point;

	private enum DrawStatus
	{
		notReady, waiting, processing, ready
	}
	private DrawStatus status = DrawStatus.notReady;

	void Start()
	{
		// Load data
		byte[] bytes = File.ReadAllBytes("D:/Users/yrmal/Desktop/FTLEField.dat");
		int lineSize = sizeof(int) + sizeof(float) * 6 + sizeof(double);
		//points = new Dictionary<int, SeedPoint>(bytes.Length / lineSize);
		points = new List<SeedPoint>(bytes.Length / lineSize);
		byte[] subset = new byte[lineSize - sizeof(int)];
		for (int i = 0; i < bytes.Length; i += lineSize)
		{
			int ID = BitConverter.ToInt32(bytes, i);
			Array.Copy(bytes, i + sizeof(int), subset, 0, subset.Length);
			SeedPoint sp = SeedPoint.DeSerialize(subset);
			//points.Add(ID, sp);
			points.Add(sp);
			if (sp.FTLE < AbsLevelMin)
			{
				AbsLevelMin = sp.FTLE;
			}
			if (sp.FTLE > AbsLevelMax)
			{
				AbsLevelMax = sp.FTLE;
			}
		}
		Debug.Log("Min = " + AbsLevelMin + " Max = " + AbsLevelMax);
		levelRange = AbsLevelMax - AbsLevelMin;

		Vector3[] vertices = new Vector3[points.Count];
		Color[] colors = new Color[points.Count];
		SeedPoint temp_sp;
		//int b05 = 0;
		//int a05 = 0;
		for (int i = 0; i < vertices.Length; i++)
		{
			temp_sp = points[i];
			vertices[i] = new Vector3(temp_sp.Pos.X, temp_sp.Pos.Y, temp_sp.Pos.Z);
			//Debug.Log("FTLE " + temp_sp.FTLE + " +Min " + (temp_sp.FTLE - AbsLevelMin));
			float num = (float)((temp_sp.FTLE - AbsLevelMin) / levelRange);
			if (num > 0.5)
			{
				colors[i] = Color.Lerp(Color.green, Color.red, (num - 0.5f) * 2);
				//a05++;
			}
			else
			{
				colors[i] = Color.Lerp(Color.blue, Color.green, num * 2);
				//b05++;
			}
		}
		//Debug.Log("Below 0.5 " + b05 + " Above 0.5 " + a05);
		surface = new Surface(vertices, colors, shader);

		bytes = File.ReadAllBytes("D:/Users/yrmal/Desktop/Squares.dat");
		lineSize = sizeof(int) * 4;
		//squares = new List<Square<SeedPoint>>(bytes.Length / lineSize);
		squares = new List<SquareSimplified>(bytes.Length / lineSize);
		//SeedPoint[] square = new SeedPoint[lineSize / sizeof(int)];
		int[] square = new int[4];
		bool printout = false;
		int printouts = 0;
		Color[] printcolors = new Color[]
		{
			Color.red,
			Color.green,
			Color.blue,
			Color.yellow,
			Color.magenta,
			Color.white,
			Color.black,
			Color.cyan
		};
		for (int i = 0; i < bytes.Length; i += lineSize)
		{
			for (int j = 0; j < lineSize; j += sizeof(int))
			{
				int ID = BitConverter.ToInt32(bytes, i + j);
				/*SeedPoint v = points[ID];
				if (!points.TryGetValue(ID, out v))
				{
					Debug.LogWarning("Unable to create square!");
					break;
				}
				if (v.Pos.X < -2.2 && v.Pos.X > -2.4)
				{
					printout = true;
				}
				square[j / sizeof(int)] = v;*/
				/*if ((points[ID].Pos.X < 3.69 && points[ID].Pos.X > 3.68) &&
					(points[ID].Pos.Y < -0.92 && points[ID].Pos.Y > -0.94) &&
					(points[ID].Pos.Z < -7.26 && points[ID].Pos.Z > -7.28))
				{
					//printout = true;
				}*/
				square[j / sizeof(int)] = ID;
			}
			/*if (printout)
			{
				printout = false;
				Debug.Log(points[square[0]].Pos + " " + points[square[1]].Pos + " " + points[square[2]].Pos + " " + points[square[3]].Pos);
				Color c = printcolors[printouts];
				foreach (int k in square)
				{
					GameObject g = Instantiate(point);
					g.transform.position = vertices[k];
					g.name = "" + i;
					g.GetComponent<Renderer>().material.color = c;
				}
				printouts = (printouts + 1) % 8;
			}*/
			//squares.Add(new Square<SeedPoint>(square[0], square[1], square[2], square[3]));
			squares.Add(new SquareSimplified(square[0], square[1], square[2], square[3]));
		}
		status = DrawStatus.waiting;
		Debug.Log("Started! Amount of points " + points.Count + "; Amount of squares " + squares.Count);

		/*for(int i = 0; i < vertices.Length; i++)
		{
			GameObject g = Instantiate(point);
			g.transform.position = vertices[i];
			g.name = "" + i;
		}*/
	}

	private Tuple<Thread, object> task_param;

	void Update()
	{
		if (status == DrawStatus.waiting)
		{
			// Do nothing
			if (newtask)
			{
				task_param.Item1.Start(task_param.Item2);
				status = DrawStatus.processing;
				newtask = false;
			}
		}
		else if (status == DrawStatus.processing)
		{
			// Do nothing
		}
		else if (status == DrawStatus.ready)
		{
			// Draw surface
			surface.UpdateTriangles(meshtriangles);
			status = DrawStatus.waiting;
		}
	}

	private float layerFraction = 0.5f;
	private float widthFraction = 1;
	private bool auto = false;

	public void LayerUpdate(float layer)
	{
		layerFraction = layer;
		if (auto)
		{
			ReDraw();
		}
	}

	public void WidthUpdate(float width)
	{
		Debug.Log("Width " + width);
		widthFraction = width;
		if (auto)
		{
			ReDraw();
		}
	}

	public void AutoUpdate(bool auto)
	{
		this.auto = auto;
	}

	public void ReDraw()
	{
		Debug.Log("ReDraw");
		if (status == DrawStatus.notReady)
		{
			Debug.LogWarning("Not Ready!");
			// TODO: show warning
			return;
		}
		double median = layerFraction * levelRange + AbsLevelMin;
		Debug.Log("Median " + median);
		double width = widthFraction * levelRange;
		Debug.Log("Width " + width + " fraction " + widthFraction + " range " + levelRange);
		double levelMin = median - width;
		double levelMax = median + width;
		task_param = new Tuple<Thread, object>(new Thread(ReEvaluateSurface), new Tuple<double, double>(levelMin, levelMax));
		newtask = true;
	}

	private void ReEvaluateSurface(object tuple_min_max)
	{
		Debug.Log("Start ReEvaluating");
		Tuple<double, double> min_max = (Tuple<double, double>)tuple_min_max;
		Debug.Log("Min: " + min_max.Item1 + " Max: " + min_max.Item2);
		double Min = min_max.Item1;
		double Max = min_max.Item2;
		/*Dictionary<SeedPoint, int> validPoints = new Dictionary<SeedPoint, int>();
		foreach (KeyValuePair<int, SeedPoint> kvp in points)
		{
			if (kvp.Value.FTLE > min_max.Item1 && kvp.Value.FTLE < min_max.Item2)
			{
				validPoints.Add(kvp.Value, kvp.Key);
			}
		}
		Debug.Log("Got valid points. Count = " + validPoints.Count);
		List<Square<SeedPoint>> validSquares = new List<Square<SeedPoint>>();
		foreach (Square<SeedPoint> s in squares)
		{
			if (validPoints.Keys.Contains(s.A) && validPoints.Keys.Contains(s.B)
			&& validPoints.Keys.Contains(s.C) && validPoints.Keys.Contains(s.D))
			{
				validSquares.Add(s);
			}
		}
		Debug.Log("Got valid squares. Count = " + validSquares.Count);*/

		List<SquareSimplified> validSquares = new List<SquareSimplified>();
		foreach(SquareSimplified s in squares)
		{
			if(PointValid(s.A, Min, Max) && PointValid(s.B, Min, Max) && PointValid(s.C, Min, Max) && PointValid(s.D, Min, Max))
			{
				validSquares.Add(s);
			}
		}

		meshtriangles = new int[(validSquares.Count * 2) * (3 * 2)]; // 2 per square, 3 per triangle, 2-sided
		int index = 0;
		//foreach (Square<SeedPoint> s in validSquares)
		foreach(SquareSimplified s in validSquares)
		{
			/*meshtriangles[index] = validPoints[s.A]; index++;
			meshtriangles[index] = validPoints[s.B]; index++;
			meshtriangles[index] = validPoints[s.C]; index++;

			meshtriangles[index] = validPoints[s.C]; index++;
			meshtriangles[index] = validPoints[s.B]; index++;
			meshtriangles[index] = validPoints[s.A]; index++;


			meshtriangles[index] = validPoints[s.A]; index++;
			meshtriangles[index] = validPoints[s.D]; index++;
			meshtriangles[index] = validPoints[s.C]; index++;

			meshtriangles[index] = validPoints[s.C]; index++;
			meshtriangles[index] = validPoints[s.D]; index++;
			meshtriangles[index] = validPoints[s.A]; index++;*/

			meshtriangles[index] = s.A; index++;
			meshtriangles[index] = s.B; index++;
			meshtriangles[index] = s.C; index++;

			meshtriangles[index] = s.C; index++;
			meshtriangles[index] = s.B; index++;
			meshtriangles[index] = s.A; index++;


			meshtriangles[index] = s.A; index++;
			meshtriangles[index] = s.D; index++;
			meshtriangles[index] = s.C; index++;

			meshtriangles[index] = s.C; index++;
			meshtriangles[index] = s.D; index++;
			meshtriangles[index] = s.A; index++;
		}
		Debug.Log("Done! Triangle count = " + meshtriangles.Length/6);
		status = DrawStatus.ready;
	}

	private bool PointValid(int ID, double Min, double Max)
	{
		return (points[ID].FTLE > Min) && (points[ID].FTLE < Max);
	}
}
