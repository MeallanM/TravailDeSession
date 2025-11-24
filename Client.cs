using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public int Identifiant { get => identifiant; set => identifiant = value;}
        public string Nom { get => nom; set 
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Le nom ne peut pas être vide.");
                }
                nom = value;
            } }
        public string Adresse { get => adresse; set => adresse = value; }
        public string Telephone { get => telephone; set 
            {
                if(!IsPhoneValid(value))
                {
                    throw new ArgumentException("Le numéro de téléphone n'est pas valide.");
                }
                telephone = value;
            } }
        public string Email { get => email; set 
            {
                if(!IsEmailValid(value))
                {
                    throw new ArgumentException("L'adresse e-mail n'est pas valide.");
                }
                email = value;
            } }

        private bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsPhoneValid(string phone)
        {
            return Regex.IsMatch(phone, @"^(\+?1[-.\s]?)?(\(?\d{3}\)?)[-.\s]?\d{3}[-.\s]?\d{4}$");
        }

        public override string ToString()
        {
            return $"Identifiant : {Identifiant}, Nom : {Nom} ";
        }
    }
}
