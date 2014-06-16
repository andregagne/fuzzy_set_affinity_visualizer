using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FuzzySetDynamicVisualizer.VizObjects
{
    public abstract class VizObject : Object
    {
        protected Point location;  
        //protected Color colour;
        public static int MINRADIUS = 2;
        protected int radius;  //is the radius
        protected bool hitBySelected = false;
        protected float scale = 1.0f;

        public VizObject()
        {
            this.location = new Point(0, 0);
            this.radius = MINRADIUS;
            //this.colour = Color.Black;
        }

        public VizObject(int radius) : this()
        {
            this.radius = radius;
        }

        //So we can have zooming
        public VizObject(int screenWidth, int screenHeight, float scale) : this()  
        {
            int smallestDimension = 0;
            if (screenWidth > screenHeight)
                smallestDimension = screenHeight;
            else
                smallestDimension = screenWidth;
            this.radius = (int)((float)smallestDimension * scale);
            if (this.radius < MINRADIUS)
                this.radius = MINRADIUS;
        }

        public abstract void visualize(Graphics graphics);

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

        public virtual VizObject objectHit(Point point)
        {
            double xDifference = point.X - location.X;
            double yDifference = point.Y - location.Y;
            double hitRadius = Math.Sqrt(xDifference * xDifference + yDifference * yDifference);

            if (hitRadius < this.radius)
                return this;

            return null;
        }

        public virtual bool isHitByObject(VizObject outsideObject)
        {
            double xDifference = (outsideObject.getLocation().X - location.X) / scale;
            double yDifference = (outsideObject.getLocation().Y - location.Y) / scale;

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
        
        public void setRadius(int newRadius)
        {
            if (newRadius >= MINRADIUS)
                this.radius = newRadius;
            else
                this.radius = MINRADIUS;
        }

        public int getRadius()
        {
            return radius;
        }

        public Point getLocation()
        {
            return location;
        }

        public virtual void setScale(float newScale)
        {
            this.scale = newScale;
        }
    }
}
