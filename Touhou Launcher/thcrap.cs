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

        internal class patchData
        {
            public List<string> dependencies = new List<string>();
            public string id;
            public List<string> servers = new List<string>();
            public string title;
            public Dictionary<string, bool> fonts = new Dictionary<string, bool>();
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
        List<string> patchStates = new List<string>();
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
            gamejs = MainForm.curCfg.crapDir + "\\config\\launcher" + MainForm.idToNumber[cfg.game] + ".js";
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
                        patchStates.Add(patch["archive"].Substring(6));
                }
            }
            foreach (FileInfo localRepo in new DirectoryInfo(MainForm.curCfg.crapDir).CreateSubdirectory("repos").GetFiles("repo.js", SearchOption.AllDirectories))
            {
                addRepo(File.ReadAllText(localRepo.FullName), true);
            }
            searchRepo(MainForm.curCfg.StartingRepo);
            games = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(MainForm.curCfg.crapDir + "\\config\\games.js"));
            for (int i = 0; i < 3; i += 2)
            {
                if (MainForm.curCfg.gameCFG[cfgForm.game].GameDir[i] != "" && !games.ContainsValue(MainForm.curCfg.gameCFG[cfgForm.game].GameDir[i].Replace("\\", "/")))
                {
                    string augment = i == 2 ? "_custom" : "";
                    games.Add("th" + (MainForm.idToNumber[cfgForm.game]).ToString("00") + augment, MainForm.curCfg.gameCFG[cfgForm.game].GameDir[i].Replace("\\", "/"));
                }
            }
            RefreshProfiles();
        }

        private void searchRepo(string address, bool child = false)
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.DownloadStringCompleted += onJsonGet;
            wc.DownloadStringAsync(new Uri(address + "/repo.js"), new string[] { address, child.ToString() });
        }

        private void onJsonGet(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled)
            {
                string json = e.Result;
                string[] args = (string[])e.UserState;
                addRepo(json);
                if (!bool.Parse(args[1]))
                {
                    searchRepo(args[0].Substring(0, args[0].LastIndexOf('/', args[0].LastIndexOf('/') - 1) + 1));
                }
            }
            else
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

        private void addRepo(string repojs, bool offline = false)
        {
            try
            {
                repoData data = JsonConvert.DeserializeObject<repoData>(repojs);
                if (!repoList.Items.ContainsKey(data.id) || (bool)repoList.Items[data.id].Tag == true)
                {
                    foreach (string neighbor in data.neighbors)
                    {
                        searchRepo(neighbor, true);
                    }
                    repos[data.id] = data;
                    if (!repoList.Items.ContainsKey(data.id))
                    {
                        ListViewItem title = repoList.Items.Add(data.title);
                        title.Name = data.id;
                        title.Tag = offline;
                        title.SubItems.Add(data.id);
                    }
                    else
                    {
                        ListViewItem title = repoList.Items[data.id];
                        title.Tag = offline;
                    }
                    repoList_SelectedIndexChanged(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void onPatchGet(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                patchData patch = JsonConvert.DeserializeObject<patchData>(e.Result);
                FileInfo jsPath = new FileInfo(MainForm.curCfg.crapDir + "\\repos\\" + (string)e.UserState + "\\" + patch.id + "\\patch.js");
                if (!jsPath.Exists)
                {
                    jsPath.Directory.Create();
                    File.WriteAllText(jsPath.FullName, e.Result);
                }
                foreach (string dependency in patch.dependencies)
                {
                    string[] dependencySet = dependency.Split('/');
                    string repository = "";
                    if (dependencySet.Length == 1)
                    {
                        foreach (KeyValuePair<string, repoData> repo in repos)
                        {
                            if (repo.Value.patches.ContainsKey(dependencySet[0]))
                            {
                                repository = repo.Key;
                            }
                        }
                    }
                    else
                        repository = dependencySet[0];
                    addPatch(repository, dependencySet[dependencySet.Length - 1], (string)e.UserState + "/" + patch.id + "/");
                }
            }
        }

        private void addPatch(string repo, string patch, string dependent = "")
        {
            if (!patchStates.Contains(repo + "/" + patch + "/"))
            {
                int depIndex = patchStates.IndexOf(dependent);
                depIndex = depIndex == -1 || dependent == "" ? patchStates.Count : depIndex;
                patchStates.Insert(depIndex, repo + "/" + patch + "/");
                repoList_SelectedIndexChanged(this, new EventArgs());
            }
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.DownloadStringCompleted += onPatchGet;
            wc.DownloadStringAsync(new Uri(repos[repo].servers[0] + "/" + patch + "/patch.js"), repo);
        }

        private void thcrap_Closing(object sender, FormClosingEventArgs e)
        {
            profileData profile = new profileData();
            foreach (string patch in patchStates)
            {
                profile.patches.Add(new Dictionary<string, string> { {"archive", "repos/" + patch} });
            }
            File.WriteAllText(gamejs, JsonConvert.SerializeObject(profile, Formatting.Indented));
            Dictionary<string, string> games = new Dictionary<string, string>();
            foreach (ListViewItem game in gameList.Items)
            {
                games[game.Text] = game.SubItems[1].Text;
            }
            File.WriteAllText(MainForm.curCfg.crapDir + "\\config\\games.js", JsonConvert.SerializeObject(games, Formatting.Indented));
            cfgForm.Refreshcrap();
        }

        private void repoList_SelectedIndexChanged(object sender, EventArgs e)
        {
            patchList.Items.Clear();
            if (repoList.SelectedItems.Count > 0)
            {
                patchList.ItemChecked -= patchList_ItemChecked;
                if (repoList.SelectedIndices[0] == 0)
                {
                    foreach (string patch in patchStates)
                    {
                        string[] patchSet = patch.Split('/');
                        ListViewItem title = patchList.Items.Add(patch);
                        title.SubItems.Add(repos[patchSet[0]].patches[patchSet[1]]);
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
                patchList.ItemChecked += patchList_ItemChecked;
            }
        }

        private void patchList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            string id = repoList.SelectedIndices[0] == 0 ? e.Item.Text : repoList.SelectedItems[0].SubItems[1].Text + "/" + e.Item.Text + "/";
            if (e.Item.Checked)
            {
                addPatch(repoList.SelectedItems[0].SubItems[1].Text, e.Item.Text);
            }
            else if (patchStates.Contains(id)) //I have no idea what this is
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
