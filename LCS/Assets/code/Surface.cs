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
		//readonly private List<GameObject> surfaces;
		readonly private GameObject surface;
		readonly private Mesh mesh;
		Shader shader;
		
		public Surface(Vector3[] vertices, Color[] colors, Shader shader)
		{
			mesh = new Mesh
			{
				vertices = vertices,
				colors = colors
			};
			//surfaces = new List<GameObject>();
			this.shader = shader;
			surface = new GameObject("0", typeof(MeshFilter), typeof(MeshRenderer));
			surface.GetComponent<MeshRenderer>().material = new Material(shader);
			surface.GetComponent<MeshFilter>().mesh = mesh;
		}

		public void UpdateTriangles(int[] triangles)
		{
			/*surfaces.Clear();
			for(int i = 0; i < triangles.Length; i += 6)
			{
				GameObject g = new GameObject(""+i, typeof(MeshFilter), typeof(MeshRenderer));
				g.GetComponent<MeshRenderer>().material = new Material(shader);
				int[] subt = new int[6];
				Array.Copy(triangles, i, subt, 0, 6);
				Mesh newmesh = new Mesh
				{
					vertices = mesh.vertices,
					colors = mesh.colors,
					triangles = subt
				};
				g.GetComponent<MeshFilter>().mesh = newmesh;
			}*/
			surface.name = "TCount [" + (triangles.Length / 6) + "]";
			surface.GetComponent<MeshFilter>().mesh.triangles = triangles;
		}
	}
}
