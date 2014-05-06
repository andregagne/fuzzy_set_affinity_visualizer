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
        //        private List<HeatmapObj> heatmapObjs = new List<HeatmapObj>();
        private int heatmapBinSize = 5;

        //heatmapSums is layed out like a multi dimensional array in memory.
        //the problem is that we don't know how large it's supposed to be, the number of sets is variable
        //
        private int[] heatmapSums = null;
        private int maxSums = 0;

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

            graphics.FillEllipse(new SolidBrush(colour), this.location.X - this.radius, this.location.Y - this.radius, radius * 2, radius * 2);
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
                        mObj.visualize(graphics);
                }
                else
                {
                    vizualizeHeatmap(graphics);
                }

                points.Add(setObj.getLocation());
                graphics.DrawLine(linePen, this.location, setObj.getLocation());
            }
            graphics.DrawPolygon(linePen, points.ToArray());
        }

        private void vizualizeHeatmap(Graphics graphics)
        {
            if (heatmapSums != null)
            {
                //the heart of the heatmap visualization
                //the basis of this is that the heatmapsums object
                //heatmapSums is layed out like any array in memory                                               
                int binsPerSet = (int)(Math.Ceiling(100f / (float)this.heatmapBinSize));
                int newRadius = 5;

                for (int i = 0; i < heatmapSums.Length; i++)
                {
                    //need to do a deep copy of this to get it to work..
                    int amount = heatmapSums[i];
                    int membershipPercent = 0;
                    int workinglocation = i;
                    int binArraySize = (int)Math.Pow(binsPerSet, setObjects.Count - 2);
                    int dotAlpha = (int)((float)amount / (float)maxSums * 255f);
                    Color dotColor = Color.FromArgb(dotAlpha, colour);

                    int xDiff = 0;
                    int yDiff = 0;

                    for (int p = setObjects.Count - 2; p >= 0; p--)
                    {
                        //i * heatmapBinSize = location
                        //largest size = size of amoutn
                        int temp = workinglocation / binArraySize;

                        int memPercent = (int)((float)temp / (float)(binsPerSet) * 100f);
                        int setXdif = (setObjects[p].getLocation().X - location.X);
                        int setYdiff = (setObjects[p].getLocation().Y - location.Y);

                        xDiff += (int)((float)memPercent / 100f * (float)setXdif);
                        yDiff += (int)((float)memPercent / 100f * (float)setYdiff);

                        workinglocation = workinglocation % binArraySize;
                        membershipPercent += memPercent;
                        binArraySize /= binsPerSet;
                    }
                    int remainderPercent = 100 - membershipPercent;

                    xDiff += (int)((float)remainderPercent / 100f * (float)(setObjects[setObjects.Count - 1].getLocation().X - location.X));
                    yDiff += (int)((float)remainderPercent / 100f * (float)(setObjects[setObjects.Count - 1].getLocation().Y - location.Y));

                    graphics.FillEllipse(new SolidBrush(dotColor), location.X + xDiff - newRadius, location.Y + yDiff - newRadius, newRadius * 2, newRadius * 2);
                }

                Font thisFont = new Font("Arial", 8);
                string text = "The maximum value is " + maxSums;

                graphics.DrawString(text, thisFont, new SolidBrush(Color.Black), (float)(location.X - (radius / 2)), (float)(location.Y + radius + 50));
            }
        }

        #endregion

        public override void move(Point newPoint)
        {
            this.move(newPoint.X, newPoint.Y);
        }

        public void move(int newX, int newY)
        {
            int xDiff = newX - this.location.X;
            int yDiff = newY - this.location.Y;

            this.location.X = newX;
            this.location.Y = newY;
            foreach (SetObject set in setObjects)
            {
                set.move(set.getLocation().X + xDiff, set.getLocation().Y + yDiff);
            }
        }

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

            if (!useHeatmap)
            {
                setupOverdraw();
            }
            else
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
                        float membership = ((float)tempMember.getMembership(setStuff.getSet()) / 100.0f);
                        int xDiff = (int)((float)(setStuff.getLocation().X - this.location.X) * membership);
                        int yDiff = (int)((float)(setStuff.getLocation().Y - this.location.Y) * membership);
                        memberX += xDiff;
                        memberY += yDiff;
                    }
                    mObj.move(memberX, memberY);
                }
            }
        }

        private void setupHeatmap()
        {
            //not sure if we should be using the heatmap object
            //heatmapSums is a multi dimensional array
            int binsPerSet = (int)(Math.Ceiling(100f / (float)this.heatmapBinSize));

            heatmapSums = new int[(int)Math.Pow(binsPerSet, (setObjects.Count - 1))];

            //need n-1 sets to get affinities for binning
            List<Set> sets = new List<Set>();
            for (int i = 0; i < setObjects.Count - 1; i++)
                sets.Add(setObjects[i].getSet());


            foreach (SetObject setObj in setObjects)
                foreach (MemberObject memberObj in setObj.getMemberObjs())
                {
                    Member mem = memberObj.getMember();

                    int tempSize = 1;
                    int binArrayLoc = 0;
                    int memberShipPercent = 0;
                    //binning will worok by first set is miner array length, next is next length, etc.
                    foreach (Set set in sets)
                    {
                        int tempPercent = mem.getMembership(set);
                        binArrayLoc += (int)((float)tempPercent / (float)this.heatmapBinSize) * tempSize;
                        tempSize *= binsPerSet;
                        memberShipPercent += tempPercent;
                    }
                    //increment correct location in bin
                    heatmapSums[binArrayLoc]++;
                    if (heatmapSums[binArrayLoc] > maxSums)
                        maxSums = heatmapSums[binArrayLoc];
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
    }
}
