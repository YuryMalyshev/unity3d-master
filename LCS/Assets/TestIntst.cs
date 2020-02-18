using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TestIntst : MonoBehaviour
{
	public GameObject PointPrefab;
	// Start is called before the first frame update
	void Start()
	{
		for(int i = 0; i < 4; i++)
		{
			Thread t = new Thread(objectCreator);
			t.Start(i);
		}
		
	}

	public Dictionary<string, GameObject> gameobjects = new Dictionary<string, GameObject>();
	//public ConcurrentQueue<object> createqueue = new ConcurrentQueue<object>();
	public ConcurrentQueue<object> updatequeue = new ConcurrentQueue<object>();

	// Update is called once per frame
	void Update()
	{
		if (!updatequeue.IsEmpty)
		{
			object result;
			List<object> resultList;
			while (updatequeue.TryDequeue(out result))
			{
				resultList = (List<object>)result;
				GameObject go;
				if(gameobjects.TryGetValue((string)resultList[3], out go))
				{
					go.transform.position = new Vector3((float)resultList[0], (float)resultList[1], (float)resultList[2]);
				}
				else
				{
					go = Instantiate(PointPrefab);
					go.name = (string)resultList[3];
					go.transform.position = new Vector3((float)resultList[0], (float)resultList[1], (float)resultList[2]);
					gameobjects.Add((string)resultList[3], go);
				}
			}
		}
	}

	public void objectCreator(object offset)
	{
		int steps = 4;
		int off = (int)offset;
		System.Random random = new System.Random();
		//Debug.Log(Thread.CurrentThread.ManagedThreadId);
		for (int i = 0; i < steps; i++)
		{
			updatequeue.Enqueue(new List<object> { (float)i, (float)0, (float)off, i + "_" +Thread.CurrentThread.ManagedThreadId });
			Thread.Sleep(100);
		}
		while(true)
		{
			Thread.Sleep((int)(1000+1000 * off));
			updatequeue.Enqueue(new List<object> { (float)random.Next(0, steps), (float)random.Next(0, 10), (float)off,
			random.Next(0, steps) + "_" +Thread.CurrentThread.ManagedThreadId });
		}
	}
}
