using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GenType
{
    public class TypeHelper
    {
        // types like int are C# aliases for CLR types - there's no 
        // method in C# to convert C# alias name (as a string) into a type, so
        // we have to provide a lookup:
        private static readonly Dictionary<string, Type> csharpAliasToType =
            new Dictionary<string, Type> {
                { "byte",     typeof(byte)      },
                { "short",    typeof(short)     },
                { "int",      typeof(int)       },
                { "uint",     typeof(uint)      },
                { "long",     typeof(long)      },
                { "float",    typeof(float)     },
                { "double",   typeof(double)    },
                { "decimal",  typeof(decimal)   },
                { "char",     typeof(char)      },
                { "string",   typeof(string)    },
            };

        public static Type GetTypeFromString(string typename)
        {
            if (!csharpAliasToType.TryGetValue(typename, out var type))
            {
                return null;
            }
            return type;
        }


        /// <summary>
        /// Return the parse function for the type as Func{string,obj}
        /// </summary>
        /// <param name="typename">the C# alias name of the type</param>
        /// <returns>The parse function as a Func{string, obj}</returns>
        public static Func<string, object> GetParser(string typename)
        {
            if (!csharpAliasToType.TryGetValue(typename, out var type))
            {
                return null;
            }
            var parseMethod = type.GetMethods().Where(x => x.Name == "Parse" && x.GetParameters().Count() == 1).FirstOrDefault();
            
            return parseMethod == null ? null : FunctionTypeConverter<string>(parseMethod);
        }


        /// <summary>
        /// Converts a function that takes a the generic type and returns a specific type into a function that takes
        /// a string and returns an object. E.g. int.Parse is Func{string, int} and is converted to Func{string, object}
        /// </summary>
        /// <typeparam name="T">type of the function's parameter</typeparam>
        /// <param name="method">MethodInfo of the function</param>
        /// <returns>a Func which can be executed via Invoke</returns>
        private static Func<T, object> FunctionTypeConverter<T>(MethodInfo method)
        {
            var parameter = method.GetParameters().Single();
            var argument = Expression.Parameter(typeof(object), "argument");

            var methodCall = Expression.Call(null, method, Expression.Convert(argument, parameter.ParameterType));

            return Expression.Lambda<Func<T,object>>(
            Expression.Convert(methodCall, typeof(object)), argument).Compile();
        }

    }
}
