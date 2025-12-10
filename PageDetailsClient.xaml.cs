using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Mysqlx.Notice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TravailDeSession;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PageDetailsClient : Page
{
    private Client currentCli;
    public PageDetailsClient()
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
            tbTitre.Text = $"Détails du Client #{cli.Identifiant}";
            tbNom.Text = cli.Nom;
            tbAdresse.Text = cli.Adresse;
            tbTelephone.Text = cli.Telephone.ToString();
            tbEmail.Text = cli.Email;
        }
    }

    private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
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
                SingletonGeneralUse.getInstance().SupprimerClient(currentCli);

                Frame.Navigate(typeof(PageAfficherClients));

                //Afficher le toast de succès (mainwindow.xaml et .cs)
                ((MainWindow)App.fenetrePrincipale).ShowToast("Client supprimé !");
            }
        }
    }

    private void BtnModifier_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(PageModifierClient), currentCli);
    }
}
