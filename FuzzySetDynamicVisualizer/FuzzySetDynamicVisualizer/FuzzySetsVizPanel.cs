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
        private ToolStripStatusLabel statusLabel;
        private Point previousMousePoint;
        private VizObject selected = null;
        private float scale = 1.0f;
        private bool heatmapSwitch = false;

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
            int numObjects = VizObjects.Count;
            int maxRadius = VizObjects[0].getRadius();

            Random randoms = new Random();

            //is supposed to randomly distribute objects on the screen, still needs some work.
            foreach (VizObject vObj in VizObjects)
            {
                int newY = randoms.Next(this.Height - vObj.getRadius());
                int newX = randoms.Next(this.Width - vObj.getRadius());
                if (newX - vObj.getRadius() < 0)
                    newX = vObj.getRadius();
                if (newY - vObj.getRadius() < 0)
                    newY = vObj.getRadius();
                vObj.move(new Point(newX, newY));
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
                    if (vObj is SetGroupObject && selected is SetObject) //special case for when we're pulling a set from a group
                    {
                        SetGroupObject group = (SetGroupObject) vObj;
                        group.removeSetObj((SetObject) selected);
                        //need to deal with case where removing the selected leaves only 1 set left in group  
                        // need to disolve group and add the set back to this panel's sets.
                        if(group.numContainedSets() == 1){
                            SetObject lastSet = group.getSets()[0];
                            this.VizObjects.Add(lastSet);
                            this.VizObjects.Remove(group);
                            lastSet.arrange();
                        }

                        this.VizObjects.Add(selected);
                        ((SetObject)selected).arrange();
                    }
                    statusLabel.Text = selected.ToString() + " selected";
                    break;
                }
            }
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            //adds a set to another set or a group of sets if the first set is over them.
            if (selected != null && selected is SetObject)
            {
                List<SetObject> sets = new List<SetObject>();
                foreach (VizObject obj in VizObjects)
                {
                    if (!obj.Equals(selected) && obj.getIsHitBySelected())
                    {
                        if (obj is SetObject)
                        {
                            sets.Add((SetObject)obj);
                            break;
                        }
                        else if (obj is SetGroupObject)
                        {
                            ((SetGroupObject)obj).addSetObj((SetObject)selected);
                            this.VizObjects.Remove(selected);
                            this.Invalidate();
                            break;
                        }
                    }
                }
                if (sets.Count > 0)
                {
                    sets.Add((SetObject)selected);
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
                previousMousePoint.X = e.Location.X;
                previousMousePoint.Y = e.Location.Y;

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
        }
        #endregion

        #region aux functions

        /**
         * sets should include the selected set
         *  
         */
        public void setupSetGroup(List<SetObject> sets)
        {
            SetGroupObject newGroup = new SetGroupObject(sets, Color.Green, heatmapSwitch);

            foreach (SetObject set in sets)
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
                if (vizObj is MemberObject)
                {
                    ((MemberObject)vizObj).setRadius(newRadius);
                }
                else if (vizObj is SetObject)
                {
                    ((SetObject)vizObj).setMemberRadius(newRadius);
                }
                else if (vizObj is SetGroupObject)
                {
                    ((SetGroupObject)vizObj).setMemberRadius(newRadius);
                }
            }
            this.Invalidate();
        }

        #endregion

        internal void changeMemberAlpha(int newAlpha)
        {
            foreach (VizObject vizObj in VizObjects)
            {
                if (vizObj is MemberObject)
                {
                    ((MemberObject)vizObj).setAlpha(newAlpha);
                }
                else if (vizObj is SetObject)
                {
                    ((SetObject)vizObj).setMemberAlpha(newAlpha);
                }
                else if (vizObj is SetGroupObject)
                {
                    ((SetGroupObject)vizObj).setMemberAlpha(newAlpha);
                }
            }
            this.Invalidate();
        }

        public void useHeatmap(bool useHeatmap)
        {
            this.heatmapSwitch = useHeatmap;
            foreach (VizObject vObj in VizObjects)
            {
                if (vObj is SetGroupObject)  // currently heatmaps are targetted for set groups only, may do sets next
                {
                    ((SetGroupObject)vObj).setUseHeatmap(useHeatmap);
                }
            }
            this.Invalidate();
        }
    }
}
