using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using OnlineChat.Models.Users;
using OnlineChat.Models.Messages;
namespace OnlineChat.Models.Chats
{
    public class ChatDAO
    {
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IDbConnection connection;
        UserDAO repos;
        public ChatDAO(UserDAO _repos, IDbConnectionFactory _connectionFactory)
        {
            connectionFactory = _connectionFactory;
            repos = _repos;
            connection = connectionFactory.GetDbConnection();
        }
        public void Create(Chat chat, string email)
        {
            var sqlQuery = "INSERT INTO Chats (Name,GUID) VALUES(@Name,@GUID)";
            connection.Execute(sqlQuery, chat);
            string p = chat.Name;
            Join(email, chat);
        }
        public void Join(string email, Chat chat)
        {
            var sqlQuery = "INSERT INTO Dependencies (ChatId,UserId) VALUES(@chat,@user)";
            chat = GetChatOnName(chat.Name);
            User user = repos.GetUserOnEmail(email);
            connection.Execute(sqlQuery, new { chat = chat.Id, user = user.UserId });
            
        }
        public Chat GetChatOnName(string name)
        {
            return connection.Query<Chat>("SELECT * FROM Chats WHERE Name = @name", new { name }).FirstOrDefault();
            
        }
        public List<Message> GetMessages(int id)
        {
            return connection.Query<Message>("SELECT M.Id,M.Color,M.Text,M.ChatId,M.UserId,M.DocId FROM Messages AS M Inner Join Chats AS C  on M.ChatId = C.Id WHERE C.id=@id", new { id }).ToList();
           
        }
        public Chat Get(int id)
        {
            return connection.Query<Chat>("SELECT * FROM Chats WHERE Id = @id", new { id }).FirstOrDefault();
            
        }
        public Chat GetOnGUID(string id)
        {
            return connection.Query<Chat>("SELECT * FROM Chats WHERE GUID = @id", new { id }).FirstOrDefault();

        }
    }
}
