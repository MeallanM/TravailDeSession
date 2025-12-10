using Google.Protobuf.WellKnownTypes;
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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TravailDeSession
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageAjouterProjet : Page
    {
        List<ComboBox> comboBoxes;
        List<NumberBox> numberboxes;
        public ObservableCollection<int> ListeClients { get; set; } = new();
        public ObservableCollection<string> MatriculesLibres { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> matriculesEmpProj { get; set; } = new ObservableCollection<string>();
        private ObservableCollection<EmployeProjet> EmployeProj { get; set; } = new ObservableCollection<EmployeProjet>();
        private double totalSalaire = 0;
        public PageAjouterProjet()
        {
            InitializeComponent();
            List<int> listeIdClient = SingletonGeneralUse.getInstance().getAllClientId();
            foreach (int id in listeIdClient) // matricule is a string
            {
                ListeClients.Add(id);
            }
            CbbxClient.ItemsSource = ListeClients;
            dpDateDebut.Date = DateTime.MinValue;
            SingletonGeneralUse.getInstance().getEmployesTermine();
            MatriculesLibres = SingletonGeneralUse.getInstance().ListeMatDispo;
            comboBoxes = new List<ComboBox> { cbMatricule1, cbMatricule2, cbMatricule3, cbMatricule4, cbMatricule5 };
            numberboxes = new List<NumberBox> { nbHeures1, nbHeures2, nbHeures3, nbHeures4, nbHeures5 };
            for (int i = 0; i < comboBoxes.Count; i++)
            {
                comboBoxes[i].ItemsSource = MatriculesLibres;
                comboBoxes[i].SelectionChanged += (s, ev) => UpdateEmployesInfos();
                numberboxes[i].ValueChanged += (s, ev) => UpdateEmployesInfos();
            }
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
        // On click du bouton créer projet
        private void ProjetCreation_Click(object sender, RoutedEventArgs e)
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
            string statut = "en cours";

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
            if (salaires <= 0 || double.IsNaN(salaires))
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
                if (comboBoxes[i].SelectedItem != null)
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
                // Crée l’objet projet
                Projet p = new Projet(
                    "",
                    titre,
                    dateDebut.Date,
                    description,
                    budget,
                    Convert.ToInt32(nbEmployes),
                    salaires,
                    client,
                    statut
                );
                string noProj = SingletonGeneralUse.getInstance().AjouterProjetRetourNumero(p);
                p.NoProjet = noProj;
                // Ajout des employés au projet
                for (int i = 0; i < comboBoxes.Count; i++)
                {
                    ComboBox cb = comboBoxes[i];
                    NumberBox nb = numberboxes[i];

                    if (cb.SelectedItem == null)
                        continue; // skip empty rows

                    string matricule = cb.SelectedItem.ToString();
                    if (string.IsNullOrEmpty(matricule))
                        continue; // skip empty rows
                    var emp = SingletonGeneralUse.getInstance().GetEmployeByMatricule(matricule);

                    if (emp != null)
                    {
                        double heureT = nb.Value;
                        EmployeProjet ep = new EmployeProjet(
                            emp.Matricule,
                            p.NoProjet,
                            heureT,
                            emp.TauxHoraire
                        );
                        SingletonGeneralUse.getInstance().UpdateEmployesForProjet(ep);
                    }
                }

                Frame.Navigate(typeof(PageAfficherProjets));
            }
        }

        private void UpdateEmployesInfos()
        {
            SingletonGeneralUse.getInstance().GetEmployesProjetInfo();
            EmployeProj = SingletonGeneralUse.getInstance().AutreListeEmpProj;
            tbEmployesDetails.Text = "";
            totalSalaire = 0;
            foreach (ComboBox cb in spEmployesSelection.Children)
            {
                if (cb.SelectedItem != null)
                {
                    string matricule = cb.SelectedItem.ToString();
                    var emp = EmployeProj.FirstOrDefault(x => x.Matricule == matricule);
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
                            double salaire = numberboxes[i].Value * empIfNull.TauxHoraire;
                            totalSalaire += salaire;
                            tbEmployesDetails.Text += $"Emp: {empIfNull.Matricule}, TxH: {empIfNull.TauxHoraire:0.00}, Hrs: {numberboxes[i].Value:0.##}, Salaire: {salaire}$\n";
                        }

                    }
                }
                
            }
            tbTotalSalaire.Text = $"Total Salaires: {totalSalaire:0.##}";
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
