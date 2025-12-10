using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class PageAfficherEmploye : Page
    {

        ObservableCollection<Employe> Employes;
        public PageAfficherEmploye()
        {
            InitializeComponent();
            SingletonGeneralUse.getInstance().getAllEmployes();
            Employes = SingletonGeneralUse.getInstance().ListeEmployes;
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
            var frame = Frame ?? ((Frame)Microsoft.UI.Xaml.Window.Current.Content);

            if (e.ClickedItem is Employe emp)
            {
                frame.Navigate(typeof(PageDetailsEmploye), emp);
            }
        }
        private async void btnAjouterEmploye_Click(object sender, RoutedEventArgs e)
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
            Frame.Navigate(typeof(PageAjouterEmploye));
        }
    }
}
