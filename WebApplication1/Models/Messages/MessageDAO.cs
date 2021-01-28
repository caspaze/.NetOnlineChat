using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using OnlineChat.Models.Documents;
namespace OnlineChat.Models.Messages
{
    public class MessageDAO : MessageIntesface
    {
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IDbConnection connection;
        public MessageDAO(IDbConnectionFactory _connectionFactory)
        {
            connectionFactory = _connectionFactory;
            connection = connectionFactory.GetDbConnection();
        }
        public void Create(Message message)
        {
 
                var sqlQuery = "INSERT INTO Messages (Color, Text, ChatId, UserId) VALUES(@Color, @Text, @ChatId,@UserId)";
            connection.Execute(sqlQuery, message);
        }

        public void CreateWithDoc(Message message,Document doc)
        {
            var sqlQuery1 = "INSERT INTO Documents (GUID, Name, Data) VALUES(@GUID, @Name, @Data)";
            connection.Execute(sqlQuery1, doc);
            message.DocId = GetDocument(doc.GUID).Id;
            var sqlQuery2 = "INSERT INTO Messages (Color, Text, ChatId, UserId,DocId) VALUES(@Color, @Text, @ChatId,@UserId,@DocId)";
            connection.Execute(sqlQuery2, message);
        }

        public Document GetDocument(string GUID)
        {
            return connection.Query<Document>("Select * FROM Documents WHERE GUID = @GUID", new { GUID }).FirstOrDefault();
        }
        public Message Get(int id)
        {
            return connection.Query<Message>("SELECT * FROM Messages WHERE Id = @id", new { id }).FirstOrDefault();
        }
        public Document GetDoc(int id)
        {
            return connection.Query<Document>("SELECT * FROM Documents WHERE Id = @id", new { id }).FirstOrDefault();
        }
    }
}
