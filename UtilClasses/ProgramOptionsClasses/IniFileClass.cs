using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable StringLiteralTypo

namespace UtilClasses.ProgramOptionsClasses
{
    public class IniFileClass
    {
        public ProgramOptions Options { get; } = new();

        private readonly IniFile _iniFileObj;

        public IniFileClass(IProgramOptionsEvent optionsEvent)
        {
            IProgramOptionsEvent programOptionsEvent = optionsEvent;

            _iniFileObj = new IniFile("QSObjectManager.ini");

            ReadOptionsFromFile();

            programOptionsEvent.NewProgramOptionsSend += ProgramOptionsReceived;

        }

        private void ReadOptionsFromFile()
        {
            if (!_iniFileObj.KeyExists("PathHistorysRoot", "Paths"))
            {
                _iniFileObj.Write("PathHistorysRoot", "", "Paths");
                Options.RepositoryPath = "";
            }
            else
            {
                Options.RepositoryPath = _iniFileObj.Read("PathHistorysRoot", "Paths");
            }
            
            if (!_iniFileObj.KeyExists("AppContentPath", "Paths"))
            {
                _iniFileObj.Write("AppContentPath", "", "Paths");
                Options.RepositoryPath = "";
            }
            else
            {
                Options.AppContentPath = _iniFileObj.Read("AppContentPath", "Paths");
            }
            
            if (!_iniFileObj.KeyExists("ContentDefault", "Paths"))
            {
                _iniFileObj.Write("ContentDefault", "", "Paths");
                Options.ContentDefault = "";
            }
            else
            {
                Options.ContentDefault = _iniFileObj.Read("ContentDefault", "Paths");
            }
            
            if (!_iniFileObj.KeyExists("Images", "OverwriteExistingContent"))
            {
                _iniFileObj.Write("Images", "false", "OverwriteExistingContent");
                Options.OverwriteExistingContentImages = false;
            }
            else
            {
                Options.OverwriteExistingContentImages = Boolean.Parse(_iniFileObj.Read("Images", "OverwriteExistingContent"));
            }
            
            Options.LocalAddress = _iniFileObj.Read("LocAddr", "LocalAddr");
            Options.RemoteAddress = _iniFileObj.Read("RemAddr", "RemoteAddr");

        }

        private void ProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(Options);
            
            UpdateOptionsInIniFile(Options);

        }

        public ProgramOptions GetOptions()
        {
            return Options.Copy();
        }



        private void UpdateOptionsInIniFile(ProgramOptions opts)
        {
            _iniFileObj.Write("PathHistorysRoot", opts.RepositoryPath, "Paths");
            _iniFileObj.Write("AppContentPath", opts.AppContentPath, "Paths");
            _iniFileObj.Write("ContentDefault", opts.ContentDefault, "Paths");
            _iniFileObj.Write("LocAddr", opts.LocalAddress, "LocalAddr");
            _iniFileObj.Write("RemAddr", opts.RemoteAddress, "RemoteAddr");
            _iniFileObj.Write("Images",opts.OverwriteExistingContentImages.ToString(), "OverwriteExistingContent");

        }

    }

    
   

    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class IniFile // revision 11
    {
        string _path;
        string _exe = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string section, string key, string @default, StringBuilder retVal,
            int size, string filePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iniPath"></param>
        public IniFile(string iniPath = null)
        {
            _path = new FileInfo(iniPath ?? _exe + ".ini").FullName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public string Read(string key, string section = null)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? _exe, key, "", retVal, 255, _path);
            return retVal.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="section"></param>
        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? _exe, key, value, _path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section ?? _exe);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        public void DeleteSection(string section = null)
        {
            Write(null, null, section ?? _exe);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }
    }
}
