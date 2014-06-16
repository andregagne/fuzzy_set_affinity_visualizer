namespace FuzzySet_Sample_Generator
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.setsSpinner = new System.Windows.Forms.NumericUpDown();
            this.samplesSpinner = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.setsSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.samplesSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of Sets";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Number Of Samples";
            // 
            // setsSpinner
            // 
            this.setsSpinner.Location = new System.Drawing.Point(13, 30);
            this.setsSpinner.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.setsSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.setsSpinner.Name = "setsSpinner";
            this.setsSpinner.Size = new System.Drawing.Size(120, 20);
            this.setsSpinner.TabIndex = 2;
            this.setsSpinner.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // samplesSpinner
            // 
            this.samplesSpinner.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.samplesSpinner.Location = new System.Drawing.Point(12, 103);
            this.samplesSpinner.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.samplesSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.samplesSpinner.Name = "samplesSpinner";
            this.samplesSpinner.Size = new System.Drawing.Size(120, 20);
            this.samplesSpinner.TabIndex = 3;
            this.samplesSpinner.ThousandsSeparator = true;
            this.samplesSpinner.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 174);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Generate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.onGenerateClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.samplesSpinner);
            this.Controls.Add(this.setsSpinner);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.setsSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.samplesSpinner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown setsSpinner;
        private System.Windows.Forms.NumericUpDown samplesSpinner;
        private System.Windows.Forms.Button button1;
    }
}

