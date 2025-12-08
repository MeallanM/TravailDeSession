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
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TravailDeSession
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageModifierProjet : Page
    {
        private Projet currentProj;
        public ObservableCollection<int> ListeClients { get; set; } = new();
        public ObservableCollection<string> MatriculesLibres { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> matriculesEmpProj { get; set; } = new ObservableCollection<string>();
        private ObservableCollection<EmployeProjet> EmployeProj { get; set; } = new ObservableCollection<EmployeProjet>();
        private double totalSalaire = 0;
        public PageModifierProjet()
        {
            InitializeComponent();
            List<int> listeIdClient = SingletonGeneralUse.getInstance().getAllClientId();
            foreach (int id in listeIdClient) // matricule is a string
            {
                ListeClients.Add(id);
            }
            
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Projet proj)
            {
                //Set le projet et emp_proj lors de la nav
                currentProj = proj;
                SingletonGeneralUse.getInstance().GetEmployesEnCoursParProjet(proj.NoProjet);
                EmployeProj = SingletonGeneralUse.getInstance().ListeEmpProj;

                //Remplir les champs
                tbTitre.Text = $"Modifier le projet #{proj.NoProjet}";
                txtTitre.Text = proj.Titre;
                CbbxClient.SelectedItem = proj.Client.Identifiant;
                dpDateDebut.Date = proj.DateDebut.Date;
                txtDescription.Text = proj.Description;
                txtBudget.Value = proj.Budget;
                txtNbEmployesRequis.Value = proj.NombreEmployesMax;
                txtTotalSalaires.Value = proj.TotalSalaireDu;
                if (proj.Statut == "en cours") tsStatut.IsOn = true;
                else tsStatut.IsOn = false;

                CbbxClient.ItemsSource = ListeClients;
            }
        }
        private void UpdateEmployesInfos()
        {
            SingletonGeneralUse.getInstance().GetEmployesProjetInfo();
            EmployeProj = SingletonGeneralUse.getInstance().ListeEmpProj;
            foreach (var empPro in EmployeProj)
            {
                if (!string.IsNullOrWhiteSpace(empPro.Matricule))
                    matriculesEmpProj.Add(empPro.Matricule);
            }
            tbEmployesDetails.Text = "";
            totalSalaire = 0;
            foreach (ComboBox cb in spEmployesSelection.Children)
            {
                if (cb.SelectedItem != null)
                {
                    string matricule = cb.SelectedItem.ToString();
                    var emp = EmployeProj.FirstOrDefault(x => x.Matricule == matricule);
                    if (emp != null)
                    {
                        double salaire = emp.HeuresTravaillees * emp.TauxHoraire;
                        totalSalaire += salaire;
                        tbEmployesDetails.Text += $"Emp: {emp.Matricule}, TxH: {emp.TauxHoraire:0.00}, Hrs: {emp.HeuresTravaillees:0.##}, Salaire: {salaire}$\n";
                    }
                }

            }
            tbTotalSalaire.Text = $"Total Salaires: {totalSalaire:0.##}";
        }
        private bool CheckDupePickedMatricule()
        {
            HashSet<string> selectedMat = new HashSet<string>();
            foreach (ComboBox cb in spEmployesSelection.Children)
            {
                if (cb.SelectedItem == null)
                {
                    tbxErrorNbEmployes.Text = "Erreur: Doublon d'employés sélectionnés ou rien de sélectionné";
                    tbxErrorNbEmployes.Visibility = Visibility.Visible;
                    return false;
                }
                string matricule = cb.SelectedItem.ToString();
                if (selectedMat.Contains(matricule))
                {
                    return false;
                }
                selectedMat.Add(matricule);
            }
            return true;
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            bool valide = true;

            // Récupération des valeurs des champs
            string titre = txtTitre.Text.Trim();
            DateTime dateDebut = dpDateDebut.Date.Date;
            string description = txtDescription.Text.Trim();
            double budget = txtBudget.Value;
            double nbEmployes = txtNbEmployesRequis.Value;
            double salaires = txtTotalSalaires.Value;
            int clientId = (int)CbbxClient.SelectedItem;
            Client client = SingletonGeneralUse.getInstance().getClientWithId(clientId);
            string statut = tsStatut.IsOn ? "en cours" : "terminé";

            if (double.IsNaN(nbEmployes))
            {
                tbxErrorNbEmployes.Visibility = Visibility.Visible;
                valide = false;
            }
            else tbxErrorNbEmployes.Visibility = Visibility.Collapsed;
            // Vérification des doublons d'employés sélectionnés
            if (CheckDupePickedMatricule())
            {
                tbxErrorSelectionEmp.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbxErrorSelectionEmp.Visibility = Visibility.Visible;
                valide = false;
            }
            // --- VALIDATIONS ---
            if (string.IsNullOrWhiteSpace(titre))
            {
                tbxErrorTitre.Visibility = Visibility.Visible;
                valide = false;
            }
            else tbxErrorTitre.Visibility = Visibility.Collapsed;
            if (dateDebut > DateTime.Today)
            {
                tbxErrorDateDebut.Visibility = Visibility.Visible;
                valide = false;
            }
            else tbxErrorDateDebut.Visibility = Visibility.Collapsed;
            if (budget < 0 || double.IsNaN(budget))
            {
                tbxErrorBudget.Visibility = Visibility.Visible;
                valide = false;
            }
            else tbxErrorBudget.Visibility = Visibility.Collapsed;
            if (salaires <= 0 || double.IsNaN(salaires))
            {
                tbxErrorSalaires.Visibility = Visibility.Visible;
                valide = false;
            }
            else tbxErrorSalaires.Visibility = Visibility.Collapsed;

            // --- SI TOUT EST OK ---
            if (valide)
            {
                // Crée l’objet projet
                currentProj.Titre = titre;
                currentProj.DateDebut = dateDebut.Date;
                currentProj.Description = description;
                currentProj.Budget = budget;
                currentProj.NombreEmployesMax = Convert.ToInt32(nbEmployes);
                currentProj.TotalSalaireDu = salaires;
                currentProj.Client = client;
                currentProj.Statut = statut;

                SingletonGeneralUse.getInstance().ModifierProjet(currentProj);
                // Ajout des employés au projet
                foreach (ComboBox cb in spEmployesSelection.Children)
                {
                    string matricule = cb.SelectedItem.ToString();
                    var emp = EmployeProj.FirstOrDefault(x => x.Matricule == matricule);
                    if (emp != null)
                    {


                        EmployeProjet ep = new EmployeProjet(
                            matricule,
                            currentProj.NoProjet,
                            emp.HeuresTravaillees,
                            emp.TauxHoraire
                        );
                        SingletonGeneralUse.getInstance().UpdateEmployesForProjet(ep);
                    }
                }

                Frame.Navigate(typeof(PageAfficherProjets));
            }
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

        private void txtNbEmployesRequis_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (sender.Value % 1 != 0)
                sender.Value = Math.Floor(sender.Value);
            if (sender.Value > 5 || sender.Value < 0 || double.IsNaN(sender.Value))
            {
                tbxErrorNbEmployes.Text = "Erreur: Nombre d'employés requis doit être entre 0 et 5";
                tbxErrorNbEmployes.Visibility = Visibility.Visible;
                return;
            }
            else if (!CheckDupePickedMatricule())
            {
                tbxErrorNbEmployes.Text = "Erreur: Doublon d'employés sélectionnés ou rien de sélectionné";
                tbxErrorNbEmployes.Visibility = Visibility.Visible;
            }
            else tbxErrorNbEmployes.Visibility = Visibility.Collapsed;
            int required = (int)sender.Value;

            // Ajout des matricules libres à la liste
            SingletonGeneralUse.getInstance().getEmployesTermine();
            MatriculesLibres = SingletonGeneralUse.getInstance().ListeMatDispo;
            foreach (var emp in EmployeProj)
            {
                if(!MatriculesLibres.Contains(emp.Matricule))
                    MatriculesLibres.Add(emp.Matricule);
            }

            int current = spEmployesSelection.Children.Count;
            if (required > current )
            {
                for (int i = current; i < current; i++)
                {
                    ComboBox cb = new ComboBox()
                    {
                        ItemsSource = MatriculesLibres,
                        PlaceholderText = "Sélectionner un employé",
                        Width = double.NaN, // stretch horizontally
                    };
                    cb.SelectedItem = EmployeProj[i].Matricule;
                    cb.SelectionChanged += (s, e) => UpdateEmployesInfos();
                    spEmployesSelection.Children.Add(cb);
                }
            }
            else if (required < current)
            {
                while (spEmployesSelection.Children.Count > required)
                    spEmployesSelection.Children.RemoveAt(spEmployesSelection.Children.Count - 1);
                UpdateEmployesInfos();
            }
        }
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Refresh de la section d'ajout des employés
            spEmployesSelection.Children.Clear();
            MatriculesLibres.Clear();
            SingletonGeneralUse.getInstance().getEmployesTermine();
            MatriculesLibres = SingletonGeneralUse.getInstance().ListeMatDispo;
            tbEmployesDetails.Text = "";
            tbTotalSalaire.Text = "Total Salaires:";
            txtNbEmployesRequis.Value = 0;
            totalSalaire = 0;
        }

    }
}
