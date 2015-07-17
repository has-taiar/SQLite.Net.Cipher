using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SQLite.Net.Cipher.Interfaces;
using SQLite.Net.Cipher.Model;
using SQLite.Net.Cipher.Security;
using SQLite.Net.Interop;

namespace SQLite.Net.Cipher.Data
{
	public abstract class SecureDatabase : SQLiteConnection, ISecureDatabase
	{
		private ICryptoService _cryptoService;

		protected SecureDatabase(ISQLitePlatform platform, string dbfile) : this(platform, dbfile, new CryptoService())
		{			
		}

		protected SecureDatabase (ISQLitePlatform platform, string dbfile, ICryptoService cryptoService) : base (platform, dbfile)
		{
			_cryptoService = cryptoService;
			CreateTables ();
		}

		protected abstract void CreateTables();

		public int SecureExecuteScalar(string query, params object[] args)
		{
			return ExecuteScalar<int> (query, args);
		}

		List<T> ISecureDatabase.SecureQuery<T>(string query, string keySeed, params object[] args)
		{
			var list = Query<T> (query, args);
			DecryptList(list, keySeed);
			return list;
		}

		int ISecureDatabase.SecureInsert<T>(T obj, string keySeed)
		{
			Encrypt(obj,keySeed);
			return base.Insert(obj);
		}

		int ISecureDatabase.SecureUpdate<T>(T obj, string keySeed)
		{
			Encrypt(obj, keySeed);
			return base.Update(obj);
		}

		int ISecureDatabase.SecureDelete<T>(string id)
		{
			return base.ExecuteScalar<int>(string.Format("Delete from {0} where id = ? ", typeof (T).Name), id);
		}

		T ISecureDatabase.SecureGet<T>(string id, string keySeed)
		{
			var matching =  base.Query<T>(string.Format("select * from {0} where id = ? ", typeof (T).Name), id);
			var item = matching.FirstOrDefault();
			Decrypt(item, keySeed);
			return item;
		}

		List<T> ISecureDatabase.SecureGetAll<T>(string keySeed)
		{
			var list = base.Query<T>(string.Format("select * from {0}", typeof (T).Name));
			DecryptList(list, keySeed);
			return list;
		}

		int ISecureDatabase.SecureGetCount<T>()
		{
			return base.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM {0} ", typeof(T).Name));
		}

		#region Implementation

		private void Encrypt(object model, string keySeed)
		{
			var type = model.GetType();

			var secureProperties = type.GetRuntimeProperties()
							.Where(pi => pi.PropertyType == typeof (string) && pi.GetCustomAttributes<Secure>(true).Any());

			foreach (var propertyInfo in secureProperties)
			{
				var rawPropertyValue = (string) propertyInfo.GetValue(model);
				var encrypted = _cryptoService.Encrypt(rawPropertyValue, keySeed, null);
				propertyInfo.SetValue(model, encrypted);
			}
		}

		private void Decrypt(object model, string keySeed)
		{
			var type = model.GetType();

			var secureProperties = type.GetRuntimeProperties()
							.Where(pi => pi.PropertyType == typeof(string) && pi.GetCustomAttributes<Secure>(true).Any());

			foreach (var propertyInfo in secureProperties)
			{
				var rawPropertyValue = (string)propertyInfo.GetValue(model);
				var decrypted = _cryptoService.Decrypt(rawPropertyValue, keySeed, null);
				propertyInfo.SetValue(model, decrypted);
			}
		}

		private void DecryptList<T>(List<T> list, string keySeed)
		{
			foreach (var item in list)
				Decrypt(item, keySeed);
		}

		#endregion
	}
}
