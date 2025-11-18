using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailDeSession
{
    class Projet
    {
        string noProjet = string.Empty;
        string titre = string.Empty;
        DateTime dateDebut = DateTime.Now;
        string description = string.Empty;
        double budget;
        int nombreEmployesMax;
        double totalSalaireDu;
        Client client = new Client();
        string statut = string.Empty;
        public Projet()
        {
            noProjet = string.Empty;
            titre = string.Empty;
            dateDebut = DateTime.Now;
            description = string.Empty;
            budget = 0;
            nombreEmployesMax = 0;
            totalSalaireDu = 0;
            client = new Client();
            statut = string.Empty;
        }
        public Projet(string noProjet, string titre, DateTime dateDebut, string description, double budget, int nombreEmployesMax, double totalSalaireDu, Client client, string statut)
        {
            NoProjet = noProjet;
            Titre = titre;
            DateDebut = dateDebut;
            Description = description;
            Budget = budget;
            NombreEmployesMax = nombreEmployesMax;
            TotalSalaireDu = totalSalaireDu;
            Client = client;
            Statut = statut;
        }

        public string NoProjet { get => noProjet; set => noProjet = value; }
        public string Titre { get => titre; set => titre = value; }
        public DateTime DateDebut { get => dateDebut; set => dateDebut = value; }
        public string Description { get => description; set => description = value; }
        public double Budget { get => budget; set => budget = value; }
        public int NombreEmployesMax { get => nombreEmployesMax; set => nombreEmployesMax = value; }
        public double TotalSalaireDu { get => totalSalaireDu; set => totalSalaireDu = value; }
        public string Statut { get => statut; set => statut = value; }
        internal Client Client { get => client; set => client = value; }

        public override string ToString()
        {
            return $"NoProjet : {NoProjet}, Titre : {Titre}";
        }
    }
}
