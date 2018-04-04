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

    using SharpDx = HelixToolkit.Wpf.SharpDX;


    static public class Interpolation
    {
        //static private Dictionary<Tuple<int, int>, double> dic = new Dictionary<Tuple<int, int>, double>();     

        static private Dictionary<Tuple<int, int>, double> trianglePointsDictionary = new Dictionary<Tuple<int, int>, double>();
        static private double averageValue;

        static public SharpDx.PointGeometryModel3D Interpolate(MeshGeometry3D modelMesh, IEnumerable<Sensor> sensorsDataList)
        {
            SharpDx.GroupModel3D interpGroupModel = null;
            SharpDx.PointGeometryModel3D PointModel = null;

            if (sensorsDataList.Count() > 1)
            {
                interpGroupModel = new SharpDx.GroupModel3D();

                BuildDictionary(modelMesh);

                Dictionary<Tuple<int, int>, double> sensorDictionary = FillSensorDataDictionary(sensorsDataList);
                Dictionary<Tuple<int, int>, double> ppDictionary = PreProcessing(sensorDictionary);
                SharpDx.Core.Vector3Collection pointsCollection = StartInterpolation2(ppDictionary, sensorDictionary);

                PointModel = new SharpDx.PointGeometryModel3D();
                SharpDx.Core.Vector3Collection ppp = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
                SharpDx.Core.IntCollection indexs = new SharpDx.Core.IntCollection();
                SharpDx.Core.Color4Collection colors = new SharpDx.Core.Color4Collection();
                SharpDx.PointGeometry3D Points = new SharpDx.PointGeometry3D();

                for (int i = 0; i < pointsCollection.Count; i++)
                {
                    PointModel = new SharpDx.PointGeometryModel3D();
                    Points = new SharpDx.PointGeometry3D();
                    ppp = new SharpDx.Core.Vector3Collection();
                    indexs = new SharpDx.Core.IntCollection();

                    ppp.Add(pointsCollection[i]);
                    indexs.Add(0);

                    Points.Positions = ppp;
                    Points.Indices = indexs;

                    PointModel.Geometry = Points;

                    Color asd = GetHeatMapColor(pointsCollection[i].Z, -1.0f, 1.0f);

                    PointModel.Color = new SharpDX.Color(asd.R, asd.G, asd.B);
                    PointModel.Size = new System.Windows.Size(100, 100);

                    interpGroupModel.Children.Add(PointModel);
                }
            }

            return PointModel;
        }

        static private void BuildDictionary(MeshGeometry3D mesh)
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

                PointsOfTriangle(p0, p1, p2);
            }
        }

        static public Dictionary<Tuple<int, int>, double> FillSensorDataDictionary(IEnumerable<Sensor> sensorsDataList)
        {
            Dictionary<Tuple<int, int>, double> sensorDictionary = new Dictionary<Tuple<int, int>, double>();

            // For each sensor in list, get the coordinates to preprocessing
            foreach (Sensor sd in sensorsDataList)
            {
                int x = Convert.ToInt32(Math.Round(sd.X));
                int y = Convert.ToInt32(Math.Round(sd.Y));

                Tuple<int, int> key = new Tuple<int, int>(x, y);

                sensorDictionary.Add(key, sd.Values.Last().Value);
                trianglePointsDictionary[key] = sd.Values.Last().Value; //Update the coordinate of the sensors with the real value
            }

            return sensorDictionary;
        }

        static public Dictionary<Tuple<int, int>, double> PreProcessing(Dictionary<Tuple<int, int>, double> sensorDictionary)
        {

            Dictionary<Tuple<int, int>, double> ndic = new Dictionary<Tuple<int, int>, double>();

            foreach (KeyValuePair<Tuple<int, int>, double> v in trianglePointsDictionary)
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

        private static void PointsOfTriangle(Point3D p0, Point3D p1, Point3D p2)
        {
            int maxX = Convert.ToInt32(Math.Max(p0.X, Math.Max(p1.X, p2.X)));
            int minX = Convert.ToInt32(Math.Min(p0.X, Math.Min(p1.X, p2.X)));
            int maxY = Convert.ToInt32(Math.Max(p0.Y, Math.Max(p1.Y, p2.Y)));
            int minY = Convert.ToInt32(Math.Min(p0.Y, Math.Min(p1.Y, p2.Y)));

            Point3D vs1 = new Point3D(p1.X - p0.X, p1.Y - p0.Y, 0);
            Point3D vs2 = new Point3D(p2.X - p0.X, p2.Y - p0.Y, 0);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Point3D q = new Point3D(x - p0.X, y - p0.Y, 0);

                    double s = (double)CrossProduct(q, vs2) / CrossProduct(vs1, vs2);
                    double t = (double)CrossProduct(vs1, q) / CrossProduct(vs1, vs2);
                    if ((s >= 0) && (t >= 0) && (s + t <= 1))
                    {
                        Tuple<int, int> key = new Tuple<int, int>(x, y);

                        if (!trianglePointsDictionary.ContainsKey(key))
                        {
                            trianglePointsDictionary.Add(key, Math.Floor(q.Z));
                        }
                    }
                }
            }
        }

        // using one sensor
        static private SharpDx.Core.Vector3Collection StartInterpolation2(Dictionary<Tuple<int, int>, double> ppDictionary, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {
            Dictionary<Tuple<int, int>, double> resultDic = new Dictionary<Tuple<int, int>, double>();

            averageValue = -0.01957;

            SharpDx.Core.Vector3Collection vecCol = new SharpDx.Core.Vector3Collection();

            foreach (KeyValuePair<Tuple<int, int>, double> item in ppDictionary)
            {
                int x = item.Key.Item1;
                int y = item.Key.Item2;

                if (sensorDictionary.ContainsKey(item.Key))
                {
                    resultDic.Add(item.Key, sensorDictionary[item.Key]);
                    vecCol.Add(new SharpDX.Vector3(item.Key.Item1, item.Key.Item2, DoubleToFloat(sensorDictionary[item.Key]))); // (x, y, value)

                    continue;
                }

                // Returns the closest point
                Tuple<int, int> neighbor = GetNeighboringPoints2(x, y, sensorDictionary);

                // Check the Qs points
                List<Tuple<int, int>> qs = CheckPoints(x, y, neighbor.Item1, neighbor.Item2);

                Tuple<int, int> q11 = qs[0];
                Tuple<int, int> q12 = qs[1];
                Tuple<int, int> q21 = qs[2];
                Tuple<int, int> q22 = qs[3];


                double q11Value, q12Value, q21Value, q22Value;
                //Q11
                //if (resultDic.ContainsKey(q11))
                //{
                //    q11Value = resultDic[q11];
                //}
                //else
                if (ppDictionary.ContainsKey(q11))
                {
                    q11Value = ppDictionary[q11];

                }
                else
                {
                    q11Value = averageValue;
                }

                ////Q12
                //if (resultDic.ContainsKey(q12))
                //{
                //    q12Value = resultDic[q12];
                //}
                //else
                if (ppDictionary.ContainsKey(q12))
                {
                    q12Value = ppDictionary[q12];
                }
                else
                {
                    q12Value = averageValue;
                }

                ////Q21
                //if (resultDic.ContainsKey(q21))
                //{
                //    q21Value = resultDic[q21];
                //}
                //else
                if (ppDictionary.ContainsKey(q21))
                {
                    q21Value = ppDictionary[q21];
                }
                else
                {
                    q21Value = averageValue;
                }

                ////Q22
                //if (resultDic.ContainsKey(q22))
                //{
                //    q22Value = resultDic[q22];
                //}
                //else
                if (ppDictionary.ContainsKey(q22))
                {
                    q22Value = ppDictionary[q22];
                }
                else
                {
                    q22Value = averageValue;
                }

                double ret = BilinearInterpolation(q11Value, q12Value, q21Value, q22Value, q11.Item1, q22.Item1, q11.Item2, q22.Item2, x, y);

                vecCol.Add(new SharpDX.Vector3(item.Key.Item1, item.Key.Item2, DoubleToFloat(ret)));

                resultDic.Add(item.Key, ret);
                //ndic[item.Key] = ret;                   
            }
            return vecCol;
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

        private static List<Tuple<int, int>> CheckPoints(int x, int y, int qx, int qy)
        {
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>();

            Tuple<int, int> q11 = null;
            Tuple<int, int> q22 = null;

            Tuple<int, int> q12 = null;
            Tuple<int, int> q21 = null;

            //Find Q11 and sensor is Q22
            if (x < qx && y < qy)
            {
                q11 = new Tuple<int, int>(x - 1, y - 1);
                q22 = new Tuple<int, int>(qx, qy);

                q12 = new Tuple<int, int>(x - 1, qy);
                q21 = new Tuple<int, int>(qx, y - 1);

            }
            //Find Q22 and sensor is Q11
            else if (x > qx && y > qy)
            {
                q11 = new Tuple<int, int>(qx, qy);
                q22 = new Tuple<int, int>(x + 1, y + 1);

                q12 = new Tuple<int, int>(qx, y + 1);
                q21 = new Tuple<int, int>(x + 1, qy);
            }
            // Sensor is Q21 and find Q12
            else if (x < qx && y > qy)
            {
                q21 = new Tuple<int, int>(qx, qy);
                q12 = new Tuple<int, int>(x - 1, y + 1);

                q11 = new Tuple<int, int>(x - 1, qy);
                q22 = new Tuple<int, int>(qx, y + 1);
            }
            // Sensor is Q12 and find Q21
            else if (x > qx && y < qy)
            {
                q12 = new Tuple<int, int>(qx, qy);
                q21 = new Tuple<int, int>(x + 1, y - 1);

                q22 = new Tuple<int, int>(x + 1, qy);
                q11 = new Tuple<int, int>(qx, y - 1);
            }
            else if (x == qx && y < qy)
            {
                q12 = new Tuple<int, int>(qx, qy);
                q11 = new Tuple<int, int>(x, y - 1);

                q22 = new Tuple<int, int>(x + 1, qy);
                q21 = new Tuple<int, int>(x + 1, y - 1);
            }

            else if (x == qx && y > qy)
            {
                q11 = new Tuple<int, int>(qx, qy);
                q12 = new Tuple<int, int>(x, y + 1);

                q21 = new Tuple<int, int>(x + 1, qy);
                q22 = new Tuple<int, int>(x + 1, y + 1);
            }
            else if (y == qy && x < qx)
            {
                q12 = new Tuple<int, int>(x - 1, qy);
                q22 = new Tuple<int, int>(qx, qy);

                q11 = new Tuple<int, int>(x - 1, y - 1);
                q21 = new Tuple<int, int>(qx, y - 1);
            }
            else if (y == qy && x > qx)
            {
                q22 = new Tuple<int, int>(x + 1, qy);
                q12 = new Tuple<int, int>(qx, qy);

                q11 = new Tuple<int, int>(qx, y - 1);
                q21 = new Tuple<int, int>(x + 1, y - 1);
            }
            else
            {

            }

            ret.Add(q11);
            ret.Add(q12);
            ret.Add(q21);
            ret.Add(q22);

            return ret;
        }

        public static double BilinearInterpolation(double q11, double q12, double q21, double q22, float x1, float x2, float y1, float y2, float x, float y)
        {
            float x2x1, y2y1, x2x, y2y, yy1, xx1;
            x2x1 = x2 - x1;
            y2y1 = y2 - y1;
            x2x = x2 - x;
            y2y = y2 - y;
            yy1 = y - y1;
            xx1 = x - x1;
            return 1.0 / (x2x1 * y2y1) * (q11 * x2x * y2y + q21 * xx1 * y2y + q12 * x2x * yy1 + q22 * xx1 * yy1);
        }

        public static float DoubleToFloat(double dValue)
        {
            if (float.IsPositiveInfinity(Convert.ToSingle(dValue)))
            {
                return float.MaxValue;
            }
            if (float.IsNegativeInfinity(Convert.ToSingle(dValue)))
            {
                return float.MinValue;
            }
            return Convert.ToSingle(dValue);
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

