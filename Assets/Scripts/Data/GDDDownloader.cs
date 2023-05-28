using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Globalization;
using ExcelDataReader;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using WildIsland.Utility;

namespace WildIsland.Data
{
    public class GDDDownloader
    {
        private static string GddFile => Application.dataPath + "/Resources/gd.xlsx";
        private static string GddSO => Application.dataPath + "/Resource/gdd.asset";

        private const string LoadXlsxUri = "https://docs.google.com/spreadsheets/d/{0}/export?format=xlsx";
        private const string DefaultSheetID = "1zbkPxEXDlor0Kvwleha7AOkWgECHD9hUXKgDkKeFNGU";
        
#if UNITY_EDITOR
        [MenuItem("Tools/Download Gdd", priority = 26)]
#endif
        private static async void LoadGDDFilesParseCopy()
        {
            await Download();
            Parse();
        }

        private static async Task Download()
        {
            if (File.Exists(GddFile))
                File.Delete(GddFile);

            WebClient downloader = new WebClient();
            await downloader.DownloadFileTaskAsync(new Uri(string.Format(LoadXlsxUri, DefaultSheetID)),
                GddFile);
        }

        private static void Parse()
        {
            if (!File.Exists(GddFile))
                throw new Exception("File " + GddFile + " not exist");

            if (File.Exists(GddSO))
                File.Delete(GddSO);

            Gdd newGdd = ScriptableObject.CreateInstance<Gdd>();
            AssetDatabase.CreateAsset(newGdd, "Assets/Resources/gdd.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(newGdd);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newGdd;

            FileStream stream = new FileStream(GddFile, FileMode.Open, FileAccess.Read);
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

                foreach (GddSheet sheet in Gdd.ParsingSheets)
                {
                    if (string.Compare(sheet.SheetId, reader.Name, StringComparison.OrdinalIgnoreCase) != 0)
                        continue;

                    if (sheet.CustomAction == null)
                        ReadSheet(sheet, rows, newGdd);
                }
            }
            while (reader.NextResult());

            reader.Dispose();
            stream.Dispose();

            Debug.Log("Parse complete");
        }

        private static void ReadSheet(GddSheet sheet, IReadOnlyList<List<string>> loadedSheet, Gdd gdd)
        {
            List<string> header = loadedSheet[0];
            object data = Activator.CreateInstance(sheet.DataType);

            for (int i = 1; i < loadedSheet.Count; ++i)
            {
                IEnumerable<FieldInfo> fields = data.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                    SheetAttribute attribute = field.GetCustomAttribute<SheetAttribute>();
                    
                    if (attribute != null)
                    {
                        for (int j = 0; j < loadedSheet[i].Count; j++)
                        {
                            if (!loadedSheet[i][0].Equals(attribute.ColumnName))
                                continue;
                            FieldSetValue(data, field, loadedSheet[i][1]);
                            break;
                        }
                        continue;
                    }

                    string loadedLower = loadedSheet[i][0].ToLower().Replace(".", "");
                    string fieldLower = field.Name.ToLower();

                    if (!loadedLower.Contains(fieldLower) && !fieldLower.Contains(loadedLower))
                        continue;

                    object item = CreateObjectFromData(field.FieldType, header, loadedSheet[i]);
                    data.SetData(item);
                }
            }

            gdd.SetData(data);
        }

        private static object CreateObjectFromData(Type type, IReadOnlyList<string> header, IReadOnlyList<string> values)
        {
            object item = Activator.CreateInstance(type);

            for (int i = 0; i < header.Count; i++)
            {
                if (header[i] == null)
                    continue;
                try
                {
                    FieldInfo field = GetFieldByColumnName(type, header[i]);
                    if (values.Count <= i)
                        continue;
                    FieldSetValue(item, field, values[i]);
                }
                catch (Exception e)
                {
                    Debug.LogError("Cannot convert value " + values[i] + " at column " + header[i] + " to field " + GetFieldByColumnName(type, header[i]) + " of " + type + "\n" + e);
                }
            }
            return item;
        }

        private static void FieldSetValue(object item, FieldInfo field, string value)
        {
            if (field == null)
                return;

            if (field.FieldType == typeof(float))
                field.SetValue(item, float.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
            else if (field.FieldType == typeof(int))
                field.SetValue(item, int.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
            else if (field.FieldType == typeof(double))
                field.SetValue(item, double.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
            else if (field.FieldType == typeof(string))
                field.SetValue(item, value);
            else if (field.FieldType.IsEnum)
                field.SetValue(item, Enum.Parse(field.FieldType, value, true));
            else if (field.FieldType == typeof(bool))
                field.SetValue(item, value.ToLower() == "true" || value == "1");
            else if (field.FieldType.BaseType == typeof(PlayerStat) || field.FieldType.BaseType == typeof(VolatilePlayerStat))
            {
                object playerStat = Activator.CreateInstance(field.FieldType);
                PropertyInfo[] properties = playerStat.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in properties)
                    propertyInfo.SetValue(playerStat, float.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
                
                item.SetData(playerStat);
            }
            else
                throw new NotImplementedException(field.FieldType + " is not implemented yet by downloader");
        }

        private static FieldInfo GetFieldByColumnName(Type type, string columnName)
        {
            return type.GetFields()
                       .Select(field => new { field, attribute = field.GetCustomAttribute<SheetAttribute>() })
                       .Where(@t => @t.attribute != null)
                       .Where(@t => string.Equals(@t.attribute.ColumnName, columnName, StringComparison.CurrentCultureIgnoreCase))
                       .Select(@t => @t.field)
                       .FirstOrDefault();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SheetAttribute : Attribute
    {
        public readonly string ColumnName;

        public SheetAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}