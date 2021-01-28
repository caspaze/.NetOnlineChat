using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using OnlineChat.Models.Chats;
namespace OnlineChat.Models.Users
{
    public class UserDAO : IUserRepository
    {
        private readonly AESCrypt crypt;
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IDbConnection connection;
        public UserDAO(IDbConnectionFactory _connectionFactory, AESCrypt _crypt)
        {
            connectionFactory = _connectionFactory;
            crypt = _crypt;
            connection = connectionFactory.GetDbConnection();
        }
        public void Create(User user)
        {
            var sqlQuery = "INSERT INTO Users (Email, Password, FirstName, LastName, Birthday, UserName,Comment,Role, IV, [Key]) " +
                "VALUES(@Email, @Password, @FirstName, @LastName,@Birthday, @UserName,@Comment,@Role, @IV, @Key)";
            user = crypt.EncryptUser(user);
            connection.Execute(sqlQuery, user);
        }
        public User GetUserOnEmail(string Email)
        {
            User user = connection.Query<User>("SELECT * FROM Users WHERE Email = @Email", new { Email }).FirstOrDefault();
            if (user != null)
            {
                user = crypt.DecryptUser(user);
            }

            return user;
        }
        public User GetUserOnLogin(string name)
        {
            User user = connection.Query<User>("SELECT * FROM Users WHERE UserName = @name", new { name }).FirstOrDefault();
            if (user != null)
            {
                user = crypt.DecryptUser(user);
            }
            return user;
        }
        public List<Chat> GetChats(int id)
        {
            return connection.Query<Chat>("SELECT C.Name,C.Id FROM Chats As C Inner Join Dependencies As S on S.ChatId=C.Id WHERE S.UserId = @id", new { id }).ToList();

        }
        public User Get(int id)
        {
            User user = connection.Query<User>("SELECT * FROM Users WHERE UserId = @id", new { id }).FirstOrDefault();
            if (user != null)
            {
                user = crypt.DecryptUser(user);
            }
            return user;
        }
        public List<string> GetAllUsers()
        {
            List<string> allUsers = connection.Query<string>("SELECT UserName FROM Users").ToList();
            return allUsers;


        }
        public List<User> GetAllUsersExceptYourself(string email)
        {
            List<User> DecryptedUsers = new List<User>();
            List<User> allUsersExceptYourself = connection.Query<User>("SELECT * FROM Users WHERE Email!=@email ORDER BY UserName", new { email }).ToList();
            foreach (User user in allUsersExceptYourself)
            {
               DecryptedUsers.Add(crypt.DecryptUser(user));             
            }
            return DecryptedUsers;
        }
        public void AddAdmin(string username)
        {
            User newAdmin = connection.Query<User>("SELECT * FROM Users WHERE UserName=@username",new { username }).FirstOrDefault();
            newAdmin = crypt.DecryptUser(newAdmin);
            newAdmin.Role = "Admin";
            newAdmin = crypt.EncryptUser(newAdmin);
            string CryptedRole = newAdmin.Role;
            var sqlQuery = "UPDATE Users SET Role='" + CryptedRole + "' WHERE UserName=@username";
            connection.Execute(sqlQuery, new { username });
        }
        public void RemoveAdmin(string username)
        {
            User PastAdmin = connection.Query<User>("SELECT * FROM Users WHERE UserName=@username", new { username }).FirstOrDefault();
            PastAdmin = crypt.DecryptUser(PastAdmin);
            PastAdmin.Role = "User";
            PastAdmin = crypt.EncryptUser(PastAdmin);
            string CryptedRole = PastAdmin.Role;
            var sqlQuery = "UPDATE Users SET Role='" + CryptedRole + "' WHERE UserName=@username";
            connection.Execute(sqlQuery, new { username });
        }
        public List<User> SearchUsers(string text, string email)
        {
            List<User> DecryptedUsers = new List<User>();
            List<User> Users = connection.Query<User>("SELECT * FROM Users WHERE Email!=@email AND UserName LIKE @text+'%' ORDER BY UserName", new { email, text }).ToList();
            foreach (User user in Users)
            {
                DecryptedUsers.Add(crypt.DecryptUser(user));
            }
            return DecryptedUsers;

        }

    }
}

