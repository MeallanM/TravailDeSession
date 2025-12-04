using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
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
    public sealed partial class PageAjouterEmploye : Page
    {
        public PageAjouterEmploye()
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

        private void EmployeCreation_Click(object sender, RoutedEventArgs e)
        {
            bool valide = true;

            string nom = tbNom.Text.Trim();
            string prenom = tbPrenom.Text.Trim();
            string email = tbEmail.Text.Trim();
            string adresse = tbAdresse.Text.Trim();
            string statut = tsStatut.IsOn ? "Journalier" : "Permanent";
            string txtTaux = tbTauxHoraire.Text.Trim();
            DateTime dateNaissance = dpDateNaissance.Date.DateTime;
            DateTime dateEmbauche = dpDateEmbauche.Date.DateTime;
            string photoIdentite = imgPhoto.Text;

            if (string.IsNullOrWhiteSpace(nom))
            {
                tbxErrorNom.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorNom.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(prenom))
            {
                tbxErrorPrenom.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorPrenom.Visibility = Visibility.Collapsed;
            if (dateNaissance == null || dateNaissance >= DateTime.Today)
            {
                tbxErrorNaissance.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorNaissance.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                tbxErrorEmail.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorEmail.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(adresse))
            {
                tbxErrorAdresse.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorAdresse.Visibility = Visibility.Collapsed;
            if (dateEmbauche == null ||
                dateNaissance == null ||
                dateEmbauche < dateNaissance ||
                dateEmbauche > DateTime.Today)
            {
                tbxErrorEmbauche.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorEmbauche.Visibility = Visibility.Collapsed;
            if (!double.TryParse(txtTaux, out double tauxHoraire) || tauxHoraire <= 0)
            {
                tbxErrorTaux.Visibility = Visibility.Visible;
                valide = false;
            }
            else
                tbxErrorTaux.Visibility = Visibility.Collapsed;

            if (valide)
            {
                SingletonGeneralUse.getInstance().AjouterEmploye(new Employe(null,nom,prenom,dateNaissance, email, adresse, dateEmbauche, tauxHoraire, photoIdentite, statut));
                Frame.Navigate(typeof(PageAfficherEmploye));
            }
        }
    }
}
