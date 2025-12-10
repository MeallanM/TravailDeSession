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
using System.Diagnostics;
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
    public sealed partial class PageAfficherProjets : Page
    {
        //Collection contexte pour lister les projets
        ObservableCollection<Projet> Projets;
        public PageAfficherProjets()
        {
            InitializeComponent();
            //Récupération de la liste des projets en cours
            SingletonGeneralUse.getInstance().getAllProjetsEnCours();
            Projets = SingletonGeneralUse.getInstance().ListeProjets;
        }
        /*On click de la gridview*/
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

            if (e.ClickedItem is Projet proj)
            {
                frame.Navigate(typeof(PageDetailsProjet), proj);
            }
        }
        //On click du bouton ajouter projet
        private async void btnAjouterProjet_Click(object sender, RoutedEventArgs e)
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
            Frame.Navigate(typeof(PageAjouterProjet));
        }
    }
}
