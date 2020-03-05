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
	public Surface()
	{
		triangles = new List<Triangle>();
	}

	public Surface(Triangle t)
	{
		triangles = new List<Triangle>();
		triangles.Add(t);
	}

	public void UpdateTriangles(List<Triangle> triangles)
	{
		//this.triangles = this.triangles.Union(triangles).ToList();
		foreach(Triangle t in this.triangles.ToList())
		{
			if(!this.triangles.Contains(t))
			{
				this.triangles.Remove(t);
			}
		}

		foreach (Triangle t in triangles)
		{
			if (!this.triangles.Contains(t))
			{
				this.triangles.Add(t);
			}
		}
	}

	public void Make()
	{
		if (surface == null)
		{
			surface = new GameObject("" + triangles.Count, typeof(MeshFilter), typeof(MeshRenderer));
		}
		else
		{
			surface.name = "" + triangles.Count;
		}

		//update only needed vertices, colors and triangles
		List<Seed> seedVertices = new List<Seed>();
		int[] meshtriangles = new int[triangles.Count * 6];
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

		Vector3[] vertices = new Vector3[seedVertices.Count];
		Color[] colors = new Color[seedVertices.Count];
		for (int i = 0; i < seedVertices.Count; i++)
		{
			vertices[i] = seedVertices[i].pos;
			colors[i] = Color.Lerp(Color.red, Color.blue, (float)((seedVertices[i].FTLE + 20.83) / 9.62));
		}


		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = meshtriangles;
		mesh.SetColors(colors.ToList());

		



		surface.GetComponent<MeshRenderer>().material = new Material(shader);
		surface.GetComponent<MeshFilter>().mesh = mesh;
	}

	public GameObject GetSurface()
	{
		return surface;
	}
}
