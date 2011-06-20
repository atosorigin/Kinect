using System;
using System.Collections.Generic;

namespace Kinect.Common
{
    public static class EnumExtensions
    {
        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return (((int) (object) type & (int) (object) value) == (int) (object) value);
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (int) (object) type == (int) (object) value;
            }
            catch
            {
                return false;
            }
        }

        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T) (object) (((int) (object) type | (int) (object) value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format("Could not append value from enumerated type '{0}'.", typeof (T).Name), ex);
            }
        }

        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T) (object) (((int) (object) type & ~(int) (object) value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format("Could not remove value from enumerated type '{0}'.", typeof (T).Name), ex);
            }
        }

        public static T GetEnum<T>(this Enum type, string str) where T : struct
        {
            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            var outPutValue = (T) Enum.Parse(typeof (T), str, true);
            return outPutValue;
        }

        public static void Add<T>(this IList<T> list, T item, bool distinct)
        {
            if (!distinct || !list.Contains(item))
            {
                list.Add(item);
            }
        }

        public static bool Remove<T>(this IList<T> list, T item, bool all)
        {
            if (!all)
            {
                return list.Remove(item);
            }

            bool success = false;
            while (list.Contains(item))
            {
                success &= list.Remove(item);
            }

            return success;
        }
    }
}