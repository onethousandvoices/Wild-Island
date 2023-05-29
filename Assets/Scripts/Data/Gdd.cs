using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using WildIsland.Utility;

namespace WildIsland.Data
{
    [Serializable]
    public class Gdd : ScriptableObject
    {
        [ReadOnly, HorizontalLine(color: EColor.Blue)] public BasicGameData BasicGameData;
        [ReadOnly, HorizontalLine(color: EColor.Blue)] public BiomesData BiomesData;
        [ReadOnly, HorizontalLine(color: EColor.Blue)] public PlayerData PlayerData;
        [ReadOnly, HorizontalLine(color: EColor.Blue)] public EdibleResourcesData EdibleResourcesData;
        
        public static readonly List<GddSheet> ParsingSheets = new List<GddSheet> 
        {
            new GddSheet("Basic", typeof(BasicGameData)),
            new GddSheet("Biomes", typeof(BiomesData)),
            new GddSheet("MainCharacter", typeof(PlayerData)),
            new GddSheet("EdibleResourcesList", typeof(EdibleResourcesData))
        };
    }

    public interface IGameData { }

    [Serializable]
    public abstract class IGameData<T> : IGameData
    {
        public SerializableDictionary<string, T> Datas = new SerializableDictionary<string, T>();
    }

    public class GddSheet
    {
        public readonly string SheetId;
        public readonly Type DataType;
        public readonly Action CustomAction;

        public GddSheet(string sheetId, Type dataType, Action customAction = null)
        {
            SheetId = sheetId;
            DataType = dataType;
            CustomAction = customAction;
        }
    }
}