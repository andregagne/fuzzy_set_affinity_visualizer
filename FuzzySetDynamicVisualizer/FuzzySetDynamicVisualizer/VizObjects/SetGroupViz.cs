using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer.VizObjects
{
    public class SetGroupViz : VizObject
    {
        public readonly List<SetViz> vizSets = new List<SetViz>();
        private readonly Dictionary<Set, SetViz> setStructures = new Dictionary<Set, SetViz>();
        private Color colour;        

        //variables for heatmaps
        private readonly List<HeatmapTriangleTree> heatmapTrees = new List<HeatmapTriangleTree>();
        private int heatmapRecursionDepth = 1;         
        private bool useHeatmap = false;

        //variables for line heatmaps
        private int lineHeatmapMax = 1;
        private int[] heatmapBins;
        private static float LINE_HEATMAP_WIDTH = 20.0f;              

        public SetGroupViz()
        {
            location = new Point(0, 0);
            this.colour = Color.LightYellow;
        }

        public SetGroupViz(List<SetViz> newSets, Color color)
        {
            vizSets.AddRange(newSets);
            this.location = this.calculateCenterPoint();
            this.Radius = newSets[0].Radius * 2;
            foreach (SetViz setVizObj in newSets)
                setStructures.Add(setVizObj.set, setVizObj);
            this.arrangeObjects();
            this.colour = Color.FromArgb(50, color);
        }

        public SetGroupViz(List<SetViz> newSets, Color color, bool heatmapSwitch, int heatmapRecursionDepth)
        {
            this.useHeatmap = heatmapSwitch;
            this.heatmapRecursionDepth = heatmapRecursionDepth;
            vizSets.AddRange(newSets);
            this.location = this.calculateCenterPoint();
            this.Radius = newSets[0].Radius * 2;
            foreach (SetViz setVizObj in newSets)
                setStructures.Add(setVizObj.set, setVizObj);
            this.arrangeObjects();
            this.colour = Color.FromArgb(50, color);
        }

        #region visualizations

        //only need this pen for the visualization
        private static Pen linePen = new Pen(Color.FromArgb(150, 255, 0, 0));
        public override void visualize(Graphics graphics)
        {
            List<Point> points = new List<Point>();            

            //draw the circle for this set group
            graphics.FillEllipse(new SolidBrush(colour), this.location.X - this.Radius, this.location.Y - this.Radius, Radius * 2, Radius * 2);

            if (useHeatmap)
                if(vizSets.Count == 2)
                    this.visualizeLineHeatmap(graphics);
                else  // we know there are 3 or more so we'll use the triangle visualization system
                {
                    foreach (HeatmapTriangleTree triangleTree in heatmapTrees)
                    {
                        this.visualizeHeatmapTriangle(graphics, triangleTree);
                    }
                }

            foreach (SetViz setVizObj in vizSets)
            {
                //draws the center point for the sets
                if (setVizObj.location.X < location.X)
                    setVizObj.drawCore(graphics, false, false);
                else
                    setVizObj.drawCore(graphics, true, false);

                if (!useHeatmap)
                {
                    setVizObj.visualizeChildrenAtLocation(graphics, this.location);
                }                

                points.Add(setVizObj.location);
                graphics.DrawLine(linePen, this.location, setVizObj.location);
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
                tree.data.visualize(graphics, this.location);
            }
            else
            {
                foreach (HeatmapTriangleTree child in tree.childrenNodes)
                {
                    this.visualizeHeatmapTriangle(graphics, child);
                }
            }
        }

        private void visualizeLineHeatmap(Graphics graphics)
        {          
            //instantiating these once for simplicity's sake
            float tempX = (float)location.X - (LINE_HEATMAP_WIDTH / 2.0f),
                  tempY = (float)vizSets[1].location.Y;  // we draw from the first set to the second.                  
            float step = (int)(((float) vizSets[0].location.Y - tempY)  / Math.Pow(2.0d, (double)heatmapRecursionDepth - 1.0d));
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
            float yRemainder = (float) (vizSets[0].location.Y) - tempY;
            brush.Color = determineColor(heatmapBins[heatmapBins.Length - 1]);            
            graphics.FillRectangle(brush, tempX, tempY, LINE_HEATMAP_WIDTH, yRemainder);
        }

        private Color determineColor(int currentAmount)
        {
            //this is for straight gradiant alphas
            int alpha = (int)((float)currentAmount / (float)lineHeatmapMax * 255);

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
            foreach (SetViz set in vizSets)
                set.moveByDiff(xDiff, yDiff);            
        }
        #endregion

        public void addSetObj(SetViz newSetVizObj)
        {
            if (!vizSets.Contains(newSetVizObj))
            {
                this.vizSets.Add(newSetVizObj);
                setStructures.Add(newSetVizObj.set, newSetVizObj);
                this.location = this.calculateCenterPoint();
                this.arrangeObjects();
            }
        }

        public void removeSetObj(SetViz removeObj)
        {
            vizSets.Remove(removeObj);
            setStructures.Remove(removeObj.set);
            this.location = this.calculateCenterPoint();
            if (vizSets.Count > 1)
            {
                this.arrangeObjects();
            }
        }

        public Point calculateCenterPoint()
        {
            int newX = 0;
            int newY = 0;

            foreach (SetViz set in vizSets)
            {
                newX += set.location.X;
                newY += set.location.Y;
            }

            newX = newX / vizSets.Count;
            newY = newY / vizSets.Count;

            return new Point(newX, newY);
        }

        #region arranging objects

        /**
         * need to do several things:
         * 1) figure out locations of the sets
         * 2) draw the core circles of the different sets
         * 3) draw the different member objects
         *    each member needs to be influenced by the different sets based upon its membership amounts
         */
        public void arrangeObjects()
        {
            int numMembers = 0;
            foreach (SetViz setVizObj in vizSets)
            {
                numMembers += setVizObj.memberVizs.Count;
            }

            double angleStep = (2D * Math.PI) / (double)this.vizSets.Count;
            double angle = 0;

            foreach (SetViz setVizObj in vizSets)
            {
                int newX = (int)((double)Radius * Math.Sin(angle));
                int newY = (int)((double)Radius * Math.Cos(angle));

                setVizObj.move(this.location.X + newX, this.location.Y + newY);
                angle += angleStep;
            }

            setupOverdraw();

            if (useHeatmap)
                if (vizSets.Count == 2)
                    setupLineHeatmap();
                else   //setVizObjects will always have 2 or more objects
                    setupTriangleHeatmap();

        }

        //draws each memberValue separately
        private void setupOverdraw()
        {
            //need to figure out where the sets will be located before we can setup the forces on the members
            foreach (SetViz setVizObj in vizSets)
            {
                foreach (MemberViz mObj in setVizObj.memberVizs)
                {
                    int memberX = 0;
                    int memberY = 0;
                    Member tempMember = mObj.member;

                    foreach (SetViz setStuff in vizSets)
                    {
                        float membership = ((float)tempMember.getMembershipAsPercent(setStuff.set) / 100.0f);
                        int xDiff = (int)((float)(setStuff.location.X - this.location.X) * membership);
                        int yDiff = (int)((float)(setStuff.location.Y - this.location.Y) * membership);
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
         * What I'm going to do is break up each triangle into 4 sub triangles and then use the 2D projected (i.e., graphical) location for each member to bin them.
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
            heatmapTrees.Clear();

            List<MemberViz> members = new List<MemberViz>();
            List<Point> setLocations = new List<Point>();
            List<HeatmapTriangleObject> vizTriangles = new List<HeatmapTriangleObject>();

            //initial setup we'll always have to do
            //pull what we want from the sets
            foreach (SetViz set in vizSets)
            {
                members.AddRange(set.memberVizs);
                setLocations.Add(new Point(set.location.X - this.location.X, set.location.Y - this.location.Y));
            }

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
            foreach (MemberViz member in members)
            {
                bool isAdded = false;
                foreach (HeatmapTriangleObject triangle in vizTriangles)
                {
                    if (triangle.isPointInside(member.location))
                    {
                        triangle.members.Add(member);
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
                heatmapTrees.Add(new HeatmapTriangleTree(triangle, null, triangle.points));
            }

            //now we find the global maximum number of members and distribute it out
            int newNumMaxMembers = 0;
            foreach (HeatmapTriangleObject triangle in vizTriangles)
            {
                if (newNumMaxMembers < triangle.members.Count)
                    newNumMaxMembers = triangle.members.Count;
            }
            foreach (HeatmapTriangleTree triangle in heatmapTrees)
            {
                triangle.setLeafMaxMembers(newNumMaxMembers);
            }

            //and now we split the trees
            this.doHeatmapRecursion(heatmapRecursionDepth);
        }
        

        /*
         *
         * splitToDepth
         * Will not use standard recursion as we can save future computation by doing each level of tree at a time.
         * 
         * We will need to first determine the depth of the triangles.
         * 
         * We'll use a list to keept track of the triangles we should be working on.  Because everything is symetric we should be able to use the 
         * first item in each list instead of checking each one.
         * 
         */
        private void doHeatmapRecursion(int maxDepth)
        {
            if (heatmapTrees[0].getDepth() != maxDepth)
            {
                if (heatmapTrees[0].getDepth() > maxDepth) // The tree is deeper than we want, collapse it
                {
                    while (heatmapTrees[0].getDepth() > maxDepth)
                    {
                        foreach (HeatmapTriangleTree tree in heatmapTrees)
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
                    while (heatmapTrees[0].getDepth() < maxDepth)
                    {
                        List<HeatmapTriangleObject> newLeaves = new List<HeatmapTriangleObject>();
                        foreach (HeatmapTriangleTree tree in heatmapTrees)
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
                            if (newNumMaxMembers < triangle.members.Count)
                                newNumMaxMembers = triangle.members.Count;
                        }
                        foreach (HeatmapTriangleTree triangle in heatmapTrees)
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

        //this will only occur if we have two sets, as a result we can base the entire thing off of one of them and just setup the line/square accordingly
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
            List<MemberViz> members = new List<MemberViz>();

            foreach(SetViz set in vizSets)
                members.AddRange(set.memberVizs);
            
            foreach (MemberViz member in members)
            {
                //the reason why this is 100 - membership percent is because we want the most affiliated items closest to the second set
                //we will base the visualization off of the second set because it makes the visual calculations easier (y is increasing)
                int memberIndex = (int)Math.Floor((double)(100 - member.member.getMembershipAsPercent(this.vizSets[1].set)) / percentStep);
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

            this.lineHeatmapMax = newNumMaxMembers;
        }


        #endregion 

        #region overrides
        public override void setIsHitBySelected(bool isHit)
        {
            this.hitBySelected = isHit;
        }

        public override VizObject objectHit(Point point)
        {
            foreach (SetViz set in vizSets)
                if (set.isCoreHit(point.X, point.Y))
                    return set;

            if (this.isHit(point))
                return this;

            return null;
        }
        
        public override void setScale(float newScale)
        {
            this.scale = newScale;
            foreach (SetViz set in vizSets)
                set.setScale(newScale);
        }
        #endregion

        #region getters and setters
        internal void setMemberRadius(int newRadius)
        {
            foreach (SetViz setVizObj in vizSets)
                setVizObj.setMemberRadius(newRadius);
        }

        internal int getMemberRadius()
        {
            if (vizSets.Count != 0)
                return vizSets[0].getMemberRadius();
            else return 0;
        }

        internal void setMemberAlpha(int newAlpha)
        {
            foreach (SetViz set in vizSets)
                set.setMemberAlpha(newAlpha);
        }

        public void setUseHeatmap(bool newUseHeatmap)
        {
            useHeatmap = newUseHeatmap;
            foreach (SetViz set in vizSets)
                set.setHeatmaps(newUseHeatmap);
            this.arrangeObjects();
        }
        
        internal bool setHeatmapRecursionDepth(int newRecursionDepth)
        {
            bool setupHeatmaps = false;
            this.heatmapRecursionDepth = newRecursionDepth;
            if (useHeatmap)
            {
                if (vizSets.Count == 2)
                {
                    this.setupLineHeatmap();
                    setupHeatmaps = true;
                } else{
                    if (heatmapTrees.Count > 0) //we have heatmap objects
                    {
                        if (newRecursionDepth > 0)
                        {
                            this.doHeatmapRecursion(newRecursionDepth);
                            setupHeatmaps = true;
                        }
                    }
                }
            }

            foreach (SetViz set in vizSets)
            {
                set.setHeatmapRecursionDepth(newRecursionDepth);
            }

            return setupHeatmaps;
        }
        #endregion
    }
}
