using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataSet
{
	private Dictionary<int, StreamLine> streamLines;
	public DataSet(string filedir)
	{
		Debug.Log(filedir);
		string[] filenames = Directory.GetFiles(filedir, "*.dat");
		streamLines = new Dictionary<int, StreamLine>(filenames.Length);
		foreach (string filename in filenames)
		{
			int start = filename.IndexOf("_") + 1;
			int end = filename.IndexOf(".dat");
			int ID = int.Parse(filename.Substring(start, end - start));
			streamLines.Add(ID, new StreamLine("", filename));
		}
	}

	public List<Seed> GetPoints()
	{
		List<Seed> points = new List<Seed>();
		foreach(StreamLine sl in streamLines.Values)
		{
			points.AddRange(sl.GetPoints());
		}
		return points;
	}

	public List<Seed> GetLine(int ID)
	{
		try
		{
			return streamLines[ID].GetPoints();
		}
		catch
		{
			return new List<Seed>();
		}
	}
}
