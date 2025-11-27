using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TravailDeSession
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageAjouterClient : Page
    {
        public PageAjouterClient()
        {
            InitializeComponent();
        }
        
        bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone,
                @"^\D*(\d\D*){10}$");
        }

        private void ClientCreation_Click(object sender, RoutedEventArgs e)
        {
            string nom = txtNom.Text.Trim();
            string adresse = txtAdresse.Text.Trim();
            string telephone = txtTelephone.Text.Trim();
            string email = txtEmail.Text.Trim();
            bool valide = true;
            if (string.IsNullOrEmpty(nom))
            {
                tbxErrorNom.Visibility = Visibility.Visible;
                valide = false;
            }
            else
            {
                valide = true;
                tbxErrorNom.Visibility = Visibility.Collapsed;
            }
            if (string.IsNullOrEmpty(adresse))
            {
                tbxErrorAdresse.Visibility = Visibility.Visible;
                valide = false;
            }
            else
            {
                valide = true;
                tbxErrorAdresse.Visibility = Visibility.Collapsed;
            }
            if (string.IsNullOrEmpty(telephone) || !IsValidPhone(telephone))
            {
                tbxErrorTelephone.Visibility = Visibility.Visible;
                valide = false;
            }
            else
            {
                valide = true;
                tbxErrorTelephone.Visibility = Visibility.Collapsed;
            }
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                tbxErrorEmail.Visibility = Visibility.Visible;
                valide = false;
            }
            else
            {
                valide = true;
                tbxErrorEmail.Visibility = Visibility.Collapsed;
            }
            if (valide)
            {
                SingletonGeneralUse.getInstance().AjouterClient(new Client(1, nom, adresse, telephone, email));
                txtAdresse.Text = "";
                txtEmail.Text = "";
                txtNom.Text = "";
                txtTelephone.Text = "";
            }
        }
    }
}
