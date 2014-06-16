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
        private Color colour;        

        //variables for heatmaps
        private List<HeatmapTriangleTree> heatmapObjects = new List<HeatmapTriangleTree>();
        private int heatmapRecursionDepth = 1;         
        private bool useHeatmap = false;
        private int globalMaxAmount = 1;
        private int[] heatmapBins;
        private static float LINE_HEATMAP_WIDTH = 20.0f;              

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

            if (useHeatmap)
                if(setObjects.Count == 2)
                    this.visualizeLineHeatmap(graphics);
                else  // we know there are 3 or more so we'll use the triangle visualization system
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

                if (!useHeatmap)
                {
                    foreach (MemberObject mObj in setObj.getMemberObjs())
                        mObj.visualize(graphics, this.location, setObj.getMemberBrush());
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
                tree.getData().visualize(graphics, this.location);
            }
            else
            {
                foreach (HeatmapTriangleTree child in tree.getChildren())
                {
                    this.visualizeHeatmapTriangle(graphics, child);
                }
            }
        }

        private void visualizeLineHeatmap(Graphics graphics)
        {          
            //instantiating these once for simplicity's sake
            float tempX = (float)location.X - (LINE_HEATMAP_WIDTH / 2.0f),
                  tempY = (float)setObjects[1].getLocation().Y;  // we draw from the first set to the second.                  
            float step = (int)(((float) setObjects[0].getLocation().Y - tempY)  / Math.Pow(2.0d, (double)heatmapRecursionDepth - 1.0d));
            SolidBrush brush = new SolidBrush(determineColor(heatmapBins[0]));            
            
            //need to do all but the last one
            for (int i = 0; i < heatmapBins.Length - 1; i++)
            {            
                brush.Color = determineColor(heatmapBins[i]);  //I want a custom alpha color based upon the intensity of items in the bin                
                graphics.FillRectangle(brush, tempX, tempY, LINE_HEATMAP_WIDTH, step);
                tempY += step;
            }

            //now we do the last chunk of percentages, we need to do it this way in case the step is kind of odd (want to make sure we capture the 100%ers)
            int index = heatmapBins.Length - 1;
            float yRemainder = (float) (setObjects[0].getLocation().Y) - tempY;
            brush.Color = determineColor(heatmapBins[heatmapBins.Length - 1]);            
            graphics.FillRectangle(brush, tempX, tempY, LINE_HEATMAP_WIDTH, yRemainder);
        }

        private Color determineColor(int currentAmount)
        {
            //this is for straight gradiant alphas
            int alpha = (int)((float)currentAmount / (float)globalMaxAmount * 255);

            //this is for a logarithmic scale
            //int alpha = (int)(Math.Log((double)members.Count, (double)maxMemberNum) * 255);

            return Color.FromArgb(alpha, Color.Black);
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

            double angleStep = (2D * Math.PI) / (double)this.setObjects.Count;
            double angle = 0;

            foreach (SetObject setObj in setObjects)
            {
                int newX = (int)((double)radius * Math.Sin(angle));
                int newY = (int)((double)radius * Math.Cos(angle));

                setObj.move(this.getLocation().X + newX, this.getLocation().Y + newY);
                angle += angleStep;
            }

            setupOverdraw();

            if (useHeatmap)
                if (setObjects.Count == 2)
                    setupLineHeatmap();
                else   //setObjects will always have 2 or more objects
                    setupTriangleHeatmap();

        }

        //draws each memberValue separately
        private void setupOverdraw()
        {
            //need to figure out where the sets will be located before we can setup the forces on the members
            foreach (SetObject setObj in setObjects)
            {
                foreach (MemberObject mObj in setObj.getMemberObjs())
                {
                    int memberX = 0;
                    int memberY = 0;
                    Member tempMember = mObj.getMember();

                    foreach (SetObject setStuff in setObjects)
                    {
                        float membership = ((float)tempMember.getMembershipAsPercent(setStuff.getSet()) / 100.0f);
                        int xDiff = (int)((float)(setStuff.getLocation().X - this.location.X) * membership);
                        int yDiff = (int)((float)(setStuff.getLocation().Y - this.location.Y) * membership);
                        memberX += xDiff;
                        memberY += yDiff;
                    }
                    mObj.setOffset(memberX, memberY);                    
                }
            }
        }

        #endregion

        #region Triangle Heatmaps!

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
        private void setupTriangleHeatmap()
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
                setLocations.Add(new Point(set.getLocation().X - this.location.X, set.getLocation().Y - this.location.Y));
            }


            //I know this can be done more concisely but I feel this is cleaner

            //adding the initial heatmap triangles
            for (int i = 0; i < setLocations.Count; i++)
            {
                if (i != setLocations.Count - 1)
                    vizTriangles.Add(new HeatmapTriangleObject(members.Count, new Point[] { new Point(0,0), setLocations[i], setLocations[i+1] }));
                else
                    vizTriangles.Add(new HeatmapTriangleObject(members.Count, new Point[] { new Point(0,0), setLocations[i], setLocations[0] }));

            }

            //initial sorting of members
            int numNotAdded = 0;
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
                    numNotAdded++;
            }

            if(numNotAdded > 0)
                Console.Out.WriteLine(numNotAdded + " members weren't added");

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

        #region 1D Heatmaps

        //this will only occur if we have two sets, as a result we can base the entire thing off of one of them and just setup the line accordingly
        private void setupLineHeatmap()
        {
            //setup the size of the bins
            double numBins = Math.Pow(2.0d, (double)heatmapRecursionDepth - 1.0d);

            heatmapBins = new int[(int)numBins];

            //remember to initialize!
            for (int i = 0; i < (int)numBins; i++)
            {
                heatmapBins[i] = 0;
            }

            double percentStep = (100.0d / numBins);

            //interesting, this is where we actually end up doing more work than the triangle/recursive method
            List<MemberObject> members = new List<MemberObject>();

            foreach(SetObject set in setObjects)
                members.AddRange(set.getMemberObjs());
            
            foreach (MemberObject member in members)
            {
                //the reason why this is 100 - membership percent is because we want the most affiliated items closest to the first set
                //we will base the visualization off of the first set
                int memberIndex = (int)Math.Floor((double)(100 - member.getMember().getMembershipAsPercent(this.setObjects[1].getSet())) / percentStep);
                if (memberIndex == heatmapBins.Length)  // only happens in the case where the membership is 0
                    memberIndex--;
                heatmapBins[memberIndex]++;
            }

            int newNumMaxMembers = 0;
            for (int i = 0; i < (int)numBins; i++)
            {
                if (newNumMaxMembers < heatmapBins[i])
                    newNumMaxMembers = heatmapBins[i];
            }

            this.globalMaxAmount = newNumMaxMembers;
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
                if (setObjects.Count == 2)
                {
                    this.setupLineHeatmap();
                } else{
                    if (heatmapObjects.Count > 0) //we have heatmap objects
                    {
                        if (newRecursionDepth > 0)
                        {
                            this.doHeatmapRecursion(newRecursionDepth);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
