using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ObjectsForWorkWithQSEngine.MainObjectsForWork;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;
// ReSharper disable StringLiteralTypo

namespace QSObjectManager
{
    public partial class ManagerForm : Form, IProgramOptionsEvent, IWriteInfoEvent, IConnectionStatusInfoEvent, ISelectAppEvent,IRestoreInfoEvent
    {
        private IConnect _locationObject;
        private IList<NameAndIdAndLastReloadTime> _lstApp;
        private IList<NameAndIdAndLastReloadTime> _lstAppsFromHubOnRestoreTab;
        private IList<NameAndIdAndLastReloadTime> _stories;
        private int SelectedIpp { get; set; }
        

        private IniFileClass _iniFileObject;

        private IList<NameAndIdAndLastReloadTime> _lstAppsInStore;
        private IList<NameAndIdAndLastReloadTime> _lstStoriesInStore;
        private int _selectedIndexAppInStore;

        private ProgramOptions _programOptions;
        private QsAppRestoreClass _qsAppRestoreObject;

        private string _searchFileAppInStore;

        public QsAppWriterClass QsAppWriter { get; private set; }

        private bool _isConnected;

        public event ProgramOptionsHandler NewProgramOptionsSend;
        public event WriterInfosHandler NewWriteInfoSend;
        public event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
        public event AppSelectedHandler NewAppSelectedSend;
        public event RestoreInfoHandler NewRestoreInfoSend;

        private bool _connectedToLocalServer;
        private bool _connectedToRemoteServer;



        public ManagerForm()
        {
            InitializeComponent();
        }

        public void OnNewRestoreInfo(RestoreInfoEventArgs e)
        {
            if (NewRestoreInfoSend != null)
                NewRestoreInfoSend(this, e);
        }

        public void OnNewOptions(ProgramOptionsEventArgs e)
        {
            if (NewProgramOptionsSend != null)
                NewProgramOptionsSend(this, e);
        }

        public void OnNewSelectedApp(SelectedAppEventArgs e)
        {
            if (NewAppSelectedSend != null)
                NewAppSelectedSend(this, e);
        }


        public void OnNewConnectionStatusInfo(ConnectionStatusInfoEventArgs e)
        {
            if (NewConnectionStatusInfoSend != null)
                NewConnectionStatusInfoSend(this, e);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetVisibleLists(bool en)
        {
            groupBoxAppsFromDevHub.Visible = en;
            groupBoxStorysFromDevHub.Visible = en;
            groupBoxActionsForImportToLocalStore.Visible = en;
            
            buttonSaveHistoryToLocalStore.Visible = en;

            buttonDisconnectFromLoacalHub.Visible = en;
            buttonDisconnectFromServer.Visible = en;

            if (_connectedToLocalServer)
                groupBoxConnectToServerOnSaveTab.Visible = false;
            if (_connectedToRemoteServer)
                groupBoxConnectToLocalCоmputer.Visible = false;

            if (!_connectedToLocalServer)
                groupBoxConnectToServerOnSaveTab.Visible = true;
            if (!_connectedToRemoteServer)
                groupBoxConnectToLocalCоmputer.Visible = false;
        }

        private void buttonConnectToServer_Click(object sender, EventArgs e)
        {
            Thread.Sleep(3000);
            try
            {
                _programOptions.RemoteAddress = textBox4.Text;
                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));

                _locationObject = new RemoteConnection(textBox4.Text);
                _locationObject.Connect();
                _connectedToRemoteServer = true;

                //using (var hub = _locationObject.GetConnection().Hub())
                //{
                //    ShowMessageForm(hub.EngineVersion().ComponentVersion, ""); 
                //}
            }
            catch(Exception ex)
            {
                ShowMessageForm(ex.Message, "Ошибка");
                _connectedToRemoteServer = false;
                _locationObject = null;
            }
            UpdateForm();
        }

        private void UpdateForm()
        {
            if (_locationObject.IsConnected())
            {
                try
                {

                    _lstApp = Utils.GetApps(_locationObject.GetConnection());

                    ListBoxAppsFromDevHub.Items.Clear();

                    foreach (var app in _lstApp)
                    {
                        ListBoxAppsFromDevHub.Items.Add(app);
                    }


                    SetVisibleLists(true);

                    _isConnected = true;

                    buttonDisconnectFromLoacalHub.Visible = _isConnected;
                    buttonDisconnectFromServer.Visible = _isConnected;
                    buttonConnectToLocalHub.Visible = false;
                    buttonConnectToServer.Visible = false;
                    
                    ButtonSelectAllHistToWrite.Visible = true;
                    ButtonSelectAllHistToRestore.Visible = true;
                    

                    OnNewConnectionStatusInfo(
                        new ConnectionStatusInfoEventArgs(new ConnectionStatusInfo(_locationObject)));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null) ShowMessageForm(ex.InnerException.Message, "Ошибка");
                    _locationObject.Disconnect();
                }
            }

        }

        private void ConnectToLocalEngineButtonClick(object sender, EventArgs e)
        {
            try
            {
            

                _programOptions.LocalAddress = textBox1.Text;

                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));

                _locationObject = new LocalConnection(textBox1.Text);
                _locationObject.Connect();

                _connectedToLocalServer = true;

            }
            catch (Exception)
            {
            
                ShowMessageForm("Проверьте условия подключения к Dev Hub","Ошибка");
                _connectedToLocalServer = false;
                _locationObject = null;

            }

            

            UpdateForm();
        }

        private void ManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_locationObject != null)
                _locationObject.Dispose();
        }

        private void ShowMessageForm(string message, string caption)
        {
            
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);
            
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            if (ListBoxAppsFromDevHub != null) ListBoxAppsFromDevHub.Items.Clear();
            if (listBoxStrorysFromDevHub != null) listBoxStrorysFromDevHub.Items.Clear();

            _connectedToLocalServer = false;
            _connectedToRemoteServer = false;

            SetVisibleLists(false);

            if (_locationObject != null)
                _locationObject.Dispose();

            _locationObject = null;

            OnNewConnectionStatusInfo(new ConnectionStatusInfoEventArgs(new ConnectionStatusInfo(_locationObject)));

            SelectedIpp = -1;
            _isConnected = false;
            
            buttonConnectToLocalHub.Visible = true;
            buttonConnectToServer.Visible = true;
            buttonDisconnectFromLoacalHub.Visible = _isConnected;
            buttonDisconnectFromServer.Visible = _isConnected;

            buttonDisconnectFromLocalHostOnRestoreTab.Visible = _isConnected;
            buttonDisconnectFromServerOnRestoreTab.Visible = _isConnected;
            
            buttonConnectionToLocalHostOnRestoreTab.Visible = true;
            buttonConnectToServerOnRestoreTab.Visible = true;
            
            groupBoxConnectoionToLocalHostOnRestoreTab.Visible = false;

            groupBoxConnectionToRemoteServer.Visible = true;

            buttonRestoreHistoryOnRestoreTab.Visible = false;

            
            ButtonSelectAllHistToRestore.Visible = false;

            ButtonSelectAllHistToWrite.Visible = false;
            groupBoxAppsFromHubOnRestoreTab.Visible = false;

            SetVisibleGroup(false);


        }

        private void ListBoxApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_locationObject == null)
                return;
            try
            {
                listBoxStrorysFromDevHub.Items.Clear();

                NameAndIdAndLastReloadTime appAndLastReloadTime = _lstApp[ListBoxAppsFromDevHub.SelectedIndex];
                SelectedIpp = ListBoxAppsFromDevHub.SelectedIndex;

                _stories = Utils.GetStories(_locationObject.GetConnection(), appAndLastReloadTime.Id);

                foreach (var story in _stories)
                {
                    listBoxStrorysFromDevHub.Items.Add(story);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                
            }
            

        }

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            SetVisibleLists(false);
            
            _iniFileObject = new IniFileClass(this);

            _programOptions = _iniFileObject.GetOptions();

            textBoxHistoryPath.Text = _programOptions.RepositoryPath;
            textBox1.Text = _programOptions.LocalAddress;
            textBox4.Text = _programOptions.RemoteAddress;
            textBoxContentPath.Text = _programOptions.AppContentPath;
            textBoxDefaultPath.Text = _programOptions.ContentDefault;

            checkBoxOverwriteImages.Checked = _programOptions.OverwriteExistingContentImages;

            _qsAppRestoreObject = new QsAppRestoreClass(this,this,this,this);

            _qsAppRestoreObject.NewResultDigestInfoSend += ResultDigestRestoreInfoHandler;
            
            QsAppWriter = new QsAppWriterClass(this, this,this);

            checkBoxOverwriteImages.Checked = false;
            _programOptions.OverwriteExistingContentImages = false;

            OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));


            ButtonSelectAllHistToRestore.Visible = false;

            ButtonSelectAllHistToWrite.Visible = false;

            SetVisibleGroup(false);

            if (UtilClasses.CurrentRole.IsAdministrator())
            {
                // ReSharper disable once StringLiteralTypo
                this.Text += @" (Администратор)";
                checkBoxOverwriteImages.Enabled = true;
                buttonOpenContentSource.Enabled = true;
                buttonOpenContentTarget.Enabled = true;
            }
            else
            {
                checkBoxOverwriteImages.Enabled = false;
                buttonOpenContentSource.Enabled = false;
                buttonOpenContentTarget.Enabled = false;
            }

            

        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private void ResultDigestRestoreInfoHandler(object sender, ResultDigestInfoEventArgs e)
        {
            bool HaveDefault()
            {
                foreach (var item in e.ResultInfo.AddContentList)
                {
                    if (item.Contains("default\\"))
                        return true;
                }

                return false;
            }

            bool HaveAppcontent()
            {
                int count = 0;
                foreach (var item in e.ResultInfo.AddContentList)
                {
                    if (!item.Contains("default\\"))
                        count ++;
                }

                return count > 0;
            }


            if (e.ResultInfo.AddContentList.Count > 0)
            {
                string result = "При восстановлении обнаружен добавленный контент:\n\n";

                if (HaveDefault())
                {
                    result += "В папке Content\\Default:\n\n";
                    //result += "\n\n";
                    foreach (var item in e.ResultInfo.AddContentList)
                    {
                        if (item.Contains("default\\"))
                        {
                            int pos2 = item.LastIndexOf("\\", StringComparison.Ordinal);

                            result += item.Substring(pos2 + 1) + " уже скопирован" + ",\n";
                        }
                    }
                }

                if (HaveAppcontent())
                {
                    result += "\n\n";

                    //result += "В папке AppContent:\n\n";

                    foreach (var item in e.ResultInfo.AddContentList)
                    {
                        if (!item.Contains("default\\"))
                        {
                            result += item + ",\n";
                        }
                    }


                    result += "\n\n";

                    result += "Необходимо загрузить его из папки на вашем рабочем столе в приложение Qlik Sense\n\n";

                    int pos = e.ResultInfo.FolderWithAddContent.LastIndexOf("\\", StringComparison.Ordinal);

                    result += "Наименование папки: \n\n" + e.ResultInfo.FolderWithAddContent.Substring(pos + 1);
                    result += "\n\nИ еще раз сделать восстановление историй в приложение Qlik Sense";


                }
                ShowMessageForm(result, "Инфо");
            }
            else
            {
                Directory.Delete(e.ResultInfo.FolderWithAddContent);
                ShowMessageForm("Выбранные истории восстановлены ","Инфо");
            }
        
        }

        private void buttonHistoryPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialogPathsHistoru.SelectedPath = textBoxHistoryPath.Text;
            if (folderBrowserDialogPathsHistoru.ShowDialog() == DialogResult.OK)
            {
                textBoxHistoryPath.Text = folderBrowserDialogPathsHistoru.SelectedPath;
                _programOptions.RepositoryPath = textBoxHistoryPath.Text;

                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));
                
            }
        }

        private void SaveHistory_Click(object sender, EventArgs e)
        {


            if (!(_locationObject != null && listBoxStrorysFromDevHub.SelectedIndices.Count > 0))
            {
                ShowMessageForm("Надо выбрать одно приложение и не менее одной истории", "Ошибка");
                return;
            }

            IList<NameAndIdAndLastReloadTime> listStoryNames = new List<NameAndIdAndLastReloadTime>();

            foreach (var item in listBoxStrorysFromDevHub.SelectedIndices)
            {
                var ts = item is int i ? i : -1;

                if (ts >= 0)
                {
                    listStoryNames.Add(new NameAndIdAndLastReloadTime(_stories[ts].Name,_stories[ts].Id,""));
                }
            }

            OnNewWriteInfo(new WriteInfoEventArgs(new WriteInfo(_lstApp[SelectedIpp].Copy(), listStoryNames)));

            ShowMessageForm("Выбранные истории сохранены","");
        }

        private void tabPageImport_Leave(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void tabPageRestore_Enter(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(_programOptions.RepositoryPath))
            {
                ShowMessageForm("Установите путь на содержимое историй приложений", "Ошибка");
                return;
            }
            if (string.IsNullOrEmpty(_programOptions.AppContentPath))
            {
                ShowMessageForm("Установите путь на папку с контентом приложения на сервере", "Ошибка");
                return;
            }

            if (string.IsNullOrEmpty(_programOptions.ContentDefault))
            {
                ShowMessageForm("Установите путь на папку с контентом Default на сервере", "Ошибка");
                return;
            }
            try
            {
                textBoxAdressLocalHostOnRestoreTab.Text = _programOptions.LocalAddress;
                textBoxAddrServer.Text = _programOptions.RemoteAddress;
                _lstAppsInStore = _qsAppRestoreObject.GetAppsFromStore();
            }
            catch
            {
                ShowMessageForm("Что то не так с каталогом - првоерьте его содержимое он должен содержать объекты или быть пустым", "Ошибка");
            }

            if (_lstAppsInStore != null)
                foreach (var item in _lstAppsInStore)
                {
                    listBoxAppsInStoreOnRestoreTab.Items.Add(item);
                }

        }

        void OnNewWriteInfo(WriteInfoEventArgs e)
        {
            if (NewWriteInfoSend != null)
                NewWriteInfoSend(this, e);
        }

        private void tabPageRestore_Leave(object sender, EventArgs e)
        {
            
            if (_lstAppsInStore != null) _lstAppsInStore.Clear();
            if (listBoxAppsInStoreOnRestoreTab != null) listBoxAppsInStoreOnRestoreTab.Items.Clear();
            if (listBoxHistorysInStoreOnRestoreTab != null) listBoxHistorysInStoreOnRestoreTab.Items.Clear();
            _selectedIndexAppInStore = -1;
            Disconnect();
        }

        private void listBoxAppsInStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(listBoxAppsInStoreOnRestoreTab.SelectedIndex >= 0))
                return;

            if (listBoxAppsInStoreOnRestoreTab.SelectedIndices.Count > 1)
            {
                ShowMessageForm("Есть возможность выбора не более одного приложения","Ошибка");
                _selectedIndexAppInStore = -1;
                return;
            }

            if (listBoxAppsInStoreOnRestoreTab.SelectedItem != null && !string.IsNullOrEmpty(listBoxAppsInStoreOnRestoreTab.SelectedItem.ToString())){

                _selectedIndexAppInStore = listBoxAppsInStoreOnRestoreTab.SelectedIndex;

                OnNewSelectedApp(new SelectedAppEventArgs(_lstAppsInStore[_selectedIndexAppInStore]));

                _lstStoriesInStore = _qsAppRestoreObject.GetHistoryListForSelectedApp();


                listBoxHistorysInStoreOnRestoreTab.Items.Clear();

                foreach (var item in _lstStoriesInStore)
                {
                    listBoxHistorysInStoreOnRestoreTab.Items.Add(item);
                }

                

            }
            if ((listBoxAppsFromHubOnRestoreTab.SelectedIndices.Count == 1) &&
                (listBoxAppsInStoreOnRestoreTab.SelectedIndices.Count == 1))
            {
                textBoxIdSource.Text = _lstAppsInStore[listBoxAppsInStoreOnRestoreTab.SelectedIndex].Id;
                textBoxIdTarget.Text = _lstAppsFromHubOnRestoreTab[listBoxAppsFromHubOnRestoreTab.SelectedIndex].Id;

                SetVisibleGroup(true);
            }
            else
            {
                SetVisibleGroup(false);
            }

            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_lstAppsInStore[listBoxAppsInStoreOnRestoreTab.SelectedIndex].Name);


            _searchFileAppInStore = _programOptions.RepositoryPath + "\\"+  Path.GetFileNameWithoutExtension(FindFiles.SearchFileAppInStore(_programOptions.RepositoryPath, mNameSelectedApp, "*.xml"))+ "\\appcontent";


        }

        private void buttonDisconnectFromServer_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private void buttonConnectionToLocalHostOnRestoreTab_Click(object sender, EventArgs e)
        {
            try
            {
                _programOptions.LocalAddress = textBoxAdressLocalHostOnRestoreTab.Text;

                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));

                _locationObject = new LocalConnection(textBoxAdressLocalHostOnRestoreTab.Text);
                _locationObject.Connect();

                _connectedToLocalServer = true;

            }
            catch (Exception)
            {

                ShowMessageForm("Проверьте условия подключения к Dev Hub", "Ошибка");
                _connectedToLocalServer = false;
                _locationObject = null;

            }

            UpdateFormOnRestoreTab();
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private void buttonConnectToServerOnRestoreTab_Click(object sender, EventArgs e)
        {
            try
            {
                Thread.Sleep(3000);
                _programOptions.RemoteAddress = textBoxAddrServer.Text;

                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));
                _locationObject = new RemoteConnection(textBoxAddrServer.Text);
                _locationObject.Connect();

                _lstAppsFromHubOnRestoreTab = Utils.GetApps(_locationObject.GetConnection());

                listBoxAppsFromHubOnRestoreTab.Items.Clear();

                foreach (var app in _lstAppsFromHubOnRestoreTab)
                {
                    listBoxAppsFromHubOnRestoreTab.Items.Add(app);
                }
                _connectedToRemoteServer = true;
                UpdateFormOnRestoreTab();
            }
            catch (Exception)
            {
                ShowMessageForm("Проверьте условия подключения к Dev Hub", "Ошибка");
                _connectedToRemoteServer = false;
                _locationObject = null;
            }
            
        }


        private void SetVisibleGroup(bool vis)
        {
            textBoxIdSource.Visible = vis;
            textBoxIdTarget.Visible = vis;
            labelidSource.Visible = vis;
            labelidTarget.Visible = vis;


            buttonOpenContentSource.Visible = vis;
            buttonOpenContentTarget.Visible = vis;
            checkBoxOverwriteImages.Visible = vis;
            //buttonCopyToClipBoard.Visible = vis;
        }

        private void UpdateFormOnRestoreTab()
        {
            if (_locationObject != null && _locationObject.IsConnected())
            {
                SetVisibleElementsOnRestoreTab();
                _isConnected = true;
                buttonDisconnectFromLocalHostOnRestoreTab.Visible = _isConnected;
                buttonDisconnectFromServerOnRestoreTab.Visible = _isConnected;
                buttonConnectionToLocalHostOnRestoreTab.Visible = false;
                buttonConnectToServerOnRestoreTab.Visible = false;
                buttonRestoreHistoryOnRestoreTab.Visible = _isConnected;
                groupBoxAppsFromHubOnRestoreTab.Visible = _isConnected;
                ButtonSelectAllHistToRestore.Visible = true;
                
                OnNewConnectionStatusInfo(new ConnectionStatusInfoEventArgs(new ConnectionStatusInfo(_locationObject)));
            }
            else
            {
                SetVisibleGroup(false);
            }
        }

        private void SetVisibleElementsOnRestoreTab()
        {
            groupBoxConnectionToRemoteServer.Visible = !_connectedToLocalServer;
            groupBoxConnectoionToLocalHostOnRestoreTab.Visible = !_connectedToRemoteServer;
        }

        private void buttonDisconnectFromLocalHostOnRestoreTab_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void buttonDisconnectFromServerOnRestoreTab_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private void buttonRestoreHistoryOnRestoreTab_Click(object sender, EventArgs e)
        {
            if (!(_locationObject != null && listBoxAppsFromHubOnRestoreTab.SelectedIndices.Count == 1 &&
                  listBoxAppsInStoreOnRestoreTab.SelectedIndices.Count == 1 && listBoxHistorysInStoreOnRestoreTab.SelectedIndices.Count >= 1))
            {
                ShowMessageForm("Надо выбрать одно приложение цель, одно приложение источник  и не менее одной истории", "Ошибка");
                return;
            }

            

            if (string.IsNullOrEmpty(_programOptions.AppContentPath))
            {
                ShowMessageForm("Установите путь на папку с контентом приложений на сервере", "Ошибка");
                return;
            }

            if (string.IsNullOrEmpty(_programOptions.ContentDefault))
            {
                ShowMessageForm("Установите путь на папку с контентом Default на сервере", "Ошибка");
                return;
            }


            if (string.IsNullOrEmpty(_programOptions.RepositoryPath))
            {
                ShowMessageForm("Установите путь на папку с содержимым историй приложений на сервере", "Ошибка");
                return;
            }

            IList<NameAndIdAndLastReloadTime> listStoryNames = new List<NameAndIdAndLastReloadTime>();

            foreach (var item in listBoxHistorysInStoreOnRestoreTab.SelectedIndices)
            {
                var ts = item is int i ? i : -1;

                if (ts >= 0)
                {
                    listStoryNames.Add(new NameAndIdAndLastReloadTime(_lstStoriesInStore[ts].Name, _lstStoriesInStore[ts].Id,""));
                }
            }

            try
            {
                OnNewRestoreInfo(new RestoreInfoEventArgs(
                    new RestoreInfo(_lstAppsInStore[_selectedIndexAppInStore].Copy(), listStoryNames,
                        _lstAppsFromHubOnRestoreTab[listBoxAppsFromHubOnRestoreTab.SelectedIndex].Copy())));

                //ShowMessageForm("Выбранные истории восстановлены", "");
               
            }
            catch (Exception ex)
            {
                ShowMessageForm(ex.Message, "Ошибка");
            }
        }

        private void AboutProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void tabPageSave_Enter(object sender, EventArgs e)
        {
            textBox1.Text = _programOptions.LocalAddress;
            textBox4.Text = _programOptions.RemoteAddress;
        }

        private void ButtonSelectAllHistToRestore_Click(object sender, EventArgs e)
        {
            listBoxHistorysInStoreOnRestoreTab.BeginUpdate();

            for (int i = 0; i < listBoxHistorysInStoreOnRestoreTab.Items.Count; i++)
            {
                listBoxHistorysInStoreOnRestoreTab.SetSelected(i, true);
            }

            listBoxHistorysInStoreOnRestoreTab.EndUpdate();
        }

        private void ButtonSelectAllHistToWrite_Click(object sender, EventArgs e)
        {
            listBoxStrorysFromDevHub.BeginUpdate();

            for (int i = 0; i < listBoxStrorysFromDevHub.Items.Count; i++)
            {
                listBoxStrorysFromDevHub.SetSelected(i, true);
            }

            listBoxStrorysFromDevHub.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialogPathsHistoru.SelectedPath = textBoxContentPath.Text;
            if (folderBrowserDialogPathsHistoru.ShowDialog() == DialogResult.OK)
            {
                textBoxContentPath.Text = folderBrowserDialogPathsHistoru.SelectedPath;
                _programOptions.AppContentPath = textBoxContentPath.Text;

                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));

            }
        }

        private void listBoxAppsFromHubOnRestoreTab_SelectedIndexChanged(object sender, EventArgs e)
        {
           
                if ((listBoxAppsFromHubOnRestoreTab.SelectedIndices.Count == 1) &&
                    (listBoxAppsInStoreOnRestoreTab.SelectedIndices.Count == 1))
                {
                    textBoxIdSource.Text = _lstAppsInStore[listBoxAppsInStoreOnRestoreTab.SelectedIndex].Id;
                    textBoxIdTarget.Text = _lstAppsFromHubOnRestoreTab[listBoxAppsFromHubOnRestoreTab.SelectedIndex].Id;

                    SetVisibleGroup(true);
                }
                else
                {
                    SetVisibleGroup(false);

                }
            
        }

        private void buttonOpenContentTarget_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\Windows\\Explorer.exe" ,_programOptions.AppContentPath + "\\AppContent\\" + textBoxIdTarget.Text);
        }

        private void buttonOpenContentSource_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\Windows\\Explorer.exe", _searchFileAppInStore);
        }

        private void tabPageRestore_DragEnter(object sender, DragEventArgs e)
        {
           
        }

        private void checkBoxOverwriteImages_CheckedChanged(object sender, EventArgs e)
        {
            _programOptions.OverwriteExistingContentImages = checkBoxOverwriteImages.Checked;

            OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));
        }

        private void buttonContentDefaultPathSelect_Click(object sender, EventArgs e)
        {
            folderBrowserDialogPathsHistoru.SelectedPath = textBoxDefaultPath.Text;
            if (folderBrowserDialogPathsHistoru.ShowDialog() == DialogResult.OK)
            {
                textBoxDefaultPath.Text = folderBrowserDialogPathsHistoru.SelectedPath;
                
                _programOptions.ContentDefault = textBoxDefaultPath.Text;

                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));

            }
        }
    }
}

