using System;
using System.Windows.Forms;
using Mandara.Business;
using Mandara.Business.Managers;
using Mandara.Entities;
using Mandara.Business.Authorization;
using System.Security.Principal;

namespace Mandara.ProductGUI
{
    public partial class CheckPasswordForm : DevExpress.XtraEditors.XtraForm
    {
        public User AuthorizedUser { get; private set; }
        public string PasswordHash { get; set; }

        public CheckPasswordForm()
        {
            InitializeComponent();

            txtUsername.Text = WindowsIdentity.GetCurrent().Name;
        }

        private void CheckRights()
        {
            PasswordHash = Business.Client.Managers.AuthorizationManager.ComputeHash(txtPassword.Text);
            var user = AuthorizationService.AuthorizationManager.AuthorizeUser(txtUsername.Text, PasswordHash);

            if (user == null || user.UserId == -1)
            {
                AuthorizationNotGranted();
                return;
            }

            AuthorizedUser = user;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AuthorizationNotGranted()
        {
            MessageBox.Show(this, "Username or password are incorrect", "Authorization Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

            txtPassword.Text = string.Empty;

            AuditManager.WriteAuditMessage(null, null, "Product tool", "LoginAttempt", "Login Attempt", string.Format("Provided username: {0}", txtUsername.Text));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckRights();
            }
        }

        private void CheckPasswordForm_Load(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }

        private void btnAuthorize_Click(object sender, EventArgs e)
        {
            CheckRights();
        }
    }
}