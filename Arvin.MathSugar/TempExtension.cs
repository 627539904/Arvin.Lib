using Arvin.MathSugar.LinearAlgebra;
using Arvin.MathSugar;
using System;
using System.Runtime.CompilerServices;

namespace Arvin.MathSugar
{
    public static class TempExtension
    {
        #region Math 无依赖的数学运算
        /// <summary>
        /// 两个浮点数的中间值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Mid(this double x, double y)
        {
            return (y - x) / 2 + x;
        }
        /// <summary>
        /// 单位化
        /// 0->0
        /// -10->-1
        /// 10->1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double ToUnit(this double x)
        {
            if (x == 0) return 0;
            return x > 0 ? 1 : -1;
        }
        /// <summary>
        /// 单位化，和ToUnit一样，只是为了兼容使用习惯
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Normalize(this double x)
        {
            return x.ToUnit();
        }
        #endregion

        #region Geo 几何运算
        /// <summary>
        /// 两点之间的距离
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance<T>(this T p1, T p2)
            where T : IMPoint
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double dz = p2.Z - p1.Z;
            return Vector.GetLength(dx, dy, dz);
        }
        /// <summary>
        /// 两个点的中间值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static T Mid<T>(this T p1, T p2)
            where T : IMPoint, new()
        {
            return new T() { X = p1.X.Mid(p2.X), Y = p1.Y.Mid(p2.Y), Z = p1.Z.Mid(p2.Z) };
        }
        /// <summary>
        /// p1=>p2的方向,没有单位化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static T Direction<T>(this T p1, T p2)
            where T : IMPoint, new()
        {
            var res = new T() { X = p2.X - p1.X, Y = p2.Y - p1.Y, Z = p2.Z - p1.Z };
            return res;
        }
        /// <summary>
        /// 单位化，与圆心距离为1的点，用于计算单位向量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        public static T ToUnitNormalize<T>(this T p)
            where T : IMPoint, new()
        {
            double scale = Distance(new T(), p);
            return new T() { X = p.X / scale, Y = p.Y / scale, Z = p.Z / scale };
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double AngleToRadian(this double angle)
        {
            return angle * MathUnit.Unit_PI_180;
        }
        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double RadianToAngel(this double radian)
        {
            return radian * MathUnit.Unit_180_PI;
        }
        #endregion
    }
}
