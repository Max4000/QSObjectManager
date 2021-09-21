
namespace QSObjectManager
{
    partial class ManagerForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.folderBrowserDialogPathsHistoru = new System.Windows.Forms.FolderBrowserDialog();
            this.tabPageAppConfiguration = new System.Windows.Forms.TabPage();
            this.groupBoxOptionsPaths = new System.Windows.Forms.GroupBox();
            this.buttonHistoryPath = new System.Windows.Forms.Button();
            this.textBoxHistoryPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPageRestore = new System.Windows.Forms.TabPage();
            this.buttonRestoreHistoryOnRestoreTab = new System.Windows.Forms.Button();
            this.groupConnectionOnRestoreTab = new System.Windows.Forms.GroupBox();
            this.groupBoxConnectionToRemoteServer = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonConnectToServerOnRestoreTab = new System.Windows.Forms.Button();
            this.textBoxAddrServer = new System.Windows.Forms.TextBox();
            this.buttonDisconnectFromServerOnRestoreTab = new System.Windows.Forms.Button();
            this.groupBoxConnectoionToLocalHostOnRestoreTab = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonConnectionToLocalHostOnRestoreTab = new System.Windows.Forms.Button();
            this.textBoxAdressLocalHostOnRestoreTab = new System.Windows.Forms.TextBox();
            this.buttonDisconnectFromLocalHostOnRestoreTab = new System.Windows.Forms.Button();
            this.groupBoxActionsOnRestoreTab = new System.Windows.Forms.GroupBox();
            this.groupBoxUsersHistoryInStoreOnRestoreTab = new System.Windows.Forms.GroupBox();
            this.listBoxHistorysInStoreOnRestoreTab = new System.Windows.Forms.ListBox();
            this.groupBoxAppsInStoreOnRestoreTab = new System.Windows.Forms.GroupBox();
            this.listBoxAppsInStoreOnRestoreTab = new System.Windows.Forms.ListBox();
            this.tabPageSave = new System.Windows.Forms.TabPage();
            this.buttonSaveHistoryToLocalStore = new System.Windows.Forms.Button();
            this.groupBoxActionsForImportToLocalStore = new System.Windows.Forms.GroupBox();
            this.groupBoxStorysFromDevHub = new System.Windows.Forms.GroupBox();
            this.listBoxStrorysFromDevHub = new System.Windows.Forms.ListBox();
            this.groupBoxAppsFromDevHub = new System.Windows.Forms.GroupBox();
            this.ListBoxAppsFromDevHub = new System.Windows.Forms.ListBox();
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab = new System.Windows.Forms.GroupBox();
            this.groupBoxConnectToServerOnSaveTab = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonConnectToServer = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.buttonDisconnectFromServer = new System.Windows.Forms.Button();
            this.groupBoxConnectToLocalCоmputer = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonConnectToLocalHub = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonDisconnectFromLoacalHub = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ButtonSelectAllHistToWrite = new System.Windows.Forms.Button();
            this.ButtonSelectAllHistToRestore = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tabPageAppConfiguration.SuspendLayout();
            this.groupBoxOptionsPaths.SuspendLayout();
            this.tabPageRestore.SuspendLayout();
            this.groupConnectionOnRestoreTab.SuspendLayout();
            this.groupBoxConnectionToRemoteServer.SuspendLayout();
            this.groupBoxConnectoionToLocalHostOnRestoreTab.SuspendLayout();
            this.groupBoxUsersHistoryInStoreOnRestoreTab.SuspendLayout();
            this.groupBoxAppsInStoreOnRestoreTab.SuspendLayout();
            this.tabPageSave.SuspendLayout();
            this.groupBoxStorysFromDevHub.SuspendLayout();
            this.groupBoxAppsFromDevHub.SuspendLayout();
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.SuspendLayout();
            this.groupBoxConnectToServerOnSaveTab.SuspendLayout();
            this.groupBoxConnectToLocalCоmputer.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.AboutProgramToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(999, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.FileToolStripMenuItem.Text = "Файл";
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.ExitToolStripMenuItem.Text = "Выход";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // AboutProgramToolStripMenuItem
            // 
            this.AboutProgramToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.AboutProgramToolStripMenuItem.Name = "AboutProgramToolStripMenuItem";
            this.AboutProgramToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            this.AboutProgramToolStripMenuItem.Text = "О программе";
            this.AboutProgramToolStripMenuItem.Click += new System.EventHandler(this.AboutProgramToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 525);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(999, 22);
            this.statusStrip1.Stretch = false;
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tabPageAppConfiguration
            // 
            this.tabPageAppConfiguration.Controls.Add(this.groupBoxOptionsPaths);
            this.tabPageAppConfiguration.Location = new System.Drawing.Point(4, 24);
            this.tabPageAppConfiguration.Name = "tabPageAppConfiguration";
            this.tabPageAppConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAppConfiguration.Size = new System.Drawing.Size(991, 473);
            this.tabPageAppConfiguration.TabIndex = 2;
            this.tabPageAppConfiguration.Text = "Настройки";
            this.tabPageAppConfiguration.UseVisualStyleBackColor = true;
            // 
            // groupBoxOptionsPaths
            // 
            this.groupBoxOptionsPaths.Controls.Add(this.buttonHistoryPath);
            this.groupBoxOptionsPaths.Controls.Add(this.textBoxHistoryPath);
            this.groupBoxOptionsPaths.Controls.Add(this.label2);
            this.groupBoxOptionsPaths.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxOptionsPaths.Location = new System.Drawing.Point(3, 3);
            this.groupBoxOptionsPaths.Name = "groupBoxOptionsPaths";
            this.groupBoxOptionsPaths.Size = new System.Drawing.Size(985, 121);
            this.groupBoxOptionsPaths.TabIndex = 0;
            this.groupBoxOptionsPaths.TabStop = false;
            this.groupBoxOptionsPaths.Text = "Папки";
            // 
            // buttonHistoryPath
            // 
            this.buttonHistoryPath.Location = new System.Drawing.Point(705, 33);
            this.buttonHistoryPath.Name = "buttonHistoryPath";
            this.buttonHistoryPath.Size = new System.Drawing.Size(75, 23);
            this.buttonHistoryPath.TabIndex = 2;
            this.buttonHistoryPath.Text = "Выбрать";
            this.buttonHistoryPath.UseVisualStyleBackColor = true;
            this.buttonHistoryPath.Click += new System.EventHandler(this.buttonHistoryPath_Click);
            // 
            // textBoxHistoryPath
            // 
            this.textBoxHistoryPath.Enabled = false;
            this.textBoxHistoryPath.Location = new System.Drawing.Point(329, 33);
            this.textBoxHistoryPath.Name = "textBoxHistoryPath";
            this.textBoxHistoryPath.Size = new System.Drawing.Size(345, 23);
            this.textBoxHistoryPath.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(6, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(275, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Папка для сохранения ползовательских историй";
            // 
            // tabPageRestore
            // 
            this.tabPageRestore.Controls.Add(this.ButtonSelectAllHistToRestore);
            this.tabPageRestore.Controls.Add(this.buttonRestoreHistoryOnRestoreTab);
            this.tabPageRestore.Controls.Add(this.groupConnectionOnRestoreTab);
            this.tabPageRestore.Controls.Add(this.groupBoxActionsOnRestoreTab);
            this.tabPageRestore.Controls.Add(this.groupBoxUsersHistoryInStoreOnRestoreTab);
            this.tabPageRestore.Controls.Add(this.groupBoxAppsInStoreOnRestoreTab);
            this.tabPageRestore.Location = new System.Drawing.Point(4, 24);
            this.tabPageRestore.Name = "tabPageRestore";
            this.tabPageRestore.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRestore.Size = new System.Drawing.Size(991, 473);
            this.tabPageRestore.TabIndex = 1;
            this.tabPageRestore.Text = "Восстановление";
            this.tabPageRestore.UseVisualStyleBackColor = true;
            this.tabPageRestore.Enter += new System.EventHandler(this.tabPageRestore_Enter);
            this.tabPageRestore.Leave += new System.EventHandler(this.tabPageRestore_Leave);
            // 
            // buttonRestoreHistoryOnRestoreTab
            // 
            this.buttonRestoreHistoryOnRestoreTab.Location = new System.Drawing.Point(701, 366);
            this.buttonRestoreHistoryOnRestoreTab.Name = "buttonRestoreHistoryOnRestoreTab";
            this.buttonRestoreHistoryOnRestoreTab.Size = new System.Drawing.Size(277, 65);
            this.buttonRestoreHistoryOnRestoreTab.TabIndex = 7;
            this.buttonRestoreHistoryOnRestoreTab.Text = "Восстановить истории";
            this.buttonRestoreHistoryOnRestoreTab.UseVisualStyleBackColor = true;
            this.buttonRestoreHistoryOnRestoreTab.Click += new System.EventHandler(this.buttonRestoreHistoryOnRestoreTab_Click);
            // 
            // groupConnectionOnRestoreTab
            // 
            this.groupConnectionOnRestoreTab.Controls.Add(this.groupBoxConnectionToRemoteServer);
            this.groupConnectionOnRestoreTab.Controls.Add(this.groupBoxConnectoionToLocalHostOnRestoreTab);
            this.groupConnectionOnRestoreTab.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupConnectionOnRestoreTab.Location = new System.Drawing.Point(3, 3);
            this.groupConnectionOnRestoreTab.Name = "groupConnectionOnRestoreTab";
            this.groupConnectionOnRestoreTab.Size = new System.Drawing.Size(985, 153);
            this.groupConnectionOnRestoreTab.TabIndex = 3;
            this.groupConnectionOnRestoreTab.TabStop = false;
            // 
            // groupBoxConnectionToRemoteServer
            // 
            this.groupBoxConnectionToRemoteServer.Controls.Add(this.label4);
            this.groupBoxConnectionToRemoteServer.Controls.Add(this.buttonConnectToServerOnRestoreTab);
            this.groupBoxConnectionToRemoteServer.Controls.Add(this.textBoxAddrServer);
            this.groupBoxConnectionToRemoteServer.Controls.Add(this.buttonDisconnectFromServerOnRestoreTab);
            this.groupBoxConnectionToRemoteServer.Location = new System.Drawing.Point(6, 22);
            this.groupBoxConnectionToRemoteServer.Name = "groupBoxConnectionToRemoteServer";
            this.groupBoxConnectionToRemoteServer.Size = new System.Drawing.Size(460, 123);
            this.groupBoxConnectionToRemoteServer.TabIndex = 5;
            this.groupBoxConnectionToRemoteServer.TabStop = false;
            this.groupBoxConnectionToRemoteServer.Text = "Подключение к серверу";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Адрес";
            // 
            // buttonConnectToServerOnRestoreTab
            // 
            this.buttonConnectToServerOnRestoreTab.Location = new System.Drawing.Point(316, 27);
            this.buttonConnectToServerOnRestoreTab.Name = "buttonConnectToServerOnRestoreTab";
            this.buttonConnectToServerOnRestoreTab.Size = new System.Drawing.Size(138, 23);
            this.buttonConnectToServerOnRestoreTab.TabIndex = 2;
            this.buttonConnectToServerOnRestoreTab.Text = "Подключение";
            this.buttonConnectToServerOnRestoreTab.UseVisualStyleBackColor = true;
            this.buttonConnectToServerOnRestoreTab.Click += new System.EventHandler(this.buttonConnectToServerOnRestoreTab_Click);
            // 
            // textBoxAddrServer
            // 
            this.textBoxAddrServer.Location = new System.Drawing.Point(52, 28);
            this.textBoxAddrServer.Name = "textBoxAddrServer";
            this.textBoxAddrServer.Size = new System.Drawing.Size(243, 23);
            this.textBoxAddrServer.TabIndex = 1;
            this.textBoxAddrServer.Text = "http://127.0.0.1:4848";
            // 
            // buttonDisconnectFromServerOnRestoreTab
            // 
            this.buttonDisconnectFromServerOnRestoreTab.Location = new System.Drawing.Point(316, 56);
            this.buttonDisconnectFromServerOnRestoreTab.Name = "buttonDisconnectFromServerOnRestoreTab";
            this.buttonDisconnectFromServerOnRestoreTab.Size = new System.Drawing.Size(138, 23);
            this.buttonDisconnectFromServerOnRestoreTab.TabIndex = 3;
            this.buttonDisconnectFromServerOnRestoreTab.Text = "Отключение";
            this.buttonDisconnectFromServerOnRestoreTab.UseVisualStyleBackColor = true;
            this.buttonDisconnectFromServerOnRestoreTab.Click += new System.EventHandler(this.buttonDisconnectFromServerOnRestoreTab_Click);
            // 
            // groupBoxConnectoionToLocalHostOnRestoreTab
            // 
            this.groupBoxConnectoionToLocalHostOnRestoreTab.Controls.Add(this.label5);
            this.groupBoxConnectoionToLocalHostOnRestoreTab.Controls.Add(this.buttonConnectionToLocalHostOnRestoreTab);
            this.groupBoxConnectoionToLocalHostOnRestoreTab.Controls.Add(this.textBoxAdressLocalHostOnRestoreTab);
            this.groupBoxConnectoionToLocalHostOnRestoreTab.Controls.Add(this.buttonDisconnectFromLocalHostOnRestoreTab);
            this.groupBoxConnectoionToLocalHostOnRestoreTab.Location = new System.Drawing.Point(511, 22);
            this.groupBoxConnectoionToLocalHostOnRestoreTab.Name = "groupBoxConnectoionToLocalHostOnRestoreTab";
            this.groupBoxConnectoionToLocalHostOnRestoreTab.Size = new System.Drawing.Size(460, 116);
            this.groupBoxConnectoionToLocalHostOnRestoreTab.TabIndex = 4;
            this.groupBoxConnectoionToLocalHostOnRestoreTab.TabStop = false;
            this.groupBoxConnectoionToLocalHostOnRestoreTab.Text = "Подключение к локальному  компьютеру";
            this.groupBoxConnectoionToLocalHostOnRestoreTab.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Адрес";
            // 
            // buttonConnectionToLocalHostOnRestoreTab
            // 
            this.buttonConnectionToLocalHostOnRestoreTab.Location = new System.Drawing.Point(308, 23);
            this.buttonConnectionToLocalHostOnRestoreTab.Name = "buttonConnectionToLocalHostOnRestoreTab";
            this.buttonConnectionToLocalHostOnRestoreTab.Size = new System.Drawing.Size(138, 23);
            this.buttonConnectionToLocalHostOnRestoreTab.TabIndex = 2;
            this.buttonConnectionToLocalHostOnRestoreTab.Text = "Подключение";
            this.buttonConnectionToLocalHostOnRestoreTab.UseVisualStyleBackColor = true;
            this.buttonConnectionToLocalHostOnRestoreTab.Click += new System.EventHandler(this.buttonConnectionToLocalHostOnRestoreTab_Click);
            // 
            // textBoxAdressLocalHostOnRestoreTab
            // 
            this.textBoxAdressLocalHostOnRestoreTab.Location = new System.Drawing.Point(79, 24);
            this.textBoxAdressLocalHostOnRestoreTab.Name = "textBoxAdressLocalHostOnRestoreTab";
            this.textBoxAdressLocalHostOnRestoreTab.Size = new System.Drawing.Size(223, 23);
            this.textBoxAdressLocalHostOnRestoreTab.TabIndex = 1;
            this.textBoxAdressLocalHostOnRestoreTab.Text = "http://127.0.0.1:4848";
            // 
            // buttonDisconnectFromLocalHostOnRestoreTab
            // 
            this.buttonDisconnectFromLocalHostOnRestoreTab.Location = new System.Drawing.Point(308, 56);
            this.buttonDisconnectFromLocalHostOnRestoreTab.Name = "buttonDisconnectFromLocalHostOnRestoreTab";
            this.buttonDisconnectFromLocalHostOnRestoreTab.Size = new System.Drawing.Size(138, 23);
            this.buttonDisconnectFromLocalHostOnRestoreTab.TabIndex = 3;
            this.buttonDisconnectFromLocalHostOnRestoreTab.Text = "Отключение";
            this.buttonDisconnectFromLocalHostOnRestoreTab.UseVisualStyleBackColor = true;
            this.buttonDisconnectFromLocalHostOnRestoreTab.Click += new System.EventHandler(this.buttonDisconnectFromLocalHostOnRestoreTab_Click);
            // 
            // groupBoxActionsOnRestoreTab
            // 
            this.groupBoxActionsOnRestoreTab.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxActionsOnRestoreTab.Location = new System.Drawing.Point(3, 434);
            this.groupBoxActionsOnRestoreTab.Name = "groupBoxActionsOnRestoreTab";
            this.groupBoxActionsOnRestoreTab.Size = new System.Drawing.Size(985, 36);
            this.groupBoxActionsOnRestoreTab.TabIndex = 2;
            this.groupBoxActionsOnRestoreTab.TabStop = false;
            // 
            // groupBoxUsersHistoryInStoreOnRestoreTab
            // 
            this.groupBoxUsersHistoryInStoreOnRestoreTab.Controls.Add(this.listBoxHistorysInStoreOnRestoreTab);
            this.groupBoxUsersHistoryInStoreOnRestoreTab.Location = new System.Drawing.Point(480, 160);
            this.groupBoxUsersHistoryInStoreOnRestoreTab.Name = "groupBoxUsersHistoryInStoreOnRestoreTab";
            this.groupBoxUsersHistoryInStoreOnRestoreTab.Size = new System.Drawing.Size(500, 200);
            this.groupBoxUsersHistoryInStoreOnRestoreTab.TabIndex = 1;
            this.groupBoxUsersHistoryInStoreOnRestoreTab.TabStop = false;
            this.groupBoxUsersHistoryInStoreOnRestoreTab.Text = "Истории";
            // 
            // listBoxHistorysInStoreOnRestoreTab
            // 
            this.listBoxHistorysInStoreOnRestoreTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxHistorysInStoreOnRestoreTab.FormattingEnabled = true;
            this.listBoxHistorysInStoreOnRestoreTab.ItemHeight = 15;
            this.listBoxHistorysInStoreOnRestoreTab.Location = new System.Drawing.Point(3, 19);
            this.listBoxHistorysInStoreOnRestoreTab.Name = "listBoxHistorysInStoreOnRestoreTab";
            this.listBoxHistorysInStoreOnRestoreTab.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxHistorysInStoreOnRestoreTab.Size = new System.Drawing.Size(494, 178);
            this.listBoxHistorysInStoreOnRestoreTab.TabIndex = 0;
            // 
            // groupBoxAppsInStoreOnRestoreTab
            // 
            this.groupBoxAppsInStoreOnRestoreTab.Controls.Add(this.listBoxAppsInStoreOnRestoreTab);
            this.groupBoxAppsInStoreOnRestoreTab.Location = new System.Drawing.Point(8, 160);
            this.groupBoxAppsInStoreOnRestoreTab.Name = "groupBoxAppsInStoreOnRestoreTab";
            this.groupBoxAppsInStoreOnRestoreTab.Size = new System.Drawing.Size(470, 200);
            this.groupBoxAppsInStoreOnRestoreTab.TabIndex = 0;
            this.groupBoxAppsInStoreOnRestoreTab.TabStop = false;
            this.groupBoxAppsInStoreOnRestoreTab.Text = "Приложения";
            // 
            // listBoxAppsInStoreOnRestoreTab
            // 
            this.listBoxAppsInStoreOnRestoreTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxAppsInStoreOnRestoreTab.FormattingEnabled = true;
            this.listBoxAppsInStoreOnRestoreTab.ItemHeight = 15;
            this.listBoxAppsInStoreOnRestoreTab.Location = new System.Drawing.Point(3, 19);
            this.listBoxAppsInStoreOnRestoreTab.Name = "listBoxAppsInStoreOnRestoreTab";
            this.listBoxAppsInStoreOnRestoreTab.Size = new System.Drawing.Size(464, 178);
            this.listBoxAppsInStoreOnRestoreTab.TabIndex = 0;
            this.listBoxAppsInStoreOnRestoreTab.SelectedIndexChanged += new System.EventHandler(this.listBoxAppsInStore_SelectedIndexChanged);
            // 
            // tabPageSave
            // 
            this.tabPageSave.Controls.Add(this.ButtonSelectAllHistToWrite);
            this.tabPageSave.Controls.Add(this.buttonSaveHistoryToLocalStore);
            this.tabPageSave.Controls.Add(this.groupBoxActionsForImportToLocalStore);
            this.tabPageSave.Controls.Add(this.groupBoxStorysFromDevHub);
            this.tabPageSave.Controls.Add(this.groupBoxAppsFromDevHub);
            this.tabPageSave.Controls.Add(this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab);
            this.tabPageSave.Location = new System.Drawing.Point(4, 24);
            this.tabPageSave.Name = "tabPageSave";
            this.tabPageSave.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSave.Size = new System.Drawing.Size(991, 473);
            this.tabPageSave.TabIndex = 0;
            this.tabPageSave.Text = "Сохранение";
            this.tabPageSave.UseVisualStyleBackColor = true;
            this.tabPageSave.Enter += new System.EventHandler(this.tabPageSave_Enter);
            this.tabPageSave.Leave += new System.EventHandler(this.tabPageImport_Leave);
            // 
            // buttonSaveHistoryToLocalStore
            // 
            this.buttonSaveHistoryToLocalStore.Location = new System.Drawing.Point(701, 366);
            this.buttonSaveHistoryToLocalStore.Name = "buttonSaveHistoryToLocalStore";
            this.buttonSaveHistoryToLocalStore.Size = new System.Drawing.Size(277, 65);
            this.buttonSaveHistoryToLocalStore.TabIndex = 4;
            this.buttonSaveHistoryToLocalStore.Text = "Сохранить истории";
            this.buttonSaveHistoryToLocalStore.UseVisualStyleBackColor = true;
            this.buttonSaveHistoryToLocalStore.Click += new System.EventHandler(this.SaveHistory_Click);
            // 
            // groupBoxActionsForImportToLocalStore
            // 
            this.groupBoxActionsForImportToLocalStore.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxActionsForImportToLocalStore.Location = new System.Drawing.Point(3, 433);
            this.groupBoxActionsForImportToLocalStore.Name = "groupBoxActionsForImportToLocalStore";
            this.groupBoxActionsForImportToLocalStore.Size = new System.Drawing.Size(985, 37);
            this.groupBoxActionsForImportToLocalStore.TabIndex = 4;
            this.groupBoxActionsForImportToLocalStore.TabStop = false;
            // 
            // groupBoxStorysFromDevHub
            // 
            this.groupBoxStorysFromDevHub.Controls.Add(this.listBoxStrorysFromDevHub);
            this.groupBoxStorysFromDevHub.Location = new System.Drawing.Point(480, 160);
            this.groupBoxStorysFromDevHub.Name = "groupBoxStorysFromDevHub";
            this.groupBoxStorysFromDevHub.Size = new System.Drawing.Size(500, 200);
            this.groupBoxStorysFromDevHub.TabIndex = 3;
            this.groupBoxStorysFromDevHub.TabStop = false;
            this.groupBoxStorysFromDevHub.Text = "Истории";
            // 
            // listBoxStrorysFromDevHub
            // 
            this.listBoxStrorysFromDevHub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxStrorysFromDevHub.FormattingEnabled = true;
            this.listBoxStrorysFromDevHub.ItemHeight = 15;
            this.listBoxStrorysFromDevHub.Location = new System.Drawing.Point(3, 19);
            this.listBoxStrorysFromDevHub.Name = "listBoxStrorysFromDevHub";
            this.listBoxStrorysFromDevHub.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxStrorysFromDevHub.Size = new System.Drawing.Size(494, 178);
            this.listBoxStrorysFromDevHub.TabIndex = 0;
            // 
            // groupBoxAppsFromDevHub
            // 
            this.groupBoxAppsFromDevHub.Controls.Add(this.ListBoxAppsFromDevHub);
            this.groupBoxAppsFromDevHub.Location = new System.Drawing.Point(8, 160);
            this.groupBoxAppsFromDevHub.Name = "groupBoxAppsFromDevHub";
            this.groupBoxAppsFromDevHub.Size = new System.Drawing.Size(470, 200);
            this.groupBoxAppsFromDevHub.TabIndex = 2;
            this.groupBoxAppsFromDevHub.TabStop = false;
            this.groupBoxAppsFromDevHub.Text = "Приложения";
            // 
            // ListBoxAppsFromDevHub
            // 
            this.ListBoxAppsFromDevHub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListBoxAppsFromDevHub.FormattingEnabled = true;
            this.ListBoxAppsFromDevHub.ItemHeight = 15;
            this.ListBoxAppsFromDevHub.Location = new System.Drawing.Point(3, 19);
            this.ListBoxAppsFromDevHub.Name = "ListBoxAppsFromDevHub";
            this.ListBoxAppsFromDevHub.Size = new System.Drawing.Size(464, 178);
            this.ListBoxAppsFromDevHub.TabIndex = 0;
            this.ListBoxAppsFromDevHub.SelectedIndexChanged += new System.EventHandler(this.ListBoxApps_SelectedIndexChanged);
            // 
            // groupBoxConnectForImportHistoryToLocalStoreOnSaveTab
            // 
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.Controls.Add(this.groupBoxConnectToServerOnSaveTab);
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.Controls.Add(this.groupBoxConnectToLocalCоmputer);
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.Location = new System.Drawing.Point(3, 3);
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.Name = "groupBoxConnectForImportHistoryToLocalStoreOnSaveTab";
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.Size = new System.Drawing.Size(985, 153);
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.TabIndex = 0;
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.TabStop = false;
            // 
            // groupBoxConnectToServerOnSaveTab
            // 
            this.groupBoxConnectToServerOnSaveTab.Controls.Add(this.label3);
            this.groupBoxConnectToServerOnSaveTab.Controls.Add(this.buttonConnectToServer);
            this.groupBoxConnectToServerOnSaveTab.Controls.Add(this.textBox4);
            this.groupBoxConnectToServerOnSaveTab.Controls.Add(this.buttonDisconnectFromServer);
            this.groupBoxConnectToServerOnSaveTab.Location = new System.Drawing.Point(6, 22);
            this.groupBoxConnectToServerOnSaveTab.Name = "groupBoxConnectToServerOnSaveTab";
            this.groupBoxConnectToServerOnSaveTab.Size = new System.Drawing.Size(460, 123);
            this.groupBoxConnectToServerOnSaveTab.TabIndex = 5;
            this.groupBoxConnectToServerOnSaveTab.TabStop = false;
            this.groupBoxConnectToServerOnSaveTab.Text = "Подключение к cерверу";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Адрес";
            // 
            // buttonConnectToServer
            // 
            this.buttonConnectToServer.Location = new System.Drawing.Point(316, 27);
            this.buttonConnectToServer.Name = "buttonConnectToServer";
            this.buttonConnectToServer.Size = new System.Drawing.Size(138, 23);
            this.buttonConnectToServer.TabIndex = 2;
            this.buttonConnectToServer.Text = "Подключение";
            this.buttonConnectToServer.UseVisualStyleBackColor = true;
            this.buttonConnectToServer.Click += new System.EventHandler(this.buttonConnectToServer_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(52, 28);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(243, 23);
            this.textBox4.TabIndex = 1;
            this.textBox4.Text = "http://127.0.0.1:4848";
            // 
            // buttonDisconnectFromServer
            // 
            this.buttonDisconnectFromServer.Location = new System.Drawing.Point(316, 56);
            this.buttonDisconnectFromServer.Name = "buttonDisconnectFromServer";
            this.buttonDisconnectFromServer.Size = new System.Drawing.Size(138, 23);
            this.buttonDisconnectFromServer.TabIndex = 3;
            this.buttonDisconnectFromServer.Text = "Отключение";
            this.buttonDisconnectFromServer.UseVisualStyleBackColor = true;
            this.buttonDisconnectFromServer.Click += new System.EventHandler(this.buttonDisconnectFromServer_Click);
            // 
            // groupBoxConnectToLocalCоmputer
            // 
            this.groupBoxConnectToLocalCоmputer.Controls.Add(this.label1);
            this.groupBoxConnectToLocalCоmputer.Controls.Add(this.buttonConnectToLocalHub);
            this.groupBoxConnectToLocalCоmputer.Controls.Add(this.textBox1);
            this.groupBoxConnectToLocalCоmputer.Controls.Add(this.buttonDisconnectFromLoacalHub);
            this.groupBoxConnectToLocalCоmputer.Location = new System.Drawing.Point(518, 22);
            this.groupBoxConnectToLocalCоmputer.Name = "groupBoxConnectToLocalCоmputer";
            this.groupBoxConnectToLocalCоmputer.Size = new System.Drawing.Size(460, 123);
            this.groupBoxConnectToLocalCоmputer.TabIndex = 4;
            this.groupBoxConnectToLocalCоmputer.TabStop = false;
            this.groupBoxConnectToLocalCоmputer.Text = "Подключение к локальному  компьютеру";
            this.groupBoxConnectToLocalCоmputer.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Адрес";
            // 
            // buttonConnectToLocalHub
            // 
            this.buttonConnectToLocalHub.Location = new System.Drawing.Point(308, 27);
            this.buttonConnectToLocalHub.Name = "buttonConnectToLocalHub";
            this.buttonConnectToLocalHub.Size = new System.Drawing.Size(138, 23);
            this.buttonConnectToLocalHub.TabIndex = 2;
            this.buttonConnectToLocalHub.Text = "Подключение";
            this.buttonConnectToLocalHub.UseVisualStyleBackColor = true;
            this.buttonConnectToLocalHub.Click += new System.EventHandler(this.ConnectToLocalEngineButtonClick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(79, 28);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(223, 23);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "http://127.0.0.1:4848";
            // 
            // buttonDisconnectFromLoacalHub
            // 
            this.buttonDisconnectFromLoacalHub.Location = new System.Drawing.Point(308, 56);
            this.buttonDisconnectFromLoacalHub.Name = "buttonDisconnectFromLoacalHub";
            this.buttonDisconnectFromLoacalHub.Size = new System.Drawing.Size(138, 23);
            this.buttonDisconnectFromLoacalHub.TabIndex = 3;
            this.buttonDisconnectFromLoacalHub.Text = "Отключение";
            this.buttonDisconnectFromLoacalHub.UseVisualStyleBackColor = true;
            this.buttonDisconnectFromLoacalHub.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageSave);
            this.tabControl1.Controls.Add(this.tabPageRestore);
            this.tabControl1.Controls.Add(this.tabPageAppConfiguration);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(999, 501);
            this.tabControl1.TabIndex = 3;
            // 
            // ButtonSelectAllHistToWrite
            // 
            this.ButtonSelectAllHistToWrite.Location = new System.Drawing.Point(483, 366);
            this.ButtonSelectAllHistToWrite.Name = "ButtonSelectAllHistToWrite";
            this.ButtonSelectAllHistToWrite.Size = new System.Drawing.Size(148, 24);
            this.ButtonSelectAllHistToWrite.TabIndex = 6;
            this.ButtonSelectAllHistToWrite.Text = "Выделить все истории";
            this.ButtonSelectAllHistToWrite.UseVisualStyleBackColor = true;
            this.ButtonSelectAllHistToWrite.Click += new System.EventHandler(this.ButtonSelectAllHistToWrite_Click);
            // 
            // ButtonSelectAllHistToRestore
            // 
            this.ButtonSelectAllHistToRestore.Location = new System.Drawing.Point(514, 366);
            this.ButtonSelectAllHistToRestore.Name = "ButtonSelectAllHistToRestore";
            this.ButtonSelectAllHistToRestore.Size = new System.Drawing.Size(148, 24);
            this.ButtonSelectAllHistToRestore.TabIndex = 9;
            this.ButtonSelectAllHistToRestore.Text = "Выделить все истории";
            this.ButtonSelectAllHistToRestore.UseVisualStyleBackColor = true;
            this.ButtonSelectAllHistToRestore.Click += new System.EventHandler(this.ButtonSelectAllHistToRestore_Click);
            // 
            // ManagerForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(999, 547);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManagerForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QSObjectManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManagerForm_FormClosing);
            this.Load += new System.EventHandler(this.ManagerForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabPageAppConfiguration.ResumeLayout(false);
            this.groupBoxOptionsPaths.ResumeLayout(false);
            this.groupBoxOptionsPaths.PerformLayout();
            this.tabPageRestore.ResumeLayout(false);
            this.groupConnectionOnRestoreTab.ResumeLayout(false);
            this.groupBoxConnectionToRemoteServer.ResumeLayout(false);
            this.groupBoxConnectionToRemoteServer.PerformLayout();
            this.groupBoxConnectoionToLocalHostOnRestoreTab.ResumeLayout(false);
            this.groupBoxConnectoionToLocalHostOnRestoreTab.PerformLayout();
            this.groupBoxUsersHistoryInStoreOnRestoreTab.ResumeLayout(false);
            this.groupBoxAppsInStoreOnRestoreTab.ResumeLayout(false);
            this.tabPageSave.ResumeLayout(false);
            this.groupBoxStorysFromDevHub.ResumeLayout(false);
            this.groupBoxAppsFromDevHub.ResumeLayout(false);
            this.groupBoxConnectForImportHistoryToLocalStoreOnSaveTab.ResumeLayout(false);
            this.groupBoxConnectToServerOnSaveTab.ResumeLayout(false);
            this.groupBoxConnectToServerOnSaveTab.PerformLayout();
            this.groupBoxConnectToLocalCоmputer.ResumeLayout(false);
            this.groupBoxConnectToLocalCоmputer.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutProgramToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogPathsHistoru;
        private System.Windows.Forms.TabPage tabPageAppConfiguration;
        private System.Windows.Forms.GroupBox groupBoxOptionsPaths;
        private System.Windows.Forms.Button buttonHistoryPath;
        private System.Windows.Forms.TextBox textBoxHistoryPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPageRestore;
        private System.Windows.Forms.Button buttonRestoreHistoryOnRestoreTab;
        private System.Windows.Forms.GroupBox groupConnectionOnRestoreTab;
        private System.Windows.Forms.GroupBox groupBoxConnectionToRemoteServer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonConnectToServerOnRestoreTab;
        private System.Windows.Forms.TextBox textBoxAddrServer;
        private System.Windows.Forms.Button buttonDisconnectFromServerOnRestoreTab;
        private System.Windows.Forms.GroupBox groupBoxConnectoionToLocalHostOnRestoreTab;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonConnectionToLocalHostOnRestoreTab;
        private System.Windows.Forms.TextBox textBoxAdressLocalHostOnRestoreTab;
        private System.Windows.Forms.Button buttonDisconnectFromLocalHostOnRestoreTab;
        private System.Windows.Forms.GroupBox groupBoxActionsOnRestoreTab;
        private System.Windows.Forms.GroupBox groupBoxUsersHistoryInStoreOnRestoreTab;
        private System.Windows.Forms.ListBox listBoxHistorysInStoreOnRestoreTab;
        private System.Windows.Forms.GroupBox groupBoxAppsInStoreOnRestoreTab;
        private System.Windows.Forms.ListBox listBoxAppsInStoreOnRestoreTab;
        private System.Windows.Forms.TabPage tabPageSave;
        private System.Windows.Forms.Button buttonSaveHistoryToLocalStore;
        private System.Windows.Forms.GroupBox groupBoxActionsForImportToLocalStore;
        private System.Windows.Forms.GroupBox groupBoxStorysFromDevHub;
        private System.Windows.Forms.ListBox listBoxStrorysFromDevHub;
        private System.Windows.Forms.GroupBox groupBoxAppsFromDevHub;
        private System.Windows.Forms.ListBox ListBoxAppsFromDevHub;
        private System.Windows.Forms.GroupBox groupBoxConnectForImportHistoryToLocalStoreOnSaveTab;
        private System.Windows.Forms.GroupBox groupBoxConnectToServerOnSaveTab;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonConnectToServer;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button buttonDisconnectFromServer;
        private System.Windows.Forms.GroupBox groupBoxConnectToLocalCоmputer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonConnectToLocalHub;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonDisconnectFromLoacalHub;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button ButtonSelectAllHistToRestore;
        private System.Windows.Forms.Button ButtonSelectAllHistToWrite;
    }
}

