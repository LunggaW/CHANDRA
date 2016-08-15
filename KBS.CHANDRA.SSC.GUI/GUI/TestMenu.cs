using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using iTextSharp.text;
using iTextSharp.text.pdf;
using KBS.CHANDRA.SSC.FUNCTION;
using KBS.CHANDRA.SSC.DATAMODEL;
using LinqToExcel;
using Microsoft.VisualBasic;
using Oracle.DataAccess.Types;
using Oracle.DataAccess.Client;
using System.Drawing.Printing;
using System.Net.Mime;
using KBS.CHANDRA.SSC.GUI;
using Microsoft.Office.Interop;
using NLog;
using Org.BouncyCastle.Asn1.Ocsp;
using Remotion.Collections;
using Font = System.Drawing.Font;
using Microsoft.Reporting.WinForms;

namespace KBS.CHANDRA.SSC.GUI
{
    public partial class TestMenu : Form
    {
        private String ConnectionStringLocal = ConfigurationManager.AppSettings["ConnectionStringLocal"];

        OracleConnection con;

        private const int SPACINGAFTER = 30;
        private int FONTSIZEMENUSALESSTRIP = 9;
        private int prevSiteIndex;
        private bool initFromSalesInput = false;
        private Int32 MinutesToLogout;
        private List<String> BrandCode = new List<String>();
        private List<String> BrandName = new List<String>();
        private Font fontMenuSalesStrip;
        private Timer timerIdle = new Timer();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //private int MinutesToLogout = Convert.ToInt32(ConfigurationManager.AppSettings["MinutesToLogout"]);
        private Item item = new Item();
        private MemoDiscountHeader memoHeader = new MemoDiscountHeader();
        private SSCFunction function = new SSCFunction();
        private Panel activePanel = new Panel();
        private DataTable DTBrandInProfile = new DataTable();
        private DataTable DTStockTakeReport = new DataTable();
        private DataTable DTStockTakeUpload = new DataTable();
        private DataTable DTBrandExcludeProfile = new DataTable();
        private DataTable DTSiteInProfile = new DataTable();
        private DataTable DTSiteExcludeProfile = new DataTable();
        private DataTable DTItem = new DataTable();
        private DataTable DTStockTakeCopy = new DataTable();
        private DataTable DTStockTakeOrigin = new DataTable();
        private DataTable DTSiteByProfile = new DataTable();
        private DataTable DTSalesInputValidasi = new DataTable();
        private DataTable DTMultipleSalesInputOri = new DataTable();
        private DataTable DTMultipleSalesInputCopy = new DataTable();
        private DataTable DTSalesInputSalesHistory = new DataTable();
        private DataTable DTStockDisplay = new DataTable();
        private DataTable DTMemoDiscountHeader = new DataTable();
        private DataTable DTMemoDiscountDetail = new DataTable();
        private DataTable DTUser = new DataTable();
        private DataTable DTMenuInProfile = new DataTable();
        private DataTable DTMenuExcludeProfile = new DataTable();
        private DataTable DTAllProfile = new DataTable();
        private DataTable DTPrintLabelSearch = new DataTable();
        private DataTable DTPrintLabel = new DataTable();
        private DataTable DTPrintLabelSTCK = new DataTable();
        private DataTable DTUploadPromo = new DataTable();
        private DataTable DTParameter = new DataTable();
        private DataTable DTDetailIvoice = new DataTable();
        private DataTable DTFakturPajak = new DataTable();
        private DataTable DTInvoiceDetail = new DataTable();

        private Item itemSalesHistory = new Item();
        private Item itemSalesDisplay = new Item();
        private decimal GrossAmount;
        private String ErrorString;
        private String StatusNota;
        private decimal NetAmount;
        private DataTable DTProfileUserManagement = new DataTable();
        private DataTable DTParameterType = new DataTable();
        private DataTable DTParameterTypePengusaha = new DataTable();
        private DataTable DTParameterTypePembeli = new DataTable();
        private User user = new User();
        private User.UserStatus status;
        private User userUpdateUserManagement = new User();
        private String FileName;
        private String FilePath;
        private EventHandler timerHandler;
        private string seqNumber = "0";
        private String InvoiceNo;
        private bool FakturPajakIsPreview;
        private bool InvoiceIsHistory;
        private String InvoiceReportType;
        Item itemMultipleSalesInput = new Item();
        //private Form CallerLoginForm;
        private ProgressBarForm progressBar;

        private SalesSearchFilter salesSearchFilter = new SalesSearchFilter();

        int TotalData;

        public TestMenu()
        {

            //CallerLoginForm = LoginForm;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;


            try
            {
                String ConnectionString = ConfigurationManager.AppSettings["ConnectionStringLocal"];
                OracleConnection con = new OracleConnection();
                con.ConnectionString = ConnectionString;
                con.Open();
                con.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Error Trying to connect to local Server", "Error Occured",
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                this.Shown += new EventHandler(MyForm_CloseOnStart);
            }



            //syncData();

            labelLastUpdated.Text = "Last Updated : " + function.lastUpdatedMV_ARTICLES();


            //function.changeSalesInputStatusFromReservedToCancelled();

            MinutesToLogout = function.GetMinutesToLogout();

            timerHandler = new EventHandler(LogOut);

            initialiseTimer();




            logger.Debug("Interval : " + timerIdle.Interval);
            timerIdle.Tick += timerHandler;
            timerIdle.Stop();
            timerIdle.Start();

            panelStartingScreen.Visible = false;
            panelDisplay.Visible = false;
            panelChangePassword.Visible = false;
            panelProfileManagement.Visible = false;
            panelSales.Visible = false;
            panelValidasi.Visible = false;
            panelUserManagement.Visible = false;
            panelSalesHistory.Visible = false;
            panelSalesSearch.Visible = false;
            panelPrintLabel.Visible = false;
            panelLogoutSetting.Visible = false;
            panelMultipleSalesInput.Visible = false;
            panelMemoDiscount.Visible = false;
            panelUploadPromo.Visible = false;
            panelGenerateLabelSTCK.Visible = false;
            panelUploadInventory.Visible = false;
            panelStockTakeScan.Visible = false;
            panelStockTakeReport.Visible = false;
            panelStockTakeUpload.Visible = false;
            panelPaymentProcess.Visible = false;
            panelProsesDetail.Visible = false;
            MoveHistoryPanel.Visible = false;
            panelPaymentProcessNew.Visible = false;
            InvoiceParamPanel.Visible = false;
            panelReportFakturPajak.Visible = false;
            groupBoxPrintInvoice.Visible = false;
            panelPrintInvoice.Visible = false;
            panelPrintPreview.Visible = false;
            panelSlipPembayaran.Visible = false;
            disableMenuStrip();


            //Application.AddMessageFilter(new FilterMess(this)); //Connect to FilterMess class
        }


        /// <summary>
        /// Handles the Load event of the TestMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TestMenu_Load(object sender, EventArgs e)
        {
            saveFileDialogMemoDiscount.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialogFakturPajak.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialogStockTake.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialogBiaya.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialogPembayaran.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialogBCATransfer.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialogBCATunai.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialogRaboBank.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialogInvoiceDetailSummary.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialogInvoiceDetailDetail.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";


            openFileDialogUploadPromo.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            fontMenuSalesStrip = new Font("Microsoft Sans Serif", FONTSIZEMENUSALESSTRIP);

            logger.Debug("Minutes To Logout : " + MinutesToLogout);
            buttonCreateProfile.Visible = false;
            textBoxProfileName.Visible = false;
            labeNewProfileName.Visible = false;
            labelContactUs.BackColor = Color.FromArgb(231, 230, 230);
            labelLastUpdated.BackColor = Color.FromArgb(231, 230, 230);
            labelCopyright.BackColor = Color.FromArgb(231, 230, 230);
            linkLabelKDS.BackColor = Color.FromArgb(231, 230, 230);
            rectangleShape2.BackColor = Color.FromArgb(231, 230, 230);
            rectangleShape2.FillColor = Color.FromArgb(231, 230, 230);

            LoginPanelInitialization();
            this.reportViewerFakturPajak.RefreshReport();
        }

        #region Menu Strip Button Event

        private void enableHeader()
        {
            labelName.Visible = true;
            labelSite.Visible = true;
            comboBoxSite.Visible = true;
            pictureBoxChandraLogoHeader.Visible = true;
        }

        private void disableHeader()
        {
            labelName.Visible = false;
            labelSite.Visible = false;
            comboBoxSite.Visible = false;
            pictureBoxChandraLogoHeader.Visible = false;
        }

        public void EnableMenuAndItems()
        {

            logger.Debug("function EnableMenuAndItems");
            enableHeader();

            initialiseTimer();

            item.StatusSales = 1;

            // TODO: This line of code loads data into the 'dataSetSALESINPUT.KDSSALESINPUTSSC' table. You can move, or remove it, as needed.

            labelName.Text = "Welcome" + GlobalVar.GlobalVarUsername;

            //buttonValidate.Enabled = false;

            #region Combo Box Site





            comboBoxSite.DataSource = DTSiteByProfile;
            comboBoxSite.DisplayMember = "SITENAME";
            comboBoxSite.ValueMember = "SITECODE";



            #endregion

            DisableUserManagementCRUD();

            #region Combo Box Brand SalesInput


            RefreshListBoxProfile1ProfileManagement();

            #endregion


            #region Validasi

            comboBoxValidasi.Items.Insert(0, SalesInput.ItemStatus.Sold);
            comboBoxValidasi.Items.Insert(1, SalesInput.ItemStatus.Cancelled);
            #endregion




            #region Initialize Menu Strip
            exitToolStripMenuItem.Visible = true;
            exitToolStripMenuItem.Enabled = true;


            salesInputToolStripMenuItem.Font = fontMenuSalesStrip;
            validasiToolStripMenuItem.Font = fontMenuSalesStrip;
            displayToolStripMenuItem.Font = fontMenuSalesStrip;
            validasiToolStripMenuItem.Font = fontMenuSalesStrip;
            userManagementToolStripMenuItem.Font = fontMenuSalesStrip;
            profileManagementToolStripMenuItem.Font = fontMenuSalesStrip;
            changePasswordToolStripMenuItem.Font = fontMenuSalesStrip;
            exitToolStripMenuItem.Font = fontMenuSalesStrip;
            salesHistoryToolStripMenuItem.Font = fontMenuSalesStrip;
            printLabelToolStripMenuItem.Font = fontMenuSalesStrip;
            settingLogoutTimerToolStripMenuItem.Font = fontMenuSalesStrip;
            multipleSalesInputToolStripMenuItem.Font = fontMenuSalesStrip;
            memoDiscountToolStripMenuItem.Font = fontMenuSalesStrip;
            uploadPromoToolStripMenuItem.Font = fontMenuSalesStrip;
            generateSTCKToolStripMenuItem.Font = fontMenuSalesStrip;
            uploadInventoryToolStripMenuItem.Font = fontMenuSalesStrip;
            scanToolStripMenuItem.Font = fontMenuSalesStrip;
            reportToolStripMenuItem.Font = fontMenuSalesStrip;
            uploadToolStripMenuItem.Font = fontMenuSalesStrip;
            stockTakeToolStripMenuItem.Font = fontMenuSalesStrip;
            scanToolStripMenuItem.Font = fontMenuSalesStrip;
            reportToolStripMenuItem.Font = fontMenuSalesStrip;
            uploadToolStripMenuItem.Font = fontMenuSalesStrip;
            administrationToolStripMenuItem.Font = fontMenuSalesStrip;
            reportInvoiceToolStripMenuItem.Font = fontMenuSalesStrip;
            invoiceToolStripMenuItem.Font = fontMenuSalesStrip;

            disableMenuStrip();

            changePasswordToolStripMenuItem.Enabled = true;
            changePasswordToolStripMenuItem.Visible = true;
            exitToolStripMenuItem.Enabled = true;
            exitToolStripMenuItem.Visible = true;


            #endregion


            //load method Profile Management
            labelProfile.Text = "Tidak ada profile yang dipilih";
            // TODO: This line of code loads data into the 'dataSetPROFILE.KDSPROFILESSC' table. You can move, or remove it, as needed.

            ////load method Sales Input
            //v_BRANDTableAdapter.Fill(dataSetBRAND.V_BRAND);
            //comboBoxBrand.DataSource = dataSetBRAND.Tables[0];
            //comboBoxBrand.ValueMember = "BRANDCODE";
            //comboBoxBrand.DisplayMember = "BRANDNAME";

            #region Sales Input Load
            //fill in SalesInput drop down


            //generate label name
            labelName.Text = "Welcome : " + GlobalVar.GlobalVarUsername;

            #region User Management
            try
            {
                DTUser = function.SelectAllUser();
            }
            catch (Exception e1)
            {
                ErrorString = e1.Message;
                MessageBox.Show(e1.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            logger.Debug("enable button validate");
            buttonValidate.Enabled = DTSalesInputValidasi.Rows.Count == 0 ? false : true;
            logger.Debug("finish enable button validate");

            logger.Debug("enable label and combobox");
            labelCurrentProfile.Visible = false;
            labelCurrentStatus.Visible = false;
            labelUserID.Visible = false;
            labelUserNameUserManagement.Visible = false;
            labelPasswordUserManagement.Visible = false;
            labelStatusUserManagement.Visible = false;
            labelProfileUserManagement.Visible = false;
            textBoxUserID.Visible = false;
            textBoxUserName.Visible = false;
            textBoxPassword.Visible = false;
            comboBoxStatus.Visible = false;
            comboBoxProfile.Visible = false;
            logger.Debug("finish enable label and combobox");

            #endregion

            logger.Debug("get GlobalVarSite");
            logger.Debug(comboBoxSite.SelectedValue.ToString());
            GlobalVar.GlobalVarSite = comboBoxSite.SelectedValue.ToString();
            logger.Debug(GlobalVar.GlobalVarSite);
            logger.Debug("end get GlobalVarSite");


            UpdateSiteDropDownListData();
            #endregion
            //System.Windows.Forms.MainMenu menuFileMenuSSC = new System.Windows.Forms.MainMenu();
            //this.Menu = menuFileMenuSSC;

            DataTable DT = new DataTable();

            try
            {
                logger.Debug("get GlobalVarSite");
                DT = function.SelectMenuByProfileID(GlobalVar.GlobalVarProfileID);
            }
            catch (Exception e1)
            {
                ErrorString = e1.Message;
                MessageBox.Show(e1.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //show or hide menu based on profile privilege
            foreach (DataRow row in DT.Rows) // Loop over the rows.
            {
                foreach (var data in row.ItemArray) // Loop over the items.
                {
                    switch (Convert.ToInt32(data))
                    {
                        case 0:
                            break;
                        case 1:
                            salesInputToolStripMenuItem.Visible = true;
                            salesInputToolStripMenuItem.Enabled = true;
                            break;
                        case 2:
                            validasiToolStripMenuItem.Visible = true;
                            validasiToolStripMenuItem.Enabled = true;
                            break;
                        case 3:
                            displayToolStripMenuItem.Visible = true;
                            displayToolStripMenuItem.Enabled = true;
                            break;
                        case 4:
                            userManagementToolStripMenuItem.Visible = true;
                            userManagementToolStripMenuItem.Enabled = true;
                            administrationToolStripMenuItem.Visible = true;
                            administrationToolStripMenuItem.Enabled = true;
                            break;
                        case 5:
                            profileManagementToolStripMenuItem.Visible = true;
                            profileManagementToolStripMenuItem.Enabled = true;
                            administrationToolStripMenuItem.Visible = true;
                            administrationToolStripMenuItem.Enabled = true;
                            break;
                        case 6:
                            break;
                        case 7:
                            salesHistoryToolStripMenuItem.Visible = true;
                            salesHistoryToolStripMenuItem.Enabled = true;
                            break;
                        case 8:
                            printLabelToolStripMenuItem.Visible = true;
                            printLabelToolStripMenuItem.Enabled = true;
                            break;
                        case 9:
                            settingLogoutTimerToolStripMenuItem.Visible = true;
                            settingLogoutTimerToolStripMenuItem.Enabled = true;
                            administrationToolStripMenuItem.Visible = true;
                            administrationToolStripMenuItem.Enabled = true;
                            break;
                        case 10:
                            multipleSalesInputToolStripMenuItem.Visible = true;
                            multipleSalesInputToolStripMenuItem.Enabled = true;
                            break;
                        case 11:
                            memoDiscountToolStripMenuItem.Visible = true;
                            memoDiscountToolStripMenuItem.Enabled = true;
                            break;
                        case 12:
                            uploadPromoToolStripMenuItem.Visible = true;
                            uploadPromoToolStripMenuItem.Enabled = true;
                            break;
                        case 13:
                            generateSTCKToolStripMenuItem.Visible = true;
                            generateSTCKToolStripMenuItem.Enabled = true;
                            break;
                        case 14:
                            uploadInventoryToolStripMenuItem.Visible = true;
                            uploadInventoryToolStripMenuItem.Enabled = true;
                            break;
                        case 15:
                            scanToolStripMenuItem.Visible = true;
                            scanToolStripMenuItem.Enabled = true;
                            stockTakeToolStripMenuItem.Visible = true;
                            stockTakeToolStripMenuItem.Enabled = true;
                            break;
                        case 16:
                            reportToolStripMenuItem.Visible = true;
                            reportToolStripMenuItem.Enabled = true;
                            stockTakeToolStripMenuItem.Visible = true;
                            stockTakeToolStripMenuItem.Enabled = true;
                            break;
                        case 17:
                            uploadToolStripMenuItem.Visible = true;
                            uploadToolStripMenuItem.Enabled = true;
                            stockTakeToolStripMenuItem.Visible = true;
                            stockTakeToolStripMenuItem.Enabled = true;
                            break;
                        case 18:
                            invoiceParameterToolStripMenuItem.Visible = true;
                            invoiceParameterToolStripMenuItem.Enabled = true;
                            paymentProcessToolStripMenuItem.Visible = true;
                            paymentProcessToolStripMenuItem.Enabled = true;
                            reportInvoiceToolStripMenuItem.Visible = true;
                            reportInvoiceToolStripMenuItem.Enabled = true;
                            invoiceToolStripMenuItem.Enabled = true;
                            invoiceToolStripMenuItem.Visible = true;
                            break;
                        default:
                            logger.Error("Menu ID more than 18 occured");
                            MessageBox.Show("Tolong hubungi Admin", "Unknown error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    //MenuItem myMenuItem = new MenuItem("&"+item+"");
                    //MenuItem myMenuItemNew = new MenuItem("&Open");
                    //menuFileMenuSSC.MenuItems.Add(myMenuItem);
                    //myMenuItem.MenuItems.Add(myMenuItemNew);
                }
            }
        }

        public void disableMenuStrip()
        {
            logger.Debug("disable Menu Strip");
            //hide all menu item
            salesInputToolStripMenuItem.Visible = false;
            validasiToolStripMenuItem.Visible = false;
            displayToolStripMenuItem.Visible = false;
            userManagementToolStripMenuItem.Visible = false;
            profileManagementToolStripMenuItem.Visible = false;
            changePasswordToolStripMenuItem.Visible = false;
            salesHistoryToolStripMenuItem.Visible = false;
            printLabelToolStripMenuItem.Visible = false;
            settingLogoutTimerToolStripMenuItem.Visible = false;
            multipleSalesInputToolStripMenuItem.Visible = false;
            memoDiscountToolStripMenuItem.Visible = false;
            uploadPromoToolStripMenuItem.Visible = false;
            generateSTCKToolStripMenuItem.Visible = false;
            uploadInventoryToolStripMenuItem.Visible = false;
            scanToolStripMenuItem.Visible = false;
            reportToolStripMenuItem.Visible = false;
            uploadToolStripMenuItem.Visible = false;
            exitToolStripMenuItem.Visible = false;
            stockTakeToolStripMenuItem.Visible = false;
            administrationToolStripMenuItem.Visible = false;
            invoiceParameterToolStripMenuItem.Visible = false;
            reportInvoiceToolStripMenuItem.Visible = false;
            paymentProcessToolStripMenuItem.Visible = false;
            invoiceToolStripMenuItem.Visible = false;


            //disable all menu item
            salesInputToolStripMenuItem.Enabled = false;
            validasiToolStripMenuItem.Enabled = false;
            displayToolStripMenuItem.Enabled = false;
            userManagementToolStripMenuItem.Enabled = false;
            profileManagementToolStripMenuItem.Enabled = false;
            changePasswordToolStripMenuItem.Enabled = false;
            salesHistoryToolStripMenuItem.Enabled = false;
            printLabelToolStripMenuItem.Enabled = false;
            settingLogoutTimerToolStripMenuItem.Enabled = false;
            multipleSalesInputToolStripMenuItem.Enabled = false;
            memoDiscountToolStripMenuItem.Enabled = false;
            uploadPromoToolStripMenuItem.Enabled = false;
            generateSTCKToolStripMenuItem.Enabled = false;
            uploadInventoryToolStripMenuItem.Enabled = false;
            scanToolStripMenuItem.Enabled = false;
            reportToolStripMenuItem.Enabled = false;
            uploadToolStripMenuItem.Enabled = false;
            exitToolStripMenuItem.Enabled = false;
            stockTakeToolStripMenuItem.Enabled = false;
            administrationToolStripMenuItem.Enabled = false;
            invoiceParameterToolStripMenuItem.Enabled = false;
            reportInvoiceToolStripMenuItem.Enabled = false;
            paymentProcessToolStripMenuItem.Enabled = false;
            invoiceToolStripMenuItem.Enabled = false;
        }


        #region Tool Strip Menu Item Click Event
        private void scanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelStockTakeScan;
            activePanel.Visible = true;
            textBoxInvNumStockTakeScan.Focus();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelStockTakeReport;
            activePanel.Visible = true;
            DTStockTakeCopy = new DataTable();
            DTStockTakeOrigin = new DataTable();
            dataGridStockTakeScan.DataSource = DTStockTakeCopy;
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelStockTakeUpload;
            activePanel.Visible = true;
        }

        private void uploadPromoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelUploadPromo;
            activePanel.Visible = true;
            comboBoxStatusUploadPromo.SelectedIndex = comboBoxStatusUploadPromo.FindStringExact("ALL");
        }

        private void settingLogoutTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelLogoutSetting;
            activePanel.Visible = true;
            textBoxMinutesToLogout.Text = function.GetMinutesToLogout().ToString();
            textBoxMinutesToLogout.Focus();
        }

        private void salesHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelSalesHistory;
            activePanel.Visible = true;
            textBoxBarcodeSalesHistory.Focus();
            ComboBoxBrandCodeSalesHistory.Text = "";
        }

        private void salesInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelSales;
            activePanel.Visible = true;
            textBoxBarcode.Focus();
            //pictureBox1.Visible = false;
        }

        private void validasiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelValidasi;
            activePanel.Visible = true;
            textBoxBarcodeValidasi.Focus();
            comboBoxBrandCodeValidasi.Text = "";
        }

        private void displayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelDisplay;
            activePanel.Visible = true;
            textBoxBarcodeDisplay.Focus();
            RefreshComboBoxBrandCodeStockDisplay();

        }

        private void userManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelUserManagement;
            activePanel.Visible = true;
            UpdateDataGridViewUser();
        }

        private void profileManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelProfileManagement;
            activePanel.Visible = true;
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelChangePassword;
            activePanel.Visible = true;
            panelChangePassword.Focus();
        }

        private void printLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelPrintLabel;
            activePanel.Visible = true;
            panelChangePassword.Focus();
        }

        private void multipleSalesInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelMultipleSalesInput;
            activePanel.Visible = true;
            panelMultipleSalesInput.Focus();
            textBoxBarcodeMultipleSalesInput.Focus();
        }

        private void generateSTCKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelGenerateLabelSTCK;
            activePanel.Visible = true;
            panelGenerateLabelSTCK.Focus();
        }

        private void uploadInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelUploadInventory;
            activePanel.Visible = true;
            panelUploadInventory.Focus();
        }

        private void reportinvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelReportFakturPajak;
            activePanel.Visible = true;
            SearchRefreshReport();
        }



        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult DR = MessageBox.Show("Apa anda yakin mau keluar ?", "Confirm Keluar",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (DR == DialogResult.Yes)
            {
                ClearDataTable();
                LoginPanelInitialization();
            }
        }

        #endregion
        #endregion

        #region User Management

        /// <summary>
        /// Updates the data grid view user.
        /// </summary>
        private void UpdateDataGridViewUser()
        {
            DTUser = function.SelectAllUser();
            dataGridViewUser.DataSource = DTUser;
        }


        /// <summary>
        /// Enables the user management textbox, label, and button based on whether its update or insert.
        /// </summary>
        /// <param name="isUpdateDelete">if set to <c>true</c> [is update delete].</param>
        private void enableUserManagementCRUD(bool isUpdateDelete)
        {
            labelCurrentProfile.Visible = true;
            labelCurrentStatus.Visible = true;
            labelUserID.Visible = true;
            labelUserNameUserManagement.Visible = true;
            labelPasswordUserManagement.Visible = true;
            labelStatusUserManagement.Visible = true;
            labelProfileUserManagement.Visible = true;
            textBoxUserID.Visible = true;
            textBoxUserName.Visible = true;
            textBoxPassword.Visible = true;
            comboBoxStatus.Visible = true;
            comboBoxProfile.Visible = true;

            try
            {
                DTProfileUserManagement = function.SelectAllProfile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            comboBoxProfile.DataSource = DTProfileUserManagement;
            comboBoxProfile.DisplayMember = "PROFILENAME";
            comboBoxProfile.ValueMember = "PROFILEID";


            comboBoxStatus.DataSource = Enum.GetValues(typeof(User.UserStatus));

            if (isUpdateDelete)
            {
                logger.Debug("Update Delete");
                buttonUpdateUserManagement.Visible = true;
                buttonInsertUserManagement.Visible = false;
                textBoxUserID.ReadOnly = true;
                buttonInsertNewUser.Visible = true;
            }
            else
            {
                logger.Debug("is not Update Delete");
                buttonUpdateUserManagement.Visible = false;
                buttonInsertUserManagement.Visible = true;
                buttonInsertNewUser.Visible = false;
                labelCurrentProfile.Visible = true;
                labelCurrentProfile.Text = "";
                labelCurrentStatus.Visible = true;
                labelCurrentStatus.Text = "";
                labelUserID.Visible = true;
                labelUserNameUserManagement.Visible = true;
                labelPasswordUserManagement.Visible = true;
                labelStatusUserManagement.Visible = true;
                labelProfileUserManagement.Visible = true;
                textBoxUserID.Visible = true;
                textBoxUserID.Text = "";
                textBoxUserName.Visible = true;
                textBoxUserName.Text = "";
                textBoxPassword.Visible = true;
                textBoxPassword.Text = "";
                comboBoxStatus.Visible = true;
                comboBoxProfile.Visible = true;
                textBoxUserID.ReadOnly = false;
            }


            try
            {
                DTProfileUserManagement = function.SelectAllProfile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);

            }

            comboBoxProfile.DataSource = DTProfileUserManagement;
            comboBoxProfile.DisplayMember = "PROFILENAME";
            comboBoxProfile.ValueMember = "PROFILEID";

        }


        /// <summary>
        /// Disables the user management label, textbox, and button.
        /// </summary>
        private void DisableUserManagementCRUD()
        {
            logger.Debug("function DisableUserManagementCRUD");
            labelCurrentProfile.Visible = false;
            labelCurrentStatus.Visible = false;
            labelUserID.Visible = false;
            labelUserNameUserManagement.Visible = false;
            labelPasswordUserManagement.Visible = false;
            labelStatusUserManagement.Visible = false;
            labelProfileUserManagement.Visible = false;
            textBoxUserID.Visible = false;
            textBoxUserName.Visible = false;
            textBoxPassword.Visible = false;
            comboBoxStatus.Visible = false;
            comboBoxProfile.Visible = false;
            buttonUpdateUserManagement.Visible = false;
            buttonInsertUserManagement.Visible = false;
            buttonInsertNewUser.Visible = true;
        }

        private void dataGridViewUser_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            int rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridViewUser.Rows[rowIndex];

            User user = new User();

            try
            {
                user = function.SelectUserByUserID(row.Cells[0].Value.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            textBoxUserID.Text = user.UserID;
            textBoxUserName.Text = user.Username;
            textBoxPassword.Text = user.Password;
            enableUserManagementCRUD(true);
            labelCurrentProfile.Text = "Current Profile is :  " + function.getProfileNameByProfileID(user.ProfileID);
            labelCurrentStatus.Text = "Current Status is :  " + user.Status;
            comboBoxProfile.SelectedIndex = comboBoxProfile.FindStringExact(function.getProfileNameByProfileID(user.ProfileID));
            comboBoxStatus.Text = user.Status.ToString();
        }

        private void buttonUpdateUserManagement_Click(object sender, EventArgs e)
        {
            try
            {
                User userUpdate = new User();

                //set the combobox option to UserStatus enum
                Enum.TryParse<User.UserStatus>(comboBoxStatus.SelectedValue.ToString(), out status);

                userUpdate.UserID = textBoxUserID.Text;
                userUpdate.Password = textBoxPassword.Text;
                userUpdate.ProfileID = comboBoxProfile.SelectedValue.ToString();
                userUpdate.Status = status;
                userUpdate.Username = textBoxUserName.Text;


                function.updateUser(userUpdate);




                MessageBox.Show("User dengan UserID : " + userUpdate.UserID + " Berhasil di update", "User Berhasil di-Update",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisableUserManagementCRUD();
                UpdateDataGridViewUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);

            }
        }

        private void buttonDeleteUserManagement_Click(object sender, EventArgs e)
        {
            if (dataGridViewUser.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row", "Tidak ada row yang terpilih",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult result;
                result = MessageBox.Show("Apakah anda yakin ingin me-delete user-user yang di highlight di atas ?", "Apakah anda yakin",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in this.dataGridViewUser.SelectedRows)
                    {
                        try
                        {
                            function.DeleteUserByUserID(row.Cells["USERID"].Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error Occured",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            logger.Error(ex.Message);
                        }

                        MessageBox.Show("User dengan User ID : " + row.Cells["USERID"].Value.ToString() + " Berhasil di delete", "Delete User Berhasil",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    UpdateDataGridViewUser();
                }
            }

        }


        private void buttonUpdateUserUserManagement_Click(object sender, EventArgs e)
        {
            if (dataGridViewUser.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row", "Tidak ada row yang terpilih",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else if (dataGridViewUser.SelectedRows.Count > 1)
            {
                MessageBox.Show("Row terpilih lebih dari 1, tolong pilih satu row saja", "Row terpilih lebih dari 1",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                textBoxUserID.Focus();
                enableUserManagementCRUD(true);
                DataGridViewRow row = dataGridViewUser.SelectedRows[0];

                User user = new User();

                try
                {
                    user = function.SelectUserByUserID(row.Cells["USERID"].Value.ToString());
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "Error Occured",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }



                textBoxUserID.Text = user.UserID;
                textBoxUserName.Text = user.Username;
                textBoxPassword.Text = user.Password;
                enableUserManagementCRUD(true);
                labelCurrentProfile.Text = "Current Profile is :  " + function.getProfileNameByProfileID(user.ProfileID);
                labelCurrentStatus.Text = "Current Status is :  " + user.Status;
                comboBoxProfile.SelectedIndex = comboBoxProfile.FindStringExact(function.getProfileNameByProfileID(user.ProfileID));
                comboBoxStatus.Text = user.Status.ToString();


            }
        }

        private void buttonInsertNewUser_Click(object sender, EventArgs e)
        {
            enableUserManagementCRUD(false);
            textBoxUserID.Focus();
        }

        private void buttonInsertUserManagement_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxUserID.Text))
            {
                MessageBox.Show("User ID kosong, tolong diisi terlebih dahulu", "User ID kosong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (String.IsNullOrWhiteSpace(textBoxUserName.Text))
            {
                MessageBox.Show("UserName kosong, tolong diisi terlebih dahulu", "User Name kosong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (String.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                MessageBox.Show("Password kosong, tolong diisi terlebih dahulu", "Password kosong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                String userIDReturn = null;
                //find a duplicate user ID
                try
                {
                    userIDReturn = function.SelectDuplicateUserID(textBoxUserID.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }


                if (!String.IsNullOrWhiteSpace(userIDReturn))
                {
                    MessageBox.Show("UserID yang sama sudah ada di database", "Duplikat User ID",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxUserID.Text = "";
                }
                else
                {
                    User userInsert = new User();
                    Enum.TryParse<User.UserStatus>(comboBoxStatus.SelectedValue.ToString(), out status);
                    userInsert.UserID = textBoxUserID.Text;
                    userInsert.Password = textBoxPassword.Text;
                    userInsert.ProfileID = comboBoxProfile.SelectedValue.ToString();
                    userInsert.Status = status;
                    userInsert.Username = textBoxUserName.Text;

                    try
                    {
                        function.insertUser(userInsert);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }

                    DisableUserManagementCRUD();
                    UpdateDataGridViewUser();
                    MessageBox.Show("User dengan user id : " + userInsert.UserID + " berhasil di Insert", "User berhasil di Update",
                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBoxUserID.Text = "";
                }

            }
        }

        #endregion

        #region Profile Management

        private void buttonSelectAllAvailableBrand_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBoxBrand.Items.Count; i++)
            {
                listBoxBrand.SetSelected(i, true);
            }
        }

        private void buttonSelectAllBrandInProfile_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBoxProfileBrand.Items.Count; i++)
            {
                listBoxProfileBrand.SetSelected(i, true);
            }
        }

        private void buttonCreateProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxProfileName.Text))
            {
                MessageBox.Show("Profile Name Kosong, tolong periksa kembali", "Profile Name Kosong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (String.IsNullOrWhiteSpace(function.GetDuplicateProfileName(textBoxProfileName.Text)))
                {
                    try
                    {
                        function.insertProfile(textBoxProfileName.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Occured",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }

                    buttonNewProfile.Visible = true;
                    buttonCreateProfile.Visible = false;
                    textBoxProfileName.Visible = false;
                    labeNewProfileName.Visible = false;
                    RefreshListBoxProfile1ProfileManagement();
                    //this.kDSPROFILESSCTableAdapter.Fill(this.dataSetPROFILE.KDSPROFILESSC);
                }
                else
                {
                    MessageBox.Show("Profile Name Sudah ada, tolong gunakan nama yang lain", "Duplicate Profile Name",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
        }

        private void buttonNewProfile_Click(object sender, EventArgs e)
        {
            buttonNewProfile.Visible = false;
            textBoxProfileName.Visible = true;
            labeNewProfileName.Visible = true;
            buttonCreateProfile.Visible = true;
            textBoxProfileName.Focus();
        }

        private void tabControlProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxProfile1.SelectedItem == null)
            {
                MessageBox.Show("Tolong pilih salah satu profile", "Profile harus dipilih",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                tabControlProfile.SelectedTab = tabPageProfile;
            }
            else
            {
                labelProfile.Text = "Showing : " + listBoxProfile1.GetItemText(listBoxProfile1.SelectedItem);
                if (tabControlProfile.SelectedTab == tabPageBrand)//your specific tabname
                {
                    FillListBoxBrands();
                }
                else if (tabControlProfile.SelectedTab == tabPageSite)
                {
                    FillListBoxSites();
                }
                else if (tabControlProfile.SelectedTab == tabPageMenu)
                {
                    FillListBoxMenus();
                }

            }
        }

        private void buttonAssignBrand_Click(object sender, EventArgs e)
        {
            foreach (DataRowView row in listBoxBrand.SelectedItems)
            {
                try
                {
                    function.insertBrandByProfileID(row["BRANDCODE"].ToString(), listBoxProfile1.SelectedValue.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

            }

            UpdateSiteDropDownListData();

            tabControlProfile.SelectedTab = tabPageProfile;
            tabControlProfile.SelectedTab = tabPageBrand;
        }

        private void buttonRemoveBrand_Click(object sender, EventArgs e)
        {
            foreach (DataRowView row in listBoxProfileBrand.SelectedItems)
            {
                try
                {
                    function.DeleteBrandByProfileID(row["BRANDCODE"].ToString(), listBoxProfile1.SelectedValue.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

            }

            tabControlProfile.SelectedTab = tabPageProfile;
            tabControlProfile.SelectedTab = tabPageBrand;
        }

        private void buttonAssignSite_Click(object sender, EventArgs e)
        {
            foreach (DataRowView row in listBoxSite.SelectedItems)
            {
                try
                {
                    function.insertSiteByProfileID(row["SITECODE"].ToString(), listBoxProfile1.SelectedValue.ToString());
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "Error Occured",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

            }

            tabControlProfile.SelectedTab = tabPageProfile;
            tabControlProfile.SelectedTab = tabPageSite;
        }

        private void buttonRemoveSite_Click(object sender, EventArgs e)
        {
            foreach (DataRowView row in listBoxProfileSite.SelectedItems)
            {
                try
                {
                    function.DeleteSiteByProfileID(row["SITECODE"].ToString(), listBoxProfile1.SelectedValue.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

            }

            tabControlProfile.SelectedTab = tabPageProfile;
            tabControlProfile.SelectedTab = tabPageSite;
        }

        private void buttonAssignProfileMenu_Click(object sender, EventArgs e)
        {
            foreach (DataRowView row in listBoxMenu.SelectedItems)
            {
                try
                {
                    function.insertMenuByProfileID(row["MENUID"].ToString(), listBoxProfile1.SelectedValue.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

            }

            tabControlProfile.SelectedTab = tabPageProfile;
            tabControlProfile.SelectedTab = tabPageMenu;
        }

        private void buttonRemoveProfileMenu_Click(object sender, EventArgs e)
        {
            foreach (DataRowView row in listBoxProfileMenu.SelectedItems)
            {
                try
                {
                    function.DeleteMenuByProfileID(row["MENUID"].ToString(), listBoxProfile1.SelectedValue.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

            }

            tabControlProfile.SelectedTab = tabPageProfile;
            tabControlProfile.SelectedTab = tabPageMenu;
        }

        /// <summary>
        /// Fills ListBox brands based on Profile ID of the user who logins.
        /// </summary>
        private void FillListBoxBrands()
        {

            try
            {
                DTBrandExcludeProfile = function.GetBrandDataExcludeByProfileID(listBoxProfile1.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }



            //foreach (DataRow row in DTBrandInProfile.Rows) // Loop over the rows.
            //{
            //    BrandCode.Add((string)row["BRANDNAME"]);
            //    BrandName.Add((string)row["BRANDCODE"]);
            //    listBoxProfileBrand.DisplayMember = (string)row["BRANDNAME"];
            //    listBoxProfileBrand.ValueMember = (string)row["BRANDCODE"];
            //}



            //fill in list box brand

            try
            {
                DTBrandInProfile = function.GetBrandDataByProfileID(listBoxProfile1.SelectedValue.ToString());
                //DTBrandInProfileStockDisplay = function.GetBrandDataByProfileID(listBoxProfile1.SelectedValue.ToString());

                //DataRow newCustomersRow = DTBrandInProfileStockDisplay.NewRow();

                //newCustomersRow["BRANDCODE"] = " ";
                //newCustomersRow["BRANDNAME"] = " ";

                //DTBrandInProfileStockDisplay.Rows.Add(newCustomersRow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
             MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            listBoxProfileBrand.DataSource = DTBrandInProfile;
            listBoxProfileBrand.DisplayMember = "BRANDNAME";
            listBoxProfileBrand.ValueMember = "BRANDCODE";

            //fill in list box profile brand

            listBoxBrand.DataSource = DTBrandExcludeProfile;
            listBoxBrand.DisplayMember = "BRANDNAME";
            listBoxBrand.ValueMember = "BRANDCODE";


        }

        /// <summary>
        /// Fills the ListBox sites based on Profile ID of the user who logins.
        /// </summary>
        private void FillListBoxSites()
        {
            //fill in list box site\

            try
            {
                DTSiteInProfile = function.GetSiteDataByProfileID(listBoxProfile1.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
         MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            listBoxProfileSite.DataSource = DTSiteInProfile;
            listBoxProfileSite.DisplayMember = "SITENAME";
            listBoxProfileSite.ValueMember = "SITECODE";

            //fill in list box profile site

            try
            {
                DTSiteExcludeProfile = function.GetSiteDataExcludeByProfileID(listBoxProfile1.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            listBoxSite.DataSource = DTSiteExcludeProfile;
            listBoxSite.DisplayMember = "SITENAME";
            listBoxSite.ValueMember = "SITECODE";
        }

        /// <summary>
        /// Fills the ListBox menus based on Profile ID of the user who logins.
        /// </summary>
        private void FillListBoxMenus()
        {
            //fill in list box brand
            try
            {
                DTMenuInProfile = function.GetMenuDataByProfileID(listBoxProfile1.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            listBoxProfileMenu.DataSource = DTMenuInProfile;
            listBoxProfileMenu.DisplayMember = "MENU";
            listBoxProfileMenu.ValueMember = "MENUID";

            //fill in list box profile brand
            try
            {
                DTMenuExcludeProfile = function.GetMenuDataExcludeByProfileID(listBoxProfile1.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            listBoxMenu.DataSource = DTMenuExcludeProfile;
            listBoxMenu.DisplayMember = "MENU";
            listBoxMenu.ValueMember = "MENUID";
        }
        #endregion

        #region Sales Input



        private void textBoxBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void textBoxQuantitySalesInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void textBoxQuantitySalesInput_TextChanged(object sender, EventArgs e)
        {
            ProcessNetAmountandPrice();
        }



        private void textBoxPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void textBoxPrice_TextChanged(object sender, EventArgs e)
        {
            ProcessNetAmountandPrice();
        }


        private void ProcessNetAmountandPrice()
        {
            if (item.StatusSales == 0)
            {
                if (string.IsNullOrWhiteSpace(textBoxBarcode.Text))
                {
                    MessageBox.Show("Barcode Kosong, tolong masukkan data di text Barcode", "Barcode kosong",
                                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxQuantitySalesInput.Text = "";
                    textBoxBarcode.Focus();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(textBoxQuantitySalesInput.Text))
                    {
                        MessageBox.Show("Quantity Kosong, tolong masukkan data di text Quantity", "Quantity kosong",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxQuantitySalesInput.Focus();
                        textBoxNetAmount.Text = "";
                    }
                    else if (string.IsNullOrWhiteSpace(textBoxPrice.Text))
                    {

                        MessageBox.Show("Price Kosong, tolong masukkan data di text Price", "Price kosong",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxPrice.Focus();
                        textBoxNetAmount.Text = "";
                    }
                    else
                    {
                        if (textBoxPrice.Text == "0")
                        {
                            MessageBox.Show("Salah Format, Price harus lebih dari 0", "Salah Value",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxPrice.Text = "1";
                            textBoxPrice.Focus();
                            textBoxNetAmount.Text = "";
                        }
                        else if (textBoxQuantitySalesInput.Text == "0")
                        {
                            MessageBox.Show("Salah Format, Quantity harus lebih dari 0", "Salah Value",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxQuantitySalesInput.Text = null;
                            textBoxQuantitySalesInput.Focus();
                            textBoxNetAmount.Text = "";
                        }
                        else if (textBoxPrice.Text.Contains(" "))
                        {
                            MessageBox.Show("Price Mengandung Spasi", "Price Tidak Boleh mengandung spasi",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxPrice.Text = "1";
                            textBoxPrice.Focus();
                            textBoxNetAmount.Text = "";
                        }
                        else if (textBoxQuantitySalesInput.Text.Contains(" "))
                        {
                            MessageBox.Show("Quantity Mengandung Spasi", "Quantity Tidak Boleh mengandung spasi",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxQuantitySalesInput.Text = null;
                            textBoxQuantitySalesInput.Focus();
                            textBoxNetAmount.Text = "";
                        }
                        else if (textBoxQuantitySalesInput.Text.Contains("."))
                        {
                            MessageBox.Show("Quantity Mengandung titik", "Quantity Tidak Boleh mengandung titik",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxQuantitySalesInput.Text = null;
                            textBoxQuantitySalesInput.Focus();
                            textBoxNetAmount.Text = "";
                        }
                        //else if (textBoxPrice.Text.Contains("."))
                        //{
                        //    MessageBox.Show("Price Mengandung titik", "Price Tidak Boleh mengandung titik",
                        //                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    textBoxPrice.Text = "1";
                        //    textBoxPrice.Focus();
                        //    textBoxNetAmount.Text = "";
                        //}
                        else
                        {

                            Decimal Price = Convert.ToDecimal(textBoxPrice.Text);
                            logger.Trace("Price : " + Price);
                            Decimal Qty = Convert.ToDecimal(textBoxQuantitySalesInput.Text);
                            logger.Trace("Qty : " + Qty);
                            //Decimal TotalDiscAmount = Convert.ToDecimal(textBoxDiscount.Text);
                            //Decimal Disc2 = Convert.ToDecimal(textBoxDiscount2.Text);

                            //Discount1Amount = (Price * (Disc1 / 100));

                            //Decimal PriceAfterDiscount1 = (Price - Discount1Amount);

                            //Discount2Amount = (PriceAfterDiscount1 * (Disc2 / 100));

                            //textBoxNetAmount.Text = string.Format("{0:0,0}", ((PriceAfterDiscount1 - Discount2Amount) * Qty));




                            if (item.FixPrice != 0)
                            {
                                NetAmount = item.FixPrice * Qty;
                                
                            }
                            else
                            {
                                if (
                                    function
                                        .SelectPriceByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput
                                        (
                                            textBoxBarcode.Text, GlobalVar.GlobalVarProfileID, GlobalVar.GlobalVarSite) ==
                                    1)
                                {
                                    Decimal Price1, Price2, Price3 = 0;
                                    Price1 = (Price - (Price * item.Discount1) / 100);
                                    logger.Trace("Price : " + Price);
                                    logger.Trace("Discount1 : " + item.Discount1);
                                    logger.Trace("Price 1 : " + Price1 );
                                    Price2 = (Price1 - (Price1 * item.Discount2) / 100);
                                    logger.Trace("Discount2 : " + item.Discount2);
                                    logger.Trace("Price 2 : " + Price2);
                                    Price3 = (Price2 - (Price2 * item.Discount3) / 100);
                                    logger.Trace("Discount1 : " + item.Discount3);
                                    logger.Trace("Price 3 : " + Price3);
                                    item.TotalDiscountAmount = Price - (Price3 - item.DiscountRP);
                                    logger.Trace("TotalDiscountAmount : " + item.TotalDiscountAmount);
                                    NetAmount = (Price - item.TotalDiscountAmount) * Qty;
                                    logger.Trace("Qty : " + Qty);
                                    logger.Trace("netAmount : " + item.TotalDiscountAmount);
                                }
                                else
                                {
                                    NetAmount = (Price - item.TotalDiscountAmount) * Qty;
                                    logger.Trace("Price : " + Price);
                                    logger.Trace("netAmount : " + item.TotalDiscountAmount);
                                    logger.Trace("Qty : " + Qty);
                                }

                            }
                            textBoxNetAmount.Text = string.Format("{0:0,0}", NetAmount);
                            GrossAmount = Price * Qty;

                        }
                    }
                }
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            initFromSalesInput = true;
            activePanel.Visible = false;
            activePanel = panelSalesSearch;
            activePanel.Visible = true;
            ComboBoxBrandCodeSalesSearch.Text = "";
        }

        private void textBoxBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return || e.KeyCode == Keys.Tab)
            {
                processSalesInputBarcodeLeave();
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonOKandPrint control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonOKandPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxQuantitySalesInput.Text))
            {
                MessageBox.Show("Quantity Masih kosong", "Quantity Kosong",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxQuantitySalesInput.Focus();
            }
            else if (textBoxQuantitySalesInput.Text.Contains(" "))
            {
                MessageBox.Show("Quantity Mengandung Spasi", "Quantity Tidak Boleh mengandung spasi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxQuantitySalesInput.Text = "";
                textBoxQuantitySalesInput.Focus();
            }
            else if (string.IsNullOrWhiteSpace(textBoxBarcode.Text))
            {
                MessageBox.Show("Barcode Masih kosong", "Barcode Kosong",
                             MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxBarcode.Focus();
                ClearSalesInputField();
            }
            else
            {
                item.Price = decimal.Parse(textBoxPrice.Text);
                item.NomorNota = function.getNextValSalesInput();
                item.Qty = Convert.ToInt16(textBoxQuantitySalesInput.Text);

                try
                {
                    function.insertSalesInput(item, textBoxQuantitySalesInput.Text, decimal.Parse(textBoxNetAmount.Text), GrossAmount);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

                try
                {
                    StatusNota = "ORIGINAL";
                    printDocument.Print();
                    StatusNota = "--COPY--";
                    printDocument.Print();
                    StatusNota = "ORIGINAL";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }


                item.StatusSales = 1;
                ClearSalesInputField();

            }
        }

        /// <summary>
        /// Clears the sales input textbox.
        /// </summary>
        private void ClearSalesInputField()
        {
            logger.Debug("Clear Sales Input Field");
            textBoxArticle.Text = "";
            textBoxColor.Text = "";
            textBoxDescription.Text = "";
            textBoxDiscount.Text = "";
            textBoxDiscount2.Text = "";
            textBoxPrice.Text = "";
            textBoxSize.Text = "";
            textBoxBrandCode.Text = "";
            textBoxBrandName.Text = "";
            textBoxBarcode.Text = "";
            textBoxNetAmount.Text = "";
            textBoxQuantitySalesInput.Text = "";
            textBoxDiscount3.Text = "";
            textBoxDiscountRP.Text = "";
            textBoxFixPriceSalesInput.Text = "";
            textBoxBarcode.Focus();
        }

        #region Sales Input Search
        private void dataGridViewItemSalesSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                }
                else
                {
                    item.StatusSales = 1;
                    int rowIndex = e.RowIndex;
                    DataGridViewRow row2 = dataGridViewItemSalesSearch.Rows[rowIndex];

                    try
                    {
                        DTItem = function.SelectItemByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteSalesInput(row2.Cells["BARCODE"].Value.ToString(), GlobalVar.GlobalVarProfileID, comboBoxSite.SelectedValue.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }


                    foreach (DataRow row in DTItem.Rows) // Loop over the rows.
                    {
                        item.Article = (string)row["ARTICLE"];
                        item.Barcode = row2.Cells["BARCODE"].Value.ToString();
                        item.Color = row["COLOR"].ToString();
                        item.Description = (string)row["DESCRIPTION"];
                        item.Discount1 = decimal.Parse(row["DISC1"].ToString());
                        item.Discount2 = decimal.Parse(row["DISC2"].ToString());
                        item.Discount3 = decimal.Parse(row["DISC3"].ToString());
                        item.DiscountRP = decimal.Parse(row["DISCRP"].ToString());
                        item.Price = decimal.Parse(row["PRICE"].ToString());
                        item.Size = row["UKURAN"].ToString();
                        item.VariantID = (int)row["VARIANTID"];
                        item.Brand = (string)row["BRAND"];
                        item.Store = row["STORE"].ToString();
                        item.UserID = GlobalVar.GlobalVarUserID;
                        item.BrandName = (string)row["BRANDNAME"];

                        item.TotalDiscountAmount = string.IsNullOrWhiteSpace(row["TOTDISAMOUNT"].ToString()) ? 0 : decimal.Parse(row["TOTDISAMOUNT"].ToString());
                        item.FixPrice = string.IsNullOrWhiteSpace(row["FIXPRICE"].ToString()) ? 0 : decimal.Parse(row["FIXPRICE"].ToString());


                    }

                    if (initFromSalesInput)
                    {
                        textBoxBarcode.Text = item.Barcode;
                        textBoxBrandCode.Text = item.Brand;
                        textBoxBrandName.Text = item.BrandName;
                        textBoxArticle.Text = item.Article;
                        textBoxDescription.Text = item.Description;
                        textBoxSize.Text = item.Size;
                        textBoxColor.Text = item.Color;
                        textBoxPrice.Text = string.Format("{0:0,0}", item.Price);
                        textBoxDiscount.Text = Convert.ToString(item.Discount1);
                        textBoxDiscount2.Text = Convert.ToString(item.Discount2);
                        textBoxDiscount3.Text = Convert.ToString(item.Discount3);
                        textBoxDiscountRP.Text = string.Format("{0:0,0}", item.DiscountRP);


                        textBoxFixPriceSalesInput.Text = string.Format("{0:0,0}", item.FixPrice);

                        activePanel.Visible = false;
                        activePanel = panelSales;
                        activePanel.Visible = true;
                        textBoxQuantitySalesInput.Text = "1";
                        textBoxQuantitySalesInput.Focus();

                        textBoxPrice.ReadOnly = item.Price != 1;
                        item.StatusSales = 0;
                        ProcessNetAmountandPrice();
                    }
                    else
                    {
                        activePanel.Visible = false;
                        activePanel = panelMultipleSalesInput;
                        activePanel.Visible = true;
                        textBoxBarcodeMultipleSalesInput.Focus();
                        ProcessDataGridViewMultipleSalesInput(item.Barcode);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                MessageBox.Show(ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }





        }

        private void buttonCancelSalesSearch_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            if (initFromSalesInput)
            {
                activePanel = panelSales;
                activePanel.Visible = true;
                textBoxBarcode.Focus();
            }
            else
            {
                activePanel = panelMultipleSalesInput;
                activePanel.Visible = true;
                textBoxBarcodeMultipleSalesInput.Focus();
            }

        }

        private void buttonSearchSalesSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxBrandNameSalesSearch.Text))
            {
                MessageBox.Show("Brand Name kosong, tolong masukkan data", "Filter Brand Name Kosong",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string BrandCode = String.IsNullOrWhiteSpace(ComboBoxBrandCodeSalesSearch.GetItemText(ComboBoxBrandCodeSalesSearch.Text)) ? "" : ComboBoxBrandCodeSalesSearch.GetItemText(ComboBoxBrandCodeSalesSearch.Text).ToUpper();
                string BrandName = textBoxBrandNameSalesSearch.Text == null ? "" : textBoxBrandNameSalesSearch.Text.ToUpper();
                string Article = textBoxArtikelSalesSearch.Text == null ? "" : textBoxArtikelSalesSearch.Text.ToUpper();
                string Description = textBoxDescriptionSalesSearch.Text == null ? "" : textBoxDescriptionSalesSearch.Text.ToUpper();
                string Size = textBoxSizeSalesSearch.Text == null ? "" : textBoxSizeSalesSearch.Text.ToUpper();


                salesSearchFilter.BrandCode = BrandCode;
                salesSearchFilter.BrandName = BrandName;
                salesSearchFilter.Article = Article;
                salesSearchFilter.Description = Description;
                salesSearchFilter.Size = Size;




                TotalData = function.CountSelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndArticleAndDescription(salesSearchFilter,
                GlobalVar.GlobalVarProfileID, GlobalVar.GlobalVarSite);


                //if (backgroundWorker1.IsBusy != true)
                //{
                //    // create a new instance of the alert form
                //    progressBar = new ProgressBarForm();
                //    // event handler for the Cancel button in AlertForm
                //    progressBar.ProgressBarMaxValue = TotalData;
                //    progressBar.Canceled += new EventHandler<EventArgs>(buttonCancel_Click);
                //    progressBar.Show();


                //    // Start the asynchronous operation.
                //    backgroundWorker1.RunWorkerAsync();
                //}

                // Set cursor as hourglass
                Cursor.Current = Cursors.WaitCursor;


                Application.DoEvents();

                // Execute your time-intensive hashing code here...

                DTItem =
                        function.SelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndArticleAndDescription(salesSearchFilter,
                            GlobalVar.GlobalVarProfileID, GlobalVar.GlobalVarSite);


                dataGridViewItemSalesSearch.DataSource = DTItem;

                //this.dataGridViewItemSalesSearch.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridViewItemSalesSearch.Columns["DESCRIPTION"].MinimumWidth = 200;
                dataGridViewItemSalesSearch.Columns["COLOR"].MinimumWidth = 150;



                var viewColumn = this.dataGridViewItemSalesSearch.Columns["PRICE"];
                if (viewColumn != null)
                    viewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                var gridViewColumn = this.dataGridViewItemSalesSearch.Columns["GROSSAMOUNT"];
                if (gridViewColumn != null)
                    gridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                var dataGridViewColumn = this.dataGridViewItemSalesSearch.Columns["NETAMOUNT"];
                if (dataGridViewColumn != null)
                    dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;

                dataGridViewColumn = this.dataGridViewItemSalesSearch.Columns["DISC1"];
                if (dataGridViewColumn != null)
                    dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                dataGridViewColumn = this.dataGridViewItemSalesSearch.Columns["DISC2"];
                if (dataGridViewColumn != null)
                    dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                dataGridViewColumn = this.dataGridViewItemSalesSearch.Columns["DISC3"];
                if (dataGridViewColumn != null)
                    dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                dataGridViewColumn = this.dataGridViewItemSalesSearch.Columns["DISCRP"];
                if (dataGridViewColumn != null)
                    dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                dataGridViewColumn = this.dataGridViewItemSalesSearch.Columns["FIXPRICE"];
                if (dataGridViewColumn != null)
                    dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;

                //backgroundWorker1.CancelAsync();

                // Set cursor as default arrow
                Cursor.Current = Cursors.Default;

            }

        }

        // This event handler cancels the backgroundworker, fired from Cancel button in AlertForm.
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
                // Close the AlertForm
                progressBar.Close();
            }
        }
        #endregion

        #region Nota
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            PrintNota(StatusNota, e);
        }


        private void PrintNota(String Status, PrintPageEventArgs e)
        {
            logger.Debug("Print Nota");
            try
            {
                logger.Debug("Barcode : " + item.Barcode);
                logger.Debug("Article : " + item.Article);
                logger.Debug("Discount 1 : " + item.Discount1);
                logger.Debug("Discount 2 : " + item.Discount2);
                logger.Debug("Discount 3 : " + item.Discount3);
                logger.Debug("Discount RP : " + item.DiscountRP);
                logger.Debug("Total Discount Amount : " + item.TotalDiscountAmount);

                Graphics graphic = e.Graphics;
                Font font = new Font("Courier New", 8);
                Font fontBold = new Font("Courier New", 8, FontStyle.Bold);
                Font fontBarcode = new Font("IDAutomationHC39M", 10);
                Font fontLargeBold = new Font("Courier New", 24, FontStyle.Bold);


                string NotaLastFourDigit = item.NomorNota;
                NotaLastFourDigit = NotaLastFourDigit.Substring(NotaLastFourDigit.Length - 4);

                float fontHeight = font.GetHeight();

                Pen pen = new Pen(Color.Black, 3);
                int startX = 0;
                int startY = 0;
                int offsetY = 35;
                int newLine = 15;

                graphic.DrawString("K", fontLargeBold, new SolidBrush(Color.Black), startX, startY);
                graphic.DrawString(NotaLastFourDigit, fontLargeBold, new SolidBrush(Color.Black), startX + 155, startY);



                graphic.DrawString("CHANDRA DEPARTMENT STORE", font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY += 15;

                //Store
                graphic.DrawString(comboBoxSite.GetItemText(comboBoxSite.SelectedItem), font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY += 30;


                graphic.DrawString("---------------" + Status + "---------------", font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY += 30;

                graphic.DrawString("Bon Penjualan", font, new SolidBrush(Color.Black), startX, startY + offsetY);
                graphic.DrawString(item.BrandName, font, new SolidBrush(Color.Black), startX + 120, startY + offsetY);


                offsetY = offsetY + newLine;





                //tanggal penjualan
                DateTime dateTime = DateTime.Now;
                //graphic.DrawString("Date : " + dateTime.Day + " " + dateTime.ToString("MMM") + " " + dateTime.Year.ToString(), font, new SolidBrush(Color.Black), startX, startY + offsetY);

                //offsetY = offsetY + newLine;

                //No Nota
                graphic.DrawString("No# " + item.NomorNota, font, new SolidBrush(Color.Black), startX, startY + offsetY);



                //SPG/SPB
                graphic.DrawString("SPG/SPB:" + GlobalVar.GlobalVarUsername.SafeSubstring(0, 14), font, new SolidBrush(Color.Black), startX + 110, startY + offsetY);

                offsetY += 14;

                //draw line separator

                graphic.DrawString("--------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY = offsetY + newLine * 2;
                //offsetY += 10;


                //article name dan size
                //graphic.DrawString(item.Description + "   " + item.Size, font, new SolidBrush(Color.Black), startX, startY + offsetY);
                graphic.DrawString(item.Article + "     " + item.Description, font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY = offsetY + 20;

                //line item
                graphic.DrawString(item.Qty.ToString() + "X      " + string.Format("{0:0,0}", item.Price) + "      " + string.Format("{0:0,0}", (Convert.ToDecimal(item.Qty * item.Price))), font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY += 20;

                String Discount = "";





                if (item.FixPrice != 0)
                {
                    logger.Debug("is Fix Price");
                    graphic.DrawString("HSP / Fix Price", font, new SolidBrush(Color.Black), startX, startY + offsetY);
                    offsetY += 20;
                }
                else
                {
                    if (item.TotalDiscountAmount != 0)
                    {
                        logger.Debug("Total Discount Amount is not 0");
                        Discount = function.getDiscount(item);
                        logger.Debug("Discount : " + Discount);
                        graphic.DrawString("DISC " + Discount/* + " "+ string.Format("{0:0,0}",(item.TotalDiscountAmount*item.Qty))*/, font, new SolidBrush(Color.Black), startX, startY + offsetY);
                        offsetY += 20;
                    }
                }


                logger.Debug("Net Amount : " + textBoxNetAmount.Text);


                graphic.DrawString("NetAmount              ", font, new SolidBrush(Color.Black), startX, startY + offsetY);
                graphic.DrawString("                        " + string.Format("{0:0,0}", textBoxNetAmount.Text), fontBold, new SolidBrush(Color.Black), startX, startY + offsetY);
                offsetY += 35;
                graphic.DrawString("*" + item.Barcode + "*", fontBarcode, new SolidBrush(Color.Black), startX + 30, startY + offsetY);

                offsetY += 50;

                //draw separator
                graphic.DrawString("--------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY += newLine;

                //draw Created Date

                graphic.DrawString("Created Date : " + dateTime.Day + " " + dateTime.ToString("MMM") + " " + dateTime.Year + "  :" + dateTime.Hour + ":" + dateTime.Minute + ":" + dateTime.Second, fontBold, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY = offsetY + (newLine * 2);


                graphic.DrawString("     BELANJA NYAMAN, BELANJA HEMAT", font, new SolidBrush(Color.Black), startX, startY + offsetY);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

        }

        private void PrintNotaMultipleSalesInput(String Status, PrintPageEventArgs e)
        {

            logger.Debug("Print Nota Multiple Sales Input");
            try
            {

                Graphics graphic = e.Graphics;
                Font font = new Font("Courier New", 8);
                Font fontBold = new Font("Courier New", 8, FontStyle.Bold);
                Font fontBarcode = new Font("IDAutomationHC39M", 10);
                Font fontLargeBold = new Font("Courier New", 24, FontStyle.Bold);

                //get the Nota Last Four digit
                string NotaLastFourDigit = itemMultipleSalesInput.NomorNota;
                NotaLastFourDigit = NotaLastFourDigit.Substring(NotaLastFourDigit.Length - 4);

                float fontHeight = font.GetHeight();

                Pen pen = new Pen(Color.Black, 3);
                int startX = 0;
                int startY = 0;
                int offsetY = 35;
                int newLine = 15;


                graphic.DrawString("K", fontLargeBold, new SolidBrush(Color.Black), startX, startY);
                graphic.DrawString(NotaLastFourDigit, fontLargeBold, new SolidBrush(Color.Black), startX + 155, startY);



                graphic.DrawString("CHANDRA DEPARTMENT STORE", font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY += 15;

                //Store
                graphic.DrawString(comboBoxSite.GetItemText(comboBoxSite.SelectedItem), font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY += 30;


                graphic.DrawString("---------------" + Status + "---------------", font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY += 30;

                graphic.DrawString("Bon Penjualan", font, new SolidBrush(Color.Black), startX, startY + offsetY);
                graphic.DrawString(itemMultipleSalesInput.BrandName, font, new SolidBrush(Color.Black), startX + 120, startY + offsetY);


                offsetY = offsetY + newLine;





                //tanggal penjualan
                DateTime dateTime = DateTime.Now;
                //graphic.DrawString("Date : " + dateTime.Day + " " + dateTime.ToString("MMM") + " " + dateTime.Year.ToString(), font, new SolidBrush(Color.Black), startX, startY + offsetY);

                //offsetY = offsetY + newLine;

                //No Nota
                graphic.DrawString("No# " + itemMultipleSalesInput.NomorNota, font, new SolidBrush(Color.Black), startX, startY + offsetY);



                //SPG/SPB
                graphic.DrawString("SPG/SPB:" + GlobalVar.GlobalVarUsername.SafeSubstring(0, 14), font, new SolidBrush(Color.Black), startX + 110, startY + offsetY);

                offsetY += 14;

                //draw line separator

                graphic.DrawString("--------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY = offsetY + newLine * 2;
                //offsetY += 10;



















                foreach (DataGridViewRow row in dataGridViewMultipleSalesInput.Rows) // Loop over the rows.
                {

                    decimal try2;
                    decimal NetAMount;


                    itemMultipleSalesInput.Article = row.Cells["ARTICLE"].Value.ToString();
                    itemMultipleSalesInput.Barcode = row.Cells["BARCODE"].Value.ToString();
                    itemMultipleSalesInput.Color = row.Cells["COLOR"].Value.ToString();
                    itemMultipleSalesInput.Description = row.Cells["DESCRIPTION"].Value.ToString();
                    itemMultipleSalesInput.Discount1 = decimal.Parse(row.Cells["DISC1"].Value.ToString());
                    itemMultipleSalesInput.Discount2 = decimal.Parse(row.Cells["DISC2"].Value.ToString());
                    itemMultipleSalesInput.Discount3 = decimal.Parse(row.Cells["DISC3"].Value.ToString());

                    decimal.TryParse(row.Cells["DISCRP"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                    itemMultipleSalesInput.DiscountRP = try2;

                    decimal.TryParse(row.Cells["PRICE"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                    itemMultipleSalesInput.Price = try2;


                    itemMultipleSalesInput.Size = row.Cells["UKURAN"].Value.ToString();
                    itemMultipleSalesInput.VariantID = int.Parse(row.Cells["VARIANTID"].Value.ToString());
                    itemMultipleSalesInput.Brand = (string)row.Cells["BRAND"].Value.ToString();
                    itemMultipleSalesInput.Store = row.Cells["STORE"].Value.ToString();
                    itemMultipleSalesInput.UserID = GlobalVar.GlobalVarUserID;
                    itemMultipleSalesInput.BrandName = (string)row.Cells["BRANDNAME"].Value.ToString();



                    if (string.IsNullOrWhiteSpace(row.Cells["FIXPRICE"].Value.ToString()))
                    {
                        itemMultipleSalesInput.FixPrice = 0;
                    }
                    else
                    {
                        decimal.TryParse(row.Cells["FIXPRICE"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                        itemMultipleSalesInput.FixPrice = try2;
                    }


                    if (string.IsNullOrWhiteSpace(row.Cells["TOTDISCAMOUNT"].Value.ToString()))
                    {
                        itemMultipleSalesInput.TotalDiscountAmount = 0;
                    }
                    else
                    {
                        decimal.TryParse(row.Cells["TOTDISCAMOUNT"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                        itemMultipleSalesInput.TotalDiscountAmount = try2;
                    }

                    itemMultipleSalesInput.Qty = int.Parse(row.Cells["QTY"].Value.ToString());








                    //article name dan size
                    //graphic.DrawString(item.Description + "   " + item.Size, font, new SolidBrush(Color.Black), startX, startY + offsetY);
                    graphic.DrawString(itemMultipleSalesInput.Article + "     " + itemMultipleSalesInput.Description, font, new SolidBrush(Color.Black), startX, startY + offsetY);

                    offsetY = offsetY + 20;

                    //line item
                    graphic.DrawString(itemMultipleSalesInput.Qty + "X      " + string.Format("{0:0,0}", itemMultipleSalesInput.Price) + "      " + string.Format("{0:0,0}", (Convert.ToDecimal(itemMultipleSalesInput.Qty * itemMultipleSalesInput.Price))), font, new SolidBrush(Color.Black), startX, startY + offsetY);

                    offsetY += 20;

                    String Discount = "";




                    if (itemMultipleSalesInput.FixPrice != 0)
                    {
                        graphic.DrawString("HSP / Fix Price", font, new SolidBrush(Color.Black), startX, startY + offsetY);
                        NetAmount = itemMultipleSalesInput.Qty * itemMultipleSalesInput.FixPrice;
                        offsetY += 20;
                    }
                    else
                    {
                        if (itemMultipleSalesInput.TotalDiscountAmount != 0)
                        {
                            Discount = function.getDiscount(item);
                            graphic.DrawString("DISC " + Discount/* + " "+ string.Format("{0:0,0}",(item.TotalDiscountAmount*item.Qty))*/, font, new SolidBrush(Color.Black), startX, startY + offsetY);
                            offsetY += 20;
                            NetAmount = itemMultipleSalesInput.Qty *
                                        (itemMultipleSalesInput.Price - itemMultipleSalesInput.TotalDiscountAmount);
                        }
                        else
                        {
                            NetAmount = itemMultipleSalesInput.Qty * itemMultipleSalesInput.Price;
                        }

                    }





                    graphic.DrawString("NetAmount              ", font, new SolidBrush(Color.Black), startX, startY + offsetY);



                    graphic.DrawString("                        " + string.Format("{0:0,0}", NetAmount), fontBold, new SolidBrush(Color.Black), startX, startY + offsetY);
                    offsetY += 35;
                    graphic.DrawString("*" + itemMultipleSalesInput.Barcode + "*", fontBarcode, new SolidBrush(Color.Black), startX + 30, startY + offsetY);

                    offsetY += 100;
                }
                //draw separator
                graphic.DrawString("--------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY += newLine;

                //draw Created Date

                graphic.DrawString("Created Date : " + dateTime.Day + " " + dateTime.ToString("MMM") + " " + dateTime.Year + "  :" + dateTime.Hour + ":" + dateTime.Minute + ":" + dateTime.Second, fontBold, new SolidBrush(Color.Black), startX, startY + offsetY);

                offsetY = offsetY + (newLine * 2);


                graphic.DrawString("     BELANJA NYAMAN, BELANJA HEMAT", font, new SolidBrush(Color.Black), startX, startY + offsetY);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


        }
        #endregion

        #endregion

        #region Sales History


        /// <summary>
        /// Updates the combo box brand code in Sales History.
        /// </summary>
        private void RefreshBrandCodeSalesHistory()
        {

            ComboBoxBrandCodeSalesHistory.DataSource = DTBrandInProfile;
            ComboBoxBrandCodeSalesHistory.DisplayMember = "BRANDCODE";
            ComboBoxBrandCodeSalesHistory.ValueMember = "BRANDCODE";
            ComboBoxBrandCodeSalesHistory.Text = "";
        }

        private void buttonSearchSalesHistory_Click(object sender, EventArgs e)
        {
            ProcessSearchSalesHistory();
        }

        private void textBoxBarcodeSalesHistory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                ProcessSearchSalesHistory();
            }
        }

        /// <summary>
        /// Processes the Data Grid View sales history.
        /// </summary>
        private void ProcessSearchSalesHistory()
        {
            itemSalesHistory.Store = GlobalVar.GlobalVarSite;

            DateTime From = dateTimePickerFromDate.Value;
            DateTime To = dateTimePickerToDate.Value;



            if (From.Date > To.Date)
            {
                MessageBox.Show("Tanggal Dari tidak boleh lebih besar dari Tanggal Sampai", "Tanggal Dari lebih besar dari Tanggal Sampai",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                String FromDate = From.Day + "-" + From.Month + "-" + From.Year;
                String ToDate = To.Day + "-" + To.Month + "-" + To.Year;

                String Status = "";
                if (comboBoxStatusItemSalesHistory.GetItemText(comboBoxStatusItemSalesHistory.Text) == "Reserved")
                {
                    Status = "1";
                }
                else if (comboBoxStatusItemSalesHistory.GetItemText(comboBoxStatusItemSalesHistory.Text) == "Sold")
                {
                    Status = "2";
                }
                else if (comboBoxStatusItemSalesHistory.GetItemText(comboBoxStatusItemSalesHistory.Text) == "Cancelled")
                {
                    Status = "3";
                }
                else
                {
                    Status = "";
                }


                itemSalesHistory.Article = textBoxArticleSalesHistory.Text == null ? "" : textBoxArticleSalesHistory.Text.ToUpper();
                itemSalesHistory.Barcode = textBoxBarcodeSalesHistory.Text == null ? "" : textBoxBarcodeSalesHistory.Text.ToUpper();
                itemSalesHistory.Brand = String.IsNullOrWhiteSpace(ComboBoxBrandCodeSalesHistory.GetItemText(ComboBoxBrandCodeSalesHistory.Text)) ? "" : ComboBoxBrandCodeSalesHistory.GetItemText(ComboBoxBrandCodeSalesHistory.Text).ToUpper();
                itemSalesHistory.BrandName = textBoxBrandNameSalesHistory.Text == null ? "" : textBoxBrandNameSalesHistory.Text.ToUpper();
                itemSalesHistory.Color = textBoxColorSalesHistory.Text == null ? "" : textBoxColorSalesHistory.Text.ToUpper();
                itemSalesHistory.Description = textBoxDescriptionSalesHistory.Text == null ? "" : textBoxDescriptionSalesHistory.Text.ToUpper();
                itemSalesHistory.Size = textBoxSizeSalesHistory.Text == null ? "" : textBoxSizeSalesHistory.Text.ToUpper();
                String NomorNotaSalesHistory = textBoxNomorNotaSalesHistory.Text == null ? "" : textBoxNomorNotaSalesHistory.Text.ToUpper();
                String UserID = textBoxUserIDSalesHistory.Text == null ? "" : textBoxUserIDSalesHistory.Text;

                try
                {
                    DTSalesInputSalesHistory = function.SelectSalesIputSalesHistory(itemSalesHistory, FromDate, ToDate, NomorNotaSalesHistory, UserID, Status);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }



                dataGridViewSalesHistory.DataSource = DTSalesInputSalesHistory;

                this.dataGridViewSalesHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                //set the column alignment to right
                this.dataGridViewSalesHistory.Columns["PRICE"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dataGridViewSalesHistory.Columns["GROSSAMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dataGridViewSalesHistory.Columns["NETAMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dataGridViewSalesHistory.Columns["DISCOUNT RP"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void buttonExportToExcelSalesHistory_Click(object sender, EventArgs e)
        {
            if (dataGridViewSalesHistory.RowCount == 0)
            {
                MessageBox.Show("Data Grid Kosong, Tolong gunakan fungsi search dahulu untuk memunculkan data", "Data Grid Kosong",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                copyAllDataGridViewtoClipboard(dataGridViewSalesHistory);
                ExportToExcelFromClipboard();
            }

        }
        #endregion

        #region Change Password
        private void buttonChangePassword_Click(object sender, EventArgs e)
        {
            if (textBoxOldPassword.Text != GlobalVar.GlobalVarPassword)
            {
                MessageBox.Show("Password Lama salah, tolong periksa kembali", "Password Tidak Sama",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxOldPassword.Text = "";

            }
            else if (textBoxNewPassword.Text == GlobalVar.GlobalVarPassword)
            {
                MessageBox.Show("Password Lama dan Password Baru Identik, tolong periksa kembali", "Password Sama",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxOldPassword.Text = "";
                textBoxNewPassword.Text = "";
                textBoxNewPasswordConfirm.Text = "";
            }
            else if (textBoxNewPassword.Text != textBoxNewPasswordConfirm.Text)
            {
                MessageBox.Show("Password Baru dan Confirm tidak sama, tolong periksa kembali", "Password Tidak Sama",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (textBoxNewPassword.Text.Contains(" "))
            {
                MessageBox.Show("Password Baru tidak boleh mengandung spasi", "Password Tidak boleh mengandung spasi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                String Message = function.changePassword(textBoxNewPassword.Text.ToString(), GlobalVar.GlobalVarUserID, GlobalVar.GlobalVarPassword);
                MessageBox.Show(Message, Message,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                GlobalVar.GlobalVarPassword = textBoxNewPassword.Text;

                timerIdle.Stop();
                timerIdle.Tick -= timerHandler;
                timerIdle.Enabled = false;
                timerIdle.Dispose();
                LoginPanelInitialization();
            }
        }

        private void LoginPanelInitialization()
        {
            disableHeader();
            disableMenuStrip();
            activePanel.Visible = false;
            activePanel = panelLogin;
            activePanel.Visible = true;
            panelLogin.Focus();
            textBoxLoginUserID.Focus();
        }

        private void ClearDataTable()
        {
            DTStockDisplay.Clear();
            DTItem.Clear();
            DTMultipleSalesInputOri.Clear();
            DTUploadPromo.Clear();
            DTMemoDiscountHeader.Clear();
            DTMemoDiscountDetail.Clear();
            DTSalesInputValidasi.Clear();
            if (DTParameter != null)
            {
                DTParameter.Clear();
            }
            DTPrintLabelSearch.Clear();
            DTPrintLabel.Clear();
            DTSalesInputSalesHistory.Clear();
        }
        #endregion

        #region Validasi Event
        private void buttonSearchValidasi_Click(object sender, EventArgs e)
        {
            updateDataGridViewSalesInputValidasi();
            buttonValidate.Enabled = DTSalesInputValidasi.Rows.Count == 0 ? false : true;
        }


        private void buttonValidate_Click(object sender, EventArgs e)
        {
            //if (String.IsNullOrWhiteSpace(comboBoxValidasi.GetItemText(comboBoxValidasi.SelectedItem)))
            //{
            //    MessageBox.Show("Tolong pilih salah satu Validasi", "Validasi Harus dipilih",
            //            MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    tabControlProfile.SelectedTab = tabPageProfile;
            //    comboBoxValidasi.Focus();
            //}
            //else
            //{
            if (dataGridViewSalesInputValidasi.ColumnCount < 2)
            {
                MessageBox.Show("Data Kosong, tolong gunakan fungsi search", "Grid Kosong",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                List<DataGridViewRow> rows_with_checked_column = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridViewSalesInputValidasi.Rows)
                {
                    //check for the checked rows and add it into List rows_with_checked_column
                    if (Convert.ToBoolean(row.Cells["CHECKED"].Value) == true)
                    {
                        rows_with_checked_column.Add(row);
                    }
                }

                if (rows_with_checked_column.Count == 0)
                {
                    MessageBox.Show("Tidak ada data yang dipilih, tolong dicheck salah satu data", "Tidak ada data yang dipilih",
                           MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
                else
                {
                    DialogResult DR = MessageBox.Show("Apa anda yakin mau mengubah status item ?", "Confirm Validasi",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (DR == DialogResult.Yes)
                    {
                        //String Status = comboBoxValidasi.GetItemText(comboBoxValidasi.SelectedItem) == "Sold" ? "2" : "3";

                        String Status = "2";

                        foreach (DataGridViewRow row in rows_with_checked_column)
                        {
                            try
                            {
                                function.updateSalesInputFlagbyNomorNota(row.Cells["NOMOR NOTA"].Value.ToString(), Status);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error Occured",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                logger.Error(ex.Message);
                            }

                        }
                        updateDataGridViewSalesInputValidasi();
                    }
                }
            }

            //}
        }

        /// <summary>
        /// Updates the data grid view sales input validasi.
        /// </summary>
        private void updateDataGridViewSalesInputValidasi()
        {
            Item itemValidasi = new Item();

            itemValidasi.Brand = String.IsNullOrWhiteSpace(comboBoxBrandCodeValidasi.GetItemText(comboBoxBrandCodeValidasi.Text)) ? "" : comboBoxBrandCodeValidasi.GetItemText(comboBoxBrandCodeValidasi.SelectedItem).ToUpper();
            itemValidasi.BrandName = textBoxBrandNameValdiasi.Text == null ? "" : textBoxBrandNameValdiasi.Text.ToUpper();
            itemValidasi.Article = textBoxArticleValidasi.Text == null ? "" : textBoxArticleValidasi.Text.ToUpper();
            itemValidasi.Barcode = textBoxBarcodeValidasi.Text == null ? "" : textBoxBarcodeValidasi.Text.ToUpper();
            itemValidasi.Color = textBoxColorValidasi.Text == null ? "" : textBoxColorValidasi.Text.ToUpper();
            itemValidasi.Size = textBoxSizeValidasi.Text == null ? "" : textBoxSizeValidasi.Text.ToUpper();
            String NomorNota = textBoxNomorNotaValidasi.Text == null ? "" : textBoxNomorNotaValidasi.Text.ToUpper();


            try
            {
                DTSalesInputValidasi = function.SelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndNomorNotaReservedOnly(itemValidasi, NomorNota);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            dataGridViewSalesInputValidasi.DataSource = DTSalesInputValidasi;

            for (int i = 1; i <= dataGridViewSalesInputValidasi.Columns.Count - 1; i++)
            {
                dataGridViewSalesInputValidasi.Columns[i].ReadOnly = true;
            }

            this.dataGridViewSalesInputValidasi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            var dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["PRICE"];

            if (dataGridViewColumn != null)
                dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["GROSS AMOUNT"];
            if (dataGridViewColumn != null)
            {
                dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["TOTAL DISCOUNT AMOUNT"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["NET AMOUNT"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["DISCOUNT RP"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //buttonValidate.Enabled = DTSalesInputValidasi.Rows.Count == 0 ? false : true;
        }

        private void textBoxBarcodeValidasi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                updateDataGridViewSalesInputValidasi();
            }
        }
        #endregion






        private void UpdateSiteDropDownListData()
        {
            logger.Debug("Update Data");
            try
            {
                DTBrandInProfile = function.GetBrandDataByProfileID(GlobalVar.GlobalVarProfileID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            RefreshBrandCodeSalesHistory();
            RefreshComboBoxBrandCodeSalesSearch();
            RefreshComboBoxBrandCodeValidasi();
            ClearMultipleSalesInput();
            prevSiteIndex = comboBoxSite.SelectedIndex;
        }


        private void InitFromSearchItem()
        {
            panelStartingScreen.Visible = false;
            panelDisplay.Visible = false;
            panelChangePassword.Visible = false;
            panelProfileManagement.Visible = false;
            panelSales.Visible = true;
            panelUserManagement.Visible = false;
            panelValidasi.Visible = false;
            panelSalesHistory.Visible = false;
            panelUploadInventory.Visible = false;
            panelGenerateLabelSTCK.Visible = false;
            panelPaymentProcess.Visible = false;

            activePanel = panelSales;
            textBoxQuantitySalesInput.Focus();
        }

        #region Auto Logout After Certain Minutes
        private void TestMenu_MouseMove(object sender, MouseEventArgs e)
        {
            logger.Trace("Mouse Moved Auto Logout");
            timerIdle.Stop();
            timerIdle.Start();
        }

        private void TestMenu_KeyDown(object sender, KeyEventArgs e)
        {
            logger.Trace("Keyboard pressed Auto Logout");
            timerIdle.Stop();
            timerIdle.Start();
        }

        private void initialiseTimer()
        {
            logger.Debug("initialize Timer");
            timerIdle.Interval = (MinutesToLogout * 60 * 1000);
            logger.Debug("Interval : " + timerIdle.Interval);
            timerIdle.Stop();
            timerIdle.Start();
        }

        private void LogOut(object sender, EventArgs e)
        {
            logger.Debug("Logout");
            timerIdle.Stop();
            timerIdle.Tick -= timerHandler;
            timerIdle.Enabled = false;
            timerIdle.Dispose();
            try
            {
                ClearDataTable();
                LoginPanelInitialization();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

        }


        //private void logOut(object sender, EventArgs e)
        //{

        //}

        private void TestMenu_MouseClick(object sender, MouseEventArgs e)
        {
            timerIdle.Stop();
            timerIdle.Start();
        }
        #endregion

        #region Export To Excel
        /// <summary>
        /// Copies all data on the grid view to clipboard.
        /// </summary>
        /// <param name="dataGridViewExcel">The data grid view excel.</param>
        private void copyAllDataGridViewtoClipboard(DataGridView dataGridViewExcel)
        {
            logger.Debug("Copy Data Grid View to ClipBoard");
            dataGridViewExcel.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridViewExcel.MultiSelect = true;
            dataGridViewExcel.SelectAll();
            DataObject dataObj = dataGridViewExcel.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        /// <summary>
        /// Exports to excel the data contained in the clipboard.
        /// </summary>
        private void ExportToExcelFromClipboard()
        {
            logger.Debug("Expor to Excel from Clipboard");
            try
            {
                Microsoft.Office.Interop.Excel.Application xlexcel;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlexcel = new Microsoft.Office.Interop.Excel.Application();
                xlexcel.Visible = true;
                xlWorkBook = xlexcel.Workbooks.Add(misValue);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
                CR.Select();
                xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
           MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

        }
        #endregion

        #region Display
        private void buttonSearchDisplay_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxBrandNameStockDisplay.Text))
            {
                MessageBox.Show("Brand Name kosong, tolong masukkan  data", "Brand Name Kosong",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxBrandNameStockDisplay.Focus();
            }
            else
            {
                //if (comboBoxBrandStockDisplay.Text != "")
                //{
                processDisplay();
                //}
                //else
                //{
                //    MessageBox.Show("Brand kosong, tolong masukkan paling tidak satu data", "Brand Kosong",
                //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

            }

        }

        private void textBoxBarcodeDisplay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                processDisplay();
            }
        }

        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            if (dataGridViewStockDisplay.RowCount == 0)
            {
                MessageBox.Show("Data Grid Kosong", "Data Grid Kosong",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                copyAllDataGridViewtoClipboard(dataGridViewStockDisplay);
                ExportToExcelFromClipboard();
            }

        }

        /// <summary>
        /// Processes the display and update the Data Grid View Sales Input Display.
        /// </summary>
        private void processDisplay()
        {
            itemSalesDisplay.Store = GlobalVar.GlobalVarSite;

            itemSalesDisplay.Article = textBoxArticleDisplay.Text == null ? "" : textBoxArticleDisplay.Text.ToUpper();
            itemSalesDisplay.Color = textBoxColorDisplay.Text == null ? "" : textBoxColorDisplay.Text.ToUpper();
            itemSalesDisplay.Description = textBoxDescriptionDisplay.Text == null ? "" : textBoxDescriptionDisplay.Text.ToUpper();
            itemSalesDisplay.Size = textBoxSizeDisplay.Text == null ? "" : textBoxSizeDisplay.Text.ToUpper();
            itemSalesDisplay.Barcode = textBoxBarcodeDisplay.Text == null ? "" : textBoxBarcodeDisplay.Text.ToUpper();
            itemSalesDisplay.Brand = comboBoxBrandStockDisplay.Text == null ? "" : comboBoxBrandStockDisplay.Text.ToUpper();
            itemSalesDisplay.BrandName = textBoxBrandNameStockDisplay.Text == null ? "" : textBoxBrandNameStockDisplay.Text.ToUpper();


            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;

            Application.DoEvents();

            try
            {
                DTStockDisplay = function.SelectStockDisplay(itemSalesDisplay);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            dataGridViewStockDisplay.DataSource = DTStockDisplay;

            dataGridViewStockDisplay.Columns["DESCRIPTION"].MinimumWidth = 200;
            //dataGridViewStockDisplay.Columns["COLOR"].MinimumWidth = 150;





            //this.dataGridViewStockDisplay.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewStockDisplay.Columns["PRICE"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewStockDisplay.Columns["RESERVED"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dataGridViewSalesInputDisplay.Columns["SOLD"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dataGridViewSalesInputDisplay.Columns["CANCELLED"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewStockDisplay.Columns["G.O.L.D STOCK"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewStockDisplay.Columns["REMAINING STOCK"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
        }
        #endregion

        private void buttonDeleteProfile_Click(object sender, EventArgs e)
        {
            DialogResult DR = MessageBox.Show("Apa anda yakin mau Me-Delete Profile/Profile-Profile yang sudah dipilih ?", "Confirm Delete",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (DR == DialogResult.Yes)
            {
                foreach (DataRowView row in listBoxProfile1.SelectedItems)
                {

                    if (function.SearchForExistingTransactionByProfileID(row["PROFILEID"].ToString()) != 0)
                    {
                        MessageBox.Show("Masih ada data sales yang bersangkutan dengan Profile ini", "Data Sales Masih ada",
                       MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    else if (function.CountExistingMenuInProfileID(row["PROFILEID"].ToString()) != 0)
                    {
                        MessageBox.Show(
                            "Masih ada Menu yang terdaftar dengan Profile ini, tolong delete terlebih dahulu",
                            "Data Menu Masih Ada",
                            MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    else
                    {
                        String Result = "";
                        try
                        {
                            Result = function.DeleteProfileByProfileID(row["PROFILEID"].ToString());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error Occured",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            logger.Error(ex.Message);
                        }

                        if (Result != "Success Deleting Profile")
                        {
                            MessageBox.Show(
                           Result,
                           "Error Has Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(
                           Result,
                           Result,
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }


                        RefreshListBoxProfile1ProfileManagement();
                    }
                }
            }
        }

        private void RefreshListBoxProfile1ProfileManagement()
        {
            logger.Debug("Refrech List Box Profile Managementt");
            try
            {
                DTAllProfile = function.SelectAllProfile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            listBoxProfile1.DataSource = DTAllProfile;
            listBoxProfile1.DisplayMember = "PROFILENAME";
            listBoxProfile1.ValueMember = "PROFILEID";
        }

        private void textBoxFilterBrandInProfileManagement_TextChanged(object sender, EventArgs e)
        {
            var a = from BrandNameRow in DTBrandInProfile.AsEnumerable()
                    where BrandNameRow.Field<string>("BRANDNAME").StartsWith(textBoxFilterBrandInProfileManagement.Text, StringComparison.OrdinalIgnoreCase)
                    select BrandNameRow;
            DataView view = a.AsDataView();

            listBoxProfileBrand.DataSource = view;
            listBoxProfileBrand.DisplayMember = "BRANDNAME";
            listBoxProfileBrand.ValueMember = "BRANDCODE";
        }

        private void textBoxFilterBrandProfileManagement_TextChanged(object sender, EventArgs e)
        {
            var a = from BrandNameRow in DTBrandExcludeProfile.AsEnumerable()
                    where BrandNameRow.Field<string>("BRANDNAME").StartsWith(textBoxFilterBrandProfileManagement.Text, StringComparison.OrdinalIgnoreCase)
                    select BrandNameRow;
            DataView view = a.AsDataView();

            listBoxBrand.DataSource = view;
            listBoxBrand.DisplayMember = "BRANDNAME";
            listBoxBrand.ValueMember = "BRANDCODE";


            //String search = textBoxProfileName.Text;

            //if (String.IsNullOrEmpty(search))
            //{
            //    listBoxBrand.Items.Clear();
            //    listBoxBrand.Items.AddRange(BrandName.ToArray());
            //}

            //var items = (from a in BrandName
            //             where a.StartsWith(search)
            //             select a).ToArray<String>();

            //listBoxBrand.Items.Clear();
            //listBoxBrand.Items.AddRange(items);
        }

        #region Print Label
        private void buttonPrintLabelSearch_Click(object sender, EventArgs e)
        {
            processSearchPrintLabel();
        }

        private void processSearchPrintLabel()
        {
            DateTime From = dateTimePickerPrintLabelDari.Value;
            DateTime To = dateTimePickerPrintLabelSampai.Value;

            String POStatus = "";
            String Printed = "";

            if (comboBoxPOStatus.GetItemText(comboBoxPOStatus.SelectedItem) == "Received")
            {
                POStatus = "7";
            }
            else if (comboBoxPOStatus.GetItemText(comboBoxPOStatus.SelectedItem) == "Awaiting Delivery")
            {
                POStatus = "5";
            }
            else if (comboBoxPOStatus.GetItemText(comboBoxPOStatus.SelectedItem) == "Reception Without Order")
            {
                POStatus = "8";
            }


            if (comboBoxPrintStatus.GetItemText(comboBoxPrintStatus.Text) == "Printed")
            {
                Printed = "1";
            }
            else if (comboBoxPrintStatus.GetItemText(comboBoxPrintStatus.Text) == "Not Printed")
            {
                Printed = "0";
            }


            if (From.Date > To.Date)
            {
                MessageBox.Show("Tanggal Dari tidak boleh lebih besar dari Tanggal Sampai", "Tanggal Dari lebih besar dari Tanggal Sampai",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                String FromDate = From.Day + "-" + From.Month + "-" + From.Year;
                String ToDate = To.Day + "-" + To.Month + "-" + To.Year;

                String Site = String.IsNullOrWhiteSpace(textBoxPrintLabelSite.Text) ? "" : textBoxPrintLabelSite.Text.ToUpper();
                String PONumber = String.IsNullOrWhiteSpace(textBoxPrintLabelPONumber.Text) ? "" : textBoxPrintLabelPONumber.Text.ToUpper();


                try
                {
                    if (POStatus == "5" || POStatus == "7")
                    {
                        DTPrintLabelSearch = function.SelectPrintLabelPurchaseOrder(Site, PONumber, FromDate, ToDate,
                            POStatus, Printed);
                    }
                    else if (POStatus == "8")
                    {
                        DTPrintLabelSearch = function.SelectPrintLabelReceivingWithoutOrder(Site, PONumber, FromDate, ToDate,
                            Printed);
                    }
                    else
                    {
                        DTPrintLabelSearch = function.SelectPrintLabel(Site, PONumber, FromDate, ToDate,
                            POStatus, Printed);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }



                dataGridViewPrintLabel.DataSource = DTPrintLabelSearch;

                for (int i = 1; i <= dataGridViewPrintLabel.Columns.Count - 1; i++)
                {
                    dataGridViewPrintLabel.Columns[i].ReadOnly = true;
                }

                this.dataGridViewPrintLabel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                //set the column alignment to right
                //this.dataGridViewSalesHistory.Columns["PRICE"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                //this.dataGridViewSalesHistory.Columns["GROSSAMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                //this.dataGridViewSalesHistory.Columns["NETAMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            }
        }

        private void buttonProcessLabel_Click(object sender, EventArgs e)
        {

            if (dataGridViewPrintLabel.ColumnCount < 2)
            {
                MessageBox.Show("Data Kosong, tolong gunakan fungsi search", "Grid Kosong",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                List<DataGridViewRow> rows_with_checked_column_print_label = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridViewPrintLabel.Rows)
                {
                    //check for the checked rows and add it into List rows_with_checked_column
                    if (Convert.ToBoolean(row.Cells["CHECK"].Value) == true)
                    {
                        rows_with_checked_column_print_label.Add(row);
                    }
                }

                if (rows_with_checked_column_print_label.Count == 0)
                {
                    MessageBox.Show("Tidak ada data yang dipilih, tolong dicheck salah satu data", "Tidak ada data yang dipilih",
                           MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
                else
                {
                    DialogResult DR = MessageBox.Show("Apa anda yakin mau me-print label/ label-label ?", "Confirm Print Label",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (DR == DialogResult.Yes)
                    {
                        String Status = comboBoxValidasi.GetItemText(comboBoxValidasi.SelectedItem) == "Sold" ? "2" : "3";



                        String ListFileName = "";
                        foreach (DataGridViewRow row in rows_with_checked_column_print_label)
                        {

                            int i = 0;

                            try
                            {
                                FileName = function.GetFileNameFromPONumber(row.Cells["PO NUMBER"].Value.ToString());
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error Occured",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                logger.Error(ex.Message);
                            }


                            try
                            {
                                FilePath = function.GetFilePathFromSite(row.Cells["SITE"].Value.ToString());
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error Occured",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                logger.Error(ex.Message);
                            }

                            if (FilePath == null)
                            {
                                FilePath = ConfigurationManager.AppSettings["DefaultPrintLabelFilePath"];
                                MessageBox.Show("FilePath tidak ditemukan dengan site : " + row.Cells["SITE"].Value + " Mengambil ke default Filepath : " + FilePath + " yang terdapat di Config File", "FilePath Tidak Ditemukan",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                                logger.Error("FilePath is null with site : " + row.Cells["SITE"].Value);
                            }

                            try
                            {
                                DTPrintLabel = function.GetFileContentByPONumber(row.Cells["PO NUMBER"].Value.ToString());
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error Occured",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                                logger.Error(ex.Message);
                            }





                            //FilePath = "C:\\Label";

                            try
                            {
                                FileStream fs = new FileStream(FilePath + "\\" + FileName + ".txt", FileMode.Create);
                                StreamWriter writer = new StreamWriter(fs);

                                logger.Debug("Writing file to : " + FilePath + "\\" + FileName + ".txt");

                                ListFileName = ListFileName + FileName + ".txt ";

                                foreach (DataRow row2 in DTPrintLabel.Rows)
                                {
                                    object[] array = row2.ItemArray;

                                    for (i = 0; i < array.Length - 1; i++)
                                    {
                                        writer.Write(array[i].ToString() + "|");
                                    }
                                    writer.Write(array[i].ToString());
                                    writer.Write("\n");

                                }

                                writer.Close();
                                fs.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error Occured",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                logger.Error(ex.Message);
                            }



                            try
                            {
                                String POStatus = "";

                                if (row.Cells["PO STATUS"].Value.ToString() == "Received")
                                {
                                    POStatus = "7";
                                }
                                else if (row.Cells["PO STATUS"].Value.ToString() == "Awaiting Delivery")
                                {
                                    POStatus = "5";
                                }
                                else if (row.Cells["PO STATUS"].Value.ToString() == "Reception Without Order")
                                {
                                    POStatus = "8";
                                }
                                function.updatePrintLabelPrintStatus(row.Cells["PO NUMBER"].Value.ToString(), POStatus);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error Occured",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                logger.Error(ex.Message);
                            }

                        }

                        MessageBox.Show("Success Me-generate file dengan nama : " + ListFileName, "Success Generating File",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        processSearchPrintLabel();
                    }
                }

            }
            //SaveFileDialog saveFileDialogLabel = new SaveFileDialog();

            //saveFileDialogLabel.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //saveFileDialogLabel.FilterIndex = 2;
            //saveFileDialogLabel.RestoreDirectory = true;
            //saveFileDialogLabel.FileName = "asd.txt";

            //if (saveFileDialogLabel.ShowDialog() == DialogResult.OK)
            //{
            //    FileStream fs = new FileStream(saveFileDialogLabel.FileName, FileMode.Create);
            //    StreamWriter writer = new StreamWriter(fs);
            //    writer.Write();       // twexit is previously created  
            //    writer.Close();
            //    fs.Close();
            //}

            //FileStream fs = new FileStream(saveFileDialogLabel.FileName, FileMode.Create);
            //StreamWriter writer = new StreamWriter(fs);
            //writer.Write("asd");       // twexit is previously created  
            //writer.Close();
            //fs.Close();
        }
        #endregion

        private void linkLabelKDS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabelKDS.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://www.kahar.co.id/enterprise/");
        }

        private void dataGridViewUser_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox t = e.Control as TextBox;
            if (t != null)
            {
                t.UseSystemPasswordChar = true;
            }
        }

        private void panelValidasiContainer_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black,
             e.ClipRectangle.Left,
             e.ClipRectangle.Top,
             e.ClipRectangle.Width - 1,
             e.ClipRectangle.Height - 1);
            base.OnPaint(e);
        }

        private void panelSalesHistoryContainer_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black,
             e.ClipRectangle.Left,
             e.ClipRectangle.Top,
             e.ClipRectangle.Width - 1,
             e.ClipRectangle.Height - 1);
            base.OnPaint(e);
        }

        private void panelSalesHistoryContainer2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black,
             e.ClipRectangle.Left,
             e.ClipRectangle.Top,
             e.ClipRectangle.Width - 1,
             e.ClipRectangle.Height - 1);
            base.OnPaint(e);
        }

        private void panelSalesHistoryContainerButton_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black,
             e.ClipRectangle.Left,
             e.ClipRectangle.Top,
             e.ClipRectangle.Width - 1,
             e.ClipRectangle.Height - 1);
            base.OnPaint(e);
        }


        private void processSalesInputBarcodeLeave()
        {
            string BrandCode = "";
            string BrandName = "";

            BrandCode = textBoxBrandCode.Text == null ? "" : textBoxBrandCode.Text;
            BrandName = textBoxBrandName.Text == null ? "" : textBoxBrandName.Text;

            try
            {
                DTItem = function.SelectItemByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteSalesInput(textBoxBarcode.Text, GlobalVar.GlobalVarProfileID, comboBoxSite.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }



            if (DTItem.Rows.Count == 0)
            {
                MessageBox.Show("Item tidak ditemukan, harap periksa Barcode, Brand, atau gunakan fungsi search", "Item tidak ditemukan",
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearSalesInputField();
                textBoxBarcode.Focus();
            }
            else
            {
                foreach (DataRow row in DTItem.Rows) // Loop over the rows.
                {

                    item.Article = (string)row["ARTICLE"];
                    item.Barcode = textBoxBarcode.Text;
                    item.Color = row["COLOR"].ToString();
                    item.Description = (string)row["DESCRIPTION"];
                    item.Discount1 = decimal.Parse(row["DISC1"].ToString());
                    item.Discount2 = decimal.Parse(row["DISC2"].ToString());
                    item.Discount3 = decimal.Parse(row["DISC3"].ToString());
                    item.DiscountRP = decimal.Parse(row["DISCRP"].ToString());
                    item.Price = decimal.Parse(row["PRICE"].ToString());
                    item.Size = row["UKURAN"].ToString();
                    item.VariantID = (int)row["VARIANTID"];
                    item.Brand = (string)row["BRAND"];
                    item.Store = row["STORE"].ToString();
                    item.UserID = GlobalVar.GlobalVarUserID;
                    item.BrandName = (string)row["BRANDNAME"];
                    item.TotalDiscountAmount = decimal.Parse(row["TOTDISAMOUNT"].ToString());

                    item.FixPrice = string.IsNullOrWhiteSpace(row["FIXPRICE"].ToString()) ? 0 : decimal.Parse(row["FIXPRICE"].ToString());

                    
                    

                    textBoxArticle.Text = (string)row["ARTICLE"];
                    textBoxColor.Text = row["COLOR"].ToString();
                    textBoxDescription.Text = (string)row["DESCRIPTION"];
                    textBoxDiscount.Text = row["DISC1"].ToString();
                    textBoxDiscount2.Text = row["DISC2"].ToString();
                    textBoxPrice.Text = string.Format("{0:0,0}", decimal.Parse(row["PRICE"].ToString()));
                    textBoxFixPriceSalesInput.Text = string.Format("{0:0,0}", item.FixPrice);
                    textBoxSize.Text = row["UKURAN"].ToString();
                    textBoxBrandCode.Text = (string)row["BRAND"];
                    textBoxBrandName.Text = (string)row["BRANDNAME"];
                    textBoxDiscount3.Text = row["DISC3"].ToString();
                    textBoxDiscountRP.Text = string.Format("{0:0,0}", decimal.Parse(row["DISCRP"].ToString()));
                    textBoxQuantitySalesInput.Text = "1";

                    textBoxNetAmount.Text = item.FixPrice != 0 ? string.Format("{0:0,0}", (item.FixPrice) * decimal.Parse(textBoxQuantitySalesInput.Text)) : string.Format("{0:0,0}", (item.Price - item.TotalDiscountAmount) * decimal.Parse(textBoxQuantitySalesInput.Text));


                    logger.Trace("Price = " + item.Price);
                    logger.Trace("TotalDiscountAmount = " + item.TotalDiscountAmount);
                    logger.Trace("NetAmount = " +
                                 (item.Price - item.TotalDiscountAmount)*decimal.Parse(textBoxQuantitySalesInput.Text));
                    logger.Trace("Fix Price = " + item.FixPrice);
                    logger.Trace("Qty = " + textBoxQuantitySalesInput.Text);

                }

                textBoxPrice.ReadOnly = item.Price != 1;

                textBoxQuantitySalesInput.Focus();
                item.StatusSales = 0;
            }
        }



        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Tab) return true;
            return base.IsInputKey(keyData);
        }

        private void textBoxBarcode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                processSalesInputBarcodeLeave();
                textBoxBarcode.Focus();
            }
        }

        private void RefreshComboBoxBrandCodeSalesSearch()
        {
            ComboBoxBrandCodeSalesSearch.DataSource = DTBrandInProfile;
            ComboBoxBrandCodeSalesSearch.DisplayMember = "BRANDCODE";
            ComboBoxBrandCodeSalesSearch.ValueMember = "BRANDCODE";
            ComboBoxBrandCodeSalesSearch.Text = "";
        }

        private void RefreshComboBoxBrandCodeStockDisplay()
        {
            comboBoxBrandStockDisplay.DataSource = DTBrandInProfile;
            comboBoxBrandStockDisplay.DisplayMember = "BRANDCODE";
            comboBoxBrandStockDisplay.ValueMember = "BRANDCODE";
            comboBoxBrandStockDisplay.Text = "";
        }

        private void RefreshComboBoxBrandCodeValidasi()
        {
            comboBoxBrandCodeValidasi.DataSource = DTBrandInProfile;
            comboBoxBrandCodeValidasi.DisplayMember = "BRANDCODE";
            comboBoxBrandCodeValidasi.ValueMember = "BRANDCODE";
            comboBoxBrandCodeValidasi.Text = "";
        }

        private void buttonMinutesToLogout_Click(object sender, EventArgs e)
        {

            ErrorString = function.changeMinutesToLogout(textBoxMinutesToLogout.Text);
            MessageBox.Show(ErrorString, "Info",
                     MessageBoxButtons.OK, MessageBoxIcon.Information);

            MinutesToLogout = function.GetMinutesToLogout();

            initialiseTimer();
        }

        private void textBoxMinutesToLogout_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
           (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }



        #region Multiple Sales INput
        private void dataGridViewMultipleSalesInput_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Decimal PriceCheck = 0;
            decimal try2 = 0;
            if (!decimal.TryParse(dataGridViewMultipleSalesInput["PRICE", e.RowIndex].Value.ToString(), out PriceCheck))
            {
                dataGridViewMultipleSalesInput["PRICE", e.RowIndex].Value = 1;
            }
            else
            {
                decimal Qty = 0;
                Decimal TotDiscAmount;
                decimal.TryParse(dataGridViewMultipleSalesInput["QTY", e.RowIndex].Value.ToString(), out Qty);

                dataGridViewMultipleSalesInput.Rows[e.RowIndex].ErrorText = String.Empty;

                decimal.TryParse(dataGridViewMultipleSalesInput["PRICE", e.RowIndex].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                Decimal Price = try2;


                //dataGridViewMultipleSalesInput["PRICE", e.RowIndex].Value = string.Format("{0:0,0}", Price);

                if (string.IsNullOrWhiteSpace(dataGridViewMultipleSalesInput["FIXPRICE", e.RowIndex].Value.ToString()))
                {
                    if (
                    function.SelectPriceByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput(
                        dataGridViewMultipleSalesInput["BARCODE", e.RowIndex].Value.ToString(), GlobalVar.GlobalVarProfileID,
                        GlobalVar.GlobalVarSite) == 1)
                    {
                        Decimal Disc1 = Convert.ToDecimal(dataGridViewMultipleSalesInput["DISC1", e.RowIndex].Value.ToString());
                        Decimal Disc2 = Convert.ToDecimal(dataGridViewMultipleSalesInput["DISC2", e.RowIndex].Value.ToString());
                        Decimal Disc3 = Convert.ToDecimal(dataGridViewMultipleSalesInput["DISC3", e.RowIndex].Value.ToString());

                        decimal.TryParse(dataGridViewMultipleSalesInput["DISCRP", e.RowIndex].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                        Decimal DiscRP = try2;

                        Decimal Price1, Price2, Price3 = 0;
                        Price1 = (Price - (Price * Disc1) / 100);
                        Price2 = (Price1 - (Price1 * Disc2) / 100);
                        Price3 = (Price2 - (Price2 * Disc3) / 100);
                        TotDiscAmount = Price - (Price3 - DiscRP);
                        dataGridViewMultipleSalesInput["TOTDISCAMOUNT", e.RowIndex].Value = string.Format("{0:0,0}", TotDiscAmount);
                        dataGridViewMultipleSalesInput["NETAMOUNT", e.RowIndex].Value = string.Format("{0:0,0}", (Price - TotDiscAmount) * Qty);
                    }
                    else
                    {
                        decimal.TryParse(dataGridViewMultipleSalesInput["TOTDISCAMOUNT", e.RowIndex].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);

                        dataGridViewMultipleSalesInput["NETAMOUNT", e.RowIndex].Value = string.Format("{0:0,0}", (Price - try2) * Qty);
                    }
                }
                else
                {
                    decimal.TryParse(dataGridViewMultipleSalesInput["FIXPRICE", e.RowIndex].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                    dataGridViewMultipleSalesInput["NETAMOUNT", e.RowIndex].Value = string.Format("{0:0,0}", try2 * Qty);
                }
            }
        }

        private void textBoxBarcodeMultipleSalesInput_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                ProcessDataGridViewMultipleSalesInput(textBoxBarcodeMultipleSalesInput.Text);
                textBoxBarcodeMultipleSalesInput.Text = "";
            }
        }

        private void ProcessDataGridViewMultipleSalesInput(String Barcode)
        {
            try
            {
                DTMultipleSalesInputOri = function.SelectItemByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput(Barcode, GlobalVar.GlobalVarProfileID, GlobalVar.GlobalVarSite);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }



            if (DTMultipleSalesInputOri.Rows.Count == 0)
            {
                MessageBox.Show("No Data Found", "No Data found",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DTMultipleSalesInputCopy.Merge(DTMultipleSalesInputOri);


                dataGridViewMultipleSalesInput.DataSource = DTMultipleSalesInputCopy;

                DTMultipleSalesInputCopy.Columns["QTY"].ReadOnly = false;
                DTMultipleSalesInputCopy.Columns["NETAMOUNT"].ReadOnly = false;
                DTMultipleSalesInputCopy.Columns["PRICE"].ReadOnly = false;
                DTMultipleSalesInputCopy.Columns["TOTDISCAMOUNT"].ReadOnly = false;

                this.dataGridViewMultipleSalesInput.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                dataGridViewMultipleSalesInput.Columns["NETAMOUNT"].ValueType = System.Type.GetType("System.Decimal");
                dataGridViewMultipleSalesInput.Columns["PRICE"].ValueType = System.Type.GetType("System.Decimal");
                dataGridViewMultipleSalesInput.Columns["TOTDISCAMOUNT"].ValueType = System.Type.GetType("System.Decimal");
                dataGridViewMultipleSalesInput.Columns["FIXPRICE"].ValueType = System.Type.GetType("System.Decimal");

                //for (int i = 0; i < 7; i++)
                //{
                //    dataGridViewMultipleSalesInput.Columns[i].ReadOnly = true;
                //}

                //for (int i = 8; i <= dataGridViewMultipleSalesInput.Columns.Count - 1; i++)
                //{
                //    dataGridViewMultipleSalesInput.Columns[i].ReadOnly = true;
                //}

                //dataGridViewMultipleSalesInput.ReadOnly = true;

                for (int i = 0; i <= dataGridViewMultipleSalesInput.Columns.Count - 1; i++)
                {
                    dataGridViewMultipleSalesInput.Columns[i].ReadOnly = true;
                }

                this.dataGridViewMultipleSalesInput.Columns["QTY"].ReadOnly = false;

                this.dataGridViewMultipleSalesInput.Columns["PRICE"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dataGridViewMultipleSalesInput.Columns["TOTDISCAMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dataGridViewMultipleSalesInput.Columns["NETAMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.dataGridViewMultipleSalesInput.Columns["DISCRP"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


                foreach (DataGridViewRow row in dataGridViewMultipleSalesInput.Rows) // Loop over the rows.
                {
                    if (function.SelectPriceByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput(row.Cells["BARCODE"].Value.ToString(), GlobalVar.GlobalVarProfileID, GlobalVar.GlobalVarSite) == 1)
                    {
                        row.Cells["PRICE"].ReadOnly = false;
                    }
                }


                dataGridViewMultipleSalesInput.CurrentCell = dataGridViewMultipleSalesInput.Rows[DTMultipleSalesInputCopy.Rows.Count - 1].Cells["QTY"];



                dataGridViewMultipleSalesInput.BeginEdit(true);
            }
        }

        private void buttonSearchMultipleSalesInput_Click(object sender, EventArgs e)
        {
            initFromSalesInput = false;
            activePanel.Visible = false;
            activePanel = panelSalesSearch;
            activePanel.Visible = true;
            ComboBoxBrandCodeSalesSearch.Text = "";


        }

        private void buttonProcessMultipleSalesInput_Click(object sender, EventArgs e)
        {
            DialogResult DR = MessageBox.Show("Apa anda yakin mau memasukkan data-data diatas ?", "Confirm Insert",
                      MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (DR == DialogResult.Yes)
            {

                Decimal GrossAmountMultipleSalesInput = 0;
                foreach (DataGridViewRow row in dataGridViewMultipleSalesInput.Rows) // Loop over the rows.
                {
                    decimal try2;

                    itemMultipleSalesInput.NomorNota = function.getNextValSalesInput();
                    itemMultipleSalesInput.Article = row.Cells["ARTICLE"].Value.ToString();
                    itemMultipleSalesInput.Barcode = row.Cells["BARCODE"].Value.ToString();
                    itemMultipleSalesInput.Color = row.Cells["COLOR"].Value.ToString();
                    itemMultipleSalesInput.Description = row.Cells["DESCRIPTION"].Value.ToString();
                    itemMultipleSalesInput.Discount1 = decimal.Parse(row.Cells["DISC1"].Value.ToString());
                    itemMultipleSalesInput.Discount2 = decimal.Parse(row.Cells["DISC2"].Value.ToString());
                    itemMultipleSalesInput.Discount3 = decimal.Parse(row.Cells["DISC3"].Value.ToString());

                    decimal.TryParse(row.Cells["DISCRP"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                    itemMultipleSalesInput.DiscountRP = try2;

                    decimal.TryParse(row.Cells["PRICE"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                    itemMultipleSalesInput.Price = try2;


                    itemMultipleSalesInput.Size = row.Cells["UKURAN"].Value.ToString();
                    itemMultipleSalesInput.VariantID = int.Parse(row.Cells["VARIANTID"].Value.ToString());
                    itemMultipleSalesInput.Brand = (string)row.Cells["BRAND"].Value.ToString();
                    itemMultipleSalesInput.Store = row.Cells["STORE"].Value.ToString();
                    itemMultipleSalesInput.UserID = GlobalVar.GlobalVarUserID;
                    itemMultipleSalesInput.BrandName = (string)row.Cells["BRANDNAME"].Value.ToString();



                    if (string.IsNullOrWhiteSpace(row.Cells["FIXPRICE"].Value.ToString()))
                    {
                        itemMultipleSalesInput.FixPrice = 0;
                    }
                    else
                    {
                        decimal.TryParse(row.Cells["FIXPRICE"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                        itemMultipleSalesInput.FixPrice = try2;
                    }


                    if (string.IsNullOrWhiteSpace(row.Cells["TOTDISCAMOUNT"].Value.ToString()))
                    {
                        itemMultipleSalesInput.TotalDiscountAmount = 0;
                    }
                    else
                    {
                        decimal.TryParse(row.Cells["TOTDISCAMOUNT"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out try2);
                        itemMultipleSalesInput.TotalDiscountAmount = try2;
                    }


                    if (itemMultipleSalesInput.FixPrice == 0)
                    {
                        GrossAmountMultipleSalesInput = itemMultipleSalesInput.Price *
                                                    decimal.Parse(row.Cells["QTY"].Value.ToString());
                    }
                    else
                    {
                        GrossAmountMultipleSalesInput = itemMultipleSalesInput.FixPrice *
                                                    decimal.Parse(row.Cells["QTY"].Value.ToString());
                    }



                    try
                    {
                        function.insertSalesInput(itemMultipleSalesInput, row.Cells["QTY"].Value.ToString(),
                        decimal.Parse(row.Cells["NETAMOUNT"].Value.ToString()), GrossAmountMultipleSalesInput);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }


                }

                MessageBox.Show("Success Inserting Data", "Success Inserting Data",
                MessageBoxButtons.OK, MessageBoxIcon.Information);


                try
                {
                    StatusNota = "ORIGINAL";
                    printDocumentMultipleSalesInput.Print();
                    StatusNota = "--COPY--";
                    printDocumentMultipleSalesInput.Print();
                    StatusNota = "ORIGINAL";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }



                ClearMultipleSalesInput();
                textBoxBarcodeMultipleSalesInput.Focus();
            }


        }

        private void buttonCancelMultipleSalesInput_Click(object sender, EventArgs e)
        {
            DialogResult DR = MessageBox.Show("Apa anda yakin mau me-cancel pemasukkan data-data diatas ?", "Confirm Cancel",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (DR == DialogResult.Yes)
            {
                ClearMultipleSalesInput();
            }

            textBoxBarcodeMultipleSalesInput.Focus();
        }

        private void ClearMultipleSalesInput()
        {

            DTMultipleSalesInputCopy = new DataTable();
            DTMultipleSalesInputOri = new DataTable();
            dataGridViewMultipleSalesInput.DataSource = DTMultipleSalesInputCopy;
        }
        #endregion

        private void dataGridViewMultipleSalesInput_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            String a = dataGridViewMultipleSalesInput["QTY", e.RowIndex].Value.ToString();
            if ((e.Exception) is FormatException)
            {
                //DataGridView view = (DataGridView)sender;
                //view.Rows[e.RowIndex].ErrorText = "Value tidak bisa diubah ke decimal";
                //view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Value tidak bisa diubah ke decimal";

                //e.ThrowException = false;
                //var ctl = dataGridViewMultipleSalesInput.EditingControl as DataGridViewTextBoxEditingControl;
                dataGridViewMultipleSalesInput.EditingControl.Text = dataGridViewMultipleSalesInput["QTY", e.RowIndex].Value.ToString();
            }
        }



        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.I))
            {
                salesInputToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.V))
            {
                validasiToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.D))
            {
                displayToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.U))
            {
                userManagementToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.P))
            {
                profileManagementToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.H))
            {
                salesHistoryToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.C))
            {
                changePasswordToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.L))
            {
                printLabelToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.T))
            {
                settingLogoutTimerToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.M))
            {
                multipleSalesInputToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.B))
            {
                textBoxBarcodeMultipleSalesInput.Focus();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.R))
            {
                buttonProcessMultipleSalesInput.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.R))
            {
                buttonProcessMultipleSalesInput.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.R))
            {
                buttonProcessMultipleSalesInput.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.R))
            {
                buttonProcessMultipleSalesInput.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.N))
            {
                memoDiscountToolStripMenuItem.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.O))
            {
                uploadPromoToolStripMenuItem.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void comboBoxSite_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (DTMultipleSalesInputCopy.Rows.Count != 0)
            {
                DialogResult DR =
                    MessageBox.Show(
                        "Masih Ada Data di Multiple Sales Input, apakah anda yakin ingin me-delete data-data tersebut ?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (DR == DialogResult.No)
                {
                    comboBoxSite.SelectedIndex = prevSiteIndex;
                }
                else
                {
                    GlobalVar.GlobalVarSite = comboBoxSite.SelectedValue.ToString();
                    UpdateSiteDropDownListData();
                }
            }
            else
            {
                GlobalVar.GlobalVarSite = comboBoxSite.SelectedValue.ToString();
            }
        }



        #region Memo Discount
        private void memoDiscountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelMemoDiscount;
            activePanel.Visible = true;
        }

        //private void updateDataGridViewMemoDiscount()
        //{
        //    Item itemValidasi = new Item();

        //    itemValidasi.Brand = String.IsNullOrWhiteSpace(comboBoxBrandCodeValidasi.GetItemText(comboBoxBrandCodeValidasi.Text)) ? "" : comboBoxBrandCodeValidasi.GetItemText(comboBoxBrandCodeValidasi.SelectedItem).ToUpper();
        //    itemValidasi.BrandName = textBoxBrandNameValdiasi.Text == null ? "" : textBoxBrandNameValdiasi.Text.ToUpper();
        //    itemValidasi.Article = textBoxArticleValidasi.Text == null ? "" : textBoxArticleValidasi.Text.ToUpper();
        //    itemValidasi.Barcode = textBoxBarcodeValidasi.Text == null ? "" : textBoxBarcodeValidasi.Text.ToUpper();
        //    itemValidasi.Color = textBoxColorValidasi.Text == null ? "" : textBoxColorValidasi.Text.ToUpper();
        //    itemValidasi.Size = textBoxSizeValidasi.Text == null ? "" : textBoxSizeValidasi.Text.ToUpper();
        //    String NomorNota = textBoxNomorNotaValidasi.Text == null ? "" : textBoxNomorNotaValidasi.Text.ToUpper();


        //    DTSalesInputValidasi = function.SelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndNomorNotaReservedOnly(itemValidasi, NomorNota);
        //    dataGridViewSalesInputValidasi.DataSource = DTSalesInputValidasi;

        //    for (int i = 1; i <= dataGridViewSalesInputValidasi.Columns.Count - 1; i++)
        //    {
        //        dataGridViewSalesInputValidasi.Columns[i].ReadOnly = true;
        //    }



        //    var dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["PRICE"];

        //    if (dataGridViewColumn != null)
        //        dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        //    dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["GROSS AMOUNT"];
        //    if (dataGridViewColumn != null)
        //    {
        //        dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        //    }

        //    dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["TOTAL DISCOUNT AMOUNT"];
        //    if (dataGridViewColumn != null)
        //        dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        //    dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["NET AMOUNT"];
        //    if (dataGridViewColumn != null)
        //        dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        //    dataGridViewColumn = dataGridViewSalesInputValidasi.Columns["DISCOUNT RP"];
        //    if (dataGridViewColumn != null)
        //        dataGridViewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        //    //buttonValidate.Enabled = DTSalesInputValidasi.Rows.Count == 0 ? false : true;
        //}

        private void buttonSearchMemoDiscount_Click(object sender, EventArgs e)
        {
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;

            Application.DoEvents();


            memoHeader.PromoCode = String.IsNullOrWhiteSpace(textBoxPromoCodeMemoDiscount.Text) ? "" : textBoxPromoCodeMemoDiscount.Text.ToUpper();
            memoHeader.Description = String.IsNullOrWhiteSpace(textBoxDescriptionMemoDiscount.Text) ? "" : textBoxDescriptionMemoDiscount.Text.ToUpper();
            memoHeader.StartDate = dateTimePickerFromMemoDiscount.Value;
            memoHeader.EndDate = dateTimePickerToMemoDiscount.Value;

            try
            {
                DTMemoDiscountHeader = function.SelectMemoHeader(memoHeader);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }




            dataGridViewMemoDiscountHeader.DataSource = DTMemoDiscountHeader;
            this.dataGridViewMemoDiscountHeader.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            Cursor.Current = Cursors.Default;
        }

        private void dataGridViewMemoDiscountHeader_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
            }
            else
            {
                // Set cursor as hourglass
                Cursor.Current = Cursors.WaitCursor;

                Application.DoEvents();

                int rowIndex = e.RowIndex;
                DataGridViewRow row = dataGridViewMemoDiscountHeader.Rows[rowIndex];

                try
                {
                    DTMemoDiscountDetail = function.SelectMemoDetail(row.Cells[0].Value.ToString().ToUpper(), memoHeader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }



                dataGridViewMemoDiscountDetail.DataSource = DTMemoDiscountDetail;
                this.dataGridViewMemoDiscountDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;


                Cursor.Current = Cursors.Default;
            }

        }

        private void buttonPrintMemoDiscount_Click(object sender, EventArgs e)
        {
            if (dataGridViewMemoDiscountHeader.RowCount == 0)
            {
                MessageBox.Show("Data Grid Kosong, Tolong gunakan fungsi search dahulu untuk memunculkan data", "Data Grid Kosong",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                saveFileDialogMemoDiscount.ShowDialog();
            }
        }

        private void saveFileDialogMemoDiscount_FileOk(object sender, CancelEventArgs e)
        {

            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;

            Application.DoEvents();


            string fileName = saveFileDialogMemoDiscount.FileName;



            var titleFont = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD);
            //var endingMessageFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL);

            //DateTime date = new DateTime();

            try
            {


                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //Create a new PDF document setting the size to A4
                    using (Document doc = new Document(PageSize.A4, 50, 50, 25, 50))
                    {
                        //Bind the PDF document to the FileStream using an iTextSharp PdfWriter
                        using (PdfWriter w = PdfWriter.GetInstance(doc, fs))
                        {
                            //Open the document for writing
                            doc.Open();

                            string imagePath = function.getImageFooterMemoDiscount();

                            doc.Add(new Paragraph(" "));

                            iTextSharp.text.Image imageHeader = iTextSharp.text.Image.GetInstance(imagePath);

                            _eventsMemoDiscount events = new _eventsMemoDiscount();
                            w.PageEvent = events;

                            events.ImageHeader = imageHeader;



                            Paragraph paragraph = new Paragraph("Memo Discount Report", titleFont);
                            paragraph.Alignment = Element.ALIGN_CENTER;
                            paragraph.SpacingBefore = 10;
                            doc.Add(paragraph);


                            paragraph = new Paragraph("Store : " + comboBoxSite.SelectedValue + " : " + comboBoxSite.GetItemText(comboBoxSite.SelectedItem), subTitleFont);
                            paragraph.SpacingBefore = 10;
                            doc.Add(paragraph);


                            paragraph = new Paragraph("Filter Data", subTitleFont);
                            paragraph.SpacingBefore = 10;
                            doc.Add(paragraph);

                            var HeaderInfo = new PdfPTable(6);
                            HeaderInfo.HorizontalAlignment = 0;
                            HeaderInfo.SpacingBefore = 10;

                            //HeaderInfo.SpacingAfter = 10;
                            HeaderInfo.DefaultCell.Border = 0;
                            //HeaderInfo.SetWidths(new int[] { 1, 4 });




                            HeaderInfo.AddCell(new Phrase("Date From", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(":", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(memoHeader.StartDate.ToString("d MMM yyyy"), boldTableFont));
                            HeaderInfo.AddCell(new Phrase("Date To", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(":", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(memoHeader.EndDate.ToString("d MMM yyyy"), boldTableFont));
                            HeaderInfo.AddCell(new Phrase("Promo Code", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(":", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(memoHeader.PromoCode, boldTableFont));
                            HeaderInfo.AddCell(new Phrase("Description", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(":", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(memoHeader.Description, boldTableFont));

                            doc.Add(HeaderInfo);

                            doc.Add(new Paragraph(""));
                            doc.Add(new Paragraph(""));
                            doc.Add(new Paragraph("Total Promo : " + dataGridViewMemoDiscountHeader.RowCount, subTitleFont));
                            doc.Add(new Paragraph(""));
                            doc.Add(new Paragraph(""));



                            PdfPCell cell = new PdfPCell();
                            PdfPCell cellHeader = new PdfPCell();

                            Phrase Title = new Phrase("Title");


                            int PageNumber = 1;

                            foreach (DataRow row in DTMemoDiscountHeader.Rows) // Loop over the rows.
                            {

                                events.rowHeader = row;

                                cellHeader = new PdfPCell();
                                cellHeader.BackgroundColor = new BaseColor(Color.Gainsboro);
                                var HeaderTable = new PdfPTable(21);
                                HeaderTable.HorizontalAlignment = 0;
                                HeaderTable.SpacingBefore = SPACINGAFTER;
                                HeaderTable.TotalWidth = 500f;
                                HeaderTable.LockedWidth = true;
                                //HeaderTable.SpacingAfter = 10;
                                HeaderTable.DefaultCell.Border = 0;
                                //HeaderTable.SetWidths(new int[] {1, 4});
                                //HeaderTable.SetWidths(new float[] { 2f, 6f, 6f, 3f, 5f, 8f, 5f, 5f, 5f, 5f, 5f });


                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = new Phrase("Promo", boldTableFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = new Phrase("Start Date", boldTableFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = new Phrase("End Date", boldTableFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = new Phrase("Start Time", boldTableFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = new Phrase("End Time", boldTableFont);
                                HeaderTable.AddCell(cellHeader);


                                cellHeader.Colspan = 6;
                                cellHeader.Phrase = new Phrase("Desc", boldTableFont);
                                HeaderTable.AddCell(cellHeader);


                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = new Phrase("Type", boldTableFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = new Phrase("Disc 1", boldTableFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = new Phrase("Disc 2", boldTableFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = new Phrase("Disc 3", boldTableFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = new Phrase("Disc RP", boldTableFont);
                                HeaderTable.AddCell(cellHeader);


                                cellHeader = new PdfPCell();

                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = (new Phrase(row["PROMOCODE"].ToString(), bodyFont));
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = (new Phrase(row["STARTDATE"].ToString(), bodyFont));
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = (new Phrase(row["ENDDATE"].ToString(), bodyFont));
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = (new Phrase(row["STARTTIME"].ToString(), bodyFont));
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = (new Phrase(row["ENDTIME"].ToString(), bodyFont));
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 6;
                                cellHeader.Phrase = new Phrase(row["DESCRIPTION"].ToString(), bodyFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = new Phrase(row["DISC_TYPE"].ToString(), bodyFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = new Phrase(row["DISCOUNT1"].ToString(), bodyFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = new Phrase(row["DISCOUNT2"].ToString(), bodyFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 1;
                                cellHeader.Phrase = new Phrase(row["DISCOUNT3"].ToString(), bodyFont);
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 2;
                                cellHeader.Phrase = new Phrase(row["DISCOUNTRP"].ToString(), bodyFont);
                                HeaderTable.AddCell(cellHeader);

                                doc.Add(HeaderTable);

                                try
                                {
                                    DTMemoDiscountDetail = function.SelectMemoDetail(row["PromoCode"].ToString(), memoHeader);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Error Occured",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    logger.Error(ex.Message);
                                }




                                if (DTMemoDiscountDetail.Rows.Count != 0)
                                {


                                    var DetailTable = new PdfPTable(11);
                                    DetailTable.HorizontalAlignment = 0;
                                    DetailTable.SpacingBefore = 10;
                                    DetailTable.TotalWidth = 500;
                                    DetailTable.LockedWidth = true;
                                    //DetailTable.SpacingAfter = SPACINGAFTER;
                                    DetailTable.DefaultCell.Border = 0;

                                    DetailTable.AddCell("");

                                    cellHeader = new PdfPCell();
                                    cellHeader.BackgroundColor = new BaseColor(Color.Gainsboro);
                                    cellHeader.Colspan = 2;
                                    cellHeader.Phrase = new Phrase("Item ID", boldTableFont);
                                    DetailTable.AddCell(cellHeader);



                                    cellHeader.Colspan = 1;
                                    cellHeader.Phrase = new Phrase("SV", boldTableFont);
                                    DetailTable.AddCell(cellHeader);


                                    cellHeader.Phrase = new Phrase("Brand", boldTableFont);
                                    DetailTable.AddCell(cellHeader);
                                    cellHeader.Colspan = 7;
                                    Phrase phrase = new Phrase();
                                    cellHeader.Phrase = new Phrase("Description", boldTableFont);
                                    DetailTable.AddCell(cellHeader);

                                    doc.Add(DetailTable);

                                    foreach (DataRow rowDetail in DTMemoDiscountDetail.Rows) // Loop over the rows.
                                    {
                                        DetailTable = new PdfPTable(11);
                                        DetailTable.HorizontalAlignment = 0;
                                        DetailTable.TotalWidth = 500;
                                        DetailTable.LockedWidth = true;
                                        //DetailTable.SpacingAfter = SPACINGAFTER;
                                        DetailTable.DefaultCell.Border = 0;

                                        DetailTable.AddCell("");


                                        cell.Colspan = 2;
                                        cell.Phrase = new Phrase(rowDetail["Item"].ToString(), bodyFont);
                                        DetailTable.AddCell(cell);


                                        cell.Colspan = 1;
                                        cell.Phrase = new Phrase(rowDetail["SalesVariant"].ToString(), bodyFont);
                                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        DetailTable.AddCell(cell);


                                        DetailTable.AddCell(new PdfPCell(new Phrase(rowDetail["Brand"].ToString(), bodyFont)));

                                        cell = new PdfPCell();
                                        cell.Colspan = 7;
                                        cell.Phrase = new Phrase(rowDetail["Description"].ToString(), bodyFont);
                                        DetailTable.AddCell(cell);
                                        doc.Add(DetailTable);
                                    }
                                }
                            }
                            //Close our document
                            doc.Close();
                        }
                    }
                }
                MessageBox.Show("file telah berhasil di generate", "Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Cursor.Current = Cursors.Default;
        }

        #endregion




        #region Login
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxLoginUserID.Text) || String.IsNullOrWhiteSpace(textBoxLoginPassword.Text))
            {
                MessageBox.Show("TextBox UserID atau Password kosong, tolong is terlebih dahulu", "TextBox Kosong",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxPassword.Text = null;
            }
            else
            {
                loginEvent();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxLoginPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                if (String.IsNullOrWhiteSpace(textBoxLoginUserID.Text) || String.IsNullOrWhiteSpace(textBoxLoginPassword.Text))
                {
                    MessageBox.Show("TextBox UserID atau Password kosong, tolong is terlebih dahulu", "TextBox Kosong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxPassword.Text = null;
                }
                else
                {
                    loginEvent();
                }
            }
        }

        private void resetLogin()
        {
            textBoxLoginUserID.Text = null;
            textBoxLoginPassword.Text = null;
            textBoxLoginUserID.Focus();
        }

        private void loginEvent()
        {

            try
            {
                user = function.Login(textBoxLoginUserID.Text, textBoxLoginPassword.Text);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.Message,
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }


            if (user == null)
            {
                MessageBox.Show("Null value, tolong cek koneksi, apakah database sudah dihidupkan apa tidak", "Unknown Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                switch ((int)user.Status)
                {
                    case 0:
                        MessageBox.Show("Tidak dapat menemukan User, Tolong periksa Username and Password", "Tidak dapat menemukan user",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        resetLogin();
                        break;
                    case 1:
                        //Successfull login
                        GlobalVar.GlobalVarUserID = user.UserID;
                        GlobalVar.GlobalVarPassword = user.Password;
                        GlobalVar.GlobalVarProfileID = user.ProfileID;
                        GlobalVar.GlobalVarUsername = user.Username;
                        textBoxLoginUserID.Text = "";
                        textBoxLoginPassword.Text = "";
                        textBoxLoginUserID.Focus();

                        logger.Debug("get the data from database");
                        DTSiteByProfile = function.GetSiteDataByProfileID(GlobalVar.GlobalVarProfileID);

                        if (DTSiteByProfile.Rows.Count == 0)
                        {
                            logger.Error("DTSiteByProfile row count is 0");
                            MessageBox.Show(
                                "Data Site Kosong, pastikan data Site ada di Database, dan Profile mempunyai akses ke site tersebut",
                                "Data Site Kosong",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                        else
                        {
                            EnableMenuAndItems();

                            activePanel.Visible = false;
                            activePanel = panelStartingScreen;
                            activePanel.Visible = true;
                        }

                        break;
                    case 2:
                        MessageBox.Show("User dalam keadaan Frozen, tolong hubungi admin", "User dalam keadaan frozen",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        resetLogin();
                        break;
                    case 3:
                        MessageBox.Show("User sudah di delete, Tolong pakai user yang lain", "User sudah di delete",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        resetLogin();
                        break;
                    default:
                        MessageBox.Show("Tolong hubungi Admin", "Unknown error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        resetLogin();
                        break;
                }
            }


        }

        #endregion

        #region Upload Promo

        private void buttonSearchUploadPromo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxSeqNumUploadPromo.Text))
            {
                MessageBox.Show("Text Box Sequence Number is Empty", "Empty Data",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                bool AllData = false;

                if (comboBoxStatusUploadPromo.GetItemText(comboBoxStatusUploadPromo.Text) == "All")
                {
                    AllData = true;
                }
                else if (comboBoxStatusUploadPromo.GetItemText(comboBoxStatusUploadPromo.Text) == "Error")
                {
                    AllData = false;
                }



                try
                {
                    //Upload DT based on the data from textbox and status
                    DTUploadPromo = function.SelectUploadPromoBySeqNumberAndStatus(textBoxSeqNumUploadPromo.Text, AllData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }





                dataGridViewUploadPromo.DataSource = DTUploadPromo;

                if (dataGridViewUploadPromo.RowCount == 0)
                {
                    MessageBox.Show("Data Grid Kosong", "Data Grid Kosong",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.dataGridViewUploadPromo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    this.dataGridViewUploadPromo.Columns["HSP"].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                }
            }

        }

        private void buttonChooseFile_Click(object sender, EventArgs e)
        {
            openFileDialogUploadPromo.ShowDialog();

        }

        private void openFileDialogUploadPromo_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = openFileDialogUploadPromo.FileName;

            try
            {
                var excel = new ExcelQueryFactory(fileName);
                var allRows = excel.WorksheetNoHeader();
                excel.AddMapping<UploadPromo>(x => x.ActionCode, "ACTION CODE");
                excel.AddMapping<UploadPromo>(x => x.Article, "ARTICLE CODE");
                excel.AddMapping<UploadPromo>(x => x.HSP, "HSP");
                excel.AddMapping<UploadPromo>(x => x.Number, "PROMO NUMBER");
                excel.AddMapping<UploadPromo>(x => x.Reference, "PROMO REFERENCE");
                excel.AddMapping<UploadPromo>(x => x.SU, "SU");
                excel.AddMapping<UploadPromo>(x => x.SUDescription, "SU DESCRIPTION");
                excel.AddMapping<UploadPromo>(x => x.TILLCode, "TILLCODE");

                var uploadPromoContainer = from c in excel.Worksheet<UploadPromo>()
                                           where c.Number != ""
                                           select c;

                try
                {
                    seqNumber = function.getNextValUploadPromo();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

                //try
                //{
                //    function.DeleteOldUploadPromo();
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message, "Error Occured",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    logger.Error(ex.Message);
                //}


                foreach (var u in uploadPromoContainer) // Loop over the rows.
                {
                    u.Sequence = seqNumber;
                    try
                    {
                        ErrorString = function.insertUploadPromo(u);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }
                }
                MessageBox.Show("Success Uploading Data, Reference Number anda adalah : " + seqNumber, "Success",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);

                textBoxSeqNumUploadPromo.Text = seqNumber;

                int numOfErrors = 1;

                if (seqNumber == "0")
                {
                    MessageBox.Show("Sequence Number is 0", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    bool result = false;

                    try
                    {
                        result = Int32.TryParse(function.validateUploadPromo(seqNumber), out numOfErrors);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }


                    if (result)
                    {
                        if (numOfErrors == 0)
                        {
                            MessageBox.Show("All data are correct", "No Data Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            try
                            {
                                DTUploadPromo = function.SelectUploadPromoBySeqNumber(seqNumber);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error Occured",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                logger.Error(ex.Message);
                            }

                            //MessageBox.Show("There are "+numOfErrors+" Errors Occured", "Error Occured",
                            //    MessageBoxButtons.OK, MessageBoxIcon.Error);

                            dataGridViewUploadPromo.DataSource = DTUploadPromo;

                            this.dataGridViewUploadPromo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                            this.dataGridViewUploadPromo.Columns["HSP"].DefaultCellStyle.Alignment =
                                DataGridViewContentAlignment.MiddleRight;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot Parse Data", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }



        }
        #endregion



        private void printDocumentMultipleSalesInput_PrintPage(object sender, PrintPageEventArgs e)
        {
            PrintNotaMultipleSalesInput(StatusNota, e);
        }

        private void MyForm_CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            BackgroundWorker worker = sender as BackgroundWorker;


            for (int i = 1; i <= TotalData; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                }
                else
                {
                    worker.ReportProgress(i);
                    System.Threading.Thread.Sleep(500);
                }
            }



        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //// Show the progress in main form (GUI)
            //labelResult.Text = (e.ProgressPercentage.ToString() + "%");
            // Pass the progress to AlertForm label and progressbar
            progressBar.Message = "In progress, please wait... fetching " + e.ProgressPercentage.ToString() + " data out of : " + TotalData + " data";
            //progressBar.Message = "In progress, please wait...  " + e.ProgressPercentage.ToString() + " %";
            progressBar.ProgressValue = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (e.Cancelled == true)
            //{
            //    labelResult.Text = "Canceled!";
            //}
            //else if (e.Error != null)
            //{
            //    labelResult.Text = "Error: " + e.Error.Message;
            //}
            //else
            //{
            //    labelResult.Text = "Done!";
            //}
            // Close the AlertForm



            progressBar.Close();


        }






        #region Generate Label STCK
        
        private void buttonProcessLabelSTCK_Click(object sender, EventArgs e)
        {
            FilePath = ConfigurationManager.AppSettings["DefaultPrintLabelFilePath"];
            FileName = ConfigurationManager.AppSettings["FileNameSTCK"];


            try
            {
                DTPrintLabelSTCK = function.GetSTCKBySite(GlobalVar.GlobalVarSite);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            try
            {
                FileStream fs = new FileStream(FilePath + "\\" + FileName + ".txt", FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);

                int i = 0;
                logger.Debug("Writing file to : " + FilePath + "\\" + FileName + ".txt");



                foreach (DataRow row2 in DTPrintLabelSTCK.Rows)
                {
                    object[] array = row2.ItemArray;

                    for (i = 0; i < array.Length - 1; i++)
                    {
                        writer.Write(array[i].ToString() + "|");
                    }
                    writer.Write(array[i].ToString());
                    writer.Write("\n");

                }

                writer.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }




            MessageBox.Show("Success Me-generate file dengan nama : " + FileName + " dengan lokasi : " + FilePath, "Success Generating File",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion


        #region UploadInventory
        private void buttonUploadInventory_Click(object sender, EventArgs e)
        {
            openFileDialogUploadInventory.ShowDialog();
        }

        private void openFileDialogUploadInventory_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = openFileDialogUploadInventory.FileName;

            try
            {
                var excel = new ExcelQueryFactory(fileName);
                var allRows = excel.WorksheetNoHeader();
                excel.AddMapping<UploadInventory>(x => x.IVFCEXINV, "IVFCEXINV");
                excel.AddMapping<UploadInventory>(x => x.IVFSITE, "IVFSITE");
                excel.AddMapping<UploadInventory>(x => x.IVFTINV, "IVFTINV");
                excel.AddMapping<UploadInventory>(x => x.IVFLIBL, "IVFLIBL");
                excel.AddMapping<UploadInventory>(x => x.IVFDINV, "IVFDINV");
                excel.AddMapping<UploadInventory>(x => x.IVFTPOS, "IVFTPOS");
                excel.AddMapping<UploadInventory>(x => x.IVFCODE, "IVFCODE");
                excel.AddMapping<UploadInventory>(x => x.IVFQTER, "IVFQTER");
                excel.AddMapping<UploadInventory>(x => x.IVFPDSINV, "IVFPDSINV");
                excel.AddMapping<UploadInventory>(x => x.IVFEMPL, "IVFEMPL");
                excel.AddMapping<UploadInventory>(x => x.IVFNORDRE, "IVFNORDRE");
                excel.AddMapping<UploadInventory>(x => x.IVFORIGCEXINV, "IVFORIGCEXINV");
                excel.AddMapping<UploadInventory>(x => x.IVFLGFI, "IVFLGFI");
                excel.AddMapping<UploadInventory>(x => x.IVFTRT, "IVFTRT");
                excel.AddMapping<UploadInventory>(x => x.IVFDTRT, "IVFDTRT");
                excel.AddMapping<UploadInventory>(x => x.IVFDCRE, "IVFDCRE");
                excel.AddMapping<UploadInventory>(x => x.IVFDMAJ, "IVFDMAJ");
                excel.AddMapping<UploadInventory>(x => x.IVFUTIL, "IVFUTIL");
                excel.AddMapping<UploadInventory>(x => x.IVFFICH, "IVFFICH");
                excel.AddMapping<UploadInventory>(x => x.IVFNLIG, "IVFNLIG");
                excel.AddMapping<UploadInventory>(x => x.IVFNERR, "IVFNERR");
                excel.AddMapping<UploadInventory>(x => x.IVFMESS, "IVFMESS");
                excel.AddMapping<UploadInventory>(x => x.IVFCEXV, "IVFCEXV");
                excel.AddMapping<UploadInventory>(x => x.IVFPV, "IVFPV");
                excel.AddMapping<UploadInventory>(x => x.IVFIDSTR, "IVFIDSTR");
                excel.AddMapping<UploadInventory>(x => x.IVFNODE, "IVFNODE");
                excel.AddMapping<UploadInventory>(x => x.IVFNLIS, "IVFNLIS");
                excel.AddMapping<UploadInventory>(x => x.IVFCACT, "IVFCACT");
                excel.AddMapping<UploadInventory>(x => x.IVFGRPS, "IVFGRPS");
                excel.AddMapping<UploadInventory>(x => x.IVFMODE, "IVFMODE");
                excel.AddMapping<UploadInventory>(x => x.IVFCEXVL, "IVFCEXVL");
                excel.AddMapping<UploadInventory>(x => x.IVFNPORT, "IVFNPORT");
                excel.AddMapping<UploadInventory>(x => x.IVFDCPPREV, "IVFDCPPREV");

                var uploadInventContainer = from c in excel.Worksheet<UploadInventory>("intinv")
                                            select c;

                var firstOrDefault = uploadInventContainer.FirstOrDefault();


                if (firstOrDefault != null)
                {
                    //Delete first with the corresponding IVFCEXINV
                    function.deleteFromINTINV(firstOrDefault.IVFCEXINV);

                    foreach (var u in uploadInventContainer) // Loop over the rows.
                    {
                        try
                        {
                            ErrorString = function.processUploadInvent(u);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            logger.Error(ex.Message);
                        }
                    }

                    MessageBox.Show("Success Uploading Data", "Success",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                }





            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
        }
        #endregion




        #region Stock Take

        private void buttonSearchInvNumber_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxInvNumStockTakeScan.Text))
            {
                MessageBox.Show("Inventory Number is empty, please insert data into inventory number textbox", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (function.CheckInvNumLocal(textBoxInvNumStockTakeScan.Text, GlobalVar.GlobalVarSite) == false)
                {
                    DialogResult DR = MessageBox.Show("Inventory Number does not exists, do you want to download ? ",
                        "Inventory Number does not exists",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (DR == DialogResult.Yes)
                    {
                        if (function.CheckInvNumCentral(textBoxInvNumStockTakeScan.Text, GlobalVar.GlobalVarSite) ==
                            false)
                        {
                            MessageBox.Show(
                                "Inventory Number cannot be found on the server, please insert the correct one",
                                "Inventory Number could not be found",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            Boolean SuccessHeader = function.DownloadInventoryHeader(textBoxInvNumStockTakeScan.Text,
                                GlobalVar.GlobalVarSite);
                            Boolean SuccessDetail = function.DownloadInventoryDetail(textBoxInvNumStockTakeScan.Text,
                                GlobalVar.GlobalVarSite);

                            if (!SuccessHeader || !SuccessDetail)
                            {
                                MessageBox.Show("Failed Download Header or Detail Data", "Failed Download",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show("Success Download Data", "Success Download",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                        }
                    }


                }

            }
        }

        private void textBoxBarcodeStockTake_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                if (String.IsNullOrWhiteSpace(textBoxInvNumStockTakeScan.Text))
                {
                    MessageBox.Show("Inventory Number is empty, please insert data into inventory number textbox", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (String.IsNullOrWhiteSpace(textBoxLocationStockTakeScan.Text))
                {
                    MessageBox.Show("Location is empty, please insert data into Location textbox", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!function.CheckStockTake(textBoxInvNumStockTakeScan.Text, GlobalVar.GlobalVarSite,
                        textBoxBarcodeStockTake.Text))
                    {
                        MessageBox.Show("Cannot find the barcode", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {

                        DTStockTakeOrigin = function.SelectDetailStockTake(GlobalVar.GlobalVarSite,
                        textBoxBarcodeStockTake.Text, textBoxInvNumStockTakeScan.Text);



                        if (checkBoxQtyInput.Checked)
                        {

                            textBoxQuantityStockTake.Focus();

                        }
                        else
                        {

                            //var rowsWithData = from row in DTStockTakeOrigin.AsEnumerable()
                            //                      where row.Field<string>("") == accountNumber // replace `<string>` with the right type
                            //                      select row;
                            //foreach (DataRow row in rowsWithAccount)
                            //    row.SetField("Balance", newBalance);

                            StockTake stockTake = new StockTake();

                            stockTake.Barcode = DTStockTakeOrigin.Rows[0]["BARCODE"].ToString();
                            stockTake.Qty = "1";
                            stockTake.Description = DTStockTakeOrigin.Rows[0]["DESCRIPTION"].ToString();
                            stockTake.InvDate = function.GetInvDateStockTake(textBoxInvNumStockTakeScan.Text,
                                GlobalVar.GlobalVarSite);
                            stockTake.Type = function.GetInvTypeStockTake(textBoxInvNumStockTakeScan.Text,
                                GlobalVar.GlobalVarSite);

                            function.insertStockTake(GlobalVar.GlobalVarSite, textBoxInvNumStockTakeScan.Text, stockTake,
                                System.Environment.MachineName, GlobalVar.GlobalVarUserID, textBoxLocationStockTakeScan.Text);


                            DTStockTakeCopy.Merge(DTStockTakeOrigin);

                            DTStockTakeOrigin = new DataTable();
                            dataGridStockTakeScan.DataSource = DTStockTakeCopy;



                            UpdateStockTakeScanGridView();

                            textBoxBarcodeStockTake.Text = String.Empty;
                            textBoxBarcodeStockTake.Focus();
                        }

                    }

                }
            }
        }

        private void UpdateStockTakeScanGridView()
        {
            dataGridStockTakeScan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridStockTakeScan.Columns["BARCODE"].MinimumWidth = 100;
            dataGridStockTakeScan.Columns["DESCRIPTION"].MinimumWidth = 200;
            dataGridStockTakeScan.Columns["QTY"].MinimumWidth = 50;

            for (int i = 0; i <= dataGridStockTakeScan.Columns.Count - 1; i++)
            {
                dataGridStockTakeScan.Columns[i].ReadOnly = true;
            }
        }

        private void textBoxQuantityStockTake_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

        }

        private void buttonUpdateQuantityStockTake_Click(object sender, EventArgs e)
        {
            textBoxInvNumStockTakeScan.Text = String.Empty;
            textBoxLocationStockTakeScan.Text = String.Empty;
            textBoxBarcodeStockTake.Text = String.Empty;
            textBoxQuantityStockTake.Text = String.Empty;
        }

        private void textBoxQuantityStockTake_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                if (checkBoxQtyInput.Checked)
                {
                    if (!function.CheckStockTake(textBoxInvNumStockTakeScan.Text, GlobalVar.GlobalVarSite,
                        textBoxBarcodeStockTake.Text))
                    {
                        MessageBox.Show("Cannot find the barcode", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        DTStockTakeOrigin = new DataTable();
                        DTStockTakeOrigin = function.SelectDetailStockTake(GlobalVar.GlobalVarSite,
                            textBoxBarcodeStockTake.Text, textBoxInvNumStockTakeScan.Text);



                        DTStockTakeOrigin.Columns["QTY"].ReadOnly = false;



                        DTStockTakeOrigin.Rows[0]["QTY"] = textBoxQuantityStockTake.Text;

                        StockTake stockTake = new StockTake();

                        stockTake.Barcode = DTStockTakeOrigin.Rows[0]["BARCODE"].ToString();
                        stockTake.Qty = DTStockTakeOrigin.Rows[0]["QTY"].ToString();
                        stockTake.Description = DTStockTakeOrigin.Rows[0]["DESCRIPTION"].ToString();
                        stockTake.InvDate = function.GetInvDateStockTake(textBoxInvNumStockTakeScan.Text,
                               GlobalVar.GlobalVarSite);
                        stockTake.Type = function.GetInvTypeStockTake(textBoxInvNumStockTakeScan.Text,
                               GlobalVar.GlobalVarSite);

                        function.insertStockTake(GlobalVar.GlobalVarSite, textBoxInvNumStockTakeScan.Text, stockTake,
                            System.Environment.MachineName, GlobalVar.GlobalVarUserID, textBoxLocationStockTakeScan.Text);


                        DTStockTakeCopy.Merge(DTStockTakeOrigin);
                        dataGridStockTakeScan.DataSource = DTStockTakeCopy;
                        DTStockTakeOrigin = new DataTable();

                        UpdateStockTakeScanGridView();

                        textBoxBarcodeStockTake.Text = String.Empty;
                        textBoxQuantityStockTake.Text = String.Empty;
                        textBoxBarcodeStockTake.Focus();
                    }

                }
            }
        }

        private void buttonSearchStockTakeReport_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxInvNumStockTakeReport.Text) || String.IsNullOrWhiteSpace(textBoxLocationStockTakeReport.Text))
            {
                MessageBox.Show("Location dan Inventory Number tidak bisa kosong", "Data Kosong",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StockTake stockTake = new StockTake();

                stockTake.Barcode = textBoxBarcodeStockTakeReport.Text;
                String Location = textBoxLocationStockTakeReport.Text;
                String InvNum = textBoxInvNumStockTakeReport.Text;
                String HostName = textBoxHostNameStockTakeReport.Text;
                DateTime From = dateTimePickerFromStockTakeReport.Value;
                DateTime To = dateTimePickerToStockTakeReport.Value;

                DTStockTakeReport = function.SelectStockTake(stockTake, GlobalVar.GlobalVarSite, HostName, From, To,
                    Location, InvNum, false);

                dataGridViewStockTakeReport.DataSource = DTStockTakeReport;
                //dataGridViewStockTakeReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                dataGridViewStockTakeReport.Columns["DESCRIPTION"].MinimumWidth = 200;
                dataGridViewStockTakeReport.Columns["BARCODE"].MinimumWidth = 100;
                dataGridViewStockTakeReport.Columns["LOCATION"].MinimumWidth = 100;
                dataGridViewStockTakeReport.Columns["INVENTORY NUMBER"].MinimumWidth = 100;
                dataGridViewStockTakeReport.Columns["CREATEDBY"].MinimumWidth = 100;
                dataGridViewStockTakeReport.Columns["HOSTNAME"].MinimumWidth = 100;
                dataGridViewStockTakeReport.Columns["CREATEDDATE"].MinimumWidth = 150;
                dataGridViewStockTakeReport.Columns["TIME"].MinimumWidth = 100;
            }



        }

        private void buttonUploadExcelStockTakeReport_Click(object sender, EventArgs e)
        {
            if (dataGridViewStockTakeReport.RowCount == 0)
            {
                MessageBox.Show("Data Grid Kosong, Tolong gunakan fungsi search dahulu untuk memunculkan data", "Data Grid Kosong",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                copyAllDataGridViewtoClipboard(dataGridViewStockTakeReport);
                ExportToExcelFromClipboard();
            }
        }

        private void buttonUploadStockTakeReport_Click(object sender, EventArgs e)
        {
            StockTake stockTake = new StockTake();

            stockTake.Barcode = textBoxBarcodeStockTakeReport.Text;
            String Location = textBoxLocationStockTakeReport.Text;
            String InvNum = textBoxInvNumStockTakeReport.Text;
            String HostName = textBoxHostNameStockTakeReport.Text;
            DateTime From = dateTimePickerFromStockTakeReport.Value;
            DateTime To = dateTimePickerToStockTakeReport.Value;

            ErrorString = function.UploadStockTakeToUSSCServer(stockTake, GlobalVar.GlobalVarSite, HostName, From, To,
                Location, InvNum);
            function.Commit(true);

            if (ErrorString == "Success")
            {

                MessageBox.Show("Success Upload to USSC Server ", "Success",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                ErrorString = function.DeleteLocalStockTake(stockTake, GlobalVar.GlobalVarSite, HostName, From,
                    To, Location, InvNum);
                function.Commit(true);
                DTStockTakeReport = new DataTable();
                dataGridViewStockTakeReport.DataSource = DTStockTakeReport;
            }
            else if (ErrorString == "Failed")
            {
                MessageBox.Show("Failed to execute Upload to USSC Server ", "An Error Occured",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonUploadtoExcelStockTakeUpload_Click(object sender, EventArgs e)
        {
            if (dataGridViewStockTakeUpload.RowCount == 0)
            {
                MessageBox.Show("Data Grid Kosong, Tolong gunakan fungsi search dahulu untuk memunculkan data", "Data Grid Kosong",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                copyAllDataGridViewtoClipboard(dataGridViewStockTakeUpload);
                ExportToExcelFromClipboard();
            }
        }

        private void buttonUploadtoGOLDServer_Click(object sender, EventArgs e)
        {
            StockTake stockTake = new StockTake();

            stockTake.Barcode = textBoxBarcodeStockTakeUpload.Text;
            String Location = textBoxLocationStockTakeUpload.Text;
            String InvNum = textBoxInvNumStockTakeUpload.Text;
            String HostName = textBoxHostnameStockTakeUpload.Text;
            DateTime From = dateTimePickerFromStockTakeUpload.Value;
            DateTime To = dateTimePickerToStockTakeUpload.Value;

            ErrorString = function.UploadToINTINVFromUSSCServer(stockTake, GlobalVar.GlobalVarSite, HostName, From,
                     To, Location, InvNum);
            function.Commit(false);



            if (ErrorString == "Success")
            {
                MessageBox.Show("Success Upload to GOLD Server ", "Success",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                ErrorString = function.UpdateUSSCServerFlagAfterUpload(stockTake, GlobalVar.GlobalVarSite, HostName, From,
                To, Location, InvNum);
                function.Commit(false);
                DTStockTakeUpload = new DataTable();
                dataGridViewStockTakeUpload.DataSource = DTStockTakeUpload;
            }
            else if (ErrorString == "Failed")
            {
                MessageBox.Show("Failed to execute Upload to GOLD Server ", "An Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSearchStockTakeUpload_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrWhiteSpace(textBoxInvNumStockTakeReport.Text) ||
                String.IsNullOrWhiteSpace(textBoxLocationStockTakeReport.Text))
            {
                MessageBox.Show("Location dan Inventory Number tidak bisa kosong", "Data Kosong",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StockTake stockTake = new StockTake();

                stockTake.Barcode = textBoxBarcodeStockTakeUpload.Text;
                String Location = textBoxLocationStockTakeUpload.Text;
                String InvNum = textBoxInvNumStockTakeUpload.Text;
                String HostName = textBoxHostnameStockTakeUpload.Text;
                DateTime From = dateTimePickerFromStockTakeUpload.Value;
                DateTime To = dateTimePickerToStockTakeUpload.Value;

                DTStockTakeUpload = function.SelectStockTake(stockTake, GlobalVar.GlobalVarSite, HostName, From, To,
                    Location, InvNum, true);

                dataGridViewStockTakeUpload.DataSource = DTStockTakeUpload;
                //dataGridViewStockTakeReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                dataGridViewStockTakeUpload.Columns["DESCRIPTION"].MinimumWidth = 200;
                dataGridViewStockTakeUpload.Columns["BARCODE"].MinimumWidth = 100;
                dataGridViewStockTakeUpload.Columns["LOCATION"].MinimumWidth = 100;
                dataGridViewStockTakeUpload.Columns["INVENTORY NUMBER"].MinimumWidth = 100;
                dataGridViewStockTakeUpload.Columns["CREATEDBY"].MinimumWidth = 100;
                dataGridViewStockTakeUpload.Columns["HOSTNAME"].MinimumWidth = 100;
                dataGridViewStockTakeUpload.Columns["CREATEDDATE"].MinimumWidth = 150;
                dataGridViewStockTakeUpload.Columns["TIME"].MinimumWidth = 100;
            }
        }

        private void buttonStockTakeReportPrint_Click(object sender, EventArgs e)
        {
            if (dataGridViewStockTakeReport.RowCount == 0)
            {
                MessageBox.Show("Data Grid Kosong, Tolong gunakan fungsi search dahulu untuk memunculkan data",
                    "Data Grid Kosong",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                saveFileDialogStockTake.ShowDialog();
            }
        }

        private void saveFileDialogStockTake_FileOk(object sender, CancelEventArgs e)
        {

            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;

            Application.DoEvents();


            string fileName = saveFileDialogStockTake.FileName;



            var titleFont = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD);
            var boldTableFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD);
            //var endingMessageFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL);

            //DateTime date = new DateTime();

            try
            {


                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //Create a new PDF document setting the size to A4
                    using (Document doc = new Document(PageSize.A4, 50, 50, 25, 50))
                    {
                        //Bind the PDF document to the FileStream using an iTextSharp PdfWriter
                        using (PdfWriter w = PdfWriter.GetInstance(doc, fs))
                        {

                            //Open the document for writing
                            doc.Open();

                            string imagePath = function.getImageFooterMemoDiscount();

                            doc.Add(new Paragraph(" "));

                            iTextSharp.text.Image imageHeader = iTextSharp.text.Image.GetInstance(imagePath);



                            _eventsStockTakeReport events = new _eventsStockTakeReport();
                            w.PageEvent = events;

                            events.ImageHeader = imageHeader;



                            Paragraph paragraph = new Paragraph("Memo Discount Report", titleFont);


                            //paragraph.Alignment = Element.ALIGN_CENTER;
                            //paragraph.SpacingBefore = 10;
                            //doc.Add(paragraph);


                            paragraph = new Paragraph("Store : " + comboBoxSite.SelectedValue + " : " + comboBoxSite.GetItemText(comboBoxSite.SelectedItem), subTitleFont);
                            paragraph.SpacingBefore = 10;
                            doc.Add(paragraph);


                            //paragraph = new Paragraph("Filter Data", subTitleFont);
                            //paragraph.SpacingBefore = 10;
                            //doc.Add(paragraph);

                            var HeaderInfo = new PdfPTable(6);
                            HeaderInfo.HorizontalAlignment = 0;
                            HeaderInfo.SpacingBefore = 10;

                            //HeaderInfo.SpacingAfter = 10;
                            HeaderInfo.DefaultCell.Border = 0;
                            //HeaderInfo.SetWidths(new int[] { 1, 4 });




                            HeaderInfo.AddCell(new Phrase("Inventory Number", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(":", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(textBoxInvNumStockTakeReport.Text, boldTableFont));
                            HeaderInfo.AddCell(new Phrase("Location", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(":", boldTableFont));
                            HeaderInfo.AddCell(new Phrase(textBoxLocationStockTakeReport.Text, boldTableFont));

                            doc.Add(HeaderInfo);

                            doc.Add(new Paragraph(""));
                            doc.Add(new Paragraph(""));
                            //doc.Add(new Paragraph("Total Promo : " + dataGridViewMemoDiscountHeader.RowCount, subTitleFont));
                            doc.Add(new Paragraph(""));
                            doc.Add(new Paragraph(""));



                            PdfPCell cell = new PdfPCell();
                            PdfPCell cellHeader = new PdfPCell();

                            Phrase Title = new Phrase("Title");


                            int PageNumber = 1;

                            cellHeader = new PdfPCell();
                            cellHeader.BackgroundColor = new BaseColor(Color.Gainsboro);
                            var HeaderTable = new PdfPTable(21);
                            HeaderTable.HorizontalAlignment = 0;
                            HeaderTable.SpacingBefore = SPACINGAFTER;
                            HeaderTable.TotalWidth = 500f;
                            HeaderTable.LockedWidth = true;
                            HeaderTable.SpacingAfter = 10;
                            HeaderTable.DefaultCell.Border = 0;
                            //HeaderTable.SetWidths(new int[] {1, 4});
                            //HeaderTable.SetWidths(new float[] { 2f, 6f, 6f, 3f, 5f, 8f, 5f, 5f, 5f, 5f, 5f });




                            cellHeader.Colspan = 3;
                            cellHeader.Phrase = new Phrase("PLU", boldTableFont);
                            HeaderTable.AddCell(cellHeader);

                            cellHeader.Colspan = 10;
                            cellHeader.Phrase = new Phrase("Description", boldTableFont);
                            HeaderTable.AddCell(cellHeader);

                            cellHeader.Colspan = 4;
                            cellHeader.Phrase = new Phrase("Harga Jual", boldTableFont);
                            HeaderTable.AddCell(cellHeader);

                            cellHeader.Colspan = 4;
                            cellHeader.Phrase = new Phrase("Hasil Stock", boldTableFont);
                            HeaderTable.AddCell(cellHeader);

                            foreach (DataRow row in DTStockTakeReport.Rows) // Loop over the rows.
                            {

                                events.rowHeader = row;



                                cellHeader = new PdfPCell();

                                cellHeader.Colspan = 3;
                                cellHeader.Phrase = (new Phrase(row["BARCODE"].ToString(), bodyFont));
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 10;
                                cellHeader.Phrase = (new Phrase(row["DESCRIPTION"].ToString(), bodyFont));
                                HeaderTable.AddCell(cellHeader);

                                String Price = function.getPrice(row["BARCODE"].ToString(), comboBoxSite.SelectedValue.ToString());

                                if (String.IsNullOrWhiteSpace(Price))
                                {
                                    Price = "0";
                                }

                                cellHeader.Colspan = 4;
                                cellHeader.Phrase = (new Phrase(Price, bodyFont));
                                HeaderTable.AddCell(cellHeader);

                                cellHeader.Colspan = 4;
                                cellHeader.Phrase = (new Phrase(row["QUANTITY"].ToString(), bodyFont));
                                HeaderTable.AddCell(cellHeader);

                            }
                            doc.Add(HeaderTable);
                            //Close our document
                            doc.Close();
                        }
                    }
                }
                MessageBox.Show("file telah berhasil di generate", "Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Cursor.Current = Cursors.Default;
        }

        #endregion





        #region Invoice Payment Process
        private void invoiceParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            activePanel.Visible = false;
            activePanel = InvoiceParamPanel;
            activePanel.Visible = true;
            DisplayParam();
            CekTypeParam();
        }


        //Header Panel
        private void paymentProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelPaymentProcess;
            activePanel.Visible = true;
            InvKodeBox();
            InvJualBox();
            InvBeliBox();
            InvHeader DataHdr = new InvHeader();
            DataHdr.KODE = InvHKodeBox.SelectedValue.ToString();
            DataHdr.IDPEMBELI = CmbKenaPajak.SelectedValue.ToString();
            DataHdr.IDPENGUSAHA = PembeliKenaBox.SelectedValue.ToString();
            DisplayHeader(DataHdr);

        }
        //End Header Panel
        private void InvKodeBox()
        {
            try
            {
                DTParameterType = function.selectinvkodeH();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            InvHKodeBox.DataSource = DTParameterType;
            InvHKodeBox.DisplayMember = "KODE";
            InvHKodeBox.ValueMember = "IDKODE";

        }

        private void InvJualBox()
        {
            try
            {
                DTParameterType = function.selectinvUsahaH();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            CmbKenaPajak.DataSource = DTParameterType;
            CmbKenaPajak.DisplayMember = "IDPENGUSAHA";
            CmbKenaPajak.ValueMember = "IDPENG";
        }

        private void InvBeliBox()
        {
            try
            {
                DTParameterType = function.selectinvbeliH();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            PembeliKenaBox.DataSource = DTParameterType;
            PembeliKenaBox.DisplayMember = "IDPEMBELI";
            PembeliKenaBox.ValueMember = "IDPEM";
        }



        private void NewBtn_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelPaymentProcessNew;
            activePanel.Visible = true;
            InvHNewKodeTxt.Text = "";
            InvHNewKodeTxt.ReadOnly = false;

            PKPBox.Enabled = true;
            PBKPBox.Enabled = true;
            OtherHeaderPanel.Visible = false;
            DetailBtn.Visible = false;
            UpdateHinvBtn.Visible = false;
            SaveBtn.Visible = true;
            InvHConfirmBtn.Visible = false;
            HComment.Enabled = true;
            FComment.Enabled = true;
            HParamInvPanel.Visible = false;
            ShowParam.Visible = false;
            PPreviewBtn.Visible = false;
            FComment.Text = "";
            HComment.Text = "";
            Loadcombotype("Header");
            LoadSupplier(PKPBox);
            LoadPembeli(PBKPBox);
            DisplayInvDetail("");
            DisplayHeaderEx("");
        }

        private void DeleteInvoiceHeaderBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ErrorString = function.DeleteInvHeader(IDKodeTxt.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            MessageBox.Show("Delete: " + IDKodeTxt.Text, "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

            activePanel.Visible = false;
            activePanel = panelPaymentProcess;
            activePanel.Visible = true;
            InvKodeBox();
            InvJualBox();
            InvBeliBox();
            InvHeader DataHdr = new InvHeader();
            DataHdr.KODE = InvHKodeBox.SelectedValue.ToString();
            DataHdr.IDPEMBELI = CmbKenaPajak.SelectedValue.ToString();
            DataHdr.IDPENGUSAHA = PembeliKenaBox.SelectedValue.ToString();
            DisplayHeader(DataHdr);
        }

        private void PHBackBtn_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelPaymentProcess;
            activePanel.Visible = true;
            InvKodeBox();
            InvJualBox();
            InvBeliBox();
            InvHeader DataHdr = new InvHeader();
            DataHdr.KODE = InvHKodeBox.SelectedValue.ToString();
            DataHdr.IDPEMBELI = CmbKenaPajak.SelectedValue.ToString();
            DataHdr.IDPENGUSAHA = PembeliKenaBox.SelectedValue.ToString();
            DisplayHeader(DataHdr);
        }

        private void Loadcombotype(String Data)
        {
            try
            {
                DTParameterType = function.SelectParameterPajak(Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            if (Data == "Header")
            {
                PHTypeBox.DataSource = DTParameterType;
                PHTypeBox.DisplayMember = "LONGDESC";
                PHTypeBox.ValueMember = "ID";
            }
            else
            {
                DparamBox.DataSource = DTParameterType;
                DparamBox.DisplayMember = "LONGDESC";
                DparamBox.ValueMember = "ID";
            }


        }

        private void LoadKodeSeriFakturPajak(ComboBox CBKodeSeriFakturPajak)
        {
            try
            {
                DTParameterType = function.SelectKodeSeriFakturPajak();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            CBKodeSeriFakturPajak.DataSource = DTParameterType;
            CBKodeSeriFakturPajak.DisplayMember = "KODE";
            CBKodeSeriFakturPajak.ValueMember = "KODE";

        }

        private void LoadPembeli(ComboBox CBPembeli)
        {
            try
            {
                DTParameterType = function.SelectPembeli();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            CBPembeli.DataSource = DTParameterType;
            CBPembeli.DisplayMember = "LONGDESC";
            CBPembeli.ValueMember = "ID";

        }

        private void LoadSupplier(ComboBox CBSupplier)
        {
            try
            {
                DTParameterType = function.SelectSupplier();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            CBSupplier.DataSource = DTParameterType;
            CBSupplier.DisplayMember = "foulibl";
            CBSupplier.ValueMember = "foucnuf";
        }


        private void InvoiceDataBox()
        {
            try
            {
                DTParameterType = function.InvoiceData(DetailUsahaTxt.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            InvoiceBox.DataSource = DTParameterType;
            InvoiceBox.DisplayMember = "InvNUm";
            InvoiceBox.ValueMember = "InvNUm";


        }


        private void DetailBtn_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = panelProsesDetail;
            activePanel.Visible = true;
            Loadcombotype("Detail");

            //DisplayInvDetailEx(IDKodeTxt.Text);            
            DetailKodeTxt.Text = InvHNewKodeTxt.Text;
            DetailUsahaTxt.Text = PKPBox.Text;
            DetailBeliTxt.Text = PBKPBox.Text;
            IdHeaderDetailLbl.Text = IDKodeTxt.Text;
            DetailSaveBtn.Visible = true;
            InvoiceBox.Enabled = true;
            PanelDetailParamInvoice.Visible = false;
            string abc = InvoiceBox.Text;

            TotalIDtxt.Text = "0";
            TaxIDtxt.Text = "0";
            TotalTaxIDtxt.Text = "0";
            TotalExpDetail.Text = "0";
            IDDetailTxt.Text = "";
            Dcomment.Text = "NB. Mohon Invoice di kirim via fax ke 0721-242439 atau 0721-269816 sebelum via Pos untuk proses pembayaran";
            DparamBox.Enabled = true;
            CancelBtnPD.Visible = false;
            DExtSaveParamBtn.Visible = true;
            UpdInvPD.Visible = false;
            DValueParamTxt.Text = "";
            UpdDetailBtn.Visible = false;
            InvoiceDataBox();
            if (InvoiceBox.Text != "")
            {
                RefreshInvoiceBox(InvoiceBox.SelectedValue.ToString());
            }
            DetailSummaryBtn.Visible = false;
            DetailRptBtn.Visible = false;
        }


        private void PanelPaymentProcess_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black,
            e.ClipRectangle.Left,
            e.ClipRectangle.Top,
            e.ClipRectangle.Width - 1,
            e.ClipRectangle.Height - 1);
            base.OnPaint(e);
        }

        private void CancelSaveDetailBtn_Click(object sender, EventArgs e)
        {

            activePanel.Visible = false;
            activePanel = panelPaymentProcessNew;
            activePanel.Visible = true;
            Loadcombotype("Header");
            LoadSupplier(PKPBox);
            LoadPembeli(PBKPBox);
            HeaderVisible(IDKodeTxt.Text);
        }



        private void DisplayHeaderEx(String Kode)
        {

            // Set cursor as hourglass            
            //ClearDataTable();
            try
            {
                DTParameter = function.SelectHinvoiceEx(Kode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            HInvExdataGridView.Columns.Clear();
            HInvExdataGridView.DataSource = DTParameter;
            HInvExdataGridView.Columns["BIAYA"].MinimumWidth = 250;
            HInvExdataGridView.Columns["AMOUNT"].MinimumWidth = 250;
            HInvExdataGridView.Columns["AMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            HInvExdataGridView.Columns["AMOUNTVISIBLE"].Visible = false;
            DataGridViewLinkColumn EditLink = new DataGridViewLinkColumn();
            EditLink.UseColumnTextForLinkValue = true;
            EditLink.HeaderText = "EDIT";
            EditLink.DataPropertyName = "lnkColumn";
            EditLink.LinkBehavior = LinkBehavior.SystemDefault;
            EditLink.Text = "EDIT";
            HInvExdataGridView.Columns.Add(EditLink);

            DataGridViewLinkColumn Deletelink = new DataGridViewLinkColumn();
            Deletelink.UseColumnTextForLinkValue = true;
            Deletelink.HeaderText = "DELETE";
            Deletelink.DataPropertyName = "lnkColumn";
            Deletelink.LinkBehavior = LinkBehavior.SystemDefault;
            Deletelink.Text = "DELETE";
            HInvExdataGridView.Columns.Add(Deletelink);


        }

        private void DisplayHeader(InvHeader DataHdr)
        {

            // Set cursor as hourglass          
            ClearDataTable();
            try
            {
                DTParameter = function.SelectHinvoice(DataHdr.KODE, DataHdr.IDPENGUSAHA, DataHdr.IDPEMBELI);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error Occured",
                //MessageBoxButtons.OK, MessageBoxIcon.Error);
                //logger.Error(ex.Message);
            }


            HInvdataGridView.Columns.Clear();
            HInvdataGridView.DataSource = DTParameter;
            HInvdataGridView.Columns["TOTAL INVOICE"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            HInvdataGridView.Columns["TOTAL BIAYA INVOICE"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            HInvdataGridView.Columns["KODE"].MinimumWidth = 150;
            HInvdataGridView.Columns["PEMBELI"].MinimumWidth = 175;
            HInvdataGridView.Columns["PENGUSAHA"].MinimumWidth = 175;
            HInvdataGridView.Columns["COMMENT HEADER"].MinimumWidth = 190;
            HInvdataGridView.Columns["COMMENT FOOTER"].MinimumWidth = 190;
            DataGridViewLinkColumn EditLink = new DataGridViewLinkColumn();
            EditLink.UseColumnTextForLinkValue = true;
            EditLink.HeaderText = "EDIT";
            EditLink.DataPropertyName = "lnkColumn";
            EditLink.LinkBehavior = LinkBehavior.SystemDefault;
            EditLink.Text = "EDIT";
            HInvdataGridView.Columns.Add(EditLink);

            DataGridViewLinkColumn Deletelink = new DataGridViewLinkColumn();
            Deletelink.UseColumnTextForLinkValue = true;
            Deletelink.HeaderText = "DELETE";
            Deletelink.DataPropertyName = "lnkColumn";
            Deletelink.LinkBehavior = LinkBehavior.SystemDefault;
            Deletelink.Text = "DELETE";
            HInvdataGridView.Columns.Add(Deletelink);


        }


        private void DisplayInvDetail(String Kode)
        {

            // Set cursor as hourglass            
            ClearDataTable();
            try
            {
                DTParameter = function.SelectDinvoice(Kode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            DInvdataGridView.Columns.Clear();
            DInvdataGridView.DataSource = DTParameter;
            DInvdataGridView.Columns["BRUTO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //DInvdataGridView.Columns["NETTO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //DInvdataGridView.Columns["DISC BRUTO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //DInvdataGridView.Columns["DISC NETTO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            DInvdataGridView.Columns["INVOICE"].MinimumWidth = 150;
            DataGridViewLinkColumn EditLink = new DataGridViewLinkColumn();
            EditLink.UseColumnTextForLinkValue = true;
            EditLink.HeaderText = "EDIT";
            EditLink.DataPropertyName = "lnkColumn";
            EditLink.LinkBehavior = LinkBehavior.SystemDefault;
            EditLink.Text = "EDIT";
            DInvdataGridView.Columns.Add(EditLink);

            DataGridViewLinkColumn Deletelink = new DataGridViewLinkColumn();
            Deletelink.UseColumnTextForLinkValue = true;
            Deletelink.HeaderText = "DELETE";
            Deletelink.DataPropertyName = "lnkColumn";
            Deletelink.LinkBehavior = LinkBehavior.SystemDefault;
            Deletelink.Text = "DELETE";
            DInvdataGridView.Columns.Add(Deletelink);

        }

        private void DisplayInvDetailEx(String IDD)
        {
            // Set cursor as hourglass            
            ClearDataTable();
            try
            {
                DTParameter = function.SelectDinvoiceEx(IDD);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            DInvExdataGridView.Columns.Clear();
            DInvExdataGridView.DataSource = DTParameter;
            DInvExdataGridView.Columns["AMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DataGridViewLinkColumn EditLink = new DataGridViewLinkColumn();
            EditLink.UseColumnTextForLinkValue = true;
            EditLink.HeaderText = "EDIT";
            EditLink.DataPropertyName = "lnkColumn";
            EditLink.LinkBehavior = LinkBehavior.SystemDefault;
            EditLink.Text = "EDIT";
            DInvExdataGridView.Columns.Add(EditLink);

            DataGridViewLinkColumn Deletelink = new DataGridViewLinkColumn();
            Deletelink.UseColumnTextForLinkValue = true;
            Deletelink.HeaderText = "DELETE";
            Deletelink.DataPropertyName = "lnkColumn";
            Deletelink.LinkBehavior = LinkBehavior.SystemDefault;
            Deletelink.Text = "DELETE";
            DInvExdataGridView.Columns.Add(Deletelink);


        }
        private void SaveBtn_Click(object sender, EventArgs e)
        {

            String DataIDH = "";
            if (InvHNewKodeTxt.Text != "")
            {
                try
                {
                    ErrorString = "";

                    ErrorString = function.CekNoFaktur(InvHNewKodeTxt.Text);
                    if (ErrorString != "Already Exists")
                    {

                        try
                        {
                            ErrorString = function.InsertInvHeader(InvHNewKodeTxt.Text, PKPBox.SelectedValue.ToString(), PBKPBox.SelectedValue.ToString(), IStartDate.Text, IEndDate.Text, HComment.Text, FComment.Text);
                            DataIDH = function.getidKodeDataIDH(InvHNewKodeTxt.Text, PKPBox.SelectedValue.ToString(), PBKPBox.SelectedValue.ToString());

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            logger.Error(ex.Message);
                        }
                        HeaderVisible(DataIDH);

                    }
                    else
                    {
                        MessageBox.Show("Kode Faktur Already Exists ", ErrorString,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Faktur Pajak Mandatory", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveHParamBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ErrorString = "";

                ErrorString = function.CekParamHeader(IDKodeTxt.Text, PHValueBox.Text, PHTypeBox.SelectedValue.ToString());
                if (ErrorString != "Already Exists")
                {
                    ErrorString = function.InsertInvParamHeader(IDKodeTxt.Text, PHValueBox.Text, PHTypeBox.SelectedValue.ToString(), "Insert");
                }
                MessageBox.Show("Insert Parameter: " + PHTypeBox.Text, ErrorString,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            PHValueBox.Text = "";
            DisplayInvDetail(IDKodeTxt.Text);
            DisplayHeaderEx(IDKodeTxt.Text);
            HeaderVisible(IDKodeTxt.Text);

        }


        private void HeaderVisible(String Kode)
        {
            InvHeader KodeId = new InvHeader();
            try
            {
                KodeId = function.getIdKodeData(Kode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            DisplayInvDetail(KodeId.IDH);
            DisplayHeaderEx(KodeId.IDH);

            StatusLabel.Text = KodeId.STATUS;
            TanggalLabel.Text = KodeId.CREATEDDATE;
            pembuatlabel.Text = KodeId.CREATEDBY;
            ModifiedDTxt.Text = KodeId.MODIFIEDBY;
            ModifiedByTxt.Text = KodeId.MODIFIEDDATE;
            IDKodeTxt.Text = KodeId.IDH;
            InvHNewKodeTxt.Text = KodeId.KODE;
            PKPBox.SelectedValue = KodeId.IDPENGUSAHA;
            PBKPBox.SelectedValue = KodeId.IDPEMBELI;
            IStartDate.Text = KodeId.STARTDATE;
            IEndDate.Text = KodeId.ENDDATE;
            FComment.Text = KodeId.FComment;
            HComment.Text = KodeId.HComment;
            InvHeader KodeIdDetail = new InvHeader();
            try
            {
                KodeIdDetail = function.getIdKodeCekData(Kode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            if (KodeIdDetail.IDH != null)
            {
                TDataInvTxt.Text = decimal.Parse(KodeIdDetail.TOTALDATAINV).ToString("N");
                TInvTxt.Text = decimal.Parse(KodeIdDetail.TOTALINV).ToString("N");
                TExpInvTxt.Text = decimal.Parse(KodeIdDetail.EXPDETAIL).ToString("N");
                TInvWTaxTxt.Text = decimal.Parse(KodeIdDetail.TOTALINVWT).ToString("N");
                TInvTaxTxt.Text = decimal.Parse(KodeIdDetail.TOTALINVT).ToString("N");
                TInvNTaxTxt.Text = decimal.Parse(KodeIdDetail.TOTALINVNT).ToString("N");
                TExpFacTxt.Text = decimal.Parse(KodeIdDetail.EXPHEADER).ToString("N");
                TFacWTaxTxt.Text = decimal.Parse(KodeIdDetail.TOTALFACWT).ToString("N");
                TFacTaxTxt.Text = decimal.Parse(KodeIdDetail.TOTALFACT).ToString("N");
                TFacNTaxTxt.Text = decimal.Parse(KodeIdDetail.TOTALFACNT).ToString("N");
            }
            else
            {
                TDataInvTxt.Text = "";
                TInvTxt.Text = "";
                TExpInvTxt.Text = "";
                TInvWTaxTxt.Text = "";
                TInvTaxTxt.Text = "";
                TInvNTaxTxt.Text = "";
                TExpFacTxt.Text = "";
                TFacWTaxTxt.Text = "";
                TFacTaxTxt.Text = "";
                TFacNTaxTxt.Text = "";
            }
            // InvHNewKodeTxt.ReadOnly = true;

            PKPBox.Enabled = false;
            PBKPBox.Enabled = false;
            DetailBtn.Visible = true;
            UpdateHinvBtn.Visible = true;
            SaveBtn.Visible = false;
            HParamInvPanel.Visible = true;
            ShowParam.Visible = true;
            PPreviewBtn.Visible = true;
            InvHConfirmBtn.Visible = true;
        }
        private void HInvdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


            if (HInvdataGridView.Columns[e.ColumnIndex].HeaderText.ToLower() == "delete")
            {
                String KODE = HInvdataGridView["KODE", e.RowIndex].Value.ToString();
                String ID = HInvdataGridView["IDH", e.RowIndex].Value.ToString();
                var confirmResult = MessageBox.Show("Are you sure you want to delete Factur ?", "Confirm Delete!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        ErrorString = function.UpdateArchiveDataHeader(ID, "0");
                        ErrorString = function.DeleteInvHeader(ID);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }
                    MessageBox.Show("Delete: " + KODE, "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    InvKodeBox();
                    InvJualBox();
                    InvBeliBox();
                    InvHeader DataHdr = new InvHeader();
                    DataHdr.KODE = InvHKodeBox.SelectedValue.ToString();
                    DataHdr.IDPEMBELI = CmbKenaPajak.SelectedValue.ToString();
                    DataHdr.IDPENGUSAHA = PembeliKenaBox.SelectedValue.ToString();
                    DisplayHeader(DataHdr); ;
                }
            }
            else if (HInvdataGridView.Columns[e.ColumnIndex].HeaderText.ToLower() == "edit")
            {
                String ID = HInvdataGridView["IDH", e.RowIndex].Value.ToString();
                activePanel.Visible = false;
                activePanel = panelPaymentProcessNew;
                activePanel.Visible = true;
                Loadcombotype("Header");
                LoadSupplier(PKPBox);
                LoadPembeli(PBKPBox);
                HeaderVisible(ID);
            }

        }
        private void ValidBtn_Click(object sender, EventArgs e)
        {

            if (HInvdataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                String DataID = "";
                foreach (DataGridViewRow row in this.HInvdataGridView.SelectedRows)
                {
                    GlobalVar.GlobalVarKodeInvoice = row.Cells["KODE"].Value.ToString();
                    DataID = row.Cells["IDH"].Value.ToString();


                }
                var confirmResult = MessageBox.Show("Are you sure you want to Confirm Factur ?", "Confirm Factur!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {


                    try
                    {
                        ErrorString = function.UpdateInvHeader(DataID);
                        ErrorString = function.UpdateArchiveDataHeader(DataID, "2");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }

                    //GlobalVar.GlobalVarKodeInvoice = InvHNewKodeTxt.Text;
                    GlobalVar.GlobalVarPanel = panelPaymentProcess;

                    FakturPajakIsPreview = false;
                    ShowPanelPrintInvoice();

                    InvKodeBox();
                    InvJualBox();
                    InvBeliBox();
                    InvHeader DataHdr = new InvHeader();
                    DataHdr.KODE = InvHKodeBox.SelectedValue.ToString();
                    DataHdr.IDPEMBELI = CmbKenaPajak.SelectedValue.ToString();
                    DataHdr.IDPENGUSAHA = PembeliKenaBox.SelectedValue.ToString();
                    DisplayHeader(DataHdr);
                }
            }


        }
        private void DetailSaveBtn_Click(object sender, EventArgs e)
        {

            try
            {
                ErrorString = function.UpdateArchiveData(InvoiceBox.SelectedValue.ToString(), "1");

                ErrorString = function.InsertInvDetail(IdHeaderDetailLbl.Text, InvoiceBox.SelectedValue.ToString(), Dcomment.Text, "Insert", DetailUsahaTxt.Text);

                MessageBox.Show("Insert Invoice Detail: " + InvHNewKodeTxt.Text, "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                String SKU = InvoiceBox.SelectedValue.ToString();
                DataDetailRefresh(SKU);
                //DisplayInvDetailEx(InvoiceBox.SelectedValue.ToString());
                RefreshDataDetail();

                DetailSummaryBtn.Visible = true;
                DetailRptBtn.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


        }

        private void DataDetailRefresh(String SKU)
        {
            InvDetail DetailData = new InvDetail();
            try
            {
                DetailData = function.getIdInvoiceDetail(SKU);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            IDDetailTxt.Text = DetailData.IDD;

            DisplayInvDetailEx(DetailData.IDD);
            DetailSaveBtn.Visible = false;
            InvoiceBox.Enabled = false;
            UpdDetailBtn.Visible = true;
            PanelDetailParamInvoice.Visible = true;
        }
        private void DExtSaveParamBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ErrorString = "";

                ErrorString = function.CekParamDetail(IDDetailTxt.Text, DValueParamTxt.Text, DparamBox.SelectedValue.ToString());
                if (ErrorString != "Already Exists")
                {
                    ErrorString = function.InsertInvParamDetail(IDDetailTxt.Text, DValueParamTxt.Text, DparamBox.SelectedValue.ToString(), "Insert");
                }

                MessageBox.Show("Insert Parameter Detail: " + DparamBox.Text, ErrorString,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            DValueParamTxt.Text = "";
            DisplayInvDetailEx(IDDetailTxt.Text);
            RefreshDataDetail();
        }

        private void InvoiceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvoiceBox.Text != "")
            {
                RefreshInvoiceBox(InvoiceBox.SelectedValue.ToString());
            }
        }


        private void RefreshInvoiceBox(string Invoice)
        {
            DTDetailIvoice.Clear();
            try
            {
                DTDetailIvoice = function.SelectInvoiceDetailData(Invoice, DetailUsahaTxt.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            InvoiceDetailGridView.DataSource = DTDetailIvoice;
        }

        #endregion






        private void HInvExdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (HInvExdataGridView.Columns[e.ColumnIndex].HeaderText.ToLower() == "delete")
            {
                var confirmResult = MessageBox.Show("Are you sure you want to delete this item ??", "Confirm Delete!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    String Param = HInvExdataGridView["BIAYA", e.RowIndex].Value.ToString();
                    String Value = HInvExdataGridView["AMOUNT", e.RowIndex].Value.ToString();
                    try
                    {

                        ErrorString = function.DeleteParamHeader(IDKodeTxt.Text, Param, Value);
                        DisplayHeaderEx(IDKodeTxt.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }
                }
            }
            else if (HInvExdataGridView.Columns[e.ColumnIndex].HeaderText.ToLower() == "edit")
            {


                String Param = HInvExdataGridView["BIAYA", e.RowIndex].Value.ToString();
                String Value = HInvExdataGridView["AMOUNTVISIBLE", e.RowIndex].Value.ToString();

                InvoiceParam Data = new InvoiceParam();
                InvoiceParam ExeData = new InvoiceParam();
                Data.IDH = IDKodeTxt.Text;
                Data.VALUE = Value;
                Data.PARAMDESC = Param;

                try
                {
                    ExeData = function.getParameterHeader(Data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

                LblIDParamH.Text = ExeData.IDHP;
                PHValueBox.Text = ExeData.VALUE;
                PHTypeBox.Text = Data.PARAMDESC;
                PHTypeBox.SelectedValue = ExeData.PARAMID;
                UpdHParamBtn.Visible = true;
                SaveHParamBtn.Visible = false;
                UpdDetailBtn.Visible = true;
            }
            /*
            Loadcombotype("Header");
            LoadSupplier();
            LoadPembeli();
            HeaderVisible(KODE);
             */
        }

        private void DInvdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (DInvdataGridView.Columns[e.ColumnIndex].HeaderText.ToLower() == "delete")
            {
                String SKUID = DInvdataGridView["INVOICE", e.RowIndex].Value.ToString();
                var confirmResult = MessageBox.Show("Are you sure you want to delete this item ??", "Confirm Delete!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {

                    try
                    {
                        ErrorString = function.UpdateArchiveData(SKUID, "0");
                        ErrorString = function.DeleteInvExpDetail(IDKodeTxt.Text, SKUID);
                        ErrorString = function.DeleteInvDetail(IDKodeTxt.Text, SKUID);
                        DisplayInvDetail(IDKodeTxt.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }
                }
            }
            else if (DInvdataGridView.Columns[e.ColumnIndex].HeaderText.ToLower() == "edit")
            {
                String SKUID = DInvdataGridView["INVOICE", e.RowIndex].Value.ToString();
                String Comment = DInvdataGridView["COMMENT DETAIL", e.RowIndex].Value.ToString();
                activePanel.Visible = false;
                activePanel = panelProsesDetail;
                activePanel.Visible = true;
                Loadcombotype("Detail");
                InvoiceBox.SelectedValue = SKUID;
                InvoiceBox.Text = SKUID;
                InvoiceBox.Enabled = false;
                DisplayInvDetailEx(IDKodeTxt.Text);
                DetailKodeTxt.Text = InvHNewKodeTxt.Text;
                DetailUsahaTxt.Text = PKPBox.Text;
                DetailBeliTxt.Text = PBKPBox.Text;
                IdHeaderDetailLbl.Text = IDKodeTxt.Text;
                Dcomment.Text = Comment;
                DetailSaveBtn.Visible = false;
                PanelDetailParamInvoice.Visible = true;
                DataDetailRefresh(SKUID);

                DparamBox.Enabled = true;
                CancelBtnPD.Visible = false;
                DExtSaveParamBtn.Visible = true;
                UpdInvPD.Visible = false;
                DValueParamTxt.Text = "";
                RefreshDataDetail();
                RefreshInvoiceBox(SKUID);
                DetailSummaryBtn.Visible = true;
                DetailRptBtn.Visible = true;
            }

        }



        #region Parameter Invoice
        private void InvoiceParamPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        private void SrchBtn_Click(object sender, EventArgs e)
        {
            DisplayParam();
        }
        private void DisplayParam()
        {

            // Set cursor as hourglass            
            ClearDataTable();
            try
            {
                DTParameter = function.SelectParamInvoice(SDescTxt.Text, LDescTxt.Text, PTypeBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            parametergridview.Columns.Clear();
            parametergridview.DataSource = DTParameter;
            DataGridViewLinkColumn EditLink = new DataGridViewLinkColumn();
            EditLink.UseColumnTextForLinkValue = true;
            EditLink.HeaderText = "EDIT";
            EditLink.DataPropertyName = "lnkColumn";
            EditLink.LinkBehavior = LinkBehavior.SystemDefault;
            EditLink.Text = "EDIT";
            parametergridview.Columns.Add(EditLink);

            DataGridViewLinkColumn Deletelink = new DataGridViewLinkColumn();
            Deletelink.UseColumnTextForLinkValue = true;
            Deletelink.HeaderText = "DELETE";
            Deletelink.DataPropertyName = "lnkColumn";
            Deletelink.LinkBehavior = LinkBehavior.SystemDefault;
            Deletelink.Text = "DELETE";
            parametergridview.Columns.Add(Deletelink);

        }
        private void PSaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (PTypeBox.Text != "Pembeli")
                {
                    ErrorString = function.insertParameter(SDescTxt.Text, LDescTxt.Text, PTypeBox.Text);
                }
                else
                {
                    ErrorString = function.insertParameterPembeli(SDescTxt.Text, LDescTxt.Text, PTypeBox.Text, NPWPText.Text, alamatTxt.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            MessageBox.Show("Insert Parameter: " + SDescTxt, "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
            SDescTxt.Text = "";
            LDescTxt.Text = "";
            PTypeBox.Text = "";
            NPWPText.Text = "";
            alamatTxt.Text = "";
            DisplayParam();

        }

        private void parametergridview_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {

        }

        private void parametergridview_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {




            if (parametergridview.Columns[e.ColumnIndex].HeaderText.ToLower() == "delete")
            {
                var confirmResult = MessageBox.Show("Are you sure you want to delete this item ??", "Confirm Delete!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    IDLbl.Text = parametergridview["ID", e.RowIndex].Value.ToString();
                    SDescTxt.Text = parametergridview["ShortDescription", e.RowIndex].Value.ToString();
                    LDescTxt.Text = parametergridview["LongDescription", e.RowIndex].Value.ToString();
                    PTypeBox.Text = parametergridview["Type", e.RowIndex].Value.ToString();
                    NPWPText.Text = parametergridview["NPWP", e.RowIndex].Value.ToString();
                    alamatTxt.Text = parametergridview["Alamat", e.RowIndex].Value.ToString();
                    DelParam();
                }
            }
            else if (parametergridview.Columns[e.ColumnIndex].HeaderText.ToLower() == "edit")
            {
                IDLbl.Text = parametergridview["ID", e.RowIndex].Value.ToString();
                SDescTxt.Text = parametergridview["ShortDescription", e.RowIndex].Value.ToString();
                LDescTxt.Text = parametergridview["LongDescription", e.RowIndex].Value.ToString();
                PTypeBox.Text = parametergridview["Type", e.RowIndex].Value.ToString();
                NPWPText.Text = parametergridview["NPWP", e.RowIndex].Value.ToString();
                alamatTxt.Text = parametergridview["Alamat", e.RowIndex].Value.ToString();
                UpdateBtn.Visible = true;
                BtlBtn.Visible = true;
                SrchBtn.Visible = false;
                PSaveBtn.Visible = false;
            }
        }



        private void BtlBtn_Click(object sender, EventArgs e)
        {
            SDescTxt.Text = "";
            LDescTxt.Text = "";
            PTypeBox.Text = "";
            DisplayParam();
            UpdateBtn.Visible = false;
            BtlBtn.Visible = false;
            SrchBtn.Visible = true;
            PSaveBtn.Visible = true;



        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (PTypeBox.Text == "Pembeli")
                {
                    ErrorString = function.UpdateParameterPembeli(IDLbl.Text, SDescTxt.Text, LDescTxt.Text, PTypeBox.Text, NPWPText.Text, alamatTxt.Text);
                }
                else
                {
                    ErrorString = function.UpdateParameter(IDLbl.Text, SDescTxt.Text, LDescTxt.Text, PTypeBox.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            MessageBox.Show("Update Parameter: " + SDescTxt, "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
            SDescTxt.Text = "";
            LDescTxt.Text = "";
            PTypeBox.Text = "";
            alamatTxt.Text = "";
            NPWPText.Text = "";
            DisplayParam();
            UpdateBtn.Visible = false;
            BtlBtn.Visible = false;
            SrchBtn.Visible = true;
            PSaveBtn.Visible = true;
            CekTypeParam();

        }

        private void PDelBtn_Click(object sender, EventArgs e)
        {

            DelParam();
        }

        private void DelParam()
        {
            try
            {
                ErrorString = function.DeleteParameter(IDLbl.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            MessageBox.Show("Delete Parameter: " + IDLbl.Text, "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
            SDescTxt.Text = "";
            LDescTxt.Text = "";
            PTypeBox.Text = "";
            NPWPText.Text = "";
            alamatTxt.Text = "";
            DisplayParam();
            UpdateBtn.Visible = false;
            BtlBtn.Visible = false;
            SrchBtn.Visible = true;
            PSaveBtn.Visible = true;
            CekTypeParam();
        }
        #endregion

        private void PTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CekTypeParam();
        }

        private void CekTypeParam()
        {
            if (PTypeBox.Text == "Pembeli")
            {
                panelParamBeli.Visible = true;
            }
            else
            {
                panelParamBeli.Visible = false;
            }
        }

        private void ShowParam_Click(object sender, EventArgs e)
        {
            OtherHeaderPanel.Visible = true;
            HComment.Enabled = false;
            FComment.Enabled = false;
        }

        private void BackPHInvoice_Click(object sender, EventArgs e)
        {
            OtherHeaderPanel.Visible = false;
            HComment.Enabled = true;
            FComment.Enabled = true;
        }

        private void panelProsesDetail_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DInvExdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


            if (DInvExdataGridView.Columns[e.ColumnIndex].HeaderText.ToLower() == "delete")
            {
                String Param = DInvExdataGridView["BIAYA", e.RowIndex].Value.ToString();


                var confirmResult = MessageBox.Show("Are you sure you want to delete this item ?", "Confirm Delete!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {

                    ErrorString = function.DeleteInvExpDetailMenu(IDDetailTxt.Text, Param);
                    DisplayInvDetailEx(IDDetailTxt.Text);
                }
            }
            else if (DInvExdataGridView.Columns[e.ColumnIndex].HeaderText.ToLower() == "edit")
            {
                String Param = DInvExdataGridView["BIAYA", e.RowIndex].Value.ToString();


                InvoiceParam Data = new InvoiceParam();
                InvoiceParam ExeData = new InvoiceParam();
                Data.IDD = IDDetailTxt.Text;
                Data.PARAMDESC = Param;

                try
                {
                    ExeData = function.getParameter(Data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }
                DExtSaveParamBtn.Visible = false;
                UpdInvPD.Visible = true;
                CancelBtnPD.Visible = true;
                IDParam.Text = ExeData.IDDP;
                DValueParamTxt.Text = ExeData.VALUE;
                DparamBox.Text = Data.PARAMDESC;
                DparamBox.SelectedValue = ExeData.PARAMID;
                DparamBox.Enabled = false;

            }
        }
        private void MToEditBtn_Click(object sender, EventArgs e)
        {
            if (dataGridViewFakturPajak.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                foreach (DataGridViewRow row in this.dataGridViewFakturPajak.SelectedRows)
                {
                    GlobalVar.GlobalVarKodeInvoice = row.Cells["KODE"].Value.ToString();


                }
                var confirmResult = MessageBox.Show("Are you sure you want to Edit Factur ?", "Edit Factur!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {


                    try
                    {
                        ErrorString = function.UpdateInvHeaderEdit(GlobalVar.GlobalVarKodeInvoice);
                        //ErrorString = function.UpdateArchiveDataHeader(DataID, "1");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }
                    SearchRefreshReport();

                }
            }
        }

        private void UpdDetailBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ErrorString = function.InsertInvDetail(IdHeaderDetailLbl.Text, InvoiceBox.Text, Dcomment.Text, "Update", DetailUsahaTxt.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            MessageBox.Show("Update Invoice Detail: " + InvHNewKodeTxt.Text, "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
            //DisplayInvDetailEx(InvoiceBox.SelectedValue.ToString());

            activePanel.Visible = false;
            activePanel = panelPaymentProcessNew;
            activePanel.Visible = true;
            Loadcombotype("Header");
            LoadSupplier(PKPBox);
            LoadPembeli(PBKPBox);
            HeaderVisible(IDKodeTxt.Text);

        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            InvHeader DataHdr = new InvHeader();
            DataHdr.KODE = InvHKodeBox.SelectedValue.ToString();
            DataHdr.IDPEMBELI = CmbKenaPajak.SelectedValue.ToString();
            DataHdr.IDPENGUSAHA = PembeliKenaBox.SelectedValue.ToString();
            DisplayHeader(DataHdr);
        }

        private void UpdInvPD_Click(object sender, EventArgs e)
        {
            try
            {
                ErrorString = "";


                ErrorString = function.InsertInvParamDetail(IDDetailTxt.Text, DValueParamTxt.Text, DparamBox.SelectedValue.ToString(), "Update");

                MessageBox.Show("Insert Parameter Detail: " + DparamBox.Text, ErrorString,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            DparamBox.Enabled = true;
            CancelBtnPD.Visible = false;
            DExtSaveParamBtn.Visible = true;
            UpdInvPD.Visible = false;
            DValueParamTxt.Text = "";
            DisplayInvDetailEx(IDDetailTxt.Text);
            RefreshDataDetail();

        }

        private void CancelBtnPD_Click(object sender, EventArgs e)
        {
            DparamBox.Enabled = true;
            CancelBtnPD.Visible = false;
            DExtSaveParamBtn.Visible = true;
            UpdInvPD.Visible = false;
            DValueParamTxt.Text = "";
        }



        private void RefreshDataDetail()
        {
            TotalInvoice Data = new TotalInvoice();

            try
            {
                Data = function.GetTotalInvoice(IDDetailTxt.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            TotalIDtxt.Text = Data.Total;
            TaxIDtxt.Text = Data.Tax;
            TotalExpDetail.Text = Data.TotalExp;
            TotalTaxIDtxt.Text = Data.TotalWithTax;





        }

        private void movePaymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activePanel.Visible = false;
            activePanel = MoveHistoryPanel;
            activePanel.Visible = true;
        }

        private void RunHistoryBtn_Click(object sender, EventArgs e)
        {
            try
            {

                ErrorString = function.MoveInvHeader(RunTxt.Text);

                MessageBox.Show("Move History: " + DparamBox.Text, ErrorString,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
        }

        private void PrintPreviewBtn_Click(object sender, EventArgs e)
        {


            if (HInvdataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                foreach (DataGridViewRow row in this.HInvdataGridView.SelectedRows)
                {
                    GlobalVar.GlobalVarKodeInvoice = row.Cells["KODE"].Value.ToString();


                }
                GlobalVar.GlobalVarPanel = panelPaymentProcess;



                InvoiceReportType = "FakturPajak";

                ShowPanelReportPreview();
            }
        }

        private void UpdateHinvBtn_Click(object sender, EventArgs e)
        {
            
            try
            {
                ErrorString = function.UpdateInvHeaderStatus(IDKodeTxt.Text, IStartDate.Text, IEndDate.Text, HComment.Text, FComment.Text, InvHNewKodeTxt.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            activePanel.Visible = false;
            activePanel = panelPaymentProcess;
            activePanel.Visible = true;
            InvKodeBox();
            InvJualBox();
            InvBeliBox();
            InvHeader DataHdr = new InvHeader();
            DataHdr.KODE = InvHKodeBox.SelectedValue.ToString();
            DataHdr.IDPEMBELI = CmbKenaPajak.SelectedValue.ToString();
            DataHdr.IDPENGUSAHA = PembeliKenaBox.SelectedValue.ToString();
            DisplayHeader(DataHdr);

        }
        private void OtherHeaderPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PPreviewBtn_Click(object sender, EventArgs e)
        {
            GlobalVar.GlobalVarKodeInvoice = InvHNewKodeTxt.Text;

            GlobalVar.GlobalVarPanel = panelPaymentProcessNew;



            InvoiceReportType = "FakturPajak";

            ShowPanelReportPreview();
        }


















































        #region Report Faktur Pajak
        private void buttonSearchFakturPajak_Click(object sender, EventArgs e)
        {

            SearchRefreshReport();

        }

        private void SearchRefreshReport()
        {
            FakturPajakSearch FPSearch = new FakturPajakSearch();

            FPSearch.KODE = textBoxKodeSeriFakturPajak.Text;
            FPSearch.IDPEMBELI = textBoxPembeli.Text;
            FPSearch.IDPENGUSAHA = textBoxPengusaha.Text;
            FPSearch.StartDate = FromDateFakturPajak.Value;
            FPSearch.EndDate = ToDateFakturPajak.Value;
            FPSearch.InvoiceNo = textBoxInvoiceNo.Text;


            DTFakturPajak = function.SelectFakturPajakConfirm(FPSearch, checkBoxInvoiceIncludeHistory.Checked);

            dataGridViewFakturPajak.DataSource = DTFakturPajak;

            dataGridViewFakturPajak.Columns["KODE"].MinimumWidth = 200;
            dataGridViewFakturPajak.Columns["PEMBELI"].MinimumWidth = 300;
            dataGridViewFakturPajak.Columns["PENGUSAHA"].MinimumWidth = 300;
        }

        #endregion

        private void dataGridViewFakturPajak_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
            }
            else
            {
                // Set cursor as hourglass
                Cursor.Current = Cursors.WaitCursor;

                Application.DoEvents();

                int rowIndex = e.RowIndex;
                DataGridViewRow row = dataGridViewFakturPajak.Rows[rowIndex];

                try
                {
                    InvoiceIsHistory = row.Cells["STATUS"].Value.ToString() == "CONFIRM HISTORY";

                    DTInvoiceDetail = InvoiceIsHistory ? function.selectTrxInvoiceByKODEHistory(row.Cells[0].Value.ToString()) : function.selectTrxInvoiceByKODE(row.Cells[0].Value.ToString());



                    GlobalVar.GlobalVarKodeInvoice = row.Cells[0].Value.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }



                dataGridViewInvoiceDetail.DataSource = DTInvoiceDetail;
                dataGridViewInvoiceDetail.Columns["COMMENT DETAIL"].MinimumWidth = 300;




                this.dataGridViewInvoiceDetail.Columns["BRUTO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //this.dataGridViewInvoiceDetail.Columns["NETTO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //this.dataGridViewInvoiceDetail.Columns["DISCOUNT BRUTO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //this.dataGridViewInvoiceDetail.Columns["DISCOUNT NETTO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;



                Cursor.Current = Cursors.Default;
            }
        }

        #region PrintInvoice

        private void buttonPrintFakturPajak_Click(object sender, EventArgs e)
        {
            if (dataGridViewFakturPajak.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                foreach (DataGridViewRow row in this.dataGridViewFakturPajak.SelectedRows)
                {
                    GlobalVar.GlobalVarKodeInvoice = row.Cells["KODE"].Value.ToString();

                    InvoiceIsHistory = row.Cells["STATUS"].Value.ToString() == "CONFIRM HISTORY";
                }
                GlobalVar.GlobalVarPanel = panelReportFakturPajak;

                FakturPajakIsPreview = false;

                ShowPanelPrintInvoice();
            }
        }

        private void ShowPanelPrintInvoice()
        {
            radioButtonFakturPajak.Checked = true;

            activePanel.Visible = false;
            activePanel = panelPrintInvoice;
            activePanel.Visible = true;

            //panelPrintInvoice.Visible = true;
            //GlobalVar.GlobalVarPanel.Enabled = false;

            groupBoxPrintInvoice.Visible = true;
            groupBoxSlip.Visible = false;
            groupBoxBiaya.Visible = false;
        }

        private void ShowPanelReportPreview()
        {
            activePanel.Visible = false;
            activePanel = panelPrintPreview;
            activePanel.Visible = true;

            if (InvoiceReportType == "FakturPajak")
            {
                FakturPajakIsPreview = true;
                PrintFakturPajak("null", FakturPajakIsPreview);
            }


        }

        private void HidePanelPrintPreview()
        {

            activePanel.Visible = false;
            activePanel = GlobalVar.GlobalVarPanel;
            activePanel.Visible = true;
        }

        private void HidePanelPrintInvoice()
        {

            //panelPrintInvoice.Visible = false;
            //GlobalVar.GlobalVarPanel.Enabled = true;

            activePanel.Visible = false;
            activePanel = GlobalVar.GlobalVarPanel;
            activePanel.Visible = true;


            groupBoxPrintInvoice.Visible = false;
        }

        private void radioButtonSlip_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxSlip.Visible = radioButtonSlip.Checked;
        }

        private void buttonPrintInvoicePrint_Click(object sender, EventArgs e)
        {

            if (radioButtonFakturPajak.Checked)
            {


                string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    path = Directory.GetParent(path).ToString();
                }
                String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
                path = path + "\\FakturPajak" + Exclude + ".pdf";
                PrintFakturPajak(path, FakturPajakIsPreview);
                HidePanelPrintInvoice();
                //saveFileDialogFakturPajak.ShowDialog();
            }
            else if (radioButtonBiaya.Checked)
            {
                if (BiayaRadio.Checked)
                {

                    string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        path = Directory.GetParent(path).ToString();
                    }
                    String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
                    path = path + "\\Biaya" + Exclude + ".pdf";
                    PrintBiaya(path);
                    HidePanelPrintInvoice();
                    //saveFileDialogBiaya.ShowDialog();
                }
                else if (PembayaranRadio.Checked)
                {

                    string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        path = Directory.GetParent(path).ToString();
                    }
                    String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
                    path = path + "\\Pembayaran" + Exclude + ".pdf";
                    PrintBiayaInvoice(path);
                    HidePanelPrintInvoice();
                    //saveFileDialogPembayaran.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please Tick one of the boxes", "Please choose one",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            else if (radioButtonSlip.Checked)
            {
                if (radioButtonRabuBank.Checked)
                {

                    string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        path = Directory.GetParent(path).ToString();
                    }
                    String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
                    path = path + "\\RaboBank" + Exclude + ".pdf";
                    PrintRaboBank(path);
                    HidePanelPrintInvoice();
                    //saveFileDialogRaboBank.ShowDialog();
                }
                else if (radioButtonBCATransfer.Checked)
                {

                    string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        path = Directory.GetParent(path).ToString();
                    }
                    String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
                    path = path + "\\BCATransfer" + Exclude + ".pdf";
                    PrintBCATransfer(path);
                    HidePanelPrintInvoice();

                    //saveFileDialogBCATransfer.ShowDialog();
                }
                else if (radioButtonBCATunai.Checked)
                {

                    string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        path = Directory.GetParent(path).ToString();
                    }
                    String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
                    path = path + "\\BCATunai" + Exclude + ".pdf";
                    PrintBCABuktiSetoran(path);

                    HidePanelPrintInvoice();
                    //saveFileDialogBCATunai.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please Tick one of the boxes", "Please choose one",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("Please Tick one of the boxes", "Please choose one",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void PrintFakturPajak(String Filename, bool IsPreview)
        {
            reportViewerFakturPajak.Visible = false;
            FakturPajakSearch FP = new FakturPajakSearch();

            try
            {
                FP = InvoiceIsHistory ? function.SelectFakturPajakByCodeHistorical(GlobalVar.GlobalVarKodeInvoice) : function.SelectFakturPajakByCode(GlobalVar.GlobalVarKodeInvoice);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            try
            {
                //Reset
                reportViewerFakturPajak.ProcessingMode = ProcessingMode.Local;
                reportViewerFakturPajak.Reset();

                //Data Source
                DataTable dt = new DataTable();

                dt = InvoiceIsHistory ? function.selectTrxInvoiceByKODEForReportHistory(FP.KODE) : function.selectTrxInvoiceByKODEForReport(FP.KODE);
                ReportDataSource rds = new ReportDataSource("DataSetTrxInvoice", dt);
                reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                dt = new DataTable();
                dt = InvoiceIsHistory ? function.selectCommentFakturPajakByKODEHistorical(FP.KODE) : function.selectCommentFakturPajakByKODE(FP.KODE);
                rds = new ReportDataSource("DataSetCommentHeader", dt);
                reportViewerFakturPajak.LocalReport.DataSources.Add(rds);

                dt = new DataTable();
                dt = InvoiceIsHistory ? function.selectExpenseFakturPajakByKODEHistorical(FP.KODE) : function.selectExpenseFakturPajakByKODE(FP.KODE);
                rds = new ReportDataSource("DataSetExpenseFakturPajak", dt);
                reportViewerFakturPajak.LocalReport.DataSources.Add(rds);



                InvHeader invH = new InvHeader();

                invH = InvoiceIsHistory ? function.getIdKodeHistorical(FP.KODE) : function.getIdKode(FP.KODE);



                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/FakturPajak.rdlc";

                //String Site = CMSfunction.GetSiteCodeandNameFromSiteCode(SalesSum.Site);

                //Parameter

                SupplierPembeli Pembeli = new SupplierPembeli();
                SupplierPembeli Penjual = new SupplierPembeli();

                Pembeli = function.SelectPembeliByID(FP.IDPEMBELI);
                Penjual = InvoiceIsHistory ? function.HistorySelectSupplierByID(FP.IDPENGUSAHA, invH.IDH) : function.SelectSupplierByID(FP.IDPENGUSAHA, invH.IDH);

                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("EndDate", FP.EndDate.ToString("dd-MMM-yyyy")),
                        new ReportParameter("StartDate", FP.StartDate.ToString("dd-MMM-yyyy")),
                        new ReportParameter("NPWPPengusahaKenaPajak", Penjual.NPWP),
                        new ReportParameter("CompanyNamePengusahaKenaPajak", Penjual.CompanyName),
                        new ReportParameter("CompanyAddressPengusahaKenaPajak", Penjual.Address),
                        new ReportParameter("FakturPajakNumber", FP.KODE),
                        new ReportParameter("NPWPPenerima", Pembeli.NPWP),
                        new ReportParameter("CompanyNamePenerima", Pembeli.CompanyName),
                        new ReportParameter("CompanyAddressPenerima", Pembeli.Address),
                        new ReportParameter("TotalInvWT", decimal.Parse(invH.TOTALINVWT).ToString("n")),
                        new ReportParameter("TotalFACWT", decimal.Parse(invH.TOTALFACWT).ToString("N")),
                        new ReportParameter("TotalFACT", decimal.Parse(invH.TOTALFACT).ToString("N")),
                        new ReportParameter("TotalFACNT", decimal.Parse(invH.TOTALFACNT).ToString("N")),
                        new ReportParameter("IsPreview", IsPreview.ToString()),
                        new ReportParameter("User", GlobalVar.GlobalVarUsername)
                    };

                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();
                reportViewerFakturPajak.RefreshReport();



                if (IsPreview)
                {
                    reportViewerFakturPajak.Visible = true;
                }
                else
                {
                    //Write to File
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string filenameExtension;

                    byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                        "PDF", null, out mimeType, out encoding, out filenameExtension,
                        out streamids, out warnings);

                    using (FileStream fs = new FileStream(Filename, FileMode.Create))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }





                System.Diagnostics.Process.Start(@"" + Filename + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

        }

        private void PrintBiaya(String Filename)
        {
            reportViewerFakturPajak.Visible = false;
            FakturPajakSearch FP = new FakturPajakSearch();
            try
            {
                FP = InvoiceIsHistory ? function.SelectFakturPajakByCodeHistorical(GlobalVar.GlobalVarKodeInvoice) : function.SelectFakturPajakByCode(GlobalVar.GlobalVarKodeInvoice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            try
            {
                //Reset
                reportViewerFakturPajak.Reset();

                //Data Source
                DataTable dt = new DataTable();

                dt = new DataTable();
                dt = InvoiceIsHistory ? function.selectExpenseFakturPajakByKODEHistorical(FP.KODE) : function.selectExpenseFakturPajakByKODE(FP.KODE);
                ReportDataSource rds = new ReportDataSource("DataSetExpenseFakturPajak", dt);
                reportViewerFakturPajak.LocalReport.DataSources.Add(rds);

                //Path  


                InvHeader invH = new InvHeader();

                invH = InvoiceIsHistory ? function.getIdKodeHistorical(FP.KODE) : function.getIdKode(FP.KODE);

                decimal? TotalExpenseTrxInvoice;

                TotalExpenseTrxInvoice = InvoiceIsHistory ? function.SumExpenseFakturPajakHistorical(invH.IDH) : function.SumExpenseFakturPajak(invH.IDH);


                reportViewerFakturPajak.LocalReport.ReportPath = TotalExpenseTrxInvoice < 500000 ? "Report/InvoiceLessThan500k.rdlc" : "Report/InvoiceGreaterThan500k.rdlc";

                //String Site = CMSfunction.GetSiteCodeandNameFromSiteCode(SalesSum.Site);

                //Parameter


                SupplierPembeli Supplier = new SupplierPembeli();
                ExpenseFakturPajak expenseFP = new ExpenseFakturPajak();


                expenseFP = InvoiceIsHistory ? function.selectExpenseFakturPajakTotalTerbilangByKODEHistorical(invH.IDH) : function.selectExpenseFakturPajakTotalTerbilangByKODE(invH.IDH);


                Supplier = function.SelectSupplierByID(FP.IDPENGUSAHA, invH.IDH);





                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("NPWPPengusahaKenaPajak", Supplier.NPWP),
                        new ReportParameter("CompanyNamePengusahaKenaPajak", Supplier.CompanyName),
                        new ReportParameter("CompanyAddressPengusahaKenaPajak", Supplier.Address),
                        new ReportParameter("FakturPajakNumber", FP.KODE),
                        new ReportParameter("Total", expenseFP.Total),
                        new ReportParameter("Terbilang", expenseFP.Terbilang),
                        new ReportParameter("LastModified", FP.LastModified.ToString("dd MMM yy")),
                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);



                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
                System.Diagnostics.Process.Start(@"" + Filename + "");
                //                System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)Filename);
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }
        }

        private void PrintBiayaInvoice(String Filename)
        {
            reportViewerFakturPajak.Visible = false;
            FakturPajakSearch FP = new FakturPajakSearch();
            try
            {
                FP = InvoiceIsHistory ? function.SelectFakturPajakByCodeHistorical(GlobalVar.GlobalVarKodeInvoice) : function.SelectFakturPajakByCode(GlobalVar.GlobalVarKodeInvoice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            try
            {
                //Reset
                reportViewerFakturPajak.Reset();

                //Data Source
                DataTable dt = new DataTable();

                dt = new DataTable();
                dt = InvoiceIsHistory ? function.selectExpenseFakturPajakByKODEHistorical(FP.KODE) : function.selectExpenseFakturPajakByKODE(FP.KODE);
                ReportDataSource rds = new ReportDataSource("DataSetExpenseFakturPajak", dt);
                reportViewerFakturPajak.LocalReport.DataSources.Add(rds);

                //Path  


                InvHeader invH = new InvHeader();

                invH = InvoiceIsHistory ? function.getIdKodeHistorical(FP.KODE) : function.getIdKode(FP.KODE);

                decimal? TotalExpenseTrxInvoice;

                TotalExpenseTrxInvoice = InvoiceIsHistory ? function.HistorySumFakturPajak(invH.IDH) : function.SumFakturPajak(invH.IDH);


                reportViewerFakturPajak.LocalReport.ReportPath = "Report/InvoiceTotal.rdlc";

                //String Site = CMSfunction.GetSiteCodeandNameFromSiteCode(SalesSum.Site);

                //Parameter


                SupplierPembeli Supplier = new SupplierPembeli();
                ExpenseFakturPajak expenseFP = new ExpenseFakturPajak();


                expenseFP = InvoiceIsHistory ? function.selectExpenseFakturPajakTotalTerbilangByKODEPembayaranHistorical(invH.IDH) : function.selectExpenseFakturPajakTotalTerbilangByKODEPembayaran(invH.IDH);



                Supplier = InvoiceIsHistory ? function.HistorySelectSupplierByID(FP.IDPENGUSAHA, invH.IDH) : function.SelectSupplierByID(FP.IDPENGUSAHA, invH.IDH);




                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("NPWPPengusahaKenaPajak", Supplier.NPWP),
                        new ReportParameter("CompanyNamePengusahaKenaPajak", Supplier.CompanyName),
                        new ReportParameter("CompanyAddressPengusahaKenaPajak", Supplier.Address),
                        new ReportParameter("FakturPajakNumber", FP.KODE),
                        new ReportParameter("Total", decimal.Parse(expenseFP.Total.ToString()).ToString("N")),
                        new ReportParameter("Terbilang", expenseFP.Terbilang),
                        new ReportParameter("LastModified", FP.LastModified.ToString("dd MMM yy")),
                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + Filename + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }
        }


        private void PrintBCABuktiSetoran(String Filename)
        {
            reportViewerFakturPajak.Visible = false;
            FakturPajakSearch FP = new FakturPajakSearch();

            try
            {
                FP = InvoiceIsHistory ? function.SelectFakturPajakByCodeHistorical(GlobalVar.GlobalVarKodeInvoice) : function.SelectFakturPajakByCode(GlobalVar.GlobalVarKodeInvoice);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            try
            {
                //Reset
                reportViewerFakturPajak.Reset();

                //Data Source
                //DataTable dt = new DataTable();

                //dt = function.selectTrxInvoiceByKODEForReport(FP.KODE);
                //ReportDataSource rds = new ReportDataSource("DataSetTrxInvoice", dt);
                //reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                //dt = new DataTable();
                //dt = function.selectCommentFakturPajakByKODE(FP.KODE);
                //rds = new ReportDataSource("DataSetCommentHeader", dt);
                //reportViewerFakturPajak.LocalReport.DataSources.Add(rds);

                //dt = new DataTable();
                //dt = function.selectExpenseFakturPajakByKODE(FP.KODE);
                //rds = new ReportDataSource("DataSetExpenseFakturPajak", dt);
                //reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/Bank/BCABuktiSetoran.rdlc";

                InvHeader invH = new InvHeader();

                invH = InvoiceIsHistory ? function.getIdKodeHistorical(FP.KODE) : function.getIdKode(FP.KODE);

                //String Site = CMSfunction.GetSiteCodeandNameFromSiteCode(SalesSum.Site);

                //Parameter

                SupplierPembeli Pembeli = new SupplierPembeli();
                SupplierPembeli Penjual = new SupplierPembeli();

                Pembeli = function.SelectPembeliByID(FP.IDPEMBELI);
                Penjual = InvoiceIsHistory ? function.HistorySelectSupplierByID(FP.IDPENGUSAHA, invH.IDH) : function.SelectSupplierByID(FP.IDPENGUSAHA, invH.IDH);

                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("Date", FP.EndDate.ToString("dd-MMM-yyyy")),
                        new ReportParameter("NPWPPenerima", Pembeli.NPWP),
                        new ReportParameter("CompanyNamePenerima", Penjual.CompanyName),
                        new ReportParameter("CompanyAddressPenerima", Penjual.Address),
                        new ReportParameter("CompanyNamePengirim", Pembeli.CompanyName),
                        new ReportParameter("CompanyAddressPengirim", Pembeli.Address),
                        new ReportParameter("NoRekeningCustomer", Penjual.NoRek),
                        new ReportParameter("Terbilang", invH.TERBILANGTotalFacNT),
                        new ReportParameter("Total", decimal.Parse(invH.LASTTOTAL).ToString("N")),
                        new ReportParameter("FakturPajakNumber", FP.KODE),

                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + Filename + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

        }

        private void PrintBCATransfer(String Filename)
        {
            reportViewerFakturPajak.Visible = false;
            FakturPajakSearch FP = new FakturPajakSearch();

            try
            {
                FP = InvoiceIsHistory ? function.SelectFakturPajakByCodeHistorical(GlobalVar.GlobalVarKodeInvoice) : function.SelectFakturPajakByCode(GlobalVar.GlobalVarKodeInvoice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            try
            {
                //Reset
                reportViewerFakturPajak.Reset();

                //Data Source
                //DataTable dt = new DataTable();

                //dt = function.selectTrxInvoiceByKODEForReport(FP.KODE);
                //ReportDataSource rds = new ReportDataSource("DataSetTrxInvoice", dt);
                //reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                //dt = new DataTable();
                //dt = function.selectCommentFakturPajakByKODE(FP.KODE);
                //rds = new ReportDataSource("DataSetCommentHeader", dt);
                //reportViewerFakturPajak.LocalReport.DataSources.Add(rds);

                //dt = new DataTable();
                //dt = function.selectExpenseFakturPajakByKODE(FP.KODE);
                //rds = new ReportDataSource("DataSetExpenseFakturPajak", dt);
                //reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/Bank/BCATransfer.rdlc";

                InvHeader invH = new InvHeader();

                invH = InvoiceIsHistory ? function.getIdKodeHistorical(FP.KODE) : function.getIdKode(FP.KODE);

                //String Site = CMSfunction.GetSiteCodeandNameFromSiteCode(SalesSum.Site);

                //Parameter

                SupplierPembeli Pembeli = new SupplierPembeli();
                SupplierPembeli Penjual = new SupplierPembeli();

                Pembeli = function.SelectPembeliByID(FP.IDPEMBELI);
                Penjual = InvoiceIsHistory ? function.HistorySelectSupplierByID(FP.IDPENGUSAHA, invH.IDH) : function.SelectSupplierByID(FP.IDPENGUSAHA, invH.IDH);
                
                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("Date", FP.EndDate.ToString("dd-MMM-yyyy")),
                        new ReportParameter("NPWPPenerima", Pembeli.NPWP),
                        new ReportParameter("CompanyNamePenerima", Penjual.CompanyName),
                        new ReportParameter("CompanyAddressPenerima", Penjual.Address),
                        new ReportParameter("CompanyNamePengirim", Pembeli.CompanyName),
                        new ReportParameter("CompanyAddressPengirim", Pembeli.Address),
                        new ReportParameter("NoRekeningCustomer", Penjual.NoRek),
                        new ReportParameter("Terbilang", invH.TERBILANGTotalFacNT),
                        new ReportParameter("Total", decimal.Parse(invH.LASTTOTAL).ToString("N")),
                        new ReportParameter("FakturPajakNumber", FP.KODE),
                        new ReportParameter("Bank", Penjual.Bank),
                        new ReportParameter("BankAddress", Penjual.BankAddress),

                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + Filename + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

        }

        private void PrintRaboBank(String Filename)
        {
            reportViewerFakturPajak.Visible = false;
            FakturPajakSearch FP = new FakturPajakSearch();

            try
            {
                FP = InvoiceIsHistory ? function.SelectFakturPajakByCodeHistorical(GlobalVar.GlobalVarKodeInvoice) : function.SelectFakturPajakByCode(GlobalVar.GlobalVarKodeInvoice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            try
            {
                //Reset
                reportViewerFakturPajak.Reset();

                //Data Source
                //DataTable dt = new DataTable();

                //dt = function.selectTrxInvoiceByKODEForReport(FP.KODE);
                //ReportDataSource rds = new ReportDataSource("DataSetTrxInvoice", dt);
                //reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                //dt = new DataTable();
                //dt = function.selectCommentFakturPajakByKODE(FP.KODE);
                //rds = new ReportDataSource("DataSetCommentHeader", dt);
                //reportViewerFakturPajak.LocalReport.DataSources.Add(rds);

                //dt = new DataTable();
                //dt = function.selectExpenseFakturPajakByKODE(FP.KODE);
                //rds = new ReportDataSource("DataSetExpenseFakturPajak", dt);
                //reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/Bank/RaboBank.rdlc";

                InvHeader invH = new InvHeader();

                invH = InvoiceIsHistory ? function.getIdKodeHistorical(FP.KODE) : function.getIdKode(FP.KODE);

                //String Site = CMSfunction.GetSiteCodeandNameFromSiteCode(SalesSum.Site);

                //Parameter

                SupplierPembeli Pembeli = new SupplierPembeli();
                SupplierPembeli Penjual = new SupplierPembeli();

                Pembeli = function.SelectPembeliByID(FP.IDPEMBELI);
                Penjual = InvoiceIsHistory ? function.HistorySelectSupplierByID(FP.IDPENGUSAHA, invH.IDH) : function.SelectSupplierByID(FP.IDPENGUSAHA, invH.IDH);


                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("Date", FP.EndDate.ToString("dd-MMM-yyyy")),
                        new ReportParameter("NPWPPengirim", Pembeli.NPWP),
                        new ReportParameter("CompanyNamePenerima", Penjual.CompanyName),
                        new ReportParameter("CompanyAddressPenerima", Penjual.Address),                        
                        new ReportParameter("CompanyNamePengirim", Pembeli.CompanyName),
                        new ReportParameter("CompanyAddressPengirim", Pembeli.Address),
                        new ReportParameter("NoRekeningCustomer", Penjual.NoRek),
                        new ReportParameter("Terbilang", invH.TERBILANGTotalFacNT),
                        new ReportParameter("Total", decimal.Parse(invH.LASTTOTAL).ToString("N")),
                        new ReportParameter("FakturPajakNumber", FP.KODE),
                        new ReportParameter("BANK",  Penjual.Bank),
                        new ReportParameter("BankAddress", Penjual.BankAddress),

                         

                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + Filename + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

        }

        private void PrintInvoiceDetailDetail(String Filename)
        {
            reportViewerFakturPajak.Visible = false;
            FakturPajakSearch FP = new FakturPajakSearch();

            try
            {
                FP = InvoiceIsHistory ? function.SelectFakturPajakByCodeHistorical(GlobalVar.GlobalVarKodeInvoice) : function.SelectFakturPajakByCode(GlobalVar.GlobalVarKodeInvoice);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            InvDetail invDet = new InvDetail();
            invDet = InvoiceIsHistory ? function.getIdInvoiceDetailHistorical(InvoiceNo) : function.getIdInvoiceDetail(InvoiceNo);



            try
            {
                //Reset
                reportViewerFakturPajak.Reset();

                //Data Source
                DataTable dt = new DataTable();

                dt = function.SelectInvoiceDetailDetail(invDet.SUPPLIER, InvoiceNo);
                ReportDataSource rds = new ReportDataSource("DataSetInvoiceDetailDetail", dt);
                reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                dt = new DataTable();
                dt = InvoiceIsHistory ? function.selectExpenseFakturPajakByIDDHistorical(invDet.IDD) : function.selectExpenseFakturPajakByIDD(invDet.IDD);
                rds = new ReportDataSource("DataSetExpenseFakturPajak", dt);
                reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                InvoiceDetailDetailHeader header = new InvoiceDetailDetailHeader();
                header = function.SelectInvoiceDetailDetailHeader(invDet.SITE);

                InvoiceDetailDetailHeaderData headerData = new InvoiceDetailDetailHeaderData();
                headerData = function.SelectInvoiceDetailDetailHeaderData(FP.StartDate, FP.EndDate, invDet.SITE,
                    invDet.SUPPLIER);

                headerData.AttTo = "N/A";
                headerData.NamaBarang = invDet.COMMERCIAL;

                Decimal? SubTotalWithoutExpense = function.SumInvoiceDetailDetail(invDet.SUPPLIER, InvoiceNo);
                if (SubTotalWithoutExpense == null)
                {
                    SubTotalWithoutExpense = InvoiceIsHistory ? function.SumExpenseTrxTotalInvoiceHistorical(invDet.IDD) : function.SumExpenseTrxTotalInvoice(invDet.IDD);
                }
                Decimal? TotalExpense;

                TotalExpense = InvoiceIsHistory ? function.SumExpenseTrxInvoiceHistorical(invDet.IDD) : function.SumExpenseTrxInvoice(invDet.IDD);

                Decimal? A = 110;

                Decimal? B = 100;

                Decimal? C = A / B;

                Decimal? SubTotal = SubTotalWithoutExpense / C;

                Decimal? Tax = SubTotalWithoutExpense - SubTotal;

                Decimal? TotalAftTax = SubTotalWithoutExpense;

                Decimal? HasilAkhir = SubTotalWithoutExpense - TotalExpense;

                headerData.SubTotal = Decimal.Parse(SubTotal.ToString()).ToString("N");
                headerData.Tax = Decimal.Parse(Tax.ToString()).ToString("N");
                headerData.TotalAfterTax = Decimal.Parse(TotalAftTax.ToString()).ToString("N");
                headerData.TotalAkhir = Decimal.Parse(HasilAkhir.ToString()).ToString("N");



                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/TransactionDetail.rdlc";

                //String Site = CMSfunction.GetSiteCodeandNameFromSiteCode(SalesSum.Site);

                //Parameter




                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("Outlet", header.OUTLET),
                        new ReportParameter("OutletAddress", header.ADDRESS),
                        new ReportParameter("SupplierName", headerData.SupplierName),
                        new ReportParameter("Branch", headerData.Cabang),
                        new ReportParameter("NamaBarang", headerData.NamaBarang),
                        new ReportParameter("PeriodePenjualan", headerData.PeriodePenjualan),
                        new ReportParameter("AttTo", headerData.AttTo),
                        new ReportParameter("Subtotal", headerData.SubTotal),
                        new ReportParameter("Tax", headerData.Tax),
                        new ReportParameter("TotalWithTax", headerData.TotalAfterTax),
                        new ReportParameter("TotalAkhir", headerData.TotalAkhir),
                        new ReportParameter("CommentDetail", invDet.COMMENT ),
                        
                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + Filename + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

        }

        private void PrintInvoiceDetailSummary(String Filename)
        {
            reportViewerFakturPajak.Visible = false;
            FakturPajakSearch FP = new FakturPajakSearch();

            try
            {
                FP = InvoiceIsHistory ? function.SelectFakturPajakByCodeHistorical(GlobalVar.GlobalVarKodeInvoice) : function.SelectFakturPajakByCode(GlobalVar.GlobalVarKodeInvoice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            InvDetail invDet = new InvDetail();

            invDet = InvoiceIsHistory ? function.getIdInvoiceDetailHistorical(InvoiceNo) : function.getIdInvoiceDetail(InvoiceNo);


            try
            {
                //Reset
                reportViewerFakturPajak.Reset();

                //Data Source
                DataTable dt = new DataTable();

                dt = function.SelectInvoiceDetailSummary(invDet.SUPPLIER, InvoiceNo);
                ReportDataSource rds = new ReportDataSource("DataSetInvoiceDetailSummary", dt);
                reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                dt = new DataTable();
                dt = InvoiceIsHistory ? function.selectExpenseFakturPajakByIDDHistorical(invDet.IDD) : function.selectExpenseFakturPajakByIDD(invDet.IDD);
                rds = new ReportDataSource("DataSetExpenseFakturPajak", dt);
                reportViewerFakturPajak.LocalReport.DataSources.Add(rds);


                InvoiceDetailDetailHeader header = new InvoiceDetailDetailHeader();
                header = function.SelectInvoiceDetailDetailHeader(invDet.SITE);

                InvoiceDetailDetailHeaderData headerData = new InvoiceDetailDetailHeaderData();
                headerData = function.SelectInvoiceDetailDetailHeaderData(FP.StartDate, FP.EndDate, invDet.SITE,
                    invDet.SUPPLIER);

                headerData.AttTo = "N/A";
                headerData.NamaBarang = invDet.COMMERCIAL;

                Decimal? SubTotalWithoutExpense = function.SumInvoiceDetailDetail(invDet.SUPPLIER, InvoiceNo);
                if (SubTotalWithoutExpense == null)
                {
                    SubTotalWithoutExpense = InvoiceIsHistory ? function.SumExpenseTrxTotalInvoiceHistorical(invDet.IDD) : function.SumExpenseTrxTotalInvoice(invDet.IDD);
                }

                Decimal? TotalExpense;

                TotalExpense = InvoiceIsHistory ? function.SumExpenseTrxInvoiceHistorical(invDet.IDD) : function.SumExpenseTrxInvoice(invDet.IDD);

                Decimal? A = 110;

                Decimal? B = 100;

                Decimal? C = A / B;

                Decimal? SubTotal = SubTotalWithoutExpense / C;

                Decimal? Tax = SubTotalWithoutExpense - SubTotal;

                Decimal? TotalAftTax = SubTotalWithoutExpense;

                Decimal? HasilAkhir = SubTotalWithoutExpense - TotalExpense;

                headerData.SubTotal = Decimal.Parse(SubTotal.ToString()).ToString("N");
                headerData.Tax = Decimal.Parse(Tax.ToString()).ToString("N");
                headerData.TotalAfterTax = Decimal.Parse(TotalAftTax.ToString()).ToString("N");
                headerData.TotalAkhir = Decimal.Parse(HasilAkhir.ToString()).ToString("N");


                InvHeader invH = new InvHeader();



                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/TransactionSummary.rdlc";

                //String Site = CMSfunction.GetSiteCodeandNameFromSiteCode(SalesSum.Site);

                //Parameter




                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("Outlet", header.OUTLET),
                        new ReportParameter("OutletAddress", header.ADDRESS),
                        new ReportParameter("SupplierName", headerData.SupplierName),
                        new ReportParameter("Branch", headerData.Cabang),
                        new ReportParameter("NamaBarang", headerData.NamaBarang),
                        new ReportParameter("PeriodePenjualan", headerData.PeriodePenjualan),
                        new ReportParameter("AttTo", headerData.AttTo),
                        new ReportParameter("Subtotal", headerData.SubTotal),
                        new ReportParameter("Tax", headerData.Tax),
                        new ReportParameter("TotalWithTax", headerData.TotalAfterTax),
                        new ReportParameter("TotalAkhir", headerData.TotalAkhir),
                        new ReportParameter("CommentDetail", invDet.COMMENT ),
                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + Filename + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

        }

        private void saveFileDialogFakturPajak_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = saveFileDialogFakturPajak.FileName;
            PrintFakturPajak(fileName, FakturPajakIsPreview);
            HidePanelPrintInvoice();
        }

        private void saveFileDialogPembayaran_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = saveFileDialogPembayaran.FileName;
            PrintBiayaInvoice(fileName);
            HidePanelPrintInvoice();
        }

        private void saveFileDialogBiaya_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = saveFileDialogBiaya.FileName;
            PrintBiaya(fileName);
            HidePanelPrintInvoice();
        }

        private void saveFileDialogBCATunai_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = saveFileDialogBCATunai.FileName;
            PrintBCABuktiSetoran(fileName);

            HidePanelPrintInvoice();
        }

        #endregion

        private void UpdHParamBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ErrorString = function.InsertInvParamHeader(IDKodeTxt.Text, PHValueBox.Text, PHTypeBox.SelectedValue.ToString(), "Update");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


            PHValueBox.Text = "";
            DisplayInvDetail(IDKodeTxt.Text);
            DisplayHeaderEx(IDKodeTxt.Text);
            UpdHParamBtn.Visible = false;
            SaveHParamBtn.Visible = true;
            HeaderVisible(IDKodeTxt.Text);
        }

        private void InvHConfirmBtn_Click(object sender, EventArgs e)
        {

            var confirmResult = MessageBox.Show("Are you sure you want to Confirm Factur ?", "Confirm Factur!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {


                try
                {
                    ErrorString = function.UpdateInvHeader(IDKodeTxt.Text);
                    ErrorString = function.UpdateArchiveDataHeader(IDKodeTxt.Text, "2");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(ex.Message);
                }

                GlobalVar.GlobalVarKodeInvoice = InvHNewKodeTxt.Text;
                GlobalVar.GlobalVarPanel = panelPaymentProcess;

                FakturPajakIsPreview = false;
                ShowPanelPrintInvoice();

                InvKodeBox();
                InvJualBox();
                InvBeliBox();
                InvHeader DataHdr = new InvHeader();
                DataHdr.KODE = InvHKodeBox.SelectedValue.ToString();
                DataHdr.IDPEMBELI = CmbKenaPajak.SelectedValue.ToString();
                DataHdr.IDPENGUSAHA = PembeliKenaBox.SelectedValue.ToString();
                DisplayHeader(DataHdr);
            }
        }


        private void buttonPrintInvoiceCancel_Click(object sender, EventArgs e)
        {
            HidePanelPrintInvoice();
        }

        private void buttonPrintDetailSummary_Click(object sender, EventArgs e)
        {
            if (dataGridViewInvoiceDetail.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                foreach (DataGridViewRow row in this.dataGridViewInvoiceDetail.SelectedRows)
                {
                    InvoiceNo = row.Cells["SKUID"].Value.ToString();


                }
                string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    path = Directory.GetParent(path).ToString();
                }
                String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
                path = path + "\\ReportSummary" + Exclude + ".pdf";

                PrintInvoiceDetailSummary(path);
                //saveFileDialogInvoiceDetailSummary.ShowDialog();
            }
        }

        private void buttonPrintDetailDetail_Click(object sender, EventArgs e)
        {
            if (dataGridViewInvoiceDetail.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                foreach (DataGridViewRow row in this.dataGridViewInvoiceDetail.SelectedRows)
                {
                    InvoiceNo = row.Cells["SKUID"].Value.ToString();


                }
                string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    path = Directory.GetParent(path).ToString();
                }
                String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
                path = path + "\\ReportSummary" + Exclude + ".pdf";
                PrintInvoiceDetailDetail(path);
                //saveFileDialogInvoiceDetailDetail.ShowDialog();
            }
        }

        private void saveFileDialogBCATransfer_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = saveFileDialogBCATransfer.FileName;
            PrintBCATransfer(fileName);
            HidePanelPrintInvoice();
        }

        private void saveFileDialogRaboBank_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = saveFileDialogRaboBank.FileName;
            PrintRaboBank(fileName);
            HidePanelPrintInvoice();
        }

        private void saveFileDialogInvoiceDetailSummary_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = saveFileDialogInvoiceDetailSummary.FileName;
            PrintInvoiceDetailSummary(fileName);
        }

        private void saveFileDialogInvoiceDetailDetail_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = saveFileDialogInvoiceDetailDetail.FileName;
            PrintInvoiceDetailDetail(fileName);

        }

        private void buttonBackPreview_Click(object sender, EventArgs e)
        {
            HidePanelPrintPreview();
        }

        private void buttonPrintPreviewFakturPajak_Click(object sender, EventArgs e)
        {
            if (dataGridViewFakturPajak.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                foreach (DataGridViewRow row in this.dataGridViewFakturPajak.SelectedRows)
                {
                    GlobalVar.GlobalVarKodeInvoice = row.Cells["KODE"].Value.ToString();


                }
                GlobalVar.GlobalVarPanel = panelReportFakturPajak;



                InvoiceReportType = "FakturPajak";

                ShowPanelReportPreview();
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            // Check if the pressed character was a backspace or numeric.
            if (e.KeyChar != (char)8 && !char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dataGridViewSalesHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void radioButtonBiaya_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxBiaya.Visible = radioButtonBiaya.Checked;
        }

        private void DetailSummaryBtn_Click(object sender, EventArgs e)
        {
            GlobalVar.GlobalVarKodeInvoice = DetailKodeTxt.Text;
            InvoiceNo = InvoiceBox.Text;
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
            String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
            path = path + "\\ReportSummary" + Exclude + ".pdf";
            PrintInvoiceDetailSummary(path);
            // saveFileDialogInvoiceDetailSummary.ShowDialog();
        }

        private void DetailRptBtn_Click(object sender, EventArgs e)
        {
            GlobalVar.GlobalVarKodeInvoice = DetailKodeTxt.Text;
            InvoiceNo = InvoiceBox.Text;
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
            String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
            path = path + "\\ReportDetail" + Exclude + ".pdf";
            PrintInvoiceDetailDetail(path);
            //saveFileDialogInvoiceDetailDetail.ShowDialog();

        }

       


        #region SlipPembayaran
        private void SearchSlipBtn_Click(object sender, EventArgs e)
        {
            SearchDataSlip();

        }

        private void SearchDataSlip()
        {
            try
            {
                FakturPajakSearch FPSearch = new FakturPajakSearch();
                if (PembeliBox.SelectedValue.ToString() != "9999")
                {
                    FPSearch.IDPEMBELI = PembeliBox.SelectedValue.ToString();
                }
                else
                {
                    FPSearch.IDPEMBELI = "";
                }
                FPSearch.IDPENGUSAHA = PengusahaBox.SelectedValue.ToString();

                FPSearch.StartDate = StartSlipDate.Value;
                FPSearch.EndDate = EndSlipDate.Value;


                DTFakturPajak = function.SelectFakturPajakConfirmSlip(FPSearch);

                dataGridSlip.DataSource = DTFakturPajak;
                dataGridSlip.Columns["IDPEMBELI"].Visible = false;
                dataGridSlip.Columns["IDPENGUSAHA"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Data Field Salah, Mohon diisi dengan benar",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                logger.Error(ex.Message);
            }

        }




        private void printSlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshSlipHeader();
            
        }
        private void RefreshSlipHeader()
        {
            activePanel.Visible = false;
            activePanel = panelSlipPembayaran;
            activePanel.Visible = true;

            try
            {
                DTParameterTypePengusaha = function.SelectSupplierSlip();
                PengusahaBox.DataSource = DTParameterTypePengusaha;
                PengusahaBox.DisplayMember = "KET";
                PengusahaBox.ValueMember = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }

            try
            {
                DTParameterTypePembeli = function.SelectPembeliSlip();
                PembeliBox.DataSource = DTParameterTypePembeli;
                PembeliBox.DisplayMember = "KET";
                PembeliBox.ValueMember = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            SlipPanelSetting("Started");
            SearchDataSlip();
        }
        private void SlipPanelSetting(String Status)
        {
            if (Status == "Started")
            {
                panelSlipNewInvoice.Visible = false;
                panelSlipBtn.Visible = true;
                PembeliBox.Enabled = true;
                PengusahaBox.Enabled = true;
                StartSlipDate.Enabled = true;
                EndSlipDate.Enabled = true;
            }
            else
            {
                panelSlipNewInvoice.Visible = true;
                panelSlipBtn.Visible = false;
                PembeliBox.Enabled = false;
                PengusahaBox.Enabled = false;
                StartSlipDate.Enabled = false;
                EndSlipDate.Enabled = false;
                if (Status == "New")
                {
                    SaveSlipBtn.Visible = true;
                    UpdateSlipBtn.Visible = false;
                    panelSlipRekening.Visible = false;
                    TotalInvoiceTxt.Text = "0";
                    TransferTxt.Text = "0";
                    TotalInvoiceSaveTxt.Text = "0";
                    LastValueTxt.Text = "0";
                    CompanyTxt.Text = "";
                    DescPayTxt.Text = "";
                }
                else
                {
                    SaveSlipBtn.Visible = false;
                    UpdateSlipBtn.Visible = true;
                    panelSlipRekening.Visible = true;

                }
            }
        }

        private void AddSlipBtn_Click(object sender, EventArgs e)
        {
            if ((PembeliBox.SelectedValue.ToString() == "9999") || (PembeliBox.SelectedValue.ToString() == "9999"))
            {
                MessageBox.Show("Please One Select Supplyer", "Error Occured",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ManualRek.Checked = false;
                SlipPanelSetting("New");
                panelSlipRekening.Visible = false;
                FakturPajakSearch FPSearch = new FakturPajakSearch();
                FPSearch.IDPEMBELI = PembeliBox.SelectedValue.ToString();
                FPSearch.IDPENGUSAHA = PengusahaBox.SelectedValue.ToString();

                FPSearch.StartDate = StartSlipDate.Value;
                FPSearch.EndDate = EndSlipDate.Value;


                DTFakturPajak = function.SelectInvoiceSlip(FPSearch, "New");
                dataGridInvoice.Columns.Clear();
                dataGridInvoice.DataSource = DTFakturPajak;
                dataGridInvoice.Columns["FLAG"].Visible = false;
                DataGridViewCheckBoxColumn SlipCmbBox = new DataGridViewCheckBoxColumn();
                SlipCmbBox.ValueType = typeof(bool);
                SlipCmbBox.Name = "SELECTED";
                SlipCmbBox.HeaderText = "SELECTED";
                dataGridInvoice.Columns.Add(SlipCmbBox);
                DescPayTxt.Focus();
            }
        }


        private void dataGridInvoice_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            TotalInvoiceTxt.Text = "0";
            TransferTxt.Text = "0";
            if (Convert.ToBoolean(dataGridInvoice["SELECTED", e.RowIndex].Value) == true)
            {
                dataGridInvoice["SELECTED", e.RowIndex].Value = false;
                String NoKode = "''";
                List<DataGridViewRow> CheckedData = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridInvoice.Rows)
                {

                    if (Convert.ToBoolean(row.Cells["SELECTED"].Value) == true)
                    {
                        CheckedData.Add(row);

                        NoKode = NoKode + ", '" + row.Cells["KODE"].Value.ToString() + "'";
                        UpdateTotal(NoKode);

                    }
                }
            }
            else
            {
                dataGridInvoice["SELECTED", e.RowIndex].Value = true;
                //Generate Total
                String NoKode = "''";
                List<DataGridViewRow> CheckedData = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridInvoice.Rows)
                {

                    if (Convert.ToBoolean(row.Cells["SELECTED"].Value) == true)
                    {
                        CheckedData.Add(row);

                        NoKode = NoKode + ", '" + row.Cells["KODE"].Value.ToString() + "'";
                        UpdateTotal(NoKode);

                    }
                }
                //End Generate 
            }
        }
        private void UpdateTotal(String Kode)
        {
            reportViewerFakturPajak.Visible = false;

            InvHeader invH = new InvHeader();

            try
            {
                invH = function.getIdKodeSlip(Kode);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }
            TotalInvoiceTxt.Text = invH.LASTTOTAL;
            TotalInvoiceSaveTxt.Text = invH.TOTALDATAINV;
            TotalInvoiceTxt.Text = (decimal.Parse(TotalInvoiceTxt.Text) - decimal.Parse(TransferTxt.Text)).ToString();

        }

        private void SaveSlipBtn_Click(object sender, EventArgs e)
        {
            if (DescPayTxt.Text != "")
            {
                SaveMasterBayar("Insert");
                panelSlipRekening.Visible = true;
                SlipPanelSetting("Save");
            }
            else
            {
                MessageBox.Show("Please Insert Description", "Warning !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveMasterBayar(String Status)
        {
            String Result = "";
            FakturPajakSearch FPSearch = new FakturPajakSearch();
            FPSearch.IDPEMBELI = PembeliBox.SelectedValue.ToString();
            FPSearch.IDPENGUSAHA = PengusahaBox.SelectedValue.ToString();
            FPSearch.StartDate = StartSlipDate.Value;
            FPSearch.EndDate = EndSlipDate.Value;
            FPSearch.InvoiceNo = DescPayTxt.Text;
            FPSearch.Total = TotalInvoiceSaveTxt.Text;
            FPSearch.Biaya = TransferTxt.Text;

            try
            {
                if (Status == "Insert")
                {
                    //Insert Data
                    Result = function.InsertInvoicePembayaran(FPSearch, Status);
                    NoPembayaranTxt.Text = function.NoInvoicePembayaran(FPSearch);

                }
                else
                {
                    FPSearch.No = NoPembayaranTxt.Text;
                    Result = function.InsertInvoicePembayaran(FPSearch, "Update Data");
                }
                if (Result == "Success")
                {
                    //Get Data                   
                    String NoKode = "''";
                    List<DataGridViewRow> CheckedData = new List<DataGridViewRow>();
                    foreach (DataGridViewRow row in dataGridInvoice.Rows)
                    {

                        if (Convert.ToBoolean(row.Cells["SELECTED"].Value) == true)
                        {
                            CheckedData.Add(row);

                            NoKode = NoKode + ", '" + row.Cells["KODE"].Value.ToString() + "'";
                        }
                    }
                    Result = function.UpdateFakturInvoicePembayaran(NoPembayaranTxt.Text, NoKode);
                }


                LastValueTxt.Text = TotalInvoiceSaveTxt.Text;
                //Update Flag Faktur
                MessageBox.Show("Data Has Been Update", "Success", MessageBoxButtons.OK);
                //Insert Data Rekening
                SupplierPembeli Pembeli = new SupplierPembeli();
                Pembeli = function.SelectPembeliByID(PembeliBox.SelectedValue.ToString());
                AdPengirimTxt.Text = Pembeli.Address;
                NPWPPengirimTxt.Text = Pembeli.NPWP;
                KrmBox.Text = Pembeli.CompanyName;

                DTParameterType = function.SelectDataPenerimaSlip(PengusahaBox.SelectedValue.ToString());
                RekTrmBox.DataSource = DTParameterType;
                RekTrmBox.DisplayMember = "NoRek";
                RekTrmBox.ValueMember = "NoRek";
                RefreshDataRekening();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


        }

        private void RefreshDataRekening()
        {

            if (RekTrmBox.Text != "Not Found")
            {
                SupplierPembeli Penjual = new SupplierPembeli();
                Penjual = function.SelectDataPenerimaDetailSlip(PengusahaBox.SelectedValue.ToString(), RekTrmBox.Text);

                ANPenerimaTxt.Text = Penjual.AN;
                BankPenerimaTxt.Text = Penjual.Bank;
                AdPenerimaTxt.Text = Penjual.Address;
                CompanyTxt.Text = Penjual.CompanyName;
            }
            else
            {
                ANPenerimaTxt.Text = "";
                BankPenerimaTxt.Text = "";
                AdPenerimaTxt.Text = "";
                CompanyTxt.Text = "";
            }

        }
        private void ConfirmSlipBtn_Click(object sender, EventArgs e)
        {

            try
            {
                FakturPajakSearch FPSearch = new FakturPajakSearch();
                FPSearch.No = NoPembayaranTxt.Text;
                if (ManualRek.Checked != true)
                {
                    FPSearch.STATUS = "0";
                    FPSearch.NoRekPenerima = RekTrmBox.SelectedValue.ToString();
                }
                else
                {
                    FPSearch.STATUS = "1";
                    FPSearch.NoRekPenerima = RekManual.Text;
                }
                FPSearch.Pengirim = KrmBox.Text;
                FPSearch.NPWP = NPWPPengirimTxt.Text;
                FPSearch.AdPengirim = AdPengirimTxt.Text;

                FPSearch.ANPenerima = ANPenerimaTxt.Text;
                FPSearch.BankPenerima = BankPenerimaTxt.Text;
                FPSearch.AdPenerima = AdPenerimaTxt.Text;
                FPSearch.Penerima = CompanyTxt.Text;
                FPSearch.Biaya = TransferTxt.Text;

                String Result = function.InsertInvoicePembayaran(FPSearch, "Update");

                MessageBox.Show("Data Has Been Confirm", "Success", MessageBoxButtons.OK);
                GlobalVar.GlobalVarKodeInvoice = NoPembayaranTxt.Text;
                SaveMasterBayar("Update Data");
                RefreshSlipHeader();
                panelPrintSlip.Visible = true;




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
            }


        }

        private void BackSlipBtn_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("Do you want to save modification ?", "Warning !!!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                SaveMasterBayar("Update Data");
                RefreshSlipHeader();
            }
            else if (dialogResult == DialogResult.No)
            {
                RefreshSlipHeader();
            }

        }

        private void UpdateSlipBtn_Click(object sender, EventArgs e)
        {
            SaveMasterBayar("Update Data");
        }


        private void EditSlipBtn_Click(object sender, EventArgs e)
        {
            //if ((PembeliBox.SelectedValue.ToString() == "9999") || (PembeliBox.SelectedValue.ToString() == "9999"))
            //{
            //    MessageBox.Show("Please One Select Supplyer", "Error Occured",
            //           MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //else
            //{
            if (dataGridSlip.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {

                FakturPajakSearch FPSearch = new FakturPajakSearch();
                FPSearch.StartDate = StartSlipDate.Value;
                FPSearch.EndDate = EndSlipDate.Value;
                ManualRek.Checked = false;
                foreach (DataGridViewRow row in this.dataGridSlip.SelectedRows)
                {
                    DescPayTxt.Text = row.Cells["DESCRIPTION"].Value.ToString();
                    NoPembayaranTxt.Text = row.Cells["ID"].Value.ToString();
                    GlobalVar.GlobalVarKodeInvoice = row.Cells["ID"].Value.ToString();
                    TotalInvoiceTxt.Text = Decimal.Parse(row.Cells["TOTAL"].Value.ToString()).ToString("N");
                    TotalInvoiceSaveTxt.Text = row.Cells["TOTAL"].Value.ToString();
                    TransferTxt.Text = row.Cells["TRANSFER"].Value.ToString();
                    LastValueTxt.Text = row.Cells["TOTAL"].Value.ToString();
                    FPSearch.IDPEMBELI = row.Cells["IDPEMBELI"].Value.ToString();
                    FPSearch.IDPENGUSAHA = row.Cells["IDPENGUSAHA"].Value.ToString();
                    PembeliBox.SelectedValue = row.Cells["IDPEMBELI"].Value.ToString();
                    PengusahaBox.SelectedValue = row.Cells["IDPENGUSAHA"].Value.ToString();
                    string FlagManual = row.Cells["MANUAL"].Value.ToString();

                    if (FlagManual == "1")
                    {
                        ManualRek.Checked = true;

                        SupplierPembeli Penjual = new SupplierPembeli();
                        Penjual = function.SelectDataManualPenerimaDetailSlip(GlobalVar.GlobalVarKodeInvoice);

                        ANPenerimaTxt.Text = Penjual.AN;
                        BankPenerimaTxt.Text = Penjual.Bank;
                        AdPenerimaTxt.Text = Penjual.Address;
                        CompanyTxt.Text = Penjual.CompanyName;
                        RekManual.Text = Penjual.NoRek;

                    }
                    else
                    {

                        DTParameterType = function.SelectDataPenerimaSlip(PengusahaBox.SelectedValue.ToString());
                        RekTrmBox.DataSource = DTParameterType;
                        RekTrmBox.DisplayMember = "NoRek";
                        RekTrmBox.ValueMember = "NoRek";
                        RefreshDataRekening();
                    }
                }
                SlipPanelSetting("Edit");
                panelSlipRekening.Visible = true;



                DTFakturPajak = function.SelectInvoiceSlip(FPSearch, "Edit");
                dataGridInvoice.Columns.Clear();
                dataGridInvoice.DataSource = DTFakturPajak;
                dataGridInvoice.Columns["FLAG"].Visible = false;
                DataGridViewCheckBoxColumn SlipCmbBox = new DataGridViewCheckBoxColumn();
                SlipCmbBox.ValueType = typeof(bool);
                SlipCmbBox.Name = "SELECTED";
                SlipCmbBox.HeaderText = "SELECTED";
                dataGridInvoice.Columns.Add(SlipCmbBox);

                List<DataGridViewRow> CheckedData = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridInvoice.Rows)
                {

                    if (row.Cells["FLAG"].Value.ToString() == "9")
                    {
                        CheckedData.Add(row);
                        row.Cells["SELECTED"].Value = true;
                    }
                }


                SupplierPembeli Pembeli = new SupplierPembeli();
                Pembeli = function.SelectPembeliByID(PembeliBox.SelectedValue.ToString());
                AdPengirimTxt.Text = Pembeli.Address;
                NPWPPengirimTxt.Text = Pembeli.NPWP;
                KrmBox.Text = Pembeli.CompanyName;







            }
            //}
        }



        private void ReprintBtn_Click(object sender, EventArgs e)
        {
            if (dataGridSlip.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            else
            {
                foreach (DataGridViewRow row in this.dataGridSlip.SelectedRows)
                {
                    GlobalVar.GlobalVarKodeInvoice = row.Cells["ID"].Value.ToString();

                }
               // panelPrintInvoice.Visible = true;
                panelPrintSlip.Visible = true;
            }
        }

        private void PrintRaboBtn_Click(object sender, EventArgs e)
        {

            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
            String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
            path = path + "\\RaboBank" + Exclude + ".pdf";

            try
            {
                //Reset
                reportViewerFakturPajak.Reset();


                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/Bank/RaboBank.rdlc";


                FakturPajakSearch Data = new FakturPajakSearch();

                Data = function.PrintSlipPembayaran(GlobalVar.GlobalVarKodeInvoice);

                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        
                        new ReportParameter("Date", Data.EndDate.ToString("dd-MMM-yyyy")),
                        new ReportParameter("NPWPPengirim", Data.NPWP),
                        new ReportParameter("NPWPPenerima", Data.NPWP),
                        new ReportParameter("CompanyNamePenerima", Data.DataPenerima),
                        new ReportParameter("CompanyAddressPenerima", Data.AdPenerima),
                        new ReportParameter("CompanyNamePengirim", Data.DataPengirim),
                        new ReportParameter("CompanyAddressPengirim", Data.AdPengirim),
                        new ReportParameter("NoRekeningCustomer",  Data.NoRekPenerima),
                        new ReportParameter("Terbilang", Data.TotalTerbilang),
                        new ReportParameter("Total", decimal.Parse(Data.Total).ToString("N")),
                        new ReportParameter("FakturPajakNumber", Data.KODE),
                        new ReportParameter("BANK", Data.BankPenerima),
                        new ReportParameter("BankAddress", ""),

                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + path + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

            panelPrintSlip.Visible = false;


        }

        private void PrintTunaiBtn_Click(object sender, EventArgs e)
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
            String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
            path = path + "\\BCATunai" + Exclude + ".pdf";

            try
            {
                //Reset
                reportViewerFakturPajak.Reset();


                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/Bank/BCABuktiSetoran.rdlc"; ;


                FakturPajakSearch Data = new FakturPajakSearch();

                Data = function.PrintSlipPembayaran(GlobalVar.GlobalVarKodeInvoice);

                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("Date", Data.EndDate.ToString("dd-MMM-yyyy")),
                        new ReportParameter("NPWPPenerima", Data.NPWP),
                        new ReportParameter("CompanyNamePenerima", Data.DataPenerima),
                        new ReportParameter("CompanyAddressPenerima", Data.AdPenerima),
                        new ReportParameter("CompanyNamePengirim", Data.DataPengirim),
                        new ReportParameter("CompanyAddressPengirim", Data.AdPengirim),
                        new ReportParameter("NoRekeningCustomer", Data.NoRekPenerima),
                        new ReportParameter("Terbilang", Data.TotalTerbilang),
                        new ReportParameter("Total", decimal.Parse(Data.Total).ToString("N")),
                        new ReportParameter("FakturPajakNumber", Data.KODE),

                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + path + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

            panelPrintSlip.Visible = false;

        }

        private void PrintTransferBtn_Click(object sender, EventArgs e)
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
            String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
            path = path + "\\BCATransfer" + Exclude + ".pdf";

            try
            {
                //Reset
                reportViewerFakturPajak.Reset();


                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/Bank/BCATransfer.rdlc";


                FakturPajakSearch Data = new FakturPajakSearch();

                Data = function.PrintSlipPembayaran(GlobalVar.GlobalVarKodeInvoice);

                ReportParameter[] rptParams = new ReportParameter[]
                    {
                        new ReportParameter("Date", Data.EndDate.ToString("dd-MMM-yyyy")),
                        new ReportParameter("NPWPPenerima", Data.NPWP),
                        new ReportParameter("CompanyNamePenerima", Data.DataPenerima),
                        new ReportParameter("CompanyAddressPenerima", Data.AdPenerima),
                        new ReportParameter("CompanyNamePengirim", Data.DataPengirim),
                        new ReportParameter("CompanyAddressPengirim", Data.AdPengirim),
                        new ReportParameter("NoRekeningCustomer", Data.NoRekPenerima),
                        new ReportParameter("Terbilang", Data.TotalTerbilang),
                        new ReportParameter("Total", decimal.Parse(Data.Total).ToString("N")),
                        new ReportParameter("FakturPajakNumber", Data.KODE),
                        new ReportParameter("Bank", Data.BankPenerima),
                        new ReportParameter("BankAddress", ""),

                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + path + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

            panelPrintSlip.Visible = false;
        }

        private void PrintCancelBtn_Click(object sender, EventArgs e)
        {
            panelPrintSlip.Visible = false;
        }
        private void DeleteSlipBtn_Click(object sender, EventArgs e)
        {
            if (dataGridSlip.SelectedRows.Count == 0)
            {
                MessageBox.Show("Tidak ada row yang terpilih, tolong pilih salah satu row",
                    "Tidak ada row yang terpilih",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                foreach (DataGridViewRow row in this.dataGridSlip.SelectedRows)
                {
                    GlobalVar.GlobalVarKodeInvoice = row.Cells["ID"].Value.ToString();
                    try
                    {
                        String Delete = function.UpdatePembayaranDelete(row.Cells["ID"].Value.ToString());
                        if (Delete == "Success")
                        {
                            Delete = function.DeleteInvoicePembayaranData(row.Cells["ID"].Value.ToString());
                        }
                        SearchDataSlip();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Occured",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Error(ex.Message);
                    }
                }

            }
        }

        private void ManualRek_CheckedChanged(object sender, EventArgs e)
        {
            checkdatamanual();
        }
        private void checkdatamanual()
        {
            if (ManualRek.Checked == true)
            {
                RekManual.Visible = true;
                ANPenerimaTxt.ReadOnly = false;
                BankPenerimaTxt.ReadOnly = false;
                AdPenerimaTxt.ReadOnly = false;
                ANPenerimaTxt.Enabled = true;
                BankPenerimaTxt.Enabled = true;
                AdPenerimaTxt.Enabled = true;
                UpdateSlipBtn.Visible = false;
            }
            else
            {
                RekManual.Visible = false;
                ANPenerimaTxt.ReadOnly = true;
                BankPenerimaTxt.ReadOnly = true;
                AdPenerimaTxt.ReadOnly = true;
                ANPenerimaTxt.Enabled = false;
                BankPenerimaTxt.Enabled = false;
                AdPenerimaTxt.Enabled = false;
                UpdateSlipBtn.Visible = true;
            }
        }
        #endregion

        private void InvoiceDetailGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private void TransferTxt_TextChanged(object sender, EventArgs e)
        {
            if (TransferTxt.Text == "")
            {
                TransferTxt.Text = "0";
                TotalInvoiceTxt.Text = (decimal.Parse(TotalInvoiceTxt.Text) - decimal.Parse(TransferTxt.Text)).ToString();
            }
            else
            {
                TotalInvoiceTxt.Text = (decimal.Parse(TotalInvoiceTxt.Text) - decimal.Parse(TransferTxt.Text)).ToString();
            }
        }

        private void TransferTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void GiroBtn_Click(object sender, EventArgs e)
        {

            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
            String Exclude = RemoveSpecialCharacters(DateTime.Now.ToString());
            path = path + "\\Giro" + Exclude + ".pdf";

            try
            {
                //Reset
                reportViewerFakturPajak.Reset();


                //Path  
                reportViewerFakturPajak.LocalReport.ReportPath = "Report/InvoiceTotal.rdlc";
                


                FakturPajakSearch Data = new FakturPajakSearch();

                Data = function.PrintSlipPembayaran(GlobalVar.GlobalVarKodeInvoice);

                ReportParameter[] rptParams = new ReportParameter[]
                    {
                                             
                        new ReportParameter("NPWPPengusahaKenaPajak", Data.NPWP),
                        new ReportParameter("CompanyNamePengusahaKenaPajak", Data.DataPenerima),
                        new ReportParameter("CompanyAddressPengusahaKenaPajak", Data.AdPenerima),
                        new ReportParameter("FakturPajakNumber", Data.KODE),
                        new ReportParameter("Total", decimal.Parse(Data.Total.ToString()).ToString("N")),
                        new ReportParameter("Terbilang", Data.TotalTerbilang),
                        new ReportParameter("LastModified", Data.LastModified.ToString("dd MMM yy")),
                        
                    };
                reportViewerFakturPajak.LocalReport.SetParameters(rptParams);


                //Refresh
                reportViewerFakturPajak.LocalReport.Refresh();


                //Write to File
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewerFakturPajak.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                System.Diagnostics.Process.Start(@"" + path + "");
                MessageBox.Show("Success Me-generate report", "Success Generating Report",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Occured",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }

            panelPrintSlip.Visible = false;



        }
    }



    #region itextsharp event
    public class _eventsMemoDiscount : PdfPageEventHelper
    {
        private const int SPACINGAFTER = 30;

        public DataRow rowHeader { get; set; }

        public iTextSharp.text.Image ImageHeader { get; set; }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            var boldTableFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD);
            //var endingMessageFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL);

            var cellHeader = new PdfPCell();
            var HeaderTable = new PdfPTable(21);
            var phrase = new Phrase();

            cellHeader.BackgroundColor = new BaseColor(Color.Gainsboro);
            HeaderTable.HeaderRows = 21;
            HeaderTable = new PdfPTable(21);
            HeaderTable.HorizontalAlignment = 0;
            HeaderTable.SpacingBefore = SPACINGAFTER;
            HeaderTable.TotalWidth = 500f;
            HeaderTable.LockedWidth = true;
            //HeaderTable.SpacingAfter = 10;
            HeaderTable.DefaultCell.Border = 0;
            //HeaderTable.SetWidths(new int[] {1, 4});
            //HeaderTable.SetWidths(new float[] { 2f, 6f, 6f, 3f, 5f, 8f, 5f, 5f, 5f, 5f, 5f });


            cellHeader.Colspan = 2;
            cellHeader.Phrase = new Phrase("Promo", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 2;
            cellHeader.Phrase = new Phrase("Start Date", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 2;
            cellHeader.Phrase = new Phrase("End Date", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = new Phrase("Start Time", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = new Phrase("End Time", boldTableFont);
            HeaderTable.AddCell(cellHeader);


            cellHeader.Colspan = 6;
            cellHeader.Phrase = new Phrase("Desc", boldTableFont);
            HeaderTable.AddCell(cellHeader);


            cellHeader.Colspan = 2;
            cellHeader.Phrase = new Phrase("Type", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = new Phrase("Disc 1", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = new Phrase("Disc 2", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = new Phrase("Disc 3", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 2;
            cellHeader.Phrase = new Phrase("Disc RP", boldTableFont);
            HeaderTable.AddCell(cellHeader);


            cellHeader = new PdfPCell();

            cellHeader.Colspan = 2;
            cellHeader.Phrase = (new Phrase(rowHeader["PROMOCODE"].ToString(), bodyFont));
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 2;
            cellHeader.Phrase = (new Phrase(rowHeader["STARTDATE"].ToString(), bodyFont));
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 2;
            cellHeader.Phrase = (new Phrase(rowHeader["ENDDATE"].ToString(), bodyFont));
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = (new Phrase(rowHeader["STARTTIME"].ToString(), bodyFont));
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = (new Phrase(rowHeader["ENDTIME"].ToString(), bodyFont));
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 6;
            cellHeader.Phrase = new Phrase(rowHeader["DESCRIPTION"].ToString(), bodyFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 2;
            cellHeader.Phrase = new Phrase(rowHeader["DISC_TYPE"].ToString(), bodyFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = new Phrase(rowHeader["DISCOUNT1"].ToString(), bodyFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = new Phrase(rowHeader["DISCOUNT2"].ToString(), bodyFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 1;
            cellHeader.Phrase = new Phrase(rowHeader["DISCOUNT3"].ToString(), bodyFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 2;
            cellHeader.Phrase = new Phrase(rowHeader["DISCOUNTRP"].ToString(), bodyFont);
            HeaderTable.AddCell(cellHeader);

            document.Add(HeaderTable);


            var DetailTable = new PdfPTable(11);

            DetailTable = new PdfPTable(11);
            DetailTable.HorizontalAlignment = 0;
            DetailTable.SpacingBefore = 10;
            DetailTable.TotalWidth = 500;
            DetailTable.LockedWidth = true;
            //DetailTable.SpacingAfter = SPACINGAFTER;
            DetailTable.DefaultCell.Border = 0;

            DetailTable.AddCell("");

            cellHeader = new PdfPCell();
            cellHeader.BackgroundColor = new BaseColor(Color.Gainsboro);
            cellHeader.Colspan = 2;
            cellHeader.Phrase = new Phrase("Item ID", boldTableFont);
            DetailTable.AddCell(cellHeader);



            cellHeader.Colspan = 1;
            cellHeader.Phrase = new Phrase("SV", boldTableFont);
            DetailTable.AddCell(cellHeader);


            cellHeader.Phrase = new Phrase("Brand", boldTableFont);
            DetailTable.AddCell(cellHeader);
            cellHeader.Colspan = 7;
            phrase = new Phrase();
            cellHeader.Phrase = new Phrase("Description", boldTableFont);
            DetailTable.AddCell(cellHeader);

            document.Add(DetailTable);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            var boldTableFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD);
            // cell height 
            float cellHeight = document.BottomMargin - 25;
            // PDF document size      
            iTextSharp.text.Rectangle page = document.PageSize;

            // create two column table
            PdfPTable head = new PdfPTable(10);
            head.SpacingBefore = 10;
            head.TotalWidth = page.Width;

            PdfPCell c = new PdfPCell();




            // add the header text
            c = new PdfPCell(new Phrase("Powered By Kahar Duta Sarana - Business Solution", boldTableFont));
            c.Colspan = 9;
            c.VerticalAlignment = Element.ALIGN_MIDDLE;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            c.Border = PdfPCell.NO_BORDER;
            c.FixedHeight = cellHeight;
            head.AddCell(c);

            // add image; PdfPCell() overload sizes image to fit cell
            c = new PdfPCell(ImageHeader, true);
            c.Colspan = 1;
            c.HorizontalAlignment = Element.ALIGN_LEFT;
            c.VerticalAlignment = Element.ALIGN_MIDDLE;
            c.FixedHeight = cellHeight;
            c.Border = PdfPCell.NO_BORDER;
            head.AddCell(c);


            // since the table header is implemented using a PdfPTable, we call
            // WriteSelectedRows(), which requires absolute positions!
            head.WriteSelectedRows(
              0, -1,  // first/last row; -1 flags all write all rows
              0,      // left offset
                // ** bottom** yPos of the table
              cellHeight + 10,
                //page.Height - cellHeight + head.TotalHeight,
              writer.DirectContent
            );

        }
    }

    public class _eventsStockTakeReport : PdfPageEventHelper
    {
        private const int SPACINGAFTER = 30;

        public DataRow rowHeader { get; set; }

        public iTextSharp.text.Image ImageHeader { get; set; }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            var boldTableFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD);
            var endingMessageFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.ITALIC);
            var bodyFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL);

            var cellHeader = new PdfPCell();
            var HeaderTable = new PdfPTable(21);
            var phrase = new Phrase();

            cellHeader.BackgroundColor = new BaseColor(Color.Gainsboro);
            HeaderTable.HeaderRows = 21;
            HeaderTable = new PdfPTable(21);
            HeaderTable.HorizontalAlignment = 0;
            HeaderTable.SpacingBefore = SPACINGAFTER;
            HeaderTable.TotalWidth = 500f;
            HeaderTable.LockedWidth = true;
            HeaderTable.SpacingAfter = 10;
            HeaderTable.DefaultCell.Border = 0;
            //HeaderTable.SetWidths(new int[] { 1, 4 });
            //HeaderTable.SetWidths(new float[] { 2f, 6f, 6f, 3f, 5f, 8f, 5f, 5f, 5f, 5f, 5f });


            cellHeader.Colspan = 3;
            cellHeader.Phrase = new Phrase("PLU", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 10;
            cellHeader.Phrase = new Phrase("Description", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 4;
            cellHeader.Phrase = new Phrase("Harga Jual", boldTableFont);
            HeaderTable.AddCell(cellHeader);

            cellHeader.Colspan = 4;
            cellHeader.Phrase = new Phrase("Hasil Stock", boldTableFont);
            HeaderTable.AddCell(cellHeader);






            document.Add(HeaderTable);

        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            var boldTableFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD);
            // cell height 
            float cellHeight = document.BottomMargin - 25;
            // PDF document size      
            iTextSharp.text.Rectangle page = document.PageSize;

            // create two column table
            PdfPTable head = new PdfPTable(10);
            head.SpacingBefore = 10;
            head.TotalWidth = page.Width;

            PdfPCell c = new PdfPCell();




            // add the header text
            c = new PdfPCell(new Phrase("Powered By Kahar Duta Sarana - Business Solution", boldTableFont));
            c.Colspan = 9;
            c.VerticalAlignment = Element.ALIGN_MIDDLE;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            c.Border = PdfPCell.NO_BORDER;
            c.FixedHeight = cellHeight;
            head.AddCell(c);

            // add image; PdfPCell() overload sizes image to fit cell
            c = new PdfPCell(ImageHeader, true);
            c.Colspan = 1;
            c.HorizontalAlignment = Element.ALIGN_LEFT;
            c.VerticalAlignment = Element.ALIGN_MIDDLE;
            c.FixedHeight = cellHeight;
            c.Border = PdfPCell.NO_BORDER;
            head.AddCell(c);


            // since the table header is implemented using a PdfPTable, we call
            // WriteSelectedRows(), which requires absolute positions!
            head.WriteSelectedRows(
              0, -1,  // first/last row; -1 flags all write all rows
              0,      // left offset
                // ** bottom** yPos of the table
              cellHeight + 10,
                //page.Height - cellHeight + head.TotalHeight,
              writer.DirectContent
            );

        }
    }
    #endregion
}
