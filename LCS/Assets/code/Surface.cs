using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.code
{
	public class Surface
	{
		public GameObject surface { get; private set; }
		readonly private Mesh mesh;
		
		public Surface(Vector3[] vertices, Color[] colors, Shader shader)
		{
			mesh = new Mesh
			{
				vertices = vertices,
				colors = colors
			};
			surface = new GameObject("0", typeof(MeshFilter), typeof(MeshRenderer));
			surface.GetComponent<MeshRenderer>().material = new Material(shader);
			surface.GetComponent<MeshFilter>().mesh = mesh;
		}

		public void UpdateTriangles(int[] triangles)
		{
			surface.name = "TCount [" + (triangles.Length / 6) + "]";
			surface.GetComponent<MeshFilter>().mesh.triangles = triangles;
		}

		public void Dispose()
		{
			UnityEngine.Object.Destroy(mesh);
			UnityEngine.Object.Destroy(surface);
		}
	}
}
