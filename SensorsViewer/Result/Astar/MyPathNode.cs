using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SensorsViewer.Result.Astar
{
    public class MyPathNode : SettlersEngine.IPathNode<Object>
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Boolean IsWall { get; set; }

        public bool IsWalkable(Object unused)
        {
            return !IsWall;
        }
    }

    public class MySolver<TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TPathNode, TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
    {
        protected override Double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
        }

        protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            return Heuristic(inStart, inEnd);
        }

        public MySolver(TPathNode[,] inGrid)
            : base(inGrid)
        {
        }


        private unsafe void Compute(int Width, int Height)
        {
            try
            {
                Random rnd = new Random();
                MyPathNode[,] grid = new MyPathNode[Width, Height];


                Dictionary<Tuple<int, int>, double> trianglePointsDictionary = new Dictionary<Tuple<int, int>, double>();

                string[] lines = System.IO.File.ReadAllLines(@"C:\Users\heitor.araujo\Documents\wokspace\GM\Tests\AStar\pontsmodel.txt");

                foreach (string line in lines)
                {
                    string[] pars = line.Split(',');

                    if (pars[2] != " 0")
                    {
                        var asd = 1;
                    }

                    Tuple<int, int> tuple = new Tuple<int, int>(Convert.ToInt32(pars[0]), Convert.ToInt32(pars[1]));

                    trianglePointsDictionary[tuple] = Convert.ToDouble(pars[2]);

                }



                for (int x = 0; x < 1185; x++)
                {
                    for (int y = 0; y < 420; y++)
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



                Point p1 = new Point(572 + 592, 124 + 210);
                Point p2 = new Point(537 + 592, -155 + 210);
                Point p3 = new Point(100 + 592, -175 + 210);
                Point p4 = new Point(-100 + 592, -175 + 210);
                Point p5 = new Point(-552 + 592, -155 + 210);
                Point p6 = new Point(-572 + 592, 124 + 210);
                Point p7 = new Point(0 + 592, 129 + 210);

                // compute and display path
                MySolver<MyPathNode, Object> aStar = new MySolver<MyPathNode, Object>(grid);
                IEnumerable<MyPathNode> path = aStar.Search(p1, p5, null);
            }
            catch (Exception ex)
            {
                throw new Exception("error on something");
            }
        }
    }
}
