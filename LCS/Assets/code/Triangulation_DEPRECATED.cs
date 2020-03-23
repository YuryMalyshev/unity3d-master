using Assets.code;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class Triangulation_DEPRECATED : MonoBehaviour
{
	private Surface_DEPRECATED surface;
	private List<Triangle> triangles;
	public List<List<Seed_DEPRECATED>> seeds;
	public Shader shader;
	public static readonly int[,] order = new int[,] {
	{0 , 1 },
	{-1 , 1 },
	{-1 , 0 },
	{-1 , -1 },
	{0 , -1 },
	{1 , -1 },
	{1 , 0 },
	{1 , 1 },
	{0 , 1 }};
	// Start is called before the first frame update
	private double AbsLevelMin = double.PositiveInfinity;
	private double AbsLevelMax = double.NegativeInfinity;
	private double levelRange;
	public bool newtask = false;
	private enum DrawStatus
	{
		notReady, waiting, processing, ready
	}
	private DrawStatus status = DrawStatus.notReady;
	FTLEField_DEPRECATED f;
	void Start()
	{
		surface = new Surface_DEPRECATED();
		Surface_DEPRECATED.shader = shader;
		triangles = new List<Triangle>();
		f = new FTLEField_DEPRECATED("D:/Users/yrmal/Desktop/output/FTLEField.dat");
		status = DrawStatus.waiting;
		seeds = new List<List<Seed_DEPRECATED>>();
		Debug.Log(f.field.GetLength(0));
		Debug.Log(f.field.GetLength(1));
		for (int i = 0; i < f.field.GetLength(0); i++)
		{
			seeds.Add(new List<Seed_DEPRECATED>());
			for (int j = 0; j < f.field.GetLength(1); j++)
			{
				Seed_DEPRECATED s = new Seed_DEPRECATED(new double[] { i*0.01, j*0.01, 0 });
				s.FTLE = f.field[i, j];
				seeds[i].Add(s);
				if(s.FTLE < AbsLevelMin)
				{
					AbsLevelMin = s.FTLE;
				}
				if(s.FTLE > AbsLevelMax)
				{
					AbsLevelMax = s.FTLE;
				}
			}
		}
		levelRange = AbsLevelMax - AbsLevelMin;
		Debug.Log(seeds.Count);
		Debug.Log(seeds[0].Count);
	}

	private Tuple<Thread, object> task_param;
	// Update is called once per frame
	void Update()
	{
		if(status == DrawStatus.waiting)
		{
			// Do nothing
			if(newtask)
			{
				task_param.Item1.Start(task_param.Item2);
				status = DrawStatus.processing;
				newtask = false;
			}
		}
		else if(status == DrawStatus.processing)
		{
			// Do nothing
		}
		else if(status == DrawStatus.ready)
		{
			surface.ApplyChange();
			status = DrawStatus.waiting;
		}
	}	

	private void ReavalutateSurface(object tuple_min_max)
	{
		Tuple<double, double> min_max = (Tuple<double, double>)tuple_min_max;
		double levelMin = min_max.Item1;
		double levelMax = min_max.Item2;
		Debug.Log("Start preparation");
		foreach(Triangle t in triangles.ToArray())
		{
			if(!(t.A.FTLE >= levelMin && t.A.FTLE <= levelMax) || 
			!(t.B.FTLE >= levelMin && t.B.FTLE <= levelMax) || 
			!(t.C.FTLE >= levelMin && t.C.FTLE <= levelMax))
			{
				triangles.Remove(t);
			}
		}
		//triangles.Clear();

		for (int i = 0; i < seeds.Count; i++)
		{
			for (int j = 0; j < seeds[0].Count; j++)
			{
				Seed_DEPRECATED A = seeds[i][j];
				if (A.FTLE >= levelMin && A.FTLE <= levelMax)
				{
					//Debug.Log("=============" + A.pos + "=============");
					Seed_DEPRECATED B = null;
					Seed_DEPRECATED C = null;

					for (int k = 0; k < 9; k++)
					{
						int nx = i + order[k, 0];
						int ny = j + order[k, 1];
						//Debug.Log("Test [" + nx + "][" + ny + "]");
						if (nx > -1 && ny > -1 && nx < seeds.Count && ny < seeds[0].Count && (nx != i || ny != j))
						{
							//Debug.Log("Test point " + seeds[nx][ny].pos.ToString() + " value " + seeds[nx][ny].FTLE);
							if (seeds[nx][ny].FTLE >= levelMin && seeds[nx][ny].FTLE <= levelMax)
							{
								B = seeds[nx][ny];
							}
						}
						//Debug.Log("[" + (B != null ? B.pos.ToString() : "null") + "]" + 
						//			 "[" + (C != null ? C.pos.ToString() : "null") + "]");
						if (B != null && C != null)
						{

							if (A != B && B != C && A != C)
							{
								//Debug.Log("[" + B.pos.ToString() + "]" +
								//						"[" + C.pos.ToString() + "]");
								//Debug.Log(A.IsInUse() + " " + B.IsInUse() + " " + C.IsInUse());
								Triangle t = new Triangle(A, B, C);
								if (!triangles.Contains(t))
								{
									//created.Add(t);
									t.Accept();
									triangles.Add(t);
								}
							}
							else
							{
								Debug.Log("Points are equal");
							}
						}
						C = B;
						B = null;
					}
				}
			}
			if(i % 2 == 0)
			{
				Debug.Log(i / 2 + "%");
			}
		}
		Debug.Log("Triangles are prepared");
		surface.UpdateTriangles(triangles);
		surface.Make();
		status = DrawStatus.ready;
	}

	private float layerFraction = 0.5f;
	private float widthFraction = 1;
	private bool auto = false;

	public void LayerUpdate(float layer)
	{
		layerFraction = layer;
		if(auto)
		{
			ReDraw();
		}
	}

	public void WidthUpdate(float width)
	{
		widthFraction = width;
		if(auto)
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
		if(status == DrawStatus.notReady)
		{
			// TODO: show warning
			return;
		}
		double median = layerFraction * levelRange + AbsLevelMin;
		double width = widthFraction * levelRange;
		double levelMin = median - width;
		double levelMax = median + width;
		task_param = new Tuple<Thread, object>(new Thread(ReavalutateSurface), new Tuple<double, double>(levelMin, levelMax));
		newtask = true;
	}

}
