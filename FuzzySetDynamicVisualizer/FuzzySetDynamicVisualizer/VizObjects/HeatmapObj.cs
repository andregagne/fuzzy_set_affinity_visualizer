using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FuzzySetDynamicVisualizer.VizObjects;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer.VisObjects
{
    class HeatmapObj : VizObject
    {
        Dictionary<Set, int> associations = null;
        int numTallys = 0;
        int maxTallys = 1;

        public HeatmapObj(Dictionary<Set, int> associations) : base()
        {
            radius = 10;
        }

        public override void visualize(System.Drawing.Graphics graphics)
        {
            graphics.FillEllipse(new SolidBrush(colour), location.X, location.Y, this.radius, this.radius);
        }

        public override void move(Point newPoint)
        {
            this.location.X = newPoint.X;
            this.location.Y = newPoint.Y;
        }

        public void move(int deltaX, int deltaY)
        {
            this.location.X += deltaX;
            this.location.Y += deltaY;
        }

        public void addTally()
        {
            this.numTallys++;
            if (numTallys > maxTallys)
            {
                setMaxTallys(numTallys);
            }
        }

        public void setMaxTallys(int maxTallys)
        {
            this.maxTallys = maxTallys;

            int newAlpha = (int) ((float) numTallys / (float)maxTallys * 255f);

            this.colour = Color.FromArgb(newAlpha, colour);            
        }

        public Dictionary<Set, int> getAssociations()
        {
            return associations;
        }
    }
}
