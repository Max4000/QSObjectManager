using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilClasses.ProgramOptionsClasses
{
    public class IniFileClass
    {
        public ProgramOptions Options { get; } = new ProgramOptions();

        private readonly IniFile _iniFileObj;

        public IniFileClass(IProgramOptionsEvent observable)
        {
            IProgramOptionsEvent observableObject = observable;

            _iniFileObj = new IniFile("QSObjectManager.ini");

            ReadOptionsFromFile();

            observableObject.NewProgramOptionsSend += _observableObject_NewProgramOptionsSend;

        }

        private void ReadOptionsFromFile()
        {
            if (!_iniFileObj.KeyExists("PathHistorysRoot", "Paths"))
            {
                _iniFileObj.Write("PathHistorysRoot", "", "Paths");
                this.Options.RepositoryPath = "";
            }
            else
            {
                this.Options.RepositoryPath = _iniFileObj.Read("PathHistorysRoot", "Paths");
            }

        }

        private void _observableObject_NewProgramOptionsSend(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(this.Options);
            
            UpdateOptionsInIniFile(this.Options);

        }

        public ProgramOptions GetOptions()
        {
            return this.Options.Clone();
        }



        private void UpdateOptionsInIniFile(ProgramOptions opts)
        {
            _iniFileObj.Write("PathHistorysRoot", opts.RepositoryPath, "Paths");

        }

    }

    public class ProgramOptions
    {
        public ProgramOptions(string repositoryPath)
        {
            RepositoryPath = repositoryPath;
        }

        public ProgramOptions()
        {

        }

        public void Copy(ProgramOptions anotherOptions)
        {
            anotherOptions.RepositoryPath = this.RepositoryPath;
        }

        public ProgramOptions Clone()
        {
            return new(this.RepositoryPath);
        }


        public string RepositoryPath { get; set; }
    }

    public class ProgramOptionsEventArgs : EventArgs
    {

        public readonly ProgramOptions ProgramOptions;

        //Конструкторы
        public ProgramOptionsEventArgs(ProgramOptions record)
        {
            ProgramOptions = record;
        }
    }

    public interface IProgramOptionsEvent
    {
        event NewProgramOptionsHandler NewProgramOptionsSend;
    }

    public delegate void NewProgramOptionsHandler(object sender, ProgramOptionsEventArgs e);

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
