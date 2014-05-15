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
        private List<HeatmapTriangleTree> childrenNodes = new List<HeatmapTriangleTree>();

        public HeatmapTriangleTree(HeatmapTriangleObject starterData, HeatmapTriangleTree parent, Point[] starterPoints)
        {
            this.data = starterData;
            this.points = starterPoints;
            this.parentNode = parent;
        }

#region recursive functions

        //standard recursion
        public List<HeatmapTriangleObject> splitToDepth(int currentDepth, int maxDepth)
        {
            List<HeatmapTriangleObject> returnList = new List<HeatmapTriangleObject>();
            //we are the leaf node, return our data?
            if (currentDepth < maxDepth)
            {
                List<HeatmapTriangleObject> subTriangles = this.data.getSubTriangles();

                foreach (HeatmapTriangleObject triangle in subTriangles)
                {
                    this.childrenNodes.Add(new HeatmapTriangleTree(triangle, this, triangle.getPoints()));
                }

                returnList.AddRange(this.splitToDepth(currentDepth +1, maxDepth));                
            } else {
                returnList.Add(this.data);
            }
            return returnList;
        }

        public List<HeatmapTriangleTree> getLeaves()
        {
            if (this.isLeaf())
            {
                return childrenNodes;
            }
            else
            {
                List<HeatmapTriangleTree> returnNodes = new List<HeatmapTriangleTree>();

                foreach (HeatmapTriangleTree child in childrenNodes)
                {
                    returnNodes.AddRange(child.getLeaves());
                }

                return returnNodes;
            }
        }

#endregion 

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

        public HeatmapTriangleObject getData()
        {
            return data;
        }

        public bool isLeaf()
        {
            return childrenNodes.Count == 0 ? true : false;
        }

        public bool isParent()
        {
            return parentNode == null ? true : false;
        }
        #endregion
    }
}
