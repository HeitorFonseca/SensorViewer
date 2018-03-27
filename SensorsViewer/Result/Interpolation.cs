// <copyright file="Interpolation.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Result
{
    using SensorsViewer.SensorOption;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    static public class Interpolation
    {        
        static private Dictionary<Tuple<int, int>, double> dic = new Dictionary<Tuple<int, int>, double>();

        static private MeshGeometry3D BuildDictionary(MeshGeometry3D mesh)
        {
            //Create the mesh to hold the new surface
            MeshGeometry3D newMesh = mesh.Clone();

            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int index0 = newMesh.TriangleIndices[i];
                int index1 = newMesh.TriangleIndices[i + 1];
                int index2 = newMesh.TriangleIndices[i + 2];

                Point3D p0 = newMesh.Positions[index0];
                Point3D p1 = newMesh.Positions[index1];
                Point3D p2 = newMesh.Positions[index2];

                p0.X = Math.Round(p0.X);
                p0.Y = Math.Round(p0.Y);
                p0.Z = Math.Round(p0.Z);

                newMesh.Positions[index0] = p0;

                p1.X = Math.Round(p1.X);
                p1.Y = Math.Round(p1.Y);
                p1.Z = Math.Round(p1.Z);

                newMesh.Positions[index1] = p1;

                p2.X = Math.Round(p2.X);
                p2.Y = Math.Round(p2.Y);
                p2.Z = Math.Round(p2.Z);

                newMesh.Positions[index2] = p2;
            }

            return newMesh;
        }

        static public Dictionary<Tuple<int, int>, double> FillSensorDataDictionary(List<Sensor> sensorsDataList)
        {
            Dictionary<Tuple<int, int>, double> newDictionary = new Dictionary<Tuple<int, int>, double>();

            foreach (Sensor sd in sensorsDataList)
            {
                int x = Convert.ToInt32(Math.Round(sd.X));
                int y = Convert.ToInt32(Math.Round(sd.Y));

                Tuple<int, int> key = new Tuple<int, int>(x, y);

                newDictionary.Add(key, sd.Z);
            }
          
            return newDictionary;
        }

        static public Dictionary<Tuple<int, int>, double> PreProcessing(Dictionary<Tuple<int, int>, double> sensorDictionary)
        {

            Dictionary<Tuple<int, int>, double> ndic = new Dictionary<Tuple<int, int>, double>();

            foreach (KeyValuePair<Tuple<int, int>, double> v in dic)
            {
                int nx = v.Key.Item1;
                int ny = v.Key.Item2;

                double d1, d2;
                List<Tuple<int, int>> neighbors = GetNeighboringPoints(nx, ny, sensorDictionary, out d1, out d2);

                d1 = 1 / d1;
                d2 = 1 / d2;

                ndic[v.Key] = ((d1 * (sensorDictionary[neighbors.ElementAt(0)]) + d2 * (sensorDictionary[neighbors.ElementAt(1)])) / (d1 + d2)); //(newDictionary[neighbors.ElementAt(0)] + newDictionary[neighbors.ElementAt(1)]) / 2;
            }

            foreach (KeyValuePair<Tuple<int, int>, double> sd in sensorDictionary)
            {
                int x = Convert.ToInt32((sd.Key.Item1));
                int y = Convert.ToInt32((sd.Key.Item2));

                Tuple<int, int> key = new Tuple<int, int>(x, y);
                ndic[key] = sd.Value;
            }

            return ndic;
        }

        static private List<Tuple<int, int>> GetNeighboringPoints(int x, int y, Dictionary<Tuple<int, int>, double> newDictionary, out double mCoord1, out double mCoord2)
        {

            List<Tuple<int, int>> ret = new List<Tuple<int, int>>();

            double min1 = double.MaxValue, min2 = double.MaxValue;
            ret.Add(newDictionary.ElementAt(0).Key);
            ret.Add(newDictionary.ElementAt(1).Key);

            foreach (KeyValuePair<Tuple<int, int>, double> v in newDictionary)
            {
                int nx = v.Key.Item1;
                int ny = v.Key.Item2;

                // HERE
                if (y * ny < 0)
                    continue;

                double euclideanDistance = Math.Sqrt(Math.Pow(nx - x, 2) + Math.Pow(ny - y, 2));

                if (euclideanDistance < min1)
                {
                    min2 = min1;
                    min1 = euclideanDistance;

                    ret[1] = ret[0];
                    ret[0] = v.Key;

                }
                else if (euclideanDistance < min2)
                {
                    min2 = euclideanDistance;
                    ret[1] = v.Key;
                }
            }

            mCoord1 = min1;
            mCoord2 = min2;

            return ret;
        }

        static private Tuple<int, int> GetNeighboringPoints2(int x, int y, Dictionary<Tuple<int, int>, double> newDictionary)
        {

            Tuple<int, int> coord;

            double min1 = double.MaxValue;
            coord = newDictionary.ElementAt(0).Key;

            foreach (KeyValuePair<Tuple<int, int>, double> v in newDictionary)
            {
                int nx = v.Key.Item1;
                int ny = v.Key.Item2;

                double euclideanDistance = Math.Sqrt(Math.Pow(nx - x, 2) + Math.Pow(ny - y, 2));

                if (euclideanDistance < min1)
                {
                    min1 = euclideanDistance;
                    coord = v.Key;
                }
            }

            return coord;
        }

        static private double CrossProduct(Point3D p1, Point3D p2)
        {
            return p1.X * p2.Y - p1.Y * p2.X;
        }

        static public Color GetHeatMapColor(double value, double min_value, double max_value)
        {
            if (value > 0.011 && value < 0.013)
            {
                var a = 123;
            }

            double rValue = (value - min_value) / (max_value - min_value);

            const int NUM_COLORS = 3;

            double[,] color = new double[NUM_COLORS, 3] { { 255, 0, 0 }, { 0, 0, 0 }, { 0, 255, 0 } };

            int idx1;
            int idx2;

            double fractBetween = 0;  // Fraction between "idx1" and "idx2" where our value is.

            if (rValue <= 0) { idx1 = idx2 = 0; }    // accounts for an input <=0
            else if (rValue >= 1) { idx1 = idx2 = NUM_COLORS - 1; }    // accounts for an input >=0
            else
            {
                rValue = rValue * (NUM_COLORS - 1);        // Will multiply value by 2.
                idx1 = Convert.ToInt32(Math.Floor(rValue));                  // Our desired color will be after this index.
                idx2 = idx1 + 1;                        // ... and before this index (inclusive).
                fractBetween = rValue - (float)(idx1);    // Distance between the two indexes (0-1).
            }

            double red = (color[idx2, 0] - color[idx1, 0]) * fractBetween + color[idx1, 0];
            double green = (color[idx2, 1] - color[idx1, 1]) * fractBetween + color[idx1, 1];
            double blue = (color[idx2, 2] - color[idx1, 2]) * fractBetween + color[idx1, 2];

            //Color asd = new Color(Convert.ToInt32(red), Convert.ToInt32(green), Convert.ToInt32(blue));
            Color asd = new Color() { R = Convert.ToByte(red), G = Convert.ToByte(green), B = Convert.ToByte(blue), A = 255 } ;

            return asd;
        }
    }    
}

