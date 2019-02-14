using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Touhou_Launcher
{
    public partial class ConfigForm : Form
    {
        int game;
        bool tr = false;
        MainForm main;

        public ConfigForm(int id, MainForm parent)
        {
            InitializeComponent();
            game = id;
            main = parent;
            InitializeLanguage();
            chkCustomBanner.Checked = MainForm.curCfg.gameCFG[game].customBanner;
            bannerOffDir.Text = MainForm.curCfg.gameCFG[game].bannerOff;
            bannerOnDir.Text = MainForm.curCfg.gameCFG[game].bannerOn;
            string trTest = Environment.SpecialFolder.ApplicationData + "\\ShaghaiAlice\\th" + MainForm.idToNumber[game].ToString("00");
            if (!Directory.Exists(trTest) && Directory.Exists(trTest + "tr"))
                tr = true;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            if (game > 4)
            {
                foreach (Control ctrl in pc98Settings.Controls)
                    ctrl.Enabled = false;
                jpDir.Text = MainForm.curCfg.gameCFG[game].GameDir[0];
                jpApplocale.Checked = MainForm.curCfg.gameCFG[game].appLocale[0];
                enDir.Text = MainForm.curCfg.gameCFG[game].GameDir[1];
                enApplocale.Checked = MainForm.curCfg.gameCFG[game].appLocale[1];
                customDir.Text = MainForm.curCfg.gameCFG[game].GameDir[2];
                customApplocale.Checked = MainForm.curCfg.gameCFG[game].appLocale[2];
                crapDir.Text = MainForm.curCfg.gameCFG[game].GameDir[3];
                crapApplocale.Checked = MainForm.curCfg.gameCFG[game].appLocale[3];
                defaultExec.SelectedIndex = MainForm.curCfg.gameCFG[game].DefaultDir;
                defaultApplocale.Checked = MainForm.curCfg.gameCFG[game].DefaultApplocale;
            }
            else
            {
                foreach (Control ctrl in windowsSettings.Controls)
                {
                    ctrl.Enabled = false;
                }
                hdiDir.Text = MainForm.curCfg.gameCFG[game].GameDir[0];
            }
        }

        private void InitializeLanguage()
        {
            foreach (Button btn in main.GetAll(this, typeof(Button)))
            {
                if (btn.Name.Contains("browse"))
                    btn.Text = MainForm.rm.GetString("browse");
                else if (btn.Name.Contains("launch"))
                    btn.Text = MainForm.rm.GetString("launch");
                else
                    btn.Text = MainForm.rm.GetString(btn.Name);
            }
            foreach (CheckBox chk in main.GetAll(this, typeof(CheckBox)))
            {
                if (chk.Name.Contains("Applocale"))
                    chk.Text = MainForm.rm.GetString("useApplocale");
                else
                    chk.Text = MainForm.rm.GetString(chk.Name);
            }
            foreach (Label lbl in main.GetAll(this, typeof(Label)))
            {
                    lbl.Text = MainForm.rm.GetString(lbl.Name);
            }
            foreach (GroupBox box in main.GetAll(this, typeof(GroupBox)))
            {
                box.Text = MainForm.rm.GetString(box.Name);
            }
            defaultExec.Items.Clear();
            defaultExec.Items.AddRange(MainForm.rm.GetString("defaultExec").Split(new string[] {", "}, 4, StringSplitOptions.None));
            this.Text = MainForm.rm.GetString("gameConfiguration") + MainForm.rm.GetString(MainForm.nameToID.FirstOrDefault(t => t.Value == game).Key);
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
            main.RefreshGames();
            MainForm.curCfg.Save();
        }

        private void Dir_LostFocus(object sender, EventArgs e)
        {
            if (File.Exists(((TextBox)sender).Text) || ((TextBox)sender).Text == "")
            {
                ((TextBox)sender).BackColor = SystemColors.Window;
                int dirID = game > 4 ? MainForm.dirToNumber[((TextBox)sender).Name.Replace("Dir", "")] : 0;
                MainForm.curCfg.gameCFG[game].GameDir[dirID] = ((TextBox)sender).Text;
            }
            else
            {
                ((TextBox)sender).BackColor = Color.Red;
            }
        }

        private void Dir_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Dir_DragDrop(object sender, DragEventArgs e)
        {
            ((TextBox)sender).Text = ((string[])e.Data.GetData(DataFormats.FileDrop)).FirstOrDefault(n => File.Exists(n));
            if (((TextBox)sender).Name.Contains("banner"))
                bannerDir_LostFocus(sender, new EventArgs());
            else
                Dir_LostFocus(sender, new EventArgs());
        }

        private void defaultExec_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm.curCfg.gameCFG[game].DefaultDir = defaultExec.SelectedIndex;
        }

        private void Applocale_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.curCfg.gameCFG[game].appLocale[MainForm.dirToNumber[((CheckBox)sender).Name.Replace("Applocale", "")]] = ((CheckBox)sender).Checked;
        }

        private void browse_Click(object sender, EventArgs e)
        {
            Control txtBox = windowsSettings.Controls.Find(((Button)sender).Name.ToLower().Substring(6) + "Dir", false).FirstOrDefault(n => n.GetType() == typeof(TextBox));
            foreach (string path in MainForm.FileBrowser(MainForm.rm.GetString("gameSelectTitle"), MainForm.rm.GetString("executableFilter") + " (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*"))
            {
                int type = MainForm.dirToNumber[txtBox.Name.Replace("Dir", "")];
                txtBox.Text = path;
                MainForm.curCfg.gameCFG[game].GameDir[type] = path;
                string jpPath = Path.GetDirectoryName(path) + "\\th" + (MainForm.idToNumber[game]).ToString("00") + ".exe";
                string enPath = Path.GetDirectoryName(path) + "\\th" + (MainForm.idToNumber[game]).ToString("00") + "e.exe";
                string customPath = Path.GetDirectoryName(path) + "\\" + "custom.exe";
                switch (type)
                {
                    case 0: if (File.Exists(enPath))
                        {
                            enDir.Text = enPath;
                            MainForm.curCfg.gameCFG[game].GameDir[1] = enPath;
                        }
                        if (File.Exists(customPath))
                        {
                            customDir.Text = customPath;
                            MainForm.curCfg.gameCFG[game].GameDir[2] = customPath;
                        }
                        break;
                    case 1: if (File.Exists(jpPath))
                        {
                            jpDir.Text = jpPath;
                            MainForm.curCfg.gameCFG[game].GameDir[0] = jpPath;
                        }
                        if (File.Exists(customPath))
                        {
                            customDir.Text = customPath;
                            MainForm.curCfg.gameCFG[game].GameDir[2] = customPath;
                        }
                        break;
                    case 2: if (File.Exists(jpPath))
                        {
                            jpDir.Text = jpPath;
                            MainForm.curCfg.gameCFG[game].GameDir[0] = jpPath;
                        }
                        if (File.Exists(enPath))
                        {
                            enDir.Text = enPath;
                            MainForm.curCfg.gameCFG[game].GameDir[1] = enPath;
                        }
                        break;
                }
                break;
            }
        }

        private void launch_Click(object sender, EventArgs e)
        {
            string path = windowsSettings.Controls.Find(((Button)sender).Name.ToLower().Substring(6) + "Dir", false).FirstOrDefault(n => n.GetType() == typeof(TextBox)).Text;
            string args = "";
            if (File.Exists(path))
            {
                if (MainForm.curCfg.gameCFG[game].appLocale[MainForm.dirToNumber[((Button)sender).Name.ToLower().Substring(6)]])
                {
                    args = "\"" + path + "\" \"/L0411\"";
                    path = "C:\\Windows\\AppPatch\\AppLoc.exe";
                    Process.Start(path, args);
                }
                else
                {
                    Process.Start(path, args);
                }
            }
            else
            {
                MessageBox.Show(MainForm.rm.GetString("errorGameNotFound"));
            }
        }

        private void defaultApplocale_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.curCfg.gameCFG[game].DefaultApplocale = defaultApplocale.Checked;
        }

        private void browseBannerOn_Click(object sender, EventArgs e)
        {
            foreach (string file in MainForm.FileBrowser(MainForm.rm.GetString("bannerOnSelectTitle"), MainForm.rm.GetString("imageFilter") + " (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*"))
            {
                try
                {
                    Image.FromFile(bannerOnDir.Text);
                    MainForm.curCfg.gameCFG[game].bannerOn = file;
                    bannerOnDir.Text = file;
                }
                catch (OutOfMemoryException ex)
                {
                    MessageBox.Show(MainForm.rm.GetString("errorOpenImage") + ex);
                }
            }
        }

        private void browseBannerOff_Click(object sender, EventArgs e)
        {
            foreach (string file in MainForm.FileBrowser(MainForm.rm.GetString("bannerOffSelectTitle"), MainForm.rm.GetString("imageFilter") + " (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*"))
            {
                try
                {
                    Image.FromFile(file);
                    MainForm.curCfg.gameCFG[game].bannerOff = file;
                    bannerOffDir.Text = file;
                }
                catch (OutOfMemoryException ex)
                {
                    MessageBox.Show(MainForm.rm.GetString("errorOpenImage") + ex);
                }
            }
        }

        private void bannerDir_LostFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text != "")
            {
                try
                {
                    Image.FromFile(((TextBox)sender).Text);
                    bool onTxtBox = ((TextBox)sender).Name.Contains("On");
                    if (onTxtBox)
                        MainForm.curCfg.gameCFG[game].bannerOn = ((TextBox)sender).Text;
                    else
                        MainForm.curCfg.gameCFG[game].bannerOff = ((TextBox)sender).Text;
                }
                catch (OutOfMemoryException ex)
                {

                    MessageBox.Show(MainForm.rm.GetString("errorOpenImage") + ex);
                }
                catch (FileNotFoundException ex)
                {
                    ((TextBox)sender).BackColor = Color.Red;
                }
            }
            else
                ((TextBox)sender).BackColor = SystemColors.Window;
        }

        private void browseHDI_Click(object sender, EventArgs e)
        {
            foreach (string file in MainForm.FileBrowser(MainForm.rm.GetString("hdiSelectTitle"), MainForm.rm.GetString("hdiFilter") + " (*.hdi)|*.hdi|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*"))
            {
                hdiDir.Text = file;
                MainForm.curCfg.gameCFG[game].GameDir[0] = file;
            }
        }

        private void launchHDI_Click(object sender, EventArgs e)
        {
            if (!File.Exists(MainForm.curCfg.np2Dir))
                MessageBox.Show(MainForm.rm.GetString("errorNP2NotFound"));
            else if (!MainForm.NekoProject(hdiDir.Text))
                MessageBox.Show(MainForm.rm.GetString("errorInvalidNP2INI"));
            else
                Process.Start(MainForm.curCfg.np2Dir);
        }

        private void openFolder_Click(object sender, EventArgs e)
        {
            string path = "";
            if (game > 4)
            {
                foreach (TextBox dir in main.GetAll(windowsSettings, typeof(TextBox)))
                {
                    if (dir.Text != "")
                    {
                        path = Path.GetDirectoryName(dir.Text);
                        if (Directory.Exists(path))
                            break;
                    }
                }
            }
            else
            {
                if (hdiDir.Text != "")
                {
                    if (Directory.Exists(Path.GetDirectoryName(hdiDir.Text)))
                        path = Path.GetDirectoryName(hdiDir.Text);
                }
            }
            if (path != "")
                Process.Start(path);
        }

        private void openReplays_Click(object sender, EventArgs e)
        {
            string path = Environment.SpecialFolder.ApplicationData + "\\ShanghaiAlice\\th";
            if (Directory.Exists(path + (MainForm.idToNumber[game]).ToString("00") + "\\replay"))
            {
                Process.Start(path + (MainForm.idToNumber[game]).ToString("00") + "\\replay");
            }
            else if (tr)
            {
                Process.Start(path + MainForm.idToNumber[game].ToString("00") + "tr\\replay");
            }
            else
            {
                foreach (TextBox dir in main.GetAll(windowsSettings, typeof(TextBox)))
                {
                    if (dir.Text != "")
                    {
                        string path2 = Path.GetDirectoryName(dir.Text) + "\\replay";
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

        private void openAppdata_Click(object sender, EventArgs e)
        {
            string path = Environment.SpecialFolder.ApplicationData + "\\Shanghai Alice\\th" + (MainForm.idToNumber[game]).ToString("00");
            if (tr)
                path += "tr";
            if (Directory.Exists(path))
                Process.Start(path);
            else
                MessageBox.Show(MainForm.rm.GetString("errorAppdataNotFound"));
        }
    }
}
