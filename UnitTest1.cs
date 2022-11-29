namespace Lab7Tech
{
    /*
    ������������� ����� ������� ������ � ������� ������ �� �� ������� �������.

    ����� ����������� ��������������, ���� ���, ���������� �� ���� ����� � ������������ ����������� ���������� ������� �������������� �������� ���������� ���. � ������ ��������� ���� � ������� ���������� ������� ������ �����������.

    ����������� � �������������� �����, ����������� � �������� ���������� ���������� �����a�� ������������:

    "1", ���� �����a���������������������;

    "-1", ���� �����a�������� ��������������;

    "0", ���� �����a������ �� ������� ��������������.
     */
    [TestClass]
    public class PolygonTest
    {
        [DynamicData(nameof(InsideGenerator), DynamicDataSourceType.Method)]
        [DataTestMethod]
        public void TestInside(Polygon poly, Point point, int expected)
        {
            Assert.AreEqual(expected, poly.IsInside(point));
        }

        private static IEnumerable<object[]> InsideGenerator()
        {
            var square = new Polygon(new Point[]
            {
                (0, -5),
                (0,  5),
                (10, 5),
                (15, 0),
                (10,-5)
            });

            // �� �������
            yield return new object[] { square, (Point)(0, 0), 0 };
            // �� ����
            yield return new object[] { square, (Point)(0, 5), 0 };
            // ������� � �� ��� �� X ���������� ��� �������
            yield return new object[] { square, (Point)(0, -6), -1 };
            // ������� � �� ��� �� Y ���������� ��� �������
            yield return new object[] { square, (Point)(-1, -5), -1 };
            // ���-�� �������
            yield return new object[] { square, (Point)(-2, 3), -1 };
            // ���-�� ������
            yield return new object[] { square, (Point)(1, 1), 1 };
            // ������ � �� ��� �� Y ���������� ��� ����
            yield return new object[] { square, (Point)(1, 0), 1 };
        }
    };

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Point((double, double) coordinates)
        {
            return new Point(coordinates.Item1, coordinates.Item2);
        }
    }

    public class Polygon
    {
        private readonly Point[] _points;

        public Polygon(Point[] points)
        {
            _points = points;
        }

        /// <summary>
        /// ��������� ���������� �� ���������� �� ����� � ����������� ������ ��� ������� ��������������
        /// </summary>
        /// <param name="first">������ ������� ��������������</param>
        /// <param name="second">����� ������� ��������������</param>
        /// <param name="point">����� �� ������� ������� ���</param>
        /// <returns>���������� �� ��� �������</returns>
        private static bool IsIntersect(Point first, Point second, Point point)
        {
            if (point.X > Math.Max(first.X, second.X))
                return false;

            double max_y, min_y;
            if (first.Y > second.Y)
            {
                max_y = first.Y;
                min_y = second.Y;
            }
            else
            {
                max_y = second.Y;
                min_y = first.Y;
            }
            return (point.Y <= max_y) && (point.Y > min_y);
        }

        /// <summary>
        /// ��������� ����� �� ����� �� ����� ������������ ����� �������
        /// </summary>
        /// <param name="first">������ �������</param>
        /// <param name="second">����� �������</param>
        /// <param name="point">�����</param>
        /// <returns></returns>
        private static bool IsOnLine(Point first, Point second, Point point)
        {
            if (point.X - Math.Max(first.X, second.X) > double.Epsilon ||
                Math.Min(first.X, second.X) - point.X > double.Epsilon ||
                point.Y - Math.Max(first.Y, second.Y) > double.Epsilon ||
                Math.Min(first.Y, second.Y) - point.Y > double.Epsilon)
                return false;

            if (Math.Abs(second.X - first.X) < double.Epsilon)
                return Math.Abs(first.X - point.X) < double.Epsilon || Math.Abs(second.X - point.X) < double.Epsilon;
            if (Math.Abs(second.Y - first.Y) < double.Epsilon)
                return Math.Abs(first.Y - point.Y) < double.Epsilon || Math.Abs(second.Y - point.Y) < double.Epsilon;

            double x = first.X + (point.Y - first.Y) * (second.X - first.X) / (second.Y - first.Y);
            double y = first.Y + (point.X - first.X) * (second.Y - first.Y) / (second.X - first.X);

            return Math.Abs(point.X - x) < double.Epsilon || Math.Abs(point.Y - y) < double.Epsilon;
        }
        
        public int IsInside(Point point)
        {
            uint count = 0;
            for (int i = 0; i < _points.Length; i++)
            {

                int j = (i + 1) % _points.Length;

                if (IsOnLine(_points[i], _points[j], point)) return 0;
                
                count += Convert.ToUInt32(IsIntersect(_points[i], _points[j], point));
            }
            return -1 + (byte)(count % 2) * 2;
        }
    }
}