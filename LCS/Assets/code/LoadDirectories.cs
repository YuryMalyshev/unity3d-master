using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadDirectories : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject entryPrefab;
	public GameObject Canvas;
	public GameObject StartUp;
	private List<GameObject> entries;
	private bool reload = true;
	private const string FTLEname = "FTLEField.dat";
	private const string ConnectionsName = "Squares.dat";
	private const string datapath = "/StreamingAssets/data";
	void Start()
	{
		entries = new List<GameObject>();
	}

	// Update is called once per frame
	void Update()
	{
		if (reload)
		{
			foreach(GameObject g in entries)
			{
				Destroy(g);
			}
			entries.Clear();
			GameObject entry;
			string[] dirs = Directory.GetDirectories(Application.dataPath + datapath);
			Debug.Log("Trying to open " + Application.dataPath + datapath);
			Debug.Log("Number of dirs " + dirs.Length);
			foreach (string dir in dirs)
			{
				Debug.Log(dir);
				string[] files = Directory.GetFiles(dir);
				//Debug.Log("Number of file: " + files.Length);
				//if (files.Length == 2)
				//{
					//foreach (string file in files)
					//	Debug.Log("File: " + file);
					entry = Instantiate(entryPrefab, transform);
				//entry.transform.localScale = transform.localScale;
				string text = dir.Substring(Math.Max(dir.LastIndexOf("\\")+1, dir.LastIndexOf("/")+1));
				entry.GetComponentInChildren<Text>().text = text;
				Button button = entry.GetComponent<UnityEngine.UI.Button>();
				button.onClick.AddListener(() => OnClick(Application.dataPath + datapath + "/" + text));
				entries.Add(entry);
				//}
				
			}
			reload = false;
		}
	}

	private void OnClick(string path)
	{
		Canvas.GetComponent<UIControl>().ShowSelectorPanel(false);
		Canvas.GetComponent<UIControl>().EnableControls(true);
		Canvas.GetComponent<UIControl>().EnableLoadBtn(true);
		StartUp.GetComponent<Visualization>().LoadData(path);
	}

	public void Reload()
	{
		reload = true;
	}
}
