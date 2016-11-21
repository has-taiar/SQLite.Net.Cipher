using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SQLite.Net.Cipher.Interfaces;
using SQLite.Net.Cipher.Model;
using SQLite.Net.Cipher.Security;
using SQLite.Net.Cipher.Utility;
using SQLite.Net.Interop;
using System;

namespace SQLite.Net.Cipher.Data
{
    /// <summary>
    /// This is the main entiry in this library. Extend this class to get the benefits of this library. 
    /// You will need to implement the abstract method CreateTables();
    /// </summary>
	public abstract class SecureDatabase : SQLiteConnection, ISecureDatabase
	{
		private ICryptoService _cryptoService;

		/// <summary>
		/// Construct a new instance of SecureDatabase. 
		/// </summary>
		/// <param name="platform">The platform specific engine of SQLite (ISQLitePlatform)</param>
		/// <param name="dbfile">The sqlite db file path</param>
		protected SecureDatabase(ISQLitePlatform platform, string dbfile) : this(platform, dbfile, "SOME-RANDOM-SALT")
		{			
		}

		/// <summary>
		/// Construct a new instance of SecureDatabase. 
		/// </summary>
		/// <param name="platform">The platform specific engine of SQLite (ISQLitePlatform)</param>
		/// <param name="dbfile">The sqlite db file path</param>
		/// /// <param name="randomSaltText">The random salt text</param>
		protected SecureDatabase(ISQLitePlatform platform, string dbfile, string randomSaltText) : this(platform, dbfile, new CryptoService(randomSaltText))
		{
		}

		/// <summary>
		/// Construct a new instance of SecureDatabase. 
		/// This ctor allows you pass an instance of the CryptoService. You could use the one provided by SQLite.Net.Cipher or build and pass your own. 
		/// </summary>
		/// <param name="platform">The platform specific engine of SQLite (ISQLitePlatform)</param>
		/// <param name="dbfile">The sqlite db file path</param>
		/// <param name="cryptoService">An instance of the Crypto Service</param>
		protected SecureDatabase (ISQLitePlatform platform, string dbfile, ICryptoService cryptoService) : base (platform, dbfile)
		{
			_cryptoService = cryptoService;
			CreateTables ();
		}

		/// <summary>
		/// Override this method to create your tables 
		/// </summary>
		protected abstract void CreateTables();

		/// <summary>
		/// Executes an sql query against the database. 
		/// This method does not do anything more than the base.ExecuteScalar();
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		/// <returns>no of affected rows</returns>
		public int SecureExecuteScalar(string query, params object[] args)
		{
			return ExecuteScalar<int> (query, args);
		}

		/// <summary>
		/// gets a list of objects from the database using Query() method 
		/// and it decrypt all object properties that have the attribute Secure. 
		/// </summary>
		/// <typeparam name="T">The type of the object</typeparam>
		/// <param name="query">The Sql query</param>
		/// <param name="keySeed">The encryption key seed (must be the same that you use when inserted into the database).</param>
		/// <param name="args">The sql query parameters.</param>
		/// <returns>List of T </returns>
		List<T> ISecureDatabase.SecureQuery<T>(string query, string keySeed, params object[] args)
		{
			var list = Query<T> (query, args);
			DecryptList(list, keySeed);
			return list;
		}

		/// <summary>
		/// Inserts into the database
		/// Before inserting, it encrypts all propertiese that have the Secure attribute. 
		/// </summary>
		/// <typeparam name="T">The Type of the object to be inserted</typeparam>
		/// <param name="obj"> the object to be inserted to the database</param>
		/// <param name="keySeed">The encryption key seed. You must use the same key seed when accessing the object out of the database.</param>
		/// <returns>no of affected rows</returns>
		int ISecureDatabase.SecureInsert<T>(T obj, string keySeed)
		{
            Guard.CheckForNull(obj, "obj cannot be null");

			Encrypt(obj,keySeed);
			return base.Insert(obj);
		}

		/// <summary>
		/// Inserts or Replace into the database
		/// Before inserting, it encrypts all propertiese that have the Secure attribute. 
		/// </summary>
		/// <typeparam name="T">The Type of the object to be inserted</typeparam>
		/// <param name="obj"> the object to be inserted to the database</param>
		/// <param name="keySeed">The encryption key seed. You must use the same key seed when accessing the object out of the database.</param>
		/// <returns>no of affected rows</returns>
		int ISecureDatabase.SecureInsertOrReplace<T>(T obj, string keySeed)
		{
			Guard.CheckForNull(obj, "obj cannot be null");

			Encrypt(obj,keySeed);
			return base.InsertOrReplace(obj);
		}

		/// <summary>
		/// Updates a row in the database
		/// Before Before Updating, it encrypts all propertiese that have the Secure attribute. 
		/// </summary>
		/// <typeparam name="T">The Type of the object to be updated</typeparam>
		/// <param name="obj"> the object to be updated to the database</param>
		/// <param name="keySeed">The encryption key seed. You must use the same key seed when accessing the object out of the database.</param>
		/// <returns>no of affected rows</returns>
		int ISecureDatabase.SecureUpdate<T>(T obj, string keySeed)
		{
            Guard.CheckForNull(obj, "obj cannot be null");

            Encrypt(obj, keySeed);
			return base.Update(obj);
		}

		/// <summary>
		/// deletes a row in the database
		/// </summary>
		/// <typeparam name="T">The Type of the object to be deleted</typeparam>
		/// <param name="id">The id of the object to be deleted.</param>
		/// <returns>no of affected rows</returns>
		int ISecureDatabase.SecureDelete<T>(string id)
		{
			return base.ExecuteScalar<int>(string.Format("Delete from {0} where id = ? ", typeof (T).Name), id);
		}

		/// <summary>
		/// Gets an object of the database
		/// If the object is found, before returned, this method will decrypt all its properties that have the Secure attribute.
		/// </summary>
		/// <typeparam name="T">The Type of the object to be accessed</typeparam>
		/// <param name="id">The id of the object to be accessed.</param>
		/// <param name="keySeed">The encryption key seed (must be the same that you use when inserted into the database).</param>
		/// <returns>returns an instance of T if found.</returns>
		T ISecureDatabase.SecureGet<T>(string id, string keySeed)
		{
			var matching =  base.Query<T>(string.Format("select * from {0} where id = ? ", typeof (T).Name), id);
			var item = matching.FirstOrDefault();
			Decrypt(item, keySeed);
			return item;
		}

		/// <summary>
		/// Gets a list of T objects from the database
		/// If any objects were found, before returned, this method will decrypt all their properties that have the Secure attribute.
		/// </summary>
		/// <typeparam name="T">The Type of the object to be accessed</typeparam>
		/// <param name="keySeed">The encryption key seed (must be the same that you use when inserted into the database).</param>
		/// <returns>returns a List of T if found.</returns>
		List<T> ISecureDatabase.SecureGetAll<T>(string keySeed)
		{
			var list = base.Query<T>(string.Format("select * from {0}", typeof (T).Name));
			DecryptList(list, keySeed);
			return list;
		}

		/// <summary>
		/// Gets a count of all rows in the table that matches the type T
		/// </summary>
		/// <typeparam name="T">The type of the object we are trying to get the count for.</typeparam>
		/// <returns>int that represent the no of rows (of T) in the db. </returns>
		int ISecureDatabase.SecureGetCount<T>()
		{
			return base.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM {0} ", typeof(T).Name));
		}

		#region Implementation

		private void Encrypt(object model, string keySeed)
        {
            if (model == null) return;

			IEnumerable<PropertyInfo> secureProperties = GetSecureProperties(model);

            foreach (var propertyInfo in secureProperties)
            {
                var rawPropertyValue = (string)propertyInfo.GetValue(model);
                var encrypted = _cryptoService.Encrypt(rawPropertyValue, keySeed, null);
                propertyInfo.SetValue(model, encrypted);
            }
        }

        private void Decrypt(object model, string keySeed)
		{
            if (model == null) return;

            IEnumerable<PropertyInfo> secureProperties = GetSecureProperties(model);

            foreach (var propertyInfo in secureProperties)
			{
				var rawPropertyValue = (string)propertyInfo.GetValue(model);
				var decrypted = _cryptoService.Decrypt(rawPropertyValue, keySeed, null);
				propertyInfo.SetValue(model, decrypted);
			}
		}

        private static IEnumerable<PropertyInfo> GetSecureProperties(object model)
        {
            var type = model.GetType();

            var secureProperties = type.GetRuntimeProperties()
                            .Where(pi => pi.PropertyType == typeof(string) && pi.GetCustomAttributes<Secure>(true).Any());
            return secureProperties;
        }

        private void DecryptList<T>(List<T> list, string keySeed)
		{
			foreach (var item in list)
				Decrypt(item, keySeed);
		}

		#endregion
	}
}
