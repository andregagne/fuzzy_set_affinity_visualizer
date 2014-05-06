using System;
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
        private static int alpha = 30;
        private Brush thisBrush;
        
        public MemberObject(Member member, Color color) : base()
        {            
            this.member = member;
            this.colour = Color.FromArgb(alpha, color.R, color.G, color.B);
            this.thisPen = new Pen(colour);
            thisBrush = new SolidBrush(colour);

            this.radius = 5;
        }

        public MemberObject(Member member, Color color, int screenWidth, int screenHeight): base(screenWidth, screenHeight, 0.01f)
        {
            this.member = member;
            this.colour = Color.FromArgb(alpha, color.R, color.G, color.B);
            this.thisPen = new Pen(colour);
            thisBrush = new SolidBrush(colour);
        }


        public Member getMember()
        {
            return member;
        }

        public override void visualize(Graphics graphics)
        {
            graphics.FillEllipse(thisBrush, this.location.X - radius, this.location.Y - radius, radius * 2, radius * 2);            
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

        internal void setAlpha(int newAlpha)
        {
            this.colour = Color.FromArgb(newAlpha, colour.R, colour.G, colour.B);
            alpha = newAlpha;
            this.thisBrush = new SolidBrush(colour);
            this.thisPen = new Pen(colour);
        }
    }
}
