using System;
using System.Reflection;
using System.Windows.Forms;

namespace AdvectionCalculationsGUI
{
	partial class GUI
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.topLevelLayout = new System.Windows.Forms.TableLayoutPanel();
			this.graphicsPanel = new System.Windows.Forms.TableLayoutPanel();
			this.canvas_holder = new System.Windows.Forms.Panel();
			this.canvas = new System.Windows.Forms.PictureBox();
			this.startBtn = new System.Windows.Forms.Button();
			this.settingsPanel = new System.Windows.Forms.Panel();
			this.settingsTable = new System.Windows.Forms.TableLayoutPanel();
			this.inputSettingsPanel = new System.Windows.Forms.Panel();
			this.inputSettingsTable = new System.Windows.Forms.TableLayoutPanel();
			this.selectInputFileBtn = new System.Windows.Forms.Button();
			this.loadDataBtn = new System.Windows.Forms.Button();
			this.label_filename = new System.Windows.Forms.Label();
			this.label_avDistance = new System.Windows.Forms.Label();
			this.label_voxelSize = new System.Windows.Forms.Label();
			this.avDistance = new System.Windows.Forms.TextBox();
			this.voxelSize = new System.Windows.Forms.TextBox();
			this.outputSettingsPanel = new System.Windows.Forms.Panel();
			this.outputSettingsTable = new System.Windows.Forms.TableLayoutPanel();
			this.selectFolderBtn = new System.Windows.Forms.Button();
			this.label_selectFolder = new System.Windows.Forms.Label();
			this.label_resolution = new System.Windows.Forms.Label();
			this.label_dt = new System.Windows.Forms.Label();
			this.label_seconds = new System.Windows.Forms.Label();
			this.label_steps = new System.Windows.Forms.Label();
			this.label_direction = new System.Windows.Forms.Label();
			this.direction = new System.Windows.Forms.ComboBox();
			this.resolution = new System.Windows.Forms.TextBox();
			this.dt = new System.Windows.Forms.TextBox();
			this.seconds = new System.Windows.Forms.TextBox();
			this.steps = new System.Windows.Forms.TextBox();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.selectDataDalog = new System.Windows.Forms.OpenFileDialog();
			this.selectOutputFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.fieldNormilizerWorker = new System.ComponentModel.BackgroundWorker();
			this.topLevelLayout.SuspendLayout();
			this.graphicsPanel.SuspendLayout();
			this.canvas_holder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
			this.settingsPanel.SuspendLayout();
			this.settingsTable.SuspendLayout();
			this.inputSettingsPanel.SuspendLayout();
			this.inputSettingsTable.SuspendLayout();
			this.outputSettingsPanel.SuspendLayout();
			this.outputSettingsTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// topLevelLayout
			// 
			this.topLevelLayout.ColumnCount = 2;
			this.topLevelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.topLevelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
			this.topLevelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.topLevelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.topLevelLayout.Controls.Add(this.graphicsPanel, 1, 0);
			this.topLevelLayout.Controls.Add(this.startBtn, 0, 1);
			this.topLevelLayout.Controls.Add(this.settingsPanel, 0, 0);
			this.topLevelLayout.Controls.Add(this.progressBar, 1, 1);
			this.topLevelLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.topLevelLayout.Location = new System.Drawing.Point(0, 0);
			this.topLevelLayout.Name = "topLevelLayout";
			this.topLevelLayout.RowCount = 2;
			this.topLevelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
			this.topLevelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.topLevelLayout.Size = new System.Drawing.Size(1366, 655);
			this.topLevelLayout.TabIndex = 0;
			// 
			// graphicsPanel
			// 
			this.graphicsPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.graphicsPanel.ColumnCount = 1;
			this.graphicsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.graphicsPanel.Controls.Add(this.canvas_holder, 0, 0);
			this.graphicsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graphicsPanel.Location = new System.Drawing.Point(344, 3);
			this.graphicsPanel.Name = "graphicsPanel";
			this.graphicsPanel.RowCount = 2;
			this.graphicsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 84.7487F));
			this.graphicsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.2513F));
			this.graphicsPanel.Size = new System.Drawing.Size(1019, 583);
			this.graphicsPanel.TabIndex = 1;
			// 
			// canvas_holder
			// 
			this.canvas_holder.Controls.Add(this.canvas);
			this.canvas_holder.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.canvas_holder.Dock = System.Windows.Forms.DockStyle.Fill;
			this.canvas_holder.Location = new System.Drawing.Point(4, 4);
			this.canvas_holder.Name = "canvas_holder";
			this.canvas_holder.Size = new System.Drawing.Size(1011, 485);
			this.canvas_holder.TabIndex = 0;
			this.canvas_holder.Resize += new System.EventHandler(this.Canvas_Resize);
			// 
			// canvas
			// 
			this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
			this.canvas.Location = new System.Drawing.Point(0, 0);
			this.canvas.Margin = new System.Windows.Forms.Padding(0);
			this.canvas.Name = "canvas";
			this.canvas.Size = new System.Drawing.Size(1011, 485);
			this.canvas.TabIndex = 0;
			this.canvas.TabStop = false;
			this.canvas.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseClick);
			this.canvas.MouseEnter += new System.EventHandler(this.Canvas_GetFocus);
			this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseMove);
			// 
			// startBtn
			// 
			this.startBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.startBtn.Enabled = false;
			this.startBtn.Location = new System.Drawing.Point(44, 599);
			this.startBtn.Name = "startBtn";
			this.startBtn.Size = new System.Drawing.Size(253, 45);
			this.startBtn.TabIndex = 2;
			this.startBtn.Text = "Start";
			this.startBtn.UseVisualStyleBackColor = true;
			this.startBtn.Click += new System.EventHandler(this.Start_Click);
			// 
			// settingsPanel
			// 
			this.settingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.settingsPanel.Controls.Add(this.settingsTable);
			this.settingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.settingsPanel.Location = new System.Drawing.Point(3, 3);
			this.settingsPanel.Name = "settingsPanel";
			this.settingsPanel.Padding = new System.Windows.Forms.Padding(4);
			this.settingsPanel.Size = new System.Drawing.Size(335, 583);
			this.settingsPanel.TabIndex = 3;
			// 
			// settingsTable
			// 
			this.settingsTable.ColumnCount = 1;
			this.settingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.settingsTable.Controls.Add(this.inputSettingsPanel, 0, 0);
			this.settingsTable.Controls.Add(this.outputSettingsPanel, 0, 1);
			this.settingsTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.settingsTable.Location = new System.Drawing.Point(4, 4);
			this.settingsTable.Name = "settingsTable";
			this.settingsTable.RowCount = 2;
			this.settingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.settingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.settingsTable.Size = new System.Drawing.Size(325, 573);
			this.settingsTable.TabIndex = 0;
			// 
			// inputSettingsPanel
			// 
			this.inputSettingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.inputSettingsPanel.Controls.Add(this.inputSettingsTable);
			this.inputSettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.inputSettingsPanel.Location = new System.Drawing.Point(3, 3);
			this.inputSettingsPanel.Name = "inputSettingsPanel";
			this.inputSettingsPanel.Padding = new System.Windows.Forms.Padding(4);
			this.inputSettingsPanel.Size = new System.Drawing.Size(319, 223);
			this.inputSettingsPanel.TabIndex = 3;
			// 
			// inputSettingsTable
			// 
			this.inputSettingsTable.ColumnCount = 3;
			this.inputSettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.inputSettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
			this.inputSettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
			this.inputSettingsTable.Controls.Add(this.selectInputFileBtn, 2, 1);
			this.inputSettingsTable.Controls.Add(this.loadDataBtn, 2, 7);
			this.inputSettingsTable.Controls.Add(this.label_filename, 0, 1);
			this.inputSettingsTable.Controls.Add(this.label_avDistance, 0, 3);
			this.inputSettingsTable.Controls.Add(this.label_voxelSize, 0, 5);
			this.inputSettingsTable.Controls.Add(this.avDistance, 2, 3);
			this.inputSettingsTable.Controls.Add(this.voxelSize, 2, 5);
			this.inputSettingsTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.inputSettingsTable.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.inputSettingsTable.Location = new System.Drawing.Point(4, 4);
			this.inputSettingsTable.MinimumSize = new System.Drawing.Size(0, 50);
			this.inputSettingsTable.Name = "inputSettingsTable";
			this.inputSettingsTable.RowCount = 9;
			this.inputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
			this.inputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.83333F));
			this.inputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
			this.inputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.83333F));
			this.inputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
			this.inputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.83333F));
			this.inputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.166667F));
			this.inputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.83333F));
			this.inputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.inputSettingsTable.Size = new System.Drawing.Size(309, 213);
			this.inputSettingsTable.TabIndex = 0;
			// 
			// selectInputFileBtn
			// 
			this.selectInputFileBtn.AutoSize = true;
			this.selectInputFileBtn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.selectInputFileBtn.Location = new System.Drawing.Point(200, 8);
			this.selectInputFileBtn.Margin = new System.Windows.Forms.Padding(0);
			this.selectInputFileBtn.MinimumSize = new System.Drawing.Size(0, 10);
			this.selectInputFileBtn.Name = "selectInputFileBtn";
			this.selectInputFileBtn.Size = new System.Drawing.Size(109, 44);
			this.selectInputFileBtn.TabIndex = 0;
			this.selectInputFileBtn.Text = "Select Data";
			this.selectInputFileBtn.UseVisualStyleBackColor = true;
			this.selectInputFileBtn.Click += new System.EventHandler(this.SelectInputFile_Click);
			// 
			// loadDataBtn
			// 
			this.loadDataBtn.AutoSize = true;
			this.loadDataBtn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loadDataBtn.Enabled = false;
			this.loadDataBtn.Location = new System.Drawing.Point(200, 164);
			this.loadDataBtn.Margin = new System.Windows.Forms.Padding(0);
			this.loadDataBtn.MinimumSize = new System.Drawing.Size(20, 40);
			this.loadDataBtn.Name = "loadDataBtn";
			this.loadDataBtn.Size = new System.Drawing.Size(109, 44);
			this.loadDataBtn.TabIndex = 3;
			this.loadDataBtn.Text = "Load Data";
			this.loadDataBtn.UseVisualStyleBackColor = true;
			this.loadDataBtn.Click += new System.EventHandler(this.LoadData_Click);
			// 
			// label_filename
			// 
			this.label_filename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.label_filename.AutoSize = true;
			this.label_filename.Location = new System.Drawing.Point(3, 21);
			this.label_filename.Margin = new System.Windows.Forms.Padding(3);
			this.label_filename.Name = "label_filename";
			this.label_filename.Size = new System.Drawing.Size(179, 17);
			this.label_filename.TabIndex = 4;
			this.label_filename.Text = "<>";
			// 
			// label_avDistance
			// 
			this.label_avDistance.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_avDistance.Enabled = false;
			this.label_avDistance.Location = new System.Drawing.Point(3, 63);
			this.label_avDistance.Margin = new System.Windows.Forms.Padding(3);
			this.label_avDistance.Name = "label_avDistance";
			this.label_avDistance.Size = new System.Drawing.Size(179, 38);
			this.label_avDistance.TabIndex = 5;
			this.label_avDistance.Text = "Distance";
			this.label_avDistance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_voxelSize
			// 
			this.label_voxelSize.AutoSize = true;
			this.label_voxelSize.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_voxelSize.Enabled = false;
			this.label_voxelSize.Location = new System.Drawing.Point(3, 115);
			this.label_voxelSize.Margin = new System.Windows.Forms.Padding(3);
			this.label_voxelSize.Name = "label_voxelSize";
			this.label_voxelSize.Size = new System.Drawing.Size(179, 38);
			this.label_voxelSize.TabIndex = 6;
			this.label_voxelSize.Text = "Voxel Size";
			this.label_voxelSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// avDistance
			// 
			this.avDistance.Enabled = false;
			this.avDistance.Location = new System.Drawing.Point(203, 63);
			this.avDistance.Name = "avDistance";
			this.avDistance.Size = new System.Drawing.Size(100, 22);
			this.avDistance.TabIndex = 7;
			// 
			// voxelSize
			// 
			this.voxelSize.Enabled = false;
			this.voxelSize.Location = new System.Drawing.Point(203, 115);
			this.voxelSize.Name = "voxelSize";
			this.voxelSize.Size = new System.Drawing.Size(100, 22);
			this.voxelSize.TabIndex = 8;
			// 
			// outputSettingsPanel
			// 
			this.outputSettingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.outputSettingsPanel.Controls.Add(this.outputSettingsTable);
			this.outputSettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.outputSettingsPanel.Enabled = false;
			this.outputSettingsPanel.Location = new System.Drawing.Point(3, 232);
			this.outputSettingsPanel.Name = "outputSettingsPanel";
			this.outputSettingsPanel.Padding = new System.Windows.Forms.Padding(4);
			this.outputSettingsPanel.Size = new System.Drawing.Size(319, 338);
			this.outputSettingsPanel.TabIndex = 3;
			// 
			// outputSettingsTable
			// 
			this.outputSettingsTable.ColumnCount = 3;
			this.outputSettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.outputSettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
			this.outputSettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
			this.outputSettingsTable.Controls.Add(this.selectFolderBtn, 2, 1);
			this.outputSettingsTable.Controls.Add(this.label_selectFolder, 0, 1);
			this.outputSettingsTable.Controls.Add(this.label_resolution, 0, 3);
			this.outputSettingsTable.Controls.Add(this.label_dt, 0, 5);
			this.outputSettingsTable.Controls.Add(this.label_seconds, 0, 7);
			this.outputSettingsTable.Controls.Add(this.label_steps, 0, 9);
			this.outputSettingsTable.Controls.Add(this.label_direction, 0, 11);
			this.outputSettingsTable.Controls.Add(this.direction, 2, 11);
			this.outputSettingsTable.Controls.Add(this.resolution, 2, 3);
			this.outputSettingsTable.Controls.Add(this.dt, 2, 5);
			this.outputSettingsTable.Controls.Add(this.seconds, 2, 7);
			this.outputSettingsTable.Controls.Add(this.steps, 2, 9);
			this.outputSettingsTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.outputSettingsTable.Location = new System.Drawing.Point(4, 4);
			this.outputSettingsTable.Name = "outputSettingsTable";
			this.outputSettingsTable.RowCount = 13;
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.857143F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.33333F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.857143F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.33333F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.857143F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.33333F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.857143F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.33333F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.857143F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.33333F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.857143F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.33333F));
			this.outputSettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.857143F));
			this.outputSettingsTable.Size = new System.Drawing.Size(309, 328);
			this.outputSettingsTable.TabIndex = 1;
			// 
			// selectFolderBtn
			// 
			this.selectFolderBtn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.selectFolderBtn.Location = new System.Drawing.Point(200, 9);
			this.selectFolderBtn.Margin = new System.Windows.Forms.Padding(0);
			this.selectFolderBtn.Name = "selectFolderBtn";
			this.selectFolderBtn.Size = new System.Drawing.Size(109, 43);
			this.selectFolderBtn.TabIndex = 0;
			this.selectFolderBtn.Text = "Select Folder";
			this.selectFolderBtn.UseVisualStyleBackColor = true;
			this.selectFolderBtn.Click += new System.EventHandler(this.SelectFolder_Click);
			// 
			// label_selectFolder
			// 
			this.label_selectFolder.AutoSize = true;
			this.label_selectFolder.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_selectFolder.Location = new System.Drawing.Point(3, 12);
			this.label_selectFolder.Margin = new System.Windows.Forms.Padding(3);
			this.label_selectFolder.Name = "label_selectFolder";
			this.label_selectFolder.Size = new System.Drawing.Size(179, 37);
			this.label_selectFolder.TabIndex = 6;
			this.label_selectFolder.Text = "<>";
			this.label_selectFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_resolution
			// 
			this.label_resolution.AutoSize = true;
			this.label_resolution.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_resolution.Location = new System.Drawing.Point(3, 64);
			this.label_resolution.Margin = new System.Windows.Forms.Padding(3);
			this.label_resolution.Name = "label_resolution";
			this.label_resolution.Size = new System.Drawing.Size(179, 37);
			this.label_resolution.TabIndex = 7;
			this.label_resolution.Text = "Field Resolution";
			this.label_resolution.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_dt
			// 
			this.label_dt.AutoSize = true;
			this.label_dt.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_dt.Location = new System.Drawing.Point(3, 116);
			this.label_dt.Margin = new System.Windows.Forms.Padding(3);
			this.label_dt.Name = "label_dt";
			this.label_dt.Size = new System.Drawing.Size(179, 37);
			this.label_dt.TabIndex = 8;
			this.label_dt.Text = "dT [ms]";
			this.label_dt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_seconds
			// 
			this.label_seconds.AutoSize = true;
			this.label_seconds.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_seconds.Location = new System.Drawing.Point(3, 168);
			this.label_seconds.Margin = new System.Windows.Forms.Padding(3);
			this.label_seconds.Name = "label_seconds";
			this.label_seconds.Size = new System.Drawing.Size(179, 37);
			this.label_seconds.TabIndex = 9;
			this.label_seconds.Text = "Seconds";
			this.label_seconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_steps
			// 
			this.label_steps.AutoSize = true;
			this.label_steps.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_steps.Location = new System.Drawing.Point(3, 220);
			this.label_steps.Margin = new System.Windows.Forms.Padding(3);
			this.label_steps.Name = "label_steps";
			this.label_steps.Size = new System.Drawing.Size(179, 37);
			this.label_steps.TabIndex = 10;
			this.label_steps.Text = "Steps";
			this.label_steps.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_direction
			// 
			this.label_direction.AutoSize = true;
			this.label_direction.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_direction.Location = new System.Drawing.Point(3, 272);
			this.label_direction.Margin = new System.Windows.Forms.Padding(3);
			this.label_direction.Name = "label_direction";
			this.label_direction.Size = new System.Drawing.Size(179, 37);
			this.label_direction.TabIndex = 11;
			this.label_direction.Text = "Direction";
			this.label_direction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// direction
			// 
			this.direction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.direction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.direction.FormattingEnabled = true;
			this.direction.Items.AddRange(new object[] {
            "Forward",
            "Backward",
            "Both"});
			this.direction.Location = new System.Drawing.Point(203, 278);
			this.direction.Name = "direction";
			this.direction.Size = new System.Drawing.Size(103, 24);
			this.direction.TabIndex = 12;
			// 
			// resolution
			// 
			this.resolution.Location = new System.Drawing.Point(203, 64);
			this.resolution.Name = "resolution";
			this.resolution.Size = new System.Drawing.Size(100, 22);
			this.resolution.TabIndex = 13;
			// 
			// dt
			// 
			this.dt.Location = new System.Drawing.Point(203, 116);
			this.dt.Name = "dt";
			this.dt.Size = new System.Drawing.Size(100, 22);
			this.dt.TabIndex = 14;
			// 
			// seconds
			// 
			this.seconds.Location = new System.Drawing.Point(203, 168);
			this.seconds.Name = "seconds";
			this.seconds.Size = new System.Drawing.Size(100, 22);
			this.seconds.TabIndex = 15;
			// 
			// steps
			// 
			this.steps.Location = new System.Drawing.Point(203, 220);
			this.steps.Name = "steps";
			this.steps.Size = new System.Drawing.Size(100, 22);
			this.steps.TabIndex = 16;
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(344, 602);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(1019, 39);
			this.progressBar.TabIndex = 4;
			// 
			// selectDataDalog
			// 
			this.selectDataDalog.FileName = "Dataset";
			// 
			// fieldNormilizerWorker
			// 
			this.fieldNormilizerWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			this.fieldNormilizerWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.ProgressChanged);
			// 
			// GUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1366, 655);
			this.Controls.Add(this.topLevelLayout);
			this.MinimumSize = new System.Drawing.Size(18, 200);
			this.Name = "GUI";
			this.Text = "Calculator";
			this.Load += new System.EventHandler(this.GUI_Load);
			this.topLevelLayout.ResumeLayout(false);
			this.graphicsPanel.ResumeLayout(false);
			this.canvas_holder.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
			this.settingsPanel.ResumeLayout(false);
			this.settingsTable.ResumeLayout(false);
			this.inputSettingsPanel.ResumeLayout(false);
			this.inputSettingsTable.ResumeLayout(false);
			this.inputSettingsTable.PerformLayout();
			this.outputSettingsPanel.ResumeLayout(false);
			this.outputSettingsTable.ResumeLayout(false);
			this.outputSettingsTable.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel topLevelLayout;

		private System.Windows.Forms.Panel settingsPanel;
		private System.Windows.Forms.TableLayoutPanel settingsTable;

		private System.Windows.Forms.TableLayoutPanel inputSettingsTable;
		private System.Windows.Forms.Panel inputSettingsPanel;

		private System.Windows.Forms.TableLayoutPanel outputSettingsTable;
		private System.Windows.Forms.Panel outputSettingsPanel;

		private System.Windows.Forms.TableLayoutPanel graphicsPanel;
		private System.Windows.Forms.Button selectInputFileBtn;
		private System.Windows.Forms.Button loadDataBtn;
		private System.Windows.Forms.Button startBtn;
		private System.Windows.Forms.Panel canvas_holder;
		private System.Windows.Forms.Label label_filename;
		private System.Windows.Forms.Label label_avDistance;
		private System.Windows.Forms.Label label_voxelSize;
		private System.Windows.Forms.Label label_selectFolder;
		private System.Windows.Forms.Label label_resolution;
		private System.Windows.Forms.Label label_dt;
		private System.Windows.Forms.Label label_seconds;
		private System.Windows.Forms.Label label_steps;
		private System.Windows.Forms.Label label_direction;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button selectFolderBtn;
		private System.Windows.Forms.ComboBox direction;
		private System.Windows.Forms.TextBox avDistance;
		private System.Windows.Forms.TextBox voxelSize;
		private System.Windows.Forms.TextBox resolution;
		private System.Windows.Forms.TextBox dt;
		private System.Windows.Forms.TextBox seconds;
		private System.Windows.Forms.TextBox steps;
		private System.Windows.Forms.OpenFileDialog selectDataDalog;
		private System.Windows.Forms.FolderBrowserDialog selectOutputFolderDialog;
		private System.ComponentModel.BackgroundWorker fieldNormilizerWorker;
		private PictureBox canvas;
	}
}

