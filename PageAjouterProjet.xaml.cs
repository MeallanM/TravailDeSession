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
                foreach (ComboBox cb in spEmployesSelection.Children)
                {
                    string matricule = cb.SelectedItem.ToString();
                    var emp = EmployeProj.FirstOrDefault(x => x.Matricule == matricule);
                    if (emp != null)
                    {
                        EmployeProjet ep = new EmployeProjet(
                            emp.Matricule,
                            p.NoProjet,
                            emp.HeuresTravaillees,
                            emp.TauxHoraire
                        );
                        SingletonGeneralUse.getInstance().AjouterEmployeProjet(ep);
                    }
                }

                Frame.Navigate(typeof(PageAfficherProjets));
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

        //Fonction qui gère l'ajout ou le retrait des combobox selon le nombre d'employés requis
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

            int current = spEmployesSelection.Children.Count;
            if (required > current)
            {
                for (int i = current; i < required; i++)
                {
                    ComboBox cb = new ComboBox()
                    {
                        ItemsSource = MatriculesLibres,
                        PlaceholderText = "Sélectionner un employé",
                        Width = double.NaN, // stretch horizontally
                    };
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
