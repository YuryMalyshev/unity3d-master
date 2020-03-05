using Assets.code;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Triangulation : MonoBehaviour
{
	private Surface surface;
	private List<Triangle> triangles;
	public List<List<Seed>> seeds;
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
	public double levelMin = 0;
	public double levelMax = 0;
	public bool redraw = false;
	private bool done = false;
	FTLEField f;
	void Start()
	{
		surface = new Surface();
		Surface.shader = shader;
		triangles = new List<Triangle>();
		f = new FTLEField("D:/UnityWS/AdvectionCalc/AdvectionCalc/bin/Debug/output/FTLEField.dat");
		seeds = new List<List<Seed>>();
		Debug.Log(f.field.GetLength(0));
		Debug.Log(f.field.GetLength(1));
		for (int i = 0; i < f.field.GetLength(0); i++)
		{
			seeds.Add(new List<Seed>());
			for (int j = 0; j < f.field.GetLength(1); j++)
			{
				Seed s = new Seed(new double[] { i*0.01, j*0.01, 0 });
				s.FTLE = f.field[i, j];
				seeds[i].Add(s);
			}
		}
		Debug.Log(seeds.Count);
		Debug.Log(seeds[0].Count);
		
	}

	// Update is called once per frame
	void Update()
	{
		if (redraw)
		{
			Thread thread = new Thread(ReavalutateSurface);
			thread.Start();
			redraw = false;
		}

		if (done)
		{
			surface.ApplyChange();
			done = false;
		}
	}

	private void ReavalutateSurface()
	{
		triangles.Clear();
		foreach (List<Seed> ls in seeds)
		{
			foreach (Seed s in ls)
			{
				s.ResetUse();
			}
		}

		for (int i = 0; i < seeds.Count; i++)
		{
			for (int j = 0; j < seeds[0].Count; j++)
			{
				List<Triangle> created = new List<Triangle>(8);
				Seed A = seeds[i][j];
				if (A.FTLE >= levelMin && A.FTLE <= levelMax)
				{
					//Debug.Log("=============" + A.pos + "=============");
					Seed B = null;
					Seed C = null;

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
								if (!A.IsInUse() || !B.IsInUse() || !C.IsInUse())
								{
									Triangle t = new Triangle(A, B, C);
									created.Add(t);
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
					foreach (Triangle t in created)
					{
						t.Accept();
						triangles.Add(t);
					}

				}
			}
		}
		surface.UpdateTriangles(triangles);
		
		Debug.Log("Done!");
		surface.Make();
		done = true;
	}

}
