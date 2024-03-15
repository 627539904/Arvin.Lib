using Arvin.Extensions;
using Arvin.MathSugar.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.MathSugar
{
    /// <summary>
    /// 点-几何基础特征
    /// </summary>
    public class MPoint : IMPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public MPoint() { }

        public MPoint(double x, double y, double z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }
        /// <summary>
        /// 实现IMPoint接口的点类型均可以直接构造转换
        /// </summary>
        /// <param name="point"></param>
        public MPoint(IMPoint point)
        {
            this.X = point.X; this.Y = point.Y; this.Z = point.Z;
        }
    }
    /// <summary>
    /// 点-数学特征：所有三维空间点均具备的属性
    /// </summary>
    public interface IMPoint
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }

    /// <summary>
    /// 点扩展
    /// </summary>
    public static class PointExtension
    {
        #region Offset
        /// <summary>
        /// 点位偏移
        /// </summary>
        /// <param name="p"></param>
        /// <param name="dir">偏移方向，取归一化</param>
        /// <param name="len">偏移距离</param>
        /// <returns></returns>
        public static MPoint Offset(this MPoint p, Vector dir, double len)
        {
            var vOffset = new Vector(dir.Direction, len);
            var res = p.ToVector().Add(vOffset);
            return res.ToMPoint();
        }
        #endregion

        #region 类型转换
        public static Vector ToVector<T>(this T p)
            where T : IMPoint
        {
            return new Vector(p);
        }
        public static MPoint ToMPoint<T>(this T v)
            where T : IMPoint
        {
            return new MPoint(v);
        }
        #endregion
    }
}
