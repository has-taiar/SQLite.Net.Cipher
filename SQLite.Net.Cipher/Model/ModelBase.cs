using System;
using SQLite.Net.Attributes;
using SQLite.Net.Cipher.Interfaces;

namespace SQLite.Net.Cipher.Model
{
	public class ModelBase : IModel
	{
		[PrimaryKey]
		public string Id { get; set; }

		public string CreateKey()
		{
			return Guid.NewGuid ().ToString ().ToUpper ();
		}
	}
}

