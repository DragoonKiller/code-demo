using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

    using static Maths;

    //
    // 这个文件存储一些计算几何算法.
    // 

    public static partial class Algorthms
    {
        /// <summary>
        /// 把一个闭合的, 没有"洞"的多边形的边界转化成三角形的网格. <br/>
        /// "割耳"算法.
        /// 复杂度 n^2, n 是点数.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static List<Triangle> Triangulation(this List<Vector2> p)
        {
            if(p.Count < 3) return new List<Triangle>();
            if(p[p.Count - 1] == p[0]) p.RemoveAt(p.Count - 1);
            if(p.Count < 3) return new List<Triangle>();

            var res = new List<Triangle>();

            (Vector2 lv, Vector2 cv, Vector2 rv) GetTriangle(int id)
            {
                var lv = p[(id - 1).ModSys(p.Count)];
                var cv = p[id];
                var rv = p[(id + 1).ModSys(p.Count)];
                return (lv, cv, rv);
            }

            /// <summary>
            /// 顺时针返回1, 逆时针返回-1, 全部共线返回0.
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            int CurveSign()
            {
                float area = 0;
                var g = p[0];
                for(int i = 2; i < p.Count; i++) area += g.To(p[i - 1]).Cross(g.To(p[i]));
                return area.Sgn();
            }

            int curveSign = CurveSign();
            bool IsEar(int id)
            {
                var (lv, cv, rv) = GetTriangle(id);
                var tr = new Triangle(lv, cv, rv);
                // "耳朵"不能是凹的.
                if(tr.sign != curveSign) return false;
                // "耳朵"内部不能包含任何点.
                for(int i = 2; i < p.Count - 1; i++)
                {
                    if(tr.Contains(p[(id + i).ModSys(p.Count)], false)) return false;
                }
                return true;
            }

            int triangleCount = p.Count - 2;
            for(int t = 0; t < triangleCount; t++)
            {
                for(int i = 0; i < p.Count; i++)
                {
                    var (lv, cv, rv) = GetTriangle(i);
                    if(IsEar(i))
                    {
                        res.Add(new Triangle(lv, cv, rv));
                        p.RemoveAt(i);
                        break;
                    }
                }
            }

            return res;
        }

        [UnitTest]
        public static void TriangulationTest(PolygonCollider2D cd)
        {
            var vts = new List<Vector2>(cd.GetPath(0));
            var tri = vts.Triangulation();
            var cls = new Color[] { Color.red, Color.blue, Color.yellow };
            int ci = 0;
            foreach(var i in tri)
            {
                ci++;
                ci %= 3;
            }
        }

    }
}