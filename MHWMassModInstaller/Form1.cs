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

namespace MHWMassModInstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "MHW Mass Mod Installer";
            textBoxSourceDir.Text = AppDomain.CurrentDomain.BaseDirectory + "pl\\f_equip\\";

            if (File.Exists("MHWIds.txt"))
                buttonLoad_Click(null, null);

            CheckSourceDir();

        }

        private void CheckSourceDir()
        {

            if (AppDomain.CurrentDomain.BaseDirectory.Substring(AppDomain.CurrentDomain.BaseDirectory.Length - 9, 8).ToLower() == "nativepc")
                AddLog("Looks like I'm in the right folder!");
            else
                AddLog("Looks like this program is not in the correct folder! Put me inside the nativePC folder of your MHW installation.");

            if (!Directory.Exists(textBoxSourceDir.Text)) 
            {
                AddLog("The source directory does not exist!");
                button1.Enabled = false;
            }
            else
            {
                string path = Path.Combine(textBoxSourceDir.Text, "pl" + textBoxBaseId.Text);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (Directory.Exists(path))
                {
                    button1.Enabled = true;
                    AddLog($"BaseID folder exists: {path}");
                    AddLog("Place your mods in this folder and click Begin to install the same mods to all the IDs loaded in the list below.");
                } else
                {
                    AddLog("Unable to create BaseID folder!");
                }
            }
        }

        private static void DirectoryCopy(string sourceDirName, string baseId, string replaceId)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            string destDirName = sourceDirName.Replace(baseId, replaceId);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name.Replace(baseId, replaceId));
                file.CopyTo(temppath, true);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                //string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, baseId, replaceId);
            }

        }

        private void textBoxSourceDir_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddLog("Processing IDs..");
            button1.Enabled = false;
            var ids = textBoxIdList.Text.Split('\n').Distinct().ToArray();
            AddLog("Total IDs to copy: " + ids.Count());
            System.Threading.Thread.Sleep(100);

            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Maximum = ids.Count() - 1;
            progressBar.Value = 0;
            progressBar.Minimum = 0;

            int skip = 0;
            int total = 0;

            foreach (var item in ids)
            {
                if (item.Length > 0 && !item.Contains(textBoxBaseId.Text))
                {
                    total++;
                    AddLog("Processing ID: " + item);
                    DirectoryCopy(Path.Combine(textBoxSourceDir.Text, "pl" + textBoxBaseId.Text), textBoxBaseId.Text, item.Trim());
                } else
                {
                    if (item.Length >= 0)
                    {
                        skip++;
                        AddLog("Skipping ID: " + item);
                    }
                }

                //System.Threading.Thread.Sleep(10);
                progressBar.PerformStep();
                Application.DoEvents();
            }

            AddLog($"Complete. Skipped: {skip} | Total: {total}");

            button1.Enabled = true;
            progressBar.Style = ProgressBarStyle.Marquee;
        }

        private void AddLog(string text)
        {
            textBoxLog.AppendText(text + Environment.NewLine);
        }
        
        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText("MHWIds.txt", textBoxIdList.Text);
                AddLog("Saved IDs to MHWIds.txt");
            } catch
            {
                AddLog("Problem reading MHWIds.txt");
            }
        }


        private void buttonLoad_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxIdList.Text = File.ReadAllText("MHWIds.txt");
                AddLog("Loaded IDs from MHWIds.txt");
            }
            catch
            {
                AddLog("Problem reading MHWIds.txt");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CheckSourceDir();
        }
    }
}
