using System;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 地图帮助类
    /// </summary>
    public class MapHelper
    {
        #region 地理位置距离计算


        private const double EarthRadius = 6371.393;

        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 两个坐标点距离
        /// </summary>
        /// <param name="lng1">经度1</param>
        /// <param name="lat1">纬度1</param>
        /// <param name="lng2">经度2</param>
        /// <param name="lat2">纬度2</param>
        /// <returns>单位米 小数点后一位</returns>
        public static double GetPositionDistance(double lng1, double lat1, double lng2, double lat2)
        {
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);
            double a = radLat1 - radLat2;
            double b = Rad(lng1) - Rad(lng2);
            double distance = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
                                                      Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            distance = distance * EarthRadius;
            distance = Math.Round(distance * 10000) / 10000;
            return Math.Round(distance * 1000, 2);
        }

        /// <summary>
        /// 两个坐标点距离
        /// </summary>
        /// <param name="lng1">经度1</param>
        /// <param name="lat1">纬度1</param>
        /// <param name="lng2">经度2</param>
        /// <param name="lat2">纬度2</param>
        /// <returns>单位米 小数点后一位</returns>
        public static double GetPositionDistance(string lng1, string lat1, string lng2, string lat2)
        {
            return GetPositionDistance(String2Long(lng1, 0),
                String2Long(lat1, 0), String2Long(lng2, 0),
                String2Long(lat2, 0));
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值，或者转换失败的值</param>
        /// <returns>转换后的数字类型结果</returns>
        public static double String2Long(string strValue, double defValue)
        {
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return defValue;
            }

            double intValue;
            if (double.TryParse(strValue, out intValue))
            {
                return intValue;
            }

            return defValue;
        }

        #endregion
    }
}