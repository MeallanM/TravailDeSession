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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TravailDeSession;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PageDetailsEmploye : Page
{
    private Employe currentEmp;
    public ObservableCollection<string> listeMatricules { get; set; } = new ObservableCollection<string>();
    public PageDetailsEmploye()
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
            tbTitre.Text = $"Détails de l'Employé #{emp.Matricule}";
            tbMatricule.Text = emp.Matricule;
            tbNom.Text = emp.Nom;
            tbPrenom.Text = emp.Prenom;
            tbDateNaissance.Text = emp.DateNaissance.ToString("yyyy-MM-dd");
            tbDateEmbauche.Text = emp.DateEmbauche.ToString("yyyy-MM-dd");
            tbEmail.Text = emp.Email;
            tbAdresse.Text = emp.Adresse;
            tbTauxHoraire.Text = emp.TauxHoraire.ToString("0.00");
            imgPhoto.Source = emp.PhotoIdentiteImage;
            if (emp.Statut == "Journalier")
            {
                tsStatut.IsOn = true;
            }
            else
            {
                tsStatut.IsOn = false;
            }
        }
    }

    private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button)
            return;

        SingletonGeneralUse.getInstance().getEmployesEnCours();
        listeMatricules = SingletonGeneralUse.getInstance().ListeMatDispo;

        if (listeMatricules == null)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Erreur",
                Content = "Impossible de vérifier les projets actifs.",
                CloseButtonText = "Ok",
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
            return;
        }
        if (listeMatricules.Contains(currentEmp.Matricule))
        {
            ContentDialog blockedDialog = new ContentDialog
            {
                Title = "Suppression impossible",
                Content = "Cet employé est associé à un projet en cours. Vous devez terminer ou retirer l'employé du projet avant de le supprimer.",
                CloseButtonText = "Ok",
                XamlRoot = this.XamlRoot
            };
            await blockedDialog.ShowAsync();

            return;
        }
        ContentDialog dialog = new ContentDialog
        {
            Title = "Confirmation",
            Content = "Voulez-vous vraiment supprimer ce client?",
            PrimaryButtonText = "Oui",
            CloseButtonText = "Non",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            SingletonGeneralUse.getInstance().SupprimerEmploye(currentEmp);

            Frame.Navigate(typeof(PageAfficherClients));

            ((MainWindow)App.fenetrePrincipale).ShowToast("Client supprimé !");
        }
    }

    private void BtnModifier_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(PageModifierEmploye), currentEmp);
    }
}
