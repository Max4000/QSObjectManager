using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ObjectsForWorkWithQSEngine.MainObjectsForWork;
using UtilClasses;

namespace QSUsersHistoryManager
{
    public partial class ManagerForm : Form
    {
        private LocationObject _locationObject;
        private IList<NameAndIdPair> _lstApp;
        private IList<NameAndIdPair> _storys;
        private int SelectedIpp { get; set; }
        private readonly IniFile _iniFile = new IniFile();

        private IList<NameAndIdPair> _lstAppsInStore;
        private IList<NameAndIdPair> _lstStorysInStore;
        private int _selectedIndexAppInStore;

        private bool _iSconnected;


        public ManagerForm()
        {
            InitializeComponent();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void SetVisibleLists(bool en)
        {
            groupBoxAppsFromDevHub.Visible = en;
            groupBoxStorysFromDevHub.Visible = en;
            groupBoxActionsForImportToLocalStore.Visible = en;
            groupBoxCommentsFromLocalStore.Visible = en;
            buttonSaveHistoryToLocalStore.Visible = en;

            buttonDisconnectFromLoacalHub.Visible = _iSconnected;
            buttonDisconnectFromServer.Visible = en;
        }

        private void ConnectToEngineButtonClick(object sender, EventArgs e)
        {
            try
            {
                _locationObject = new LocationObject(this.textBox1.Text);

                _lstApp = Utils.GetApps(_locationObject.LocationPersonalEdition);

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

            }
            catch (Exception)
            {
                //Console.WriteLine(exception);

                Form frm = new MessageForm("Проверьте условия подключения к Dev Hub");
                frm.ShowDialog();
                frm.Dispose();

            }
        }

        private void ManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_locationObject != null)
                _locationObject.Dispose();
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
            SelectedIpp = -1;
            _iSconnected = false;
            buttonConnectToLocalHub.Visible = true;
            this.buttonConnectToServer.Visible = true;
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

                _storys = Utils.GetStorys(_locationObject.LocationPersonalEdition, appPair.Id);

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
            if (!_iniFile.KeyExists("PathHistorysRoot", "Paths"))
            {
                _iniFile.Write("PathHistorysRoot", "", "Paths");
                this.textBoxHistoryPath.Text = "";
            }
            else
            {
                this.textBoxHistoryPath.Text = _iniFile.Read("PathHistorysRoot", "Paths");
            }


        }

        private void buttonHistoryPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialogPathsHistoru.SelectedPath = this.textBoxHistoryPath.Text;
            if (folderBrowserDialogPathsHistoru.ShowDialog() == DialogResult.OK)
            {
                this.textBoxHistoryPath.Text = folderBrowserDialogPathsHistoru.SelectedPath;
                _iniFile.Write("PathHistorysRoot", this.textBoxHistoryPath.Text, "Paths");
                
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
                    listStoryNames.Add(new NameAndIdPair(this._storys[ts].Name,this._storys[ts].Id));
                }
            }

            string pathToRootFolder = _iniFile.Read("PathHistorysRoot", "Paths");

            ImportUtilClass.ImportStorysFromAppQs(pathToRootFolder, _locationObject.LocationPersonalEdition,
                _lstApp[SelectedIpp], listStoryNames);


        }

        private void tabPage1_Leave(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            this._lstAppsInStore = StoreAppsInfoClass.GetAppsFromStore(_iniFile.Read("PathHistorysRoot", "Paths"));
            foreach (var item in _lstAppsInStore)
            {
                listBoxAppsInStoreOnRestoreTab.Items.Add(item);
            }

        }

        private void tabPage2_Leave(object sender, EventArgs e)
        {
            this._lstAppsInStore.Clear();
            listBoxAppsInStoreOnRestoreTab.Items.Clear();
            listBoxHistorysInStoreOnRestoreTab.Items.Clear();
            _selectedIndexAppInStore = -1;
        }

        private void listBoxAppsInStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxAppsInStoreOnRestoreTab.SelectedIndices.Count > 1)
            {
                new MessageForm("Есть возможность выбора не более одного приложения").ShowDialog();
                _selectedIndexAppInStore = -1;
                return;
            }

            _selectedIndexAppInStore = listBoxAppsInStoreOnRestoreTab.SelectedIndex;

            this._lstStorysInStore = StoreAppsInfoClass.GetHistoryListForSelectedApp(_iniFile.Read("PathHistorysRoot", "Paths"),new NameAndIdPair(
                _lstAppsInStore[_selectedIndexAppInStore].Name,
                _lstAppsInStore[_selectedIndexAppInStore].Id));

            this.listBoxHistorysInStoreOnRestoreTab.Items.Clear();

            foreach (var item in _lstStorysInStore)
            {
                this.listBoxHistorysInStoreOnRestoreTab.Items.Add(item);
            }

        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }
    }
}

