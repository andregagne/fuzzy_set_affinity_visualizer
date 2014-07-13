using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer.VizObjects
{
    public class MemberViz : VizAttachedObject
    {
        public readonly Member member;

        public MemberViz(Member member, Brush brush)
            : base()
        {
            this.member = member;

            this.Radius = 5;
        }

        public MemberViz(Member member, Brush brush, int screenWidth, int screenHeight)
            : base(screenWidth, screenHeight, 0.01f)
        {
            this.member = member;
        }

        public override void visualize(Graphics graphics, Point parentPoint)
        {
            graphics.FillEllipse(new SolidBrush(Color.Black),
                parentPoint.X + this.location.X - Radius, parentPoint.Y + this.location.Y - Radius, Radius * 2, Radius * 2);
        }

        public void visualize(Graphics graphics, Point parentPoint, Brush brush)
        {
            graphics.FillEllipse(brush, parentPoint.X + this.location.X - Radius, parentPoint.Y + this.location.Y - Radius, Radius * 2, Radius * 2);
        }

        public override string ToString()
        {
            return member.label;
        }

        public override void move(Point newPoint)
        {
            this.move(newPoint.X, newPoint.Y);
        }

        public void move(int newX, int newY)
        {
            this.location.X = newX;
            this.location.Y = newY;
        }

        public override void moveByDiff(int xDiff, int yDiff)
        {
            this.location.X = this.location.X + xDiff;
            this.location.Y = this.location.Y + yDiff;
        }

        //This is special in that it sets the offset from the parent, not the actual X and Y location of the visualization object.
        public void setOffset(int xOffset, int yOffset)
        {
            this.location.X = xOffset;
            this.location.Y = yOffset;
        }

        internal Point getOffset()
        {
            return this.location;
        }
    }
}
