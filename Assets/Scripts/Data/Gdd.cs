using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WildIsland.Data
{
    [Serializable]
    public class Gdd : ScriptableObject
    {
        [ReadOnly, HorizontalLine(color: EColor.Blue)] public BasicGameData BasicGameData;
        [ReadOnly, HorizontalLine(color: EColor.Blue)] public BiomesData BiomesData;
        [ReadOnly, HorizontalLine(color: EColor.Blue)] public PlayerData PlayerData;
        
        public static readonly List<GddSheet> ParsingSheets = new List<GddSheet> 
        {
            new GddSheet("Basic", typeof(BasicGameData)),
            new GddSheet("Biomes", typeof(BiomesData)),
            new GddSheet("MainCharacter", typeof(PlayerData))
        };
    }

    public interface IGameData { }

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