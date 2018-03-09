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
    using System.Windows.Media.Media3D;

    public class Interpolation
    {
        private Dictionary<Tuple<int, int>, double> dic = new Dictionary<Tuple<int, int>, double>();


        private MeshGeometry3D BuildDictionary(MeshGeometry3D mesh)
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

        public Dictionary<Tuple<int, int>, double> FillSensorDataDictionary(List<Sensor> sensorsDataList)
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

        public Dictionary<Tuple<int, int>, double> PreProcessing(Dictionary<Tuple<int, int>, double> sensorDictionary)
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

        private List<Tuple<int, int>> GetNeighboringPoints(int x, int y, Dictionary<Tuple<int, int>, double> newDictionary, out double mCoord1, out double mCoord2)
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

        private double CrossProduct(Point3D p1, Point3D p2)
        {
            return p1.X * p2.Y - p1.Y * p2.X;
        }
    }    
}

