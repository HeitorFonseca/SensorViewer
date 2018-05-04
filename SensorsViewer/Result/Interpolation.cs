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

    /// <summary>
    /// Interpolation class
    /// </summary>
    public class Interpolation
    {
        /// <summary>
        /// Fake sensors list
        /// </summary>
        private List<Tuple<Sensor, Sensor, Sensor>> fakeSensors;

        /// <summary>
        /// Sensor dictionary
        /// </summary>
        private Dictionary<Tuple<int, int>, double> sensorDictionary;

        /// <summary>
        /// Triangle points dictionary
        /// </summary>
        private Dictionary<Tuple<int, int>, double> modelTrianglePoints = new Dictionary<Tuple<int, int>, double>();

        /// <summary>
        /// List of triangle points
        /// </summary>
        private List<Point3D> listPoint3d = new List<Point3D>();

        /// <summary>
        /// Grid to seach path finding
        /// </summary>
        private MyPathNode[,] grid3;

        /// <summary>
        /// Offset X
        /// </summary>
        private int offsetX;

        /// <summary>
        /// Offset Y
        /// </summary>
        private int offsetY;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interpolation"/> class
        /// </summary>
        /// <param name="modelMesh">Model mesh</param>
        /// <param name="sensorsDataList">Sensor data list</param>
        public Interpolation(MeshGeometry3D modelMesh, IEnumerable<Sensor> sensorsDataList)
        {
            this.grid3 = new MyPathNode[Convert.ToInt32(modelMesh.Bounds.SizeX / 10) + 1, Convert.ToInt32(modelMesh.Bounds.SizeY / 10) + 1];

            this.offsetX = Convert.ToInt32(modelMesh.Bounds.X);
            this.offsetY = Convert.ToInt32(modelMesh.Bounds.Y);

            if (sensorsDataList.Count() > 1)
            {
                this.sensorDictionary = this.FillSensorDataDictionary2(sensorsDataList);
                this.BuildDictionary2(modelMesh, this.sensorDictionary);
                this.BuildGrid2(Convert.ToInt32(modelMesh.Bounds.SizeX / 10), Convert.ToInt32(modelMesh.Bounds.SizeY / 10));
                this.fakeSensors = this.CreateRandomSensors4(sensorsDataList);
            }
        }

        /// <summary>
        /// Get heat map color
        /// </summary>
        /// <param name="value">Value to calculate respective color</param>
        /// <param name="min_value">Minimum value</param>
        /// <param name="max_value">Maximum value</param>
        /// <returns>Respective color</returns>
        public static Color GetHeatMapColor(double value, double min_value, double max_value)
        {
            double normValue = (value - min_value) / (max_value - min_value);

            const int NUM_COLORS = 3;

            double[,] color = new double[NUM_COLORS, 3] { { 255, 0, 0 }, { 0, 0, 0 }, { 0, 255, 0 } };

            int idx1;
            int idx2;

            double fractBetween = 0;  // Fraction between "idx1" and "idx2" where our value is.

            // Accounts for an input <= 0
            if (normValue <= 0)
            {
                idx1 = idx2 = 0;
            }
            else if (normValue >= 1)
            {
                // Accounts for an input >=0
                idx1 = idx2 = NUM_COLORS - 1;
            }
            else
            {
                normValue = normValue * (NUM_COLORS - 1);         // Will multiply value by 2.
                idx1 = Convert.ToInt32(Math.Floor(normValue)); // Our desired color will be after this index.
                idx2 = idx1 + 1;                            // ... and before this index (inclusive).
                fractBetween = normValue - (float)idx1;      // Distance between the two indexes (0-1).
            }

            double red = ((color[idx2, 0] - color[idx1, 0]) * fractBetween) + color[idx1, 0];
            double green = ((color[idx2, 1] - color[idx1, 1]) * fractBetween) + color[idx1, 1];
            double blue = ((color[idx2, 2] - color[idx1, 2]) * fractBetween) + color[idx1, 2];

            Color asd = new Color() { R = Convert.ToByte(red), G = Convert.ToByte(green), B = Convert.ToByte(blue), A = 255 };

            return asd;
        }

        /// <summary>
        /// Perform Interpolation
        /// </summary>
        /// <param name="modelMesh">Model mesh</param>
        /// <param name="sensorsDataList">Sensor data list</param>
        /// <returns>Resultant vertices</returns>
        public Vertex[] Interpolate2(MeshGeometry3D modelMesh, IEnumerable<Sensor> sensorsDataList)
        {          
            Vertex[] vertices = null;

            DateTime lastMeasureTime = DateTime.Now;

            if (sensorsDataList.Count() > 1)
            {
                this.sensorDictionary = this.FillSensorDataDictionary2(sensorsDataList);
                this.ReCalculateFakeSensors(this.fakeSensors, this.sensorDictionary);
                vertices = this.PreProcessing2(this.sensorDictionary);                
            }

            TimeSpan asd = DateTime.Now.Subtract(lastMeasureTime);

            return vertices;
        }

        /// <summary>
        /// Create sensor data dictionary
        /// </summary>
        /// <param name="sensorsDataList">Sensors data list</param>
        /// <returns>returns a dictionary with sensors data</returns>
        private Dictionary<Tuple<int, int>, double> FillSensorDataDictionary2(IEnumerable<Sensor> sensorsDataList)
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
                    this.modelTrianglePoints[key2] = sd.Values.Last().Value; // Update the coordinate of the sensors with the real value
                }
                else
                {
                    sensorDictionary.Add(key, 0);
                    this.modelTrianglePoints[key2] = 0; // Update the coordinate of the sensors with the real value
                }
            }

            return sensorDictionary;
        }

        /// <summary>
        /// Convert double to float
        /// </summary>
        /// <param name="doubleValue">Value to be converted</param>
        /// <returns>Value converted</returns>
        private float DoubleToFloat(double doubleValue)
        {
            if (float.IsPositiveInfinity(Convert.ToSingle(doubleValue)))
            {
                return float.MaxValue;
            }

            if (float.IsNegativeInfinity(Convert.ToSingle(doubleValue)))
            {
                return float.MinValue;
            }

            return Convert.ToSingle(doubleValue);
        }

        /// <summary>
        /// Get the two neighboors points
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="sensorDictionary">Sensor dictionary</param>
        /// <param name="distCoord1">Out distance to the closest sensor</param>
        /// <param name="distCoord2">Out distance to the second closest sensor</param>
        /// <returns>Two neighboors points</returns>
        private Tuple<int, int>[] GetNeighboringPoints(int x, int y, Dictionary<Tuple<int, int>, double> sensorDictionary, out double distCoord1, out double distCoord2)
        {
            Tuple<int, int>[] ret = new Tuple<int, int>[2];

            double min1 = double.MaxValue, min2 = double.MaxValue;

            ret[0] = sensorDictionary.ElementAt(0).Key;
            ret[1] = sensorDictionary.ElementAt(1).Key;

            foreach (KeyValuePair<Tuple<int, int>, double> v in sensorDictionary)
            {
                int nx = v.Key.Item1;
                int ny = v.Key.Item2;

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

            distCoord1 = min1;
            distCoord2 = min2;

            return ret;
        }

        /// <summary>
        /// Returns the closes point
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="newDictionary">Sensor dictionary</param>
        /// <returns>Closest point</returns>
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

        /// <summary>
        /// Calculate cross product
        /// </summary>
        /// <param name="p1">Point one</param>
        /// <param name="p2">Point two</param>
        /// <returns>Cross product of point one and point two</returns>
        private double CrossProduct(Point3D p1, Point3D p2)
        {
            return (p1.X * p2.Y) - (p1.Y * p2.X);
        }        

        /// <summary>
        /// Build triangle points dictionary
        /// </summary>
        /// <param name="mesh">Model mesh</param>
        /// <param name="sensorDictionary">Sensor dictionary</param>
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

                this.PointsOfTriangle2(p0, p1, p2, sensorDictionary);

                this.listPoint3d.Add(new Point3D((int)p0.X, (int)p0.Y, 0));
                this.listPoint3d.Add(new Point3D((int)p1.X, (int)p1.Y, 0));
                this.listPoint3d.Add(new Point3D((int)p2.X, (int)p2.Y, 0));
            }
        }

        /// <summary>
        /// Finds points inside triangle
        /// </summary>
        /// <param name="p0">Point a</param>
        /// <param name="p1">Point b</param>
        /// <param name="p2">Point c</param>
        /// <param name="sensorDictionary">Sensor dictionary</param>
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

                    double s = (double)this.CrossProduct(q, vs2) / this.CrossProduct(vs1, vs2);
                    double t = (double)this.CrossProduct(vs1, q) / this.CrossProduct(vs1, vs2);

                    if ((s >= 0) && (t >= 0) && (s + t <= 1))
                    {
                        Tuple<int, int> key = new Tuple<int, int>(x, y);

                        if (!this.modelTrianglePoints.ContainsKey(key))
                        {
                            this.modelTrianglePoints.Add(key, 0);                            
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Build grid map
        /// </summary>
        /// <param name="width">Grid width</param>
        /// <param name="height">Grid height</param>
        private void BuildGrid2(int width, int height)
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    bool isWall = false;

                    Tuple<int, int> tuple = new Tuple<int, int>(x + (this.offsetX / 10), y + (this.offsetY / 10));
                    if (!this.modelTrianglePoints.ContainsKey(tuple))
                    {
                        isWall = true;
                    }

                    this.grid3[x, y] = new MyPathNode()
                    {
                        IsWall = isWall,
                        X = x,
                        Y = y,
                    };
                }
            }
        }

        /// <summary>
        /// Preprocesse data
        /// </summary>
        /// <param name="sensorDictionary">Sensor dictionary</param>
        /// <returns>Preprocessed points</returns>
        private Vertex[] PreProcessing2(Dictionary<Tuple<int, int>, double> sensorDictionary)
        {
            var vertices = new Vertex[this.listPoint3d.Count];
            int counter = 0;
          
            foreach (Point3D point in this.listPoint3d)
                {
                    int nx = (int)point.X;
                    int ny = (int)point.Y;

                    Tuple<int, int> tuple = new Tuple<int, int>(nx, ny);

                    if (sensorDictionary.ContainsKey(tuple))
                    {
                        vertices[counter].X = nx;
                        vertices[counter].Y = ny;
                        vertices[counter++].Z = this.DoubleToFloat(sensorDictionary[tuple]);
                        continue;
                    }

                    double d1, d2;
                    Tuple<int, int>[] neighbors = this.GetNeighboringPoints(nx, ny, sensorDictionary, out d1, out d2);

                    d1 = 1 / d1;
                    d2 = 1 / d2;

                    double var = ((d1 * sensorDictionary[neighbors[0]]) + (d2 * sensorDictionary[neighbors[1]])) / (d1 + d2); 

                    vertices[counter].X = nx;
                    vertices[counter].Y = ny;
                    vertices[counter++].Z = this.DoubleToFloat(var);
            }

            return vertices;
        }        

        /// <summary>
        /// Create random fake sensors
        /// </summary>
        /// <param name="sensorsDataList">Sensor list</param>
        /// <returns>Fake sensors</returns>
        private List<Tuple<Sensor, Sensor, Sensor>> CreateRandomSensors4(IEnumerable<Sensor> sensorsDataList)
        {        
            List<Tuple<Sensor, Sensor, Sensor>> fakeSensors = new List<Tuple<Sensor, Sensor, Sensor>>();

            List<Tuple<int, int>> keyList = new List<Tuple<int, int>>(this.modelTrianglePoints.Keys);

            Random rand = new Random();
            Sensor closestSensor = new Sensor();
            Sensor sndClosestSensor = new Sensor();

            for (int i = 0; i < 90; i++)
            {
                int min1 = int.MaxValue;
                int min2 = int.MaxValue;

                Tuple<int, int> randomKey = keyList[rand.Next(keyList.Count)];

                Point rndSensorPnt = new Point(randomKey.Item1 - (this.offsetX / 10), randomKey.Item2 - (this.offsetY / 10));

                foreach (Sensor sensor in sensorsDataList)
                {
                    // Get the distance between randomKey and 7 sensors and return the minumum
                    Point sensorPnt = new Point(Convert.ToInt32((sensor.X - this.offsetX) / 10), Convert.ToInt32((sensor.Y - this.offsetY) / 10));

                    int pathLen = 0;

                    try
                    {
                        MySolver<MyPathNode, Object> astar = new MySolver<MyPathNode, Object>(this.grid3);
                        IEnumerable<MyPathNode> path = astar.Search(rndSensorPnt, sensorPnt, null);
                        pathLen = path.Count();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Unexpected astar algorithm error");
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

                Sensor rstSensor = new Sensor("Fake Sensor", (rndSensorPnt.X * 10) + this.offsetX, (rndSensorPnt.Y * 10) + this.offsetY, 0);               

                Tuple<int, int> key = new Tuple<int, int>(Convert.ToInt32(rstSensor.X), Convert.ToInt32(rstSensor.Y));

                Tuple<Sensor, Sensor, Sensor> fk = new Tuple<Sensor, Sensor, Sensor>(rstSensor, closestSensor, sndClosestSensor);

                fakeSensors.Add(fk);

                keyList.Remove(randomKey);
            }

            return fakeSensors;
        }

        /// <summary>
        /// Recalculate fake sensors value based in new values
        /// </summary>
        /// <param name="fakeSensors">Fake sensors</param>
        /// <param name="sensorDictionary">Sensor dictionary</param>
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

                double value = ((d1 * closestSensorValue) + (d2 * sndClosestSensorValue)) / (d1 + d2);

                Tuple<int, int> key = new Tuple<int, int>(Convert.ToInt32(rstSensor.X), Convert.ToInt32(rstSensor.Y));

                sensorDictionary.Add(key, value);
            }
        }
    }
}
