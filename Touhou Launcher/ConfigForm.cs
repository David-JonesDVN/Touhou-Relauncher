﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Touhou_Launcher
{
    public partial class ConfigForm : Form
    {
        public int game;
        Button parentButton;
        Dictionary<string, string> crap = new Dictionary<string, string>();

        public ConfigForm(Button parentBtn)
        {
            InitializeComponent();
            parentButton = parentBtn;
            game = MainForm.gameNames.IndexOf(parentBtn.Name.Substring(3));
            InitializeLanguage();
            chkCustomBanner.Checked = MainForm.curCfg.gameCFG[game].customBanner;
            bannerOffFile.Text = MainForm.curCfg.gameCFG[game].bannerOff;
            bannerOnFile.Text = MainForm.curCfg.gameCFG[game].bannerOn;
            chkCustomText.Checked = MainForm.curCfg.gameCFG[game].customText;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            if (game < MainForm.pc98GameCount)
            {
                foreach (Control ctrl in windowsSettings.Controls)
                {
                    ctrl.Enabled = false;
                }
                hdiFile.Text = MainForm.curCfg.gameCFG[game].GameDir[0];
            }
            else
            {
                foreach (Control ctrl in pc98Settings.Controls)
                    ctrl.Enabled = false;
                jpExe.Text = MainForm.curCfg.gameCFG[game].GameDir[0];
                jpApplocale.Checked = MainForm.curCfg.gameCFG[game].appLocale[0];
                enExe.Text = MainForm.curCfg.gameCFG[game].GameDir[1];
                enApplocale.Checked = MainForm.curCfg.gameCFG[game].appLocale[1];
                customExe.Text = MainForm.curCfg.gameCFG[game].GameDir[2];
                customApplocale.Checked = MainForm.curCfg.gameCFG[game].appLocale[2];
                Refreshcrap();
                crapApplocale.Checked = MainForm.curCfg.gameCFG[game].appLocale[3];
                defaultExec.SelectedIndex = MainForm.curCfg.gameCFG[game].DefaultDir;
                defaultApplocale.Checked = MainForm.curCfg.gameCFG[game].DefaultApplocale;
            }
        }

        public void Refreshcrap()
        {
            crapCfg.Items.Clear();
            crapCfg.Items.Add("None");
            crapGame.Items.Clear();
            crapGame.Items.Add("None");
            if (MainForm.curCfg.crapDir != "")
            {
                if (File.Exists(MainForm.curCfg.crapDir + "\\config\\games.js"))
                {
                    crap = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(MainForm.curCfg.crapDir + "\\config\\games.js"));
                    foreach (KeyValuePair<string, string> line in crap)
                    {
                        string number = new string(line.Key.Where(char.IsDigit).ToArray());
                        if (number == MainForm.FormatGameNumber(MainForm.gameNumbers[game]) || !line.Key.StartsWith("th"))
                        {
                            crapGame.Items.Add(line.Key);
                        }
                    }
                }
                foreach (FileInfo file in new DirectoryInfo(MainForm.curCfg.crapDir).CreateSubdirectory("config").GetFiles("*.js").Where(n => n.Name != "games.js" && n.Name != "config.js"))
                {
                    crapCfg.Items.Add(file.Name);
                }
            }
            if (MainForm.curCfg.crapDir != "")
                crapCfg.Items.Add("Custom");
            crapGame.SelectedIndexChanged -= crapCfg_SelectedIndexChanged;
            crapCfg.SelectedIndexChanged -= crapCfg_SelectedIndexChanged;
            crapGame.SelectedItem = MainForm.curCfg.gameCFG[game].crapCFG[0];
            crapCfg.SelectedItem = MainForm.curCfg.gameCFG[game].crapCFG[1];
            crapGame.SelectedIndexChanged += crapCfg_SelectedIndexChanged;
            crapCfg.SelectedIndexChanged += crapCfg_SelectedIndexChanged;
        }

        private void InitializeLanguage()
        {
            foreach (Button btn in MainForm.GetAll<Button>(this))
            {
                if (btn.Name.Contains("browse"))
                    btn.Text = MainForm.rm.GetString("browse");
                else if (btn.Name.Contains("launch"))
                    btn.Text = MainForm.rm.GetString("launch");
                else
                    btn.Text = MainForm.rm.GetString(btn.Name);
            }
            foreach (CheckBox chk in MainForm.GetAll<CheckBox>(this))
            {
                if (chk.Name.Contains("Applocale"))
                    chk.Text = MainForm.rm.GetString("useApplocale");
                else
                    chk.Text = MainForm.rm.GetString(chk.Name);
            }
            foreach (Label lbl in MainForm.GetAll<Label>(this))
            {
                lbl.Text = MainForm.rm.GetString(lbl.Name);
            }
            foreach (GroupBox box in MainForm.GetAll<GroupBox>(this))
            {
                box.Text = MainForm.rm.GetString(box.Name);
            }
            defaultExec.Items.Clear();
            defaultExec.Items.AddRange(MainForm.rm.GetString("defaultExec").Split(new string[] { ", " }, 4, StringSplitOptions.None));
            this.Text = MainForm.rm.GetString("gameConfiguration") + MainForm.rm.GetString(MainForm.gameNames.ElementAtOrDefault(game));
        }

        private void ConfigForm_Closing(object sender, FormClosingEventArgs e)
        {
            this.Focus();
            if (MainForm.curCfg.gameCFG[game].GameDir[MainForm.curCfg.gameCFG[game].DefaultDir] == "")
                for (int i = 0; i < MainForm.curCfg.gameCFG[game].GameDir.Count; i++)
                {
                    if (MainForm.curCfg.gameCFG[game].GameDir[i] != "")
                    {
                        MainForm.curCfg.gameCFG[game].DefaultDir = i;
                        break;
                    }
                }
            MainForm.RefreshButton(parentButton);
            MainForm.curCfg.Save();
        }

        private void File_LostFocus(object sender, EventArgs e)
        {
            if (File.Exists(((TextBox)sender).Text) || ((TextBox)sender).Text == "")
            {
                ((TextBox)sender).BackColor = SystemColors.Window;
                int defaultExe = game < MainForm.pc98GameCount ? 0 : MainForm.defaultExeOptions.IndexOf(((TextBox)sender).Name.Replace("Exe", ""));
                MainForm.curCfg.gameCFG[game].GameDir[defaultExe] = ((TextBox)sender).Text;
            }
            else
            {
                ((TextBox)sender).BackColor = Color.Red;
            }
        }

        private void Path_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void File_DragDrop(object sender, DragEventArgs e)
        {
            ((TextBox)sender).Text = ((string[])e.Data.GetData(DataFormats.FileDrop)).FirstOrDefault(n => File.Exists(n));
            if (((TextBox)sender).Name.Contains("banner"))
                bannerFile_LostFocus(sender, new EventArgs());
            else
                File_LostFocus(sender, new EventArgs());
        }

        private void Applocale_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.curCfg.gameCFG[game].appLocale[MainForm.defaultExeOptions.IndexOf(((CheckBox)sender).Name.Replace("Applocale", ""))] = ((CheckBox)sender).Checked;
        }

        private void browse_Click(object sender, EventArgs e)
        {
            Control txtBox = windowsSettings.Controls.Find(((Button)sender).Name.Substring(6).ToLower() + "Exe", false).FirstOrDefault(n => n.GetType() == typeof(TextBox));
            int defaultExe = MainForm.defaultExeOptions.IndexOf(txtBox.Name.Replace("Exe", ""));
            string initialDirectory = MainForm.curCfg.gameCFG[game].GameDir[defaultExe] == "" ? null : Path.GetDirectoryName(MainForm.curCfg.gameCFG[game].GameDir[defaultExe]);
            foreach (string path in MainForm.FileBrowser(this, MainForm.rm.GetString("gameSelectTitle"), MainForm.rm.GetString("executableFilter") + " (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*", initialDirectory))
            {
                txtBox.Text = path;
                MainForm.curCfg.gameCFG[game].GameDir[defaultExe] = path;
                string jpPath = Path.GetDirectoryName(path) + "\\th" + MainForm.FormatGameNumber(MainForm.gameNumbers[game]) + ".exe";
                string enPath = Path.GetDirectoryName(path) + "\\th" + MainForm.FormatGameNumber(MainForm.gameNumbers[game]) + "e.exe";
                string customPath = Path.GetDirectoryName(path);
                customPath += MainForm.gameNumbers[game] == 7.5 ? "\\Config.exe" : "\\custom.exe";
                if (defaultExe != 0 && File.Exists(jpPath) && MainForm.curCfg.gameCFG[game].GameDir[0] == "")
                {
                    jpExe.Text = jpPath;
                    MainForm.curCfg.gameCFG[game].GameDir[0] = jpPath;
                }
                if (defaultExe != 1 && File.Exists(enPath) && MainForm.curCfg.gameCFG[game].GameDir[1] == "")
                {
                    enExe.Text = enPath;
                    MainForm.curCfg.gameCFG[game].GameDir[1] = enPath;
                }
                if (defaultExe != 2 && File.Exists(customPath) && MainForm.curCfg.gameCFG[game].GameDir[2] == "")
                {
                    customExe.Text = customPath;
                    MainForm.curCfg.gameCFG[game].GameDir[2] = customPath;
                }
                break;
            }
        }

        private void launch_Click(object sender, EventArgs e)
        {
            int defaultExe = MainForm.defaultExeOptions.IndexOf(((Button)sender).Name.ToLower().Substring(6));
            MainForm.launchGame(game, defaultExe, MainForm.curCfg.gameCFG[game].appLocale[defaultExe]);
        }

        private void crapCfg_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm.curCfg.gameCFG[game].crapCFG[0] = crapGame.SelectedItem.ToString();
            if (crapCfg.SelectedItem.ToString() == "Custom")
            {
                using (thcrap profileConfig = new thcrap(this))
                {
                    profileConfig.ShowDialog(this);
                }
            }
            else
                MainForm.curCfg.gameCFG[game].crapCFG[1] = crapCfg.SelectedItem.ToString();
            if (crap.ContainsKey(MainForm.curCfg.gameCFG[game].crapCFG[0]))
                MainForm.curCfg.gameCFG[game].GameDir[3] = MainForm.curCfg.gameCFG[game].crapCFG[0] != "None" ? crap[MainForm.curCfg.gameCFG[game].crapCFG[0]] : "";
        }

        private void launchcrap_Click(object sender, EventArgs e)
        {
            MainForm.launchcrap(game);
        }

        private void defaultExec_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm.curCfg.gameCFG[game].DefaultDir = defaultExec.SelectedIndex;
        }

        private void defaultApplocale_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.curCfg.gameCFG[game].DefaultApplocale = defaultApplocale.Checked;
        }

        private void browseBannerOn_Click(object sender, EventArgs e)
        {
            string initialDirectory = MainForm.curCfg.gameCFG[game].bannerOn == "" ? null : Path.GetDirectoryName(MainForm.curCfg.gameCFG[game].bannerOn);
            foreach (string file in MainForm.FileBrowser(this, MainForm.rm.GetString("bannerOnSelectTitle"), MainForm.rm.GetString("imageFilter") + " (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*", initialDirectory))
            {
                try
                {
                    Image.FromFile(file);
                    MainForm.curCfg.gameCFG[game].bannerOn = file;
                    bannerOnFile.Text = file;
                }
                catch (OutOfMemoryException ex)
                {
                    MessageBox.Show(MainForm.rm.GetString("errorOpenImage") + ex);
                }
            }
        }

        private void browseBannerOff_Click(object sender, EventArgs e)
        {
            string initialDirectory = MainForm.curCfg.gameCFG[game].bannerOff == "" ? null : Path.GetDirectoryName(MainForm.curCfg.gameCFG[game].bannerOff);
            foreach (string file in MainForm.FileBrowser(this, MainForm.rm.GetString("bannerOffSelectTitle"), MainForm.rm.GetString("imageFilter") + " (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*", initialDirectory))
            {
                try
                {
                    Image.FromFile(file);
                    MainForm.curCfg.gameCFG[game].bannerOff = file;
                    bannerOffFile.Text = file;
                }
                catch (OutOfMemoryException ex)
                {
                    MessageBox.Show(MainForm.rm.GetString("errorOpenImage") + ex);
                }
            }
        }

        private void bannerFile_LostFocus(object sender, EventArgs e)
        {
            bool onTxtBox = ((TextBox)sender).Name.Contains("On");
            if (((TextBox)sender).Text != "")
            {
                try
                {
                    Image.FromFile(((TextBox)sender).Text);
                }
                catch (OutOfMemoryException ex)
                {
                    ((TextBox)sender).BackColor = SystemColors.Window;
                    MessageBox.Show(MainForm.rm.GetString("errorOpenImage") + ex);
                }
                catch (FileNotFoundException)
                {
                    ((TextBox)sender).BackColor = Color.Red;
                }
            }
            else
                ((TextBox)sender).BackColor = SystemColors.Window;
            if (onTxtBox)
                MainForm.curCfg.gameCFG[game].bannerOn = ((TextBox)sender).Text;
            else
                MainForm.curCfg.gameCFG[game].bannerOff = ((TextBox)sender).Text;
        }

        private void chkCustomBanner_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.curCfg.gameCFG[game].customBanner = chkCustomBanner.Checked;
        }

        private void chkCustomText_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.curCfg.gameCFG[game].customText = chkCustomText.Checked;
        }

        private void btnCustomText_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorSet = new ColorDialog())
            {
                colorSet.Color = Color.FromArgb(MainForm.curCfg.gameCFG[game].textColor);
                if (colorSet.ShowDialog(this) == DialogResult.OK)
                    MainForm.curCfg.gameCFG[game].textColor = colorSet.Color.ToArgb();
            }
        }

        private void browseHDI_Click(object sender, EventArgs e)
        {
            string initialDirectory = MainForm.curCfg.gameCFG[game].GameDir[0] == "" ? null : Path.GetDirectoryName(MainForm.curCfg.gameCFG[game].GameDir[0]);
            foreach (string file in MainForm.FileBrowser(this, MainForm.rm.GetString("hdiSelectTitle"), MainForm.rm.GetString("hdiFilter") + " (*.hdi)|*.hdi|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*", initialDirectory))
            {
                hdiFile.Text = file;
                MainForm.curCfg.gameCFG[game].GameDir[0] = file;
            }
        }

        private void launchHDI_Click(object sender, EventArgs e)
        {
            MainForm.launchHDI(MainForm.curCfg.gameCFG[game].GameDir[0]);
        }

        private void openFolder_Click(object sender, EventArgs e)
        {
            string path = "";
            if (game < MainForm.pc98GameCount)
            {
                if (hdiFile.Text != "")
                {
                    if (Directory.Exists(Path.GetDirectoryName(hdiFile.Text)))
                        path = Path.GetDirectoryName(hdiFile.Text);
                }
            }
            else
            {
                foreach (TextBox file in MainForm.GetAll<TextBox>(windowsSettings))
                {
                    if (file.Text != "")
                    {
                        path = Path.GetDirectoryName(file.Text);
                        if (Directory.Exists(path))
                            break;
                    }
                }
            }
            if (path != "")
                Process.Start(path);
        }

        private void openAppdata_Click(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.DoNotVerify) + "\\ShanghaiAlice\\th" + MainForm.FormatGameNumber(MainForm.gameNumbers[game]);
            if (Directory.Exists(path))
                Process.Start(path);
            else if (Directory.Exists(path + "tr"))
                Process.Start(path + "tr");
            else
                MessageBox.Show(MainForm.rm.GetString("errorAppdataNotFound"));
        }

        private void openReplays_Click(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.DoNotVerify) + "\\ShanghaiAlice\\th";
            if (Directory.Exists(path + MainForm.FormatGameNumber(MainForm.gameNumbers[game]) + "\\replay"))
            {
                Process.Start(path + MainForm.FormatGameNumber(MainForm.gameNumbers[game]) + "\\replay");
                return;
            }
            else if (Directory.Exists(path + MainForm.FormatGameNumber(MainForm.gameNumbers[game]) + "tr\\replay"))
            {
                Process.Start(path + MainForm.FormatGameNumber(MainForm.gameNumbers[game]) + "tr\\replay");
                return;
            }
            else
            {
                foreach (TextBox file in MainForm.GetAll<TextBox>(windowsSettings))
                {
                    if (file.Text != "")
                    {
                        string path2 = Path.GetDirectoryName(file.Text) + "\\replay";
                        if (Directory.Exists(path2))
                        {
                            Process.Start(path2);
                            return;
                        }
                    }
                }
            }
            MessageBox.Show(MainForm.rm.GetString("errorReplaysNotFound"));
        }

        private void openvpatch_Click(object sender, EventArgs e)
        {
            foreach (TextBox txt in windowsSettings.Controls.OfType<TextBox>())
            {
                if (txt.Text != "")
                {
                    string path = Path.GetDirectoryName(txt.Text) + "\\vpatch.ini";
                    if (File.Exists(path))
                    {
                        Process.Start(path);
                        return;
                    }
                }
            }
            MessageBox.Show(MainForm.rm.GetString("errorvpatchNotFound"));
        }

        private void openNP2Folder_Click(object sender, EventArgs e)
        {
            if (MainForm.curCfg.np2Dir != "")
                Process.Start(Path.GetDirectoryName(MainForm.curCfg.np2Dir));
            else
                MessageBox.Show(MainForm.rm.GetString("errorNP2NotFound"));
        }
    }
}
