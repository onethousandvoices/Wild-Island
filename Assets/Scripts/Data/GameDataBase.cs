using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using WildIsland.Utility;

namespace WildIsland.Data
{
    [Serializable]
    public class GameData : GameDataBase
    {
        [SerializeField] private BasicGameData[] _basicGameData = Array.Empty<BasicGameData>();
        [SerializeField] private PlayerData[] _playerData = Array.Empty<PlayerData>();
        [SerializeField] private BiomesData[] _biomesData = Array.Empty<BiomesData>();

        public GameData()
        {
            _supportedTypes = new Dictionary<Type, FieldInfo>()
            {
                { typeof(BasicGameData), GetFieldByName<GameData>("_basicGameData") }, 
                { typeof(PlayerData), GetFieldByName<GameData>("_playerData") },
                { typeof(BiomesData), GetFieldByName<GameData>("_biomesData") }
            };
        }
    }

    [Serializable]
    public class GameDataBase : IGDDDataStorage
    {
        private static int loadedType = -1;

        protected Dictionary<Type, FieldInfo> _supportedTypes = null;

        public static void ResetLoadedType()
            => loadedType = -1;

        public static string GDPath1
        {
            get { return Application.dataPath + "/Editor/Data/"; }
        }

        protected static string GDPath2
        {
            get { return Application.dataPath + "/Resources/Data/"; }
        }

        public static string GDCachePath
        {
            get { return Application.persistentDataPath + "/Data/"; }
        }

        public void SetData(Type type, object[] data)
        {
            if (!_supportedTypes.ContainsKey(type))
                throw new NotImplementedException($"Type {type} is not supported");

            SetData(type, data, _supportedTypes[type]);
        }

        private void SetData(Type type, object[] source, FieldInfo field)
        {
            try
            {
                Array ar = Array.CreateInstance(type, source.Length);
                Array.Copy(source, ar, source.Length);
                field.SetValue(this, ar);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void SetData(Type type, object data)
        {
            //mb need deep copy
            if (!_supportedTypes.ContainsKey(type))
                throw new NotImplementedException("Type " + type.ToString() + " is not supported");
            _supportedTypes[type].SetValue(this, data);
        }

        public T Get<T>(string index) where T : IGDDDataTypeString
        {
            return (T)(object)Find(GetAll<T>(), index);
        }

        public T[] GetAll<T>() where T : IGDDDataType
        {
            Type type = typeof(T);

            if (!_supportedTypes.ContainsKey(type))
                throw new NotImplementedException("Type " + type.ToString() + " is not supported");

            return (T[])_supportedTypes[type].GetValue(this);
        }

        public IPartialGameDataContainer PrepareContainer(Type containerType)
        {
            if (!typeof(IPartialGameDataContainer).IsAssignableFrom(containerType))
                throw new NotImplementedException(containerType + " must be derived from IPartialGameDataContainer");

            IPartialGameDataContainer container = (IPartialGameDataContainer)Activator.CreateInstance(containerType);
            FieldInfo[] fields = containerType.BaseType!.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                try
                {
                    if (field.FieldType.IsArray)
                    {
                        Type fieldType = field.FieldType.GetElementType();
                        if (_supportedTypes.ContainsKey(fieldType))
                        {
                            object obj = _supportedTypes[fieldType].GetValue(this);
                            Array array = (Array)obj;
                            field.SetValue(container, array.Clone());
                        }
                    }
                    else if (field.FieldType.IsGenericType &&
                             field.FieldType.GetGenericTypeDefinition() == typeof(StringKeyArray<>))
                    {
                        Type fieldType = field.FieldType.GetGenericArguments()[0];
                        if (_supportedTypes.ContainsKey(fieldType))
                        {
                            object obj = _supportedTypes[fieldType].GetValue(this);
                            object array = Activator.CreateInstance(field.FieldType, obj);
                            field.SetValue(container, array);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Cannot fill " + field.FieldType + " for " + containerType + "\n" + e);
                }
            }

            return container;
        }

        private static void CheckLoadingType()
        {
            byte[] bytes = null;
            if (Directory.Exists(GDCachePath) && File.Exists(GDCachePath + "loadGameData.bytes"))
            {
                bytes = File.ReadAllBytes(GDCachePath + "loadGameData.bytes");
            }
            LoadGameData gdCache = null;
            if (bytes != null && bytes.Length != 0)
            {
                gdCache = LoadFromBytes<LoadGameData>(bytes);
            }
            LoadGameData gdRes = LoadFromResources<LoadGameData>("loadGameData");
            if (gdCache != null && gdCache.SupportedAppVersionData.GameDataVersion > gdRes.SupportedAppVersionData.GameDataVersion)
            {
                AppVersion currAppVer = AppVersion.Parse(Application.version);
                AppVersion cacheAppVer =
#if UNITY_ANDROID || UNITY_STANDALONE
                    gdCache.SupportedAppVersionData.AndroidVersion;
#elif UNITY_IOS
                    gdCache.SupportedAppVersionData.IOSVersion;
#endif
                if (cacheAppVer.CorrectForGameData(currAppVer))
                    loadedType = 1;
                else
                    loadedType = 0;
            }
            else
                loadedType = 0;
        }

        public static T Load<T>(string name) where T : GameDataBase
        {
            if (loadedType == -1)
                CheckLoadingType();
            if (loadedType == 0)
            {
#if !UNITY_EDITOR
                Debug.LogError("LOAD GD " + name + " FROM RESOURCES");
#endif
                return LoadFromResources<T>(name);
            }
            else
            {
                byte[] bytes = null;
                if (Directory.Exists(GDCachePath) && File.Exists(GDCachePath + name + ".bytes"))
                {
                    bytes = File.ReadAllBytes(GDCachePath + name + ".bytes");
                }
                T gdCache = null;
                if (bytes != null && bytes.Length != 0)
                {
                    gdCache = LoadFromBytes<T>(bytes);
                }
                if (gdCache != null)
                {
#if !UNITY_EDITOR
                Debug.LogError("LOAD GD " + name + " FROM CACHE");
#endif
                    return gdCache;
                }
                else
                {
#if !UNITY_EDITOR
                Debug.LogError("LOAD GD " + name + " FROM RESOURCES");
#endif
                    return LoadFromResources<T>(name);
                }
            }
        }

        public static T LoadFromCompressedJSON<T>(string name, string json) where T : GameDataBase
        {
            JObject jObject = JObject.Parse(json);
            byte[] bytesData = (byte[])jObject["Data"];
            MemoryStream stream = new MemoryStream(bytesData);
            GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress);
            MemoryStream decompressedStream = new MemoryStream();
            int readedByte = zipStream.ReadByte();
            while (readedByte > -1)
            {
                decompressedStream.WriteByte((byte)readedByte);
                readedByte = zipStream.ReadByte();
            }
            zipStream.Close();
            stream.Close();
            decompressedStream.Position = 0;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                T gameData = (T)binaryFormatter.Deserialize(decompressedStream);
                decompressedStream.Close();
                return gameData;
            }
            catch (Exception e)
            {
                Debug.LogError("Cannot read decompressed GD " + name + "|" + e.ToString());
                decompressedStream.Close();
                return Activator.CreateInstance<T>();
            }
        }

        public static T LoadFromResources<T>(string name) where T : GameDataBase
        {
            // Make sure file was found by Unity after saving
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                TextAsset asset = Resources.Load<TextAsset>("Data/" + name);
                MemoryStream stream = new MemoryStream(asset.bytes);
                T cfg = (T)bf.Deserialize(stream);
                stream.Close();
                return cfg;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError(new IOException("Cannot read GD file at " + GDPath2 + name + ".bytes" + ": " +
                                               e.ToString()));
#else
                Debug.LogError(new IOException("Cannot read GD file"));
#endif
                return Activator.CreateInstance<T>();
            }
        }

        public static T LoadFromBytes<T>(byte[] bytes) where T : GameDataBase
        {
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                MemoryStream stream = new MemoryStream(bytes);
                return (T)bf.Deserialize(stream);
            }
            catch (Exception e)
            {
                Debug.LogError("Cannot deserialize bytes to gamedata: " + e.ToString());
                return null;
            }
        }

        public void Save(string name, bool onlyInEditorFolder = false)
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(GDPath1 + name + ".bytes", FileMode.Create);
            bf.Serialize(file, this);
            file.Close();

            if (!onlyInEditorFolder)
            {
                file = File.Open(GDPath2 + name + ".bytes", FileMode.Create);
                bf.Serialize(file, this);
                file.Close();
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static GameDataBase Create()
        {
            return new GameDataBase();
        }

        private T Find<T>(T[] array, string id) where T : IGDDDataTypeString
        {
            foreach (T item in array)
                if (item.ID == id)
                    return item;

            throw new IndexOutOfRangeException(String.Format("Cannot find object of type {0} with id={1}", typeof(T),
                id));
        }

        protected FieldInfo GetFieldByName<T>(string name)
        {
            FieldInfo field = typeof(T).GetField(name,
                BindingFlags.NonPublic | BindingFlags.Public |
                BindingFlags.Instance);
            if (field == null)
                throw new NotImplementedException("Cannot found field " + name + " on GameData");
            return field;
        }

        public static void CreateGdVersionFile()
        {
            byte[] data = File.ReadAllBytes(GDPath1 + "/loadGameData.bytes");
            LoadGameData gd = LoadFromBytes<LoadGameData>(data);
            string versionText = gd.SupportedAppVersionData.ToString();
            File.WriteAllText(GDPath1 + "/gamedataversion.ver", versionText);
        }
    }

    [Serializable]
    public class LoadGameData : GameDataBase
    {
        [SerializeField] public SupportedAppVersionData SupportedAppVersionData = null;
        [SerializeField] private HintsData[] _hintsData = Array.Empty<HintsData>();
        //[SerializeField] private LocalizationData[] _localizationData = new LocalizationData[0];

        public LoadGameData()
        {
            _supportedTypes = new Dictionary<Type, FieldInfo>()
            {
                //{typeof(LocalizationData), GetFieldByName<LoadGameData>("_localizationData")},
                { typeof(HintsData), GetFieldByName<LoadGameData>("_hintsData") }, { typeof(SupportedAppVersionData), GetFieldByName<LoadGameData>("SupportedAppVersionData") }
            };
        }
    }

    [Serializable]
    public class HintsData : IGDDDataTypeString
    {
        [SheetColumnName("Idgroupe")]
        public readonly string HintsID;

        [SheetColumnName("Idhints")]
        public readonly string IdLocHints;

        public string ID => IdLocHints;
    }

    public interface IPartialGameDataContainer { }

    public abstract class BaseContainer<T> : IPartialGameDataContainer
    {
        private readonly T[] _gd = Array.Empty<T>();
        public T Default => _gd[0];
    }

    public class BasicGameDataContainer : BaseContainer<BasicGameData> { }

    public class PlayerDataContainer : BaseContainer<PlayerData> { }

    public class BiomesDataContainer : BaseContainer<BiomesData> { }
}