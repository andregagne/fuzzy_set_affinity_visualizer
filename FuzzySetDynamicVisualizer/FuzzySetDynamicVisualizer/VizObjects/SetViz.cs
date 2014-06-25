using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer.VizObjects
{

    public class SetViz : VizObject
    {
        public readonly Set set;
        private Brush thisBrush;
        private int alpha = 50;
        private Font thisFont;
        private static float CORE_SCALE = 0.05f;
        protected readonly Random randGenerator = new Random();

        //for Heatmaps       
        private bool useHeatmaps = false;
        private int heatmapRecursionDepth = 1;
        private int globalMaxAmount = 1;
        private int[] heatmapBins;

        //memberStuff
        private List<MemberViz> members = new List<MemberViz>();
        private int memberAlpha = 30;
        private Color memberColor;
        private Brush memberBrush;

        public SetViz(Set set, Color color)
        {
            this.set = set;
            thisBrush = new SolidBrush(color);
            this.Radius = 10;
            this.thisFont = new Font("Arial", 8);

            this.memberColor = Color.FromArgb(alpha, Color.Black.R, Color.Black.G, Color.Black.B);
            this.memberBrush = new SolidBrush(memberColor);
        }

        public SetViz(Set set, Color color, int screenWidth, int screenHeight)
            : base(screenWidth, screenHeight, 0.2f)
        {
            this.set = set;
            thisBrush = new SolidBrush(color);
            this.thisFont = new Font("Arial", 8);

            this.memberColor = Color.FromArgb(memberAlpha, Color.Black.R, Color.Black.G, Color.Black.B);
            this.memberBrush = new SolidBrush(memberColor);

            foreach (Member m in set.members)
                this.addMemberObject(new MemberViz(m, memberBrush));
        }

        public SetViz(Set set, Color color, int screenWidth, int screenHeight, int heatmapRecursionDepth, bool useHeatmap)
            : base(screenWidth, screenHeight, 0.2f)
        {
            this.set = set;
            this.useHeatmaps = useHeatmap;
            this.heatmapRecursionDepth = heatmapRecursionDepth;
            thisBrush = new SolidBrush(color);
            this.thisFont = new Font("Arial", 8);

            this.memberColor = Color.FromArgb(memberAlpha, Color.Black.R, Color.Black.G, Color.Black.B);
            this.memberBrush = new SolidBrush(memberColor);

            foreach (Member m in set.members)
                this.addMemberObject(new MemberViz(m, memberBrush));

            if (useHeatmaps)
                this.arrange();
        }

        #region visualizations

        private static readonly Pen outterPen = new Pen(Color.DarkOrange) { Width = 3 };
        public override void visualize(Graphics graphics)
        {
            Brush tempBrush = new SolidBrush(Color.FromArgb(alpha, Color.Green));
            float tempX = this.location.X - Radius;
            float tempY = this.location.Y - Radius;
            graphics.FillEllipse(tempBrush, tempX, tempY, Radius * 2, Radius * 2);
            this.drawCore(graphics, true, true);

            if (hitBySelected)
            {
                graphics.DrawEllipse(outterPen, tempX, tempY, Radius * 2, Radius * 2);
            }

            if (useHeatmaps)
            {
                this.visualizeHeatmap(graphics);
            }
            else
            {
                visualizeChildrenAtLocation(graphics, this.location);
            }
        }

        public void visualizeChildrenAtLocation(Graphics graphics, Point location)
        {
            foreach (MemberViz m in members)
                m.visualize(graphics, this.location, memberBrush);
        }

        private void visualizeHeatmap(Graphics graphics)
        {
            float percentStep = (int)(100.0f / Math.Pow(2.0d, (double)heatmapRecursionDepth - 1.0d));
            float percent = 0;

            //instantiating these once for simplicity's sake
            SolidBrush brush = new SolidBrush(determineColor(heatmapBins[0]));
            Pen pen = new Pen(brush, percentStep);
            float tempX = 0, tempY = 0;
            int currentRadius = 0;

            //need to do all but the last one
            for (int i = 0; i < heatmapBins.Length - 1; i++)
            {
                percent += percentStep;
                brush.Color = determineColor(heatmapBins[i]);  //I want a custom alpha color based upon the intensity of items in the bin
                pen = new Pen(brush, percentStep);  //and now we need a new pen because apparently pens don't update well
                currentRadius = (int)(((float)Radius * (percent / 100.0f)) - percentStep / 2.0f) + 1;
                tempX = location.X - currentRadius;
                tempY = location.Y - currentRadius;
                graphics.DrawEllipse(pen, tempX, tempY, currentRadius * 2, currentRadius * 2);
            }

            //now we do the last chunk of percentages, we need to do it this way in case the step is kind of odd (want to make sure we capture the 100%ers)
            int index = heatmapBins.Length - 1;
            float percentRemainder = 100.0f - percent;

            brush.Color = determineColor(heatmapBins[heatmapBins.Length - 1]);
            pen = new Pen(brush, percentStep);
            currentRadius = Radius - (int)(percentRemainder / 2.0f) + 1;
            tempX = location.X - currentRadius;
            tempY = location.Y - currentRadius;
            graphics.DrawEllipse(pen, tempX, tempY, currentRadius * 2, currentRadius * 2);
        }

        public void drawCore(Graphics graph, bool textOnRight, bool textBelowRadius)
        {
            float smallRadius = ((float)Radius * CORE_SCALE);
            string setLabel = this.set.label;

            graph.FillEllipse(thisBrush, this.location.X - smallRadius, this.location.Y - smallRadius, smallRadius * 2.0f, smallRadius * 2.0f);

            //now we figure out where to put the label
            int textXLocation, textYLocation = this.location.Y;

            if (textOnRight)
                textXLocation = (int)(this.location.X + smallRadius * 2);
            else
                textXLocation = (int)(this.location.X - graph.MeasureString(setLabel, thisFont).Width - smallRadius);

            if (textBelowRadius)
                textYLocation += Radius;

            graph.DrawString(setLabel, thisFont, thisBrush, new Point(textXLocation, textYLocation));
        }

        private Color determineColor(int currentAmount)
        {
            //this is for straight gradiant alphas
            int alpha = (int)((float)currentAmount / (float)globalMaxAmount * 255);

            //this is for a logarithmic scale
            //int alpha = (int)(Math.Log((double)members.Count, (double)maxMemberNum) * 255);

            return Color.FromArgb(alpha, Color.Black);
        }
        #endregion

        public bool isCoreHit(int newX, int newY)
        {
            double xDifference = newX - location.X;
            double yDifference = newY - location.Y;
            double hitRadius = Math.Sqrt(xDifference * xDifference + yDifference * yDifference);

            if (hitRadius < (this.Radius * CORE_SCALE))
                return true;

            return false;
        }

        public void arrange()
        {
            foreach (MemberViz memObj in members)
                arrangeMember(memObj);

            if (useHeatmaps)
                this.setupHeatmaps();
        }

        /*
         * arranges the members in the set.  
         * 
         * The angle around the center is random but the 
        */
        private void arrangeMember(MemberViz memObj)
        {
            double membershipRate = ((double)memObj.member.getMembershipAsPercent(set) / 100D);
            double memberRadius = membershipRate * (double)Radius;
            double angle = randGenerator.NextDouble() * (2D * Math.PI);

            int xOffset = (int)((double)memberRadius * Math.Sin(angle));
            int yOffset = (int)((double)memberRadius * Math.Cos(angle));

            memObj.move(0, 0);
            memObj.move(xOffset, yOffset);

        }

        #region moves
        //moves
        // start with the most visible
        public override void move(Point newPoint)
        {
            this.move(newPoint.X, newPoint.Y);
        }

        //and now the guts
        public void move(int newX, int newY)
        {
            this.location.X = newX;
            this.location.Y = newY;
        }

        //and now so it doesn't snap the middle to the cursor
        public override void moveByDiff(int xDiff, int yDiff)
        {
            this.location.X = this.location.X + xDiff;
            this.location.Y = this.location.Y + yDiff;
        }
        #endregion

        #region basic getters and setters

        public override string ToString()
        {
            return set.label;
        }

        public List<MemberViz> getMemberObjs()
        {
            return members;
        }
        
        public void addMemberObject(MemberViz newObj)
        {
            this.members.Add(newObj);

            arrangeMember(newObj);
        }

        internal void setMemberRadius(int newRadius)
        {
            foreach (MemberViz memObj in members)
                memObj.Radius = newRadius;
        }

        internal int getMemberRadius()
        {
            //I can do this because I know that all of the members are uniform. 
            if (members.Count != 0)
                return members[0].Radius;
            else return 0;
        }

        public override void setScale(float newScale)
        {
            this.scale = newScale;
            foreach (MemberViz memObj in members)
            {
                memObj.setScale(newScale);
            }
        }

        internal void setMemberAlpha(int newAlpha)
        {
            this.memberAlpha = newAlpha;
            this.memberColor = Color.FromArgb(newAlpha, memberColor.R, memberColor.G, memberColor.B);
            this.memberBrush = new SolidBrush(memberColor);
        }

        public void setHeatmaps(bool useHeatmaps)
        {
            this.useHeatmaps = useHeatmaps;

            if (useHeatmaps)
                this.setupHeatmaps();
        }

        #endregion

        #region heatmapstuff

        /**
         * 
         * We're going to try setting up the heatmaps using a non-recursive method. 
         * 
         */
        private void setupHeatmaps()
        {
            //setup the size of the bins
            double numBins = Math.Pow(2.0d, (double)heatmapRecursionDepth - 1.0d);

            heatmapBins = new int[(int)numBins];

            //remember to initialize!
            for (int i = 0; i < (int)numBins; i++)
            {
                heatmapBins[i] = 0;
            }

            double percentStep = (100.0d / numBins);

            //this is where we actually end up doing more work than the triangle/recursive method
            foreach (MemberViz member in members)
            {
                int memberIndex = (int)Math.Floor((double)(100 - member.member.getMembershipAsPercent(this.set)) / (double)percentStep);
                if (memberIndex == heatmapBins.Length)  // only happens in the case where the membership is 0
                    memberIndex--;
                heatmapBins[memberIndex]++;
            }

            int newNumMaxMembers = 0;
            for (int i = 0; i < (int)numBins; i++)
            {
                if (newNumMaxMembers < heatmapBins[i])
                    newNumMaxMembers = heatmapBins[i];
            }

            this.globalMaxAmount = newNumMaxMembers;
        }

        internal bool setHeatmapRecursionDepth(int newRecursionDepth)
        {
            //quick and dirty to make sure it works.
            this.heatmapRecursionDepth = newRecursionDepth;
            if (useHeatmaps)
            {
                this.setupHeatmaps();
            }

            return true;
        }

        #endregion

    }
}
