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
    public sealed partial class PageModifierClient : Page
    {
        private Client currentCli;
        public PageModifierClient()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Client cli)
            {
                //Set le client lors de la nav
                currentCli = cli;

                //Remplir les champs
                tbTitre.Text = $"Modifier Client #{cli.Identifiant}";
                tbNom.Text = cli.Nom;
                tbAdresse.Text = cli.Adresse;
                tbTelephone.Text = cli.Telephone.ToString();
                tbEmail.Text = cli.Email;
            }
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
        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            bool valide = true;

            string nom = tbNom.Text.Trim();
            string adresse = tbAdresse.Text.Trim();
            string telephone = tbTelephone.Text.Trim();
            string email = tbEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(nom))
            {
                tbxErrorNom.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorNom.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(adresse))
            {
                tbxErrorAdresse.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorAdresse.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(telephone) || !IsValidPhone(telephone))
            {
                tbxErrorTelephone.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorTelephone.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                tbxErrorEmail.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorEmail.Visibility = Visibility.Collapsed;

            if (valide)
            {

                //Set les nouvelles valeurs
                currentCli.Nom = nom;
                currentCli.Adresse = adresse;
                currentCli.Telephone = telephone;
                currentCli.Email = email;

                //Modifier le client dans la BDD
                SingletonGeneralUse.getInstance().ModifierClient(currentCli);

                //Naviguer vers la page liste client
                Frame.Navigate(typeof(PageAfficherClients));
                //Afficher le toast de succès (mainwindow.xaml et .cs)
                ((MainWindow)App.fenetrePrincipale).ShowToast("Client modifié !");
            }
        }
    }
}
