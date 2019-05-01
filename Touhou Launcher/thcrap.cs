using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Touhou_Launcher
{
    public partial class thcrap : Form
    {
        internal class repoData
        {
            public string contact;
            public string id;
            public List<string> neighbors = new List<string>();
            public Dictionary<string, string> patches = new Dictionary<string, string>();
            public List<string> servers = new List<string>();
            public string title;
        }

        internal class profileData
        {
            public bool console = false;
            public bool dat_dump = false;
            public List<Dictionary<string, string>> patches = new List<Dictionary<string, string>>();
        }

        ConfigForm cfgForm;
        string gamejs;
        Dictionary<string, repoData> repos = new Dictionary<string, repoData>();
        List<string> patchStates = new List<string> { "nmlgc/base_tasofro/", "nmlgc/base_tsa/" };
        Dictionary<string, string> games = new Dictionary<string, string>();

        private void InitializeLanguage()
        {
            this.Text = MainForm.rm.GetString("thcrapTitle") + MainForm.rm.GetString(MainForm.nameToID.FirstOrDefault(t => t.Value == cfgForm.game).Key);
            foreach (ListView list in MainForm.GetAll(this, typeof(ListView)))
            {
                foreach (ColumnHeader column in list.Columns)
                {
                    column.Text = MainForm.rm.GetString(column.Name);
                }
            }
            gameGroup.Text = MainForm.rm.GetString("gameGroup");
            patchGroup.Text = MainForm.rm.GetString("patchGroup");
            gameID.Text = MainForm.rm.GetString("gameIDColumn") + ":";
            gamePath.Text = MainForm.rm.GetString("pathColumn") + ":";
            browsePath.Text = MainForm.rm.GetString("browse");
            addGame.Text = MainForm.rm.GetString("customAdd");
            removeGame.Text = MainForm.rm.GetString("customRemove");
        }

        private void RefreshProfiles()
        {
            gameList.Items.Clear();
            foreach (KeyValuePair<string, string> game in games)
            {
                ListViewItem gameID = gameList.Items.Add(game.Key);
                gameID.SubItems.Add(game.Value);
            }
        }

        public thcrap(ConfigForm cfg)
        {
            cfgForm = cfg;
            gamejs = Path.GetDirectoryName(MainForm.curCfg.crapDir) + "\\launcher" + MainForm.idToNumber[cfg.game] + ".js";
            InitializeComponent();
            InitializeLanguage();
        }

        private void thcrap_Load(object sender, EventArgs e)
        {
            repoList.Items.Add(MainForm.rm.GetString("selectedPatches"));
            if (File.Exists(gamejs))
            {
                profileData profile = JsonConvert.DeserializeObject<profileData>(File.ReadAllText(gamejs));
                foreach (Dictionary<string, string> patch in profile.patches)
                {
                    if (!patchStates.Contains(patch["archive"]))
                        patchStates.Add(patch["archive"]);
                }
            }
            foreach (string localRepo in Directory.GetFiles(Path.GetDirectoryName(MainForm.curCfg.crapDir), "repo.js", SearchOption.AllDirectories))
            {
                addRepo(File.ReadAllText(localRepo));
            }
            searchRepo(MainForm.curCfg.StartingRepo);
            games = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.GetDirectoryName(MainForm.curCfg.crapDir) + "\\games.js"));
            RefreshProfiles();
        }

        private void searchRepo(string address, bool child = false)
        {
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += onJsonGet;
            wc.DownloadStringAsync(new Uri(address + "/repo.js"), new string[] { address, child.ToString() });
        }

        private void onJsonGet(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string json = e.Result;
                string[] args = (string[])e.UserState;
                addRepo(json);
                if (!bool.Parse(args[1]))
                {
                    searchRepo(args[0].Substring(0, args[0].LastIndexOf('/', args[0].LastIndexOf('/') - 1) + 1));
                }
            }
            catch (Exception ex)
            {
                /* Code for exploring the thcrap mirror manually.
                using (var reader = new StreamReader(WebRequest.Create(address).GetResponse().GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    System.Text.RegularExpressions.MatchCollection matches = new System.Text.RegularExpressions.Regex("<a href=\".*\">(?<name>.*)</a>").Matches(result);
                    //Alt Regex: <a\s+(?:[^>]*?\s+)?href=(["'])(.*?)\1
                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        if (!match.Success) { continue; }
                        if (match.Groups["name"].Value.EndsWith("/"))
                            searchRepo(address + match.Groups["name"].Value, true);
                    }
                }
                */
            }
        }

        private void addRepo(string repojs)
        {
            try
            {
                repoData data = JsonConvert.DeserializeObject<repoData>(repojs);
                foreach (string neighbor in data.neighbors)
                {
                    searchRepo(neighbor, true);
                }
                if (!repos.ContainsKey(data.id))
                {
                    repos[data.id] = data;
                    ListViewItem title = repoList.Items.Add(data.title);
                    title.SubItems.Add(data.id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void thcrap_Closing(object sender, FormClosingEventArgs e)
        {
            profileData profile = new profileData();
            foreach (string patch in patchStates)
            {
                profile.patches.Add(new Dictionary<string, string> { {"archive", patch} });
            }
            File.WriteAllText(gamejs, JsonConvert.SerializeObject(profile, Formatting.Indented));
            Dictionary<string, string> games = new Dictionary<string, string>();
            foreach (ListViewItem game in gameList.Items)
            {
                games[game.Text] = game.SubItems[1].Text;
            }
            File.WriteAllText(Path.GetDirectoryName(MainForm.curCfg.crapDir) + "\\games.js", JsonConvert.SerializeObject(games, Formatting.Indented));
            cfgForm.Refreshcrap();
        }

        private void repoList_SelectedIndexChanged(object sender, EventArgs e)
        {
            patchList.Items.Clear();
            if (repoList.SelectedItems.Count > 0)
            {
                patchList.ItemChecked -= patchList_ItemCheck;
                if (repoList.SelectedIndices[0] == 0)
                {
                    foreach (string patch in patchStates)
                    {
                        ListViewItem title = patchList.Items.Add(patch);
                        title.Checked = true;
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, string> patch in repos[repoList.SelectedItems[0].SubItems[1].Text].patches)
                    {
                        ListViewItem title = patchList.Items.Add(patch.Key);
                        title.SubItems.Add(patch.Value);
                        title.Checked = patchStates.Contains(repoList.SelectedItems[0].SubItems[1].Text + "/" + patch.Key + "/");
                    }
                }
                patchList.ItemChecked += patchList_ItemCheck;
            }
        }

        private void patchList_ItemCheck(object sender, ItemCheckedEventArgs e)
        {
            string id = repoList.SelectedIndices[0] == 0 ? e.Item.Text : repoList.SelectedItems[0].SubItems[1].Text + "/" + e.Item.Text + "/";
            if (e.Item.Checked && !patchStates.Contains(id))
                patchStates.Add(id);
            else if (patchStates.Contains(id))
            {
                patchStates.Remove(id);
                if (repoList.SelectedIndices[0] == 0)
                    e.Item.Remove();
            }
        }

        private void browsePath_Click(object sender, EventArgs e)
        {
            foreach (string file in MainForm.FileBrowser(MainForm.rm.GetString("gameSelectTitle"), MainForm.rm.GetString("executableFilter") + " (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*"))
                path.Text = file;
        }

        private void addGame_Click(object sender, EventArgs e)
        {
            if (File.Exists(path.Text))
            {
                games[id.Text] = path.Text.Replace("\\", "/");
                RefreshProfiles();
            }
        }

        private void removeGame_Click(object sender, EventArgs e)
        {
            games.Remove(id.Text);
            RefreshProfiles();
        }

        private void gameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gameList.SelectedItems.Count > 0)
            {
                id.Text = gameList.SelectedItems[0].Text;
                path.Text = gameList.SelectedItems[0].SubItems[1].Text.Replace("/", "\\");
            }
        }
    }
}
