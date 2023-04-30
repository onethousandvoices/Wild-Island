using LiteDB;
using System;
using UnityEngine;

namespace WildIsland.Data
{
    public class DbValue<TValue>
    {
        public TValue Value;

        private readonly LiteCollection<SavedValue<TValue>> _collection;
        private readonly SavedValue<TValue> _value;

        public DbValue(string key, TValue defaultValue = default(TValue))
        {
            _collection = MainDataBase.GetCollection<SavedValue<TValue>>();
            if (!_collection.Exists(value => value.Id == key))
            {
                _value = new SavedValue<TValue>
                    {Id = key, Value = defaultValue};
                _collection.Upsert(_value);
            }
            else
            {
                try
                {
                    _value = _collection.FindOne(value => value.Id == key);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Cannot read value " + key + " with type " + typeof(TValue) + " from database\n" + e);
                    _value = new SavedValue<TValue>
                        {Id = key, Value = defaultValue};
                    _collection.Upsert(_value);
                }
            }
            Value = _value.Value;
        }

        public void Save()
        {
            _value.Value = Value;
            _collection.Upsert(_value);
            
            //test
            // _value = _collection.FindOne(value => value.Id == _value.Id);
            // Value = _value.Value;
        }

        public void Save(TValue value)
        {
            Value = value;
            Save();
        }
    }
    
    public class SavedValue<TValue>
    {
        public string Id { get; set; }
        public TValue Value { get; set; }
    }
}