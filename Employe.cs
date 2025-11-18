using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailDeSession
{
    class Employe
    {
        string matricule = string.Empty;
        string nom = string.Empty;
        string prenom = string.Empty;
        DateTime dateNaissance = DateTime.Now;
        string email = string.Empty;
        string adresse = string.Empty;
        DateTime dateEmbauche = DateTime.Now;
        double tauxHoraire;
        Uri photoIdentite = new Uri(string.Empty);
        string statut = string.Empty;

        public Employe()
        {
            matricule = string.Empty;
            nom = string.Empty;
            prenom = string.Empty;
            dateNaissance = DateTime.Now;
            email = string.Empty;
            adresse = string.Empty;
            dateEmbauche = DateTime.Now;
            tauxHoraire = 0;
            photoIdentite = new Uri(string.Empty);
            statut = string.Empty;
        }
        public Employe(string matricule, string nom, string prenom, DateTime dateNaissance, string email, string adresse, DateTime dateEmbauche, double tauxHoraire, Uri photoIdentite, string statut)
        {
            Matricule = matricule;
            Nom = nom;
            Prenom = prenom;
            DateNaissance = dateNaissance;
            Email = email;
            Adresse = adresse;
            DateEmbauche = dateEmbauche;
            TauxHoraire = tauxHoraire;
            PhotoIdentite = photoIdentite;
            Statut = statut;
        }

        public string Matricule { get => matricule; set => matricule = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public DateTime DateNaissance { get => dateNaissance; set => dateNaissance = value; }
        public string Email { get => email; set => email = value; }
        public string Adresse { get => adresse; set => adresse = value; }
        public DateTime DateEmbauche { get => dateEmbauche; set => dateEmbauche = value; }
        public double TauxHoraire { get => tauxHoraire; set => tauxHoraire = value; }
        public Uri PhotoIdentite { get => photoIdentite; set => photoIdentite = value; }
        public string Statut { get => statut; set => statut = value; }

        public override string ToString()
        {
            return $"Matricule : {Matricule}, Nom : {Nom}, Prenom : {Prenom}";
        }
    }
}
