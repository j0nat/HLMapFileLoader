using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace HLMapFileLoader
{
    public class Map
    {
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static List<Brush> Load(string FileName, ContentManager contentManager)
        {
            List<Brush> brushes = FileParser.LoadBrushes(FileName);


            SortVerticesCW(brushes);
            LoadTextures(brushes, contentManager);
            CalculateTextureCoordinates(brushes);
            Inverse(brushes);

            return brushes;
        }

        private static void Inverse(List<Brush> brushes)
        {
            foreach (Brush brush in brushes)
            {
                foreach (Polygon poly in brush.polygons)
                {
                    for (int i = 0; i < poly.Vertices.Count; i++)
                    {
                        poly.Vertices[i] = new Vector3(poly.Vertices[i].X * -1, poly.Vertices[i].Y, poly.Vertices[i].Z);
                    }
                }
            }
        }

        private static void CalculateTextureCoordinates(List<Brush> brushes)
        {
            foreach (Brush brush in brushes)
            {
                foreach (Polygon poly in brush.polygons)
                {
                    GeometryMath.CalculateTextureCoordinates(poly);
                }
            }
        }

        private static void SortVerticesCW(List<Brush> brushes)
        {
            foreach (Brush brush in brushes)
            {
                foreach (Polygon poly in brush.polygons)
                {
                    GeometryMath.SortVerticesCW(poly);
                }
            }
        }

        private static void LoadTextures(List<Brush> brushes, ContentManager contentManager)
        {
            foreach (Brush brush in brushes)
            {
                foreach (Polygon poly in brush.polygons)
                {
                    poly.Texture = GetTexture(poly.Face.TextureName, contentManager);
                }
            }
        }

        private static Texture2D GetTexture(string textureName, ContentManager contentManager)
        {
            if (textures.ContainsKey(textureName))
            {
                return textures[textureName];
            }
            else
            {
                Texture2D texture = contentManager.Load<Texture2D>(textureName);

                textures.Add(textureName, texture);

                return texture;
            }
        }
    }
}