using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace GenType
{
    /// <summary>
    /// Converts a data type to another data type.
    /// </summary>
    public static class Cast
    {
        /// <summary>
        /// Converts input to Type of default value or given as typeparam T
        /// </summary>
        /// <typeparam name="T">typeparam is the type in which value will be returned, it could be any type eg. int, string, bool, decimal etc.</typeparam>
        /// <param name="input">Input that need to be converted to specified type</param>
        /// <param name="defaultValue">defaultValue will be returned in case of value is null or any exception occures</param>
        /// <returns>Input is converted in Type of default value or given as typeparam T and returned</returns>
        private static T To<T>(object input, T defaultValue)
        {
            var result = defaultValue;
            try
            {
                if (input == null || input == DBNull.Value) return result;
                if (typeof(T).IsEnum)
                {
                    result = (T)Enum.ToObject(typeof(T), To(input, Convert.ToInt32(defaultValue)));
                }
                else
                {
                    result = (T)Convert.ChangeType(input, typeof(T));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"error: {ex}");
            }

            return result;
        }

        /// <summary>
        /// Converts input to Type of typeparam T
        /// </summary>
        /// <typeparam name="T">typeparam is the type in which value will be returned, it could be any type eg. int, string, bool, decimal etc.</typeparam>
        /// <param name="input">Input that need to be converted to specified type</param>
        /// <returns>Input is converted in Type of default value or given as typeparam T and returned</returns>
        public static T To<T>(object input)
        {
            return To(input, default(T));
        }



    }


    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var parse1 = TypeHelper.GetParser("double");
                var res = parse1.Invoke("12345.4");
                Console.WriteLine();
                //var method = typeof(Cast).GetMethod("To").MakeGenericMethod(t1);
                //var res = method.Invoke(null, new object[] { "1234" });
                //Console.WriteLine();
                //Type[] typeArgs = { Type.GetType("TypeRepository") };
                ////Type repositoryType = genericType.MakeGenericType(typeArgs);
                //var param = new System.Data.SqlClient.SqlParameter();
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine($"{progname} Error: {ex.Message}");
            }

        }
    }
}
