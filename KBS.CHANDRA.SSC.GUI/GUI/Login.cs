using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KBS.CHANDRA.SSC.FUNCTION;
using KBS.CHANDRA.SSC.DATAMODEL;
 
namespace KBS.CHANDRA.SSC.GUI
{
    public partial class Login : Form
    {

        SSCFunction function = new SSCFunction();
        User user = new User();

        public Login()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(textBoxUserID.Text) || String.IsNullOrWhiteSpace(textBoxPassword.Text))
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

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                if (String.IsNullOrWhiteSpace(textBoxUserID.Text) || String.IsNullOrWhiteSpace(textBoxPassword.Text))
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

        private void loginEvent()
        {
            try
            {
                user = function.Login(textBoxUserID.Text, textBoxPassword.Text);
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
                        textBoxPassword.Text = null;
                        break;
                    case 1:
                        GlobalVar.GlobalVarUserID = user.UserID;
                        GlobalVar.GlobalVarPassword = user.Password;
                        GlobalVar.GlobalVarProfileID = user.ProfileID;
                        GlobalVar.GlobalVarUsername = user.Username;
                        textBoxUserID.Text = "";
                        textBoxPassword.Text = "";
                        textBoxUserID.Focus();


                        this.Hide();
                        TestMenu mainMenuForm = new TestMenu();
                        
                        mainMenuForm.ShowDialog();
                        this.Close();
                        break;
                    case 2:
                        MessageBox.Show("User dalam keadaan Frozen, tolong hubungi admin", "User dalam keadaan frozen",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case 3:
                        MessageBox.Show("User sudah di delete, Tolong pakai user yang lain", "User sudah di delete",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    default:
                        MessageBox.Show("Tolong hubungi Admin", "Unknown error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }

            
        }

        private void Login_Load(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(0,112,192);
            btnExit.BackColor = Color.FromArgb(0, 112, 192);
            labelSSC.ForeColor = Color.FromArgb(0, 112, 192);
            labelCopyright.BackColor = Color.FromArgb(231, 230, 230);
            linkLabelKDS.BackColor = Color.FromArgb(231, 230, 230);
            labelContactUs.BackColor = Color.FromArgb(231, 230, 230);
            rectangleShape1.BackColor = Color.FromArgb(231, 230, 230);
            rectangleShape1.FillColor = Color.FromArgb(231, 230, 230);
            textBoxUserID.Focus();

        }

        private void linkLabelKDS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabelKDS.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://www.kahar.co.id/enterprise/");
        }

    }
}
