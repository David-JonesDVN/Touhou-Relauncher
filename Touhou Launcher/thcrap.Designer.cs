namespace Touhou_Launcher
{
    partial class thcrap
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(thcrap));
            this.repoList = new System.Windows.Forms.ListView();
            this.titleColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.idColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gameList = new System.Windows.Forms.ListView();
            this.gameIDColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pathColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.path = new System.Windows.Forms.TextBox();
            this.gameID = new System.Windows.Forms.Label();
            this.gamePath = new System.Windows.Forms.Label();
            this.gameGroup = new System.Windows.Forms.GroupBox();
            this.id = new System.Windows.Forms.TextBox();
            this.removeGame = new System.Windows.Forms.Button();
            this.addGame = new System.Windows.Forms.Button();
            this.browsePath = new System.Windows.Forms.Button();
            this.patchGroup = new System.Windows.Forms.GroupBox();
            this.patchList = new System.Windows.Forms.ListView();
            this.patchColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descriptionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gameGroup.SuspendLayout();
            this.patchGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // repoList
            // 
            this.repoList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.repoList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.titleColumn,
            this.idColumn});
            this.repoList.FullRowSelect = true;
            this.repoList.GridLines = true;
            this.repoList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.repoList.HideSelection = false;
            this.repoList.Location = new System.Drawing.Point(6, 19);
            this.repoList.MultiSelect = false;
            this.repoList.Name = "repoList";
            this.repoList.ShowItemToolTips = true;
            this.repoList.Size = new System.Drawing.Size(275, 319);
            this.repoList.TabIndex = 0;
            this.repoList.UseCompatibleStateImageBehavior = false;
            this.repoList.View = System.Windows.Forms.View.Details;
            this.repoList.SelectedIndexChanged += new System.EventHandler(this.repoList_SelectedIndexChanged);
            // 
            // titleColumn
            // 
            this.titleColumn.Name = "titleColumn";
            this.titleColumn.Text = "Title";
            this.titleColumn.Width = 175;
            // 
            // idColumn
            // 
            this.idColumn.Name = "idColumn";
            this.idColumn.Text = "ID";
            this.idColumn.Width = 95;
            // 
            // gameList
            // 
            this.gameList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.gameIDColumn,
            this.pathColumn});
            this.gameList.FullRowSelect = true;
            this.gameList.GridLines = true;
            this.gameList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.gameList.Location = new System.Drawing.Point(6, 19);
            this.gameList.Name = "gameList";
            this.gameList.Size = new System.Drawing.Size(275, 115);
            this.gameList.TabIndex = 0;
            this.gameList.UseCompatibleStateImageBehavior = false;
            this.gameList.View = System.Windows.Forms.View.Details;
            this.gameList.SelectedIndexChanged += new System.EventHandler(this.gameList_SelectedIndexChanged);
            // 
            // gameIDColumn
            // 
            this.gameIDColumn.Name = "gameIDColumn";
            this.gameIDColumn.Text = "Game ID";
            this.gameIDColumn.Width = 87;
            // 
            // pathColumn
            // 
            this.pathColumn.Name = "pathColumn";
            this.pathColumn.Text = "Game Path";
            this.pathColumn.Width = 183;
            // 
            // path
            // 
            this.path.Location = new System.Drawing.Point(287, 74);
            this.path.Name = "path";
            this.path.Size = new System.Drawing.Size(189, 20);
            this.path.TabIndex = 2;
            // 
            // gameID
            // 
            this.gameID.AutoSize = true;
            this.gameID.Location = new System.Drawing.Point(287, 19);
            this.gameID.Name = "gameID";
            this.gameID.Size = new System.Drawing.Size(52, 13);
            this.gameID.TabIndex = 5;
            this.gameID.Text = "Game ID:";
            // 
            // gamePath
            // 
            this.gamePath.AutoSize = true;
            this.gamePath.Location = new System.Drawing.Point(287, 58);
            this.gamePath.Name = "gamePath";
            this.gamePath.Size = new System.Drawing.Size(63, 13);
            this.gamePath.TabIndex = 6;
            this.gamePath.Text = "Game Path:";
            // 
            // gameGroup
            // 
            this.gameGroup.Controls.Add(this.id);
            this.gameGroup.Controls.Add(this.removeGame);
            this.gameGroup.Controls.Add(this.addGame);
            this.gameGroup.Controls.Add(this.browsePath);
            this.gameGroup.Controls.Add(this.gamePath);
            this.gameGroup.Controls.Add(this.gameList);
            this.gameGroup.Controls.Add(this.gameID);
            this.gameGroup.Controls.Add(this.path);
            this.gameGroup.Location = new System.Drawing.Point(9, 362);
            this.gameGroup.Margin = new System.Windows.Forms.Padding(0);
            this.gameGroup.Name = "gameGroup";
            this.gameGroup.Size = new System.Drawing.Size(563, 140);
            this.gameGroup.TabIndex = 1;
            this.gameGroup.TabStop = false;
            this.gameGroup.Text = "Game Profiles";
            // 
            // id
            // 
            this.id.Location = new System.Drawing.Point(290, 36);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(267, 20);
            this.id.TabIndex = 1;
            // 
            // removeGame
            // 
            this.removeGame.Location = new System.Drawing.Point(427, 111);
            this.removeGame.Name = "removeGame";
            this.removeGame.Size = new System.Drawing.Size(130, 23);
            this.removeGame.TabIndex = 5;
            this.removeGame.Text = "Remove Game";
            this.removeGame.UseVisualStyleBackColor = true;
            this.removeGame.Click += new System.EventHandler(this.removeGame_Click);
            // 
            // addGame
            // 
            this.addGame.Location = new System.Drawing.Point(290, 111);
            this.addGame.Name = "addGame";
            this.addGame.Size = new System.Drawing.Size(130, 23);
            this.addGame.TabIndex = 4;
            this.addGame.Text = "Add Game";
            this.addGame.UseVisualStyleBackColor = true;
            this.addGame.Click += new System.EventHandler(this.addGame_Click);
            // 
            // browsePath
            // 
            this.browsePath.Location = new System.Drawing.Point(482, 72);
            this.browsePath.Name = "browsePath";
            this.browsePath.Size = new System.Drawing.Size(75, 23);
            this.browsePath.TabIndex = 3;
            this.browsePath.Text = "Browse";
            this.browsePath.UseVisualStyleBackColor = true;
            this.browsePath.Click += new System.EventHandler(this.browsePath_Click);
            // 
            // patchGroup
            // 
            this.patchGroup.Controls.Add(this.patchList);
            this.patchGroup.Controls.Add(this.repoList);
            this.patchGroup.Location = new System.Drawing.Point(9, 9);
            this.patchGroup.Margin = new System.Windows.Forms.Padding(0);
            this.patchGroup.Name = "patchGroup";
            this.patchGroup.Size = new System.Drawing.Size(566, 347);
            this.patchGroup.TabIndex = 0;
            this.patchGroup.TabStop = false;
            this.patchGroup.Text = "Custom Profile";
            // 
            // patchList
            // 
            this.patchList.CheckBoxes = true;
            this.patchList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.patchColumn,
            this.descriptionColumn});
            this.patchList.FullRowSelect = true;
            this.patchList.GridLines = true;
            this.patchList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.patchList.HideSelection = false;
            this.patchList.Location = new System.Drawing.Point(287, 19);
            this.patchList.MultiSelect = false;
            this.patchList.Name = "patchList";
            this.patchList.ShowItemToolTips = true;
            this.patchList.Size = new System.Drawing.Size(273, 319);
            this.patchList.TabIndex = 1;
            this.patchList.UseCompatibleStateImageBehavior = false;
            this.patchList.View = System.Windows.Forms.View.Details;
            // 
            // patchColumn
            // 
            this.patchColumn.Name = "patchColumn";
            this.patchColumn.Text = "Patch";
            this.patchColumn.Width = 100;
            // 
            // descriptionColumn
            // 
            this.descriptionColumn.Name = "descriptionColumn";
            this.descriptionColumn.Text = "Description";
            this.descriptionColumn.Width = 169;
            // 
            // thcrap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 512);
            this.Controls.Add(this.patchGroup);
            this.Controls.Add(this.gameGroup);
            this.Icon = global::Touhou_Launcher.Properties.Resources.thicon;
            this.Name = "thcrap";
            this.Text = "Profile Configuration: ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.thcrap_Closing);
            this.Load += new System.EventHandler(this.thcrap_Load);
            this.gameGroup.ResumeLayout(false);
            this.gameGroup.PerformLayout();
            this.patchGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView repoList;
        private System.Windows.Forms.ColumnHeader titleColumn;
        private System.Windows.Forms.ColumnHeader idColumn;
        private System.Windows.Forms.ListView gameList;
        private System.Windows.Forms.ColumnHeader gameIDColumn;
        private System.Windows.Forms.ColumnHeader pathColumn;
        private System.Windows.Forms.TextBox path;
        private System.Windows.Forms.Label gameID;
        private System.Windows.Forms.Label gamePath;
        private System.Windows.Forms.GroupBox gameGroup;
        private System.Windows.Forms.Button removeGame;
        private System.Windows.Forms.Button addGame;
        private System.Windows.Forms.Button browsePath;
        private System.Windows.Forms.GroupBox patchGroup;
        private System.Windows.Forms.ListView patchList;
        private System.Windows.Forms.ColumnHeader patchColumn;
        private System.Windows.Forms.ColumnHeader descriptionColumn;
        private System.Windows.Forms.TextBox id;

    }
}