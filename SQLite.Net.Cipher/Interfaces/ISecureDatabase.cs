using System;
using System.Collections.Generic;

namespace SQLite.Net.Cipher.Interfaces
{
	public interface ISecureDatabase : IDisposable
	{
		int SecureInsert<T>(T obj, string keySeed) where T : class, IModel, new();
		int SecureUpdate<T>(T obj, string keySeed) where T : class, IModel, new();
		int SecureDelete<T>(string id) where T : class, IModel, new();
		T SecureGet<T>(string id, string keySeed) where T : class, IModel, new();
		List<T> SecureGetAll<T>(string keySeed) where T : class, IModel, new();
		int SecureGetCount<T>() where T : class, IModel, new();
		int SecureExecuteScalar(string query, params object[] args);
		List<T> SecureQuery<T>(string query, string keySeed, params object[] args) where T : class, IModel, new();
	}
}

