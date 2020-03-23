using Assets.code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Surface
{
	List<Triangle> triangles;
	public GameObject surface = null;
	public static Shader shader;
	private Vector3[] vertices;
	private Color[] colors;
	private int[] meshtriangles;

	public Surface()
	{
		triangles = new List<Triangle>();
		surface = new GameObject("" + triangles.Count, typeof(MeshFilter), typeof(MeshRenderer));
	}

	public Surface(Triangle t)
	{
		triangles = new List<Triangle>();
		triangles.Add(t);
	}

	public void UpdateTriangles(List<Triangle> triangles)
	{
		this.triangles = triangles.ToList();
		//System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		//sw.Start();
		//this.triangles = this.triangles.Union(triangles).ToList();
		//sw.Stop();
		//Debug.Log("Union performed " + sw.ElapsedMilliseconds);

		/*sw.Start();
		int count = 0;
		foreach(Triangle t in this.triangles.ToList())
		{
			if(!triangles.Contains(t))
			{
				this.triangles.Remove(t);
				count++;
			}
		}
		sw.Stop();
		Debug.Log("Removed " + count + " Took " + sw.ElapsedMilliseconds);
		sw.Restart();
		count = 0;
		foreach (Triangle t in triangles)
		{
			if (!this.triangles.Contains(t))
			{
				this.triangles.Add(t);
				count++;
			}
		}
		sw.Stop();

		Debug.Log("And Added " + count + " Took " + sw.ElapsedMilliseconds);*/
	}

	public void Make()
	{
		List<Seed_DEPRECATED> seedVertices = new List<Seed_DEPRECATED>();
		//update only needed vertices, colors and triangles

		

		meshtriangles = new int[triangles.Count * 6];
		int iter = 0;
		foreach(Triangle t in triangles)
		{
			if(!seedVertices.Contains(t.A))
			{
				seedVertices.Add(t.A);
			}
			if (!seedVertices.Contains(t.B))
			{
				seedVertices.Add(t.B);
			}
			if (!seedVertices.Contains(t.C))
			{
				seedVertices.Add(t.C);
			}
			meshtriangles[iter] = seedVertices.IndexOf(t.A);
			iter++;
			meshtriangles[iter] = seedVertices.IndexOf(t.B);
			iter++;
			meshtriangles[iter] = seedVertices.IndexOf(t.C);
			iter++;
			meshtriangles[iter] = seedVertices.IndexOf(t.C);
			iter++;
			meshtriangles[iter] = seedVertices.IndexOf(t.B);
			iter++;
			meshtriangles[iter] = seedVertices.IndexOf(t.A);
			iter++;
		}

		vertices = new Vector3[seedVertices.Count];
		colors = new Color[seedVertices.Count];
		double offset = 20.8368614175521;
		double range = 8.6188302426474;
		for (int i = 0; i < seedVertices.Count; i++)
		{
			vertices[i] = seedVertices[i].pos;
			float num = (float)((seedVertices[i].FTLE + offset) / range);
			if (num > 0.5)
			{
				colors[i] = Color.Lerp(Color.green, Color.red, (num-0.5f)*2);
			}
			else
			{
				colors[i] = Color.Lerp(Color.blue, Color.green, num * 2);
			}
		}
		Debug.Log("Mesh is calculated!");
	}

	public void ApplyChange()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = meshtriangles;
		mesh.colors = colors;
		surface.name = "" + triangles.Count;
		surface.GetComponent<MeshRenderer>().material = new Material(shader);
		surface.GetComponent<MeshFilter>().mesh = mesh;
		Debug.Log("Mesh updated!");
	}

	public GameObject GetSurface()
	{
		return surface;
	}
}
