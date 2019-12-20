using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace HLMapFileLoader
{
    public class Polygon
    {
        public List<Vector3> Vertices { get; set; }
        public List<Vector2> TextureScales { get; set; }
        public Plane Plane { get; private set; }
        public Face Face { get; private set; }
        public Texture2D Texture { get; set; }

        public Polygon(Plane plane, Face face)
        {
            this.Face = face;
            this.TextureScales = new List<Vector2>();
            this.Vertices = new List<Vector3>();
            this.Plane = plane;
        }
    }
}
