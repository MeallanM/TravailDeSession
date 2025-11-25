using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using Microsoft.WindowsAppSDK.Runtime.Packages;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using TravailDeSession;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using WinRT.Interop;

namespace TravailDeSession
{
    internal class SingletonGeneralUse
    {
        string stringConnectionSql;
        ObservableCollection<Client> listeClients;
        ObservableCollection<Employe> listeEmployes;
        ObservableCollection<Projet> listeProjets;
        List<int> listeIdentifiants;
        static SingletonGeneralUse instance = null;
        protected string AdminUsername = "Admin";
        protected string AdminPassword = "root"; // to be hashed later------------------------------------------------------------------------------------------------------------------------------
        protected bool adminIsAdmin = false;
        private SingletonGeneralUse()
        {
            stringConnectionSql = "Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq7;Uid=2377057;Pwd=2377057;";
            listeClients = new ObservableCollection<Client>();
            listeEmployes = new ObservableCollection<Employe>();
            listeProjets = new ObservableCollection<Projet>();
            listeIdentifiants = new List<int>();
        }
        //retourne l’instance du singleton
        public static SingletonGeneralUse getInstance()
        {
            if (instance == null)
                instance = new SingletonGeneralUse();
            return instance;
        }
        //Propriété qui retourne la liste des clients
        public ObservableCollection<Client> ListeClients { get => listeClients; }
        public ObservableCollection<Employe> ListeEmployes { get => listeEmployes; }
        public ObservableCollection<Projet> ListeProjets { get => listeProjets; }

        /*-------------------------------------------------------Méthodes Générales-------------------------------------------------------*/

        public int getNombreItemInTable(string nomTable)
        {
            MySqlConnection con = new MySqlConnection(stringConnectionSql);
            try
            {
                MySqlCommand commande = new MySqlCommand();
                commande.Connection = con;
                commande.CommandText = $"select count(*) from {nomTable}";
                con.Open();
                var res = commande.ExecuteScalar();
                con.Close();
                if (res is not null)
                    return Convert.ToInt32(res);
                else
                    return 0;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex.Message);
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
                return 0;
            }
        }

        /*-------------------------------------------------------Gestion des clients-------------------------------------------------------*/

        public void getAllClients() //charge la liste avec tous les clients
        {
            ListeClients.Clear(); //permet de vider la liste avant de la recharger
            try
            {
                //Connection sql
                using MySqlConnection con = new MySqlConnection(stringConnectionSql);
                con.Open();

                //Commande sql
                using MySqlCommand commande = con.CreateCommand();
                commande.CommandText = "SELECT * FROM VAClient";
                using MySqlDataReader r = commande.ExecuteReader();

                //Lecture et copie des résultats
                while (r.Read())
                {
                    try
                    {
                        int identifiant = r.GetInt32("id");
                        string nom = r.GetString("nom");
                        string adresse = r.GetString("adresse");
                        string telephone = r.GetString("telephone");
                        string email = r.GetString("email");

                        //Création et ajout du client
                        Client client = new Client(identifiant, nom, adresse, telephone, email);
                        ListeClients.Add(client);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Erreur lors de la lecture d'un client: " + ex.Message);
                    }
                }
            }
            catch (MySqlException ex)
            {
                //Coder l'erreure ici
                Debug.WriteLine(ex.Message);
            }
        }

        public Client? getClientWithId(int id) //charge la liste avec tous les clients
        {
            try
            {
                //Connection sql
                using MySqlConnection con = new MySqlConnection(stringConnectionSql);
                con.Open();

                //Commande sql
                using MySqlCommand commande = con.CreateCommand();
                commande.CommandText = "SELECT * FROM clients WHERE id = @id";
                commande.Parameters.AddWithValue("@id", id);
                using MySqlDataReader r = commande.ExecuteReader();

                //Lecture et copie des résultats
                if (r.Read())
                {
                    int identifiant = r.GetInt32("identifiant");
                    string nom = r.GetString("nom");
                    string adresse = r.GetString("adresse");
                    string telephone = r.GetString("telephone");
                    string email = r.GetString("email");

                    //Retour du client recherché
                    return new Client(identifiant, nom, adresse, telephone, email);
                }
                return null;
            }
            catch (MySqlException ex)
            {
                //Coder l'erreure ici
                Debug.WriteLine("Erreur lors de la recherche du client par son ID");
                return null;
            }
        }

        public List<int> getAllClientId() //charge la liste avec tous les clients
        {
            try
            {
                //Connection sql
                using MySqlConnection con = new MySqlConnection(stringConnectionSql);
                con.Open();

                //Commande sql
                using MySqlCommand commande = con.CreateCommand();
                commande.CommandText = "SELECT identifiant FROM clients";
                using MySqlDataReader r = commande.ExecuteReader();

                //Lecture et copie des résultats
                while (r.Read())
                {
                    try
                    {
                        //Ajout des identifiants à la liste des identifiants
                        foreach (int id in r)
                        {
                            int identifiant = r.GetInt32("identifiant");
                            listeIdentifiants.Add(identifiant);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Erreur lors de la lecture d'un client: " + ex.Message);
                        return null;
                    }
                }
                return listeIdentifiants;
            }
            catch (MySqlException ex)
            {
                //Coder l'erreure ici
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async void AjouterClient(Client c)
        {
            try
            {
                //Connection sql
                MySqlConnection con = new MySqlConnection(stringConnectionSql);

                //Commande sql
                MySqlCommand commandeSql = new MySqlCommand("ProcNouvClient", con);
                commandeSql.CommandType = CommandType.StoredProcedure;

                //Ajout des paramètres
                commandeSql.Parameters.AddWithValue("@id", c.Identifiant);
                commandeSql.Parameters.AddWithValue("@nom", c.Nom);
                commandeSql.Parameters.AddWithValue("@adresse", c.Adresse);
                commandeSql.Parameters.AddWithValue("@telephone", c.Telephone);
                commandeSql.Parameters.AddWithValue("@email", c.Email);

                //Début traitement SQL
                commandeSql.Prepare();
                await con.OpenAsync();
                await commandeSql.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur d'ajout du client (AjouterClient())");
            }
        }
        public void ModifierClient(Client c)
        {
            try
            {
                //Connection sql
                MySqlConnection con = new MySqlConnection(stringConnectionSql);

                //Commande sql
                MySqlCommand commandeSql = new MySqlCommand();
                commandeSql.Connection = con;
                commandeSql.CommandText = @"UPDATE clients SET nom = @nom, adresse = @adresse, telephone = @telephone,  email = @email WHERE identifiant = @identifiant";

                //Ajout des paramètres
                commandeSql.Parameters.AddWithValue("@nom", c.Nom);
                commandeSql.Parameters.AddWithValue("@adresse", c.Adresse);
                commandeSql.Parameters.AddWithValue("@telephone", c.Telephone);
                commandeSql.Parameters.AddWithValue("@email", c.Email);
                commandeSql.Parameters.AddWithValue("@identifiant", c.Identifiant);

                //Début traitement SQL
                commandeSql.Prepare();
                con.Open();
                int i = commandeSql.ExecuteNonQuery();

                //Vérification de la mise à jour
                if (i > 0)
                {
                    Debug.WriteLine($"Client #{c.Identifiant} ({c.Nom}) mis à jour avec succès !");
                    getAllClients();
                }
                else
                    Debug.WriteLine($"Erreur dans la modification du client {c.Identifiant} ({c.Nom})");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur dans la modification du client {c.Identifiant} ({c.Nom})");
            }
        }

        public void SupprimerClient(Client c)
        {
            try
            {
                //Connection sql
                MySqlConnection con = new MySqlConnection(stringConnectionSql);

                //Commande sql
                MySqlCommand commandeSql = new MySqlCommand();
                commandeSql.Connection = con;
                commandeSql.CommandText = @"DELETE FROM clients WHERE identifiant = @identifiant";

                //Ajout des paramètres
                commandeSql.Parameters.AddWithValue("@identifiant", c.Identifiant);

                //Début traitement SQL
                commandeSql.Prepare();
                con.Open();
                int i = commandeSql.ExecuteNonQuery();

                //Vérification de la Supression
                if (i > 0)
                {
                    Debug.WriteLine($"Client #{c.Identifiant} ({c.Nom}) supprimé avec succès");
                    getAllClients();
                }
                else
                    Debug.WriteLine($"Erreur dans la suppression du client {c.Identifiant} ({c.Nom})");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur dans la suppression du client {c.Identifiant} ({c.Nom})");
            }
        }

        /*-------------------------------------------------------Gestion des Employés-------------------------------------------------------*/

        public void getAllEmployes() //charge la liste avec tous les employés
        {
            ListeEmployes.Clear(); //permet de vider la liste avant de la recharger

            try
            {
                //Connection sql
                using MySqlConnection connection = new MySqlConnection(stringConnectionSql);
                connection.Open();

                //Commande sql
                using MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM employes";
                using MySqlDataReader r = command.ExecuteReader();

                //Lecture et copie des résultats
                while (r.Read())
                {
                    try
                    {
                        string matricule = r.IsDBNull("matricule") ? "" : r.GetString("matricule");
                        string nom = r.IsDBNull("nom") ? "" : r.GetString("nom");
                        string prenom = r.IsDBNull("prenom") ? "" : r.GetString("prenom");

                        DateTime? dateNaissance = r.IsDBNull("dateNaissance")
                            ? null
                            : r.GetDateTime("dateNaissance");

                        string email = r.IsDBNull("email") ? "" : r.GetString("email");
                        string adresse = r.IsDBNull("adresse") ? "" : r.GetString("adresse");

                        DateTime? dateEmbauche = r.IsDBNull("dateEmbauche")
                            ? null
                            : r.GetDateTime("dateEmbauche");

                        double? tauxHoraire = r.IsDBNull("tauxHoraire")
                            ? null
                            : r.GetDouble("tauxHoraire");

                        Uri? photoIdentite = r.IsDBNull("photoIdentite")
                            ? null
                            : new Uri(r.GetString("photoIdentite"));

                        string statut = r.IsDBNull("statut") ? "" : r.GetString("statut");

                        // If your constructor expects non-null values,
                        // you need to update the constructor OR pass defaults.
                        Employe employe = new Employe(
                            matricule,
                            nom,
                            prenom,
                            dateNaissance ?? DateTime.MinValue,
                            email,
                            adresse,
                            dateEmbauche ?? DateTime.MinValue,
                            tauxHoraire ?? 0,
                            photoIdentite,
                            statut
                        );

                        listeEmployes.Add(employe);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Erreur lors de la lecture d'un employé: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                //Coder l'erreure ici
                Debug.WriteLine(ex.Message);
            }
        }

        public void AjouterEmploye(Employe e)
        {
            try
            {
                //Connection sql
                MySqlConnection con = new MySqlConnection(stringConnectionSql);

                //Commande sql
                MySqlCommand commandeSql = new MySqlCommand();
                commandeSql.Connection = con;
                commandeSql.CommandText = @"INSERT INTO employes (matricule, nom, prenom, dateNaissance, email, adresse, dateEmbauche, tauxHoraire, photoIdentite, statut)
                                        VALUES (@matricule, @nom, @prenom, @dateNaissance, @email, @adresse, @dateEmbauche, @tauxHoraire, @photoIdentite, @statut)";

                //Ajout des paramètres
                commandeSql.Parameters.AddWithValue("@matricule", e.Matricule);
                commandeSql.Parameters.AddWithValue("@nom", e.Nom);
                commandeSql.Parameters.AddWithValue("@prenom", e.Prenom);
                commandeSql.Parameters.AddWithValue("@dateNaissance", e.DateNaissance);
                commandeSql.Parameters.AddWithValue("@email", e.Email);
                commandeSql.Parameters.AddWithValue("@adresse", e.Adresse);
                commandeSql.Parameters.AddWithValue("@dateEmbauche", e.DateEmbauche);
                commandeSql.Parameters.AddWithValue("@tauxHoraire", e.TauxHoraire);
                commandeSql.Parameters.AddWithValue("@photoIdentite", e.PhotoIdentite.ToString());
                commandeSql.Parameters.AddWithValue("@statut", e.Statut);

                //Début traitement SQL
                commandeSql.Prepare();
                con.Open();
                int i = commandeSql.ExecuteNonQuery();

                //Vérification de l'ajout
                if (i > 0)
                {
                    Debug.WriteLine($"Employé #{e.Matricule} ({e.Nom}) ajouté avec succès");
                    getAllEmployes();
                }
                else
                    Debug.WriteLine($"Erreur dans l'ajout de l'employé");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur dans l'ajout de l'employé");
            }
        }

        public void ModifierEmploye(Employe e)
        {
            try
            {
                //Connection sql
                MySqlConnection con = new MySqlConnection(stringConnectionSql);

                //Commande sql
                MySqlCommand commandeSql = new MySqlCommand();
                commandeSql.Connection = con;
                commandeSql.CommandText = @"UPDATE employes SET nom = @nom, prenom = @prenom, dateNaissance = @dateNaissance, email = @email,
                                        adresse = @adresse, dateEmbauche = @dateEmbauche, tauxHoraire = @tauxHoraire, photoIdentite = @photoIdentite,
                                        statut = @statut WHERE matricule = @matricule";

                //Ajout des paramètres
                commandeSql.Parameters.AddWithValue("@nom", e.Nom);
                commandeSql.Parameters.AddWithValue("@prenom", e.Prenom);
                commandeSql.Parameters.AddWithValue("@dateNaissance", e.DateNaissance);
                commandeSql.Parameters.AddWithValue("@email", e.Email);
                commandeSql.Parameters.AddWithValue("@adresse", e.Adresse);
                commandeSql.Parameters.AddWithValue("@dateEmbauche", e.DateEmbauche);
                commandeSql.Parameters.AddWithValue("@tauxHoraire", e.TauxHoraire);
                commandeSql.Parameters.AddWithValue("@photoIdentite", e.PhotoIdentite.ToString());
                commandeSql.Parameters.AddWithValue("@statut", e.Statut);
                commandeSql.Parameters.AddWithValue("@matricule", e.Matricule);

                //Début traitement SQL
                commandeSql.Prepare();
                con.Open();
                int i = commandeSql.ExecuteNonQuery();

                //Vérification de la mise à jour
                if (i > 0)
                {
                    Debug.WriteLine($"Employé #{e.Matricule} ({e.Nom}) modifié avec succès");
                    getAllEmployes();
                }
                else
                    Debug.WriteLine($"Erreur dans la modification de l'employé");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur dans la modification de l'employé");
            }
        }

        public void SupprimerEmploye(Employe e)
        {
            try
            {
                //Connection sql
                MySqlConnection con = new MySqlConnection(stringConnectionSql);

                //Commande sql
                MySqlCommand commandeSql = new MySqlCommand();
                commandeSql.Connection = con;
                commandeSql.CommandText = @"DELETE FROM employes WHERE matricule = @matricule";

                //Ajout des paramètres
                commandeSql.Parameters.AddWithValue("@matricule", e.Matricule);

                //Début traitement SQL
                commandeSql.Prepare();
                con.Open();
                int i = commandeSql.ExecuteNonQuery();

                //Vérification de la Supression
                if (i > 0)
                {
                    Debug.WriteLine($"Employé #{e.Matricule} ({e.Nom}) supprimé avec succès");
                    getAllEmployes();
                }
                else
                    Debug.WriteLine($"Erreur dans la suppression de l'employé #{e.Matricule} ({e.Nom})");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur dans la suppression de l'employé #{e.Matricule} ({e.Nom})");
            }
        }

        /*-------------------------------------------------------Gestion des Projets-------------------------------------------------------*/

        public void getAllProjets() //charge la liste avec tous les clients
        {
            ListeProjets.Clear(); //permet de vider la liste avant de la recharger
            try
            {
                //Connection sql
                using MySqlConnection con = new MySqlConnection(stringConnectionSql);
                con.Open();

                //Commande sql
                using MySqlCommand commande = con.CreateCommand();
                commande.CommandText = "Select * from projets";
                using MySqlDataReader r = commande.ExecuteReader();

                //Lecture et copie des résultats
                while (r.Read())
                {
                    string noProjet = r.GetString("noProjet");
                    string titre = r.GetString("titre");
                    DateTime dateDebut = r.GetDateTime("dateDebut");
                    string description = r.GetString("description");
                    double budget = r.GetDouble("budget");
                    int nombreEmployesMax = r.GetInt32("nombreEmployesMax");
                    double totalSalaireDu = r.GetDouble("totalSalaireDu");
                    string statut = r.GetString("statut");
                    Client? client = getClientWithId(r.GetInt32("client"));

                    //Création et ajout du projet dans la liste du singleton
                    Projet projet = new Projet(noProjet, titre, dateDebut, description, budget, nombreEmployesMax, totalSalaireDu, null, statut);
                    listeProjets.Add(projet);
                }
            }
            catch (MySqlException ex)
            {
                //Coder l'erreure ici
                Debug.WriteLine(ex.Message);
            }
        }

        public void AjouterProjet(Projet p)
        {
            try
            {
                //Connection sql
                MySqlConnection con = new MySqlConnection(stringConnectionSql);

                //Commande sql
                MySqlCommand commandeSql = new MySqlCommand();
                commandeSql.Connection = con;
                commandeSql.CommandText = @"INSERT INTO projets (noProjet, titre, dateDebut, description, budget, nombreEmployesMax, totalSalaireDu, clientId, statut)
                                            VALUES (@noProjet, @titre, @dateDebut, @description, @budget, @nombreEmployesMax, @totalSalaireDu, @clientId, @statut)";

                //Ajout des paramètres
                commandeSql.Parameters.AddWithValue("@noProjet", p.NoProjet);
                commandeSql.Parameters.AddWithValue("@titre", p.Titre);
                commandeSql.Parameters.AddWithValue("@dateDebut", p.DateDebut);
                commandeSql.Parameters.AddWithValue("@description", p.Description);
                commandeSql.Parameters.AddWithValue("@budget", p.Budget);
                commandeSql.Parameters.AddWithValue("@nombreEmployesMax", p.NombreEmployesMax);
                commandeSql.Parameters.AddWithValue("@totalSalaireDu", p.TotalSalaireDu);
                commandeSql.Parameters.AddWithValue("@clientId", p.Client.Identifiant); // assuming Client has Identifiant
                commandeSql.Parameters.AddWithValue("@statut", p.Statut);

                //Début traitement SQL
                commandeSql.Prepare();
                con.Open();
                int i = commandeSql.ExecuteNonQuery();

                //Vérification de l'ajout
                if (i > 0)
                {
                    Debug.WriteLine($"Projet #{p.NoProjet} ({p.Titre}) ajouté avec succès!");
                    getAllProjets();
                }
                else
                    Debug.WriteLine($"Erreur d'ajout du projet (AjouterProjet())");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur d'ajout du projet (AjouterProjet())");
            }
        }

        public void ModifierProjet(Projet p)
        {
            try
            {
                //Connection sql
                MySqlConnection con = new MySqlConnection(stringConnectionSql);

                //Commande sql
                MySqlCommand commandeSql = new MySqlCommand();
                commandeSql.Connection = con;
                commandeSql.CommandText = @"UPDATE projets SET titre = @titre, dateDebut = @dateDebut, description = @description, budget = @budget,
                                        nombreEmployesMax = @nombreEmployesMax, totalSalaireDu = @totalSalaireDu, clientId = @clientId,
                                        statut = @statut WHERE noProjet = @noProjet";

                //Ajout des paramètres
                commandeSql.Parameters.AddWithValue("@titre", p.Titre);
                commandeSql.Parameters.AddWithValue("@dateDebut", p.DateDebut);
                commandeSql.Parameters.AddWithValue("@description", p.Description);
                commandeSql.Parameters.AddWithValue("@budget", p.Budget);
                commandeSql.Parameters.AddWithValue("@nombreEmployesMax", p.NombreEmployesMax);
                commandeSql.Parameters.AddWithValue("@totalSalaireDu", p.TotalSalaireDu);
                commandeSql.Parameters.AddWithValue("@clientId", p.Client.Identifiant);
                commandeSql.Parameters.AddWithValue("@statut", p.Statut);
                commandeSql.Parameters.AddWithValue("@noProjet", p.NoProjet);

                //Début traitement SQL
                commandeSql.Prepare();
                con.Open();
                int i = commandeSql.ExecuteNonQuery();

                //Vérification de la mise à jour
                if (i > 0)
                {
                    Debug.WriteLine($"Projet #{p.NoProjet} ({p.Titre}) modifié avec succès");
                    getAllProjets();
                }
                else
                    Debug.WriteLine($"Erreur dans la modification du projet");

            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur dans la modification du projet");
            }
        }

        public void SupprimerProjet(Projet p)
        {
            try
            {
                //Connection sql
                MySqlConnection con = new MySqlConnection(stringConnectionSql);

                //Commande sql
                MySqlCommand commandeSql = new MySqlCommand();
                commandeSql.Connection = con;
                commandeSql.CommandText = @"DELETE FROM projets WHERE noProjet = @noProjet";

                //Ajout des paramètres
                commandeSql.Parameters.AddWithValue("@noProjet", p.NoProjet);

                //Début traitement SQL
                commandeSql.Prepare();
                con.Open();
                int i = commandeSql.ExecuteNonQuery();

                //Vérification de la Supression
                if (i > 0)
                {
                    Debug.WriteLine($"Projet #{p.NoProjet} ({p.Titre}) supprimé avec succès");
                    getAllProjets();
                }
                else
                    Debug.WriteLine($"Erreur dans la suppression du projet #{p.NoProjet} ({p.Titre})");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur dans la suppression du projet #{p.NoProjet} ({p.Titre})");
            }
        }

        /*-------------------------------------------------------Gestion Imports-------------------------------------------------------*/

        public async void ImportClientsAsync()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.FileTypeFilter.Add(".csv");
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.fenetrePrincipale);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

                Windows.Storage.StorageFile monFichier = await picker.PickSingleFileAsync();
                if (monFichier != null)
                {
                    var lignes = await Windows.Storage.FileIO.ReadLinesAsync(monFichier);
                    List<Client> liste = new List<Client>();

                    foreach (var ligne in lignes)
                    {
                        var v = ligne.Split(";");
                        if (v.Length >= 5) // must have all columns
                        {
                            int identifiant = Convert.ToInt32(v[0]);
                            string nom = v[1];
                            string adresse = v[2];
                            string telephone = v[3];
                            string email = v[4];

                            Client client = new Client(identifiant, nom, adresse, telephone, email);
                            liste.Add(client);
                        }
                    }

                    // Add all clients to singleton after parsing CSV
                    foreach (var client in liste)
                    {
                        SingletonGeneralUse.getInstance().AjouterClient(client);
                    }

                    SingletonGeneralUse.getInstance().getAllClients(); // reload list once
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Erreur import CSV Clients: " + ex.Message);
            }
        }

        public async void ImportEmployesAsync()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.FileTypeFilter.Add(".csv");
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.fenetrePrincipale);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

                Windows.Storage.StorageFile monFichier = await picker.PickSingleFileAsync();
                if (monFichier != null)
                {
                    var lignes = await Windows.Storage.FileIO.ReadLinesAsync(monFichier);
                    List<Employe> liste = new List<Employe>();

                    foreach (var ligne in lignes)
                    {
                        var v = ligne.Split(";");
                        if (v.Length >= 10)
                        {
                            //Récupération des valeurs
                            string matricule = v[0];
                            string nom = v[1];
                            string prenom = v[2];
                            DateTime dateNaissance = DateTime.Parse(v[3]);
                            string email = v[4];
                            string adresse = v[5];
                            DateTime dateEmbauche = DateTime.Parse(v[6]);
                            double tauxHoraire = Convert.ToDouble(v[7]);
                            Uri photoIdentite = new Uri(v[8]);
                            string statut = v[9];

                            //Création et ajout de l'employé
                            Employe employe = new Employe(matricule, nom, prenom, dateNaissance, email, adresse, dateEmbauche, tauxHoraire, photoIdentite, statut);
                            liste.Add(employe);
                        }
                    }

                    //Ajoute les employés importés à la liste du singleton
                    foreach (var e in liste)
                    {
                        SingletonGeneralUse.getInstance().AjouterEmploye(e);
                    }

                    //Rafraichis la liste après l'import
                    SingletonGeneralUse.getInstance().getAllEmployes(); // reload list once
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Erreur import CSV Employes: " + ex.Message);
            }
        }

        public async void ImportProjetsAsync()
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.FileTypeFilter.Add(".csv");
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.fenetrePrincipale);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

                Windows.Storage.StorageFile monFichier = await picker.PickSingleFileAsync();
                if (monFichier != null)
                {
                    var lignes = await Windows.Storage.FileIO.ReadLinesAsync(monFichier);
                    List<Projet> liste = new List<Projet>();

                    foreach (var ligne in lignes)
                    {
                        var v = ligne.Split(";");
                        if (v.Length >= 9)
                        {
                            string noProjet = v[0];
                            string titre = v[1];
                            DateTime dateDebut = DateTime.Parse(v[2]);
                            string description = v[3];
                            double budget = Convert.ToDouble(v[4]);
                            int nombreEmployesMax = Convert.ToInt32(v[5]);
                            double totalSalaireDu = Convert.ToDouble(v[6]);
                            Client? client = getClientWithId(Convert.ToInt32(v[7]));
                            string statut = v[8];

                            if (client != null)
                            {
                                Projet projet = new Projet(noProjet, titre, dateDebut, description, budget, nombreEmployesMax, totalSalaireDu, client, statut);
                                liste.Add(projet);
                            }
                        }
                    }

                    //Ajoute les projets importés à la liste du singleton
                    foreach (var p in liste)
                    {
                        SingletonGeneralUse.getInstance().AjouterProjet(p);
                    }

                    //Rafraichis la liste après l'import
                    SingletonGeneralUse.getInstance().getAllProjets();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Erreur import CSV Projets: " + ex.Message);
            }
        }



        /*-------------------------------------------------------Gestion Exports-------------------------------------------------------*/

        public async void ExportClientsAsync()
        {
            try
            {
                SingletonGeneralUse.getInstance().getAllClients();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.fenetrePrincipale);

                var picker = new Windows.Storage.Pickers.FileSavePicker();
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                picker.SuggestedFileName = "clients_export";
                picker.FileTypeChoices.Add("Fichier CSV", new List<string>() { ".csv" });

                Windows.Storage.StorageFile monFichier = await picker.PickSaveFileAsync();
                if (monFichier == null) return;

                List<string> lignes = new List<string>();
                foreach (var c in SingletonGeneralUse.getInstance().ListeClients)
                {
                    // Format: identifiant;nom;adresse;telephone;email
                    string ligne = $"{c.Identifiant};{c.Nom};{c.Adresse};{c.Telephone};{c.Email}";
                    lignes.Add(ligne);
                }

                await Windows.Storage.FileIO.WriteLinesAsync(monFichier, lignes, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Erreur export CSV Clients: " + ex.Message);
            }
        }

        public async void ExportEmployesAsync()
        {
            try
            {
                SingletonGeneralUse.getInstance().getAllEmployes();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.fenetrePrincipale);

                var picker = new Windows.Storage.Pickers.FileSavePicker();
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                picker.SuggestedFileName = "employes_export";
                picker.FileTypeChoices.Add("Fichier CSV", new List<string>() { ".csv" });

                Windows.Storage.StorageFile monFichier = await picker.PickSaveFileAsync();
                if (monFichier == null) return;

                List<string> lignes = new List<string>();
                foreach (var e in SingletonGeneralUse.getInstance().ListeEmployes)
                {
                    // Format: matricule;nom;prenom;dateNaissance;email;adresse;dateEmbauche;tauxHoraire;photoIdentite;statut
                    string ligne = $"{e.Matricule};{e.Nom};{e.Prenom};{e.DateNaissance:yyyy-MM-dd};{e.Email};{e.Adresse};{e.DateEmbauche:yyyy-MM-dd};{e.TauxHoraire};{e.PhotoIdentite};{e.Statut}";
                    lignes.Add(ligne);
                }

                await Windows.Storage.FileIO.WriteLinesAsync(monFichier, lignes, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Erreur export CSV Employes: " + ex.Message);
            }
        }

        public async void ExportProjetsAsync()
        {
            try
            {
                SingletonGeneralUse.getInstance().getAllProjets();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.fenetrePrincipale);

                var picker = new Windows.Storage.Pickers.FileSavePicker();
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                picker.SuggestedFileName = "projets_export";
                picker.FileTypeChoices.Add("Fichier CSV", new List<string>() { ".csv" });

                Windows.Storage.StorageFile monFichier = await picker.PickSaveFileAsync();
                if (monFichier == null) return;

                List<string> lignes = new List<string>();
                foreach (var p in SingletonGeneralUse.getInstance().ListeProjets)
                {
                    // Format: noProjet;titre;dateDebut;description;budget;nombreEmployesMax;totalSalaireDu;clientId;statut
                    string ligne = $"{p.NoProjet};{p.Titre};{p.DateDebut:yyyy-MM-dd};{p.Description};{p.Budget};{p.NombreEmployesMax};{p.TotalSalaireDu};{p.Client.Identifiant};{p.Statut}";
                    lignes.Add(ligne);
                }

                await Windows.Storage.FileIO.WriteLinesAsync(monFichier, lignes, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Erreur export CSV Projets: " + ex.Message);
            }
        }

        /*-------------------------------------------------------Connections Administrateur-------------------------------------------------------*/

        public bool AdminIsAdmin { get => adminIsAdmin; set => adminIsAdmin = value; }

        public bool ValiderAdmin(string nom, string mdp)
        {
            if (nom.Equals(AdminUsername) && mdp.Equals(AdminPassword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}