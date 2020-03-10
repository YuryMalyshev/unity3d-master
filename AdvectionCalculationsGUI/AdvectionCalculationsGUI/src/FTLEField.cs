using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class FTLEField
{
	private double[,] field; //TODO: 3D; TODO: Custom shape
	private double resolution;
	List<Seed> seeds;
	private object fieldlock = new object();
	public ManualResetEvent notifier = new ManualResetEvent(false);
	private string directory;

	public FTLEField(double resolution, double dimX, double dimY, FTLEDataSet FDS, string directory)
	{
		this.resolution = resolution;
		field = new double[(int)(dimX / resolution), (int)(dimY / resolution)];
		seeds = FDS.GetPoints();
		this.directory = directory;
		Console.WriteLine("Seed count: " + seeds.Count);
	}

	public List<Thread> Start()
	{
		List<Thread> threads = new List<Thread>(field.Length);
		for (int i = 0; i < field.GetLength(0); i++)
		{
			for (int j = 0; j < field.GetLength(1); j++)
			{
				double[] pos = new double[3] { i * resolution, j * resolution, 0 };
				int[] cell = new int[2] { i, j };
				Thread t = new Thread(CalculatePoint);
				threads.Add(t);
				t.Start(new List<object> { cell, pos, seeds });
			}
		}
		return threads;




	}

	private void CalculatePoint(object cell_pos_seeds)
	{
		List<object> param = (List<object>)cell_pos_seeds;
		int[] cell = (int[])param[0];
		double[] pos = (double[])param[1];
		List<Seed> seeds = (List<Seed>)param[2];

		double FTLE = Interpolator.InterpolateFTLE(pos, seeds, 0.5);
		lock(fieldlock)
		{
			field[cell[0], cell[1]] = FTLE;
		}
		notifier.Set();
	}

	public void Serialize()
	{
		byte[] output = new byte[0];
		output = output.Concat(BitConverter.GetBytes(field.GetLength(0))).ToArray();
		output = output.Concat(BitConverter.GetBytes(field.GetLength(1))).ToArray();
		for (int i = 0; i < field.GetLength(0); i++)
		{
			for (int j = 0; j < field.GetLength(1); j++)
			{
				output = output.Concat(BitConverter.GetBytes(field[i,j])).ToArray();
			}
		}
		using (FileStream fileStream = new FileStream(directory + "\\FTLEField.dat", FileMode.Append))
		{
			fileStream.Write(output, 0, output.Length);
		}
	}
}
