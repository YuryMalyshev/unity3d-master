using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdvectionCalculationsGUI.src
{
	public class StreamLine
	{
		private static int IDCount = 0;
		public readonly int ID;
		public List<SeedPoint> Points { get; private set; }
		public Seed Head { get; private set; }
		public StreamLine(Seed Head)
		{
			this.Head = Head;
			Points = new List<SeedPoint>();
			ID = IDCount;
			IDCount++;
		}

		public void CalculateStreamLine(object Steps_dt_Semaphore_Notifier)
		{
			List<object> param = (List<object>)Steps_dt_Semaphore_Notifier;
			int steps = (int)param[0];
			double dt = (double)param[1];
			Semaphore semaphore = (Semaphore)param[2];
			ManualResetEvent notifier = (ManualResetEvent)param[3];

			int startstep = 0;
			byte[] output = new byte[0];

			// TODO: create file with header

			for (int i = startstep; i < steps; i++)
			{
				semaphore.WaitOne();
				Head.Calculate((float)dt);
				Points.Add(Head.Simplify());
				semaphore.Release();
				// TODO: append into file
				if ((100 * (float)(i - startstep) / (steps - startstep)) % 25 == 0)
					Console.WriteLine(ID + "\t:\t" + (100 * (i - startstep) / (steps - startstep)) + " % Done");
			}
			
		}
		
		public static void ResetCount()
		{
			IDCount = 0;
		}
	}
}
