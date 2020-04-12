using AdvectionCalculationsGUI.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace AdvectionCalculationsGUI
{
	public partial class GUI : Form
	{
		// graphics
		Picture picture;
		// Data
		private InputDataSet ids;
		private StreamLines sls;
		//private FTLEField ffield;
		private int dir;

		// Timing check
		private Stopwatch sw = new Stopwatch();

		public GUI()
		{
			// Initialize
			InitializeComponent();
			this.xm90.Click += (sender, EventArgs) => { this.ChangeRotationDecrease(sender, EventArgs, 1); };
			this.ym90.Click += (sender, EventArgs) => { this.ChangeRotationDecrease(sender, EventArgs, 2); };
			this.zm90.Click += (sender, EventArgs) => { this.ChangeRotationDecrease(sender, EventArgs, 3); };
			this.xp90.Click += (sender, EventArgs) => { this.ChangeRotationIncrease(sender, EventArgs, 1); };
			this.yp90.Click += (sender, EventArgs) => { this.ChangeRotationIncrease(sender, EventArgs, 2); };
			this.zp90.Click += (sender, EventArgs) => { this.ChangeRotationIncrease(sender, EventArgs, 3); };
			ResetFields();
			fieldNormilizerWorker.WorkerReportsProgress = true;
			loadDataWorker.WorkerReportsProgress = true;
			this.progressBar.Maximum = 1000;

			// Create tooltips
			ToolTip tooltip_avDistance = new ToolTip();
			tooltip_avDistance.ShowAlways = true;
			tooltip_avDistance.SetToolTip(label_avDistance, "Estimated average distance between points");

			// Add keyboard handlers to the canvas
			(this.canvas as Control).KeyDown += new KeyEventHandler(CanvasKeyDown);
			(this.canvas as Control).KeyUp += new KeyEventHandler(CanvasKeyUp);

			picture = new Picture();
		}

		private void GUI_Load(object sender, EventArgs e)
		{
			
		}

		/// <summary>
		/// Action Perfromed on "Select Data" click. Opens a dialog for file selection; resets state of the GUI
		/// </summary>
		private void SelectInputFile_Click(object sender, EventArgs e)
		{
			if (selectDataDalog.ShowDialog() == DialogResult.OK)
			{
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
				this.GraphicalInputPanel.Enabled = false;
			}
		}

		/// <summary>
		/// Action Perfromed on "Load Data" click. Loads the data from text file and then draws it on canvas.
		/// </summary>
		private void LoadData_Click(object sender, EventArgs e)
		{
			this.outputSettingsPanel.Enabled = true;
			this.GraphicalInputPanel.Enabled = true;
			loadDataWorker.RunWorkerAsync();
		}

		/// <summary>
		/// Action Perfromed on "Select Folder" click. Opens folder selection dialog, unlocks "Start" button
		/// </summary>
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
				
				this.startBtn.Enabled = true;
			}
		}

		private void Start_Click(object sender, EventArgs e)
		{
			SetAllEnabled(false);
			fieldNormilizerWorker.RunWorkerAsync();
		}

		/// <summary>
		/// Called when mouse has moved over the canvas. Updates
		/// </summary>
		private void Canvas_MouseMove(object sender, MouseEventArgs e)
		{
			picture.UpdateMousePointer(canvas, e.X, e.Y);
		}

		private void Canvas_Resize(object sender, EventArgs e)
		{
			if(picture != null)
				picture.DrawScaledObjectOn(canvas);
		}

		private void CanvasKeyDown(object sender, KeyEventArgs e)
		{
			// Possible TODO: snapping
			if (e.KeyCode == Keys.Enter)
			{
				picture.FinishLine();
			}
		}

		private void CanvasKeyUp(object sender, KeyEventArgs e)
		{
			// Possible TODO: snapping
		}

		private void ResetFields()
		{
			this.avDistance.Text = "0.01";
			this.voxelSize.Text = "0.25";
			this.resolution.Text = "5";
			this.dt.Text = "0.1";
			this.steps.Text = "200";
			this.seconds.Text = "20";
			this.direction.SelectedIndex = 0;
		}

		private void Canvas_GetFocus(object sender, EventArgs e)
		{
			canvas.Focus();
			Cursor.Hide();
		}

		private void Canvas_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				picture.AddPoint(canvas, e.X, e.Y, ids);
			}
			else if (e.Button == MouseButtons.Right)
			{
				picture.RemovePoint(canvas, e.X, e.Y);
			}
			}

		private void MakeCalculations(object sender, DoWorkEventArgs e)
		{
			List<Thread> threads;
			ManualResetEvent notifier = new ManualResetEvent(false);
			BackgroundWorker worker = sender as BackgroundWorker;
			worker.ReportProgress(0);
			Debug.WriteLine("Start doing work!");
			sls = new StreamLines();
			Advection adv = new Advection(ids, sls);
			// start advection routine
			float radius = float.Parse(this.avDistance.Text) / 2;
			int steps = int.Parse(this.steps.Text);
			double dt = double.Parse(this.dt.Text);
			List<Point> entryPoints = new List<Point>();
			int segments = 20; //segments => points = segments - 1 + end_point //TODO
			foreach (List<Point> line in picture.Lines)
			{
				Point last = null;
				foreach (Point p in line)
				{
					if (last != null)
					{
						//interpolate a line
						for (int i = 1; i < segments; i++)
						{
							float ratio = (float)i / segments;
							Vector3 pos = new Vector3(
								((p.Pos.X - last.Pos.X) * ratio) + last.Pos.X,
								((p.Pos.Y - last.Pos.Y) * ratio) + last.Pos.Y,
								((p.Pos.Z - last.Pos.Z) * ratio) + last.Pos.Z
							);
							entryPoints.Add(ids.GetPoint(pos));
						}
					}
					entryPoints.Add(p);
					last = p;
				}
			}
			Debug.WriteLine("N-points: " + entryPoints.Count);
			threads = adv.Start(radius, steps, dt, dir, entryPoints, 40, notifier, worker); //TODO

			Debug.WriteLine("Threads are creared, waiting untilthey all die");
			int threadMax = threads.Count;
			worker.ReportProgress(0);
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
				worker.ReportProgress((threadMax - threads.Count) * 1000 / threadMax);
			}

			worker.ReportProgress(0);
			Debug.WriteLine("StreamLines were calculated");
			Debug.WriteLine("Start creating uniform FTLE field");
			int resolution = int.Parse(this.resolution.Text);
			FTLEField field = new FTLEField(ids, resolution);

			threads = field.PrepareField(notifier, sls, 40);
			threadMax = threads.Count;
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
				worker.ReportProgress((threadMax - threads.Count) * 1000 / threadMax);
				Debug.WriteLine("threads.Count " + threads.Count + " Max " + threadMax +
				" Fraction " + ((threadMax - threads.Count) * 1000 / threadMax) + "/" + 1000);
			}

			worker.ReportProgress(0);
			threads = field.Start(notifier, resolution, 40);
			Debug.WriteLine("Threads are done, waiting for their end");
			threadMax = threads.Count;
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
				worker.ReportProgress((threadMax - threads.Count) * 1000 / threadMax);
				Debug.WriteLine("threads.Count " + threads.Count + " Max " + threadMax +
				" Fraction " + ((threadMax - threads.Count) * 1000 / threadMax) + "/" + 1000);
			}

			worker.ReportProgress(0);
			field.CreateSquares(resolution, worker);
			field.Serialize(selectOutputFolderDialog.SelectedPath, worker);
			Debug.WriteLine("All done!");
		}

		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.progressBar.Value = e.ProgressPercentage;
		}

		private void GenericTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			if(!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.')
			{
				// NOK
				e.Handled = true;
			}

			// only allow one decimal point
			if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
			{
				// NOK
				e.Handled = true;
			}
		}

		private void IntegerTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			GenericTextBoxKeyPress(sender, e);
			if(e.KeyChar == '.')
			{
				// actually, NOK
				e.Handled = true;
			}
		}

		private void GenericTextChange(object sender, EventArgs e)
		{
			Debug.WriteLine(sender);
			System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)sender;
			if (tb.Text.Length == 0)
			{
				tb.BackColor = Color.PaleVioletRed;
			}
			else
			{
				if(double.TryParse(tb.Text, out double result))
				{
					if(result != 0)
						tb.BackColor = System.Drawing.SystemColors.Window;
					else
						tb.BackColor = Color.PaleVioletRed;
				}
				else
				{
					tb.Text = "1";
				}
			}
		}

		private void StepsParamsTextChange(object sender, EventArgs e)
		{
			GenericTextChange(sender, e);
			//double dt = double.Parse(this.dt.Text);
			if(!double.TryParse(this.dt.Text, out double dt))
			{
				dt = 1;
			}
			if (!double.TryParse(this.seconds.Text, out double seconds))
			{
				seconds = 1;
			}
			if (!int.TryParse(this.steps.Text, out int steps))
			{
				if(this.steps.Text.Length > 0)
				{
					this.steps.Text = "1";
				}
				steps = 1;
			}
			if(sender.Equals(this.dt))
			{
				this.seconds.Text = "" + steps * dt;
			}
			else if(sender.Equals(this.seconds))
			{
				this.steps.Text = "" + (int)(Math.Ceiling(seconds / dt));
			}
			else if(sender.Equals(this.steps))
			{
				this.seconds.Text = "" + steps * dt;
			}
		}

		private void SecondsTextChange(object sender, EventArgs e)
		{
			Debug.WriteLine(",");
		}

		private void GenericTextBoxLeave(object sender, EventArgs e)
		{
			System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)sender;
			if (tb.TextLength == 0)
			{
				tb.Text = "1";
			}
		}

		private void SetAllEnabled(bool enable)
		{
			inputSettingsPanel.Enabled = enable;
			outputSettingsPanel.Enabled = enable;
			startBtn.Enabled = enable;
			GraphicalInputPanel.Enabled = enable;
		}

		private void WorkerDone(object sender, RunWorkerCompletedEventArgs e)
		{
			SetAllEnabled(true);
		}

		private void ChangeRotationIncrease(object sender, EventArgs e, int axis)
		{
			int valx = int.Parse(xAxisValue.Text);
			int valy = int.Parse(yAxisValue.Text);
			int valz = int.Parse(zAxisValue.Text);
			switch (axis)
			{
				case 1:
					valx = (valx + 90) % 360;
					break;
				case 2:
					valy = (valy + 90) % 360;
					break;
				case 3:
					valz = (valz + 90) % 360;
					break;
				default:
					return;
			}
			xAxisValue.Text = "" + valx;
			yAxisValue.Text = "" + valy;
			zAxisValue.Text = "" + valz;
			picture.RotateObject(Deg2Rad(valx), Deg2Rad(valy), Deg2Rad(valz));
			picture.DrawScaledObjectOn(canvas);
		}

		private void ChangeRotationDecrease(object sender, EventArgs e, int axis)
		{
			int valx = int.Parse(xAxisValue.Text);
			int valy = int.Parse(yAxisValue.Text);
			int valz = int.Parse(zAxisValue.Text);
			switch (axis)
			{
				case 1:
					valx = (valx - 90) % 360;
					break;
				case 2:
					valy = (valy - 90) % 360;
					break;
				case 3:
					valz = (valz - 90) % 360;
					break;
				default:
					return;
			}
			xAxisValue.Text = "" + valx;
			yAxisValue.Text = "" + valy;
			zAxisValue.Text = "" + valz;
			picture.RotateObject(Deg2Rad(valx), Deg2Rad(valy), Deg2Rad(valz));
			picture.DrawScaledObjectOn(canvas);
		}

		private double Deg2Rad(int deg)
		{
			return (Math.PI / 180) * deg;
		}

		private void LoadData(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			ids = new InputDataSet(selectDataDalog.FileName, double.Parse(voxelSize.Text), worker);
		}

		private void DataLoaded(object sender, RunWorkerCompletedEventArgs e)
		{
			List<Point> points = ids.GetPoints();
			picture.Clear();

			picture.MakeSelection(double.Parse(this.avDistance.Text), points);
			picture.RotateObject(0, 0, 0);
			picture.DrawScaledObjectOn(canvas);
		}

		private void Canvas_LoseFocus(object sender, EventArgs e)
		{
			Cursor.Show();
			picture.UpdateMousePointer(canvas, -20, -20);
		}

		private void MaxLevelChange(object sender, EventArgs e)
		{
			picture.ChangeLevel(levelTracker.Value, levelTracker.Maximum);
			picture.DrawScaledObjectOn(canvas);
		}

		private void UpdateDirection(object sender, EventArgs e)
		{
			dir = 0;
			if (this.direction.SelectedIndex == 0)
			{
				dir = 1;
			}
			else if (this.direction.SelectedIndex == 1)
			{
				dir = -1;
			}
		}

		private void DrawStreamLineChange(object sender, EventArgs e)
		{
			if (sls != null)
			{
				Debug.WriteLine("sls: " + sls.GetStreamLines().Count);
				if (displayStreamLines.Checked && sls.GetStreamLines().Count > 0)
				{
					List<StreamLine> temp = new List<StreamLine>();
					for (int i = 0; i < sls.GetStreamLines().Count; i += streamLineDensity.Value)
					{
						temp.Add(sls.GetStreamLines()[i]);
					}
					Debug.WriteLine("Drawing " + temp.Count + " streamlines");
					picture.DrawStream(true, temp);
				}
				else
				{
					picture.DrawStream(false, null);
				}
				int valx = int.Parse(xAxisValue.Text);
				int valy = int.Parse(yAxisValue.Text);
				int valz = int.Parse(zAxisValue.Text);
				picture.RotateObject(Deg2Rad(valx), Deg2Rad(valy), Deg2Rad(valz));
				picture.DrawScaledObjectOn(canvas);
			}
		}
	}
}
