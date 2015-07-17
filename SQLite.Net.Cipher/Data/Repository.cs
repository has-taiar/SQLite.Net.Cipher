//using System.Collections.Generic;
//using System.Linq;
//using SQLite.Net.Cipher.Interfaces;
//
//// library limitation:
////1. Protected properties must be of type string
////2. Only supports UTF8 encoding 
////3. Limited support to Query<> method, use CryptoService along with Query<> to construct your own query
//
//
//
//namespace SQLite.Net.Cipher.Data
//{
//    public class Repository<T> where T : class, IModel, new()
//    {
//		protected readonly ISecureDatabase SecureSqliteDatabase;
//
//		public Repository (ISecureDatabase secureDatabase)
//		{
//			SecureSqliteDatabase = secureDatabase;
//		}
//
//		public int GetCount ()
//		{
//			var sql = string.Format ("select count(*) from {0}", typeof(T).Name);
//			return SecureSqliteDatabase.ExecuteScalar (sql);
//		}
//
//		public List<T> GetAll (string keySeed)
//		{
//			var query = string.Format ("select * from {0}", typeof(T).Name);
//			return SecureSqliteDatabase.Query<T> (query, keySeed);
//		}
//
//		public bool Insert(T model, string keySeed)
//		{
//			if (!Exists (model.Id, keySeed))
//			{
//				return SecureSqliteDatabase.Insert (model, keySeed) == 1;
//			}
//			return false;
//		}
//
//		public int Update(T model, string keySeed)
//		{
//			return SecureSqliteDatabase.Update (model, keySeed);
//		}
//
//		public int Delete (T model)
//		{
//			var query = string.Format ("Delete from {0} where id = ? ", typeof(T).Name);
//			var result = SecureSqliteDatabase.ExecuteScalar(query, model.Id);
//			return result;
//		}
//
//		public T GetById(string id, string keySeed)
//		{
//			var query = string.Format ("select * from {0} where id = ? ", typeof(T).Name);
//			var results = SecureSqliteDatabase.Query<T>(query, keySeed, id);
//			return results.FirstOrDefault ();
//		}
//
//		public virtual bool Exists(string id, string keySeed)
//        {
//	        return GetById(id, keySeed) != null;
//        }
//	}
//}
