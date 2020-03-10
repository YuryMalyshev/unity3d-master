using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvectionCalculationsGUI
{
	public partial class GUI : Form
	{
		private readonly Pen dotPen;
		private readonly Pen linePen;
		private readonly Pen pointerPen;
		private Bitmap completeBackground;
		private Graphics graphics;
		private bool dataLoaded = false;
		private List<Point> pointsToDraw;

		private List<Point> line = new List<Point>();

		private FTLEDataSet fds;
		private InputDataSet ids;


		double xMin = 0;
		double xMax = 0;
		double yMin = 0;
		double yMax = 0;

		double ratio = 0;
		double offX = 0;
		double offY = 0;

		private int snapDist;
		private bool snap = false;

		public GUI()
		{
			InitializeComponent();
			ResetFields();

			(this.canvas_data as Control).KeyDown += new KeyEventHandler(CanvasKeyDown);
			(this.canvas_data as Control).KeyUp += new KeyEventHandler(CanvasKeyUp);

			dotPen = new Pen(Color.Black, 2);
			linePen = new Pen(Color.Green, 2);
			pointerPen = new Pen(Color.Red, 5);
			snapDist = (int)(pointerPen.Width * 4);
			pointsToDraw = new List<Point>();

			ToolTip tooltip_avDistance = new ToolTip();
			tooltip_avDistance.ShowAlways = true;
			tooltip_avDistance.SetToolTip(label_avDistance, "Estimated average distance between points");

			backgroundWorker1.WorkerReportsProgress = true;
		}

		private void SelectInputFile_Click(object sender, EventArgs e)
		{
			if (selectDataDalog.ShowDialog() == DialogResult.OK)
			{
				Debug.WriteLine("Selected " + selectDataDalog.FileName);
				// Update/Reset fields
				this.label_filename.Text = selectDataDalog.SafeFileName;
				ResetFields();

				// Enable/Disable components
				this.label_avDistance.Enabled = true;
				this.label_voxelSize.Enabled = true;
				this.avDistance.Enabled = true;
				this.voxelSize.Enabled = true;
				this.loadDataBtn.Enabled = true;

				this.outputSettingsPanel.Enabled = false;
				this.startBtn.Enabled = false;
			}
		}

		private void GUI_Load(object sender, EventArgs e)
		{
			this.progressBar.Maximum = 1000;
		}

		private void LoadData_Click(object sender, EventArgs e)
		{
			this.outputSettingsPanel.Enabled = true; //TODO: reset values
			ids = new InputDataSet(selectDataDalog.FileName, double.Parse(voxelSize.Text));
			List<Point> points = ids.GetPoints();
			double maxDist = double.Parse(this.avDistance.Text) * (15000f / ids.GetPoints().Count * 10);
			pointsToDraw.Clear();
			foreach (Point p in ids.GetPoints())
			{
				//TODO: based on amount
				double localMin = double.PositiveInfinity;
				foreach(Point pd in pointsToDraw)
				{
					double dist = pd.DistanceTo(p);
					if(localMin > dist)
					{
						localMin = dist;
					}
					if(dist < maxDist)
					{
						break;
					}
				}
				if(localMin > maxDist)
				{
					pointsToDraw.Add(p);
				}
			}
			//canvas.Refresh(); //TODO: draw
			dataLoaded = true;
			DrawData();	
		}

		private void DrawData()
		{
			completeBackground = new Bitmap(canvas_data.Width, canvas_data.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics background = Graphics.FromImage(completeBackground);
			background.Clear(Color.White);
			if (dataLoaded)
			{
				//TODO: dynamic
				xMin = 0;
				xMax = 2;
				yMin = 0;
				yMax = 1;

				ratio = Math.Min((completeBackground.Width * 0.75) / (xMax - xMin), (completeBackground.Height * 0.75) / (yMax - yMin));
				offX = completeBackground.Width / 2 - (xMax - xMin) / 2 * ratio;
				offY = completeBackground.Height / 2 - (yMax - yMin) / 2 * ratio;
				foreach (Point p in pointsToDraw)
				{
					background.DrawEllipse(dotPen, (float)(p.Pos.X * ratio + offX - dotPen.Width),
												(float)(p.Pos.Y * ratio + offY - dotPen.Width),
												dotPen.Width, dotPen.Width);
				}
			}
			background.Dispose();
			canvas_data.BackgroundImage = completeBackground;
		}

		private void SelectFolder_Click(object sender, EventArgs e)
		{
			if (selectOutputFolderDialog.ShowDialog() == DialogResult.OK)
			{
				string path = selectOutputFolderDialog.SelectedPath;
				Debug.WriteLine("Selected " + path);
				int first = path.IndexOf('\\');
				int last = path.LastIndexOf('\\');
				if (first == last)
				{
					this.label_selectFolder.Text = path;
				}
				else
				{
					this.label_selectFolder.Text = path.Substring(0, first + 1) + "..." + path.Substring(last);
				}
				fds = new FTLEDataSet(path);
				this.startBtn.Enabled = true;
			}
		}

		private void Start_Click(object sender, EventArgs e)
		{
			backgroundWorker1.RunWorkerAsync();
			
			//Thread t = new Thread();
			//t.Start(progressBar);
		}



		private void Canvas_MouseMove(object sender, MouseEventArgs e)
		{
			canvas_data.Refresh(); //deletes everything

			Point last = null;
			foreach (Point p in line)
			{
				if (last != null)
				{
					int pX = (int)((float)(p.Pos.X * ratio + offX - dotPen.Width));
					int pY = (int)((float)(p.Pos.Y * ratio + offY - dotPen.Width));
					int lastX = (int)((float)(last.Pos.X * ratio + offX - dotPen.Width));
					int lastY = (int)((float)(last.Pos.Y * ratio + offY - dotPen.Width));
					graphics.DrawLine(linePen, lastX, lastY, pX, pY);
				}
				last = p;
			}

			int mX = e.X;
			int mY = e.Y;
			if (snap)
			{
				//Debug.WriteLine("Snapping!");
				foreach (Point p in pointsToDraw)
				{
					int pX = (int)((float)(p.Pos.X * ratio + offX - dotPen.Width));
					int pY = (int)((float)(p.Pos.Y * ratio + offY - dotPen.Width));
					if (Math.Sqrt(Math.Pow(pX - mX, 2) + Math.Pow(pY - mY, 2)) < snapDist)
					{
						mX = pX;
						mY = pY;
						break;
					}
				}
			}

			if (last != null)
			{
				int lastX = (int)((float)(last.Pos.X * ratio + offX - dotPen.Width));
				int lastY = (int)((float)(last.Pos.Y * ratio + offY - dotPen.Width));
				graphics.DrawLine(linePen, lastX, lastY, mX, mY);
			}

			graphics.DrawEllipse(new Pen(Color.Red, 10), mX - 5, mY - 5, 10, 10);

		}

		private void Canvas_Resize(object sender, EventArgs e)
		{
			DrawData();
			graphics = canvas_data.CreateGraphics();
			
		}

		private void CanvasKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Menu)
			{
				snap = true;
			}
			e.Handled = true;
		}

		private void CanvasKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Menu)
			{
				snap = false;
			}
			e.Handled = true;
		}

		private void ResetFields()
		{
			this.avDistance.Text = "0.01";
			this.voxelSize.Text = "0.5";
			this.resolution.Text = "666";
			this.dt.Text = "666";
			this.steps.Text = "100";
			this.seconds.Text = "unused";
			this.direction.SelectedIndex = 0;
		}

		private void Canvas_GetFocus(object sender, EventArgs e)
		{
			canvas_data.Focus();
		}

		private void Canvas_MouseClick(object sender, MouseEventArgs e)
		{
			//Debug.WriteLine("Click in mouseClick");
			
			if (e.Button == MouseButtons.Left)
			{
				Point p = SnapPoint(e);
				if(p != null)
				{
					line.Add(p);
				}
			}
			if (e.Button == MouseButtons.Right)
			{
				if(line.Count > 0)
					line.RemoveAt(line.Count-1);
			}
		}

		private Point SnapPoint(MouseEventArgs e)
		{
			
			int mX = e.X;
			int mY = e.Y;
			if (snap)
			{
				//Debug.WriteLine("Snapping!");
				foreach (Point p in pointsToDraw)
				{
					int pX = (int)((float)(p.Pos.X * ratio + offX - dotPen.Width));
					int pY = (int)((float)(p.Pos.Y * ratio + offY - dotPen.Width));
					if (Math.Sqrt(Math.Pow(pX - mX, 2) + Math.Pow(pY - mY, 2)) < snapDist)
					{
						return p;
					}
				}
			}
			try
			{
				Vector3 pos = new Vector3(
					(float)((double)(mX - offX) / ratio),
					(float)((double)(mY - offY) / ratio),
					0
				);
				return ids.GetPoint(pos);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.StackTrace);
				return null;
			}
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			Debug.WriteLine("Start doing work!");
			BackgroundWorker worker = sender as BackgroundWorker;
			FTLEField field = new FTLEField(0.1, 2, 1, fds, selectOutputFolderDialog.SelectedPath);
			List<Thread> threads = field.Start();
			Debug.WriteLine("Threads are done, waiting for their end");
			int max = threads.Count;
			ManualResetEvent notifier = field.notifier;
			while (threads.Count > 0)
			{
				notifier.WaitOne(2000);
				notifier.Reset();
				foreach (Thread t in threads.ToList())
				{
					if (!t.IsAlive)
					{
						threads.Remove(t);
					}
				}
				worker.ReportProgress((max-threads.Count)*1000/max);
				Debug.WriteLine("threads.Count " + threads.Count + " Max " + max + " Fraction " + (threads.Count * 1000 / max) + "/" + 1000);
			}
			field.Serialize();
			Debug.WriteLine("All done!");
		}

		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.progressBar.Value = e.ProgressPercentage;
		}
	}
}
