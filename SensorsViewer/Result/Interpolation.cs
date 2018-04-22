// <copyright file="Interpolation.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Result
{    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using SensorsViewer.Result.Astar;
    using SensorsViewer.SensorOption;

    using SharpGL.SceneGraph;

    static public class Interpolation
    {
        static private Dictionary<Tuple<int, int>, double> trianglePointsDictionary = new Dictionary<Tuple<int, int>, double>();
        static private double averageValue;
        static private MyPathNode[,] grid;
        static private int offsetX;
        static private int offsetY;
        static private int c = 0;

        static public Vertex[] Interpolate(MeshGeometry3D modelMesh, IEnumerable<Sensor> sensorsDataList)
        {

            c = 0;
            grid = new MyPathNode[Convert.ToInt32(modelMesh.Bounds.SizeX), Convert.ToInt32(modelMesh.Bounds.SizeY)];
            offsetX = Convert.ToInt32(modelMesh.Bounds.X);
            offsetY = Convert.ToInt32(modelMesh.Bounds.Y);

            Vertex[] vertices = null;

            if (sensorsDataList.Count() > 1)
            {
                Dictionary<Tuple<int, int>, double> sensorDictionary = FillSensorDataDictionary(sensorsDataList);

                BuildDictionary(modelMesh, sensorDictionary);
                BuildGrid(Convert.ToInt32(modelMesh.Bounds.SizeX), Convert.ToInt32(modelMesh.Bounds.SizeY));

                //Dictionary<Tuple<int, int>, double> sensorDictionary = FillSensorDataDictionary(sensorsDataList);

                CreateFakeSensors(sensorsDataList, sensorDictionary);
                Dictionary<Tuple<int, int>, double> ppDictionary = PreProcessing(sensorDictionary);
                vertices = StartInterpolation2(ppDictionary, sensorDictionary);
            }

            return vertices;
        }

        static private Dictionary<Tuple<int, int>, double> FindBug(Dictionary<Tuple<int, int>, double> ppDictionary)
        {
            Dictionary<Tuple<int, int>, double> ppDictionary2 = new Dictionary<Tuple<int, int>, double>();

            foreach (KeyValuePair<Tuple<int, int>, double> kvp in ppDictionary)
            {

                int x = kvp.Key.Item1;
                int y = kvp.Key.Item2;

                //ppDictionary2[kvp.Key] = kvp.Value;
                Tuple<int, int> p1 = new Tuple<int, int>(x+1, y);
                Tuple<int, int> p2 = new Tuple<int, int>(x-1, y);
                Tuple<int, int> p3 = new Tuple<int, int>(x, y+1);
                Tuple<int, int> p4 = new Tuple<int, int>(x, y-1);
                Tuple<int, int> p5 = new Tuple<int, int>(x+1, y+1);
                Tuple<int, int> p6 = new Tuple<int, int>(x+1, y-1);
                Tuple<int, int> p7 = new Tuple<int, int>(x-1, y+1);
                Tuple<int, int> p8 = new Tuple<int, int>(x-1, y-1);

                if (ppDictionary.ContainsKey(p1) && ppDictionary.ContainsKey(p2) && ppDictionary.ContainsKey(p3) &&
                    ppDictionary.ContainsKey(p4) && ppDictionary.ContainsKey(p5) && ppDictionary.ContainsKey(p6) &&
                    ppDictionary.ContainsKey(p7) && ppDictionary.ContainsKey(p8))
                {
                    double min = Math.Min(ppDictionary[p1], Math.Min(ppDictionary[p2], Math.Min(ppDictionary[p3],
                        Math.Min(ppDictionary[p4], Math.Min(ppDictionary[p5], Math.Min(ppDictionary[p6],
                        Math.Min(ppDictionary[p7], ppDictionary[p8])))))));

                    double max = Math.Max(ppDictionary[p1], Math.Max(ppDictionary[p2], Math.Max(ppDictionary[p3],
                        Math.Max(ppDictionary[p4], Math.Max(ppDictionary[p5], Math.Max(ppDictionary[p6],
                        Math.Max(ppDictionary[p7], ppDictionary[p8])))))));

                    ppDictionary2[kvp.Key] = (ppDictionary[p1] + ppDictionary[p2] + ppDictionary[p3] + ppDictionary[p4]
                        + ppDictionary[p5] + ppDictionary[p6] + ppDictionary[p7] + ppDictionary[p8]) / 8;
                }

            }
            return ppDictionary2;

        }

        static private void BuildDictionary(MeshGeometry3D mesh, Dictionary<Tuple<int, int>, double> sensorDictionary)
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

                PointsOfTriangle(p0, p1, p2, sensorDictionary);
            }

            foreach (KeyValuePair<Tuple<int, int>, double> sd in sensorDictionary)
            {
                int x = Convert.ToInt32((sd.Key.Item1));
                int y = Convert.ToInt32((sd.Key.Item2));

                Tuple<int, int> key = new Tuple<int, int>(x, y);
                trianglePointsDictionary[key] = sd.Value;
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

        private static void PointsOfTriangle(Point3D p0, Point3D p1, Point3D p2, Dictionary<Tuple<int, int>, double> sensorDictionary)
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

                            double d1, d2;
                            List<Tuple<int, int>> neighbors = GetNeighboringPoints(x, y, sensorDictionary, out d1, out d2);
                            d1 = 1 / d1;
                            d2 = 1 / d2;
                            double value = ((d1 * (sensorDictionary[neighbors.ElementAt(0)]) + d2 * (sensorDictionary[neighbors.ElementAt(1)])) / (d1 + d2));

                            trianglePointsDictionary.Add(key, value);
                        }
                    }
                }
            }
        }

        static public Dictionary<Tuple<int, int>, double> PreProcessing(Dictionary<Tuple<int, int>, double> sensorDictionary)
        {

            Dictionary<Tuple<int, int>, double> ndic = new Dictionary<Tuple<int, int>, double>();

            foreach (KeyValuePair<Tuple<int, int>, double> v in trianglePointsDictionary)
            {
                int nx = v.Key.Item1;
                int ny = v.Key.Item2;

                if (sensorDictionary.ContainsKey(v.Key))
                {
                    ndic[v.Key] = sensorDictionary[v.Key];
                    continue;
                }

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

        // Using one sensor
        static private Vertex[] StartInterpolation2(Dictionary<Tuple<int, int>, double> ppDictionary, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {
            Dictionary<Tuple<int, int>, double> resultDic = new Dictionary<Tuple<int, int>, double>();

            averageValue = 0;

            var vertices = new  Vertex[ppDictionary.Count];

            foreach (KeyValuePair<Tuple<int, int>, double> item in ppDictionary)
            {
                int x = item.Key.Item1;
                int y = item.Key.Item2;

                if (sensorDictionary.ContainsKey(item.Key))
                {
                    resultDic.Add(item.Key, sensorDictionary[item.Key]);
                    vertices[c].X = item.Key.Item1;
                    vertices[c].Y = item.Key.Item2;
                    vertices[c++].Z = DoubleToFloat(sensorDictionary[item.Key]); // (x, y, value)

                    continue;
                }

                //// Returns the closest point
                //Tuple<int, int> neighbor = GetNeighboringPoints2(x, y, sensorDictionary);

                //// Check the Qs points
                //List<Tuple<int, int>> qs = CheckPoints(x, y, neighbor.Item1, neighbor.Item2);

                //Tuple<int, int> q11 = qs[0];
                //Tuple<int, int> q12 = qs[1];
                //Tuple<int, int> q21 = qs[2];
                //Tuple<int, int> q22 = qs[3];


                //double q11Value, q12Value, q21Value, q22Value;
                ////Q11
                ////if (resultDic.ContainsKey(q11))
                ////{
                ////    q11Value = resultDic[q11];
                ////}
                ////else
                //if (ppDictionary.ContainsKey(q11))
                //{
                //    q11Value = ppDictionary[q11];

                //}
                //else
                //{
                //    q11Value = averageValue;
                //}

                //////Q12
                ////if (resultDic.ContainsKey(q12))
                ////{
                ////    q12Value = resultDic[q12];
                ////}
                ////else
                //if (ppDictionary.ContainsKey(q12))
                //{
                //    q12Value = ppDictionary[q12];
                //}
                //else
                //{
                //    q12Value = averageValue;
                //}

                //////Q21
                ////if (resultDic.ContainsKey(q21))
                ////{
                ////    q21Value = resultDic[q21];
                ////}
                ////else
                //if (ppDictionary.ContainsKey(q21))
                //{
                //    q21Value = ppDictionary[q21];
                //}
                //else
                //{
                //    q21Value = averageValue;
                //}

                //////Q22
                ////if (resultDic.ContainsKey(q22))
                ////{
                ////    q22Value = resultDic[q22];
                ////}
                ////else
                //if (ppDictionary.ContainsKey(q22))
                //{
                //    q22Value = ppDictionary[q22];
                //}
                //else
                //{
                //    q22Value = averageValue;
                //}

                //double ret = BilinearInterpolation(q11Value, q12Value, q21Value, q22Value, q11.Item1, q22.Item1, q11.Item2, q22.Item2, x, y);

                vertices[c].X = item.Key.Item1;
                vertices[c].Y = item.Key.Item2;
                vertices[c++].Z = DoubleToFloat(item.Value);

                resultDic.Add(item.Key, item.Value);
            }

            return vertices;
        }

        static private List<Tuple<int, int>> GetNeighboringPoints(int x, int y, Dictionary<Tuple<int, int>, double> sensorDictionary, out double mCoord1, out double mCoord2)
        {

            List<Tuple<int, int>> ret = new List<Tuple<int, int>>();

            double min1 = double.MaxValue, min2 = double.MaxValue;
            ret.Add(sensorDictionary.ElementAt(0).Key);
            ret.Add(sensorDictionary.ElementAt(1).Key);

            foreach (KeyValuePair<Tuple<int, int>, double> v in sensorDictionary)
            {
                int nx = v.Key.Item1;
                int ny = v.Key.Item2;

                // HERE
                //if (y * ny < 0)
                //    continue;

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

        static public Color GetHeatMapColor(double value, double min_value, double max_value)
        {
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

        static private void CreateFakeSensors(IEnumerable<Sensor> sensorsDataList, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {

            List<Sensor> fakeSensors = new List<Sensor>();
            Sensor closestSensor = new Sensor(); Sensor sndClosestSensor = new Sensor();

            for (int i = 0; i < sensorsDataList.Count(); i++)
            {
                for (int j = i + 1; j < sensorsDataList.Count(); j++)
                {
                    // TODO: ElementAt is slow
                    double x1 = sensorsDataList.ElementAt(i).X;
                    double y1 = sensorsDataList.ElementAt(i).Y;

                    double x2 = sensorsDataList.ElementAt(j).X;
                    double y2 = sensorsDataList.ElementAt(j).Y;

                    int newX = (int) (x1 + x2) / 2;
                    int newY = (int) (y1 + y2) / 2;

                    Tuple<int, int> key = new Tuple<int, int>(newX, newY);

                    int min1 = int.MaxValue, min2 = int.MaxValue;

                    if (trianglePointsDictionary.ContainsKey(key))
                    {
                        Point rndSensorPnt = new Point(key.Item1 - offsetX, key.Item2 - offsetY);

                        foreach (Sensor sensor in sensorsDataList)
                        {
                            //Get the distance between randomKey and 7 sensors and return the minumum
                            Point sensorPnt = new Point(Convert.ToInt32(sensor.X) - offsetX, Convert.ToInt32(sensor.Y) - offsetY);

                            MySolver<MyPathNode, Object> aStar = new MySolver<MyPathNode, Object>(grid);
                            IEnumerable<MyPathNode> path = aStar.Search(rndSensorPnt, sensorPnt, null);

                            int pathLen = path.Count();

                            if (pathLen < min1)
                            {
                                min2 = min1;
                                min1 = pathLen;

                                sndClosestSensor = closestSensor;
                                closestSensor = sensor;
                            }
                            else if (pathLen < min2)
                            {
                                min2 = pathLen;
                                sndClosestSensor = sensor;
                            }
                        }

                        Sensor rstSensor = new Sensor("Fake Sensor", rndSensorPnt.X + offsetX, rndSensorPnt.Y + offsetY, 0);

                        double d1 = Math.Sqrt(Math.Pow(rstSensor.X - closestSensor.X, 2) + Math.Pow(rstSensor.Y - closestSensor.Y, 2));
                        double d2 = Math.Sqrt(Math.Pow(rstSensor.X - sndClosestSensor.X, 2) + Math.Pow(rstSensor.Y - sndClosestSensor.Y, 2));

                        d1 = 1 / d1;
                        d2 = 1 / d2;

                        double value = ((d1 * closestSensor.Values.Last().Value) + d2 * sndClosestSensor.Values.Last().Value) / (d1 + d2);

                        rstSensor.Values.Add(new SensorValue(value));

                        fakeSensors.Add(rstSensor);

                    }
                }
            }

            foreach (Sensor s in fakeSensors)
            {
                Tuple<int, int> key = new Tuple<int, int>(Convert.ToInt32(Math.Round(s.X)), Convert.ToInt32(Math.Round(s.Y)));

                sensorDictionary.Add(key, s.Values.Last().Value);
            }
        }

        static private void CreateRandomSensors(IEnumerable<Sensor> sensorsDataList, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {

            List<Tuple<int, int>> keyList = new List<Tuple<int, int>>(trianglePointsDictionary.Keys);

            List<Sensor> fakeSensors = new List<Sensor>();
            Random rand = new Random();
            Sensor closestSensor = new Sensor(); Sensor sndClosestSensor = new Sensor();

            for (int i = 0; i < 30; i++)
            {
                int min1 = int.MaxValue;
                int min2 = int.MaxValue;

                Tuple<int, int> randomKey = keyList[rand.Next(keyList.Count)];
                Point rndSensorPnt = new Point(randomKey.Item1 - offsetX, randomKey.Item2 - offsetY);

                foreach (Sensor sensor in sensorsDataList)
                {
                    //Get the distance between randomKey and 7 sensors and return the minumum
                    Point sensorPnt = new Point(Convert.ToInt32(sensor.X) - offsetX, Convert.ToInt32(sensor.Y) - offsetY);

                    MySolver<MyPathNode, Object> aStar = new MySolver<MyPathNode, Object>(grid);
                    IEnumerable<MyPathNode> path = aStar.Search(rndSensorPnt, sensorPnt, null);

                    int pathLen = path.Count();

                    if (pathLen < min1)
                    {
                        min2 = min1;
                        min1 = pathLen;

                        sndClosestSensor = closestSensor;
                        closestSensor = sensor;
                    }
                    else if (pathLen < min2)
                    {
                        min2 = pathLen;
                        sndClosestSensor = sensor;
                    }
                }

                Sensor rstSensor = new Sensor("Fake Sensor", rndSensorPnt.X + offsetX, rndSensorPnt.Y + offsetY, 0);

                double d1 = Math.Sqrt(Math.Pow(rstSensor.X - closestSensor.X, 2) + Math.Pow(rstSensor.Y - closestSensor.Y, 2));
                double d2 = Math.Sqrt(Math.Pow(rstSensor.X - sndClosestSensor.X, 2) + Math.Pow(rstSensor.Y - sndClosestSensor.Y, 2));

                d1 = 1 / d1;
                d2 = 1 / d2;

                double value = ((d1 * closestSensor.Values.Last().Value) + d2 * sndClosestSensor.Values.Last().Value) / (d1 + d2);

                rstSensor.Values.Add(new SensorValue(value));

                fakeSensors.Add(rstSensor);
            }

            foreach (Sensor s in fakeSensors)
            {
                Tuple<int, int> key = new Tuple<int, int>(Convert.ToInt32(Math.Round(s.X)), Convert.ToInt32(Math.Round(s.Y)));

                sensorDictionary.Add(key, s.Values[0].Value);
            }
        }

        static void BuildGrid(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Boolean isWall = false;

                    Tuple<int, int> tuple = new Tuple<int, int>(x - 592, y - 210);
                    if (!trianglePointsDictionary.ContainsKey(tuple))
                    {
                        isWall = true;

                    }

                    grid[x, y] = new MyPathNode()
                    {
                        IsWall = isWall,
                        X = x,
                        Y = y,
                    };
                }
            }
        }

        static private double CrossProduct(Point3D p1, Point3D p2)
        {
            return p1.X * p2.Y - p1.Y * p2.X;
        }

    }
}

