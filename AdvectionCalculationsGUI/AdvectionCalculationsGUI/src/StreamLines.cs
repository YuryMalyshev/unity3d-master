using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvectionCalculationsGUI.src
{
	public class StreamLines
	{
		private Dictionary<int, StreamLine> streamLines;

		public StreamLines()
		{
			streamLines = new Dictionary<int, StreamLine>();
		}

		public void AddLine(StreamLine sl)
		{
			streamLines.Add(sl.ID, sl);
		}

		public List<StreamLine> GetStreamLines()
		{
			return streamLines.Values.ToList();
		}
	}
}
