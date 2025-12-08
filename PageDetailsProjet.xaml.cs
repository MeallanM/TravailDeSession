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
public sealed partial class PageDetailsProjet : Page
{
    private Projet currentProj;
    private ObservableCollection<EmployeProjet> EmployeProj { get; set; } = new ObservableCollection<EmployeProjet>();
    public PageDetailsProjet()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Projet proj)
        {
            //Set le client lors de la nav
            currentProj = proj;
            Client c = proj.Client;

            //Remplir les champs
            tbTitre.Text = proj.Titre;
            tbNumeroProjet.Text = proj.NoProjet;
            tbClientId.Text = c.Identifiant.ToString();
            tbDateDebut.Text = proj.DateDebut.ToString();
            tbDescription.Text = proj.Description;
            tbBudget.Text = proj.Budget.ToString();
            tbNbEmployesRequis.Text = proj.NombreEmployesMax.ToString();
            tbTotalSalaires.Text = proj.TotalSalaireDu.ToString();
            if (proj.Statut == "en cours") tbStatut.Text = "En cours";
            else tbStatut.Text = "Terminé";

            SingletonGeneralUse.getInstance().GetEmployesEnCoursParProjet(proj.NoProjet);
            EmployeProj = SingletonGeneralUse.getInstance().ListeEmpProj;

            spEmployesSelectionDetails.Children.Clear();

            foreach (var emp in EmployeProj)
            {
                spEmployesSelectionDetails.Children.Add(new TextBlock()
                {
                    Text = $"{emp.Matricule}",
                    Margin = new Thickness(0, 5, 0, 0),
                    FontSize = 14,
                });
            }
            UpdateEmployesInfos_Readonly();
        }
    }

    private void UpdateEmployesInfos_Readonly()
    {
        // Reload employee-project info
        SingletonGeneralUse.getInstance().GetEmployesProjetInfo();
        EmployeProj = SingletonGeneralUse.getInstance().ListeEmpProj;

        tbEmployesDetails.Text = "";
        double totalSalaire = 0;

        foreach (TextBlock tb in spEmployesSelectionDetails.Children)
        {
            string matricule = tb.Text?.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(matricule))
            {
                var emp = EmployeProj.FirstOrDefault(x => x.Matricule == matricule);

                if (emp != null)
                {
                    double salaire = emp.HeuresTravaillees * emp.TauxHoraire;
                    totalSalaire += salaire;

                    tbEmployesDetails.Text +=
                        $"Emp: {emp.Matricule}, TxH: {emp.TauxHoraire:0.00}, " +
                        $"Hrs: {emp.HeuresTravaillees:0.##}, Salaire: {salaire}$\n";
                }
            }
        }

        tbTotalSalaireDetails.Text = $"Total Salaires: {totalSalaire:0.##}";
    }

    private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Confirmation",
                Content = "Voulez-vous vraiment supprimer ce projet? Cela supprimera \n" +
                            "aussi tous les employe_projet liés à ce projet.",
                PrimaryButtonText = "Oui",
                CloseButtonText = "Non",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                SingletonGeneralUse.getInstance().SupprimerProjet(currentProj);

                Frame.Navigate(typeof(PageAfficherProjets));

                //Afficher le toast de succès (mainwindow.xaml et .cs)
                ((MainWindow)App.fenetrePrincipale).ShowToast("Projet supprimé !");
            }
        }
    }
    private void BtnModifier_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(PageModifierProjet), currentProj);
    }
}
