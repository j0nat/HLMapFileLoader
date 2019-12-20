using System;
using Microsoft.Xna.Framework;

namespace HLMapFileLoader
{
    public class GeometryMath
    {
        private static double epsilon = 1e-3;
        public enum eCP { FRONT = 0, BACK, ONPLANE };

        public static float DistanceToPlane(Vector3 vector, Plane plane)
        {
            return Vector3.Dot(plane.Normal, vector) + plane.D;
        }

        public static eCP ClassifyPoint(Vector3 vector, Plane plane)
        {
            float distance = DistanceToPlane(vector, plane);

            if (distance > epsilon)
            {
                return eCP.FRONT;
            }
            else if (distance < -epsilon)
            {
                return eCP.BACK;
            }

            return eCP.ONPLANE;
        }

        public static bool GetIntersection(Plane p1, Plane p2, Plane p3, out Vector3 v)
        {
            v = new Vector3(0, 0, 0);

            float denom;
            denom = Vector3.Dot(p3.Normal, Vector3.Cross(p1.Normal, p2.Normal));

            if (Math.Abs(denom) < epsilon)
            {
                return false;
            }

            v = ((Vector3.Cross(p1.Normal, p2.Normal) * -p3.D)
                - (Vector3.Cross(p2.Normal, p3.Normal) * p1.D)
                - (Vector3.Cross(p3.Normal, p1.Normal) * p2.D)) / denom;

            return true;
        }

        public static void SortVerticesCW(Polygon poly)
        {
            Vector3 center = new Vector3();

            foreach (Vector3 vector in poly.Vertices)
            {
                center += vector;
            }

            center = center / poly.Vertices.Count;

            for (int i = 0; i < (poly.Vertices.Count - 2); i++)
            {
                Vector3 a;
                Plane p;
                double SmallestAngle = -1;
                int Smallest = -1;

                a = poly.Vertices[i] - center;
                a.Normalize();

                p = new Plane(poly.Vertices[i], center, center + poly.Plane.Normal);

                for (int j = i + 1; j < poly.Vertices.Count; j++)
                {
                    if (ClassifyPoint(poly.Vertices[j], p) != eCP.BACK)
                    {
                        Vector3 b;
                        double Angle;

                        b = poly.Vertices[j] - center;
                        b.Normalize();

                        Angle = Vector3.Dot(a, b);

                        if (Angle > SmallestAngle)
                        {
                            SmallestAngle = Angle;
                            Smallest = j;
                        }
                    }
                }

                if (Smallest == -1)
                {
                    return;
                }

                Vector3 t = poly.Vertices[Smallest];
                poly.Vertices[Smallest] = poly.Vertices[i + 1];
                poly.Vertices[i + 1] = t;
            }

            Plane beforePlane = poly.Plane;
            Plane afterPlane;

            CalculatePlane(poly, out afterPlane);

            if (Vector3.Dot(afterPlane.Normal, beforePlane.Normal) < 0)
            {
                int j = poly.Vertices.Count;
                
                for (int i = 0; i < j / 2; i++)
                {
                    Vector3 v = poly.Vertices[i];
                    poly.Vertices[i] = poly.Vertices[j - i - 1];
                    poly.Vertices[j - i - 1] = v;
                }
            }
        }

        public static void CalculateTextureCoordinates(Polygon poly)
        {
            for (int i = 0; i < poly.Vertices.Count; i++)
            {
                float U, V;
                
                U = Vector3.Dot(poly.Face.TextureAxisU.Normal, poly.Vertices[i]);
                U = U / poly.Texture.Width / poly.Face.TextureScale[0];
                U = U + (poly.Face.TextureAxisU.D / poly.Texture.Width);

                V = Vector3.Dot(poly.Face.TextureAxisV.Normal, poly.Vertices[i]);
                V = V / poly.Texture.Height / poly.Face.TextureScale[1];
                V = V + (poly.Face.TextureAxisV.D / poly.Texture.Height);

                poly.TextureScales.Add(new Vector2(U, V));
            }

            bool bDoU = true;
            bool bDoV = true;
            for (int i = 0; i < poly.Vertices.Count; i++)
            {
                if ((poly.TextureScales[i].X < 1) && (poly.TextureScales[i].Y > -1))
                {
                    bDoU = false;
                }

                if ((poly.TextureScales[i].X < 1) && (poly.TextureScales[i].Y > -1))
                {
                    bDoV = false;
                }
            }

            if (bDoU || bDoV)
            {
                double NearestU = 0;
                double U = poly.TextureScales[0].X;

                double NearestV = 0;
                double V = poly.TextureScales[0].Y;

                if (bDoU)
                {
                    if (U > 1)
                    {
                        NearestU = Math.Floor(U);
                    }
                    else
                    {
                        NearestU = Math.Ceiling(U);
                    }
                }

                if (bDoV)
                {
                    if (V > 1)
                    {
                        NearestV = Math.Floor(V);
                    }
                    else
                    {
                        NearestV = Math.Ceiling(V);
                    }
                }

                for (int i = 0; i < poly.Vertices.Count; i++)
                {
                    if (bDoU)
                    {
                        U = poly.TextureScales[i].X;

                        if (Math.Abs(U) < Math.Abs(NearestU))
                        {
                            if (U > 1)
                            {
                                NearestU = Math.Floor(U);
                            }
                            else
                            {
                                NearestU = Math.Ceiling(U);
                            }
                        }
                    }

                    if (bDoV)
                    {
                        V = poly.TextureScales[i].Y;

                        if (Math.Abs(V) < Math.Abs(NearestV))
                        {
                            if (V > 1)
                            {
                                NearestV = Math.Floor(V);
                            }
                            else
                            {
                                NearestV = Math.Ceiling(V);
                            }
                        }
                    }
                }

                for (int i = 0; i < poly.Vertices.Count; i++)
                {
                    poly.TextureScales[i] = new Vector2(poly.TextureScales[i].X - (float)NearestU, poly.TextureScales[i].Y - (float)NearestV);
                }
            }
        }

        public static void CalculatePlane(Polygon poly, out Plane plane)
        {
            Vector3 centerOfMass;
            double magnitude;
            int i, j;

            plane = poly.Plane;

            if (poly.Vertices.Count < 3)
            {
                return;
            }

            plane.Normal.X = 0f;
            plane.Normal.Y = 0f;
            plane.Normal.Z = 0f;
            centerOfMass.X = 0;
            centerOfMass.Y = 0;
            centerOfMass.Z = 0;

            for (i = 0; i < poly.Vertices.Count; i++)
            {
                j = i + 1;

                if (j >= poly.Vertices.Count)
                {
                    j = 0;
                }

                plane.Normal.X += (poly.Vertices[i].Y - poly.Vertices[j].Y) * (poly.Vertices[i].Z + poly.Vertices[j].Z);
                plane.Normal.Y += (poly.Vertices[i].Z - poly.Vertices[j].Z) * (poly.Vertices[i].X + poly.Vertices[j].X);
                plane.Normal.Z += (poly.Vertices[i].X - poly.Vertices[j].X) * (poly.Vertices[i].Y + poly.Vertices[j].Y);

                centerOfMass.X += poly.Vertices[i].X;
                centerOfMass.Y += poly.Vertices[i].Y;
                centerOfMass.Z += poly.Vertices[i].Z;
            }

            if ((Math.Abs(plane.Normal.X) < epsilon) && (Math.Abs(plane.Normal.Y) < epsilon)
                && (Math.Abs(plane.Normal.Z) < epsilon))
            {
                return;
            }

            magnitude = Math.Sqrt(plane.Normal.X * plane.Normal.X + plane.Normal.Y * plane.Normal.Y + plane.Normal.Z * plane.Normal.Z);

            if (magnitude < epsilon)
            {
                return;
            }

            plane.Normal.X /= (float)magnitude;
            plane.Normal.Y /= (float)magnitude;
            plane.Normal.Z /= (float)magnitude;

            centerOfMass.X /= poly.Vertices.Count;
            centerOfMass.Y /= poly.Vertices.Count;
            centerOfMass.Z /= poly.Vertices.Count;

            plane.D = -(Vector3.Dot(centerOfMass, plane.Normal));

            return;
        }
    }
}
