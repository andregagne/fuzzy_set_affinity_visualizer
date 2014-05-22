using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer.VizObjects
{
    public class SetGroupObject : VizObject
    {
        private List<SetObject> setObjects = new List<SetObject>();
        private Dictionary<Set, SetObject> setStructures = new Dictionary<Set, SetObject>();

        //variables for heatmaps
        private List<HeatmapTriangleTree> heatmapObjects = new List<HeatmapTriangleTree>();
        private int heatmapRecursionDepth = 1;         
        private bool useHeatmap = false;

        public SetGroupObject()
        {
            location = new Point(0, 0);
            this.colour = Color.LightYellow;
        }

        public SetGroupObject(List<SetObject> newSets, Color color)
        {
            setObjects.AddRange(newSets);
            this.location = this.calculateCenterPoint();
            this.radius = newSets[0].getRadius() * 2;
            foreach (SetObject setObj in newSets)
                setStructures.Add(setObj.getSet(), setObj);
            this.arrangeObjects();
            this.colour = Color.FromArgb(50, color);
        }

        public SetGroupObject(List<SetObject> newSets, Color color, bool heatmapSwitch)
        {
            this.useHeatmap = heatmapSwitch; setObjects.AddRange(newSets);
            this.location = this.calculateCenterPoint();
            this.radius = newSets[0].getRadius() * 2;
            foreach (SetObject setObj in newSets)
                setStructures.Add(setObj.getSet(), setObj);
            this.arrangeObjects();
            this.colour = Color.FromArgb(50, color);
        }

        #region visualizations
        public override void visualize(Graphics graphics)
        {
            List<Point> points = new List<Point>();
            Pen linePen = new Pen(Color.FromArgb(150, 255, 0, 0));

            //draw the circle for this set group
            graphics.FillEllipse(new SolidBrush(colour), this.location.X - this.radius, this.location.Y - this.radius, radius * 2, radius * 2);

            if (useHeatmap && setObjects.Count >= 3) //don't want to have triangles on a straight line
            {
                foreach (HeatmapTriangleTree triangleTree in heatmapObjects)
                {
                    this.visualizeHeatmapTriangle(graphics, triangleTree);
                }
            }

            foreach (SetObject setObj in setObjects)
            {
                //draws the center point for the sets
                if (setObj.getLocation().X < location.X)
                    setObj.drawCore(graphics, false);
                else
                    setObj.drawCore(graphics, true);

                if (!useHeatmap | setObjects.Count < 3)
                {
                    foreach (MemberObject mObj in setObj.getMemberObjs())
                        mObj.visualize(graphics);
                }                

                points.Add(setObj.getLocation());
                graphics.DrawLine(linePen, this.location, setObj.getLocation());
            }
            graphics.DrawPolygon(linePen, points.ToArray());
        }

        /*
         * visualizeHeatmapTriangle
         * Because we are containing the objects in the tree we can do it recursively
         */
        private void visualizeHeatmapTriangle(Graphics graphics, HeatmapTriangleTree tree)
        {
            if (tree.isLeaf())
            {
                tree.getData().visualize(graphics);
            }
            else
            {
                foreach (HeatmapTriangleTree child in tree.getChildren())
                {
                    this.visualizeHeatmapTriangle(graphics, child);
                }
            }
        }
        
        #endregion

        #region moves
        
        // Regions for moving the object

        public override void move(Point newPoint)
        {
            this.move(newPoint.X, newPoint.Y);
        }

        public void move(int newX, int newY)
        {
            int xDiff = newX - this.location.X;
            int yDiff = newY - this.location.Y;

            this.moveByDiff(xDiff, yDiff);
        }

        public override void moveByDiff(int xDiff, int yDiff)
        {
            this.location.X = this.location.X + xDiff;
            this.location.Y = this.location.Y + yDiff;
            foreach (SetObject set in setObjects)
                set.moveByDiff(xDiff, yDiff);
            if (useHeatmap)
            {
                foreach (HeatmapTriangleTree triangle in heatmapObjects)
                    triangle.moveByDiff(xDiff, yDiff);
            }
        }
        #endregion

        public void addSetObj(SetObject newSetObj)
        {
            if (!setObjects.Contains(newSetObj))
            {
                this.setObjects.Add(newSetObj);
                setStructures.Add(newSetObj.getSet(), newSetObj);
                this.location = this.calculateCenterPoint();
                this.arrangeObjects();
            }
        }

        public void removeSetObj(SetObject removeObj)
        {
            setObjects.Remove(removeObj);
            setStructures.Remove(removeObj.getSet());
            this.location = this.calculateCenterPoint();
            if (setObjects.Count > 1)
            {
                this.arrangeObjects();
            }
        }

        public Point calculateCenterPoint()
        {
            int newX = 0;
            int newY = 0;

            foreach (SetObject set in setObjects)
            {
                newX += set.getLocation().X;
                newY += set.getLocation().Y;
            }

            newX = newX / setObjects.Count;
            newY = newY / setObjects.Count;

            return new Point(newX, newY);
        }

        #region arranging objects

        /**
         * need to do several things:
         * pre 1) figure out locations of the sets
         * 1) draw the core circles of the different sets
         * 2) draw the different member objects
         *    each member needs to be influenced by the different sets based upon its membership amounts
         */
        public void arrangeObjects()
        {
            int numMembers = 0;
            foreach (SetObject setObj in setObjects)
            {
                numMembers += setObj.getMemberObjs().Count;
            }

            double startAngle = (2D * Math.PI) / (double)this.setObjects.Count;
            double angle = 0;

            foreach (SetObject setObj in setObjects)
            {
                int newX = (int)((double)radius * Math.Sin(angle));
                int newY = (int)((double)radius * Math.Cos(angle));

                setObj.move(this.getLocation().X + newX, this.getLocation().Y + newY);
                angle += startAngle;
            }

            setupOverdraw();

            if (useHeatmap && setObjects.Count >= 3)
            {
                setupHeatmap();
            }

        }

        //draws each memberValue separately
        private void setupOverdraw()
        {
            //need to figure out where the sets will be located before we can setup the forces on the members
            foreach (SetObject setObj in setObjects)
            {
                foreach (MemberObject mObj in setObj.getMemberObjs())
                {
                    int memberX = this.location.X;
                    int memberY = this.location.Y;
                    Member tempMember = mObj.getMember();

                    foreach (SetObject setStuff in setObjects)
                    {
                        float membership = ((float)tempMember.getMembershipAsPercent(setStuff.getSet()) / 100.0f);
                        int xDiff = (int)((float)(setStuff.getLocation().X - this.location.X) * membership);
                        int yDiff = (int)((float)(setStuff.getLocation().Y - this.location.Y) * membership);
                        memberX += xDiff;
                        memberY += yDiff;
                    }
                    mObj.move(memberX, memberY);
                }
            }
        }

        #endregion

        #region Heatmaps!

        /*
         * 
         * There are several ways to setup the heatmap.  
         * What I'm going to do is break up each triangle into 4 sub triangles and then use the graphical location for each to bin the members.
         * Not the prettiest method, but it will work in a vast majority of situations and may be faster than calculating N dimensional barycentric coordinates.
         * 
         * Ok, so what I need to have here are a few methods:
         * 
         * setupHeatmap
         * sets up the different tree structures that will contain the triangles needed for visualization and populate them with the appropriate triangle objects
         * 
         * adjustToDepth (int depth)
         * This will force the tree structure to the appropriate depth.  There are several things that need to be taken into account
         *  1) splitting
         *      if the current depth is shallower than the desired then we need to split down to it.
         *  2) collapsing
         *      if the current depth is deeper than desired we will need collapse it back up, this will require grabbing all of the member objects from 
         *      the triangles below the desired depth and adding them to a triangle that is higher up.
         *  3) maximum members within a given layer of the tree
         *      for any given level of the tree, the heatmap visualization requires that you distribute the maximum value across all of the triangles.  
         *      We can save time when collapsing by saving those maximums, but that requires not using a recursive function to split them.
         * 
         */
        private void setupHeatmap()
        {
            heatmapObjects.Clear();

            List<MemberObject> members = new List<MemberObject>();
            List<Point> setLocations = new List<Point>();
            List<HeatmapTriangleObject> vizTriangles = new List<HeatmapTriangleObject>();

            //initial setup we'll always have to do
            //pull what we want from the sets
            foreach (SetObject set in setObjects)
            {
                members.AddRange(set.getMemberObjs());
                setLocations.Add(set.getLocation());
            }


            //I know this can be done more concisely but I feel this is cleaner

            //adding the initial heatmap triangles
            for (int i = 0; i < setLocations.Count; i++)
            {
                if (i != setLocations.Count - 1)
                    vizTriangles.Add(new HeatmapTriangleObject(members.Count, new Point[] { this.getLocation(), setLocations[i], setLocations[i+1] }));
                else
                    vizTriangles.Add(new HeatmapTriangleObject(members.Count, new Point[] { this.getLocation(), setLocations[i], setLocations[0] }));

            }

            foreach (MemberObject member in members)
            {
                bool isAdded = false;
                foreach (HeatmapTriangleObject triangle in vizTriangles)
                {
                    if (triangle.isPointInside(member.getLocation()))
                    {
                        triangle.addMember(member);
                        isAdded = true;
                        break;
                    }
                }
                if (!isAdded)
                    Console.Out.WriteLine("Member wasn't added to a triangle " + member.getLocation().X + ", " + member.getLocation().Y);
            }


            foreach (HeatmapTriangleObject triangle in vizTriangles)
            {
                heatmapObjects.Add(new HeatmapTriangleTree(triangle, null, triangle.getPoints()));
            }

            //now we find the global maximum number of members and distribute it out
            int newNumMaxMembers = 0;
            foreach (HeatmapTriangleObject triangle in vizTriangles)
            {
                if (newNumMaxMembers < triangle.getMembers().Count)
                    newNumMaxMembers = triangle.getMembers().Count;
            }
            foreach (HeatmapTriangleTree triangle in heatmapObjects)
            {
                triangle.setLeafMaxMembers(newNumMaxMembers);
            }

            //and now we split the trees
            this.doHeatmapRecursion(heatmapRecursionDepth);
        }
        

        /*
         *
         * splitToDepth
         * Will not use standard recursion (despite wanting to) as we can save computation by doing each level of tree at a time.
         * 
         * We will need to first determine the depth of the triangles.
         * 
         * We'll use a list to keept track of the triangles we should be working on.  Because everything is symetric we should be able to use the 
         * first item in each list instead of checking each one.
         * 
         */
        private void doHeatmapRecursion(int maxDepth)
        {
            if (heatmapObjects[0].getDepth() != maxDepth)
            {
                if (heatmapObjects[0].getDepth() > maxDepth) // The tree is deeper than we want, collapse it
                {
                    while (heatmapObjects[0].getDepth() > maxDepth)
                    {
                        foreach (HeatmapTriangleTree tree in heatmapObjects)
                        {
                            foreach (HeatmapTriangleTree leaf in tree.getLeavesAtDepth(tree.getDepth() - 1))
                            {
                                leaf.collapseChildren();
                            }
                        }
                    }
                }
                else  // the heart of the splitting
                {
                    while (heatmapObjects[0].getDepth() < maxDepth)
                    {
                        List<HeatmapTriangleObject> newLeaves = new List<HeatmapTriangleObject>();
                        foreach (HeatmapTriangleTree tree in heatmapObjects)
                        {
                            foreach (HeatmapTriangleTree leaf in tree.getLeaves())  //split and get the subtraingles.
                            {
                                newLeaves.AddRange(leaf.splitChildren());
                            }
                        }

                        //now we find the global maximum number of members and distribute it out
                        int newNumMaxMembers = 0;
                        foreach (HeatmapTriangleObject triangle in newLeaves)
                        {
                            if (newNumMaxMembers < triangle.getMembers().Count)
                                newNumMaxMembers = triangle.getMembers().Count;
                        }
                        foreach (HeatmapTriangleTree triangle in heatmapObjects)
                        {
                            triangle.setLeafMaxMembers(newNumMaxMembers);
                        }
                    }

                    return;
                }
            }
        }
      
        #endregion

        #region overrides
        public override void setIsHitBySelected(bool isHit)
        {
            this.hitBySelected = isHit;
        }

        /* public override bool isHit(Point mousePoint)
                {
                    foreach (SetObject set in setObjects)
                        if (set.isHit(mousePoint))
                            return true;

                    return false;
                }
         */

        public override VizObject objectHit(Point point)
        {
            foreach (SetObject set in setObjects)
                if (set.isCoreHit(point.X, point.Y))
                    return set;

            if (this.isHit(point))
                return this;

            return null;
        }

        /*public override bool isHitByObject(VizObject outsideObject)
        {

            foreach (SetObject set in setObjects)
                if (set.isHitByObject(outsideObject))
                    return true;

            return false;
        }
        */
        public override void setScale(float newScale)
        {
            this.scale = newScale;
            foreach (SetObject set in setObjects)
                set.setScale(newScale);
        }
        #endregion

        #region getters and setters
        internal void setMemberRadius(int newRadius)
        {
            foreach (SetObject setObj in setObjects)
                setObj.setMemberRadius(newRadius);
        }

        internal int getMemberRadius()
        {
            if (setObjects.Count != 0)
                return setObjects[0].getMemberRadius();
            else return 0;
        }

        internal void setMemberAlpha(int newAlpha)
        {
            foreach (SetObject set in setObjects)
                set.setMemberAlpha(newAlpha);
        }

        public void setUseHeatmap(bool newUseHeatmap)
        {
            useHeatmap = newUseHeatmap;
            this.arrangeObjects();
        }

        public bool containsSetObject(SetObject set)
        {
            return setObjects.Contains(set);
        }

        public int numContainedSets()
        {
            return setObjects.Count;
        }

        internal List<SetObject> getSets()
        {
            return setObjects;
        }

        internal bool setHeatmapRecursionDepth(int newRecursionDepth)
        {
            this.heatmapRecursionDepth = newRecursionDepth;
            if (useHeatmap)
            {
                if (heatmapObjects.Count > 0) //we have heatmap objects
                {
                    if (newRecursionDepth > 0)
                    {
                        this.doHeatmapRecursion(newRecursionDepth);
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
