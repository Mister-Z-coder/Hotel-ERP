using Hotel.Data;
using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Hotel.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        //private List<User> _users = new List<User>
        //{
        //    new User { Id = 1, Username = "admin", Password = "admin123", Role = "Admin" },
        //    new User { Id = 2, Username = "user", Password = "user123", Role = "User" }
        //};

        //public User Authenticate(string username, string password)
        //{
        //    return _db.Users.SingleOrDefault(u => u.Username == username && u.Password == password);
        //}
        public User Authenticate(string username, string password)
        {
            // Récupérer l'utilisateur par son nom d'utilisateur
            var user = _db.Users.SingleOrDefault(u => u.Username == username);

            // Si l'utilisateur n'existe pas, retourner null
            if (user == null)
            {
                return null;
            }

            // Vérifier si le mot de passe fourni correspond au mot de passe hashé stocké
            bool isPasswordValid = PasswordHasher.VerifyPassword(password, user.Password);
            //bool isPasswordValid = String.Equals(password, user.Password, StringComparison.Ordinal);

            bool isUserNameValid = String.Equals(username, user.Username, StringComparison.Ordinal);
            // Si le mot de passe est valide, retourner l'utilisateur
            if (isPasswordValid && isUserNameValid)
            {
                return user;
            }

            // Si le mot de passe est invalide, retourner null
            return null;
        }

    }
}
