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

		public void CalculateStreamLine(object Steps_dt_Direction_Semaphore_Notifier)
		{
			List<object> param = (List<object>)Steps_dt_Direction_Semaphore_Notifier;
			int steps = (int)param[0];
			double dt = (double)param[1];
			int direction = (int)param[2];
			Semaphore semaphore = (Semaphore)param[3];
			ManualResetEvent notifier = (ManualResetEvent)param[4];

			int startstep = 0;
			//byte[] output = new byte[0];

			// TODO: create file with header
			if(direction != 0)
			{
				Calculate(startstep, steps, (float)(dt*direction), semaphore);
			}
			else
			{
				Seed originalHead = Head.Clone();
				Calculate(startstep, steps/2, (float)(dt * -1), semaphore);
				Head = originalHead;
				Calculate(startstep, steps/2, (float)(dt), semaphore);
			}
			notifier.Set();
		}

		private void Calculate(int startstep, int steps, float dt, Semaphore semaphore)
		{
			for (int i = startstep; i < steps; i++)
			{
				semaphore.WaitOne();
				Head.Calculate(dt);
				SeedPoint sp = Head.Simplify();
				if (sp == null)
				{
					semaphore.Release();
					return;
				}
				Points.Add(sp);
				semaphore.Release();
				// TODO: append into file
			}
		}
		
		public static void ResetCount()
		{
			IDCount = 0;
		}
	}
}
