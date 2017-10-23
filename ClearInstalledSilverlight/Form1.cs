using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ClearInstalledSilverlight
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnClear.Click += btnClear_Click;
        }

        void btnClear_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblOS.Text))
            {
                MessageBox.Show("未知操作系统，暂不支持！");
                return;
            }
            OperatingSystem os = Environment.OSVersion;
            string username = Environment.UserName;

            string path = string.Format("C:\\Documents and Settings\\{0}\\Local Settings\\Application Data\\Microsoft\\Silverlight\\OutOfBrowser\\", username);
            if (!Directory.Exists(path))
            {
                MessageBox.Show("未检测到可清理程序！");
                return;
            }

            string[] regs = Directory.GetDirectories(path);
            RegistryKey uninstallKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true);
            foreach (string reg in regs)
            {
                string tmp = Path.GetFileName(reg);
                if (tmp == "index") continue;
                uninstallKey.DeleteSubKey(tmp, false);
            }
            uninstallKey.Close();

            List<string> paths = new List<string>();
            List<string> files = new List<string>();
            paths.Add(path);

            if (lblOS.Text != "Windows XP")
            {
                if (os.Platform == PlatformID.Win32NT && os.Version.Major >= 6)
                {
                    path = string.Format(@"C:\Users\{0}\AppData\LocalLow\Microsoft\Silverlight\OutOfBrowser\", username);
                    if (Directory.Exists(path))
                    {
                        paths.Add(path);
                    }
                }
            }


            try
            {
                foreach (string p in paths)
                {
                    string[] fs = Directory.GetDirectories(p);
                    if (fs.Length > 0)
                    {
                        files.AddRange(fs);
                    }
                }

                if (files.Count > 0)
                {
                    foreach (string file in files)
                    {
                        Directory.Delete(file, true);
                    }
                }

                string deskTop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string programs = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

                List<string> shortCuts = new List<string>();
                string[] shortcutPaths = Directory.GetFiles(deskTop, "监控端*.lnk", SearchOption.TopDirectoryOnly);
                shortCuts.AddRange(shortcutPaths);
                shortcutPaths = Directory.GetFiles(programs, "监控端*.lnk", SearchOption.TopDirectoryOnly);
                shortCuts.AddRange(shortcutPaths);
                shortcutPaths = Directory.GetFiles(deskTop, "接警平台*.lnk", SearchOption.TopDirectoryOnly);
                shortCuts.AddRange(shortcutPaths);
                shortcutPaths = Directory.GetFiles(programs, "接警平台*.lnk", SearchOption.TopDirectoryOnly);
                shortCuts.AddRange(shortcutPaths);
                shortcutPaths = Directory.GetFiles(deskTop, "110接警*.lnk", SearchOption.TopDirectoryOnly);
                shortCuts.AddRange(shortcutPaths);
                shortcutPaths = Directory.GetFiles(programs, "110接警*.lnk", SearchOption.TopDirectoryOnly);
                shortCuts.AddRange(shortcutPaths);

                foreach (string shortCut in shortCuts)
                {
                    File.Delete(shortCut);
                }


                MessageBox.Show("清理成功！");
            }
            catch
            {
                MessageBox.Show("清理失败，请重试！");
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            lblOS.Text = GetOSName();
            base.OnLoad(e);
        }

        public string GetOSName()
        {
            OperatingSystem os = Environment.OSVersion;
            string osname = "";
            switch (os.Platform)
            {
                case PlatformID.Win32Windows:
                    switch (os.Version.Minor)
                    {
                        case 0:
                            osname = "Windows 95";
                            break;
                        case 10:
                            if (os.Version.Revision.ToString() == "2222A")
                                osname = "Windows 98 第二版";
                            else
                                osname = "Windows 98";
                            break;
                        case 90:
                            osname = "Windows Me";
                            break;
                    }
                    break;
                case PlatformID.Win32NT:
                    switch (os.Version.Major)
                    {
                        case 3:
                            osname = "Windows NT 3.51";
                            break;
                        case 4:
                            osname = "Windows NT 4.0";
                            break;
                        case 5:
                            switch (os.Version.Minor)
                            {
                                case 0:
                                    osname = "Windows 200";
                                    break;
                                case 1:
                                    osname = "Windows XP";
                                    break;
                                case 2:
                                    osname = "Windows 2003";
                                    break;
                            }
                            break;
                        case 6:
                            switch (os.Version.Minor)
                            {
                                case 0:
                                    osname = "Windows Vista";
                                    break;
                                case 1:
                                    osname = "Windows 7/2008";
                                    break;
                                case 2:
                                    osname = "Windows 8/10";
                                    break;
                            }
                            break;
                        case 10:
                            switch (os.Version.Minor)
                            {
                                case 0:
                                    osname = "Windows 10";
                                    break;
                            }
                            break;
                    }
                    break;
            }
            return osname;
        }
    }
}
