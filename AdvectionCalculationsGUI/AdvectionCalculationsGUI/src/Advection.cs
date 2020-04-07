using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace AdvectionCalculationsGUI.src
{
	class Advection
	{
		public InputDataSet IDS { get; private set;}
		public StreamLines SLS { get; private set; }

		public Advection(InputDataSet ids, StreamLines sls)
		{
			IDS = ids;
			SLS = sls;
		}

		private Semaphore semaphore;

		public List<Thread> Start(float radius, int steps, double dt, int direction, List<Point> entryPoints, int maxThreads, 
		ManualResetEvent notifier, BackgroundWorker worker)
		{
			List<Thread> threads = new List<Thread>(maxThreads);
			semaphore = new Semaphore(maxThreads, maxThreads);

			StreamLine.ResetCount();
			worker.ReportProgress(0);
			int total = entryPoints.Count;
			int count = total;
			foreach (Point p in entryPoints)
			{
				if (p == null)
				{
					Debug.WriteLine("[SEVERE] Advection.Start: point p is null in entrypoints!");
					continue;
				}
				StreamLine sl = new StreamLine(new Seed(p, radius, IDS));
				SLS.AddLine(sl);
				Thread t = new Thread(sl.CalculateStreamLine);
				t.Start(new List<object> { steps, dt, direction, semaphore, notifier });
				threads.Add(t);
				count--;
				worker.ReportProgress((total - count) * 1000 / total);
			}
			return threads;
		}
	}
}
