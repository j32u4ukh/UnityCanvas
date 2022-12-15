using System;

namespace CSharpExpansion
{
    public class CSharpMath {
        public const double Deg2Rad = Math.PI / 180.0;
        public const double Rad2Deg = 180.0 / Math.PI;

        #region 三角函數
        /// <summary>
        /// r / y
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double Sin(double angle)
        {
            return Math.Sin(angle * Deg2Rad);
        }

        /// <summary>
        /// r/ x
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double Cos(double angle)
        {
            return Math.Cos(angle * Deg2Rad);
        }

        /// <summary>
        /// y / x
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double Tan(double angle)
        {
            return Math.Tan(angle * Deg2Rad);
        }

        /// <summary>
        /// x / y
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double Cot(double angle)
        {
            return 1.0 / Tan(angle);
        }

        public static double Asin(double sin)
        {
            return Rad2Deg * Math.Asin(sin);
        }

        public static double Acos(double cos)
        {
            return Rad2Deg * Math.Acos(cos);
        }

        public static double Atan(double tan)
        {
            return Rad2Deg * Math.Atan(tan);
        }
        #endregion

        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Pow(Math.E, -x));
        }

        public static double Asigmoid(double y)
        {
            return -Math.Log(1.0 / y - 1.0);
        }
    }
}
