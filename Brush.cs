using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HLMapFileLoader
{
    public class Brush
    {
        private Face[] faces;
        public List<Polygon> polygons { get; private set; }

        public Brush(List<Face> faces)
        {
            this.faces = faces.ToArray();

            BrushToVertices();
        }

        private void BrushToVertices()
        {
            List<Plane> planes = new List<Plane>();

            for (int i = 0; i < faces.Length; i++)
            {
                planes.Add(new Plane(faces[i].P1, faces[i].P2, faces[i].P3));
            }

            Plane lfi = new Plane();
            Plane lfj = new Plane();
            Plane lfk = new Plane();

            polygons = new List<Polygon>();

            for (int i = 0; i < faces.Length; i++)
            {
                polygons.Add(new Polygon(planes[i], faces[i]));

                if (i == faces.Length - 3)
                {
                    if ((i + 1) < faces.Length)
                    {
                        lfi = planes[i + 1];
                    }
                }
                else if (i == faces.Length - 2)
                {
                    if ((i + 1) < faces.Length)
                    {
                        lfj = planes[i + 1];
                    }
                }
                else if (i == faces.Length - 1)
                {
                    if ((i + 1) < faces.Length)
                    {
                        lfk = planes[i + 1];
                    }
                }
            }

            for (int fi = 0; planes[fi] != lfi; fi++)
            {
                for (int fj = (fi + 1); planes[fj] != lfj; fj++)
                {
                    for (int fk = (fj + 1); planes[fk] != lfk; fk++)
                    {
                        Vector3 p;

                        if (GeometryMath.GetIntersection(planes[fj], planes[fk], planes[fi], out p))
                        {
                            bool illegal = false;

                            for (int i = 0; i < faces.Length; i++)
                            {
                                if (GeometryMath.ClassifyPoint(p, planes[i]) == GeometryMath.eCP.FRONT)
                                {
                                    illegal = true;
                                    break;
                                }
                            }

                            if (!illegal)
                            {
                                polygons[fi].Vertices.Add(p);
                                polygons[fj].Vertices.Add(p);
                                polygons[fk].Vertices.Add(p);
                            }
                        }

                        if ((fk + 1) >= faces.Length)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
