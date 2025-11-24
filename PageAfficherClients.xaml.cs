using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TravailDeSession
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageAfficherClients : Page
    {
        ObservableCollection<Client> Clients;
        public PageAfficherClients()
        {
            InitializeComponent();
            SingletonGeneralUse.getInstance().getAllClients();
            Clients = SingletonGeneralUse.getInstance().ListeClients;
        }

        private void lvAfficher_ItemClick(object sender, ItemClickEventArgs e)
        {
            var frame = Frame ?? ((Frame)Microsoft.UI.Xaml.Window.Current.Content);

            if (e.ClickedItem is Client cli)
            {
                frame.Navigate(typeof(PageModifierClient), cli);
            }
        }
        private void btnAfficher_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Client cli)
            {
                Frame.Navigate(typeof(/*AJouter le whatever qui gère sa. Je mets dequoi temporaire*/PageAfficherClients), cli);
            }
        }
        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn2 && btn2.Tag is Client cli)
            {
                Frame.Navigate(typeof(/*AJouter le whatever qui gère sa. Je mets dequoi temporaire*/PageAfficherClients), cli);
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
                    if (btn.Tag is Client prod)
                        SingletonGeneralUse.getInstance().SupprimerClient(prod);
                }
            }
        }
    }
}
