using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
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
    public sealed partial class PageModifierClient : Page
    {
        private Client currentCli;
        public PageModifierClient()
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
                /*------------------------------Décommenter pour que ça fonctionne------------------------------
                txtTitre.Text = $"Modifier Client #{cli.Identifiant}";
                txtNom.Text = cli.Nom;
                txtAdresse.Text = cli.Adresse;
                txtTelephone = cli.Telephone.ToString();
                txtCourriel.Text = cli.Email;
                */
            }
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            if (currentCli != null)
            {
                //Set les nouvelles valeurs
                /*------------------------------Décommenter pour que ça fonctionne------------------------------
                currentCli.Nom = txtNom.Text;
                currentCli.Adresse = txtAdresse.Text;
                currentCli.Telephone = txtTelephone.Text;
                currentCli.Email = txtCourriel.Text;
                */
                //Modifier le client dans la BDD
                SingletonGeneralUse.getInstance().ModifierClient(currentCli);
                //Afficher le toast de succès (mainwindow.xaml et .cs)
                ((MainWindow)App.fenetrePrincipale).ShowToast("Client modifié !");
            }
        }
    }
}
