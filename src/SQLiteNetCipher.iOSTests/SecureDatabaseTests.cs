using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SQLite.Net;
using SQLite.Net.Cipher.Data;
using SQLite.Net.Cipher.Interfaces;
using SQLite.Net.Cipher.Model;
using SQLite.Net.Cipher.Security;
using SQLite.Net.Interop;

namespace SQLiteNetCipher.iOSTests
{
	[TestFixture]
	public class SecureDatabaseTests
	{
		[Test]
		public void MainDbTests()
		{
			var dbFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "mysequredb.db3");
			var platform = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
			var saltText = CryptoService.GenerateRandomKey(16);
			ISecureDatabase database = new MyDatabase(platform, dbFilePath, saltText);
			var keySeed = "my very very secure key seed. You should use PCLCrypt strong random generator for this";

			var user = new SampleUser()
			{
				Name = "Has AlTaiar",
				Password = "very secure password :)",
				Bio = "Very cool guy :) ",
				Id = Guid.NewGuid().ToString()
			};

			var inserted = database.SecureInsert<SampleUser>(user, keySeed);
			Assert.AreEqual(1, inserted);
			Assert.AreNotEqual("very secure password :)", user.Password);


			var userFromDb = database.SecureGet<SampleUser>(user.Id, keySeed);
			Assert.IsNotNull(userFromDb);
			Assert.AreEqual("Has AlTaiar",  userFromDb.Name);
			Assert.AreEqual("very secure password :)", userFromDb.Password);


			var directAccessDb = (SQLiteConnection)database;
			var userAccessedDirectly = directAccessDb.Query<SampleUser>("SELECT * FROM SampleUser").FirstOrDefault();

			Assert.IsNotNull(userAccessedDirectly);
			Assert.AreEqual("Has AlTaiar", userAccessedDirectly.Name);
			Assert.AreNotEqual("very secure password :)", userAccessedDirectly.Password);
		}
	}

	public class MyDatabase : SecureDatabase
	{
		public MyDatabase(ISQLitePlatform platform, string dbfile, string saltText) : base(platform, dbfile, saltText)
		{
		}

		protected override void CreateTables()
		{
			CreateTable<SampleUser>();
		}
	}

	public class SampleUser : IModel
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[Secure]
		public string Password { get; set; }

		public string Bio { get; set; }
	}
}