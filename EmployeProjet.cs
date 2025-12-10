using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailDeSession
{
    internal class EmployeProjet
    {
        string matricule;
        string codeProjet;
        double heuresTravaillees;
        double tauxHoraire;
        public EmployeProjet(string matricule, string codeProjet, double heuresTravaillees, double tauxHoraire)
        {
            this.matricule = matricule;
            this.codeProjet = codeProjet;
            this.heuresTravaillees = heuresTravaillees;
            this.tauxHoraire = tauxHoraire;
        }
        public string Matricule { get => matricule; set => matricule = value; }
        public string CodeProjet { get => codeProjet; set => codeProjet = value; }
        public double HeuresTravaillees { get => heuresTravaillees; set => heuresTravaillees = value; }
        public double TauxHoraire { get => tauxHoraire; set => tauxHoraire = value; }

        public override string ToString()
        {
            return $"Matricule: {Matricule}, CodeProjet: {CodeProjet}, HeuresTravaillées: {HeuresTravaillees}, TauxHoraire: {TauxHoraire}";
        }
    }
}
