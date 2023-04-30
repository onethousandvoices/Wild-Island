using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;
using System.Reflection;
using System.Threading;
using System.Globalization;
using ExcelDataReader;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WildIsland.Data
{
    public class GDDDownloader
    {
        private bool InProcess => _sheetCounter > 0;

        private int _sheetCounter = 0;
        private static string _oldSeparator, _newSeparator;

        private static string GdXlsxFile => Application.dataPath + "/Editor/Data/gd.xlsx";
        private static string LdXlsxFile => Application.dataPath + "/Editor/Data/ld.xlsx";

        private const string LoadXlsxUri = "https://docs.google.com/spreadsheets/d/{0}/export?format=xlsx";

        [UnityEditor.MenuItem("Tools/GDD/Download, parse, copy", priority = 26)]
        private static async void LoadGDDFilesParseCopy()
        {
            await LoadGDDXlsxs();
            ParseGDDFromExcelAndCopy();
        }
        
        // [UnityEditor.MenuItem("Tools/GDD/Download gdd xlsxs", priority = 27)]
        private static async Task LoadGDDXlsxs()
        {
            await DownloadXlsxs();
            Debug.LogError("GDD Xlsxs downloaded");
        }
        
        // [UnityEditor.MenuItem("Tools/GDD/Parse excel with copy", priority = 28)]
        private static void ParseGDDFromExcelAndCopy()
        {
            ParseGDDFromExcel(true);
        }
        
        private static void ParseGDDFromExcel(bool copy)
        {
            Dictionary<string, GameDataBase> _gameDataContainer = new Dictionary<string, GameDataBase>()
            {
                {"gameData", new GameData()},
                {"loadGameData", new LoadGameData()}
            };
            
            ParseExcelToGd(_gameDataContainer);

            Debug.Log("Parse complete");

            foreach (KeyValuePair<string, GameDataBase> keyValue in _gameDataContainer)
            {
                keyValue.Value.Save(keyValue.Key, !copy);
            }
            Debug.Log("Saving complete");
        }

        private static async Task DownloadXlsxs()
        {
            if (File.Exists(GdXlsxFile))
                File.Delete(GdXlsxFile);
            if (File.Exists(LdXlsxFile))
                File.Delete(LdXlsxFile);
            WebClient downloader = new WebClient();
            await downloader.DownloadFileTaskAsync(new Uri(string.Format(LoadXlsxUri, GDDDownloaderConfig.DefaultSheetID)),
                GdXlsxFile);
            // await downloader.DownloadFileTaskAsync(new Uri(string.Format(LoadXlsxUri, GDDDownloaderConfig.LocalizationSheetID)),
                // LdXlsxFile);
        }

        private static void ParseExcelToGd(Dictionary<string, GameDataBase> gdContainer)
        {
            SetSeparators();
            ParseExcelToGd(GdXlsxFile, gdContainer, FileGdId.Main);
            // ParseExcelToGd(LdXlsxFile, gdContainer, FileGdId.Localization);
        }

        private static void ParseExcelToGd(string filePath, IReadOnlyDictionary<string, GameDataBase> gdContainer, FileGdId gdId)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File " + filePath + " not exist");
            }

            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

            do
            {
                List<List<string>> rows = new List<List<string>>(reader.RowCount);
                while (reader.Read())
                {
                    List<string> row = new List<string>(reader.FieldCount);
                    int cellsReallyCount = 0;
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        CellRange[] a = reader.MergeCells;
                        object value = reader.GetValue(i);
                        row.Add(value?.ToString());
                        if (value != null)
                            cellsReallyCount = i + 1;
                    }

                    if (cellsReallyCount <= 0)
                        continue;
                    row.RemoveRange(cellsReallyCount, row.Count - cellsReallyCount);
                    rows.Add(row);
                }

                foreach (GDDDownloaderConfig.WorksheetConfig sheetInfo in GDDDownloaderConfig.ParsingDataTypes)
                {
                    if (sheetInfo.SheetID != gdId || string.Compare(sheetInfo.TableID, reader.Name, StringComparison.Ordinal) != 0)
                        continue;
                    
                    if (sheetInfo.CustomAction == null)
                        ReadWorksheet(sheetInfo, rows, gdContainer[sheetInfo.GameDataId]);
                    else
                        sheetInfo.CustomAction(sheetInfo, rows, gdContainer[sheetInfo.GameDataId]);
                }
            }
            while (reader.NextResult());

            reader.Dispose();
            stream.Dispose();
        }

        public async Task Download(Dictionary<string, GameDataBase> gdContainer)
        {
            if (InProcess)
                throw new Exception("Download in process, wait until it ends");

            SetSeparators();
            foreach (GDDDownloaderConfig.WorksheetConfig sheetInfo in GDDDownloaderConfig.ParsingDataTypes)
            {
                string gameDataId = sheetInfo.GameDataId;
                Interlocked.Increment(ref _sheetCounter);
                string sheetID = sheetInfo.SheetID switch
                                 {
                                     FileGdId.Main         => GDDDownloaderConfig.DefaultSheetID,
                                     // FileGdId.Localization => GDDDownloaderConfig.LocalizationSheetID,
                                     _                     => string.Empty
                                 };

                SpreadsheetManager.ReadPublicSpreadsheet(
                    new GSTU_Search(
                        sheetID,
                        sheetInfo.TableID,
                        "A1", "EE1000"
                    ),
                    sheet =>
                    {
                        if (sheetInfo.CustomAction == null)
                            ReadWorksheet(sheetInfo, sheet.ConvertRowsToListString(), gdContainer[gameDataId]);
                        else
                            sheetInfo.CustomAction(sheetInfo, sheet.ConvertRowsToListString(), gdContainer[gameDataId]);
                        Interlocked.Decrement(ref _sheetCounter);
                    }
                );
                Thread.Sleep(250);
            }

            while (_sheetCounter > 0)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
        }

        private static void SetSeparators()
        {
            string currentSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            _oldSeparator = (currentSeparator.Equals(".")) ? "," : ".";
            _newSeparator = (currentSeparator.Equals(".")) ? "." : ",";
        }

        private static void ReadWorksheet(GDDDownloaderConfig.WorksheetConfig sheetConfig, List<List<string>> sheet, IGDDDataStorage dataStorage)
        {
            List<string> header = sheet[0];
            List<object> items = new List<object>();

            for (int i = 1; i < sheet.Count; ++i)
            {
                object item = CreateObjectFromData(sheetConfig.DataType, header, sheet[i]);
                items.Add(item);
            }
            dataStorage.SetData(sheetConfig.DataType, items.ToArray());
        }

        public static object CreateObjectFromData(System.Type type, List<string> header, List<string> values, int? rawIndex = null)
        {
            object item = Activator.CreateInstance(type);

            for (int i = 0; i < header.Count; i++)
            {
                if (header[i] == null)
                    continue;
                try
                {
                    FieldInfo field = GetFieldByColumnName(type, header[i]);
                    if (field == null || values.Count <= i)
                        continue;

                    if (field.FieldType == typeof(float))
                        field.SetValue(item, float.Parse(values[i].Replace(_oldSeparator, _newSeparator)));
                    else if (field.FieldType == typeof(int))
                        field.SetValue(item, int.Parse(values[i]));
                    else if (field.FieldType == typeof(double))
                        field.SetValue(item, double.Parse(values[i].Replace(_oldSeparator, _newSeparator)));
                    else if (field.FieldType == typeof(Range))
                        field.SetValue(item, Range.Parse(values[i].Replace(_oldSeparator, _newSeparator)));
                    else if (field.FieldType == typeof(RangeArray))
                        field.SetValue(item, RangeArray.Parse(values[i].Replace(_oldSeparator, _newSeparator)));
                    else if (field.FieldType == typeof(ObjectArray<string>))
                        field.SetValue(item, ObjectArray<string>.Parse(values[i].Replace(_oldSeparator, _newSeparator)));
                    else if (field.FieldType == typeof(string))
                        field.SetValue(item, values[i]);
                    else if (field.FieldType.IsEnum)
                        field.SetValue(item, Enum.Parse(field.FieldType, values[i], true));
                    else if (field.FieldType == typeof(bool))
                        field.SetValue(item, values[i].ToLower() == "true" || values[i] == "1");
                    else
                        throw new NotImplementedException(field.FieldType + " is not implemented yet by downloader");
                }
                catch (Exception e)
                {
                    Debug.LogError("Cannot convert value " + values[i] + " at column " + header[i] + " to field " + GetFieldByColumnName(type, header[i]) + " of " + type + "\n" + e);
                }
            }
            return item;
        }

        private static FieldInfo GetFieldByColumnName(Type type, string columnName)
        {
            foreach (FieldInfo field in type.GetFields())
            {
                SheetColumnNameAttribute attribute = field.GetCustomAttribute<SheetColumnNameAttribute>();
                if (attribute == null)
                    continue;
                if (string.Equals(attribute.columnName, columnName, StringComparison.CurrentCultureIgnoreCase))
                    return field;
            }
            return null;
        }
    }

    internal static class Extensions
    {
        /// <summary>
        /// returns a List<List<string>> of values in Row
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static List<List<string>> ConvertRowsToListString(this GstuSpreadSheet sheet)
        {
            List<KeyValuePair<int, List<GSTU_Cell>>> rowsList = sheet.rows.primaryDictionary.ToList();
            return rowsList.Select(row => row.Value.Select(val => val.value).ToList()).ToList();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SheetColumnNameAttribute : Attribute
    {
        public readonly string columnName;

        public SheetColumnNameAttribute(string columnName)
        {
            this.columnName = columnName;
        }
    }
}