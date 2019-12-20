using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;

namespace HLMapFileLoader
{
    public class FileParser
    {
        public static List<Brush> LoadBrushes(string fileName)
        {
            List<Brush> brushes = new List<Brush>();

            string mapData = File.ReadAllText(fileName);

            for (int i = 2; i < mapData.Split('{').Length; i++)
            {
                string mapDataSplit = mapData.Split('{')[i].Split('}')[0].Trim();
                string[] planeData = mapDataSplit.Split('\n');

                if (!planeData[0].Trim().StartsWith("("))
                    continue;

                List<Face> faces = new List<Face>();
                for (int ii = 0; ii < planeData.Length; ii++)
                {
                    faces.Add(ParseFace(planeData[ii]));
                }

                brushes.Add(new Brush(faces));
            }

            return brushes;
        }

        private static Face ParseFace(string brushData)
        {
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            string brushDataP1 = brushData.Split('(')[1].Split(')')[0].Trim();
            string brushDataP2 = brushData.Split('(')[2].Split(')')[0].Trim();
            string brushDataP3 = brushData.Split('(')[3].Split(')')[0].Trim();
            string textureName = brushData.Split('(')[3].Split(')')[1].Trim().Split('[')[0].Trim();

            string textureAxisU = brushData.Split(new string[] { "[" }, StringSplitOptions.None)[1].Split(']')[0].Trim().Trim();
            string textureAxisV = brushData.Split(new string[] { "[" }, StringSplitOptions.None)[2].Split(']')[0].Trim().Trim();

            string textureScale = brushData.Split(new string[] { "]" }, StringSplitOptions.None)[2].Trim();

            Vector3 p1 = new Vector3(float.Parse(brushDataP1.Split(' ')[0]),
                float.Parse(brushDataP1.Split(' ')[1], NumberStyles.Any, ci),
                float.Parse(brushDataP1.Split(' ')[2], NumberStyles.Any, ci));

            Vector3 p2 = new Vector3(float.Parse(brushDataP2.Split(' ')[0]),
                float.Parse(brushDataP2.Split(' ')[1], NumberStyles.Any, ci),
                float.Parse(brushDataP2.Split(' ')[2], NumberStyles.Any, ci));

            Vector3 p3 = new Vector3(float.Parse(brushDataP3.Split(' ')[0], NumberStyles.Any, ci),
                float.Parse(brushDataP3.Split(' ')[1], NumberStyles.Any, ci),
                float.Parse(brushDataP3.Split(' ')[2], NumberStyles.Any, ci));

            Plane planeU = new Plane(
                float.Parse(textureAxisU.Split(' ')[0], NumberStyles.Any, ci),
                float.Parse(textureAxisU.Split(' ')[2], NumberStyles.Any, ci),
                float.Parse(textureAxisU.Split(' ')[1], NumberStyles.Any, ci),
                float.Parse(textureAxisU.Split(' ')[3], NumberStyles.Any, ci));

            Plane planeV = new Plane(
                float.Parse(textureAxisV.Split(' ')[0], NumberStyles.Any, ci),
                float.Parse(textureAxisV.Split(' ')[2], NumberStyles.Any, ci),
                float.Parse(textureAxisV.Split(' ')[1], NumberStyles.Any, ci),
                float.Parse(textureAxisV.Split(' ')[3], NumberStyles.Any, ci));

            //  texture rotation is given (which is useless information because the texture axis are already rotated). 

            float textureScaleU = float.Parse(textureScale.Split(' ')[1].Trim(), NumberStyles.Any, ci);
            float textureScaleV = float.Parse(textureScale.Split(' ')[2], NumberStyles.Any, ci);

            // Rotate plane X Z Y
            return new Face(
                new Vector3(p1.X, p1.Z, p1.Y),
                new Vector3(p2.X, p2.Z, p2.Y),
                new Vector3(p3.X, p3.Z, p3.Y), textureName, planeU, planeV, new float[] { textureScaleU, textureScaleV });
        }
    }
}
