using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WildIsland.Data
{
    public static class MainDataBase
    {
        public static readonly string Path = System.IO.Path.Combine(Application.persistentDataPath, "main.db");
        private static readonly Dictionary<Type, object> _collections;
        private static readonly LiteDatabase _db;

        static MainDataBase()
        {
            BsonMapper.Global.IncludeFields = true;
            _db = new LiteDatabase(Path);
            _collections = new Dictionary<Type, object>();

            BsonMapper.Global.IncludeFields = true;
        }

        public static LiteCollection<T> GetCollection<T>()
        {
            Type type = typeof(T);
            if (_collections.TryGetValue(type, out object collection))
                return (LiteCollection<T>)collection;
            LiteCollection<T> newCollection = _db.GetCollection<T>(type.GetGenericArguments()[0].ToString().Replace(".", string.Empty));
            _collections.Add(type, newCollection);
            return newCollection;
        }
    }
}