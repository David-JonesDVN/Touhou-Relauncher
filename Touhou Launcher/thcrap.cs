using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;

namespace Touhou_Launcher
{
    public partial class thcrap : Form
    {
        internal class repoData
        {
            public string contact = "";
            public string id = "";
            public List<string> neighbors = new List<string>();
            public Dictionary<string, string> patches = new Dictionary<string, string>();
            public List<string> servers = new List<string>();
            public string title = "";
        }

        internal class patchData
        {
            public List<string> dependencies = new List<string>();
            public string id = "";
            public List<string> servers = new List<string>();
            public string title = "";
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
        List<string> checkedRepos = new List<string>();
        List<string> patchStates = new List<string>();
        Dictionary<string, string> games = new Dictionary<string, string>();
        Queue<string> repoQueue = new Queue<string>();

        private void InitializeLanguage()
        {
            this.Text = MainForm.rm.GetString("thcrapTitle") + MainForm.rm.GetString(MainForm.gameNames.ElementAtOrDefault(cfgForm.game));
            foreach (ListView list in MainForm.GetAll<ListView>(this))
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
            gamejs = MainForm.curCfg.crapDir + "\\config\\launcher" + MainForm.FormatGameNumber(MainForm.gameNumbers[cfg.game]) + ".js";
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
            if (File.Exists(MainForm.curCfg.crapDir + "\\config\\games.js"))
                games = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(MainForm.curCfg.crapDir + "\\config\\games.js"));
            for (int i = 0; i < 3; i += 2)
            {
                if (MainForm.curCfg.gameCFG[cfgForm.game].GameDir[i] != "" && !games.ContainsValue(MainForm.curCfg.gameCFG[cfgForm.game].GameDir[i].Replace("\\", "/")))
                {
                    string thId = "th" + MainForm.FormatGameNumber(MainForm.gameNumbers[cfgForm.game]);
                    string augment = i == 2 ? "_custom" : "";
                    if (!games.ContainsKey(thId + augment))
                    {
                        games.Add(thId + augment, MainForm.curCfg.gameCFG[cfgForm.game].GameDir[i].Replace("\\", "/"));
                    }
                }
            }
            RefreshProfiles();
        }

        private async void searchRepo(string address)
        {
            HttpClient client = MainForm.client;
            while (true)
            {
                if (!checkedRepos.Contains(address))
                {
                    try
                    {
                        addRepo(await client.GetStringAsync(address + "/repo.js"));
                        checkedRepos.Add(address);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (repoQueue.Count > 0)
                {
                    address = repoQueue.Dequeue();
                }
                else break;
            }
        }

        private void addRepo(string repojs, bool offline = false)
        {
            try
            {
                repoData data = JsonConvert.DeserializeObject<repoData>(repojs);
                FileInfo jsPath = new FileInfo(MainForm.curCfg.crapDir + "\\repos\\" + data.id + "\\repo.js");
                jsPath.Directory.Create();
                File.WriteAllText(jsPath.FullName, repojs);
                if (!repoList.Items.ContainsKey(data.id) || (bool)repoList.Items[data.id].Tag == true)
                {
                    foreach (string neighbor in data.neighbors)
                    {
                        repoQueue.Enqueue(neighbor);
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
                    repoList_SelectedIndexChanged(repoList.Items[data.id], new EventArgs());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void onPatchGet(string patchString, string activeRepo)
        {
            patchData patch = JsonConvert.DeserializeObject<patchData>(patchString);
            FileInfo jsPath = new FileInfo(MainForm.curCfg.crapDir + "\\repos\\" + activeRepo + "\\" + patch.id + "\\patch.js");
            jsPath.Directory.Create();
            File.WriteAllText(jsPath.FullName, patchString);
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
                addPatch(repository, dependencySet[dependencySet.Length - 1], patchStates.IndexOf(activeRepo + "/" + patch.id + "/"));
            }
        }

        private async void addPatch(string repo, string patch, int dependent = -1)
        {
            if (!patchStates.Contains(repo + "/" + patch + "/"))
            {
                patchStates.Insert(dependent == -1 ? patchStates.Count : dependent, repo + "/" + patch + "/");
                repoList_SelectedIndexChanged(repoList.Items[repo], new EventArgs());
            }
            try
            {
                HttpClient client = MainForm.client;
                using (HttpResponseMessage response = await client.GetAsync(repos[repo].servers[0] + "/" + patch + "/patch.js"))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();
                    onPatchGet(content, repo);
                }
            }
            catch (Exception)
            {
            }
        }

        private void thcrap_Closing(object sender, FormClosingEventArgs e)
        {
            profileData profile = new profileData();
            foreach (string patch in patchStates)
            {
                profile.patches.Add(new Dictionary<string, string> { { "archive", "repos/" + patch } });
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
            if (sender == repoList || repoList.SelectedItems.Contains((ListViewItem)sender) || repoList.SelectedIndices.Contains(0))
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
        }

        private void patchList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            string id = repoList.SelectedIndices[0] == 0 ? e.Item.Text : repoList.SelectedItems[0].SubItems[1].Text + "/" + e.Item.Text + "/";
            if (e.Item.Checked)
            {
                addPatch(repoList.SelectedItems[0].SubItems[1].Text, e.Item.Text);
            }
            else
            {
                patchStates.Remove(id);
                if (repoList.SelectedIndices[0] == 0)
                    e.Item.Remove();
            }
        }

        private void browsePath_Click(object sender, EventArgs e)
        {
            foreach (string file in MainForm.FileBrowser(this, MainForm.rm.GetString("gameSelectTitle"), MainForm.rm.GetString("executableFilter") + " (*.exe, *.bat, *.lnk)|*.exe;*.bat;*.lnk|" + MainForm.rm.GetString("allFilter") + " (*.*)|*.*"))
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
