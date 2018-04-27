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

    public class Interpolation
    {
        private List<Tuple<Sensor, Sensor, Sensor>> fakeSensors;
        Dictionary<Tuple<int, int>, double> sensorDictionary;

        private Dictionary<Tuple<int, int>, double> trianglePointsDictionary = new Dictionary<Tuple<int, int>, double>();
        private Dictionary<Tuple<int, int>, double> dividedDic = new Dictionary<Tuple<int, int>, double>();
        private Dictionary<Tuple<int, int>, double> modelTrianglePoints = new Dictionary<Tuple<int, int>, double>();

        private List<Point3D> listPoint3d = new List<Point3D>();

        private MyPathNode[,] grid;
        private MyPathNode[,] grid2;
        private MyPathNode[,] grid3;
        private double averageValue;

        private int offsetX;
        private int offsetY;
        private int c;

        public Interpolation(MeshGeometry3D modelMesh, IEnumerable<Sensor> sensorsDataList)
        {
            c = 0;

            grid3 = new MyPathNode[Convert.ToInt32(modelMesh.Bounds.SizeX / 10)+1, Convert.ToInt32(modelMesh.Bounds.SizeY / 10)+1];

            offsetX = Convert.ToInt32(modelMesh.Bounds.X);
            offsetY = Convert.ToInt32(modelMesh.Bounds.Y);

            if (sensorsDataList.Count() > 1 )
            {
                sensorDictionary = FillSensorDataDictionary2(sensorsDataList);
                BuildDictionary2(modelMesh, sensorDictionary);
                BuildGrid2(Convert.ToInt32(modelMesh.Bounds.SizeX / 10), Convert.ToInt32(modelMesh.Bounds.SizeY / 10));
                fakeSensors = CreateRandomSensors4(sensorsDataList);
            }
        }

        public Vertex[] Interpolate(MeshGeometry3D modelMesh, IEnumerable<Sensor> sensorsDataList)
        {

            c = 0;
            grid = new MyPathNode[Convert.ToInt32(modelMesh.Bounds.SizeX), Convert.ToInt32(modelMesh.Bounds.SizeY)];
            grid2 = new MyPathNode[Convert.ToInt32(modelMesh.Bounds.SizeX/10), Convert.ToInt32(modelMesh.Bounds.SizeY/10)];

            offsetX = Convert.ToInt32(modelMesh.Bounds.X);
            offsetY = Convert.ToInt32(modelMesh.Bounds.Y);

            Vertex[] vertices = null;

            if (sensorsDataList.Count() > 1)
            {
                Dictionary<Tuple<int, int>, double> sensorDictionary = FillSensorDataDictionary(sensorsDataList);

                //Test(modelMesh, sensorDictionary);

                BuildDictionary(modelMesh, sensorDictionary);
                BuildGrid(Convert.ToInt32(modelMesh.Bounds.SizeX), Convert.ToInt32(modelMesh.Bounds.SizeY));

                CreateRandomSensors3(sensorsDataList, sensorDictionary);

                //CreateRandomSensors2(sensorsDataList, sensorDictionary);
                //CreateRandomSensors(sensorsDataList, sensorDictionary);
                Dictionary<Tuple<int, int>, double> ppDictionary = PreProcessing(sensorDictionary);
                //Dictionary<Tuple<int, int>, double> ppDictionary2 = FindBug(ppDictionary);
                vertices = StartInterpolation(ppDictionary, sensorDictionary);
            }

            return vertices;
        }


        public Vertex[] Interpolate2(MeshGeometry3D modelMesh, IEnumerable<Sensor> sensorsDataList)
        {
          
            Vertex[] vertices = null;

            DateTime lastMeasureTime = DateTime.Now;

            if (sensorsDataList.Count() > 1)
            {
                sensorDictionary = FillSensorDataDictionary2(sensorsDataList);
                ReCalculateFakeSensors(fakeSensors, sensorDictionary);
                vertices = PreProcessing2(sensorDictionary);                
            }

            TimeSpan asd = DateTime.Now.Subtract(lastMeasureTime);

            return vertices;
        }        

        private void BuildDictionary(MeshGeometry3D mesh, Dictionary<Tuple<int, int>, double> sensorDictionary)
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
                PointsOfTriangle2(p0, p1, p2, sensorDictionary);

            }

            foreach (KeyValuePair<Tuple<int, int>, double> sd in sensorDictionary)
            {
                int x = Convert.ToInt32((sd.Key.Item1));
                int y = Convert.ToInt32((sd.Key.Item2));

                Tuple<int, int> key = new Tuple<int, int>(x, y);
                trianglePointsDictionary[key] = sd.Value;
            }
        }

        void PointsOfTriangle(Point3D p0, Point3D p1, Point3D p2, Dictionary<Tuple<int, int>, double> sensorDictionary)
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
                            Tuple<int, int>[] neighbors = GetNeighboringPoints(x, y, sensorDictionary, out d1, out d2);
                            d1 = 1 / d1;
                            d2 = 1 / d2;
                            double value = ((d1 * (sensorDictionary[neighbors[0]]) + d2 * (sensorDictionary[neighbors[1]])) / (d1 + d2));

                            trianglePointsDictionary.Add(key, value);

                            Tuple<int, int> key2 = new Tuple<int, int>(x/10, y/10);

                            if (!dividedDic.ContainsKey(key2))
                            {
                                dividedDic.Add(key2, value);
                            }
                        }
                    }
                }
            }
        }        

        public Dictionary<Tuple<int, int>, double> FillSensorDataDictionary(IEnumerable<Sensor> sensorsDataList)
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

        public Dictionary<Tuple<int, int>, double> PreProcessing(Dictionary<Tuple<int, int>, double> sensorDictionary)
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
                Tuple<int, int>[] neighbors = GetNeighboringPoints(nx, ny, sensorDictionary, out d1, out d2);

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
        private Vertex[] StartInterpolation(Dictionary<Tuple<int, int>, double> ppDictionary, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {
            Dictionary<Tuple<int, int>, double> resultDic = new Dictionary<Tuple<int, int>, double>();

            averageValue = 0;

            var vertices = new  Vertex[ppDictionary.Count];

            foreach (KeyValuePair<Tuple<int, int>, double> item in ppDictionary)
            {
                int x = item.Key.Item1;
                int y = item.Key.Item2;

                //if (sensorDictionary.ContainsKey(item.Key))
                //{
                //    resultDic.Add(item.Key, sensorDictionary[item.Key]);
                //    vertices[c].X = item.Key.Item1;
                //    vertices[c].Y = item.Key.Item2;
                //    vertices[c++].Z = DoubleToFloat(sensorDictionary[item.Key]); // (x, y, value)

                //    continue;
                //}

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

        private Tuple<int, int>[] GetNeighboringPoints(int x, int y, Dictionary<Tuple<int, int>, double> sensorDictionary, out double mCoord1, out double mCoord2)
        {

            Tuple<int, int>[] ret = new Tuple<int, int>[2];

            double min1 = double.MaxValue, min2 = double.MaxValue;

            ret[0] = sensorDictionary.ElementAt(0).Key;
            ret[1] = sensorDictionary.ElementAt(1).Key;

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

        private Tuple<int, int> GetNeighboringPoints2(int x, int y, Dictionary<Tuple<int, int>, double> newDictionary)
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

        private List<Tuple<int, int>> CheckPoints(int x, int y, int qx, int qy)
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

        public double BilinearInterpolation(double q11, double q12, double q21, double q22, float x1, float x2, float y1, float y2, float x, float y)
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

        public float DoubleToFloat(double dValue)
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

        private void CreateFakeSensors(IEnumerable<Sensor> sensorsDataList, Dictionary<Tuple<int, int>, double> sensorDictionary)
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

        private void CreateRandomSensors(IEnumerable<Sensor> sensorsDataList, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {

            List<Tuple<int, int>> keyList = new List<Tuple<int, int>>(trianglePointsDictionary.Keys);

            List<Sensor> fakeSensors = new List<Sensor>();
            Random rand = new Random();
            Sensor closestSensor = new Sensor(); Sensor sndClosestSensor = new Sensor();

            for (int i = 0; i < 40; i++)
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

        private void CreateRandomSensors2(IEnumerable<Sensor> sensorsDataList, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {

            List<Tuple<int, int>> keyList = new List<Tuple<int, int>>(dividedDic.Keys);

            List<Sensor> fakeSensors = new List<Sensor>();
            Random rand = new Random();
            Sensor closestSensor = new Sensor(); Sensor sndClosestSensor = new Sensor();

            for (int i = 0; i < 100; i++)
            {
                int min1 = int.MaxValue;
                int min2 = int.MaxValue;

                Tuple<int, int> randomKey = keyList[rand.Next(keyList.Count)];
                
                Point rndSensorPnt = new Point(randomKey.Item1 - offsetX/10, randomKey.Item2 - offsetY/10);

                if (rndSensorPnt.X > 117 || rndSensorPnt.Y > 41)
                {
                    i = i - 1;
                    continue;
                }

                foreach (Sensor sensor in sensorsDataList)
                {
                    //Get the distance between randomKey and 7 sensors and return the minumum
                    Point sensorPnt = new Point(Convert.ToInt32(sensor.X/10) - offsetX/10, Convert.ToInt32(sensor.Y/10) - offsetY/10);
                    int pathLen = 0;
                    try
                    {
                        MySolver<MyPathNode, Object> aStar = new MySolver<MyPathNode, Object>(grid2);
                        IEnumerable<MyPathNode> path = aStar.Search(rndSensorPnt, sensorPnt, null);
                        pathLen = path.Count();
                    }
                    catch (Exception e)
                    {
                        var a = 1;
                    }                    

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

                Sensor rstSensor = new Sensor("Fake Sensor", rndSensorPnt.X*10 + offsetX, rndSensorPnt.Y*10 + offsetY, 0);

                double d1 = Math.Sqrt(Math.Pow(rstSensor.X - closestSensor.X, 2) + Math.Pow(rstSensor.Y - closestSensor.Y, 2));
                double d2 = Math.Sqrt(Math.Pow(rstSensor.X - sndClosestSensor.X, 2) + Math.Pow(rstSensor.Y - sndClosestSensor.Y, 2));

                d1 = 1 / d1;
                d2 = 1 / d2;

                double value = ((d1 * closestSensor.Values.Last().Value) + d2 * sndClosestSensor.Values.Last().Value) / (d1 + d2);

                rstSensor.Values.Add(new SensorValue(value));

                fakeSensors.Add(rstSensor);

                keyList.Remove(randomKey);
            }

            foreach (Sensor s in fakeSensors)
            {
                Tuple<int, int> key = new Tuple<int, int>(Convert.ToInt32(Math.Round(s.X)), Convert.ToInt32(Math.Round(s.Y)));

                sensorDictionary.Add(key, s.Values[0].Value);
            }
        }       

        private void BuildGrid(int width, int height)
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

            for (int x = 0; x < width/10; x++)
            {
                for (int y = 0; y < height/10; y++)
                {
                    Boolean isWall = false;

                    Tuple<int, int> tuple = new Tuple<int, int>(x - 592/10, y - 210/10);
                    if (!dividedDic.ContainsKey(tuple))
                    {
                        isWall = true;

                    }

                    grid2[x, y] = new MyPathNode()
                    {
                        IsWall = isWall,
                        X = x,
                        Y = y,
                    };
                }
            }
        }

        private double CrossProduct(Point3D p1, Point3D p2)
        {
            return p1.X * p2.Y - p1.Y * p2.X;
        }

        //******************************************************************************************************

        public Dictionary<Tuple<int, int>, double> FillSensorDataDictionary2(IEnumerable<Sensor> sensorsDataList)
        {
            Dictionary<Tuple<int, int>, double> sensorDictionary = new Dictionary<Tuple<int, int>, double>();

            // For each sensor in list, get the coordinates to preprocessing
            foreach (Sensor sd in sensorsDataList)
            {               
                int x = Convert.ToInt32(Math.Round(sd.X));
                int y = Convert.ToInt32(Math.Round(sd.Y));

                Tuple<int, int> key = new Tuple<int, int>(x, y);
                Tuple<int, int> key2 = new Tuple<int, int>(x / 10, y / 10);

                if (sd.Values.Count > 0)
                {
                    sensorDictionary.Add(key, sd.Values.Last().Value);
                    modelTrianglePoints[key2] = sd.Values.Last().Value; //Update the coordinate of the sensors with the real value
                }
                else
                {
                    sensorDictionary.Add(key, 0);
                    modelTrianglePoints[key2] = 0; //Update the coordinate of the sensors with the real value
                }
                
            }

            return sensorDictionary;
        }

        private void BuildDictionary2(MeshGeometry3D mesh, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int index0 = mesh.TriangleIndices[i];
                int index1 = mesh.TriangleIndices[i + 1];
                int index2 = mesh.TriangleIndices[i + 2];

                Point3D p0 = mesh.Positions[index0];
                Point3D p1 = mesh.Positions[index1];
                Point3D p2 = mesh.Positions[index2];

                p0.X = Math.Round(p0.X);
                p0.Y = Math.Round(p0.Y);
                p0.Z = Math.Round(p0.Z);

                p1.X = Math.Round(p1.X);
                p1.Y = Math.Round(p1.Y);
                p1.Z = Math.Round(p1.Z);

                p2.X = Math.Round(p2.X);
                p2.Y = Math.Round(p2.Y);
                p2.Z = Math.Round(p2.Z);

                PointsOfTriangle2(p0, p1, p2, sensorDictionary);

                listPoint3d.Add(new Point3D((int)p0.X, (int)p0.Y, 0));
                listPoint3d.Add(new Point3D((int)p1.X, (int)p1.Y, 0));
                listPoint3d.Add(new Point3D((int)p2.X, (int)p2.Y, 0));
            }
        }

        private void PointsOfTriangle2(Point3D p0, Point3D p1, Point3D p2, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {
            p0.X = p0.X / 10;
            p0.Y = p0.Y / 10;

            p1.X = p1.X / 10;
            p1.Y = p1.Y / 10;

            p2.X = p2.X / 10;
            p2.Y = p2.Y / 10;

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

                        if (!modelTrianglePoints.ContainsKey(key))
                        {                           
                              modelTrianglePoints.Add(key, 0);                            
                        }
                    }
                }
            }
        }

        void BuildGrid2(int width, int height)
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    Boolean isWall = false;

                    Tuple<int, int> tuple = new Tuple<int, int>(x + offsetX / 10, y + offsetY / 10);
                    if (!modelTrianglePoints.ContainsKey(tuple))
                    {
                        isWall = true;
                    }

                    grid3[x, y] = new MyPathNode()
                    {
                        IsWall = isWall,
                        X = x,
                        Y = y,
                    };
                }
            }
        }

        public Vertex[] PreProcessing2(Dictionary<Tuple<int, int>, double> sensorDictionary)
        {
            var vertices = new Vertex[listPoint3d.Count];
            int counter = 0;
          
            foreach (Point3D point in listPoint3d)
                {
                    int nx = (int)point.X;
                    int ny = (int)point.Y;

                    Tuple<int, int> tuple = new Tuple<int, int>(nx, ny);

                    if (sensorDictionary.ContainsKey(tuple))
                    {
                        vertices[counter].X = nx;
                        vertices[counter].Y = ny;
                        vertices[counter++].Z = DoubleToFloat(sensorDictionary[tuple]);
                        continue;
                    }

                    double d1, d2;
                    Tuple<int, int>[] neighbors = GetNeighboringPoints(nx, ny, sensorDictionary, out d1, out d2);

                    d1 = 1 / d1;
                    d2 = 1 / d2;

                    double var = ((d1 * (sensorDictionary[neighbors[0]]) + d2 * (sensorDictionary[neighbors[1]])) / (d1 + d2)); 

                    vertices[counter].X = nx;
                    vertices[counter].Y = ny;
                    vertices[counter++].Z = DoubleToFloat(var);

            }
           
            return vertices;
        }        

        private void CreateRandomSensors3(IEnumerable<Sensor> sensorsDataList, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {

            List<Tuple<int, int>> keyList = new List<Tuple<int, int>>(modelTrianglePoints.Keys);

            Random rand = new Random();
            Sensor closestSensor = new Sensor(); Sensor sndClosestSensor = new Sensor();

            for (int i = 0; i < 50; i++)
            {
                int min1 = int.MaxValue;
                int min2 = int.MaxValue;

                Tuple<int, int> randomKey = keyList[rand.Next(keyList.Count)];

                Point rndSensorPnt = new Point(randomKey.Item1 - offsetX / 10, randomKey.Item2 - offsetY / 10);

                foreach (Sensor sensor in sensorsDataList)
                {
                    //Get the distance between randomKey and 7 sensors and return the minumum
                    Point sensorPnt = new Point(Convert.ToInt32(sensor.X / 10 - offsetX / 10), Convert.ToInt32(sensor.Y / 10 - offsetY / 10));
                    int pathLen = 0;
               
                    MySolver<MyPathNode, Object> aStar = new MySolver<MyPathNode, Object>(grid3);
                    IEnumerable<MyPathNode> path = aStar.Search(rndSensorPnt, sensorPnt, null);
                    pathLen = path.Count();

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

                Sensor rstSensor = new Sensor("Fake Sensor", rndSensorPnt.X * 10 + offsetX, rndSensorPnt.Y * 10 + offsetY, 0);

                double d1 = Math.Sqrt(Math.Pow(rstSensor.X - closestSensor.X, 2) + Math.Pow(rstSensor.Y - closestSensor.Y, 2));
                double d2 = Math.Sqrt(Math.Pow(rstSensor.X - sndClosestSensor.X, 2) + Math.Pow(rstSensor.Y - sndClosestSensor.Y, 2));

                d1 = 1 / d1;
                d2 = 1 / d2;

                double value = (d1 * closestSensor.Values.Last().Value + d2 * sndClosestSensor.Values.Last().Value) / (d1 + d2);

                rstSensor.Values.Add(new SensorValue(value));

                Tuple<int, int> key = new Tuple<int, int>(Convert.ToInt32(rstSensor.X), Convert.ToInt32(rstSensor.Y));

                sensorDictionary.Add(key, value);

                keyList.Remove(randomKey);
            }          
        }

        private List<Tuple<Sensor, Sensor, Sensor>> CreateRandomSensors4(IEnumerable<Sensor> sensorsDataList)
        {

            List<Tuple<Sensor, Sensor, Sensor>> fakeSensors = new List<Tuple<Sensor, Sensor, Sensor>>();

            List<Tuple<int, int>> keyList = new List<Tuple<int, int>>(modelTrianglePoints.Keys);

            Random rand = new Random();
            Sensor closestSensor = new Sensor(); Sensor sndClosestSensor = new Sensor();

            for (int i = 0; i < 90; i++)
            {
                int min1 = int.MaxValue;
                int min2 = int.MaxValue;

                Tuple<int, int> randomKey = keyList[rand.Next(keyList.Count)];

                Point rndSensorPnt = new Point(randomKey.Item1 - offsetX / 10, randomKey.Item2 - offsetY / 10);

                foreach (Sensor sensor in sensorsDataList)
                {
                    //Get the distance between randomKey and 7 sensors and return the minumum
                    Point sensorPnt = new Point(Convert.ToInt32((sensor.X - offsetX) / 10), Convert.ToInt32((sensor.Y - offsetY) / 10));

                    int pathLen = 0;

                    try
                    {
                        MySolver<MyPathNode, Object> aStar = new MySolver<MyPathNode, Object>(grid3);
                        IEnumerable<MyPathNode> path = aStar.Search(rndSensorPnt, sensorPnt, null);
                        pathLen = path.Count();
                    }
                    catch (Exception e)
                    {
                        var a = 1;
                    }

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

                Sensor rstSensor = new Sensor("Fake Sensor", rndSensorPnt.X * 10 + offsetX, rndSensorPnt.Y * 10 + offsetY, 0);               

                Tuple<int, int> key = new Tuple<int, int>(Convert.ToInt32(rstSensor.X), Convert.ToInt32(rstSensor.Y));

                Tuple<Sensor, Sensor, Sensor> fk = new Tuple<Sensor, Sensor, Sensor>(rstSensor, closestSensor, sndClosestSensor);

                fakeSensors.Add(fk);

                keyList.Remove(randomKey);
            }

            return fakeSensors;
        }

        private void ReCalculateFakeSensors(List<Tuple<Sensor, Sensor, Sensor>> fakeSensors, Dictionary<Tuple<int, int>, double> sensorDictionary)
        {

            foreach (Tuple<Sensor, Sensor, Sensor> tp in fakeSensors)
            {
                Tuple<int, int> clstSensorTp = new Tuple<int, int>(Convert.ToInt32(tp.Item2.X), Convert.ToInt32(tp.Item2.Y));
                Tuple<int, int> sndClstSensorTp = new Tuple<int, int>(Convert.ToInt32(tp.Item3.X), Convert.ToInt32(tp.Item3.Y));

                Sensor rstSensor = tp.Item1;
                double closestSensorValue = sensorDictionary[clstSensorTp];
                double sndClosestSensorValue = sensorDictionary[sndClstSensorTp];

                double d1 = Math.Sqrt(Math.Pow(rstSensor.X - tp.Item2.X, 2) + Math.Pow(rstSensor.Y - tp.Item2.Y, 2));
                double d2 = Math.Sqrt(Math.Pow(rstSensor.X - tp.Item3.X, 2) + Math.Pow(rstSensor.Y - tp.Item3.Y, 2));

                d1 = 1 / d1;
                d2 = 1 / d2;

                double value = (d1 * closestSensorValue + d2 * sndClosestSensorValue) / (d1 + d2);

                Tuple<int, int> key = new Tuple<int, int>(Convert.ToInt32(rstSensor.X), Convert.ToInt32(rstSensor.Y));

                sensorDictionary.Add(key, value);
            }
        }
    }
}

