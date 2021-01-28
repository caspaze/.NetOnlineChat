using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.Models.Users
{
	public class User
	{
		public int UserId { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime Birthday { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Comment { get; set; }
		public string Role { get; set; }
		public byte[] IV { get; set; }
		public byte[] Key { get; set; }

		public override string ToString()
		{
			return "UserId = " + UserId + '\n' +
				"FirstName = " + FirstName + '\n' +
				"LastName = " + LastName + '\n' +
				"Birthday = " + Birthday + '\n' +
				"UserName = " + UserName + '\n' +
				"Email = " + Email + '\n' +
				"Password = " + Password + '\n' +
				"Comment = " + Comment + '\n' +
				"Role = " + Role + '\n' +
				"IV = " + Convert.ToBase64String(IV) + '\n' +
				"Key = " + Convert.ToBase64String(Key) + '\n' + '\n' + '\n';
		}
	}
}
