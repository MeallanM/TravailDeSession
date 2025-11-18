using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailDeSession
{
    class Client
    {
        int identifiant;
        string nom = string.Empty;
        string adresse = string.Empty;
        string telephone = string.Empty;
        string email = string.Empty;
        public Client()
        {
            identifiant = 0;
            nom = string.Empty;
            adresse = string.Empty;
            telephone = string.Empty;
            email = string.Empty;
        }
        public Client(int identifiant, string nom, string adresse, string telephone, string email)
        {
            Identifiant = identifiant;
            Nom = nom;
            Adresse = adresse;
            Telephone = telephone;
            Email = email;
        }

        public int Identifiant { get => identifiant; set => identifiant = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Adresse { get => adresse; set => adresse = value; }
        public string Telephone { get => telephone; set => telephone = value; }
        public string Email { get => email; set => email = value; }

        public override string ToString()
        {
            return $"Identifiant : {Identifiant}, Nom : {Nom} ";
        }
    }
}
