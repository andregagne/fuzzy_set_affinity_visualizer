using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using FuzzySetDynamicVisualizer.DataStructures;

namespace FuzzySetDynamicVisualizer.VizObjects
{
    public class HeatmapTriangleObject : VizObject
    {
        protected List<MemberObject> members;
        protected Point[] points = new Point[3];
        protected int numMaxMembers = 1; //needed to determine alpha/shading levels
        

        public HeatmapTriangleObject() : base(){
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point(0, 0);
            }
        }

        public HeatmapTriangleObject(int maxMemberNum, Point[] points)
            : base()
        {
            this.points = points;
            this.numMaxMembers = maxMemberNum;
            this.members = new List<MemberObject>();

            //now we find the middle point of the points
            int x = 0, y = 0;
            for (int i = 0; i < points.Length; i++)
            {
                x += points[i].X;
                y += points[i].Y;
            }
            x = (int)((float)x / (float)points.Length);
            y = (int)((float)y / (float)points.Length);

            location.X = x;
            location.Y = y;
        }

        public HeatmapTriangleObject(List<MemberObject> members, int maxMemberNum, Point[] points) : base()
        {
            this.points = points;
            this.members = members;
            this.numMaxMembers = maxMemberNum;

            //now we find the middle point of the points
            int x = 0, y = 0;
            for (int i = 0; i < points.Length; i++)
            {
                x += points[i].X;
                y += points[i].Y;
            }
            x = (int)((float)x / (float)points.Length);
            y = (int)((float)y / (float)points.Length);

            location.X = x;
            location.Y = y;
        }

        public HeatmapTriangleObject(List<MemberObject> members, int maxMemberNum, Point point1, Point point2, Point point3) : base()
        {
            this.members = members;
            this.numMaxMembers = maxMemberNum;
            this.points[0] = point1;
            this.points[1] = point2;
            this.points[2] = point3;

            //now we find the middle point of the points
            int x = 0, y = 0;
            for (int i = 0; i < points.Length; i++)
            {
                x += points[i].X;
                y += points[i].Y;
            }
            x = (int)((float)x / (float)points.Length);
            y = (int)((float)y / (float)points.Length);

            location.X = x;
            location.Y = y;
        }

        public override void visualize(Graphics graphics)
        {
            SolidBrush brush = new SolidBrush(determineColor());
            graphics.FillPolygon(brush, points);
        }

        public override void move(Point newPoint)
        {
            this.move(newPoint.X, newPoint.Y);
        }

        public void move(int newX, int newY)
        {
            moveByDiff(newX - location.X, newY - location.Y);            
        }

        public override void moveByDiff(int xDiff, int yDiff)
        {            
            for (int i = 0; i < points.Length; i++)
            {                
                points[i].X += xDiff;
                points[i].Y += yDiff;                
            }
            this.location.X += xDiff;
            this.location.Y += yDiff;
        }

        private Color determineColor()
        {
            //this is for straight gradiant alphas
            int alpha = (int)((float)members.Count / (float)numMaxMembers * 255);

            //this is for a logarithmic scale
            //int alpha = (int)(Math.Log((double)members.Count, (double)maxMemberNum) * 255);

            return Color.FromArgb(alpha, Color.Black);            
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

        public int getNumMaxMembers()
        {
            return numMaxMembers;
        }

        public void setNumMaxMembers(int newNumMaxMembers)
        {
            this.numMaxMembers = newNumMaxMembers;
        }

        public List<HeatmapTriangleObject> getSubTriangles()
        {
            List<HeatmapTriangleObject> subTriangles = new List<HeatmapTriangleObject>();

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

            subTriangles.Add(new HeatmapTriangleObject(numMaxMembers, new Point[] { points[0], medialPoint1, medialPoint3}));
            subTriangles.Add(new HeatmapTriangleObject(numMaxMembers, new Point[] { points[1], medialPoint1, medialPoint2 }));
            subTriangles.Add(new HeatmapTriangleObject(numMaxMembers, new Point[] { points[2], medialPoint2, medialPoint3 }));
            subTriangles.Add(new HeatmapTriangleObject(numMaxMembers, new Point[] { medialPoint1, medialPoint2, medialPoint3 }));

            int numFailures = 0;

            foreach (MemberObject member in members)
            {
                if (subTriangles[0].isPointInside(member.getLocation()))
                    subTriangles[0].addMember(member);
                else if (subTriangles[1].isPointInside(member.getLocation()))
                    subTriangles[1].addMember(member);
                else if (subTriangles[2].isPointInside(member.getLocation()))
                    subTriangles[2].addMember(member);
                else if (subTriangles[3].isPointInside(member.getLocation()))
                    subTriangles[3].addMember(member);
                else
                    numFailures++;
            }

            if(numFailures > 0)
                Console.Out.WriteLine(numFailures + " of points were not added");

            return subTriangles;

        }

        //tests to see if the given point is inside the triangle.
        public bool isPointInside(Point testPoint)
        {
            bool isInside = false;

            //will use the barycentric coordinates version
            //initial vectors
            PointF vector1 = new PointF((float)points[1].X - (float)points[0].X, (float)points[1].Y - (float)points[0].Y);
            PointF vector2 = new PointF((float)points[2].X - (float)points[0].X, (float)points[2].Y - (float)points[0].Y);
            PointF vector3 = new PointF((float) testPoint.X - (float)points[0].X, (float) testPoint.Y - (float)points[0].Y);

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
            isInside = (u >= 0) && (v >= 0) && (u + v <= 1);
            return isInside;
        }

        public Point[] getPoints()
        {
            return this.points;
        }
    }
}
