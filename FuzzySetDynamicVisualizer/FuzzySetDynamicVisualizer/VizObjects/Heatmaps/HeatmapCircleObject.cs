using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer.VizObjects
{
    public class HeatmapCircleObject : VizObject
    {
        protected List<MemberObject> members;
        int maxPercent = 0, minPercent = 0;
        protected int numMaxMembers = 1; //needed to determine alpha/shading levels
        private int parentRadius = 1;


        public HeatmapCircleObject(int setRadius) : base()
        {
            this.members = new List<MemberObject>();
            this.parentRadius = setRadius;
        }

        public HeatmapCircleObject(int maxPercent, int minPercent, int setRadius) : base()
        {            
            this.members = new List<MemberObject>();
            this.parentRadius = setRadius;
            this.maxPercent = maxPercent;
            this.minPercent = minPercent;
           
        }       

        public override void visualize(Graphics graphics)
        {
            SolidBrush brush = new SolidBrush(determineColor());
            float tempX = this.location.X - parentRadius;
            float tempY = this.location.Y - parentRadius;
            graphics.FillEllipse(brush, tempX, tempY, parentRadius * 2, parentRadius * 2);
        }

        public void visualize(Graphics graphics, Point parentPoint)
        {            
            SolidBrush brush = new SolidBrush(determineColor());
            int maxRadius = (int)((float)parentRadius * (100.0f / (float)this.maxPercent));
            int minRadius = (int)((float)parentRadius * (100.0f / (float)this.minPercent));
            float tempX = parentPoint.X - maxRadius;
            float tempY = parentPoint.Y - maxRadius;
            graphics.FillEllipse(brush, tempX, tempY, maxRadius * 2, maxRadius * 2);
        }

        public override void move(Point newPoint)
        {
            this.move(newPoint.X, newPoint.Y);
        }

        public void move(int newX, int newY)
        {
            moveByDiff(newX - location.X, newY - location.Y);
        }

        public override void moveByDiff(int xDiff, int yDiff)
        {            
            this.location.X += xDiff;
            this.location.Y += yDiff;
        }

        private Color determineColor()
        {
            //this is for straight gradiant alphas
            int alpha = (int)((float)members.Count / (float)numMaxMembers * 255);

            //this is for a logarithmic scale
            //int alpha = (int)(Math.Log((double)members.Count, (double)maxMemberNum) * 255);

            return Color.FromArgb(alpha, Color.Black);
        }

        #region basic getters and setters
        public void addMember(MemberObject newMember)
        {
            this.members.Add(newMember);
        }

        public void addMembers(List<MemberObject> newMembers)
        {
            this.members.AddRange(newMembers);
        }

        public List<MemberObject> getMembers()
        {
            return members;
        }

        public int getNumMaxMembers()
        {
            return numMaxMembers;
        }

        public void setNumMaxMembers(int newNumMaxMembers)
        {
            this.numMaxMembers = newNumMaxMembers;
        }
        #endregion

        //tests to see if the given point is inside the triangle.
        //requires that the points be relative to the 
        public bool isMemberInside(int memberPercent)
        {
            bool isInside = false;
             
            if (memberPercent >= this.minPercent)
                if (memberPercent <= this.maxPercent)
                    isInside = true;

            return isInside;            
        }        
    }
}
