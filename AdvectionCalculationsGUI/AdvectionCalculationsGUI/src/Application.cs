using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdvectionCalc.src
{
	class Application
	{
		static void Main_1(string[] args)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			FTLEDataSet fds = new FTLEDataSet("D:/UnityWS/AdvectionCalc/AdvectionCalc/bin/Debug/output/");
			//new FTLEField(0.01, 2, 1, fds);
			sw.Stop();
			Console.WriteLine("Took " + sw.ElapsedMilliseconds);
			Console.ReadLine();

			/*InputDataSet ds = new InputDataSet("", "doublegyro", 0.5);
			FTLEDataSet fds = new FTLEDataSet();
			Advection adv = new Advection(ds, fds);
			double radius = 0.005;
			int steps = 1200;
			double dt = 0.15;
			Thread t = new Thread(adv.Start);
			List<Point> points = new List<Point>();
			int seeds = 200;

			Stopwatch sw = new Stopwatch();
			sw.Start();

			double x = 0;
			double y = 0;
			for (int i = 0; i < seeds; i++)
			{
				if (y >= 1)
					y = 0;
				double[] pos = new double[] { x, y, 0 };
				points.Add(ds.GetPoint(pos));
				pos = new double[] { x, 1-y, 0 };
				points.Add(ds.GetPoint(pos));
				x += 2f / seeds;
				y += 2f / seeds;
			}
			t.Start(new List<object> { radius, steps, dt, points, 50 }); //TODO

			t.Join();
			sw.Stop();
			Console.WriteLine("Took " + sw.ElapsedMilliseconds);
			Console.ReadLine();
			return;*/
		}
	}
}
