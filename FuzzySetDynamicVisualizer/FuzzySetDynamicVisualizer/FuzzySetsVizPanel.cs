using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FuzzySetDynamicVisualizer.VizObjects;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer
{
    class FuzzySetsVizPanel : Panel
    {
        private List<VizObject> VizObjects = new List<VizObject>();
        private static int SET_BUFFER_SPACE = 10;
        private ToolStripStatusLabel statusLabel;
        private Point previousMousePoint;
        private VizObject selected = null;
        private float scale = 1.0f;
        private bool heatmapSwitch = false;
        private int heatmapRecursionDepth = 1;

        public FuzzySetsVizPanel(ToolStripStatusLabel label)
        {
            Console.WriteLine("FuzzySets Panel starting up");
            statusLabel = label;
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        public void loadVizObjects(List<VizObject> vizObjects)
        {
            this.VizObjects = vizObjects;
            this.arrangeVizObjects();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FuzzySetsVizPanel
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MinimumSize = new System.Drawing.Size(10, 10);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.onPaint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.onMouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.onMouseUp);
            this.Resize += new System.EventHandler(this.onResize);
            this.ResumeLayout(false);
        }

        #region graphics stuff
        private void onPaint(object sender, PaintEventArgs e)
        {
            Graphics graph = e.Graphics;
            graph.ScaleTransform(scale, scale);
            foreach (VizObject vObj in VizObjects)
            {
                vObj.visualize(graph);
            }
        }

        private void arrangeVizObjects()
        {
            int maxRadius = VizObjects[0].Radius;

            Random randoms = new Random();

            /**
             * this will:
             *   1) randomly set the location of the first object
             *   
             */

            if (VizObjects.Count > 0)
            {
                //set first object
                //  arrange objects around first

                VizObject firstObject = VizObjects[0];
                int newY = Math.Max(randoms.Next(this.Height - firstObject.Radius), firstObject.Radius);
                int newX = Math.Max(randoms.Next(this.Width - firstObject.Radius), firstObject.Radius);

                firstObject.move(new Point(newX, newY));

                //to do this properly, I need to first know the circumfrence of the circle around a set object
                int numAvailableLocations = 1;  //because there's always one location you can put it
                double outerRadius = 2D * (double)SET_BUFFER_SPACE;
                if (firstObject is SetViz)
                {
                    outerRadius = 2.0D * (double)((SetViz)firstObject).Radius + (double)SET_BUFFER_SPACE;
                    double outerRing = 2.0D * Math.PI * outerRadius;
                    //I'm adding the +1 in here to make sure there's some space between the outer sets
                    numAvailableLocations = (int)(outerRing / ((double)((SetViz)firstObject).Radius * 2D));  
                }

                double angleStep = (2D * Math.PI) / (double)numAvailableLocations;
                double angle = 0;
                //now arrange the rest of the objects around the first.
                for (int i = 1; i < VizObjects.Count & i < numAvailableLocations; i++)
                {
                    newX = (int)(outerRadius * Math.Sin(angle));
                    newY = (int)(outerRadius * Math.Cos(angle));

                    VizObjects[i].move(new Point(firstObject.location.X + newX, firstObject.location.Y + newY));
                    angle += angleStep;
                }

                this.recenterObjects();
            }
        }

        private void onResize(object sender, EventArgs e)
        {

        }

        #endregion

        #region mouse stuff
        private void onMouseDown(object sender, MouseEventArgs e)
        {
            Point tempPoint = new Point((int)((float)e.Location.X / scale), (int)((float)e.Location.Y / scale));

            foreach (VizObject vObj in VizObjects)
            {
                selected = vObj.objectHit(tempPoint);
                if (selected != null)
                {
                    previousMousePoint = tempPoint; //saving this for the move
                    if (vObj is SetGroupViz && selected is SetViz) //special case for when we're pulling a set from a group
                    {
                        SetGroupViz group = (SetGroupViz)vObj;
                        group.removeSetObj((SetViz)selected);
                        //need to deal with case where removing the selected leaves only 1 set left in group  
                        // need to disolve group and add the set back to this panel's sets.
                        if (group.vizSets.Count == 1)
                        {
                            SetViz lastSet = group.vizSets[0];
                            this.VizObjects.Add(lastSet);
                            this.VizObjects.Remove(group);
                            lastSet.arrange();
                        }

                        this.VizObjects.Add(selected);
                        ((SetViz)selected).arrange();
                    }
                    statusLabel.Text = selected.ToString() + " selected";
                    break;
                }
            }
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            //adds a set to another set or a group of sets if the first set is over them.
            if (selected != null && selected is SetViz)
            {
                List<SetViz> sets = new List<SetViz>();
                foreach (VizObject obj in VizObjects)
                {
                    if (!obj.Equals(selected) && obj.getIsHitBySelected())
                    {
                        if (obj is SetViz)
                        {
                            sets.Add((SetViz)obj);
                        }
                        else if (obj is SetGroupViz)
                        {
                            ((SetGroupViz)obj).addSetObj((SetViz)selected);
                            sets.Clear();
                            this.VizObjects.Remove(selected);
                            this.Invalidate();
                            break;
                        }
                    }
                }
                if (sets.Count > 0)
                {
                    sets.Add((SetViz)selected);
                    this.setupSetGroup(sets);
                }
            }

            statusLabel.Text = "";
            selected = null;
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            if (selected != null)
            {
                //calculate the amount we've moved
                int xDiff = (int)((float)e.Location.X / scale) - previousMousePoint.X;
                int yDiff = (int)((float)e.Location.Y / scale) - previousMousePoint.Y;

                //save the new location
                previousMousePoint.X = (int)((float)e.Location.X / scale);
                previousMousePoint.Y = (int)((float)e.Location.Y / scale);

                //now get the objects to move
                selected.moveByDiff(xDiff, yDiff);

                //check to see if anything has been hit
                foreach (VizObject vObj in VizObjects)
                {
                    if (vObj != selected)
                        if (vObj.isHitByObject(selected))
                            vObj.setIsHitBySelected(true);
                        else
                            vObj.setIsHitBySelected(false);
                }
                this.Invalidate();
            }
            statusLabel.Text = e.Location.X + ", " + e.Location.Y;
        }
        #endregion

        #region aux functions

        /**
         * sets should include the selected set
         *  
         */
        public void setupSetGroup(List<SetViz> sets)
        {
            SetGroupViz newGroup = new SetGroupViz(sets, Color.Green, heatmapSwitch, heatmapRecursionDepth);

            foreach (SetViz set in sets)
            {
                this.VizObjects.Remove(set);
                set.setIsHitBySelected(false);
            }

            this.VizObjects.Add(newGroup);
            this.Invalidate();
        }

        public void setScale(float newScale)
        {
            this.scale = newScale;
            this.Invalidate();
        }

        public float getScale()
        {
            return scale;
        }

        public void setMemberRadius(int newRadius)
        {
            foreach (VizObject vizObj in VizObjects)
            {
                if (vizObj is SetViz)
                {
                    ((SetViz)vizObj).setMemberRadius(newRadius);
                }
                else if (vizObj is SetGroupViz)
                {
                    ((SetGroupViz)vizObj).setMemberRadius(newRadius);
                }
            }
            this.Invalidate();
        }

        #endregion

        internal void changeMemberAlpha(int newAlpha)
        {
            foreach (VizObject vizObj in VizObjects)
            {
                if (vizObj is SetViz)
                {
                    ((SetViz)vizObj).setMemberAlpha(newAlpha);
                }
                else if (vizObj is SetGroupViz)
                {
                    ((SetGroupViz)vizObj).setMemberAlpha(newAlpha);
                }
            }
            this.Invalidate();
        }


        #region heatmaps

        public void useHeatmap(bool useHeatmap)
        {
            this.heatmapSwitch = useHeatmap;
            foreach (VizObject vObj in VizObjects)
            {
                if (vObj is SetGroupViz)  // currently heatmaps are targetted for set groups only, may do sets next
                {
                    ((SetGroupViz)vObj).setUseHeatmap(useHeatmap);
                }
                else if (vObj is SetViz)
                {
                    ((SetViz)vObj).setHeatmaps(useHeatmap);
                }
            }
            this.Invalidate();
        }

        #endregion

        //Need to recenter the objects based upon the current zoom and available space.
        internal void recenterObjects()
        {
            //first we need to determine the centerpoint of all of the vizObjects
            //we do this by averaging all of the X's and Y's
            int calculatedX = 0;
            int calculatedY = 0;

            //sum the X's and Ys
            foreach (VizObject vObj in VizObjects)
            {
                calculatedX += vObj.location.X;
                calculatedY += vObj.location.Y;
            }

            //now we average the sums
            calculatedX = calculatedX / VizObjects.Count;
            calculatedY = calculatedY / VizObjects.Count;

            //get the center of the window
            int windowCenterX = this.Size.Width / 2;
            int windowCenterY = this.Size.Height / 2;

            //calculate difference
            int xDiff = windowCenterX - calculatedX;
            int yDiff = windowCenterY - calculatedY;

            foreach (VizObject vObj in VizObjects)
            {
                vObj.moveByDiff(xDiff, yDiff);
            }
            this.Invalidate();
        }

        internal void heatmapValueChanged(int newRecursionDepth)
        {
            heatmapRecursionDepth = newRecursionDepth;
            int hasChanged = 0;
            foreach (VizObject vizObj in VizObjects)
            {
                if (vizObj is SetGroupViz)
                {
                    if (((SetGroupViz)vizObj).setHeatmapRecursionDepth(newRecursionDepth))
                        hasChanged++;
                }
                else if (vizObj is SetViz)
                {
                    if (((SetViz)vizObj).setHeatmapRecursionDepth(newRecursionDepth))
                        hasChanged++;
                }
            }
            if (hasChanged > 0)
            {
                this.Invalidate();
            }
        }
    }
}
