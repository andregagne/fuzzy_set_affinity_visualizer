using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer.VizObjects
{

    public class SetObject : VizObject
    {
        private Set set;
        private Brush thisBrush;
        private int alpha = 50;
        private List<MemberObject> members = new List<MemberObject>();
        private Font thisFont;
        private float coreScale = 0.05f;

        public SetObject(Set set, Color color)
        {
            this.set = set;
            this.colour = color;
            thisBrush = new SolidBrush(color);
            thisPen = new Pen(colour);
            this.radius = 10;
            this.thisFont = new Font("Arial", 8);
        }

        public SetObject(Set set, Color color, int screenWidth, int screenHeight)
            : base(screenWidth, screenHeight, 0.2f)
        {
            this.set = set;
            this.colour = color;
            thisBrush = new SolidBrush(color);
            thisPen = new Pen(colour);
            this.thisFont = new Font("Arial", 8);

            foreach (Member m in set.getMembers())
                this.addMemberObject(new MemberObject(m, Color.Black));
        }

        public override void visualize(Graphics graphics)
        {
            Brush tempBrush = new SolidBrush(Color.FromArgb(alpha, Color.Green));
            float tempX = this.location.X - radius;
            float tempY = this.location.Y - radius;
            graphics.FillEllipse(tempBrush, tempX, tempY, radius * 2, radius * 2);
            this.drawCore(graphics, true);
            
            if (hitBySelected)
            {
                Pen outterPen = new Pen(Color.DarkOrange);
                outterPen.Width += 2;
                graphics.DrawEllipse(outterPen, tempX, tempY, radius * 2, radius * 2);
            }

            foreach (MemberObject m in members)
                m.visualize(graphics, this.location);
        }

        public void drawCore(Graphics graph, bool textOnRight)
        {
            float smallRadius = ((float)radius * coreScale);
            string setLabel = this.getSet().getLabel();

            graph.FillEllipse(thisBrush, this.location.X - smallRadius, this.location.Y - smallRadius, smallRadius * 2.0f, smallRadius * 2.0f);
            if(textOnRight)
                graph.DrawString(setLabel, thisFont, thisBrush, new Point((int)(this.location.X + smallRadius * 2), this.location.Y));
            else
                graph.DrawString(setLabel, thisFont, thisBrush, new Point((int)(this.location.X - graph.MeasureString(setLabel, thisFont).Width - smallRadius), this.location.Y));
        }

        public bool isCoreHit(int newX, int newY)
        {
            double xDifference = newX - location.X;
            double yDifference = newY - location.Y;
            double hitRadius = Math.Sqrt(xDifference * xDifference + yDifference * yDifference);

            if (hitRadius < (this.radius * coreScale))
                return true;

            return false;
        }

        public Set getSet()
        {
            return set;
        }

        public List<MemberObject> getMemberObjs()
        {
            return members;
        }

        public override string ToString()
        {
            return set.getLabel();
        }

        public void addMemberObject(MemberObject newObj)
        {
            this.members.Add(newObj);

            arrangeMember(newObj);
        }

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

        public void arrange()
        {
            foreach(MemberObject memObj in members)
                arrangeMember(memObj);
        }

        /*
         * arranges the members in the set.  
         * 
         * The angel around the center is random but the 
        */
        private void arrangeMember(MemberObject memObj)
        {
            double membershipRate = ((double)memObj.getMember().getMembershipAsPercent(set) / 100D);
            double memberRadius = membershipRate * (double)radius;
            double angle = randGenerator.NextDouble() * (2D * Math.PI);

            int xOffset = (int)((double)memberRadius * Math.Sin(angle));
            int yOffset = (int)((double)memberRadius * Math.Cos(angle));

            memObj.move(0, 0);
            memObj.move(xOffset, yOffset);
            
        }

        internal void setMemberRadius(int newRadius)
        {
            foreach (MemberObject memObj in members)
                memObj.setRadius(newRadius);
        }

        internal int getMemberRadius()
        {
            if (members.Count != 0)
                return members[0].getRadius();
            else return 0;
        }

        public override void setScale(float newScale)
        {
            this.scale = newScale;
            foreach (MemberObject memObj in members)
            {
                memObj.setScale(newScale);
            }
        }

        internal void setMemberAlpha(int newAlpha)
        {
            foreach (MemberObject memObj in members)
            {
                memObj.setAlpha(newAlpha);
            }
        }


    }
}
