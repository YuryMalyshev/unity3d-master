using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FTLEDataSet
{
	private Dictionary<int, StreamLine> streamLines;
	public FTLEDataSet(string filedir)
	{
		string[] filenames = Directory.GetFiles(filedir, "*.dat");
		streamLines = new Dictionary<int, StreamLine>(filenames.Length);
		foreach (string filename in filenames)
		{
			try
			{
				int start = filename.IndexOf("_") + 1;
				int end = filename.IndexOf(".dat");
				int ID = int.Parse(filename.Substring(start, end - start));
				streamLines.Add(ID, new StreamLine("", filename));
			}
			catch
			{
				Debug.WriteLine("Wrong Format!");
			}
		}
		Debug.WriteLine("Created " + streamLines.Count + " streamlines");
	}

	public FTLEDataSet()
	{
		streamLines = new Dictionary<int, StreamLine>();
	}

	public void AddPoint(Seed s, int StreamID)
	{
		if (streamLines.TryGetValue(StreamID, out StreamLine line))
		{
			line.AddPoint(s);
		}
		else
		{
			StreamLine _line = new StreamLine();
			_line.AddPoint(s);
			streamLines.Add(StreamID, _line);
		}
	}

	public List<Seed> GetPoints()
	{
		List<Seed> points = new List<Seed>();
		foreach (StreamLine sl in streamLines.Values)
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
