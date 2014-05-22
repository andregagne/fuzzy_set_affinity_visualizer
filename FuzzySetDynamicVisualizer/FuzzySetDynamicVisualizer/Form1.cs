using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FuzzySetDynamicVisualizer.DataStructures;
using FuzzySetDynamicVisualizer.VizObjects;

namespace FuzzySetDynamicVisualizer
{
    public partial class Form1 : Form
    {
        FuzzySetsVizPanel vizPanel;
        static string titleText = "Fuzzy Set Affinity Visualizer";

        public Form1()
        {
            InitializeComponent();
            vizPanel = new FuzzySetsVizPanel(this.statusLabel);
            this.tableLayoutPanel1.Controls.Add(vizPanel);
        }

        private void onOpenClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                loadFile(fileDialog.FileName);
            }
        }

        private void loadFile(string fileName)
        {
            StreamReader file = new StreamReader(fileName);
            if (vizPanel != null)
            {
                List<Member> memberList = new List<Member>();
                List<Set> sets = getSets(file);

                while (!file.EndOfStream)
                {
                    Dictionary<Set, int> setMemberships = new Dictionary<Set, int>();

                    string[] linePieces = file.ReadLine().Split(new char[] { '\t' });
                    for (int i = 1; i < linePieces.Length; i++)
                    {
                        int membership = 0;
                        int.TryParse(linePieces[i], out membership);
                        setMemberships.Add(sets[i - 1], membership);
                    }

                    memberList.Add(new Member(linePieces[0], setMemberships));
                }

                List<VizObject> vizObj = new List<VizObject>();

                foreach (Set s in sets)
                {
                    vizObj.Add(new SetObject(s, Color.Blue, vizPanel.Width, vizPanel.Height));
                }

                vizPanel.loadVizObjects(vizObj);
            }
            vizPanel.Invalidate();
            this.Text = titleText += fileName;
        }

        private List<Set> getSets(StreamReader file)
        {
            List<Set> sets = new List<Set>();

            string fileLine = file.ReadLine();
            string[] broken = fileLine.Split(new char[] { '\t' });

            for (int i = 1; i < broken.Length; i++)
            {
                sets.Add(new Set(broken[i]));
            }
            Console.WriteLine("Sets found " + sets.ToString());
            return sets;
        }

        private void onZoomSpinnerValueChanged(object sender, EventArgs e)
        {
            this.vizPanel.setScale((float)zoomSpinner.Value);
        }

        private void onMemberAlphaChange(object sender, EventArgs e)
        {
            this.vizPanel.changeMemberAlpha((int)memberAlphaSpinner.Value);
        }

        private void onMemberRadiusSpinnerClick(object sender, EventArgs e)
        {
            this.vizPanel.setMemberRadius((int)memberRadiusSpinner.Value);
        }

        private void onHeatmapChecked(object sender, EventArgs e)
        {
            vizPanel.useHeatmap(heatmapCheckbox.Checked);
        }

        private void onAboutClick(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.ShowDialog();
        }

        private void onGettingStartedClick(object sender, EventArgs e)
        {
            GettingStarted box = new GettingStarted();
            box.ShowDialog();
        }

        private void onCenterButtonClick(object sender, EventArgs e)
        {
            this.vizPanel.recenterObjects();
        }

        private void heatmapValueChanged(object sender, EventArgs e)
        {
            this.vizPanel.heatmapValueChanged((int) heatmapRecursionSpinner.Value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
