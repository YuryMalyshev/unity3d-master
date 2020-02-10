using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Test : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		//grid resoluton 0.015
		double res = 0.1;
		DataSet ds = new DataSet("Assets/data/", "doublegyro", res);
		//1.545455 0.369231 0.000000 -0.006208 -0.019098 0.000000
		//0.378788 0.630769 0.000000 - 0.016213 - 0.017913 0.000000
		//0.984848 0.476923 0.000000 0.047403 0.000167 0.000000
		double[] pos = { 0.984848, 0.476923, 0.000000 };
		Stopwatch sw;

		long inter = 0;
		long simple = 0;
		long complex = 0;

		for(int i = 0; i < 5; i++)
		{
			sw = new Stopwatch();
			sw.Start();
			Interpolator.Interpolate(pos, ds.GetPoints(), res);
			sw.Stop();
			inter += sw.ElapsedTicks;
			//UnityEngine.Debug.Log("Elapsed Time is {" + sw.ElapsedTicks + "} ms");

			sw = new Stopwatch();
			sw.Start();
			ds.GetPoint(pos, true);
			sw.Stop();
			simple += sw.ElapsedTicks;
			//UnityEngine.Debug.Log("Elapsed Time is {" + sw.ElapsedTicks + "} ms");

			sw = new Stopwatch();
			sw.Start();
			ds.GetPoint(pos, false);
			sw.Stop();
			complex += sw.ElapsedTicks;
			//UnityEngine.Debug.Log("Elapsed Time is {" + sw.ElapsedTicks + "} ms");

			UnityEngine.Debug.Log("=================================================");
		}
		UnityEngine.Debug.Log("Inter av: " + inter / 5);
		UnityEngine.Debug.Log("Simple av: " + simple / 5);
		UnityEngine.Debug.Log("Complex av: " + complex / 5);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
