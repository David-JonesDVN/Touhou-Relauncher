using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

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
            public int category;

            public GameConfig(int i)
            {
                category = i;
            }
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
                if (File.Exists(fileName))
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
                else return new T();
            }
        }

        public class Configs : AppSettings<Configs>
        {
            public GameConfig[] gameCFG = new GameConfig[totalGameCount];
            public SubNode Custom = new SubNode();
            public View customView = View.LargeIcon;
            public SortOrder customSort = SortOrder.Ascending;
            public bool autoClose = false;
            public bool showTray = false;
            public bool minimizeToTray = false;
            public int language = 0;
            public string np2Dir = "";
            public string crapDir = "";
            public string StartingRepo = @"https://srv.thpatch.net/";
            public Configs()
            {
                for (int i = 0; i < gameCFG.Length; i++)
                {
                    int category;
                    if (i < mainGameCount)
                        category = 0;
                    else if (i < mainGameCount + fightingGameCount)
                        category = 1;
                    else
                        category = 2;

                    gameCFG[i] = new GameConfig(category);
                    gameCFG[i].GameDir = new List<string> { "", "", "", "" };
                    gameCFG[i].crapCFG = new List<string> { "None", "None" };
                    gameCFG[i].appLocale = new List<bool> { false, false, false, false };
                }
            }
        }

        private FormWindowState lastState = FormWindowState.Normal;
        private const int mainGameCount = 19;
        private const int fightingGameCount = 6;
        private const int otherGameCount = 7;
        private const int totalGameCount = mainGameCount + fightingGameCount + otherGameCount;
        public static Configs curCfg = Configs.Load();
        public static System.Resources.ResourceManager rm;
        public static HttpClient client = new HttpClient();
        public static Dictionary<string, int> dirToNumber = new Dictionary<string, int>
        {
            {"jp", 0},
            {"en", 1},
            {"custom", 2},
            {"crap", 3}
        };
        public static List<double> idToNumber = new List<double>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 7.5, 10.5, 12.3, 13.5, 14.5, 15.5, 9.5, 12.5, 12.8, 14.3, 16.5, 17.5, 18.5
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
            {"WBaWC", 16},
            {"UM", 17},
            {"UDoALG", 18},
            {"IaMP", 19},
            {"SWR", 20},
            {"UoNL", 21},
            {"HM", 22},
            {"ULiL", 23},
            {"AoCF", 24},
            {"StB", 25},
            {"DS", 26},
            {"GFW", 27},
            {"ISC", 28},
            {"VD", 29},
            {"GI", 30},
            {"HBM", 31}
        };

        public static string FormatGameNumber(double gameNumber)
        {
            string formatted = gameNumber.ToString("00.0");
            if (formatted.EndsWith(".0"))
                formatted = formatted.Replace(".0", "");
            else
                formatted = formatted.Replace(".", "");

            return formatted;
        }

        public static double UnformatGameNumber(string formatted)
        {
            // Replay sites don't prefix the game numbers with 0s, so these two games have to be checked manually.
            // As long as ZUN doesn't make 75 mainline Touhou games, this should work properly.
            if (formatted == "75")
                return 7.5;
            else if (formatted == "95")
                return 9.5;

            double number = Convert.ToDouble(formatted);
            if (formatted.Length == 3)
                number /= 10;

            return number;
        }

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

        public static Process startProcess(string dir, string args = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(dir, args);
            startInfo.WorkingDirectory = Path.GetDirectoryName(startInfo.FileName);
            return Process.Start(startInfo);
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
                        catch (Exception)
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
                startProcess(curCfg.np2Dir);
        }

        public static void launchGame(int game, int dir, bool applocale)
        {
            string gameDir = curCfg.gameCFG[game].GameDir[dir];
            if (File.Exists(gameDir))
            {
                if (applocale && File.Exists("C:\\Windows\\AppPatch\\AppLoc.exe"))
                {
                    startProcess("C:\\Windows\\AppPatch\\AppLoc.exe", "\"" + gameDir + "\" \"/L0411\"");
                }
                else
                {
                    startProcess(gameDir);
                }
            }
            else
            {
                MessageBox.Show(rm.GetString("errorGameNotFound"));
            }
        }

        public static void launchcrap(int game)
        {
            if (!File.Exists(curCfg.crapDir + "\\bin\\thcrap_loader.exe"))
                MessageBox.Show(rm.GetString("errorcrapNotFound"));
            else if (curCfg.gameCFG[game].crapCFG[0] == "None" || curCfg.gameCFG[game].crapCFG[1] == "None")
                MessageBox.Show(rm.GetString("errorcrapConfigNotSet"));
            else
            {
                startProcess(curCfg.crapDir + "\\bin\\thcrap_loader.exe", "\"" + curCfg.crapDir + "\\config\\" + curCfg.gameCFG[game].crapCFG[1] + "\" " + curCfg.gameCFG[game].crapCFG[0]);
            }
        }

        public static string[] FileBrowser(IWin32Window owner, string title, string filter, string initialDirectory = "", bool multiSelect = false)
        {
            using (OpenFileDialog browser = new OpenFileDialog())
            {
                browser.Filter = filter;
                browser.FilterIndex = 1;
                browser.InitialDirectory = initialDirectory;
                browser.Multiselect = multiSelect;
                browser.Title = title;
                if (browser.ShowDialog(owner) == DialogResult.OK)
                    return browser.FileNames;
                else return Array.Empty<string>();
            }
        }

        public static string FolderBrowser(IWin32Window owner, string title, string rootFolder = "")
        {
            using (FolderBrowserDialog browser = new FolderBrowserDialog())
            {
                browser.SelectedPath = rootFolder; // Sets the initial folder
                browser.Description = title;
                if (browser.ShowDialog(owner) == DialogResult.OK)
                    return browser.SelectedPath;
                else return null;
            }
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
            if (confirm == DialogResult.Yes)
            {
                DownloadFile(url, path + name);
            }
            else if (confirm == DialogResult.No)
            {
                SaveFileDialog browser = new SaveFileDialog();
                browser.Filter = rm.GetString("replayFilter") + " (*.rpy)|*.rpy|" + rm.GetString("allFilter") + " (*.*)|*.*";
                browser.InitialDirectory = path;
                browser.RestoreDirectory = true;
                browser.FileName = name;
                if (browser.ShowDialog(this) == DialogResult.OK)
                    DownloadFile(url, browser.FileName);
            }
        }

        private async void DownloadFile(Uri uri, string path)
        {
            byte[] bytes = await client.GetByteArrayAsync(uri);
            File.WriteAllBytes(path, bytes);
        }

        private void LoadSettings()
        {
            InitializeLanguage();
            foreach (Button btn in GetAll(games, typeof(Button)))
                if (btn.Name != "btnRandom")
                    RefreshButton(btn);
            JSONToTree(curCfg.Custom, customTree.Nodes);
            JSONToTray(curCfg.Custom, trayCustom);
            languageBox.SelectedIndexChanged -= languageBox_SelectedIndexChanged;
            languageBox.SelectedIndex = curCfg.language;
            languageBox.SelectedIndexChanged += languageBox_SelectedIndexChanged;
            autoClose.Checked = curCfg.autoClose;
            minimizeToTray.Checked = curCfg.minimizeToTray;
            showTray.Checked = curCfg.showTray;
            np2Dir.Text = curCfg.np2Dir;
            crapDir.Text = curCfg.crapDir;
            crapStartingRepo.Text = curCfg.StartingRepo;
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
                case 0:
                    rm = new System.Resources.ResourceManager("Touhou_Launcher.Resources_en", System.Reflection.Assembly.GetExecutingAssembly());
                    break;
                case 1:
                    rm = new System.Resources.ResourceManager("Touhou_Launcher.Resources_jp", System.Reflection.Assembly.GetExecutingAssembly());
                    break;
                case 2:
                    rm = new System.Resources.ResourceManager("Touhou_Launcher.Resources_ru", System.Reflection.Assembly.GetExecutingAssembly());
                    break;
                case 3:
                    if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\Resources_custom.resources"))
                        rm = System.Resources.ResourceManager.CreateFileBasedResourceManager("Resources_custom", Path.GetDirectoryName(Application.ExecutablePath), null);
                    else
                        languageBox.SelectedIndex = 0;
                    break;
            }
            foreach (ToolStripMenuItem tMenu in trayPC98.DropDownItems)
            {
                tMenu.Text = rm.GetString(tMenu.Name.Substring(4));
            }
            foreach (ToolStripMenuItem tMenu in trayMain.DropDownItems)
            {
                tMenu.Text = rm.GetString(tMenu.Name.Substring(4));
            }
            foreach (ToolStripMenuItem tMenu in traySpinoff.DropDownItems)
            {
                tMenu.Text = rm.GetString(tMenu.Name.Substring(4));
            }
            foreach (ToolStripMenuItem tMenu in trayTasofro.DropDownItems)
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
            trayPC98.Text = rm.GetString("pc98Group");
            trayMain.Text = rm.GetString("mainGroup");
            traySpinoff.Text = rm.GetString("spinoffGroup");
            trayTasofro.Text = rm.GetString("tasofroGroup");
            trayCustom.Text = rm.GetString("customGames");
            trayRandom.Text = rm.GetString("trayRandom");
            trayOpen.Text = rm.GetString("trayOpen");
            trayExit.Text = rm.GetString("trayExit");
            pc98Group.Text = rm.GetString("pc98Group");
            mainGroup.Text = rm.GetString("mainGroup");
            spinoffGroup.Text = rm.GetString("spinoffGroup");
            tasofroGroup.Text = rm.GetString("tasofroGroup");
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
            crapStartingRepoLabel.Text = rm.GetString("crapStartingRepoLabel");
            crapResetStartingRepo.Text = rm.GetString("crapResetStartingRepo");
            crapConfigure.Text = rm.GetString("crapConfigure");
            randomSettings.Text = rm.GetString("randomSettings");
            randomLabel.Text = rm.GetString("randomLabel");
            randomAll.Text = rm.GetString("randomAll");
            randomNone.Text = rm.GetString("randomNone");
            pc98Random.Text = rm.GetString("pc98Group");
            mainRandom.Text = rm.GetString("mainGroup");
            spinoffRandom.Text = rm.GetString("spinoffGroup");
            tasofroRandom.Text = rm.GetString("tasofroGroup");
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
                {
                    if (dir != "" && dir != curCfg.gameCFG[game].GameDir[3])
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists)
                {
                    if (curCfg.gameCFG[game].customBanner && curCfg.gameCFG[game].bannerOn != "")
                        btn.BackgroundImage = Image.FromFile(curCfg.gameCFG[game].bannerOn);
                    else
                        btn.BackgroundImage = (Bitmap)Touhou_Launcher.Properties.Resources.ResourceManager.GetObject((btn.Name == "btnIN" ? "_" : "") + btn.Name.Substring(3).ToLower());
                }
                else
                {
                    if (curCfg.gameCFG[game].customBanner && curCfg.gameCFG[game].bannerOff != "")
                        btn.BackgroundImage = Image.FromFile(curCfg.gameCFG[game].bannerOff);
                    else
                        btn.BackgroundImage = (Bitmap)Touhou_Launcher.Properties.Resources.ResourceManager.GetObject((btn.Name == "btnIN" ? "_" : "") + btn.Name.Substring(3).ToLower() + "g");

                }
            }
            else
                btn.BackgroundImage = null;

        }

        private void customAddItem(string[] files)
        {
            foreach (string file in files)
            {
                if (!((Dictionary<string, string>)customTree.SelectedNode.Tag).ContainsKey(file))
                    ((Dictionary<string, string>)customTree.SelectedNode.Tag).Add(file, Path.GetFileNameWithoutExtension(file));
            }
            curCfg.Custom = TreeToJSON(customTree.Nodes, new SubNode());
            curCfg.Save();
            RefreshList(ref customList, (Dictionary<string, string>)customTree.SelectedNode.Tag);
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
            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
            InitializeComponent();
            if (totalGameCount > curCfg.gameCFG.Length)
            {
                GameConfig[] backwardsComp = new GameConfig[totalGameCount];
                int offset = 0;
                for (int i = 0; i < backwardsComp.Length; i++)
                {
                    int category;
                    if (i < mainGameCount)
                        category = 0;
                    else if (i < mainGameCount + fightingGameCount)
                        category = 1;
                    else
                        category = 2;

                    if (i - offset < curCfg.gameCFG.Length)
                    {
                        if (curCfg.gameCFG[i - offset].category == category)
                        {
                            backwardsComp[i] = curCfg.gameCFG[i - offset];
                            continue;
                        }
                        else
                            offset++;
                    }
                    backwardsComp[i] = new GameConfig(category);
                    backwardsComp[i].GameDir = new List<string> { "", "", "", "" };
                    backwardsComp[i].crapCFG = new List<string> { "None", "None" };
                    backwardsComp[i].appLocale = new List<bool> { false, false, false, false };
                }
                curCfg.gameCFG = backwardsComp;
            }
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
            if (this.WindowState == FormWindowState.Minimized)
            {
                foreach (FlowLayoutPanel panel in GetAll(mainControl.SelectedTab, typeof(FlowLayoutPanel)))
                {
                    panel.AutoScroll = false;
                }
                if (curCfg.minimizeToTray)
                {
                    trayIcon.Visible = true;
                    this.Hide();
                }
            }
            else if (lastState == FormWindowState.Minimized)
            {
                foreach (FlowLayoutPanel panel in GetAll(mainControl.SelectedTab, typeof(FlowLayoutPanel)))
                {
                    panel.AutoScroll = true;
                }
            }
            lastState = this.WindowState;
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
                    launchHDI(curGame.GameDir[0]);
                }
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
            startProcess((string)((ToolStripItem)sender).Tag);
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
            gameConfig.ShowDialog(this);
        }

        private void customAdd_Click(object sender, EventArgs e)
        {
            if (customTree.SelectedNode != null)
            {
                customAddItem(FileBrowser(this, rm.GetString("gameSelectTitle"), rm.GetString("executableFilter") + " (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|" + rm.GetString("allFilter") + " (*.*)|*.*"));
            }
            else
            {
                MessageBox.Show(rm.GetString("errorNoCategorySelected"));
            }
        }

        private void customTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (new Rectangle(e.Node.Bounds.X - 15, e.Node.Bounds.Y, e.Node.Bounds.Width + 15, e.Node.Bounds.Height).Contains(e.Location))
            {
                customTree.SelectedNode = e.Node;
                RefreshList(ref customList, (Dictionary<string, string>)e.Node.Tag);
            }
            else
            {
                customTree.SelectedNode = null;
                RefreshList(ref customList, new Dictionary<string, string>());
            }
        }

        private void customTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                e.Node.Text = e.Label;
                curCfg.Custom = TreeToJSON(customTree.Nodes, new SubNode());
                curCfg.Save();
            }
        }

        private void newCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (customTree.SelectedNode != null)
            {
                TreeNode newNode = customTree.SelectedNode.Nodes.Add("New Category");
                newNode.Tag = new Dictionary<string, string>();
                customTree.SelectedNode.Expand();
            }
            else
            {
                TreeNode newNode = customTree.Nodes.Add("New Category");
                newNode.Tag = new Dictionary<string, string>();
            }
            curCfg.Custom = TreeToJSON(customTree.Nodes, new SubNode());
            curCfg.Save();
        }

        private void renameCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (customTree.SelectedNode != null)
                customTree.SelectedNode.BeginEdit();
        }

        private void deleteCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (customTree.SelectedNode != null)
            {
                if (MessageBox.Show(String.Format(rm.GetString("customCategoryDeleteConfirm"), customTree.SelectedNode.Text), "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    customTree.SelectedNode.Remove();
                    curCfg.Custom = TreeToJSON(customTree.Nodes, new SubNode());
                    curCfg.Save();
                    RefreshList(ref customList, customTree.SelectedNode != null ? (Dictionary<string, string>)customTree.SelectedNode.Tag : new Dictionary<string, string>());
                }
            }
        }

        private void customList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            string text = e.IsSelected ? e.Item.Name : null;
            customLabel.Text = text;
            toolTip.SetToolTip(customLabel, text);
        }

        private void customList_DragDrop(object sender, DragEventArgs e)
        {
            if (customTree.SelectedNode != null)
            {
                customAddItem((string[])e.Data.GetData(DataFormats.FileDrop));
            }
        }

        private void customList_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                ((Dictionary<string, string>)customTree.SelectedNode.Tag)[customList.Items[e.Item].Name] = e.Label;
                curCfg.Custom = TreeToJSON(customTree.Nodes, new SubNode());
                curCfg.Save();
            }
        }

        private void customList_DoubleClick(object sender, EventArgs e)
        {
            if (customList.SelectedItems.Count > 0)
            {
                startProcess(customList.SelectedItems[0].Name);
            }
        }

        private void customContextMenu_Opening(object sender, CancelEventArgs e)
        {
            foreach (ToolStripMenuItem menu in viewToolStripMenuItem.DropDownItems)
            {
                menu.Checked = false;
            }
            switch (customList.View)
            {
                case View.LargeIcon:
                    largeIconsToolStripMenuItem.Checked = true;
                    break;
                case View.SmallIcon:
                    smallIconsToolStripMenuItem.Checked = true;
                    break;
                case View.List:
                    listToolStripMenuItem.Checked = true;
                    break;
                case View.Details:
                    detailsToolStripMenuItem.Checked = true;
                    break;
                case View.Tile:
                    tileToolStripMenuItem.Checked = true;
                    break;
            }
            foreach (ToolStripMenuItem menu in sortToolStripMenuItem.DropDownItems)
            {
                menu.Checked = false;
            }
            switch (customList.Sorting)
            {
                case SortOrder.Ascending:
                    ascendingToolStripMenuItem.Checked = true;
                    break;
                case SortOrder.Descending:
                    descendingToolStripMenuItem.Checked = true;
                    break;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem game in customList.SelectedItems)
            {
                if (MessageBox.Show(String.Format(rm.GetString("customDeleteConfirm"), game.Text), "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    customList.Items.Remove(game);
                    ((Dictionary<string, string>)customTree.SelectedNode.Tag).Remove(game.Name);
                    curCfg.Custom = TreeToJSON(customTree.Nodes, new SubNode());
                    curCfg.Save();
                    RefreshList(ref customList, (Dictionary<string, string>)customTree.SelectedNode.Tag);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customList.SelectedItems[0].BeginEdit();
        }

        private void playRandomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (customList.Items.Count > 0)
                startProcess(customList.Items[new Random().Next(customList.Items.Count) - 1].Name);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.GetDirectoryName(customList.SelectedItems[0].Name));
        }

        private void openWithApplocaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (customList.SelectedItems.Count > 0)
            {
                startProcess("C:\\Windows\\AppPatch\\AppLoc.exe", customList.SelectedItems[0].Name + " /L");
            }
        }

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customList.View = View.LargeIcon;
            curCfg.customView = View.LargeIcon;
        }

        private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customList.View = View.SmallIcon;
            curCfg.customView = View.SmallIcon;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customList.View = View.List;
            curCfg.customView = View.List;
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customList.View = View.Details;
            curCfg.customView = View.Details;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customList.View = View.Tile;
            curCfg.customView = View.Tile;
        }

        private void ascendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customList.Sorting = SortOrder.Ascending;
            curCfg.customSort = SortOrder.Ascending;
        }

        private void descendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customList.Sorting = SortOrder.Descending;
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
                maribelReplays.Checked = false;
                lunarcastReplays.Checked = false;
                Replays_CheckedChanged(sender, new EventArgs());
            }
        }

        private void replayBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().EndsWith(".rpy"))
            {
                e.Cancel = true;
                string name = e.Url.ToString().Substring(e.Url.ToString().LastIndexOf("/") + 1);
                string game = name.Substring(2, name.LastIndexOf("_") - 2);
                double gameNum = UnformatGameNumber(game);
                if (Directory.Exists(Environment.SpecialFolder.ApplicationData + "\\ShanghaiAlice\\th" + game))
                {
                    downloadReplay(Environment.SpecialFolder.ApplicationData + "\\ShanghaiAlice\\th" + game, name, e.Url);
                }
                else if (Directory.Exists(Environment.SpecialFolder.ApplicationData + "\\ShanghaiAlice\\th" + game + "tr"))
                {
                    downloadReplay(Environment.SpecialFolder.ApplicationData + "\\ShanghaiAlice\\th" + game + "tr", name, e.Url);
                }
                else
                {
                    foreach (string dir in curCfg.gameCFG[idToNumber.IndexOf(gameNum)].GameDir)
                    {
                        if (dir == "")
                            continue;
                        if (gameNum == 10)
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

        private void crapResetStartingRepo_Click(object sender, EventArgs e)
        {
            curCfg.StartingRepo = @"https://srv.thpatch.net/";
            crapStartingRepo.Text = curCfg.StartingRepo;
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

        private void browseFile_Click(object sender, EventArgs e)
        {
            string controlName = ((Button)sender).Name.Substring(6).ToLower() + "Dir";
            FieldInfo field = curCfg.GetType().GetField(controlName);
            string initialDirectory = field == null ? null : Path.GetDirectoryName((string)(field.GetValue(curCfg)));
            TextBox txtbox = (TextBox)launcherSettings.Controls.Find(controlName, false).FirstOrDefault(n => n.GetType() == typeof(TextBox));
            foreach (string file in MainForm.FileBrowser(this, MainForm.rm.GetString(((Button)sender).Name.Substring(6).ToLower() + "SelectTitle"), MainForm.rm.GetString("executableFilter") + " (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*", initialDirectory))
            {
                txtbox.BackColor = SystemColors.Window;
                txtbox.Text = file;
                field?.SetValue(curCfg, txtbox.Text);
            }
        }

        private void browseFolder_Click(object sender, EventArgs e)
        {
            string controlName = ((Button)sender).Name.Substring(6).ToLower() + "Dir";
            FieldInfo field = curCfg.GetType().GetField(controlName);
            string rootFolder = (string)(field?.GetValue(curCfg));
            TextBox txtbox = (TextBox)launcherSettings.Controls.Find(controlName, false).FirstOrDefault(n => n.GetType() == typeof(TextBox));
            string folder = MainForm.FolderBrowser(this, MainForm.rm.GetString(((Button)sender).Name.Substring(6).ToLower() + "SelectTitle"), rootFolder);
            if (folder != null)
            {
                txtbox.BackColor = SystemColors.Window;
                txtbox.Text = folder;
                field?.SetValue(curCfg, txtbox.Text);
            }
        }

        private void Dir_LostFocus(object sender, EventArgs e)
        {
            if (sender == np2Dir)
                if (File.Exists(np2Dir.Text) || np2Dir.Text == "")
                {
                    np2Dir.BackColor = SystemColors.Window;
                    curCfg.np2Dir = np2Dir.Text;
                }
                else
                    np2Dir.BackColor = Color.Red;
            else if (sender == crapDir)
                if (Directory.Exists(crapDir.Text) || crapDir.Text == "")
                {
                    crapDir.BackColor = SystemColors.Window;
                    curCfg.crapDir = crapDir.Text;
                }
                else
                    crapDir.BackColor = Color.Red;
            else if (sender == crapStartingRepo)
                curCfg.StartingRepo = crapStartingRepo.Text;
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
                // Check for thcrap_configure_v3 first
                string procDir = curCfg.crapDir + "\\bin\\thcrap_configure_v3.exe";
                if (!File.Exists(procDir))
                    procDir = curCfg.crapDir + "\\bin\\thcrap_configure.exe";
                if (!File.Exists(procDir))
                    MessageBox.Show(rm.GetString("errorcrapNotFound"));
                else
                    startProcess(procDir);
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
