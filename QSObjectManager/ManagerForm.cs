using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ObjectsForWorkWithQSEngine.MainObjectsForWork;
using UtilClasses.ProgramOptionsClasses;

namespace QSObjectManager
{
    public partial class ManagerForm : Form, IProgramOptionsEvent, IWriteInfoEvent, IConnectionStatusInfoEvent, ISelectAppEvent,IRestoreInfoEvent
    {
        private IConnect _locationObject;
        private IList<NameAndIdPair> _lstApp;
        private IList<NameAndIdPair> _storys;
        private int SelectedIpp { get; set; }
        

        private IniFileClass _iniFileObject;

        private IList<NameAndIdPair> _lstAppsInStore;
        private IList<NameAndIdPair> _lstStorysInStore;
        private int _selectedIndexAppInStore;

        private ProgramOptions _programOptions;
        private QsAppRestoreClass _qsAppRestoreObject;

        public QsAppWriterClass QsAppWriter { get; private set; }

        private bool _isConnected;

        public event NewProgramOptionsHandler NewProgramOptionsSend;
        public event NewWriterInfosHandler NewWriteInfoSend;
        public event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
        public event NewAppSelectedHandler NewAppSelectedSend;
        public event NewRestoreInfoHandler NewRestoreInfoSend;

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
                groupBoxConnectToLocalCоmputer.Visible = true;
        }

        private void buttonConnectToServer_Click(object sender, EventArgs e)
        {
            try
            {
                _programOptions.RemoteAddress = textBox4.Text;
                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));

                _locationObject = new RemoteConnection(textBox4.Text);
                _locationObject.Connect();
                _connectedToRemoteServer = true;
            }
            catch(Exception)
            {
                ShowMessageForm("Проверьте условия подключения к Dev Hub", "Ошибка");
                _connectedToRemoteServer = false;
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

                    OnNewConnectionStatusInfo(
                        new ConnectionStatusInfoEventArgs(new ConnectionStatusInfo(_locationObject)));
                }
                catch (Exception)
                {
                    ShowMessageForm("Проверьте условия подключения к Dev Hub", "Ошибка");
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
            
            groupBoxConnectoionToLocalHostOnRestoreTab.Visible = true;

            groupBoxConnectionToRemoteServer.Visible = true;

            buttonRestoreHistoryOnRestoreTab.Visible = false;
        }

        private void ListBoxApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_locationObject == null)
                return;
            try
            {
                listBoxStrorysFromDevHub.Items.Clear();

                NameAndIdPair appPair = _lstApp[ListBoxAppsFromDevHub.SelectedIndex];
                SelectedIpp = ListBoxAppsFromDevHub.SelectedIndex;

                _storys = Utils.GetStories(_locationObject.GetConnection(), appPair.Id);

                foreach (var story in _storys)
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

            _qsAppRestoreObject = new QsAppRestoreClass(this,this,this,this);
            
            QsAppWriter = new QsAppWriterClass(this, this,this);

            OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));

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


            if (!(_locationObject !=null && listBoxStrorysFromDevHub.SelectedIndices.Count > 0))
                return;

            IList<NameAndIdPair> listStoryNames = new List<NameAndIdPair>();

            foreach (var item in listBoxStrorysFromDevHub.SelectedIndices)
            {
                var ts = item is int i ? i : -1;

                if (ts >= 0)
                {
                    listStoryNames.Add(new NameAndIdPair(_storys[ts].Name,_storys[ts].Id));
                }
            }

            OnNewWriteInfo(new WriteInfoEventArgs(new WriteInfo(_lstApp[SelectedIpp].Copy(), listStoryNames)));
        }

        private void tabPageImport_Leave(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void tabPageRestore_Enter(object sender, EventArgs e)
        {
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
            if (listBoxAppsInStoreOnRestoreTab.SelectedIndices.Count > 1)
            {
                ShowMessageForm("Есть возможность выбора не более одного приложения","Ошибка");
                _selectedIndexAppInStore = -1;
                return;
            }

            if (listBoxAppsInStoreOnRestoreTab.SelectedItem != null && !string.IsNullOrEmpty(listBoxAppsInStoreOnRestoreTab.SelectedItem.ToString())){

                _selectedIndexAppInStore = listBoxAppsInStoreOnRestoreTab.SelectedIndex;

                OnNewSelectedApp(new SelectedAppEventArgs(_lstAppsInStore[_selectedIndexAppInStore]));

                _lstStorysInStore = _qsAppRestoreObject.GetHistoryListForSelectedApp();


                listBoxHistorysInStoreOnRestoreTab.Items.Clear();

                foreach (var item in _lstStorysInStore)
                {
                    listBoxHistorysInStoreOnRestoreTab.Items.Add(item);
                }
            }

        }

        private void buttonDisconnectFromServer_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

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

            }

            UpdateFormOnRestoreTab();
        }

        private void buttonConnectToServerOnRestoreTab_Click(object sender, EventArgs e)
        {
            try
            {
                _programOptions.RemoteAddress = textBoxAddrServer.Text;

                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Copy()));
                _locationObject = new RemoteConnection(textBoxAddrServer.Text);
                _locationObject.Connect();
                _connectedToRemoteServer = true;
            }
            catch (Exception)
            {
                ShowMessageForm("Проверьте условия подключения к Dev Hub", "Ошибка");
                _connectedToRemoteServer = false;
            }
            UpdateFormOnRestoreTab();
        }

        private void UpdateFormOnRestoreTab()
        {
            if (_locationObject.IsConnected())
            {
                
                SetVisibleElementsOnRestoreTab();

                _isConnected = true;

                buttonDisconnectFromLocalHostOnRestoreTab.Visible = _isConnected;
                buttonDisconnectFromServerOnRestoreTab.Visible = _isConnected;
                buttonConnectionToLocalHostOnRestoreTab.Visible = false;
                buttonConnectToServerOnRestoreTab.Visible = false;

                buttonRestoreHistoryOnRestoreTab.Visible = _isConnected;

                OnNewConnectionStatusInfo(new ConnectionStatusInfoEventArgs(new ConnectionStatusInfo(_locationObject)));
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

        private void buttonRestoreHistoryOnRestoreTab_Click(object sender, EventArgs e)
        {
            if (!(_locationObject != null && listBoxHistorysInStoreOnRestoreTab.SelectedIndices.Count > 0))
                return;

            IList<NameAndIdPair> listStoryNames = new List<NameAndIdPair>();

            foreach (var item in listBoxHistorysInStoreOnRestoreTab.SelectedIndices)
            {
                var ts = item is int i ? i : -1;

                if (ts >= 0)
                {
                    listStoryNames.Add(new NameAndIdPair(_lstStorysInStore[ts].Name, _lstStorysInStore[ts].Id));
                }
            }

            OnNewRestoreInfo(new RestoreInfoEventArgs(new RestoreInfo(_lstAppsInStore[_selectedIndexAppInStore].Copy(), listStoryNames)));
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
    }
}

