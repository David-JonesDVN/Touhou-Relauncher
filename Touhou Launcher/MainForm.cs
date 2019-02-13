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
            public List<bool> appLocale = new List<bool>(4);
            public int DefaultDir = 0;
            public bool DefaultApplocale = false;
            public bool customBanner = false;
            public string bannerOn;
            public string bannerOff;
        }

        public class SubNode
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public List<SubNode> Nodes = new List<SubNode>();
            public Dictionary<string, string> Games = new Dictionary<string, string>();
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
            public int language = 0;
            public string np2Dir = "";
            public bool autoClose = false;
            public View customView = View.LargeIcon;
            public SortOrder customSort = SortOrder.Ascending;
            public Configs()
            {
                for (int i = 0; i < gameCFG.Length ; i++)
                {
                    gameCFG[i] = new GameConfig();
                    gameCFG[i].GameDir = new List<string> { "", "", "", "" };
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

        public IEnumerable<Control> GetAll(Control control, Type type)
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

        private SubNode TreeToJSON(TreeNode parent, SubNode serializableTree)
        {
            foreach (TreeNode node in parent.Nodes)
            {
                SubNode snode = new SubNode();
                snode.Name = node.Name;
                snode.Text = node.Text;
                if (node.Tag != null)
                    snode.Games = (Dictionary<string, string>)node.Tag;
                serializableTree.Nodes.Add(snode);
                TreeToJSON(node, snode);
            }
            return serializableTree;
        }

        private SubNode TreeToJSON(TreeView parent, SubNode serializableTree)
        {
            serializableTree.Name = "Root";
            serializableTree.Text = "The root node";
            foreach (TreeNode node in parent.Nodes)
            {
                SubNode snode = new SubNode();
                snode.Name = node.Name;
                snode.Text = node.Text;
                if (node.Tag != null)
                    snode.Games = (Dictionary<string, string>)node.Tag;
                serializableTree.Nodes.Add(snode);
                TreeToJSON(node, snode);
            }
            return serializableTree;
        }

        private TreeNodeCollection JSONToTree(SubNode nodeList, TreeView parent)
        {
            for (int i = 0; i < nodeList.Nodes.Count; i++)
            {
                parent.Nodes.Add(nodeList.Nodes[i].Name, nodeList.Nodes[i].Text);
                parent.Nodes[parent.Nodes.Count - 1].Tag = nodeList.Nodes[i].Games;
                JSONToTree(nodeList.Nodes[i], parent.Nodes[i]);
            }
            return parent.Nodes;
        }

        private TreeNodeCollection JSONToTree(SubNode nodeList, TreeNode parent)
        {
            for (int i = 0; i < nodeList.Nodes.Count; i++)
            {
                parent.Nodes.Add(nodeList.Nodes[i].Name, nodeList.Nodes[i].Text);
                parent.Nodes[parent.Nodes.Count - 1].Tag = nodeList.Nodes[i].Games;
                JSONToTree(nodeList.Nodes[i], parent.Nodes[i]);
            }
            return parent.Nodes;
        }

        public static bool NekoProject(string hdi)
        {
            string[] config = File.ReadAllLines(Path.GetDirectoryName(curCfg.np2Dir) + "\\np21nt.ini", Encoding.Unicode);
            for (int i = 0; i < config.Length; i++)
            {
                if (config[i].Contains("HDD1FILE="))
                {
                    if (config[i] != "HDD1FILE=" + hdi)
                    {
                        config[i] = "HDD1FILE=" + hdi;
                        File.WriteAllLines(Path.GetDirectoryName(curCfg.np2Dir) + "\\np21nt.ini", config, Encoding.Unicode);
                    }
                    return true;
                }
            }
            return false;
        }

        public static string[] FileBrowser(string title, string filter, bool multiSelect = false)
        {
            OpenFileDialog browser = new OpenFileDialog();
            browser.Filter = filter;
            browser.FilterIndex = 1;
            browser.InitialDirectory = Directory.GetCurrentDirectory();
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
            DialogResult confirm = MessageBox.Show(message, "Download Replay?", MessageBoxButtons.YesNoCancel);
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
            RefreshGames();
            JSONToTree(curCfg.Custom, treeView1);
            languageBox.SelectedIndexChanged -= languageBox_SelectedIndexChanged;
            languageBox.SelectedIndex = curCfg.language;
            languageBox.SelectedIndexChanged += languageBox_SelectedIndexChanged;
            np2Dir.Text = curCfg.np2Dir;
        }

        private void InitializeLanguage()
        {
            switch (curCfg.language)
            {
                case 0: rm = new System.Resources.ResourceManager("Touhou_Launcher.Resources_en", System.Reflection.Assembly.GetExecutingAssembly());
                    break;
                case 1: rm = new System.Resources.ResourceManager("Touhou_Launcher.Resources_jp", System.Reflection.Assembly.GetExecutingAssembly());
                    break;
                case 2: if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\Resources_custom.resources"))
                        rm = System.Resources.ResourceManager.CreateFileBasedResourceManager("Resources_custom", Path.GetDirectoryName(Application.ExecutablePath), null);
                    else
                        languageBox.SelectedIndex = 0;
                    break;
            }
            foreach (Button btn in GetAll(games, typeof(Button)))
            {
                btn.Text = rm.GetString(btn.Name.Substring(3));
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
            mainGroup.Text = rm.GetString("mainGroup");
            fightingGroup.Text = rm.GetString("fightingGroup");
            otherGroup.Text = rm.GetString("otherGroup");
            configureToolStripMenuItem.Text = rm.GetString("configureToolStripMenuItem");
            customAdd.Text = rm.GetString("customAdd");
            newCategoryToolStripMenuItem.Text = rm.GetString("newCategoryToolStripMenuItem");
            renameCategoryToolStripMenuItem.Text = rm.GetString("renameCategoryToolStripMenuItem");
            deleteCategoryToolStripMenuItem.Text = rm.GetString("deleteCategoryToolStripMenuItem");
            ascendingToolStripMenuItem.Text = rm.GetString("ascendingToolStripMenuItem");
            descendingToolStripMenuItem.Text = rm.GetString("descendingToolStripMenuItem");
            autoClose.Text = rm.GetString("autoClose");
            toolTip.SetToolTip(autoClose, rm.GetString("autoCloseToolTip"));
            langLabel.Text = rm.GetString("langLabel");
            browseNP2.Text = rm.GetString("browse");
            randomAll.Text = rm.GetString("randomAll");
            randomNone.Text = rm.GetString("randomNone");
            mainRandom.Text = rm.GetString("mainGroup");
            fightingRandom.Text = rm.GetString("fightingGroup");
            otherRandom.Text = rm.GetString("otherGroup");
            this.Text = rm.GetString("Title");
        }

        public void RefreshGames()
        {
            foreach (Button btn in GetAll(games, typeof(Button)))
            {
                if (btn.Name != "btnRandom")
                {
                    int game = nameToID[btn.Name.Substring(3)];
                    bool exists = false;
                    foreach (string dir in curCfg.gameCFG[game].GameDir)
                        if (dir != "")
                        {
                            exists = true;
                            break;
                        }
                    if (exists)
                    {
                        if (curCfg.gameCFG[game].customBanner && curCfg.gameCFG[game].bannerOn != null)
                            btn.Image = Image.FromFile(curCfg.gameCFG[game].bannerOn);
                        else
                            btn.Image = (System.Drawing.Bitmap)Touhou_Launcher.Properties.Resources.ResourceManager.GetObject((btn.Name == "btnIN" ? "_" : "") + btn.Name.Substring(3).ToLower());
                    }
                    else
                    {
                        if (curCfg.gameCFG[game].customBanner && curCfg.gameCFG[game].bannerOff != null)
                            btn.Image = Image.FromFile(curCfg.gameCFG[game].bannerOff);
                        else
                            btn.Image = (System.Drawing.Bitmap)Touhou_Launcher.Properties.Resources.ResourceManager.GetObject((btn.Name == "btnIN" ? "_" : "") + btn.Name.Substring(3).ToLower() + "g");

                    }
                }
            }
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
            LoadSettings();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            int game = nameToID[((Button)sender).Name.Substring(3)];
            string path = "";
            string args = "";
            GameConfig curGame = curCfg.gameCFG[game];
            if (File.Exists(curGame.GameDir[curGame.DefaultDir]))
            {
            	if (game > 4)
            	{
	                path = curGame.GameDir[curGame.DefaultDir];
	                if (curGame.DefaultApplocale)
	                {
	                    args = "\"" + path + "\" \"/L0411\"";
	                    path = "C:\\Windows\\AppPatch\\AppLoc.exe";
	                }
            	}
            	else
            	{
                    if (!File.Exists(curCfg.np2Dir))
                        MessageBox.Show(rm.GetString("errorNP2NotFound"));
                    else if (!NekoProject(curGame.GameDir[0]))
                        MessageBox.Show(rm.GetString("errorInvalidNP2INI"));
                    else
                        path = curCfg.np2Dir;
            	}
            }
            if (path != "")
            {
                Process.Start(path, args);
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
                MessageBox.Show("No games selected");
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = nameToID[((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).SourceControl.Name.Substring(3)];
            ConfigForm gameConfig = new ConfigForm(id, this);
            gameConfig.ShowDialog();
        }

        private void customAdd_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                foreach (string file in FileBrowser("Select fangame executable(s)", "Executable Files (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|All Files (*.*)|*.*", true))
                {
                    ((Dictionary<string, string>)treeView1.SelectedNode.Tag).Add(file, Path.GetFileNameWithoutExtension(file));
                }
                curCfg.Custom = TreeToJSON(treeView1, new SubNode());
                curCfg.Save();
                RefreshList(ref listView1, (Dictionary<string, string>)treeView1.SelectedNode.Tag);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Bounds.Contains(new Point(e.X, e.Y)))
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
            int i = 0;
        search:
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.Name == e.Label.Replace(" ", "") + i)
                {
                    i++;
                    goto search;
                }
            }
            e.Node.Name = e.Label.Replace(" ", "") + i;
            e.Node.Text = e.Label;
            curCfg.Custom = TreeToJSON(treeView1, new SubNode());
            curCfg.Save();
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
            curCfg.Custom = TreeToJSON(treeView1, new SubNode());
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
                treeView1.SelectedNode.Remove();
                curCfg.Custom = TreeToJSON(treeView1, new SubNode());
                curCfg.Save();
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            string text = e.IsSelected ? e.Item.Name : null;
            customLabel.Text = text;
            toolTip.SetToolTip(customLabel, text);
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                ((Dictionary<string, string>)treeView1.SelectedNode.Tag).Add(file, Path.GetFileNameWithoutExtension(file));
            }
            curCfg.Custom = TreeToJSON(treeView1, new SubNode());
            curCfg.Save();
            RefreshList(ref listView1, (Dictionary<string, string>)treeView1.SelectedNode.Tag);
        }

        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ((Dictionary<string, string>)treeView1.SelectedNode.Tag)[listView1.Items[e.Item].Name] = e.Label;
            curCfg.Custom = TreeToJSON(treeView1, new SubNode());
            curCfg.Save();
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
                listView1.Items.Remove(game);
                ((Dictionary<string, string>)treeView1.SelectedNode.Tag).Remove(game.Name);
                curCfg.Custom = TreeToJSON(treeView1, new SubNode());
                curCfg.Save();
                RefreshList(ref listView1, (Dictionary<string, string>)treeView1.SelectedNode.Tag);
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
            replayBrowser.Navigate(((RadioButton)sender).Text);
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
                    e.Cancel = false;
                }
            }
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

        private void browseNP2_Click(object sender, EventArgs e)
        {
            foreach (string file in MainForm.FileBrowser(MainForm.rm.GetString("np2SelectTitle"), MainForm.rm.GetString("executableFilter") + " (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*"))
            {
                np2Dir.BackColor = SystemColors.Window;
                np2Dir.Text = file;
                MainForm.curCfg.np2Dir = file;
            }
        }

        private void np2Dir_LostFocus(object sender, EventArgs e)
        {
            if (File.Exists(np2Dir.Text) || np2Dir.Text == "")
            {
                ((TextBox)sender).BackColor = SystemColors.Window;
                MainForm.curCfg.np2Dir = np2Dir.Text;
            }
            else
                ((TextBox)sender).BackColor = Color.Red;
        }

        private void autoClose_CheckedChanged(object sender, EventArgs e)
        {
            curCfg.autoClose = autoClose.Checked;
            curCfg.Save();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.widdiful.co.uk/irc/touhou-launcher.htm");
        }

    }
}
