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
        char delimiter = '\t';

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
            using (StreamReader file = new StreamReader(fileName))
            {
                if (vizPanel != null)
                {
                    List<Set> sets = getSets(file);  //reads and strips off the first line
                    int membership = 0;
                    int numMembers = 0;                    
                    int[] setMemberships;
                    
                    //for line parsing
                    String lineString;
                    int stringStartIndex = 0;
                    int tabIndex = 0;
                    String memberName = null;
                    int setIndex = 0;
                    String membershipValue = null;
                    

                    while (!file.EndOfStream)
                    {
                        stringStartIndex = 0;
                        setIndex = 0;                        
                        setMemberships = new int[sets.Count];

                                                
                        lineString = file.ReadLine();                                            

                        /*
                         * in order to cut down on memory usage we're going to step through the string for the file line
                         * manually looking for tabdeliniations
                         */

                        //grab the name first
                        tabIndex = lineString.IndexOf(delimiter);
                        memberName = lineString.Substring(0, tabIndex);
                        stringStartIndex = tabIndex + 1;

                        //now we go until we can't find anymore
                        // TODO: make this harder to break with more error checking code
                        while (stringStartIndex < lineString.Length)
                        {                           
                            membership = 0;
                            
                            //supposed to find the location of the next tab stop and parse the int within it.
                            tabIndex = lineString.IndexOf(delimiter, stringStartIndex);
                            if (tabIndex > 0 - 1) // this means there's still stuff at the end of the line without a tab at the end of it
                            {
                                membershipValue = lineString.Substring(stringStartIndex, tabIndex - stringStartIndex);
                                stringStartIndex = tabIndex + 1;
                            }
                            else
                            {
                                membershipValue = lineString.Substring(stringStartIndex, lineString.Length - stringStartIndex);
                                stringStartIndex = lineString.Length;
                            }
                            
                            //and now we parse
                            if (int.TryParse(membershipValue, out membership))
                                //TODO: consider removing dictionaries entirely as they take up unneeded space
                                setMemberships[setIndex] = membership;
                            else
                                System.Console.WriteLine("Could not parse " + membershipValue);                                

                            
                            setIndex++;
                        }

                        if(setIndex != sets.Count)
                            System.Console.WriteLine("Something is off, the # of sets parsed was " + setIndex +", should have been " + sets.Count);

                        new Member(memberName, sets, setMemberships);  //adds itself to the set that it has the highest affinity to
                        numMembers++;
                    }


                    List<VizObject> vizObj = new List<VizObject>();

                    if (numMembers >= 100000)
                    {
                        this.heatmapCheckbox.Checked = true;
                        vizPanel.useHeatmap(true);
                    }
                    
                    foreach (Set s in sets)
                    {
                        vizObj.Add(new SetObject(s, Color.Blue, vizPanel.Width, vizPanel.Height, (int)this.heatmapRecursionSpinner.Value, this.heatmapCheckbox.Checked));
                    }
                    
                    vizPanel.loadVizObjects(vizObj);
                }
                vizPanel.Invalidate();
                this.Text = titleText += fileName;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private List<Set> getSets(StreamReader file)
        {
            List<Set> sets = new List<Set>();

            string fileLine = file.ReadLine();
            string[] broken = fileLine.Split(delimiter);

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
