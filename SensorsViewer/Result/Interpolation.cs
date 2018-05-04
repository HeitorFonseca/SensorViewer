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

        private int offsetX;
        private int offsetY;
        private int c;

        public Interpolation(MeshGeometry3D modelMesh, IEnumerable<Sensor> sensorsDataList)
        {
            c = 0;

            grid3 = new MyPathNode[Convert.ToInt32(modelMesh.Bounds.SizeX / 10) + 1, Convert.ToInt32(modelMesh.Bounds.SizeY / 10) + 1];

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

            Color asd = new Color() { R = Convert.ToByte(red), G = Convert.ToByte(green), B = Convert.ToByte(blue), A = 255 };

            return asd;
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

                Sensor rstSensor = new Sensor("Fake Sensor", rndSensorPnt.X * 10 + this.offsetX, rndSensorPnt.Y * 10 + this.offsetY, 0);               

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

