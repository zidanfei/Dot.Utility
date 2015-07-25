using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Drawing;
using System.Security.Permissions;
using System.Security;
using System.Reflection;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Windows.Forms;

namespace Dot.Utility.Media.Diagram
{
    [Serializable]
    internal sealed class DiagramViewLicenseProvider : LicenseProvider
    {
        static DiagramViewLicenseProvider()
        {
            DiagramViewLicenseProvider.GONAME = "GoDiagram";
            DiagramViewLicenseProvider.err = false;
        }

        public DiagramViewLicenseProvider()
        {
        }

        private int Dispose(string keystring, bool run)
        {
            int num18 = 0;
            if (keystring.Length == 0)
            {
                return 0;
            }
            try
            {
                byte[] buffer1 = Convert.FromBase64String(keystring);
                byte[] buffer2 = new byte[0x10] { 0x21, 0x53, 0x1b, 0x5f, 0x1c, 0x54, 0xc5, 0xa9, 0x27, 0x5d, 0x4b, 0x69, 0x52, 0x61, 0x31, 0x2c };
                MemoryStream stream1 = new MemoryStream(buffer1);
                RijndaelManaged managed1 = new RijndaelManaged();
                CryptoStream stream2 = new CryptoStream(stream1, managed1.CreateDecryptor(buffer2, buffer2), CryptoStreamMode.Read);
                byte[] buffer3 = new byte[0x1000];
                int num1 = stream2.Read(buffer3, 0, buffer3.Length);
                System.Text.Decoder decoder1 = new UTF8Encoding().GetDecoder();
                char[] chArray1 = new char[decoder1.GetCharCount(buffer3, 0, num1)];
                int num2 = decoder1.GetChars(buffer3, 0, num1, chArray1, 0);
                string text1 = new string(chArray1, 0, num2);
                char[] chArray2 = new char[1] { '|' };
                string[] textArray1 = text1.Split(chArray2);
                int num3 = textArray1.Length;
                DateTime time1 = DateTime.Today;
                CultureInfo info1 = CultureInfo.CurrentCulture;
                if (run)
                {
                    if (num3 < 11)
                    {
                        return 0;
                    }
                    string text2 = (num3 > 0) ? textArray1[0] : "";
                    string text3 = (num3 > 1) ? textArray1[1] : "";
                    int num4 = (num3 > 2) ? this.ParseInt(textArray1[2]) : -1;
                    int num5 = (num3 > 3) ? this.ParseInt(textArray1[3]) : -1;
                    int num6 = (num3 > 9) ? this.ParseInt(textArray1[9]) : 7;
                    string text4 = (num3 > 10) ? textArray1[10] : "E";
                    int num7 = (num3 > 11) ? this.ParseInt(textArray1[11]) : 0x270f;
                    int num8 = (num3 > 12) ? this.ParseInt(textArray1[12]) : 1;
                    int num9 = (num3 > 13) ? this.ParseInt(textArray1[13]) : 1;
                    int num10 = (num3 > 14) ? this.ParseInt(textArray1[14]) : 360;
                    AssemblyName name1 = Assembly.GetExecutingAssembly().GetName();
                    if ((text3.Length > 0) && (string.Compare(text3, name1.Name, true, info1) != 0))
                    {
                        return 0;
                    }
                    if (num4 >= 0)
                    {
                        if (name1.Version.Major > num4)
                        {
                            return 0;
                        }
                        if (((num5 >= 0) && (name1.Version.Major == num4)) && (name1.Version.Minor > num5))
                        {
                            return 0;
                        }
                    }
                    if (text4[0] == 'E')
                    {
                        return 0;
                    }
                    if ((text4[0] == 'R') && ((DiagramView.myVersionAssembly == null) || (string.Compare(text2, DiagramView.myVersionAssembly.GetName().Name, true, info1) != 0)))
                    {
                        return 0;
                    }
                    DateTime time2 = new DateTime(num7, num8, num9);
                    if (time1.AddDays((double)num10) <= time2)
                    {
                        return 4;
                    }
                    if (time1.AddDays(7) <= time2)
                    {
                        return 6;
                    }
                    if (time1.AddDays((double)-num6) <= time2)
                    {
                        return 5;
                    }
                    goto Label_0522;
                }
                string text5 = (num3 > 1) ? textArray1[1] : "";
                int num11 = (num3 > 2) ? this.ParseInt(textArray1[2]) : -1;
                int num12 = (num3 > 3) ? this.ParseInt(textArray1[3]) : -1;
                string text6 = (num3 > 4) ? textArray1[4] : "";
                string text7 = (num3 > 5) ? textArray1[5] : "";
                int num13 = (num3 > 6) ? this.ParseInt(textArray1[6]) : 1;
                int num14 = (num3 > 7) ? this.ParseInt(textArray1[7]) : 1;
                int num15 = (num3 > 8) ? this.ParseInt(textArray1[8]) : 1;
                DateTime time3 = new DateTime(num13, num14, num15);
                int num16 = (num3 > 9) ? this.ParseInt(textArray1[9]) : 7;
                string text8 = (num3 > 10) ? textArray1[10] : "E";
                int num17 = (num3 > 14) ? this.ParseInt(textArray1[14]) : 360;
                AssemblyName name2 = Assembly.GetExecutingAssembly().GetName();
                if ((text5.Length > 0) && (string.Compare(text5, name2.Name, true, info1) != 0))
                {
                    return 0;
                }
                if (num11 >= 0)
                {
                    if (name2.Version.Major > num11)
                    {
                        return 0;
                    }
                    if (((num12 >= 0) && (name2.Version.Major == num11)) && (name2.Version.Minor > num12))
                    {
                        return 0;
                    }
                }
                if ((text6.Length > 0) && (string.Compare(Environment.MachineName, text6, true, info1) != 0))
                {
                    return 0;
                }
                if ((text7.Length > 0) && (string.Compare(Environment.UserName, text7, true, info1) != 0))
                {
                    return 0;
                }
                if (text8[0] == 'B')
                {
                    if (time1.AddDays((double)num17) <= time3)
                    {
                        return 4;
                    }
                    if (time1.AddDays(7) <= time3)
                    {
                        return 6;
                    }
                    if (time1.AddDays((double)-num16) <= time3)
                    {
                        return 5;
                    }
                    goto Label_0522;
                }
                if (time1.AddDays(7) <= time3)
                {
                    return 2;
                }
                if (time1.AddDays((double)-num16) > time3)
                {
                    goto Label_0522;
                }
                num18 = 1;
            }
            catch (Exception)
            {
            }
            return num18;
        Label_0522:
            return 0;
        }

        public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        {
            int num2;
            string[] textArray1;
            string text1 = "\nEnvironment info:\n";
            text1 = text1 + "version: " + DiagramView.VersionName + "\n";
            if (Assembly.GetEntryAssembly() != null)
            {
                text1 = text1 + "entry: ";
                try
                {
                    text1 = text1 + Assembly.GetEntryAssembly().FullName;
                }
                catch (SecurityException)
                {
                    text1 = text1 + "?fn?";
                }
                object obj1 = null;
                try
                {
                    obj1 = Assembly.GetEntryAssembly().EntryPoint;
                }
                catch (SecurityException)
                {
                    text1 = text1 + "?ep?";
                }
                if (obj1 != null)
                {
                    text1 = text1 + ", has entry point\n";
                }
                else
                {
                    text1 = text1 + ", no entry point\n";
                }
            }
            else
            {
                text1 = text1 + "null entry\n";
            }
            int num1 = 0;
            string text2 = "";
            try
            {
                try
                {
                    text2 = context.GetSavedLicenseKey(type, null);
                }
                catch (SecurityException exception1)
                {
                    text2 = null;
                    text1 = text1 + "\n" + exception1.ToString();
                }
                if (text2 != null)
                {
                    text1 = text1 + "key: " + text2 + "\n";
                }
                else
                {
                    text1 = text1 + "null key\n";
                }
                if ((text2 != null) && (text2.Length != 0))
                {
                    goto Label_01AF;
                }
                try
                {
                    Assembly[] assemblyArray1 = AppDomain.CurrentDomain.GetAssemblies();
                    Assembly[] assemblyArray2 = assemblyArray1;
                    for (num2 = 0; num2 < assemblyArray2.Length; num2++)
                    {
                        Assembly assembly1 = assemblyArray2[num2];
                        text1 = text1 + assembly1.FullName + "\n";
                        try
                        {
                            text2 = context.GetSavedLicenseKey(type, assembly1);
                        }
                        catch (SecurityException)
                        {
                            text2 = null;
                        }
                        if ((text2 != null) && (text2.Length > 0))
                        {
                            goto Label_015A;
                        }
                    }
                }
                catch (SecurityException)
                {
                }
            Label_015A:
                text1 = text1 + DiagramView.myVersionName + "\n";
                if (DiagramView.myVersionAssembly != null)
                {
                    text1 = text1 + DiagramView.myVersionAssembly.GetName().Name + "\n";
                }
                else
                {
                    text1 = text1 + "null licensed assembly\n";
                }
                if (DiagramView.myVersionName.Length > 0x18)
                {
                    text2 = DiagramView.myVersionName;
                }
            Label_01AF:
                if ((text2 != null) && (text2.Length > 0))
                {
                    num1 = this.Dispose(text2, true);
                    if (num1 == 4)
                    {
                        return new DiagramViewLicense(text2, num1);
                    }
                }
                else
                {
                    text2 = "";
                    RegistryKey key1 = null;
                    try
                    {
                        key1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Northwoods Software\Go.NET");
                    }
                    catch (SecurityException exception2)
                    {
                        text1 = text1 + "\n" + exception2.ToString();
                    }
                    if (key1 == null)
                    {
                        try
                        {
                            key1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Northwoods Software\GoDiagram");
                        }
                        catch (SecurityException)
                        {
                        }
                    }
                    if (key1 != null)
                    {
                        string text3 = type.Assembly.GetName().Name;
                        object obj2 = null;
                        try
                        {
                            obj2 = key1.GetValue(text3);
                        }
                        catch (SecurityException)
                        {
                        }
                        if ((obj2 != null) && (obj2 is byte[]))
                        {
                            text2 = Convert.ToBase64String((byte[])obj2);
                        }
                    }
                    num1 = this.Dispose(text2, false);
                    if ((num1 == 0) && (key1 != null))
                    {
                        text2 = "";
                        string text4 = type.Assembly.GetName().Name + " eval";
                        object obj3 = null;
                        try
                        {
                            obj3 = key1.GetValue(text4);
                        }
                        catch (SecurityException)
                        {
                        }
                        if ((obj3 != null) && (obj3 is byte[]))
                        {
                            text2 = Convert.ToBase64String((byte[])obj3);
                        }
                        num1 = this.Dispose(text2, false);
                    }
                    if ((num1 >= 4) && (context.UsageMode == LicenseUsageMode.Designtime))
                    {
                        context.SetSavedLicenseKey(type, text2);
                    }
                    if ((num1 == 4) || (context.UsageMode == LicenseUsageMode.Designtime))
                    {
                        return new DiagramViewLicense(text2, num1);
                    }
                }
            }
            catch (Exception exception3)
            {
                text1 = text1 + "\n" + exception3.ToString();
            }
            num2 = num1 & 3;
            switch (num2)
            {
                case 1:
                    {
                        string text7 = (num1 > 4) ? "beta" : "evaluation";
                        textArray1 = new string[0x10] { "Built using ", DiagramViewLicenseProvider.GONAME, " for .NET Windows Forms ", this.StringFloat(DiagramView.Version), Environment.NewLine, "Copyright \x00a9 Northwoods Software, 1998-2004.  All Rights Reserved.", Environment.NewLine, "This ", text7, " copy of ", DiagramViewLicenseProvider.GONAME, " is about to expire.", Environment.NewLine, Environment.NewLine, "DO NOT DISTRIBUTE OR DEPLOY THIS SOFTWARE.", Environment.NewLine };
                        string text8 = string.Concat(textArray1);
                        if (num1 < 4)
                        {
                            text8 = text8 + Environment.NewLine + "Please purchase a license at www.nwoods.com" + Environment.NewLine;
                        }
                        if (SystemInformation.UserInteractive)
                        {
                            MessageBox.Show(text8, type.Name + " License Check", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            Console.WriteLine(type.Name + " License Check");
                            Console.WriteLine(text8);
                        }
                        return new DiagramViewLicense(text8, num1);
                    }
                case 2:
                    {
                        string text5 = (num1 > 4) ? "beta" : "evaluation";
                        textArray1 = new string[14] { "Built using ", DiagramViewLicenseProvider.GONAME, " for .NET Windows Forms ", this.StringFloat(DiagramView.Version), Environment.NewLine, "Copyright \x00a9 Northwoods Software, 1998-2004.  All Rights Reserved.", Environment.NewLine, "This software is licensed for a limited ", text5, " period.", Environment.NewLine, Environment.NewLine, "DO NOT DISTRIBUTE OR DEPLOY THIS SOFTWARE.", Environment.NewLine };
                        string text6 = string.Concat(textArray1);
                        if (num1 < 4)
                        {
                            text6 = text6 + Environment.NewLine + "Please purchase a license at www.nwoods.com" + Environment.NewLine;
                        }
                        if (SystemInformation.UserInteractive)
                        {
                            if (!DiagramViewLicenseProvider.err)
                            {
                                DiagramViewLicenseProvider.err = true;
                                MessageBox.Show(text6, type.Name + " License Check", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        else
                        {
                            Console.WriteLine(type.Name + " License Check");
                            Console.WriteLine(text6);
                        }
                        return new DiagramViewLicense(text6, num1);
                    }
            }
            textArray1 = new string[0x1c] { 
                "Built using ", DiagramViewLicenseProvider.GONAME, " for .NET Windows Forms ", this.StringFloat(DiagramView.Version), Environment.NewLine, "Copyright \x00a9 Northwoods Software, 1998-2004.  All Rights Reserved.", Environment.NewLine, "The license for this copy of ", DiagramViewLicenseProvider.GONAME, " is invalid or has expired.", Environment.NewLine, Environment.NewLine, "Please purchase a license at www.nwoods.com", Environment.NewLine, "If you have already purchased a ", DiagramViewLicenseProvider.GONAME, 
                " development license,", Environment.NewLine, "  have you requested an Unlock Code for your development machine by running the GoDiagram LicenseManager?", Environment.NewLine, "If you have already entered an Unlock Code in the LicenseManager,", Environment.NewLine, "  did you link license objects into your application via the Microsoft license compiler?", Environment.NewLine, "  (Make sure the needed components and correct VERSION are listed in the LICENSES.LICX file,", Environment.NewLine, "   and that the LICENSES.LICX file is part of your EXECUTABLE's project, not in a DLL.)", Environment.NewLine
             };
            string text9 = string.Concat(textArray1);
            text9 = text9 + text1;
            if (SystemInformation.UserInteractive)
            {
                MessageBox.Show(text9, type.Name + " License Check", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                Console.WriteLine(type.Name + " License Check");
                Console.WriteLine(text9);
            }
            if (allowExceptions)
            {
                throw new LicenseException(type, instance, text9);
            }
            return null;
        }

        private int ParseInt(string s)
        {
            return int.Parse(s, NumberFormatInfo.InvariantInfo);
        }

        private string StringFloat(float f)
        {
            return f.ToString(NumberFormatInfo.InvariantInfo);
        }


        private static bool err;
        private static readonly string GONAME;

        [Serializable]
        internal sealed class DiagramViewLicense : License, ISerializable
        {
            internal DiagramViewLicense(SerializationInfo info, StreamingContext context)
            {
                this.myRandom = null;
                this.myFont = null;
                this.myBrush = null;
                this.myKey = null;
                this.myRandom = (Random)info.GetValue("myRandom", typeof(Random));
                this.myKey = (string)info.GetValue("myKey", typeof(string));
            }

            internal DiagramViewLicense(string key, int c)
            {
                this.myRandom = null;
                this.myFont = null;
                this.myBrush = null;
                this.myKey = null;
                this.myKey = key;
                this.myRandom = new Random();
                int num1 = 80 + this.myRandom.Next(100);
                num1 = System.Math.Min(0xff, System.Math.Max(0, num1));
                int num2 = 80 + this.myRandom.Next(100);
                num2 = System.Math.Min(0xff, System.Math.Max(0, num2));
                int num3 = 80 + this.myRandom.Next(100);
                num3 = System.Math.Min(0xff, System.Math.Max(0, num3));
                num3 &= -8;
                num3 |= c;
                this.myBrush = new SolidBrush(Color.FromArgb(0xff, num1, num2, num3));
                this.myFont = new Font("Microsoft Sans Serif", (float)(8 + this.myRandom.Next(4)));
            }

            public override void Dispose()
            {
                if (this.myFont != null)
                {
                    this.myFont.Dispose();
                    this.myFont = null;
                }
                if (this.myBrush != null)
                {
                    this.myBrush.Dispose();
                    this.myBrush = null;
                }
            }

            internal void Dispose(DiagramView view)
            {
                Rectangle rectangle1 = view.myPaintEventArgs.ClipRectangle;
                view.myGraphics.DrawImage(view.myBuffer, rectangle1, rectangle1, GraphicsUnit.Pixel);
                if ((this.myBrush.Color.B & 7) < 4)
                {
                    view.myGraphics.DrawString(DiagramViewLicenseProvider.GONAME + ", for evaluation only\r\nNorthwoods Software\r\nwww.nwoods.com", this.myFont, this.myBrush, (float)10f, (float)10f);
                }
            }

            [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("myRandom", this.myRandom);
                info.AddValue("myKey", this.myKey);
            }


            public override string LicenseKey
            {
                get
                {
                    return this.myKey;
                }
            }


            [NonSerialized]
            internal SolidBrush myBrush;
            [NonSerialized]
            internal Font myFont;
            private string myKey;
            internal Random myRandom;
        }
    }
}
