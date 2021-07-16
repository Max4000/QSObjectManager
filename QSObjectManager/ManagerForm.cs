using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ObjectsForWorkWithQSEngine.MainObjectsForWork;
using UtilClasses.ProgramOptionsClasses;

namespace QSObjectManager
{
    public partial class ManagerForm : Form, IProgramOptionsEvent, IWriteInfoEvent, IConnectionStatusInfoEvent, ISelectAppEvent
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

        private bool _iSconnected;

        public event NewProgramOptionsHandler NewProgramOptionsSend;
        public event NewWriterInfosHandler NewWriteInfoSend;
        public event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
        public event NewAppSelectedHandler NewAppSelectedSend;

        private bool _connectedToLocalServer;
        private bool _connectedToRemoteServer;

        public ManagerForm()
        {
            InitializeComponent();
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


        public void OnNewConnectioStatusInfo(ConnectionStatusInfoEventArgs e)
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
                _lstApp = Utils.GetApps(_locationObject.GetConnection());

                ListBoxAppsFromDevHub.Items.Clear();

                foreach (var app in _lstApp)
                {
                    ListBoxAppsFromDevHub.Items.Add(app);
                }


                SetVisibleLists(true);

                _iSconnected = true;

                buttonDisconnectFromLoacalHub.Visible = _iSconnected;
                buttonDisconnectFromServer.Visible = _iSconnected;
                buttonConnectToLocalHub.Visible = false;
                buttonConnectToServer.Visible = false;

                OnNewConnectioStatusInfo(new ConnectionStatusInfoEventArgs(new ConnectionStatusInfo(_locationObject)));
            }

        }

        private void ConnectToLocalEngineButtonClick(object sender, EventArgs e)
        {
            try
            {

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

            OnNewConnectioStatusInfo(new ConnectionStatusInfoEventArgs(new ConnectionStatusInfo(_locationObject)));

            SelectedIpp = -1;
            _iSconnected = false;
            
            buttonConnectToLocalHub.Visible = true;
            buttonConnectToServer.Visible = true;
            buttonDisconnectFromLoacalHub.Visible = _iSconnected;
            buttonDisconnectFromServer.Visible = _iSconnected;

            buttonDisconnectFromLocalHostOnRestoreTab.Visible = _iSconnected;
            buttonDisconnectFromServerOnRestoreTab.Visible = _iSconnected;
            
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

                _storys = Utils.GetStorys(_locationObject.GetConnection(), appPair.Id);

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

            _qsAppRestoreObject = new QsAppRestoreClass(this,this,this);
            
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
        }

        private void listBoxAppsInStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxAppsInStoreOnRestoreTab.SelectedIndices.Count > 1)
            {
                ShowMessageForm("Есть возможность выбора не более одного приложения","Ошибка");
                _selectedIndexAppInStore = -1;
                return;
            }

            _selectedIndexAppInStore = listBoxAppsInStoreOnRestoreTab.SelectedIndex;

            OnNewSelectedApp(new SelectedAppEventArgs(_lstAppsInStore[_selectedIndexAppInStore]));

            _lstStorysInStore = _qsAppRestoreObject.GetHistoryListForSelectedApp();
                

            listBoxHistorysInStoreOnRestoreTab.Items.Clear();

            foreach (var item in _lstStorysInStore)
            {
                listBoxHistorysInStoreOnRestoreTab.Items.Add(item);
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
                _locationObject = new RemoteConnection(textBox4.Text);
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

                _iSconnected = true;

                buttonDisconnectFromLocalHostOnRestoreTab.Visible = _iSconnected;
                buttonDisconnectFromServerOnRestoreTab.Visible = _iSconnected;
                buttonConnectionToLocalHostOnRestoreTab.Visible = false;
                buttonConnectToServerOnRestoreTab.Visible = false;

                buttonRestoreHistoryOnRestoreTab.Visible = _iSconnected;

                OnNewConnectioStatusInfo(new ConnectionStatusInfoEventArgs(new ConnectionStatusInfo(_locationObject)));
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
    }
}

