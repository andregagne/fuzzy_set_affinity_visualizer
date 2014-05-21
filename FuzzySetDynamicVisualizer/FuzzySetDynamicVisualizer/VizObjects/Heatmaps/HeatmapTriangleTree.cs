using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace FuzzySetDynamicVisualizer.VizObjects
{
    /*
     * HeatmapTriangleTree 
     * 
     * The idea behind this is that it will be a tree with all of the data contained at the leaves.  Each node, however, will contain the the coordinates for the triangle that it is contained by.
     * 
     */
    class HeatmapTriangleTree
    {
        private HeatmapTriangleObject data = null;
        private HeatmapTriangleTree parentNode = null;
        private Point[] points;
        private Point middlePoint = new Point(0, 0);
        private List<HeatmapTriangleTree> childrenNodes = new List<HeatmapTriangleTree>();
        private int numMaxMembers = 0;

        public HeatmapTriangleTree(HeatmapTriangleObject starterData, HeatmapTriangleTree parent, Point[] starterPoints)
        {
            this.data = starterData;
            this.points = starterPoints;            
            this.parentNode = parent;

            //now we find the middle point of the points
            int x = 0, y = 0;
            for (int i = 0; i < points.Length; i++)
            {
                x += points[i].X;
                y += points[i].Y;
            }
            x = (int)((float)x / (float)points.Length);
            y = (int)((float)y / (float)points.Length);

            middlePoint.X = x;
            middlePoint.Y = y;
        }

        #region direct children editors

        //gets the data from the children
        public List<HeatmapTriangleObject> getChildrenData()
        {
            List<HeatmapTriangleObject> returnTriangles = new List<HeatmapTriangleObject>();

            foreach (HeatmapTriangleTree childNode in childrenNodes){
                if (childNode.isLeaf())
                    returnTriangles.Add(childNode.getData());              
                else
                    returnTriangles.AddRange(childNode.getChildrenData());                
            }                

            return returnTriangles;            
        }

        //collapses the children of this node into a single triangle object
        public HeatmapTriangleObject collapseChildren()
        {
            HeatmapTriangleObject returnTriangle = new HeatmapTriangleObject(childrenNodes[0].getData().getNumMaxMembers(), points);

            foreach (HeatmapTriangleTree childNode in childrenNodes)
            {
                returnTriangle.addMembers(childNode.getData().getMembers());
            }

            childrenNodes.Clear();

            return returnTriangle;
        }

        public List<HeatmapTriangleObject> splitChildren()
        {
            List<HeatmapTriangleObject> subTriangles = this.data.getSubTriangles();

            foreach (HeatmapTriangleObject triangle in subTriangles)
            {
                this.childrenNodes.Add(new HeatmapTriangleTree(triangle, this, triangle.getPoints()));
            }

            return subTriangles;
        }

        #endregion 

        #region getters and setters

        public List<HeatmapTriangleObject> getTriangles()
        {
            List<HeatmapTriangleObject> returnList = new List<HeatmapTriangleObject>();

            if (this.isLeaf())
            {
                returnList.Add(data);
                return returnList;
            }
            else
            {
                foreach (HeatmapTriangleTree child in childrenNodes)
                {
                    returnList.AddRange(child.getTriangles());
                }

                return returnList;
            }
        }

        public HeatmapTriangleObject getData()
        {
            return data;
        }

        internal List<HeatmapTriangleTree> getChildren()
        {
            return childrenNodes;
        }

        public bool isLeaf()
        {
            return childrenNodes.Count == 0 ? true : false;
        }

        public bool isParent()
        {
            return parentNode == null ? true : false;
        }
        
        internal int getDepth()
        {
            if(this.isLeaf()){
                return 1;
            } else {
                return 1 + childrenNodes[0].getDepth();  //we can do this because we know the tree is evenly balanced
            }
        }

        internal void setNumMaxMembers(int newMax)
        {
            this.numMaxMembers = newMax;           
        }
               
        internal List<HeatmapTriangleTree> getLeaves()
        {
            return this.getLeavesAtDepth(this.getDepth());
        }


        internal List<HeatmapTriangleTree> getLeavesAtDepth(int maxDepth)
        {
            if(maxDepth > this.getDepth()) //want deeper than we have
                return new List<HeatmapTriangleTree>();
            else 
                return this.getLeavesAtDepth(1, maxDepth); //we use 1 for the current depth because a tree of one node returns 1 from getDepth()
        }

        private List<HeatmapTriangleTree> getLeavesAtDepth(int currentDepth, int maxDepth)
        {
            List<HeatmapTriangleTree> returnList = new List<HeatmapTriangleTree>();
            if (currentDepth == maxDepth)
            {                
                returnList.Add(this);
            }
            else {
                foreach (HeatmapTriangleTree child in childrenNodes)
                {
                    returnList.AddRange(child.getLeavesAtDepth(currentDepth + 1, maxDepth));
                }
            }
            return returnList;
        }      

        internal void setLeafMaxMembers(int newNumMaxMembers)
        {
            if (this.isLeaf())
            {
                this.setNumMaxMembers(newNumMaxMembers);
                this.data.setNumMaxMembers(newNumMaxMembers);
            }
            else
            {
                foreach (HeatmapTriangleTree child in childrenNodes)
                {
                    child.setLeafMaxMembers(newNumMaxMembers);
                }
            }

        }

        internal void move(int newX, int newY)
        {
            int xDiff = middlePoint.X - newX;
            int yDiff = middlePoint.Y - newY;
            this.moveByDiff(xDiff, yDiff);
        }

        internal void moveByDiff(int xDiff, int yDiff)
        {
            for(int i = 0; i < points.Length; i++)
            {
                points[i].X += xDiff;
                points[i].Y += yDiff;
            }
            if (this.isLeaf())
            {
                this.data.moveByDiff(xDiff, yDiff);
            }
            else
            {
                foreach (HeatmapTriangleTree child in childrenNodes)
                {
                    child.moveByDiff(xDiff, yDiff);
                }
            }
        }

        #endregion
    }
}
