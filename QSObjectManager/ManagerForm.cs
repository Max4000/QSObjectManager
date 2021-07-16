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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void SetVisibleLists(bool en)
        {
            groupBoxAppsFromDevHub.Visible = en;
            groupBoxStorysFromDevHub.Visible = en;
            groupBoxActionsForImportToLocalStore.Visible = en;
            
            buttonSaveHistoryToLocalStore.Visible = en;

            buttonDisconnectFromLoacalHub.Visible = _iSconnected;
            buttonDisconnectFromServer.Visible = en;
        }

        private void ConnectToEngineButtonClick(object sender, EventArgs e)
        {
            try
            {

                buttonConnectToLocalHub.Enabled = false;

                _locationObject = new LocalConnection(textBox1.Text);
                _locationObject.Connect();

                buttonConnectToLocalHub.Enabled = true;

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
            catch (Exception)
            {
                
                ShowMessageForm("Проверьте условия подключения к Dev Hub","Ошибка");

            }
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

            OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Clone()));

        }

        private void buttonHistoryPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialogPathsHistoru.SelectedPath = textBoxHistoryPath.Text;
            if (folderBrowserDialogPathsHistoru.ShowDialog() == DialogResult.OK)
            {
                textBoxHistoryPath.Text = folderBrowserDialogPathsHistoru.SelectedPath;
                _programOptions.RepositoryPath = textBoxHistoryPath.Text;

                OnNewOptions(new ProgramOptionsEventArgs(_programOptions.Clone()));
                
            }
        }

        private void textBoxHistoryPath_TextChanged(object sender, EventArgs e)
        {

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

        private void tabPage1_Leave(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void tabPage2_Enter(object sender, EventArgs e)
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

        private void tabPage2_Leave(object sender, EventArgs e)
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

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonDisconnectFromServerOnRestoreTab_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}

