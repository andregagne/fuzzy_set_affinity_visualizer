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
    class HeatmapTriangleTree: VizObject
    {
        private HeatmapTriangleTree parentNode = null;        
        private List<HeatmapTriangleTree> childrenNodes = new List<HeatmapTriangleTree>();
        protected List<MemberObject> members;
        protected Point[] points = new Point[3];
        protected int numMaxMembers = 1; //needed to determine alpha/shading levels

        public HeatmapTriangleTree (int numMaxMembers, HeatmapTriangleTree parent, Point[] starterPoints) : base()
        {
            this.numMaxMembers = numMaxMembers;
            this.members = new List<MemberObject>();
            this.points = starterPoints;
            this.parentNode = parent;
        }

        public HeatmapTriangleTree(List<MemberObject> members, int maxMemberNum, HeatmapTriangleTree parent, Point[] points)
            : base()
        {
            this.points = points;
            this.members = members;
            this.numMaxMembers = maxMemberNum;
            this.parentNode = parent;
        }

        public HeatmapTriangleTree(List<MemberObject> members, int maxMemberNum, HeatmapTriangleTree parent, Point point1, Point point2, Point point3)
            : base()
        {
            this.members = members;
            this.numMaxMembers = maxMemberNum;
            this.points[0] = point1;
            this.points[1] = point2;
            this.points[2] = point3;
            this.parentNode = parent;
        }

        #region VizObject stuff

        public override void visualize(Graphics graphics)
        {
            if(this.isLeaf()){
                SolidBrush brush = new SolidBrush(determineColor());
                graphics.FillPolygon(brush, points);
            } else {
                foreach (HeatmapTriangleTree triangle in childrenNodes){
                    triangle.visualize(graphics);
                }
            }
        }

        public override void move(Point newPoint)
        {
            this.move(newPoint.X, newPoint.Y);
        }

        public void move(int newX, int newY)
        {
            this.location.X = newX;
            this.location.Y = newY;
            if (this.isLeaf())
            {
                foreach (MemberObject member in members)
                {
                    member.move(newX, newY);
                }
            }
            else
            {
                foreach (HeatmapTriangleTree triangle in childrenNodes)
                {
                    triangle.move(newX, newY);
                }
            }
        }

        public override void moveByDiff(int xDiff, int yDiff)
        {
            if (this.isLeaf())
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].X += xDiff;
                    points[i].Y += yDiff;
                }

                //housekeeping
                foreach (MemberObject member in members)
                {
                    member.moveByDiff(xDiff, yDiff);
                }
            }
            else
            {
                foreach (HeatmapTriangleTree triangle in childrenNodes)
                {
                    triangle.moveByDiff(xDiff, yDiff);
                }
            }
        }

        private Color determineColor()
        {
            //this is for straight gradiant alphas
            int alpha = (int)((float)members.Count / (float)numMaxMembers * 255);

            //this is for a logarithmic scale
            //int alpha = (int)(Math.Log((double)members.Count, (double)maxMemberNum) * 255);

            return Color.FromArgb(alpha, Color.Black);
        }

        //tests to see if the given point is inside the triangle.
        public bool isPointInside(Point testPoint)
        {
            bool isInside = false;

            //will use the barycentric coordinates version
            //initial vectors
            PointF vector1 = new PointF((float)points[1].X - (float)points[0].X, (float)points[1].Y - (float)points[0].Y);
            PointF vector2 = new PointF((float)points[2].X - (float)points[0].X, (float)points[2].Y - (float)points[0].Y);
            PointF vector3 = new PointF((float)testPoint.X - (float)points[0].X, (float)testPoint.Y - (float)points[0].Y);

            //and now the dot products of the different vectors
            float dot11 = vector1.X * vector1.X + vector1.Y * vector1.Y;
            float dot12 = vector1.X * vector2.X + vector1.Y * vector2.Y;
            float dot13 = vector1.X * vector3.X + vector1.Y * vector3.Y;
            float dot22 = vector2.X * vector2.X + vector2.Y * vector2.Y;
            float dot23 = vector2.X * vector3.X + vector2.Y * vector3.Y;

            //and now to determin U and V
            //calculate the denomenator only once
            float denomenator = 1.0f / (dot11 * dot22 - dot12 * dot12);
            //calculate u and v
            float u = (dot22 * dot13 - dot12 * dot23) * denomenator;
            float v = (dot11 * dot23 - dot12 * dot13) * denomenator;

            // u and v will only be positive if the point is between them on the smallest side, will only sum to 1 if it is within the triangles
            //normally this wouldn't allow a point if u and v summed to 1 but I am going to allow it as it means that the point is on the edge of the triangle.
            isInside = (u >= 0) && (v >= 0) && (u + v <= 1);
            return isInside;
        }
        #endregion

        #region recursive functions

        //standard recursion
        public void splitToDepth(int currentDepth, int maxDepth)
        {
            if (currentDepth < maxDepth)
            { //need to dig deeper
                if (this.isLeaf())
                { //need to split this one down
                    this.splitChildren();
                }
                foreach (HeatmapTriangleTree triangle in childrenNodes)
                { // and now for the recursion
                    triangle.splitToDepth(currentDepth + 1, maxDepth);
                }
            }
            else if (currentDepth == maxDepth)//we're at the end of the recursion.
            {
                if (!this.isLeaf())
                {
                    Console.Out.WriteLine("Should never have gotten here, we're trying to split a triangle to a depth that is shallower than it already is");
                }
            }
            else
            {
                Console.Out.WriteLine("Should never have gotten here, we're at a deeper depth than we wanted to be");
            }
        }

        //standard recursion
        public void collapseToDepth(int currentDepth, int maxDepth)
        {
            if (!this.isLeaf()){ // not a leaf so we can collapse or recurse further
                if(currentDepth >= maxDepth){ //if we're below or at the desired depth we should collapse
                    if (!this.childrenNodes[0].isParent()) //see if we've hit the bottom and need to start collapsing
                    {
                        this.collapseChildren();
                    }
                    else
                    { // go deeper
                        foreach (HeatmapTriangleTree triangle in childrenNodes)
                        {
                            triangle.collapseToDepth(currentDepth + 1, maxDepth);
                        }
                        this.collapseChildren(); //we know we can collapse because we've collapse everything below us.
                    }
                } else { // just need to dig deeper
                    foreach (HeatmapTriangleTree triangle in childrenNodes)
                    {
                        triangle.collapseToDepth(currentDepth + 1, maxDepth);
                    }
                }
            }
            else 
                Console.Out.WriteLine("Should never have gotten here, we're trying to collapse a leaf node");
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

        //simple check for depths
        public int getMaxDepth(int currentDepth)
        {
            if (this.isLeaf())
            {
                return currentDepth;
            }
            else
            {
                return childrenNodes[0].getMaxDepth(currentDepth + 1);
            }
        }

        #endregion 

        #region direct children editors

        //collapses the children of this node into a single triangle object
        public void collapseChildren()
        {            
            foreach (HeatmapTriangleTree childNode in childrenNodes)
            {
                this.addMembers(childNode.getMembers());
            }

            childrenNodes.Clear();
        }

        //splits the children into the 4 smaller triangles that can make up this one.
        public void splitChildren()
        {
            //set up the medial points
            Point medialPoint1 = new Point(
                (int)(((float)points[0].X + (float)points[1].X) / 2.0f),
                (int)(((float)points[0].Y + (float)points[1].Y) / 2.0f)
                );
            Point medialPoint2 = new Point(
                (int)(((float)points[1].X + (float)points[2].X) / 2.0f),
                (int)(((float)points[1].Y + (float)points[2].Y) / 2.0f)
                );
            Point medialPoint3 = new Point(
                (int)(((float)points[2].X + (float)points[0].X) / 2.0f),
                (int)(((float)points[2].Y + (float)points[0].Y) / 2.0f)
                );

            //create the 4 smaller triangles
            childrenNodes.Add(new HeatmapTriangleTree(numMaxMembers, this, new Point[] { points[0], medialPoint1, medialPoint3 }));
            childrenNodes.Add(new HeatmapTriangleTree(numMaxMembers, this, new Point[] { points[1], medialPoint1, medialPoint2 }));
            childrenNodes.Add(new HeatmapTriangleTree(numMaxMembers, this, new Point[] { points[2], medialPoint2, medialPoint3 }));
            childrenNodes.Add(new HeatmapTriangleTree(numMaxMembers, this, new Point[] { medialPoint1, medialPoint2, medialPoint3 }));


            foreach (MemberObject member in members)
            {
                if (childrenNodes[0].isPointInside(member.getLocation()))
                    childrenNodes[0].addMember(member);
                else if (childrenNodes[1].isPointInside(member.getLocation()))
                    childrenNodes[1].addMember(member);
                else if (childrenNodes[2].isPointInside(member.getLocation()))
                    childrenNodes[2].addMember(member);
                else if (childrenNodes[3].isPointInside(member.getLocation()))
                    childrenNodes[3].addMember(member);
                else
                    Console.Out.WriteLine("Member wasn't added to a triangle " + member.getLocation().X + ", " + member.getLocation().Y);
            }
            this.members.Clear();
        }

        #endregion 

        #region getters and setters

        public bool isLeaf()
        {
            return childrenNodes.Count == 0 ? true : false;
        }

        public bool isParent()
        {
            return parentNode == null ? true : false;
        }

        //gets the data from the children
        public List<HeatmapTriangleTree> getChildren()
        {
            return this.childrenNodes;
        }

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


        #endregion
    }
}
