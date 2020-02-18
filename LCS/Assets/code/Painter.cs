using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Painter class is used to 
/// </summary>
public class Painter : MonoBehaviour
{
	public GameObject PointPrefab;
	private ConcurrentQueue<List<object>> toBeUpdated;
	private Dictionary<string, GameObject> gameobjects;
	// Start is called before the first frame update
	void Start()
	{
		Debug.Log("Painter.Start()");
		toBeUpdated = new ConcurrentQueue<List<object>>();
		gameobjects = new Dictionary<string, GameObject>();
		Debug.Log("Is toBeUpdated NULL? " + (toBeUpdated == null));
	}

	// Update is called once per frame
	void Update()
	{
		if (toBeUpdated != null)
		{
			if (!toBeUpdated.IsEmpty)
			{
				List<object> resultList;
				while (toBeUpdated.TryDequeue(out resultList))
				{
					double[] pos = (double[])resultList[0];
					string id = (string)resultList[1];
					Color color = (Color)resultList[2];
					GameObject go;
					if (gameobjects.TryGetValue(id, out go))
					{
						go.transform.position = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
					}
					else
					{
						go = Instantiate(PointPrefab);
						go.name = id;
						go.transform.position = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
						go.GetComponent<Renderer>().material.color = color;
						gameobjects.Add(id, go);
					}
				}
			}
		}
		else
		{
			//Debug.Log("WTF? toBeUpdated is null");
		}
	}

	public void UpdateObject(List<object> PosNameAndColor)
	{
		toBeUpdated.Enqueue(PosNameAndColor);
	}

	public void test()
	{
		Debug.Log("TEST");
		Debug.Log("Is toBeUpdated NULL? " + (toBeUpdated == null));
	}
}
