using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      Vector3[] vertices = new Vector3[10];
      vertices[0] = new Vector3(0, 0, 0);
      vertices[1] = new Vector3(0, 0, 1);
      vertices[2] = new Vector3(0, 1, 1);
      vertices[3] = new Vector3(1, 1, 1);
      vertices[4] = new Vector3(1, 1, 2);
      vertices[5] = new Vector3(1, 2, 2);
      vertices[6] = new Vector3(3, 0, 1);
      vertices[7] = new Vector3(4, 0, 1);
      vertices[8] = new Vector3(5, 0, 1);
      vertices[9] = new Vector3(6, 0, 1);

      int[] meshtriangles = new int[12];
      meshtriangles[0] = 0;
      meshtriangles[1] = 1;
      meshtriangles[2] = 2;
      meshtriangles[3] = 2;
      meshtriangles[4] = 1;
      meshtriangles[5] = 0;

      meshtriangles[6] = 3;
      meshtriangles[7] = 4;
      meshtriangles[8] = 5;
      meshtriangles[9] = 5;
      meshtriangles[10] = 4;
      meshtriangles[11] = 3;

      GameObject surface = new GameObject("test", typeof(MeshFilter), typeof(MeshRenderer));
      Mesh mesh = new Mesh();
      mesh.vertices = vertices;
      mesh.triangles = meshtriangles;
      surface.GetComponent<MeshFilter>().mesh = mesh;
   }

    // Update is called once per frame
    void Update()
    {
        
    }
}
