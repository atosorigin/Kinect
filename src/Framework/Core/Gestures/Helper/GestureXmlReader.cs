using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using Kinect.Common;
using log4net;
using Common;

namespace Kinect.Core.Gestures.Helper
{
    /// <summary>
    /// GestureXmlReader
    /// </summary>
    public static class GestureXmlReader
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(GestureXmlReader));

        /// <summary>
        /// Reads the nodes to list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static List<T> ReadNodesToList<T>(string filename) where T : new()
        {
            XmlTextReader reader = new XmlTextReader(filename);

            List<T> list = new List<T>();
            var type = typeof(T);
            var properties = type.GetProperties();

            while (reader.Read())
            {
                if (reader.Name.Equals(type.Name))
                {
                    try
                    {
                        T instance = new T();

                        foreach (var prop in properties)
                        {
                            var value = reader.GetAttribute(prop.Name);

                            if (prop.PropertyType == typeof(string))
                            {
                                prop.SetValue(instance, value, null);
                            }
                            else
                            {
                                MethodInfo mi = prop.PropertyType.GetMethod("Parse", new Type[] { typeof(string) });
                                object ovalue = mi.Invoke(null, new object[] { value });
                                prop.SetValue(instance, ovalue, null);
                            }
                        }

                        list.Add(instance);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("An error occured while reading xml file {0}", filename);
                        Trace.WriteLine(ex.Message);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Reads the specific value.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="element">The element.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns></returns>
        public static string ReadSpecificValue(string filename, string element, string attribute)
        {
            XmlTextReader reader = new XmlTextReader(filename);

            while (reader.ReadToFollowing(element))
            {
                if (reader.HasAttributes)
                {
                    var value = reader.GetAttribute(attribute);

                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Reads the specific value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">The filename.</param>
        /// <param name="element">The element.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns></returns>
        public static T ReadSpecificValue<T>(string filename, string element, string attribute)
        {
            XmlTextReader reader = new XmlTextReader(filename);
            
            while (reader.ReadToFollowing(element))
            {
                if (reader.HasAttributes)
                {
                    var value = reader.GetAttribute(attribute);

                    if (!string.IsNullOrEmpty(value))
                    {
                        try
                        {
                            MethodInfo mi = typeof(T).GetMethod("Parse", new Type[] { typeof(string) });
                            object ovalue = mi.Invoke(null, new object[] { value });
                            return (T)ovalue;
                        }
                        catch (Exception ex)
                        {
                            _log.IfError(string.Format("An error occured while parsing value {0}", value), ex);
                        }
                    }
                }
            }

            return default(T);
        }
    }
}
