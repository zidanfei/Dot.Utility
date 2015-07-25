#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace Dot.Utility
{
    /// <summary>
    /// INI文件操作辅助类
    /// </summary>
    public class INIHelper
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public INIHelper()
        {
            this._FileName = string.Empty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileName">Name of the file</param>
        public INIHelper(string FileName)
        {
            this.FileName = FileName;
        }

        private void InternalLoad(string Location)
        {
            LoadFromData(this.ReadFileInfo(new FileInfo(Location)));
        }

        private string ReadFileInfo(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                return "";
            using (StreamReader Reader = fileInfo.OpenText())
            {
                string Contents = Reader.ReadToEnd();
                return Contents;
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Writes a change to an INI file
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <param name="Value">Value</param>
        public virtual void WriteToINI(string Section, string Key, string Value)
        {
            if (FileContents.Keys.Contains(Section))
            {
                if (FileContents[Section].Keys.Contains(Key))
                {
                    FileContents[Section][Key] = Value;
                }
                else
                {
                    FileContents[Section].Add(Key, Value);
                }
            }
            else
            {
                Dictionary<string, string> TempDictionary = new Dictionary<string, string>();
                TempDictionary.Add(Key, Value);
                FileContents.Add(Section, TempDictionary);
            }
            Save(_FileName);
        }

        /// <summary>
        /// Reads a value from an INI file
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <param name="DefaultValue">Default value if it does not exist</param>
        public virtual string ReadFromINI(string Section, string Key, string DefaultValue = "")
        {
            if (FileContents.Keys.Contains(Section) && FileContents[Section].Keys.Contains(Key))
                return FileContents[Section][Key];
            return DefaultValue;
        }

        /// <summary>
        /// Returns an XML representation of the INI file
        /// </summary>
        /// <returns>An XML representation of the INI file</returns>
        public virtual string ToXML()
        {
            if (string.IsNullOrEmpty(this.FileName))
                return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<INI>\r\n</INI>";
            StringBuilder Builder = new StringBuilder();
            Builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
            Builder.Append("<INI>\r\n");
            foreach (string Header in FileContents.Keys)
            {
                Builder.Append("<section name=\"" + Header + "\">\r\n");
                foreach (string Key in FileContents[Header].Keys)
                {
                    Builder.Append("<key name=\"" + Key + "\">" + FileContents[Header][Key] + "</key>\r\n");
                }
                Builder.Append("</section>\r\n");
            }
            Builder.Append("</INI>");
            return Builder.ToString();
        }

        /// <summary>
        /// Deletes a section from the INI file
        /// </summary>
        /// <param name="Section">Section to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public virtual bool DeleteFromINI(string Section)
        {
            bool ReturnValue = false;
            if (FileContents.ContainsKey(Section))
            {
                ReturnValue = FileContents.Remove(Section);
                Save(_FileName);
            }
            return ReturnValue;
        }

        /// <summary>
        /// Deletes a key from the INI file
        /// </summary>
        /// <param name="Section">Section the key is under</param>
        /// <param name="Key">Key to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public virtual bool DeleteFromINI(string Section, string Key)
        {
            bool ReturnValue = false;
            if (FileContents.ContainsKey(Section) && FileContents[Section].ContainsKey(Key))
            {
                ReturnValue = FileContents[Section].Remove(Key);
                Save(_FileName);
            }
            return ReturnValue;
        }

        private void Save(string fileName)
        {
            SaveFile(new FileInfo(fileName), this.GetContentString());
        }

        private string GetContentString()
        {
            StringBuilder Builder = new StringBuilder();
            foreach (string Header in FileContents.Keys)
            {
                Builder.Append("[" + Header + "]\r\n");
                foreach (string Key in FileContents[Header].Keys)
                    Builder.Append(Key + "=" + FileContents[Header][Key] + "\r\n");
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Convert the INI to a string
        /// </summary>
        /// <returns>The INI file as a string</returns>
        public override string ToString()
        {
            StringBuilder Builder = new StringBuilder();
            foreach (string Header in FileContents.Keys)
            {
                Builder.Append("[" + Header + "]\r\n");
                foreach (string Key in FileContents[Header].Keys)
                    Builder.Append(Key + "=" + FileContents[Header][Key] + "\r\n");
            }
            return Builder.ToString();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Loads the object from the data specified
        /// </summary>
        /// <param name="Data">Data to load into the object</param>
        private void LoadFromData(string Data)
        {
            FileContents = new Dictionary<string, Dictionary<string, string>>();
            string Contents = Data;
            Regex Section = new Regex("[" + Regex.Escape(" ") + "\t]*" + Regex.Escape("[") + ".*" + Regex.Escape("]\r\n"));
            string[] Sections = Section.Split(Contents);
            MatchCollection SectionHeaders = Section.Matches(Contents);
            int Counter = 1;
            foreach (Match SectionHeader in SectionHeaders)
            {
                string[] Splitter = { "\r\n" };
                string[] Splitter2 = { "=" };
                string[] Items = Sections[Counter].Split(Splitter, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string> SectionValues = new Dictionary<string, string>();
                foreach (string Item in Items)
                {
                    SectionValues.Add(Item.Split(Splitter2, StringSplitOptions.None)[0], Item.Split(Splitter2, StringSplitOptions.None)[1]);
                }
                FileContents.Add(SectionHeader.Value.Replace("[", "").Replace("]\r\n", ""), SectionValues);
                ++Counter;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the file
        /// </summary>
        public virtual string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
                InternalLoad(_FileName);
            }
        }

        private Dictionary<string, Dictionary<string, string>> FileContents
        {
            get;
            set;
        }

        private string _FileName = string.Empty;

        #endregion

        private void SaveFile(System.IO.FileInfo file, string content)
        {
            var tempContent = (new ASCIIEncoding()).GetBytes(content);
            new DirectoryInfo(file.DirectoryName).Create();
            using (FileStream Writer = file.Open(FileMode.Open, FileAccess.Write))
            {
                Writer.Write(tempContent, 0, tempContent.Length);
            }
        }
    }
}