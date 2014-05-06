using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FuzzySet_Sample_Generator
{
    public partial class Form1 : Form
    {
        private static Random random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void onGenerateClick(object sender, EventArgs e)
        {
            int numSets = (int)this.setsSpinner.Value;
            int numSamples = (int)this.samplesSpinner.Value;

            StreamWriter outFile = new StreamWriter("Generated_sample_" + numSets + "_Sets_" + numSamples + "_Samples.txt");

            outFile.WriteLine(getHeader(numSets));

            for (int i = 0; i < numSamples; i++)
            {
                StringBuilder sampleLine = new StringBuilder();

                //add name
                sampleLine.Append(RandomString(4) + "\t");

                //the idea is to start at 100% and essentially bag pick the values for the sets
                int randomness = 100;
                for (int j = 0; j < numSets - 1; j++)
                {
                    int setRandom = random.Next(randomness);

                    sampleLine.Append(setRandom + "\t");
                }

                //now we add the remainder to the last set
                sampleLine.Append(random.Next(randomness));

                outFile.WriteLine(sampleLine.ToString());
            }
            outFile.Close();
        }

        private string getHeader(int numSets)
        {
            StringBuilder headerString = new StringBuilder();

            headerString.Append("Name\t");
            for (int i = 0; i < numSets; i++)
            {
                headerString.Append(RandomString(4));
                if (i + 1 != numSets)
                {
                    headerString.Append("\t");
                }
            }
            return headerString.ToString();
        }

        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

    }
}
