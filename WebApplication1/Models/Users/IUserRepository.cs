using OnlineChat.Models.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.Models.Users
{
	public interface IUserRepository
	{
		void Create(User user);
		User GetUserOnEmail(string Email);
		public User GetUserOnLogin(string name);
		public List<Chat> GetChats(int id);
		public User Get(int id);
	}
}
