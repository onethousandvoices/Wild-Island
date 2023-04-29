using System;
using System.Collections.Generic;
using System.Linq;
using WildIsland.Data;

namespace Data
{
public class SupportedAppVersionDataParse
    {
        [SheetColumnName("GameDataVersion")]
        public int GameDataVersion;
        [SheetColumnName("FirstSupportedAppVersionAndroid")]
        public string AndroidVersion;
        [SheetColumnName("FirstSupportedAppVersioniOS")]
        public string IOSVersion;
    }

    [Serializable]
    public class SupportedAppVersionData
    {
        public int GameDataVersion;
        public AppVersion AndroidVersion, IOSVersion;

        private static string[] _tags = new string[] { "GameDataVersion", "AndroidVersion", "iOSVersion" };


        public static SupportedAppVersionData Parse(string input)
        {
            try
            {
                List<string> splitInput = input.Split('|', ':').ToList();
                SupportedAppVersionData data = new SupportedAppVersionData
                {
                    GameDataVersion = int.Parse(splitInput[splitInput.FindIndex(x => x == _tags[0]) + 1]),
                    AndroidVersion = AppVersion.Parse(splitInput[splitInput.FindIndex(x => x == _tags[1]) + 1]),
                    IOSVersion = AppVersion.Parse(splitInput[splitInput.FindIndex(x => x == _tags[2]) + 1])
                };
                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return _tags[0] + ":" + GameDataVersion.ToString() + "|" + _tags[1] + ":" + AndroidVersion.ToString() + "|" + _tags[2] + ":" + IOSVersion.ToString();
        }
    }

    [Serializable]
    public struct AppVersion
    {
        public short[] Version;


        public AppVersion(short[] version)
        {
            Version = version;
        }

        public bool CorrectForGameData(AppVersion currentAppVersion)
        {
            short[] appVersion = currentAppVersion.Version;
            int length = Math.Max(Version.Length, appVersion.Length);
            for (int i = 0; i < length; ++i)
            {
                if (appVersion[i] > Version[i]) return true;
                else if (appVersion[i] < Version[i]) return false;
            }
            return true;
        }

        public static AppVersion Parse(string str)
        {
            short[] version = new short[3];
            str = string.Join(string.Empty, str.Where(c => char.IsDigit(c) || c.Equals('.')));
            string[] s = str.Split('.');

            for (int i = 0; i < version.Length; i++)
            {
                version[i] = short.Parse(s[i]);
            }

            return new AppVersion(version);
        }

        public override string ToString()
        {
            if (Version == null || Version.Length == 0) return string.Empty;
            string ver = string.Empty;
            for (int i = 0; i < Version.Length - 1; ++i)
            {
                ver += Version[i].ToString() + ".";
            }
            ver += Version[Version.Length - 1].ToString();
            return ver;
        }
    }
}