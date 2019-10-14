using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utils
{
    public static partial class Maths
    {

        /// <summary>
        /// 值域在 [-1, 1] 的锯齿函数. <br/>
        /// x = 0 时 y = 0, 斜率为 1, 周期为 2. 跳跃点处的定义是 y = 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Serration(this float x) => x % 2f - x.Sgn() + 1f;

        /// <summary>
        /// 值域在 [0, 1] 的三角波. <br/>
        /// x = 0 时 y = 0, 周期为 2.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Triangular(this float x) => ((x + 1f).ModSys(2f) - 1f).Abs();
    }
}