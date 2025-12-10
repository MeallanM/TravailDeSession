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
        bool AwaitingClick = false;
        public PageAfficherClients()
        {
            InitializeComponent();
            SingletonGeneralUse.getInstance().getAllClients();
            Clients = SingletonGeneralUse.getInstance().ListeClients;
        }
        private async void btnAjoutClient_Click(object sender, RoutedEventArgs e)
        {
            if (!SingletonGeneralUse.getInstance().IsAdminLogged)
            {
                ContentDialog d = new ContentDialog
                {
                    Title = "Accès refusé",
                    Content = "Vous devez être administrateur.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await d.ShowAsync();
                return;
            }
            Frame.Navigate(typeof(PageAjouterClient));
        }
        private async void lvAfficher_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!SingletonGeneralUse.getInstance().IsAdminLogged)
            {
                ContentDialog d = new ContentDialog
                {
                    Title = "Accès refusé",
                    Content = "Vous devez être administrateur.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await d.ShowAsync();
                return;
            }
            var selectedClient = e.ClickedItem as Client;
            var frame = Frame ?? ((Frame)Microsoft.UI.Xaml.Window.Current.Content);
                if (selectedClient is Client cli)
                {
                    frame.Navigate(typeof(PageDetailsClient), cli);
                }
        }
    }
}
