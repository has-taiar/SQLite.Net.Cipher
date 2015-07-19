using SQLite.Net.Cipher.Interfaces;
using SQLite.Net.Cipher.Model;

namespace SQLiteNetCipher.Sample.Data
{
	public class SampleUser : IModel
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[Secure]
		public string Password { get; set; }
		
		public string Bio { get; set; }
	}


}