namespace FuzzySetDynamicVisualizer
{
    partial class Form1
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gettingStartedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.heatmapRecursionDepth = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.centerButton = new System.Windows.Forms.Button();
            this.heatmapCheckbox = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.memberRadiusSpinner = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.memberAlphaSpinner = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.zoomSpinner = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.heatmapRecursionDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memberRadiusSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memberAlphaSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 522);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(848, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(48, 17);
            this.statusLabel.Text = "Starting";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(848, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.onOpenClicked);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gettingStartedToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // gettingStartedToolStripMenuItem
            // 
            this.gettingStartedToolStripMenuItem.Name = "gettingStartedToolStripMenuItem";
            this.gettingStartedToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.gettingStartedToolStripMenuItem.Text = "Getting started";
            this.gettingStartedToolStripMenuItem.Click += new System.EventHandler(this.onGettingStartedClick);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.onAboutClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(848, 498);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.heatmapRecursionDepth);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.centerButton);
            this.panel2.Controls.Add(this.heatmapCheckbox);
            this.panel2.Controls.Add(this.numericUpDown1);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.memberRadiusSpinner);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.memberAlphaSpinner);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.zoomSpinner);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(751, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(94, 492);
            this.panel2.TabIndex = 1;
            // 
            // heatmapRecursionDepth
            // 
            this.heatmapRecursionDepth.Location = new System.Drawing.Point(4, 319);
            this.heatmapRecursionDepth.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.heatmapRecursionDepth.Name = "heatmapRecursionDepth";
            this.heatmapRecursionDepth.Size = new System.Drawing.Size(75, 20);
            this.heatmapRecursionDepth.TabIndex = 12;
            this.heatmapRecursionDepth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.heatmapRecursionDepth.ValueChanged += new System.EventHandler(this.onRecursionDepthChange);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1, 303);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Heatmap Depth";
            // 
            // centerButton
            // 
            this.centerButton.Location = new System.Drawing.Point(7, 57);
            this.centerButton.Name = "centerButton";
            this.centerButton.Size = new System.Drawing.Size(75, 23);
            this.centerButton.TabIndex = 10;
            this.centerButton.Text = "Center";
            this.centerButton.UseVisualStyleBackColor = true;
            this.centerButton.Click += new System.EventHandler(this.onCenterButtonClick);
            // 
            // heatmapCheckbox
            // 
            this.heatmapCheckbox.AutoSize = true;
            this.heatmapCheckbox.Location = new System.Drawing.Point(3, 274);
            this.heatmapCheckbox.Name = "heatmapCheckbox";
            this.heatmapCheckbox.Size = new System.Drawing.Size(91, 17);
            this.heatmapCheckbox.TabIndex = 9;
            this.heatmapCheckbox.Text = "Use Heatmap";
            this.heatmapCheckbox.UseVisualStyleBackColor = true;
            this.heatmapCheckbox.CheckStateChanged += new System.EventHandler(this.onHeatmapChecked);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(7, 236);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(75, 20);
            this.numericUpDown1.TabIndex = 7;
            this.numericUpDown1.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 220);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "SetGroupRadius";
            // 
            // memberRadiusSpinner
            // 
            this.memberRadiusSpinner.Location = new System.Drawing.Point(7, 174);
            this.memberRadiusSpinner.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.memberRadiusSpinner.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.memberRadiusSpinner.Name = "memberRadiusSpinner";
            this.memberRadiusSpinner.Size = new System.Drawing.Size(75, 20);
            this.memberRadiusSpinner.TabIndex = 5;
            this.memberRadiusSpinner.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.memberRadiusSpinner.Click += new System.EventHandler(this.onMemberRadiusSpinnerClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Member Radius";
            // 
            // memberAlphaSpinner
            // 
            this.memberAlphaSpinner.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.memberAlphaSpinner.Location = new System.Drawing.Point(7, 117);
            this.memberAlphaSpinner.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.memberAlphaSpinner.Name = "memberAlphaSpinner";
            this.memberAlphaSpinner.Size = new System.Drawing.Size(75, 20);
            this.memberAlphaSpinner.TabIndex = 3;
            this.memberAlphaSpinner.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.memberAlphaSpinner.ValueChanged += new System.EventHandler(this.onMemberAlphaChange);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Member Visiblity";
            // 
            // zoomSpinner
            // 
            this.zoomSpinner.DecimalPlaces = 1;
            this.zoomSpinner.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.zoomSpinner.Location = new System.Drawing.Point(7, 30);
            this.zoomSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.zoomSpinner.Name = "zoomSpinner";
            this.zoomSpinner.Size = new System.Drawing.Size(78, 20);
            this.zoomSpinner.TabIndex = 1;
            this.zoomSpinner.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.zoomSpinner.ValueChanged += new System.EventHandler(this.onZoomSpinnerValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Zoom";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 544);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Fuzzy Set Affinity Visualizer";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.heatmapRecursionDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memberRadiusSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memberAlphaSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomSpinner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown zoomSpinner;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown memberAlphaSpinner;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown memberRadiusSpinner;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox heatmapCheckbox;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gettingStartedToolStripMenuItem;
        private System.Windows.Forms.Button centerButton;
        private System.Windows.Forms.NumericUpDown heatmapRecursionDepth;
        private System.Windows.Forms.Label label5;
    }
}

