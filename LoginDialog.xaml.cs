using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

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
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text?.Trim() ?? "";
            string password = tbPassword.Password ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await ShowMessage("Veuillez entrer un nom d'utilisateur et un mot de passe.");
                return;
            }

            bool ok = false;
            try
            {
                ok = SingletonGeneralUse.getInstance().LoginAdmin(username, password);
            }
            catch (Exception ex)
            {
                // log ex
                await ShowMessage("Erreur lors de la connexion : " + ex.Message);
                return;
            }

            if (ok)
            {
                // close the dialog and notify success
                this.Hide();
                await ShowMessage("Connexion admin réussie.");
            }
            else
            {
                await ShowMessage("Nom d'utilisateur ou mot de passe incorrect.");
            }
        }

        private async Task ShowMessage(string text)
        {
            // Use ContentDialog or MessageDialog pattern appropriate for your app:
            var dlg = new ContentDialog()
            {
                Title = "Info",
                Content = text,
                CloseButtonText = "OK"
            };
            await dlg.ShowAsync();
        }
    }
}