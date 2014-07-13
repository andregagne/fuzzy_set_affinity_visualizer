using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FuzzySetDynamicVisualizer.VizObjects
{
    public abstract class VizAttachedObject : Object
    {
        public Point location;
        public const int MIN_RADIUS = 2;
        private int radius;
        public int Radius  //is the radius
        {
            get { return radius; }
            set { radius = Math.Max(value, MIN_RADIUS); }
        }
        protected bool hitBySelected = false;
        protected float scale;

        public VizAttachedObject()
        {
            this.location = new Point(0, 0);
            this.Radius = MIN_RADIUS;
            //this.colour = Color.Black;
            this.scale = 1.0f;
        }

        public VizAttachedObject(int radius)
            : this()
        {
            this.Radius = radius;
        }

        //So we can have zooming
        public VizAttachedObject(int screenWidth, int screenHeight, float scale)
            : this()
        {
            int smallestDimension = 0;
            if (screenWidth > screenHeight)
                smallestDimension = screenHeight;
            else
                smallestDimension = screenWidth;
            Radius = (int)((float)smallestDimension * scale);
        }

        //this acknowledges that the member object is a special object that will always be a part of another visual object
        // as a result we never need to keep the location updated.
        public abstract void visualize(Graphics graphics, Point parentPoint);

        public abstract void move(Point newPoint);
        public abstract void moveByDiff(int xDiff, int yDiff);

        public virtual bool isHit(Point point)
        {
            double xDifference = point.X - location.X;
            double yDifference = point.Y - location.Y;
            double hitRadius = Math.Sqrt(xDifference * xDifference + yDifference * yDifference);

            if (hitRadius < this.radius)
                return true;

            return false;
        }

        public virtual VizAttachedObject objectHit(Point point)
        {
            double xDifference = point.X - location.X;
            double yDifference = point.Y - location.Y;
            double hitRadius = Math.Sqrt(xDifference * xDifference + yDifference * yDifference);

            if (hitRadius < this.radius)
                return this;

            return null;
        }

        public virtual bool isHitByObject(VizAttachedObject outsideObject)
        {
            double xDifference = (outsideObject.location.X - location.X) / scale;
            double yDifference = (outsideObject.location.Y - location.Y) / scale;

            if (Math.Sqrt(xDifference * xDifference + yDifference * yDifference) < (this.radius + outsideObject.radius))
                return true;

            return false;
        }

        public bool getIsHitBySelected()
        {
            return hitBySelected;
        }

        public virtual void setIsHitBySelected(bool isHit)
        {
            this.hitBySelected = isHit;
        }

        public virtual void setScale(float newScale)
        {
            this.scale = newScale;
        }
    }
}
