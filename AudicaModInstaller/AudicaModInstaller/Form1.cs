using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace AudicaModInstaller
{
    public partial class Audica : Form
    {
        private List<string> steamGameDirs = new List<string>();

        public Audica()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SearchSteam();
            foreach(var dir in steamGameDirs)
            {
                var subDirs = Directory.GetDirectories(dir);
                foreach(var subDir in subDirs)
                {
                    if(subDir.Contains("Audica"))
                    {
                        label1.Text = subDir;
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string MelonDLAddress = new WebClient().DownloadString("https://raw.githubusercontent.com/MeepsKitten/AUsefulDownloadableInstallerContainingAwesomeness/main/MelonVersion.txt");         
            Uri uri = new Uri(MelonDLAddress);

            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(MelonZipDownloadCallback);
                client.DownloadFileAsync(uri, $"{label1.Text}\\melon.zip");
            }
        }

        private void MelonZipDownloadCallback(object sender, AsyncCompletedEventArgs e)
        {
            ZipArchive melonZip = ZipFile.Open($"{label1.Text}\\melon.zip", ZipArchiveMode.Read);

            foreach(var entry in melonZip.Entries)
            {
                var filename = $"{label1.Text}\\{entry.FullName}";

                string directory = Path.GetDirectoryName(filename);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (entry.Name != "")
                    entry.ExtractToFile(filename, true);
            }           
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                label1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void SearchSteam()
        {
            steamGameDirs.Clear();
            string steam32 = "SOFTWARE\\VALVE\\";
            string steam64 = "SOFTWARE\\Wow6432Node\\Valve\\";
            string steam32path;
            string steam64path;
            string config32path;
            string config64path;
            RegistryKey key32 = Registry.LocalMachine.OpenSubKey(steam32);
            RegistryKey key64 = Registry.LocalMachine.OpenSubKey(steam64);
            if (key64.ToString() == null || key64.ToString() == "")
            {
                foreach (string k32subKey in key32.GetSubKeyNames())
                {
                    using (RegistryKey subKey = key32.OpenSubKey(k32subKey))
                    {
                        steam32path = subKey.GetValue("InstallPath").ToString();
                        config32path = steam32path + "/steamapps/libraryfolders.vdf";
                        string driveRegex = @"[A-Z]:\\";
                        if (File.Exists(config32path))
                        {
                            string[] configLines = File.ReadAllLines(config32path);
                            foreach (var item in configLines)
                            {
                                Console.WriteLine("32:  " + item);
                                Match match = Regex.Match(item, driveRegex);
                                if (item != string.Empty && match.Success)
                                {
                                    string matched = match.ToString();
                                    string item2 = item.Substring(item.IndexOf(matched));
                                    item2 = item2.Replace("\\\\", "\\");
                                    item2 = item2.Replace("\"", "\\steamapps\\common\\");
                                    steamGameDirs.Add(item2);
                                }
                            }
                            steamGameDirs.Add(steam32path + "\\steamapps\\common\\");
                        }
                    }
                }
            }
            foreach (string k64subKey in key64.GetSubKeyNames())
            {
                using (RegistryKey subKey = key64.OpenSubKey(k64subKey))
                {
                    var thing = subKey.GetValue("InstallPath");
                    if (thing == null) return;
                    steam64path = thing.ToString();
                    config64path = steam64path + "/steamapps/libraryfolders.vdf";
                    string driveRegex = @"[A-Z]:\\";
                    if (File.Exists(config64path))
                    {
                        string[] configLines = File.ReadAllLines(config64path);
                        foreach (var item in configLines)
                        {
                            Console.WriteLine("64:  " + item);
                            Match match = Regex.Match(item, driveRegex);
                            if (item != string.Empty && match.Success)
                            {
                                string matched = match.ToString();
                                string item2 = item.Substring(item.IndexOf(matched));
                                item2 = item2.Replace("\\\\", "\\");
                                item2 = item2.Replace("\"", "\\steamapps\\common\\");
                                steamGameDirs.Add(item2);
                            }
                        }
                        steamGameDirs.Add(steam64path + "\\steamapps\\common\\");
                    }
                }
            }
        }
    }
}


