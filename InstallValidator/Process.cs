using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace InstallValidator
{
    public class InstallLocInfo
    {
        public string Name = null;
        public string FileName = null;
        public string Directory = null;
        public string Path = null;
        public string Message = null;

        public InstallLocInfo(object obj)
        {
            this.ParseJson(obj);
        }

        private void ParseJson( object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null)
            {
                Process.ParseError = true;
                Process.AddParseErrorMsg = "[InstallValidator] No data in dictionary (ParseJson)";
                Log.Error("No data in dictionary (ParseJson)");
                throw new ArgumentException("[InstallValidator] No data in dictionary (ParseJson)");
            }

            foreach (var key in data.Keys)
            {
                switch (key)
                {
                    case "NAME":
                        this.Name =(string)data[key];
                        Log.Info("NAME: " + Name);
                        break;

                    case "FILE":
                        this.FileName = (string)data[key];
                        Log.Info("FILE: " + FileName);
                        break;

                    case "DIRECTORY":
                        this.Directory = (string)data[key];
                        Log.Info("DIRECTORY: " + Directory);
                        break;
                    case "PATH":
                        this.Path = (string)data[key];
                        Log.Info("PATH: " + Path);
                        break;

                    case "MESSAGE":
                        this.Message = (string)data[key];
                        Log.Info("MESSAGE: " + Message);
                        break;
                }
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class Process: MonoBehaviour
    {
        internal static List<string> parseErrorMsgs = new List<string>();

        //List<InstallLocInfo> installLocInfoList = new List<InstallLocInfo>();
        public static bool ParseError { get; set; }
        public static string AddParseErrorMsg { set { parseErrorMsgs.Add(value); } }

        string ModName = "";    // This comes from the main NAME in the .version file

        bool DoProcess(string fname)
        {
            string json = File.ReadAllText("GameData/" + fname);
            var data = Json.Deserialize(json) as Dictionary<string, object>;
            if (data == null)
            {
                ParseError = true;
                AddParseErrorMsg = "[InstallValidator] Error in Json.Deserialize, file: " + fname;
                Log.Error("[InstallValidator] Error in Json.Deserialize, file: " + fname);
                return false;
            }
            
            foreach (var key in data.Keys)
            {
                if (key == "NAME")
                {
                    ModName = (string)data[key];
                }
                if (key == "INSTALL_LOC" || key.StartsWith("INSTALL_LOC_"))
                {
                    var installLocInfo = new InstallLocInfo(data[key]);
                    //installLocInfoList.Add(installLocInfo);
                    ValidateInstallLoc(key, installLocInfo);
                }
            }
            return true;
        }

        void ValidateInstallLoc(string stanza, InstallLocInfo ili)
        {
            string fullPath = "GameData/" + ili.Path;
             if (ili.Path != null && !Directory.Exists(fullPath))
            {
                ParseError = true;
                AddParseErrorMsg = ProcessMessage("Path", ili, stanza,
                                        "<MODNAME> has been installed incorrectly and will not function properly. All files should be located in KSP/GameData/<PATH>/<DIRECTORY>. Do not move any files from inside that folder.");

                Log.Error("Missing path: " + ili.Path);
                return;
            }
            fullPath += "/" + ili.Directory;
            fullPath.Replace("//", "/");

            if (ili.Directory != null && !Directory.Exists(fullPath))
            {
                ParseError = true;
                AddParseErrorMsg = ProcessMessage("Directory", ili, stanza,
                                        "<MODNAME> has been installed incorrectly and will not function properly. All files should be located in KSP/GameData/<PATH>/<DIRECTORY>. Do not move any files from inside that folder.");

                Log.Error("Missing directory: " + ili.Directory);
                return;
            }
            fullPath += "/" + ili.FileName;           
            fullPath.Replace("//", "/");
            if (!File.Exists(fullPath))
            {
                ParseError = true;

                AddParseErrorMsg = ProcessMessage("File", ili, stanza,
                     "<MODNAME> has been installed incorrectly and will not function properly. All files should be located in KSP/GameData/<PATH>/<DIRECTORY>. Do not move any files from inside that folder.");
                Log.Error("Missing file: " + ili.FileName);
                return;
            }
        }

        string ProcessMessage(string fieldName, InstallLocInfo ili, string stanza, string defaultMsg)
        {
            if (ili.Message == null || ili.Message == "")
                return defaultMsg;

            if (ili.Name == null)
                ili.Name = ModName;
            string msg = ili.Message;

            msg = msg.Replace("<MODNAME>", ili.Name);
            msg = msg.Replace("<FILE>", ili.FileName);
            msg = msg.Replace("<DIRECTORY>", ili.Directory);
            msg = msg.Replace("<PATH>", ili.Path);
            msg = msg.Replace("<STANZA>", stanza);
            msg = msg.Replace("<FIELD>", fieldName);
            msg.Replace("//", "/");
            return msg;
        }

        public void Start()
        {
            Log.Info("[InstallValidator] Start");
            DoProcess("InstallValidator/test.version");
            if (ParseError)
            {
                gameObject.AddComponent<IssueGui>();
            }
        }
    }
}
