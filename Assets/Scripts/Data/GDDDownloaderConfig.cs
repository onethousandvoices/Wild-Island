using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WildIsland.Data;

namespace Data
{
    public class GDDDownloaderConfig
    {
        public const string DefaultSheetID = "1zbkPxEXDlor0Kvwleha7AOkWgECHD9hUXKgDkKeFNGU";
        // public const string LocalizationSheetID = "1GEyD9d15ZJxTQiy1kwip13kvZnUCXJCFzDmbNM_TraM";
        private const string GameDataId = "gameData";

        public static readonly WorksheetConfig[] ParsingDataTypes =
        {
            new WorksheetConfig(CustomGDDParsers.BasicSheetParser, FileGdId.Main, "Basic", GameDataId),
            // new WorksheetConfig(typeof(BuildingsData), FileGdId.Main, "BuildingsBasic", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.CustomBuildingsUpgradesParser, FileGdId.Main, "Buildings", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.CustomWorkerPriceParser, FileGdId.Main, "WorkersPrice", GameDataId),
            // new WorksheetConfig(typeof(StorageBasicData), FileGdId.Main, "BasicCapacity", GameDataId),
            // new WorksheetConfig(typeof(LoadersData), FileGdId.Main, "Loaders", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.CustomCarPriceParser, FileGdId.Main, "CarPrice", GameDataId),
            // new WorksheetConfig(typeof(LocationsData), FileGdId.Main, "Locations", GameDataId),
            // new WorksheetConfig(typeof(TimeTempering), FileGdId.Main, "TimeTempering", GameDataId),
            // new WorksheetConfig(typeof(BerryFieldsData), FileGdId.Main, "BerryFields", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.CustomWorkerParametersParser, FileGdId.Main, "WorkingFields", GameDataId),
            // new WorksheetConfig(typeof(TimePickersData), FileGdId.Main, "TimePicker", GameDataId),
            // new WorksheetConfig(typeof(TierListData), FileGdId.Main, "TierList", GameDataId),
            // new WorksheetConfig(typeof(TaskData), FileGdId.Main, "TasksList", GameDataId),
            // new WorksheetConfig(CustomGDDParsers.CustomOrderProductDataParser, FileGdId.Main, "ProductOrders", GameDataId),
            // new WorksheetConfig(typeof(OrderProductPriceModifierData), FileGdId.Main, "ModOrders", GameDataId),
            // new WorksheetConfig(typeof(SoundData), FileGdId.Main, "Sounds", GameDataId),

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

            public WorksheetConfig(System.Type DataType, FileGdId SheetID, string TableID, string gdId)
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
//         private static List<LocalizationData> _localizationTexts = new List<LocalizationData>();
//
//         /*
//         internal static void CustomSupportedAppVersionParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
//         {
//             var dataParse = (SupportedAppVersionDataParse)GDDDownloader
//                 .CreateObjectFromData(typeof(SupportedAppVersionDataParse), sheet[0], sheet[1]);
//             SupportedAppVersionData data = new SupportedAppVersionData()
//             {
//                 GameDataVersion = dataParse.GameDataVersion,
//                 AndroidVersion = AppVersion.Parse(dataParse.AndroidVersion),
//                 IOSVersion = AppVersion.Parse(dataParse.IOSVersion)
//             };
//             dataStorage.SetData(typeof(SupportedAppVersionData), data);
//         }
//
//         */
//         internal static void CustomBuildingsUpgradesParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
//         {
//             List<string> header = sheet[0];
//
//             List<object> items = new List<object>();
//
//             string buildingName = "", curBdId = null;
//             List<BuildingUpgradeLevelData> levels = new List<BuildingUpgradeLevelData>();
//
//             Action addNewItem = () =>
//             {
//                 BuildingsUpgradeData bd = new BuildingsUpgradeData(buildingName, levels.ToArray());
//                 items.Add(bd);
//                 levels.Clear();
//             };
//
//             for (int i = 1; i < sheet.Count; ++i)
//             {
//                 curBdId = sheet[i][0];
//
//                 if (!string.IsNullOrEmpty(curBdId) && curBdId != buildingName)
//                 { // Building name changed in current row
//                     if (buildingName.Length > 0)
//                     { // Previous name was not empty
//                         addNewItem.Invoke();
//                     }
//                     buildingName = curBdId;
//                 }
//
//                 BuildingUpgradeLevelData item = (BuildingUpgradeLevelData)GDDDownloader.CreateObjectFromData(typeof(BuildingUpgradeLevelData), header, sheet[i]);
//                 levels.Add(item);
//             }
//             addNewItem.Invoke();
//             dataStorage.SetData(typeof(BuildingsUpgradeData), items.ToArray());
//         }
//
//         internal static void CustomWorkerPriceParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
//         {
//             var header = sheet[0];
//
//             var items = new List<object>();
//
//             var workerID = string.Empty;
//             var levels = new List<WorkerPriceData>();
//
//             void AddNewItem()
//             {
//                 var bd = new WorkersPriceData(workerID, levels.ToArray());
//                 items.Add(bd);
//                 levels.Clear();
//             }
//
//             for (var i = 1; i < sheet.Count; ++i)
//             {
//                 var workerNum = sheet[i][0];
//
//                 if (!string.IsNullOrEmpty(workerNum) && workerNum != workerID)
//                 {
//                     if (workerID.Length > 0)
//                         AddNewItem();
//                     workerID = workerNum;
//                 }
//
//                 var item = (WorkerPriceData)GDDDownloader.CreateObjectFromData(typeof(WorkerPriceData), header, sheet[i]);
//                 levels.Add(item);
//             }
//             AddNewItem();
//             dataStorage.SetData(typeof(WorkersPriceData), items.ToArray());
//         }
//
//         internal static void CustomWorkerParametersParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
//         {
//             var header = sheet[0];
//
//             var items = new List<object>();
//
//             var workerID = string.Empty;
//             var levels = new List<FieldWorkerParametersData>();
//
//             void AddNewItem()
//             {
//                 var bd = new FieldWorkerData(workerID, levels.ToArray());
//                 items.Add(bd);
//                 levels.Clear();
//             }
//
//             for (var i = 1; i < sheet.Count; ++i)
//             {
//                 var workerNum = sheet[i][0];
//
//                 if (!string.IsNullOrEmpty(workerNum) && workerNum != workerID)
//                 {
//                     if (workerID.Length > 0)
//                         AddNewItem();
//                     workerID = workerNum;
//                 }
//
//                 var item = (FieldWorkerParametersData)GDDDownloader.CreateObjectFromData(typeof(FieldWorkerParametersData), header, sheet[i]);
//                 levels.Add(item);
//             }
//             AddNewItem();
//             dataStorage.SetData(typeof(FieldWorkerData), items.ToArray());
//         }
//
//         internal static void CustomCarPriceParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
//         {
//             var header = sheet[0];
//
//             var items = new List<object>();
//
//             var workerID = string.Empty;
//             var levels = new List<LoaderPriceData>();
//
//             void AddNewItem()
//             {
//                 var bd = new LoadersPriceData(workerID, levels.ToArray());
//                 items.Add(bd);
//                 levels.Clear();
//             }
//
//             for (var i = 1; i < sheet.Count; ++i)
//             {
//                 var workerNum = sheet[i][0];
//
//                 if (!string.IsNullOrEmpty(workerNum) && workerNum != workerID)
//                 {
//                     if (workerID.Length > 0)
//                         AddNewItem();
//                     workerID = workerNum;
//                 }
//
//                 var item = (LoaderPriceData)GDDDownloader.CreateObjectFromData(typeof(LoaderPriceData), header, sheet[i]);
//                 levels.Add(item);
//             }
//             AddNewItem();
//             dataStorage.SetData(typeof(LoadersPriceData), items.ToArray());
//         }

        // internal static void CustomOrderProductDataParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
        // {
        //     var header = sheet[0];
        //
        //     var items = new List<object>();
        //
        //     var storageCapacity = string.Empty;
        //     var dataItems = new List<OrderProductDataItem>();
        //
        //     void AddNewItem()
        //     {
        //         var bd = new OrderProductData(int.Parse(storageCapacity), dataItems.ToArray());
        //         items.Add(bd);
        //         dataItems.Clear();
        //     }
        //
        //     for (var i = 1; i < sheet.Count; ++i)
        //     {
        //         var workerNum = sheet[i][0];
        //
        //         if (!string.IsNullOrEmpty(workerNum) && workerNum != storageCapacity)
        //         {
        //             if (storageCapacity.Length > 0)
        //                 AddNewItem();
        //             storageCapacity = workerNum;
        //         }
        //
        //         var item = (OrderProductDataItem)GDDDownloader.CreateObjectFromData(typeof(OrderProductDataItem), header, sheet[i]);
        //         dataItems.Add(item);
        //     }
        //     AddNewItem();
        //     dataStorage.SetData(typeof(OrderProductData), items.ToArray());
        // }

        /*
         internal static void CustomBuildingsEffectUpgradesParser(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
        {
            var buildingIds = new HashSet<string>();
            var header = sheet[2];
            header.ForEach(cell =>
            {
                if (cell.ToLower() != "lvl") buildingIds.Add(cell);
            });
            var buildingsCount = buildingIds.Count;

            Type[] EffectTypes = new Type[] {
                typeof(BuildingsQueueCapacityUpgradeData),
                typeof(BuildingsInnerCapacityUpgradeData)
            };

            double BaseUpgradeCost;
            double LevelUpModifier;
            int Condition;
            int Effect;

            // Effect -> Building -> Levels
            var items = new List<BuildingEffectUpgradeLevelData>[EffectTypes.Length, buildingsCount];

            for (int i = 3; i < sheet.Count; ++i)
            {
                List<string> row = sheet[i];
                if (row[0].ToLower() == "lvl") break;
                for (int effectIndex = 0, effectIndexOffset; effectIndex < EffectTypes.Length; effectIndex++)
                {
                    effectIndexOffset = effectIndex * (1 + buildingsCount * 4);
                    for (int buildingIndex = 0; buildingIndex < buildingsCount; buildingIndex++)
                    {
                        BaseUpgradeCost = doubleTryParse(row[effectIndexOffset + buildingIndex + 1]);
                        LevelUpModifier = doubleTryParse(row[effectIndexOffset + buildingIndex + 1 + buildingsCount]);
                        Condition = intTryParse(row[effectIndexOffset + buildingIndex + 1 + buildingsCount * 2]);
                        Effect = intTryParse(row[effectIndexOffset + buildingIndex + 1 + buildingsCount * 3]);

                        if (BaseUpgradeCost == 0 && LevelUpModifier == 0 && Condition == 0 && Effect == 0)
                        {
                            continue;
                        }

                        var item = new BuildingEffectUpgradeLevelData(BaseUpgradeCost, LevelUpModifier, Condition, Effect);

                        if (items[effectIndex, buildingIndex] == null)
                            items[effectIndex, buildingIndex] = new List<BuildingEffectUpgradeLevelData>();
                        items[effectIndex, buildingIndex].Add(item);
                    }
                }
            }

            List<string> buildingsNames = sheet[2];


            for (int effectIndex = 0; effectIndex < EffectTypes.Length; effectIndex++)
            {
                var effectData = new List<BuildingsEffectUpgradeData>();
                for (int buildingIndex = 0; buildingIndex < buildingsCount; buildingIndex++)
                {
                    BuildingsEffectUpgradeData data = (BuildingsEffectUpgradeData)Activator.CreateInstance(
                        EffectTypes[effectIndex],
                        new object[] {
                            buildingsNames[buildingIndex + 1],
                            items[effectIndex, buildingIndex].ToArray()
                        }
                    );
                    effectData.Add(data);
                }
                dataStorage.SetData(EffectTypes[effectIndex], effectData.ToArray());
            }
        }
        */

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

        private static int intTryParse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            try
            {
                return int.Parse(value);
            }
            catch (Exception e)
            {
                Debug.LogError("Cannot parse " + value + " to int: " + e.ToString());
                return 0;
            }
        }

        private static double doubleTryParse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            try
            {
                return double.Parse(value);
            }
            catch (Exception e)
            {
                Debug.LogError("Cannot parse " + value + " to double: " + e.ToString());
                return 0;
            }
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

    [System.Serializable]
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
        {
            return Left + " / " + Right;
        }
    }

    [System.Serializable]
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
            {
                intervals[i] = double.Parse(parts[i].Trim());
            }

            RangeArray rangeArray = new RangeArray(intervals);

            return rangeArray;
        }
    }

    [System.Serializable]
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
                {
                    intervals[i] = parts[i];
                }

                if (typeof(T) == typeof(double))
                {
                    intervals[i] = double.Parse(parts[i].Trim());
                }

                if (typeof(T) == typeof(int))
                {
                    intervals[i] = int.Parse(parts[i].Trim());
                }
            }

            ObjectArray<T> objArray = new ObjectArray<T>(intervals);

            return objArray;
        }
    }
}

