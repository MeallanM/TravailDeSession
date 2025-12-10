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
        private DispatcherTimer _toastTimer;
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(typeof(PageAfficherProjets));
        }

        private async void navView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "AfficherProjets":
                        mainFrame.Navigate(typeof(PageAfficherProjets));
                        break;
                    case "AfficherClients":
                        mainFrame.Navigate(typeof(PageAfficherClients));
                        break;
                    case "AfficherEmployes":
                        mainFrame.Navigate(typeof(PageAfficherEmploye));
                        break;
                    case "AjouterAdmin":
                        var dialog = new AddDialog();
                        dialog.XamlRoot = this.Content.XamlRoot;
                        var result = await dialog.ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            bool ok = SingletonGeneralUse.getInstance()
                                         .AjouterAdmin(dialog.Nom, dialog.Mdp);

                            ContentDialog info = new ContentDialog
                            {
                                XamlRoot = this.Content.XamlRoot,
                                Title = ok ? "Succès" : "Erreur",
                                Content = ok ? "Administrateur ajouté" : "Impossible d'ajouter l'admin",
                                CloseButtonText = "OK"
                            };

                            await info.ShowAsync();
                        }
                        break;
                    case "Connection":
                        ShowLoginDialogAsync();
                        break;
                    case "Deconnection":
                        SingletonGeneralUse.getInstance().LogoutAdmin();
                        UpdateAdminFooter();
                        mainFrame.Navigate(typeof(PageAfficherProjets));
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
            var dialog = new LoginDialog();
            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string nom = dialog.Username;
                string mdp = dialog.Password;

                // NEW correct logic
                bool success = SingletonGeneralUse.getInstance().LoginAdmin(nom, mdp);

                if (success)
                {
                    UpdateAdminFooter();
                    return;
                }
                else
                {
                    ContentDialog error = new ContentDialog
                    {
                        Title = "Connexion échouée",
                        Content = "Nom ou mot de passe invalide.",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };

                    await error.ShowAsync();

                    

                    // Allow retry by showing the login dialog again
                    ShowLoginDialogAsync();
                }
            }
        }

        public void ShowToast(string message)
        {
            ToastText.Text = message;
            ToastContainer.Visibility = Visibility.Visible;

            // Reset timer if already active
            _toastTimer?.Stop();

            _toastTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            _toastTimer.Tick += (s, e) =>
            {
                ToastContainer.Visibility = Visibility.Collapsed;
                _toastTimer.Stop();
            };

            _toastTimer.Start();
        }

        private void ToastClose_Click(object sender, RoutedEventArgs e)
        {
            ToastContainer.Visibility = Visibility.Collapsed;
            _toastTimer?.Stop();
        }

        public void UpdateAdminFooter()
        {
            if (SingletonGeneralUse.getInstance().IsAdminLogged)
            {
                txtAdminFooter.Text = $"Connecté en tant que {SingletonGeneralUse.getInstance().CurrentAdmin.Username}";
            }
            else
            {
                txtAdminFooter.Text = "";
            }
        }
    }
}
