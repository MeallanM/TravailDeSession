using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TravailDeSession
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void navView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "AfficherProjets":
                        mainFrame.Navigate(typeof(PageAfficherProjets));
                        break;
                    case "AjouterProjet":
                        mainFrame.Navigate(typeof(PageAjouterProjet));
                        break;
                    case "ModifierProjet":
                        mainFrame.Navigate(typeof(PageModifierProjet));
                        break;
                    case "AfficherClients":
                        mainFrame.Navigate(typeof(PageAfficherClients));
                        break;
                    case "AjouterClient":
                        mainFrame.Navigate(typeof(PageAjouterClient));
                        break;
                    case "ModifierClient":
                        mainFrame.Navigate(typeof(PageModifierClient));
                        break;
                    case "AfficherEmployes":
                        mainFrame.Navigate(typeof(PageAfficherEmploye));
                        break;
                    case "AjouterEmploye":
                        mainFrame.Navigate(typeof(PageAjouterEmploye));
                        break;
                    case "ModifierEmploye":
                        mainFrame.Navigate(typeof(PageModifierEmploye));
                        break;
                    case "Connection":
                        ShowLoginDialogAsync();
                        break;
                    case "Deconnection":
                        SingletonGeneralUse.getInstance().AdminIsAdmin = false;
                        break;
                    case "Quitter":
                        Application.Current.Exit();
                        break;
                    default:
                        break;
                }
            }
        }

        private void clickMenuBar(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;
            if (item != null)
            {
                //Switch sur les options du menu pour gèrer les imports/exports et la fermeture de l'application
                switch (item.Tag)
                {
                    case "ClientImport":
                        SingletonGeneralUse.getInstance().ImportClientsAsync();
                        break;
                    case "EmployeImport":
                        SingletonGeneralUse.getInstance().ImportEmployesAsync();
                        break;
                    case "ProjetImport":
                        SingletonGeneralUse.getInstance().ImportProjetsAsync();
                        break;
                    case "ClientExport":
                        SingletonGeneralUse.getInstance().ExportClientsAsync();
                        break;
                    case "EmployeExport":
                        SingletonGeneralUse.getInstance().ExportEmployesAsync();
                        break;
                    case "ProjetExport":
                        SingletonGeneralUse.getInstance().ExportProjetsAsync();
                        break;
                    case "Quitter":
                        Application.Current.Exit();
                        break;
                    default:
                        break;
                }
            }
        }

        // Afficher la boîte de dialogue de connexion (LoginDialog.xaml)
        public async void ShowLoginDialogAsync()
        {
            //Création et initialisation de la boîte de dialogue
            var dialog = new LoginDialog();
            dialog.XamlRoot = this.Content.XamlRoot;
            var result = await dialog.ShowAsync();

            //Traitement des informations de connexion
            if (result == ContentDialogResult.Primary)
            {
                string nom = dialog.Username;
                string mdp = dialog.Password;

                if (SingletonGeneralUse.getInstance().ValiderAdmin(nom, mdp)) // your logic
                {
                    // Connection réussie
                    SingletonGeneralUse.getInstance().AdminIsAdmin = true;
                }
                else
                {
                    // Connection échouée
                    ContentDialog error = new ContentDialog
                    {
                        Title = "Connection échouée",
                        Content = "Nom ou mot de passe invalide",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };

                    await error.ShowAsync();
                    return;
                }
            }

        }
    }
}
