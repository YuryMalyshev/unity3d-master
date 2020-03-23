using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataSet_DEPRECATED
{
	private Dictionary<int, StreamLine_DEPRECATED> streamLines;
	public DataSet_DEPRECATED(string filedir)
	{
		Debug.Log(filedir);
		string[] filenames = Directory.GetFiles(filedir, "*.dat");
		streamLines = new Dictionary<int, StreamLine_DEPRECATED>(filenames.Length);
		foreach (string filename in filenames)
		{
			try
			{
				int start = filename.IndexOf("_") + 1;
				int end = filename.IndexOf(".dat");
				
				int ID = int.Parse(filename.Substring(start, end - start));
				streamLines.Add(ID, new StreamLine_DEPRECATED("", filename));
			}
			catch
			{
				Debug.Log(filename + " is invalid!");
			}
		}
	}

	public List<Seed_DEPRECATED> GetPoints()
	{
		List<Seed_DEPRECATED> points = new List<Seed_DEPRECATED>();
		foreach(StreamLine_DEPRECATED sl in streamLines.Values)
		{
			points.AddRange(sl.GetPoints());
		}
		return points;
	}

	public List<Seed_DEPRECATED> GetLine(int ID)
	{
		try
		{
			return streamLines[ID].GetPoints();
		}
		catch
		{
			return new List<Seed_DEPRECATED>();
		}
	}
}
