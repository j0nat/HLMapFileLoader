using Microsoft.Xna.Framework;

namespace HLMapFileLoader
{
    public class Face
    {
        public Vector3 P1 { get; private set; }
        public Vector3 P2 { get; private set; }
        public Vector3 P3 { get; private set; }
        public Plane TextureAxisU { get; private set; }
        public Plane TextureAxisV { get; private set; }
        public string TextureName { get; private set; }
        public float[] TextureScale { get; private set; }

        public Face(Vector3 p1, Vector3 p2, Vector3 p3, string textureName, Plane textureAxisU, Plane textureAxisV, float[] textureScale)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.P3 = p3;
            this.TextureName = textureName;
            this.TextureAxisU = textureAxisU;
            this.TextureAxisV = textureAxisV;
            this.TextureScale = textureScale; 
        }
    }
}
