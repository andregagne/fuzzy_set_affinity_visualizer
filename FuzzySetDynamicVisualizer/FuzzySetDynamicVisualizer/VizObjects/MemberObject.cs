﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer.VizObjects
{
    public class MemberObject : VizObject
    {
        private Member member;
        
        public MemberObject(Member member) : base()
        {            
            this.member = member;

            this.radius = 5;
        }

        public MemberObject(Member member, int screenWidth, int screenHeight): base(screenWidth, screenHeight, 0.01f)
        {
            this.member = member;
        }


        public Member getMember()
        {
            return member;
        }

        public override void visualize(Graphics graphics)
        {
            graphics.FillEllipse(new SolidBrush(Color.Black), this.location.X - radius, this.location.Y - radius, radius * 2, radius * 2);            
        }

        //this acknowledges that the member object is a special object that will always be a part of another visual object
        // as a result we never need to keep it updated.
        public void visualize(Graphics graphics, Point parentPoint, Brush brush)
        {
            graphics.FillEllipse(brush, parentPoint.X + this.location.X - radius, parentPoint.Y + this.location.Y - radius, radius * 2, radius * 2);            
        }        

        public override string ToString()
        {
            return member.getLabel();
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
