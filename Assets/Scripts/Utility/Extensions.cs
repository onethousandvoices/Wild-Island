using System;
using System.Collections.Generic;
using System.Reflection;
using WildIsland.Processors;

namespace WildIsland.Utility
{
    public static class Extensions
    {
        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (int i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static bool HasFlagOptimized(this InputState input, InputState has)
            => (input & has) > 0;

        public static void SetData(this object obj, object objToAdd)
        {
            FieldInfo[] fields = obj.GetType().GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType != objToAdd.GetType())
                    continue;

                field.SetValue(obj, objToAdd);
            }
        }
    }
}