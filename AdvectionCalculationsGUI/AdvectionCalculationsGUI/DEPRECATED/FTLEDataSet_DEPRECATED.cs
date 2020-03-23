using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FTLEDataSet_DEPRECATED
{
	private Dictionary<int, StreamLine_DEPRECATED> streamLines;
	public FTLEDataSet_DEPRECATED(string filedir)
	{
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
				Debug.WriteLine("Wrong Format!");
			}
		}
		Debug.WriteLine("Created " + streamLines.Count + " streamlines");
	}

	public FTLEDataSet_DEPRECATED()
	{
		streamLines = new Dictionary<int, StreamLine_DEPRECATED>();
	}

	public void AddPoint(Seed_DEPRECATED s, int StreamID)
	{
		if (streamLines.TryGetValue(StreamID, out StreamLine_DEPRECATED line))
		{
			line.AddPoint(s);
		}
		else
		{
			StreamLine_DEPRECATED _line = new StreamLine_DEPRECATED();
			_line.AddPoint(s);
			streamLines.Add(StreamID, _line);
		}
	}

	public List<Seed_DEPRECATED> GetPoints()
	{
		List<Seed_DEPRECATED> points = new List<Seed_DEPRECATED>();
		foreach (StreamLine_DEPRECATED sl in streamLines.Values)
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
