using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WildIsland.Data
{
    public class GDDDownloaderConfig
    {
        public const string DefaultSheetID = "1zbkPxEXDlor0Kvwleha7AOkWgECHD9hUXKgDkKeFNGU";
        // public const string LocalizationSheetID = "1GEyD9d15ZJxTQiy1kwip13kvZnUCXJCFzDmbNM_TraM";
        private const string GameDataId = "gameData";

        public static readonly WorksheetConfig[] ParsingDataTypes =
        {
            new WorksheetConfig(CustomGDDParsers.BasicSheetParser, FileGdId.Main, "Basic", GameDataId),
            new WorksheetConfig(CustomGDDParsers.MainCharacterParser, FileGdId.Main, "MainCharacter", GameDataId),
            new WorksheetConfig(CustomGDDParsers.BiomeSheetParser, FileGdId.Main, "Biomes", GameDataId)

            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "UI", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "prestige", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "tasks", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "pop_up_builds", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "equipment", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "storage_and_orders", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "fields", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "tutorial", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "Back_window", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "statistics", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "ADS_campaign_boosts_investor", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "Shop", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "Workshop", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.LocalizationParserBasic, FileGdId.Localization, "Settings", GameDataId),
        };

        public class WorksheetConfig
        {
            public readonly Type DataType;
            public readonly FileGdId SheetID;
            public readonly string TableID;
            public readonly string GameDataId;
            public readonly Action<WorksheetConfig, List<List<string>>, IGDDDataStorage> CustomAction;

            public WorksheetConfig(Type DataType, FileGdId SheetID, string TableID, string gdId)
            {
                this.DataType = DataType;
                this.SheetID = SheetID;
                this.TableID = TableID;
                this.CustomAction = null;
                GameDataId = gdId;
            }

            public WorksheetConfig(
                Action<WorksheetConfig, List<List<string>>, IGDDDataStorage> CustomAction, FileGdId SheetID, string TableID, string gdId)
            {
                this.DataType = null;
                this.SheetID = SheetID;
                this.TableID = TableID;
                this.CustomAction = CustomAction;
                GameDataId = gdId;
            }
        }
    }

    internal static class CustomGDDParsers
    {
#region LoaclizationParsers
        // internal static void LocalizationParserBasic(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
        // {
        //     var header = sheet[1];
        //     header[0] = string.Empty; //fucking fuck
        //     for (var i = 2; i < sheet.Count; ++i)
        //     {
        //         var item = (LocalizationDataDTO)GDDDownloader.CreateObjectFromData(typeof(LocalizationDataDTO), header, sheet[i]);
        //
        //         _localizationTexts.Add(new LocalizationData(item.ID, item.RuText, item.EnText, item.GrText, item.FrText, item.KrText, item.JpText));
        //     }
        //
        //     dataStorage.SetData(typeof(LocalizationData), _localizationTexts.ToArray());
        // }

/*
        internal static void LocalizationParserWithDescription(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
        {
            string descriptionAddition = ".description";
            for (int i = 2; i < sheet.Count; ++i)
            {
                int languageIndex = 1;
                _localizationTexts.Add(
                    new LocalizationData(sheet[i][0], sheet[i][languageIndex++], sheet[i][languageIndex++]));

                _localizationTexts.Add(
                    new LocalizationData(sheet[i][0] + descriptionAddition, sheet[i][languageIndex++], sheet[i][languageIndex]));
            }

            dataStorage.SetData(typeof(LocalizationData), _localizationTexts.ToArray());
        }
        */
#endregion

        internal static void BasicSheetParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
        {
            List<string> header = sheet[0];

            BasicGameData gameData = new BasicGameData();

            for (int i = 1; i < sheet.Count; ++i)
            {
                string rowKey = sheet[i][0];
                if (!BasicGameData.RowTypes.ContainsKey(rowKey))
                    continue;
                object data = GDDDownloader.CreateObjectFromData(BasicGameData.RowTypes[rowKey], header, sheet[i]);

                bool foundField = false;
                foreach (FieldInfo field in gameData.GetType().GetFields())
                {
                    if (field.FieldType != BasicGameData.RowTypes[rowKey])
                        continue;
                    field.SetValue(gameData, data);
                    foundField = true;
                    break;
                }
                if (!foundField)
                    Debug.LogError("Field not found in BasicGameData for row " + rowKey);
            }
            dataStorage.SetData(typeof(BasicGameData), new object[]
            {
                gameData
            });
        }

        internal static void MainCharacterParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
        {
            PlayerData playerData = new PlayerData();

            for (int i = 1; i < sheet.Count; ++i)
            {
                string rowKey = sheet[i][0];
                string rowValue = sheet[i][1];

                bool foundField = false;
                FieldInfo[] fields = playerData.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                    if (!string.Equals(field.Name, rowKey, StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    object stat = Activator.CreateInstance(field.FieldType);
                    stat.GetType().GetField("Value").SetValue(stat, float.Parse(rowValue, CultureInfo.InvariantCulture.NumberFormat));

                    field.SetValue(playerData, stat);
                    foundField = true;
                    break;
                }
                if (!foundField)
                    Debug.LogError("Field not found in PlayerData for row " + rowKey);
            }
            dataStorage.SetData(typeof(PlayerData), new object[]
            {
                playerData
            });
        }

        internal static void BiomeSheetParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
        {
            BiomesData biomesData = new BiomesData();

            for (int i = 1; i < sheet.Count; ++i)
            {
                string rowKey = sheet[i][0];
                string biomeTemperature = sheet[i][1];
                string effectValue = sheet[i][2];

                bool foundField = false;
                foreach (FieldInfo field in biomesData.GetType().GetFields())
                {
                    string fieldName = field.Name.Replace("BiomeData", "").ToLower();
                    if (!rowKey.Contains(fieldName))
                        continue;
                    object data = Activator.CreateInstance(field.FieldType);
                    Type dataType = data.GetType();
                    dataType.GetField("Temperature").SetValue(data, int.Parse(biomeTemperature));
                    dataType.GetField("EffectValue").SetValue(data, float.Parse(effectValue, CultureInfo.InvariantCulture.NumberFormat));
                    field.SetValue(biomesData, data);
                    foundField = true;
                    break;
                }
                if (!foundField)
                    Debug.LogError("Field not found in BasicGameData for row " + rowKey);
            }
            dataStorage.SetData(typeof(BiomesData), new object[]
            {
                biomesData
            });
        }
    }

    public enum FileGdId { Main, Localization }

    public interface IGDDDataType { }

    public interface IGDDDataTypeString : IGDDDataType
    {
        string ID { get; }
    }

    public interface IGDDDataTypeInt : IGDDDataType
    {
        int ID { get; }
    }

    public interface IGDDDataStorage
    {
        void SetData(Type type, object[] data);
        void SetData(Type type, object data);
    }

    [Serializable]
    public class Range
    {
        public float Left;
        public float Right;

        public Range(float left, float right)
        {
            Left = left;
            Right = right;
        }

        public Range(string source)
        {
            Range range = Parse(source);
            Left = range.Left;
            Right = range.Right;
        }

        public static Range Parse(string source)
        {
            string[] parts = source.Split('/');
            string ls = parts[0].Trim();
            string rs = parts[1].Trim();
            float l = float.Parse(ls);
            float r = float.Parse(rs);
            Range range = new Range(l, r);
            return range;
        }

        public override string ToString()
            => Left + " / " + Right;
    }

    [Serializable]
    public class RangeArray
    {
        public double[] Intervals;

        public RangeArray(IList intervals)
        {
            Intervals = intervals as double[];
        }

        public static RangeArray Parse(string source)
        {
            string[] parts = source.Split('/');
            double[] intervals = new double[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                intervals[i] = double.Parse(parts[i].Trim());

            RangeArray rangeArray = new RangeArray(intervals);

            return rangeArray;
        }
    }

    [Serializable]
    public class ObjectArray<T>
    {
        public T[] Intervals;

        public ObjectArray(IList<object> intervals)
        {
            Intervals = intervals.Cast<T>().ToArray();
        }

        public static ObjectArray<T> Parse(string source)
        {
            string[] parts = source.Split('/');
            object[] intervals = new object[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                if (typeof(T) == typeof(string))
                    intervals[i] = parts[i];

                if (typeof(T) == typeof(double))
                    intervals[i] = double.Parse(parts[i].Trim());

                if (typeof(T) == typeof(int))
                    intervals[i] = int.Parse(parts[i].Trim());
            }

            ObjectArray<T> objArray = new ObjectArray<T>(intervals);

            return objArray;
        }
    }
}