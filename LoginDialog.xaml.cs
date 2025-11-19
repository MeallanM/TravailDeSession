using Microsoft.UI.Xaml.Controls;

namespace TravailDeSession
{
    //Dialog de connection pour l'admin
    public sealed partial class LoginDialog : ContentDialog
    {
        public string Username => tbUsername.Text;
        public string Password => tbPassword.Password;

        public LoginDialog()
        {
            this.InitializeComponent();
        }
    }
}