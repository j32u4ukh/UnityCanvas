using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace UnityExpansion
{
    public class Debugging
    {
        public enum Level
        {
            Log = 0,
            Warn = 1,
            Error = 2
        }

        public static Level ELevel
        {
            get {
                return level;
            }
            set {
                level = value;
            }
        }

        private static Level level = Level.Log;
        
        /* 以下屬性只能置於參數位置，不能於函式內呼叫
         * CallerLineNumber: 實際呼叫的行數位置
         * CallerMemberName: 實際呼叫的函數名稱
         * CallerFilePath: 實際呼叫的腳本路徑
         * 參考網站: https://stackoverflow.com/questions/12556767/how-do-i-get-the-current-line-number
         */
        static string DebugMessage(string message, int line_num, string member, string file_path)
        {
            string[] split_path = file_path.Split('\\');
            string script_name = split_path[split_path.Length - 1];
            return string.Format("[{0}] {1} ({2}) | {3}", script_name, member, line_num, message);
        }

        public static void Log(string message = "", [CallerLineNumber] int line_num = 0, [CallerMemberName] string member = "", [CallerFilePath] string file_path = "")
        {
            if (level <= Level.Log) { 
                Debug.Log(DebugMessage(message, line_num, member, file_path));
            }
        }

        public static void Warn(string message, [CallerLineNumber] int line_num = 0, [CallerMemberName] string member = "", [CallerFilePath] string file_path = "")
        {
            if (level <= Level.Warn)
            {
                Debug.LogWarning(DebugMessage(message, line_num, member, file_path));
            }            
        }

        public static void Error(string message, [CallerLineNumber] int line_num = 0, [CallerMemberName] string member = "", [CallerFilePath] string file_path = "")
        {
            if (level <= Level.Error) {
                Debug.LogError(DebugMessage(message, line_num, member, file_path));
            }
        }
    }

    public class Array
    {
        private static System.Random rand = new System.Random();

        /// <summary>
        /// 產生按照順序的陣列，可正序、可逆序
        /// </summary>
        /// <param name="start">起始值(包含)</param>
        /// <param name="end">結束值(不包含)</param>
        /// <returns></returns>
        public static int[] RangeArray(int start, int end)
        {
            if (start <= end)
            {
                return Enumerable.Range(start, end - start).ToArray();
            }
            else
            {
                // start > end
                int[] array = Enumerable.Range(end + 1, start - end).ToArray();
                System.Array.Reverse(array);
                return array;
            }
        }

        /// <summary>
        /// 打亂陣列，不影響原始陣列，原始陣列不再使用應該會被視為 garbage 並回收
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">陣列</param>
        /// <returns>打亂後的陣列</returns>
        public static T[] ShuffleArray<T>(T[] array)
        {
            return array.OrderBy(x => rand.Next()).ToArray();
        }

        // 返回格式化陣列的字串
        public static string ToString<T>(T[] array)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            int i, len = array.Length;

            if (len > 0)
            {
                sb.Append($"{array[0]}");

                for (i = 1; i < len; i++)
                {
                    sb.Append($", {array[i]}");
                }
            }

            sb.Append("]");

            return sb.ToString();
        }
    }

    public class List
    {
        private static System.Random rand = new System.Random();

        public static List<T> ShuffleList<T>(List<T> list)
        {
            return list.OrderBy(x => rand.Next()).ToList();
        }

        public static string ToString<T>(List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            int i, len = list.Count;

            if (len > 0)
            {
                sb.Append($"{list[0]}");

                for (i = 1; i < len; i++)
                {
                    sb.Append($", {list[i]}");
                }
            }

            sb.Append("]");

            return sb.ToString();
        }

        public static string ToString<T>(List<List<T>> list2d)
        {
            List<string> reslut = new List<string>();

            foreach (List<T> list in list2d)
            {
                reslut.Add(ToString(list));
            }

            return ToString(reslut);
        }
    }

    public class Vector
    {
        public static string ToString(Vector3 vector, int digits = 4)
        {
            string format = string.Format("{0}{1}{2}{3}{4}{5}{6}", "({0:F", digits, "}, {1:F", digits, "}, {2:F", digits, "})");

            return string.Format(format, vector.x, vector.y, vector.z);
        }
    }

    public class EnumTool
    {
        /// <summary>
        /// 封裝 getDescription，簡化輸入的參數形式，取得 Enum 的 Description 字串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetDescription<T>(Enum @enum)
        {
            return GetDescription(@enum.ToString(), typeof(T));
        }

        /// <summary>
        /// 取得 Enum 的 Description 字串
        /// </summary>
        /// <param name="value">字串形式的 Enum</param>
        /// <param name="type">Enum 的類型</param>
        /// <returns></returns>
        public static string GetDescription(string value, Type type)
        {
            string name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                                             .Select(d => d)
                                             .FirstOrDefault();

            //// 找無相對應的列舉
            if (name == null)
            {
                return string.Empty;
            }

            // 利用反射找出相對應的欄位
            var field = type.GetField(name);

            // 取得欄位設定DescriptionAttribute的值
            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            //// 無設定Description Attribute, 回傳Enum欄位名稱
            if (attribute == null || attribute.Length == 0)
            {
                return name;
            }

            //// 回傳Description Attribute的設定
            return ((DescriptionAttribute)attribute[0]).Description;
        }
    }
}