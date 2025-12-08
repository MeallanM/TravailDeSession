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
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TravailDeSession
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageModifierEmploye : Page
    {
        private Employe currentEmp;
        public PageModifierEmploye()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Employe emp)
            {
                //Set le client lors de la nav
                currentEmp = emp;

                //Remplir les champs
                tbTitre.Text = $"Modifier l'Employé #{emp.Matricule}";
                txtNom.Text = emp.Nom;
                txtPrenom.Text = emp.Prenom;
                txtEmail.Text = emp.Email;
                txtAdresse.Text = emp.Adresse;
                nbTauxHoraire.Value = emp.TauxHoraire;
                txtPhotoURL.Text = emp.PhotoIdentite ?? string.Empty;
                if(emp.Statut == "Journalier") tsStatut.IsOn = true;
                else tsStatut.IsOn = false;
            }
        }

        bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            bool valide = true;

            // Lire les valeurs
            string nom = txtNom.Text.Trim();
            string prenom = txtPrenom.Text.Trim();
            string email = txtEmail.Text.Trim();
            string adresse = txtAdresse.Text.Trim();
            double tauxHoraire = nbTauxHoraire.Value;
            string photoUrl = txtPhotoURL.Text.Trim();
            bool statutActif = tsStatut.IsOn;

            if (string.IsNullOrWhiteSpace(nom))
            {
                tbxErrorNom.Visibility = Visibility.Visible;
                valide = false;
            }
            else tbxErrorNom.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(prenom))
            {
                tbxErrorPrenom.Visibility = Visibility.Visible;
                valide = false;
            }
            else tbxErrorPrenom.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                tbxErrorEmail.Visibility = Visibility.Visible;
                valide = false;
            }
            else tbxErrorEmail.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(adresse))
            {
                tbxErrorAdresse.Visibility = Visibility.Visible;
                valide = false;
            }
            else tbxErrorAdresse.Visibility = Visibility.Collapsed;
            if (valide)
            {
                // Mettre à jour l'employé
                currentEmp.Nom = nom;
                currentEmp.Prenom = prenom;
                currentEmp.Email = email;
                currentEmp.Adresse = adresse;
                currentEmp.TauxHoraire = tauxHoraire;
                currentEmp.PhotoIdentite = photoUrl;
                currentEmp.Statut = statutActif ? "Journalier" : "Permanent";

                // Modifier dans la BDD
                SingletonGeneralUse.getInstance().ModifierEmploye(currentEmp);

                // Navigation
                Frame.Navigate(typeof(PageAfficherEmploye));

                // Toast
                ((MainWindow)App.fenetrePrincipale).ShowToast("Employé modifié !");
            }
        }
    }
}
