using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WildIsland.Utility
{
    [Serializable]
    public class StringKeyArray<T> : IEnumerable where T : IGDDDataType
    {
        private T[] _array;
        private Dictionary<string, T> _dictionary;
        private Dictionary<int, T> _indexDictionary;

        public StringKeyArray(T[] source)
        {
            _array = (T[])source.Clone();
            _dictionary = new Dictionary<string, T>(_array.Length);
            _indexDictionary = new Dictionary<int, T>(_array.Length);
            Type type = typeof(T);
            if (typeof(IGDDDataTypeString).IsAssignableFrom(type))
            {
                for (int i = 0; i < _array.Length; ++i)
                {
                    _dictionary[((IGDDDataTypeString)_array[i]).ID] = _array[i];
                    _indexDictionary[i] = _array[i];
                }
            }
            else if (typeof(IGDDDataTypeInt).IsAssignableFrom(type))
            {
                for (int i = 0; i < _array.Length; ++i)
                {
                    IGDDDataTypeInt typeInt = (IGDDDataTypeInt)_array[i];
                    _dictionary.Add(typeInt.ID.ToString(), _array[i]);
                    _indexDictionary.Add(typeInt.ID, _array[i]);
                }
            }
        }

        public int IndexOf(string ID)
        {
            T obj = _dictionary[ID];
            for (int i = 0; i < _indexDictionary.Count; i++)
                if (_array[i].Equals(obj)) return i;

            return -1;
        }

        public int Length => _dictionary.Count;

        public T this[string id] => _dictionary[id];

        public T this[int index] => _indexDictionary[index];

        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        public bool ContainsIndex(int index) => _indexDictionary.ContainsKey(index);

        public IEnumerator GetEnumerator() => _array.GetEnumerator();

        public IEnumerable Keys => _dictionary.Keys;
        public IList<T> Values => _dictionary.Values.ToList();
    }
}