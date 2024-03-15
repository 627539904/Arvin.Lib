using Arvin.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Arvin.MathSugar.LinearAlgebra
{
    /// <summary>
    /// 向量(代数)
    /// </summary>
    public class Vector : IMPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        /// <summary>
        /// 模长
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// 向量方向（单位向量）
        /// </summary>
        public Vector Direction { get; set; }

        #region 构造函数，初始化
        /// <summary>
        /// 0向量
        /// </summary>
        public Vector()
        {

        }

        public Vector(double x, double y, double z)
        {
            this.X = x; this.Y = y; this.Z = z;
            this.Length = this.GetLength();
            this.Direction = this.Normalize();
        }
        public Vector(IMPoint p) : this(p.X, p.Y, p.Z)
        {

        }
        public Vector(Vector dir, double len)
        {
            this.Direction = dir.Normalize();
            this.Length = len;
            this.X = this.Direction.X * len;
            this.Y = this.Direction.Y * len;
            this.Z = this.Direction.Z * len;
        }
        public static readonly Vector vectorX = new Vector(1, 0, 0);
        public static readonly Vector vectorY = new Vector(0, 1, 0);
        public static readonly Vector vectorZ = new Vector(0, 0, 1);
        #endregion

        #region 向量性质
        /// <summary>
        /// 计算模长
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double GetLength(double x, double y, double z)
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }
        public double GetLength()
        {
            return Vector.GetLength(this.X, this.Y, this.Z);
        }
        /// <summary>
        /// 转换为单位向量/归一化
        /// </summary>
        /// <returns></returns>
        public Vector Normalize()
        {
            return this.ToUnitNormalize();
        }
        #endregion
    }

    /// <summary>
    /// 向量计算扩展
    /// </summary>
    public static class VectorExtension
    {
        #region 基本运算 加减，数乘，点乘(内积)，叉乘（外积）
        public static Vector Add(this Vector x, Vector y)
        {
            return new Vector(x.X + y.X, x.Y + y.Y, x.Z + y.Z);
        }
        public static Vector Sub(Vector x, Vector y)
        {
            return new Vector(x.X - y.X, x.Y - y.Y, x.Z - y.Z);
        }
        /// <summary>
        /// 数乘
        /// </summary>
        /// <param name="x"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Vector NumberProduct(Vector x, double num)
        {
            return new Vector(num * x.X, num * x.Y, num * x.Z);
        }
        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double DotProduct(this Vector x, Vector y)
        {
            //x*y=|x||y|Cosθ
            return x.X * y.X + y.X * y.Y + x.Z * y.Z;
        }
        /// <summary>
        /// 叉乘
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector CrossProduct(this Vector x, Vector y)
        {
            //结果模长|C|=|x*y|=|x|*|y|*Sinθ
            return new Vector(x.Y * y.Z - x.Z * y.Y, x.Z * y.X - x.X * y.Z, x.X * y.Y - x.Y * y.X);
        }
        #endregion

        #region 几何相关操作 ,平行、垂直、向量之间的夹角
        /// <summary>
        /// 向量夹角（弧度）
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double RadianTo(this Vector a, Vector b)
        {
            if (a == null || b == null || a.Length == 0 || b.Length == 0) return 0;
            //Cosθ=x*y/(|x||y|)
            var dot = a.DotProduct(b);
            //Cosθ
            var cos = dot / (a.Length * b.Length);
            //使用反余弦函数计算夹角（注意：MathF.Acos返回的是弧度）
            return Math.Acos(cos);
        }
        /// <summary>
        /// 向量夹角（角度）
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double AngleTo(this Vector a, Vector b)
        {
            return a.RadianTo(b).RadianToAngel();
        }
        /// <summary>
        /// 向量平行
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsParallel(this Vector x, Vector y, double tolerance = 0.0001)
        {
            var angle = x.AngleTo(y);
            return angle.IsEqual(0, tolerance) || angle.IsEqual(180, tolerance);
        }
        /// <summary>
        /// 向量垂直
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsVertical(this Vector x, Vector y, double tolerance = 0.0001)
        {
            var angle = x.AngleTo(y);
            return angle.IsEqual(90, tolerance) || angle.IsEqual(270, tolerance);
        }
        /// <summary>
        /// 同方向
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsSameDir(this Vector x, Vector y, double tolerance = 0.0001)
        {
            var angle = x.AngleTo(y);
            return angle.IsEqual(0, tolerance);
        }
        /// <summary>
        /// 反方向
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsReverseDir(this Vector x, Vector y, double tolerance = 0.0001)
        {
            var angle = x.AngleTo(y);
            return angle.IsEqual(0, tolerance);
        }
        public static bool IsOppositeDir(this Vector x, Vector y, double tolerance = 0.0001)
        {
            return x.IsReverseDir(y, tolerance);
        }
        #endregion

        #region IsEqual
        public static bool IsEqual(this Vector x, Vector y, double tolerance = 0.0001)
        {
            return x.IsEqualCore(y, () => x.Distance(y).IsEqual(0, tolerance));
        }
        public static bool IsEqual<T>(this T x, T y, double tolerance = 0.0001)
            where T : IMPoint
        {
            return x.IsEqualCore(y, () => x.Distance(y).IsEqual(0, tolerance));
        }
        #endregion

        #region 空间关系
        /// <summary>
        /// 分量，在目标向量上的投影
        /// </summary>
        /// <param name="v"></param>
        /// <param name="targetV"></param>
        /// <returns></returns>
        public static Vector ProjectTo(this Vector v, Vector targetV)
        {
            return new Vector(targetV.Direction, v.ProjectLen(targetV));
        }
        /// <summary>
        /// 投影长度
        /// </summary>
        /// <param name="v"></param>
        /// <param name="targetV"></param>
        /// <returns></returns>
        public static double ProjectLen(this Vector v, Vector targetV)
        {
            return v.DotProduct(targetV.Direction);
        }
        /// <summary>
        /// 逆时针旋转指定角度
        /// </summary>
        /// <param name="v"></param>
        /// <param name="angle"></param>
        /// <param name="rotaAxis">旋转轴，默认Z轴</param>
        /// <returns></returns>
        public static Vector Rotate2D(this Vector v, double angle)
        {
            var x1 = Math.Cos(angle) * v.X - Math.Sin(angle) * v.Y;
            var y1 = Math.Cos(angle) * v.Y + Math.Sin(angle) * v.X;
            return new Vector(x1, y1, v.Z);

        }
        #endregion
    }
}
