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
using Newtonsoft.Json;

namespace Touhou_Launcher
{
    public partial class MainForm : Form
    {

        public class GameConfig
        {
            public List<string> GameDir = new List<string>(4);
            public List<string> crapCFG = new List<string>(2);
            public List<bool> appLocale = new List<bool>(4);
            public int DefaultDir = 0;
            public bool DefaultApplocale = false;
            public bool customBanner = false;
            public string bannerOn = "";
            public string bannerOff = "";
            public bool customText = false;
            public int textColor = 0;
            public bool showBanner = true;
            public bool showText = true;
            public int defaultTextColor = 0;
            public bool randomCheck = true;
        }

        public class SubNode
        {
            public string Text { get; set; }
            public List<SubNode> Nodes = new List<SubNode>();
            public Dictionary<string, string> Games = new Dictionary<string, string>();
            
            public SubNode(string Text = "The root node")
            {
                this.Text = Text;
            }
        }

        public class AppSettings<T> where T : new()
        {
            private const string DEFAULT_FILENAME = "settings.json";

            public void Save(string fileName = DEFAULT_FILENAME)
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(this, Formatting.Indented));
            }

            public static void Save(T pSettings, string fileName = DEFAULT_FILENAME)
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(pSettings, Formatting.Indented));
            }

            public static T Load(string fileName = DEFAULT_FILENAME)
            {
                T t = new T();
                if (File.Exists(fileName))
                    t = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
                return t;
            }
        }

        public class Configs : AppSettings<Configs>
        {
            public GameConfig[] gameCFG = new GameConfig[27];
            public SubNode Custom = new SubNode();
            public View customView = View.LargeIcon;
            public SortOrder customSort = SortOrder.Ascending;
            public bool autoClose = false;
            public bool showTray = false;
            public bool minimizeToTray = false;
            public int language = 0;
            public string np2Dir = "";
            public string crapDir = "";
            public Configs()
            {
                for (int i = 0; i < gameCFG.Length ; i++)
                {
                    gameCFG[i] = new GameConfig();
                    gameCFG[i].GameDir = new List<string> { "", "", "", "" };
                    gameCFG[i].crapCFG = new List<string> { "None", "None" };
                    gameCFG[i].appLocale = new List<bool> { false, false, false, false };
                }
            }
        }

        public static Configs curCfg = Configs.Load();
        public static System.Resources.ResourceManager rm;
        public static Dictionary<string, int> dirToNumber = new Dictionary<string, int>
        {
            {"jp", 0},
            {"en", 1},
            {"custom", 2},
            {"crap", 3}
        };
        public static List<int> idToNumber = new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 75, 105, 123, 135, 145, 155, 95, 125, 128, 143, 165
        };
        public static Dictionary<string, int> nameToID = new Dictionary<string, int>
        {
            {"HRtP", 0},
            {"SoEW", 1},
            {"PoDD", 2},
            {"LLS", 3},
            {"MS", 4},
            {"EoSD", 5},
            {"PCB", 6},
            {"IN", 7},
            {"PoFV", 8},
            {"MoF", 9},
            {"SA", 10},
            {"UFO", 11},
            {"TD", 12},
            {"DDC", 13},
            {"LoLK", 14},
            {"HSiFS", 15},
            {"IaMP", 16},
            {"SWR", 17},
            {"UoNL", 18},
            {"HM", 19},
            {"ULiL", 20},
            {"AoCF", 21},
            {"StB", 22},
            {"DS", 23},
            {"GFW", 24},
            {"ISC", 25},
            {"VD", 26}
        };

        public static IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        private IEnumerable<ContextMenuStrip> GetContextMenus(Control control)
        {
            return control.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).Select(f => f.GetValue(this)).OfType<ContextMenuStrip>();
        }

        private SubNode TreeToJSON(TreeNodeCollection parent, SubNode serializableTree)
        {
            foreach (TreeNode node in parent)
            {
                SubNode snode = new SubNode(node.Text);
                if (node.Tag != null)
                    snode.Games = (Dictionary<string, string>)node.Tag;
                serializableTree.Nodes.Add(snode);
                TreeToJSON(node.Nodes, snode);
            }
            return serializableTree;
        }

        private void JSONToTree(SubNode nodeList, TreeNodeCollection parent)
        {
            for (int i = 0; i < nodeList.Nodes.Count; i++)
            {
                TreeNode newNode = parent.Add(nodeList.Nodes[i].Text);
                newNode.Tag = nodeList.Nodes[i].Games;
                JSONToTree(nodeList.Nodes[i], parent[i].Nodes);
            }
        }

        private void JSONToTray(SubNode nodeList, ToolStripMenuItem parent)
        {
            for (int i = 0; i < nodeList.Nodes.Count; i++)
            {
                ToolStripMenuItem category = (ToolStripMenuItem)parent.DropDownItems.Add(nodeList.Nodes[i].Text);
                JSONToTray(nodeList.Nodes[i], (ToolStripMenuItem)parent.DropDownItems[i]);
                foreach (KeyValuePair<string, string> game in nodeList.Nodes[i].Games)
                {
                    ToolStripItem gameItem = category.DropDownItems.Add(game.Value, Icon.ExtractAssociatedIcon(game.Key).ToBitmap(), trayCustom_Click);
                    gameItem.Tag = game.Key;
                    gameItem.ImageScaling = ToolStripItemImageScaling.None;
                }
            }
        }

        public static bool NekoProject(string hdi)
        {
            /* Code for dedicating a config file to each game
            if (File.Exists(Path.GetDirectoryName(curCfg.np2Dir + "\\np21nt." + game + ".ini")))
                File.Copy(Path.GetDirectoryName(curCfg.np2Dir) + "\\np21nt" + game + ".ini", Path.GetDirectoryName(curCfg.np2Dir) + "\\np21nt.ini", true);
            else
            {
                File.Copy(Path.GetDirectoryName(curCfg.np2Dir) + "\\np21nt.ini", Path.GetDirectoryName(curCfg.np2Dir) + "\\np21nt" + game + ".ini");
                return NekoProject(hdi, game);
            }
             */
            string[] config = File.ReadAllLines(Path.GetDirectoryName(curCfg.np2Dir) + "\\np21nt.ini", Encoding.Unicode);
            for (int i = 0; i < config.Length; i++)
            {
                if (config[i].Contains("HDD1FILE="))
                {
                    if (config[i] != "HDD1FILE=" + hdi)
                    {
                        try
                        {
                            config[i] = "HDD1FILE=" + hdi;
                            File.WriteAllLines(Path.GetDirectoryName(curCfg.np2Dir) + "\\np21nt.ini", config, Encoding.Unicode);
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static void launchHDI(string dir)
        {
            if (!File.Exists(curCfg.np2Dir))
                MessageBox.Show(rm.GetString("errorNP2NotFound"));
            else if (!NekoProject(dir))
                MessageBox.Show(rm.GetString("errorInvalidNP2INI"));
            else
                Process.Start(curCfg.np2Dir);
        }

        public static void launchGame(int game, int dir, bool applocale)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(curCfg.gameCFG[game].GameDir[dir]);
            if (File.Exists(startInfo.FileName))
            {
                if (applocale && File.Exists("C:\\Windows\\AppPatch\\AppLoc.exe"))
                {
                    startInfo.Arguments = "\"" + startInfo.FileName + "\" \"/L0411\"";
                    startInfo.FileName = "C:\\Windows\\AppPatch\\AppLoc.exe";
                    Process.Start(startInfo);
                }
                else
                {
                    Process.Start(startInfo);
                }
            }
            else
            {
                MessageBox.Show(rm.GetString("errorGameNotFound"));
            }
        }

        public static void launchcrap(int game)
        {
            if (!File.Exists(curCfg.crapDir))
                MessageBox.Show(rm.GetString("errorcrapNotFound"));
            else if (curCfg.gameCFG[game].crapCFG[0] == "None" || curCfg.gameCFG[game].crapCFG[1] == "None")
                MessageBox.Show(rm.GetString("errorcrapConfigNotSet"));
            else
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(curCfg.crapDir, "\"" + Path.GetDirectoryName(curCfg.crapDir) + "\\" + curCfg.gameCFG[game].crapCFG[1] + "\" " + curCfg.gameCFG[game].crapCFG[0]);
                startInfo.WorkingDirectory = Path.GetDirectoryName(curCfg.crapDir);
                Process.Start(startInfo);
            }
        }

        public static string[] FileBrowser(string title, string filter, bool multiSelect = false)
        {
            OpenFileDialog browser = new OpenFileDialog();
            browser.Filter = filter;
            browser.FilterIndex = 1;
            browser.RestoreDirectory = false;
            browser.Multiselect = multiSelect;
            browser.Title = title;
            if (browser.ShowDialog() == DialogResult.OK)
                return browser.FileNames;
            else return new string[0];
        }

        private void downloadReplay(string path, string name, Uri url, bool th10full = false)
        {
            string message = String.Format(rm.GetString("replayDownload"), name, path);
            Console.WriteLine(rm.GetString("replayDownload"));
            if (th10full)
            {
                message = rm.GetString("replayFull") + message;
            }
            DialogResult confirm = MessageBox.Show(message, rm.GetString("replayDownloadTitle"), MessageBoxButtons.YesNoCancel);
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                if (confirm == DialogResult.Yes)
                {
                    wc.DownloadFile(url, path + name);
                }
                else if (confirm == System.Windows.Forms.DialogResult.No)
                {
                    SaveFileDialog browser = new SaveFileDialog();
                    browser.Filter = rm.GetString("replayFilter") + " (*.rpy)|*.rpy|" + rm.GetString("allFilter") + " (*.*)|*.*";
                    browser.InitialDirectory = path;
                    browser.RestoreDirectory = true;
                    browser.FileName = name;
                    if (browser.ShowDialog() == DialogResult.OK)
                        wc.DownloadFile(url, browser.FileName);
                }
            }
        }

        private void LoadSettings()
        {
            InitializeLanguage();
            foreach (Button btn in GetAll(games, typeof(Button)))
                if (btn.Name != "btnRandom")
                    RefreshButton(btn);
            JSONToTree(curCfg.Custom, treeView1.Nodes);
            JSONToTray(curCfg.Custom, trayCustom);
            languageBox.SelectedIndexChanged -= languageBox_SelectedIndexChanged;
            languageBox.SelectedIndex = curCfg.language;
            languageBox.SelectedIndexChanged += languageBox_SelectedIndexChanged;
            autoClose.Checked = curCfg.autoClose;
            minimizeToTray.Checked = curCfg.minimizeToTray;
            showTray.Checked = curCfg.showTray;
            np2Dir.Text = curCfg.np2Dir;
            crapDir.Text = curCfg.crapDir;
            foreach (CheckBox chk in GetAll(randomSettings, typeof(CheckBox)))
            {
                chk.CheckedChanged -= chkRandom_CheckedChanged;
                chk.Checked = curCfg.gameCFG[nameToID[chk.Name.Substring(3)]].randomCheck;
                chk.CheckedChanged += chkRandom_CheckedChanged;
            }
        }

        private void InitializeLanguage()
        {
            switch (curCfg.language)
            {
                case 0: rm = new System.Resources.ResourceManager("Touhou_Launcher.Resources_en", System.Reflection.Assembly.GetExecutingAssembly());
                    break;
                case 1: rm = new System.Resources.ResourceManager("Touhou_Launcher.Resources_jp", System.Reflection.Assembly.GetExecutingAssembly());
                    break;
                case 2: rm = new System.Resources.ResourceManager("Touhou_Launcher.Resources_ru", System.Reflection.Assembly.GetExecutingAssembly());
                    break;
                case 3: if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\Resources_custom.resources"))
                        rm = System.Resources.ResourceManager.CreateFileBasedResourceManager("Resources_custom", Path.GetDirectoryName(Application.ExecutablePath), null);
                    else
                        languageBox.SelectedIndex = 0;
                    break;
            }
            foreach (ToolStripMenuItem tMenu in trayMain.DropDownItems)
            {
                tMenu.Text = rm.GetString(tMenu.Name.Substring(4));
            }
            foreach (ToolStripMenuItem tMenu in trayFighting.DropDownItems)
            {
                tMenu.Text = rm.GetString(tMenu.Name.Substring(4));
            }
            foreach (ToolStripMenuItem tMenu in trayOther.DropDownItems)
            {
                tMenu.Text = rm.GetString(tMenu.Name.Substring(4));
            }
            foreach (Button btn in GetAll(games, typeof(Button)))
            {
                if (btn.Name == "btnRandom")
                    btn.Text = rm.GetString(btn.Name.Substring(3));
                else if (curCfg.gameCFG[nameToID[btn.Name.Substring(3)]].showText)
                    btn.Text = rm.GetString(btn.Name.Substring(3));
                else
                    btn.Text = "";
                toolTip.SetToolTip(btn, rm.GetString(btn.Name.Substring(3) + "Title"));
            }
            foreach (ToolStripMenuItem tMenu in customContextMenu.Items.OfType<ToolStripMenuItem>())
            {
                tMenu.Text = rm.GetString(tMenu.Name);
            }
            foreach (ToolStripMenuItem tMenu in viewToolStripMenuItem.DropDownItems)
            {
                tMenu.Text = rm.GetString(tMenu.Name);
            }
            foreach (CheckBox chk in GetAll(randomSettings, typeof(CheckBox)))
            {
                chk.Text = rm.GetString(chk.Name.Substring(3) + "Short");
                toolTip.SetToolTip(chk, rm.GetString(chk.Name.Substring(3) + "Title"));
            }
            foreach (TabPage tab in mainControl.TabPages)
            {
                tab.Text = rm.GetString(tab.Name);
            }
            trayMain.Text = rm.GetString("mainGroup");
            trayFighting.Text = rm.GetString("fightingGroup");
            trayOther.Text = rm.GetString("otherGroup");
            trayCustom.Text = rm.GetString("customGames");
            trayRandom.Text = rm.GetString("trayRandom");
            trayOpen.Text = rm.GetString("trayOpen");
            trayExit.Text = rm.GetString("trayExit");
            mainGroup.Text = rm.GetString("mainGroup");
            fightingGroup.Text = rm.GetString("fightingGroup");
            otherGroup.Text = rm.GetString("otherGroup");
            configureToolStripMenuItem.Text = rm.GetString("configureToolStripMenuItem");
            buttonToolStripMenuItem.Text = rm.GetString("buttonToolStripMenuItem");
            bannerToolStripMenuItem.Text = rm.GetString("bannerToolStripMenuItem");
            textToolStripMenuItem.Text = rm.GetString("textToolStripMenuItem");
            customAdd.Text = rm.GetString("customAdd");
            newCategoryToolStripMenuItem.Text = rm.GetString("newCategoryToolStripMenuItem");
            renameCategoryToolStripMenuItem.Text = rm.GetString("renameCategoryToolStripMenuItem");
            deleteCategoryToolStripMenuItem.Text = rm.GetString("deleteCategoryToolStripMenuItem");
            ascendingToolStripMenuItem.Text = rm.GetString("ascendingToolStripMenuItem");
            descendingToolStripMenuItem.Text = rm.GetString("descendingToolStripMenuItem");
            launcherSettings.Text = rm.GetString("launcherSettings");
            autoClose.Text = rm.GetString("autoClose");
            toolTip.SetToolTip(autoClose, rm.GetString("autoCloseToolTip"));
            minimizeToTray.Text = rm.GetString("minimizeToTray");
            showTray.Text = rm.GetString("showTray");
            langLabel.Text = rm.GetString("langLabel");
            np2Label.Text = rm.GetString("np2Label");
            crapDirLabel.Text = rm.GetString("crapDirLabel");
            browseNP2.Text = rm.GetString("browse");
            browsecrap.Text = rm.GetString("browse");
            crapConfigure.Text = rm.GetString("crapConfigure");
            randomSettings.Text = rm.GetString("randomSettings");
            randomLabel.Text = rm.GetString("randomLabel");
            randomAll.Text = rm.GetString("randomAll");
            randomNone.Text = rm.GetString("randomNone");
            mainRandom.Text = rm.GetString("mainGroup");
            fightingRandom.Text = rm.GetString("fightingGroup");
            otherRandom.Text = rm.GetString("otherGroup");
            this.Text = rm.GetString("Title");
        }

        public static void RefreshButton(Button btn)
        {
            int game = nameToID[btn.Name.Substring(3)];
            btn.ForeColor = curCfg.gameCFG[game].customText ? Color.FromArgb(curCfg.gameCFG[game].textColor) : Color.FromArgb(curCfg.gameCFG[game].defaultTextColor);
            if (curCfg.gameCFG[game].showBanner)
            {
                bool exists = curCfg.gameCFG[game].crapCFG[0] != "None" && curCfg.gameCFG[game].crapCFG[1] != "None";
                foreach (string dir in curCfg.gameCFG[game].GameDir)
                    if (dir != "" && dir != curCfg.gameCFG[game].GameDir[3])
                    {
                        exists = true;
                        break;
                    }
                if (exists)
                {
                    if (curCfg.gameCFG[game].customBanner && curCfg.gameCFG[game].bannerOn != "")
                        btn.BackgroundImage = Image.FromFile(curCfg.gameCFG[game].bannerOn);
                    else
                        btn.BackgroundImage = (System.Drawing.Bitmap)Touhou_Launcher.Properties.Resources.ResourceManager.GetObject((btn.Name == "btnIN" ? "_" : "") + btn.Name.Substring(3).ToLower());
                }
                else
                {
                    if (curCfg.gameCFG[game].customBanner && curCfg.gameCFG[game].bannerOff != "")
                        btn.BackgroundImage = Image.FromFile(curCfg.gameCFG[game].bannerOff);
                    else
                        btn.BackgroundImage = (System.Drawing.Bitmap)Touhou_Launcher.Properties.Resources.ResourceManager.GetObject((btn.Name == "btnIN" ? "_" : "") + btn.Name.Substring(3).ToLower() + "g");

                }
            }
            else
                btn.BackgroundImage = null;

        }

        private void customAddItem(string[] files)
        {
            foreach (string file in files)
            {
                if (!((Dictionary<string, string>)treeView1.SelectedNode.Tag).ContainsKey(file))
                    ((Dictionary<string, string>)treeView1.SelectedNode.Tag).Add(file, Path.GetFileNameWithoutExtension(file));
            }
            curCfg.Custom = TreeToJSON(treeView1.Nodes, new SubNode());
            curCfg.Save();
            RefreshList(ref listView1, (Dictionary<string, string>)treeView1.SelectedNode.Tag);
        }

        private void RefreshList(ref ListView list, Dictionary<string, string> files)
        {
            list.Clear();
            customImages.Images.Clear();
            foreach (KeyValuePair<string, string> file in files)
            {
                customImages.Images.Add(file.Value, Icon.ExtractAssociatedIcon(file.Key));
                list.Items.Add(file.Key, file.Value, file.Value);
            }
        }

        public MainForm()
        {
            InitializeComponent();
            foreach (Button btn in GetAll(games, typeof(Button)))
                if (btn.Name != "btnRandom")
                    curCfg.gameCFG[nameToID[btn.Name.Substring(3)]].defaultTextColor = btn.ForeColor.ToArgb();
            LoadSettings();
        }

        private void MainForm_Show(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            trayIcon.Visible = curCfg.showTray;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && curCfg.minimizeToTray)
            {
                trayIcon.Visible = true;
                this.Hide();
            }
        }

        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            this.Focus();
            trayIcon.Visible = false;
            curCfg.Save();
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            int game = nameToID[((Button)sender).Name.Substring(3)];
            ProcessStartInfo startInfo = new ProcessStartInfo();
            GameConfig curGame = curCfg.gameCFG[game];
            if (File.Exists(curGame.GameDir[curGame.DefaultDir]) || curGame.DefaultDir == 3)
            {
                if (game > 4)
                {
                    if (curGame.DefaultDir == 3)
                        launchcrap(game);
                    else
                        launchGame(game, curCfg.gameCFG[game].DefaultDir, curCfg.gameCFG[game].DefaultApplocale);
                }
                else
                {
                    if (!File.Exists(curCfg.np2Dir))
                    {
                        MessageBox.Show(rm.GetString("errorNP2NotFound"));
                        return;
                    }
                    else if (!NekoProject(curGame.GameDir[0]))
                    {
                        MessageBox.Show(rm.GetString("errorInvalidNP2INI"));
                        return;
                    }
                    else
                        startInfo.FileName = curCfg.np2Dir;
                }
                Process.Start(startInfo);
                if (curCfg.autoClose)
                    Application.Exit();
            }
            else
                MessageBox.Show(rm.GetString("errorGameNotFound"));
        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            List<string> gameList = new List<string>();
            foreach (CheckBox box in GetAll(randomSettings, typeof(CheckBox)))
            {
                if (box.Checked)
                    gameList.Add(box.Name.Substring(3));
            }
            if (gameList.Count > 0)
                btn_Click(games.Controls.Find("btn" + gameList[new Random().Next(gameList.Count - 1)], true)[0], new EventArgs());
            else
                MessageBox.Show(rm.GetString("errorRandomListEmpty"));
        }

        private void tray_Click(object sender, EventArgs e)
        {
            btn_Click(games.Controls.Find("btn" + ((ToolStripMenuItem)sender).Name.Substring(4), true)[0], new EventArgs());
        }

        private void trayCustom_Click(object sender, EventArgs e)
        {
            Process.Start((string)((ToolStripItem)sender).Tag);
        }

        private void trayMenu_Opening(object sender, CancelEventArgs e)
        {
            trayCustom.DropDownItems.Clear();
            JSONToTray(curCfg.Custom, trayCustom);
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button btn = (Button)((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).Tag;
            ConfigForm gameConfig = new ConfigForm(btn);
            gameConfig.ShowDialog();
        }

        private void customAdd_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                customAddItem(FileBrowser(rm.GetString("gameSelectTitle"), rm.GetString("executableFilter") + " (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|" + rm.GetString("allFilter") + " (*.*)|*.*", true));
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (new Rectangle(e.Node.Bounds.X - 15, e.Node.Bounds.Y, e.Node.Bounds.Width + 15, e.Node.Bounds.Height).Contains(e.Location))
            {
                treeView1.SelectedNode = e.Node;
                RefreshList(ref listView1, (Dictionary<string, string>)e.Node.Tag);
            }
            else
            {
                treeView1.SelectedNode = null;
                RefreshList(ref listView1, new Dictionary<string, string>());
            }
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                e.Node.Text = e.Label;
                curCfg.Custom = TreeToJSON(treeView1.Nodes, new SubNode());
                curCfg.Save();
            }
        }

        private void newCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                TreeNode newNode = treeView1.SelectedNode.Nodes.Add("New Category");
                newNode.Tag = new Dictionary<string, string>();
                treeView1.SelectedNode.Expand();
            }
            else
            {
                TreeNode newNode = treeView1.Nodes.Add("New Category");
                newNode.Tag = new Dictionary<string, string>();
            }
            curCfg.Custom = TreeToJSON(treeView1.Nodes, new SubNode());
            curCfg.Save();
        }

        private void renameCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
                treeView1.SelectedNode.BeginEdit();
        }

        private void deleteCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (MessageBox.Show(String.Format(rm.GetString("customCategoryDeleteConfirm"), treeView1.SelectedNode.Text), "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    treeView1.SelectedNode.Remove();
                    curCfg.Custom = TreeToJSON(treeView1.Nodes, new SubNode());
                    curCfg.Save();
                    RefreshList(ref listView1, treeView1.SelectedNode != null ? (Dictionary<string, string>)treeView1.SelectedNode.Tag : new Dictionary<string, string>());
                }
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            string text = e.IsSelected ? e.Item.Name : null;
            customLabel.Text = text;
            toolTip.SetToolTip(customLabel, text);
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                customAddItem((string[])e.Data.GetData(DataFormats.FileDrop));
            }
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                ((Dictionary<string, string>)treeView1.SelectedNode.Tag)[listView1.Items[e.Item].Name] = e.Label;
                curCfg.Custom = TreeToJSON(treeView1.Nodes, new SubNode());
                curCfg.Save();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Process.Start(listView1.SelectedItems[0].Name);
            }
        }

        private void customContextMenu_Opening(object sender, CancelEventArgs e)
        {
            foreach (ToolStripMenuItem menu in viewToolStripMenuItem.DropDownItems)
            {
                menu.Checked = false;
            }
            switch (listView1.View)
            {
                case View.LargeIcon: largeIconsToolStripMenuItem.Checked = true;
                    break;
                case View.SmallIcon: smallIconsToolStripMenuItem.Checked = true;
                    break;
                case View.List: listToolStripMenuItem.Checked = true;
                    break;
                case View.Details: detailsToolStripMenuItem.Checked = true;
                    break;
                case View.Tile: tileToolStripMenuItem.Checked = true;
                    break;
            }
            foreach (ToolStripMenuItem menu in sortToolStripMenuItem.DropDownItems)
            {
                menu.Checked = false;
            }
            switch (listView1.Sorting)
            {
                case SortOrder.Ascending: ascendingToolStripMenuItem.Checked = true;
                    break;
                case SortOrder.Descending: descendingToolStripMenuItem.Checked = true;
                    break;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem game in listView1.SelectedItems)
            {
                if (MessageBox.Show(String.Format(rm.GetString("customDeleteConfirm"), game.Text), "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    listView1.Items.Remove(game);
                    ((Dictionary<string, string>)treeView1.SelectedNode.Tag).Remove(game.Name);
                    curCfg.Custom = TreeToJSON(treeView1.Nodes, new SubNode());
                    curCfg.Save();
                    RefreshList(ref listView1, (Dictionary<string, string>)treeView1.SelectedNode.Tag);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.SelectedItems[0].BeginEdit();
        }

        private void playRandomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count > 0)
                Process.Start(listView1.Items[new Random().Next(listView1.Items.Count) - 1].Name);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.GetDirectoryName(listView1.SelectedItems[0].Name));
        }

        private void openWithApplocaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Process.Start("C:\\Windows\\AppPatch\\AppLoc.exe", listView1.SelectedItems[0].Name + " /L");
            }
        }

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
            curCfg.customView = View.LargeIcon;
        }

        private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.SmallIcon;
            curCfg.customView = View.SmallIcon;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.List;
            curCfg.customView = View.List;
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            curCfg.customView = View.Details;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Tile;
            curCfg.customView = View.Tile;
        }

        private void ascendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Sorting = SortOrder.Ascending;
            curCfg.customSort = SortOrder.Ascending;
        }

        private void descendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Sorting = SortOrder.Descending;
            curCfg.customSort = SortOrder.Descending;
        }

        private void Replays_CheckedChanged(object sender, EventArgs e)
        {
            replayBrowser.Navigate(((Control)sender).Text);
        }

        private void linkReplays_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                gensokyoReplays.Checked = false;
                royalflareReplays.Checked = false;
                appspotReplays.Checked = false;
                Replays_CheckedChanged(sender, new EventArgs());
            }
        }
        
        private void replayBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().EndsWith(".rpy"))
            {
                e.Cancel = true;
                string name = e.Url.ToString().Substring(e.Url.ToString().LastIndexOf("/") + 1);
                int game = Convert.ToInt32(name.Substring(2, name.LastIndexOf("_") - 2));
                Console.WriteLine(name);
                if (Directory.Exists(Environment.SpecialFolder.ApplicationData + "\\ShanghaiAlice\\th" + game))
                {
                    downloadReplay(Environment.SpecialFolder.ApplicationData + "\\ShanghaiAlice\\th" + game, name, e.Url);
                }
                else
                {
                    foreach (string dir in curCfg.gameCFG[idToNumber.IndexOf(game)].GameDir)
                    {
                        if (dir == "")
                            continue;
                        if (game == 9)
                        {
                            for (int i = 1; i < 26; i++)
                            {
                                if (!File.Exists(Path.GetDirectoryName(dir) + "\\replay\\th10_" + i.ToString("00") + ".rpy"))
                                {
                                    name = "th10_" + i.ToString("00") + ".rpy";
                                    downloadReplay(Path.GetDirectoryName(dir) + "\\replay\\", "th10_" + i.ToString("00") + ".rpy", e.Url);
                                    return;
                                }
                            }
                            downloadReplay(Path.GetDirectoryName(dir) + "\\replay\\", name, e.Url, true);
                        }
                        else
                        {
                            downloadReplay(Path.GetDirectoryName(dir) + "\\replay\\", name, e.Url);
                            return;
                        }
                    }
                    MessageBox.Show(rm.GetString("errorGameNotFound"));
                }
            }
        }

        private void mainControl_Deselected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 3)
                curCfg.Save();
        }

        private void autoClose_CheckedChanged(object sender, EventArgs e)
        {
            curCfg.autoClose = autoClose.Checked;
        }

        private void minimizeToTray_CheckedChanged(object sender, EventArgs e)
        {
            curCfg.minimizeToTray = minimizeToTray.Checked;
        }

        private void showTray_CheckedChanged(object sender, EventArgs e)
        {
            curCfg.showTray = showTray.Checked;
            trayIcon.Visible = showTray.Checked;
        }

        private void languageBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            curCfg.language = languageBox.SelectedIndex;
            curCfg.Save();
            InitializeLanguage();
        }

        private void randomAll_Click(object sender, EventArgs e)
        {
            foreach (CheckBox chk in GetAll(randomSettings, typeof(CheckBox)))
            {
                chk.Checked = true;
            }
        }

        private void randomNone_Click(object sender, EventArgs e)
        {
            foreach (CheckBox chk in GetAll(randomSettings, typeof(CheckBox)))
            {
                chk.Checked = false;
            }
        }

        private void chkRandom_CheckedChanged(object sender, EventArgs e)
        {
            curCfg.gameCFG[nameToID[((CheckBox)sender).Name.Substring(3)]].randomCheck = ((CheckBox)sender).Checked;
        }

        private void browse_Click(object sender, EventArgs e)
        {
            TextBox txtbox = (TextBox)launcherSettings.Controls.Find(((Button)sender).Name.Substring(6).ToLower() + "Dir", false).FirstOrDefault(n => n.GetType() == typeof(TextBox));
            foreach (string file in MainForm.FileBrowser(MainForm.rm.GetString(((Button)sender).Name.Substring(6).ToLower() + "SelectTitle"), MainForm.rm.GetString("executableFilter") + " (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*"))
            {
                txtbox.BackColor = SystemColors.Window;
                txtbox.Text = file;
                curCfg.np2Dir = np2Dir.Text;
                curCfg.crapDir = crapDir.Text;
            }
        }

        private void Dir_LostFocus(object sender, EventArgs e)
        {
            if (File.Exists(((TextBox)sender).Text) || ((TextBox)sender).Text == "")
            {
                ((TextBox)sender).BackColor = SystemColors.Window;
                curCfg.np2Dir = np2Dir.Text;
                curCfg.crapDir = crapDir.Text;
            }
            else
                ((TextBox)sender).BackColor = Color.Red;
        }

        private void Dir_DragDrop(object sender, DragEventArgs e)
        {
            ((TextBox)sender).Text = ((string[])e.Data.GetData(DataFormats.FileDrop)).FirstOrDefault(n => File.Exists(n));
            Dir_LostFocus(sender, new EventArgs());
        }

        private void crapConfigure_Click(object sender, EventArgs e)
        {
            if (curCfg.crapDir != "")
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(Path.GetDirectoryName(curCfg.crapDir) + "\\thcrap_configure.exe");
                startInfo.WorkingDirectory = Path.GetDirectoryName(curCfg.crapDir);
                if (File.Exists(startInfo.FileName))
                    Process.Start(startInfo);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.widdiful.co.uk/irc/touhou-launcher.htm");
        }

        private void trayExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button btn = (Button)((ContextMenuStrip)((ToolStripMenuItem)sender).OwnerItem.GetCurrentParent()).Tag;
            int game = nameToID[btn.Name.Substring(3)];
            curCfg.gameCFG[game].showText = textToolStripMenuItem.Checked;
            curCfg.gameCFG[game].showBanner = bannerToolStripMenuItem.Checked;
            curCfg.Save();
            RefreshButton(btn);
            btn.Text = curCfg.gameCFG[game].showText ? rm.GetString(btn.Name.Substring(3)) : "";
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            ((ContextMenuStrip)sender).Tag = ((ContextMenuStrip)sender).SourceControl;
            int game = nameToID[((ContextMenuStrip)sender).SourceControl.Name.Substring(3)];
            textToolStripMenuItem.Checked = curCfg.gameCFG[game].showText;
            bannerToolStripMenuItem.Checked = curCfg.gameCFG[game].showBanner;
        }
    }
}
