using System;
using System.Collections.Generic;

namespace SQLite.Net.Cipher.Interfaces
{
	public interface ISecureDatabase : IDisposable
	{

		/// <summary>
		/// Inserts into the database
		/// Before inserting, it encrypts all propertiese that have the Secure attribute. 
		/// </summary>
		/// <typeparam name="T">The Type of the object to be inserted</typeparam>
		/// <param name="obj"> the object to be inserted to the database</param>
		/// <param name="keySeed">The encryption key seed. You must use the same key seed when accessing the object out of the database.</param>
		/// <returns>no of affected rows</returns>
		int SecureInsert<T>(T obj, string keySeed) where T : class, IModel, new();

		/// <summary>
		/// Updates a row in the database
		/// Before Before Updating, it encrypts all propertiese that have the Secure attribute. 
		/// </summary>
		/// <typeparam name="T">The Type of the object to be updated</typeparam>
		/// <param name="obj"> the object to be updated to the database</param>
		/// <param name="keySeed">The encryption key seed. You must use the same key seed when accessing the object out of the database.</param>
		/// <returns>no of affected rows</returns>
		int SecureUpdate<T>(T obj, string keySeed) where T : class, IModel, new();

		/// <summary>
		/// deletes a row in the database
		/// </summary>
		/// <typeparam name="T">The Type of the object to be deleted</typeparam>
		/// <param name="id">The id of the object to be deleted.</param>
		/// <returns>no of affected rows</returns>
		int SecureDelete<T>(string id) where T : class, IModel, new();

		/// <summary>
		/// Gets an object of the database
		/// If the object is found, before returned, this method will decrypt all its properties that have the Secure attribute.
		/// </summary>
		/// <typeparam name="T">The Type of the object to be accessed</typeparam>
		/// <param name="id">The id of the object to be accessed.</param>
		/// <param name="keySeed">The encryption key seed (must be the same that you use when inserted into the database).</param>
		/// <returns>returns an instance of T if found.</returns>
		T SecureGet<T>(string id, string keySeed) where T : class, IModel, new();

		/// <summary>
		/// Gets a list of T objects from the database
		/// If any objects were found, before returned, this method will decrypt all their properties that have the Secure attribute.
		/// </summary>
		/// <typeparam name="T">The Type of the object to be accessed</typeparam>
		/// <param name="keySeed">The encryption key seed (must be the same that you use when inserted into the database).</param>
		/// <returns>returns a List of T if found.</returns>
		List<T> SecureGetAll<T>(string keySeed) where T : class, IModel, new();

		/// <summary>
		/// Gets a count of all rows in the table that matches the type T
		/// </summary>
		/// <typeparam name="T">The type of the object we are trying to get the count for.</typeparam>
		/// <returns>int that represent the no of rows (of T) in the db. </returns>
		int SecureGetCount<T>() where T : class, IModel, new();

		/// <summary>
		/// Executes an sql query against the database. 
		/// This method does not do anything more than the base.ExecuteScalar();
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		/// <returns>no of affected rows</returns>
		int SecureExecuteScalar(string query, params object[] args);

		/// <summary>
		/// gets a list of objects from the database using Query() method 
		/// and it decrypt all object properties that have the attribute Secure. 
		/// </summary>
		/// <typeparam name="T">The type of the object</typeparam>
		/// <param name="query">The Sql query</param>
		/// <param name="keySeed">The encryption key seed (must be the same that you use when inserted into the database).</param>
		/// <param name="args">The sql query parameters.</param>
		/// <returns>List of T </returns>
		List<T> SecureQuery<T>(string query, string keySeed, params object[] args) where T : class, IModel, new();
	}
}

