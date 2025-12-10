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
using Windows.UI.StartScreen;

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
        List<ComboBox> comboBoxes;
        List<NumberBox> numberboxes;
        public ObservableCollection<int> ListeClients { get; set; } = new();
        public ObservableCollection<string> MatriculesLibres { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> matriculesEmpProj { get; set; } = new ObservableCollection<string>();
        private ObservableCollection<EmployeProjet> EPToAdd { get; set; } = new ObservableCollection<EmployeProjet>();
        private ObservableCollection<EmployeProjet> EmployeProjTemp { get; set; } = new ObservableCollection<EmployeProjet>();
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

                SingletonGeneralUse.getInstance().GetEmployesEnCoursParProjet(currentProj.NoProjet);
                EmployeProjTemp = SingletonGeneralUse.getInstance().ListeEmpProj;

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
                SingletonGeneralUse.getInstance().getEmployesTermine();
                MatriculesLibres = SingletonGeneralUse.getInstance().ListeMatDispo;
                foreach (var empProj in EmployeProjTemp)
                {
                    if(!MatriculesLibres.Contains(empProj.Matricule))
                        MatriculesLibres.Add(empProj.Matricule);
                }

                comboBoxes = new List<ComboBox> { cbMatricule1, cbMatricule2, cbMatricule3, cbMatricule4, cbMatricule5 };
                numberboxes = new List<NumberBox> { nbHeures1, nbHeures2, nbHeures3, nbHeures4, nbHeures5 };

                for (int i = 0; i < comboBoxes.Count; i++)
                {
                    comboBoxes[i].ItemsSource = MatriculesLibres;
                    comboBoxes[i].SelectionChanged += (s, ev) => UpdateEmployesInfos();
                    numberboxes[i].ValueChanged += (s, ev) => UpdateEmployesInfos();
                }
                for (int i = 0; i < EmployeProjTemp.Count; i++)
                {
                    comboBoxes[i].SelectedItem = EmployeProjTemp[i].Matricule;
                    numberboxes[i].Value = EmployeProjTemp[i].HeuresTravaillees;
                }
            }
        }
        private void UpdateEmployesInfos()
        {
            tbEmployesDetails.Text = "";
            totalSalaire = 0;
            SingletonGeneralUse.getInstance().GetEmployesProjetInfo();
            EmployeProj = SingletonGeneralUse.getInstance().AutreListeEmpProj;
            foreach (ComboBox cb in spEmployesSelection.Children)
            {
                if (cb.SelectedItem != null)
                {
                    string matricule = cb.SelectedItem.ToString();
                    var emp = EmployeProj.FirstOrDefault(x => x.Matricule == matricule && x.CodeProjet == currentProj.NoProjet);
                    var empIfNull = SingletonGeneralUse.getInstance().GetEmployeByMatricule(matricule);
                    if (emp != null)
                    {
                        double salaire = emp.HeuresTravaillees * emp.TauxHoraire;
                        totalSalaire += salaire;
                        tbEmployesDetails.Text += $"Emp: {emp.Matricule}, TxH: {emp.TauxHoraire:0.00}, Hrs: {emp.HeuresTravaillees:0.##}, Salaire: {salaire}$\n";
                    }
                    else
                    {
                        int i = comboBoxes.IndexOf(cb);
                        if (numberboxes[i].Value != double.NaN)
                        {
                            double salaire = numberboxes[i].Value  * empIfNull.TauxHoraire;
                            totalSalaire += salaire;
                            tbEmployesDetails.Text += $"Emp: {empIfNull.Matricule}, TxH: {empIfNull.TauxHoraire:0.00}, Hrs: {numberboxes[i].Value:0.##}, Salaire: {salaire}$\n";
                        }
                        
                    }
                }

            }
            tbTotalSalaire.Text = $"Total Salaires: {totalSalaire:0.##}";
        }

        private bool CheckDupePickedMatricule()
        {
            HashSet<string> selectedMat = new HashSet<string>();

            foreach (var cb in comboBoxes)
            {
                if (cb.SelectedItem == null)
                {
                    continue;
                }

                // If using SelectedValuePath="Matricule" in ComboBox
                string matricule = cb.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(matricule))
                {
                    tbxErrorNbEmployes.Text = "Erreur : Doublon d'employés sélectionnés ou rien de sélectionné";
                    tbxErrorNbEmployes.Visibility = Visibility.Visible;
                    return false;
                }

                if (selectedMat.Contains(matricule))
                {
                    tbxErrorNbEmployes.Text = "Erreur : Doublon d'employés sélectionnés ou rien de sélectionné";
                    tbxErrorNbEmployes.Visibility = Visibility.Visible;
                    return false;
                }

                selectedMat.Add(matricule);
            }

            // Clear error if all is fine
            tbxErrorNbEmployes.Text = "";
            tbxErrorNbEmployes.Visibility = Visibility.Collapsed;

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
            
            
            int filledCount = 0;
            double maxPeople = txtNbEmployesRequis.Value;
            // Count how many rows are fully filled
            for (int i = 0; i < comboBoxes.Count; i++)
            {
                if(comboBoxes[i].SelectedItem != null)
                {
                    if (!string.IsNullOrEmpty(comboBoxes[i].SelectedItem.ToString()) && numberboxes[i].Value != double.NaN)
                    {
                        filledCount++;
                    }
                }
                
            }
            if (filledCount != maxPeople)
            {
                tbxErrorNbEmployes.Text = $"Erreur : vous devez remplir exactement {maxPeople} entrées.";
                tbxErrorNbEmployes.Visibility = Visibility.Visible;
                valide = false;
            }
            else
            {
                tbxErrorNbEmployes.Visibility = Visibility.Collapsed;
            }
            // --- SI TOUT EST OK ---
            if (valide)
            {
                // Update the project object
                currentProj.Titre = titre;
                currentProj.DateDebut = dateDebut.Date;
                currentProj.Description = description;
                currentProj.Budget = budget;
                currentProj.NombreEmployesMax = Convert.ToInt32(nbEmployes);
                currentProj.TotalSalaireDu = salaires;
                currentProj.Client = client;
                currentProj.Statut = statut;

                SingletonGeneralUse.getInstance().ModifierProjet(currentProj);


                for (int i = 0; i < comboBoxes.Count; i++)
                {
                    ComboBox cb = comboBoxes[i];
                    NumberBox nb = numberboxes[i];

                    if (cb.SelectedItem == null)
                        continue; // skip empty rows

                    string matricule = cb.SelectedItem.ToString();
                    if(string.IsNullOrEmpty(matricule))
                        continue; // skip empty rows
                    var emp = SingletonGeneralUse.getInstance().GetEmployeByMatricule(matricule);

                    if (emp != null)
                    {
                        double heureT = nb.Value;
                        EmployeProjet ep = new EmployeProjet(
                            emp.Matricule,
                            currentProj.NoProjet,
                            heureT,
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

    }
}
