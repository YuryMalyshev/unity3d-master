using System;
using System.Collections.Generic;
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
		private ManualResetEvent notifier;

		public List<Thread> Start(float radius, int steps, double dt, List<Point> entryPoints, int maxThreads, ManualResetEvent notifier)
		{
			this.notifier = notifier;
			List<Thread> threads = new List<Thread>(maxThreads);
			semaphore = new Semaphore(maxThreads, maxThreads);

			StreamLine.ResetCount();
			foreach (Point p in entryPoints)
			{
				StreamLine sl = new StreamLine(new Seed(p, radius, IDS));
				SLS.AddLine(sl);
				Thread t = new Thread(sl.CalculateStreamLine);
				t.Start(new List<object> { steps, dt, semaphore, notifier });
				threads.Add(t);
			}
			return threads;
		}
	}
}
