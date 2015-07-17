//using System.Linq;
//using SQLite.Net.Cipher.Interfaces;
//using SQLite.Net.Cipher.Model;

//namespace SQLite.Net.Cipher.Data
//{
//	public class UserRepository : IUserRepository
//	{
//		ISecureDbConnectionGenerator _connectionGenerator;

//		public UserRepository (ISecureDbConnectionGenerator connectionGenerator)
//		{
//			_connectionGenerator = connectionGenerator;
//		}

//		public virtual User GetUser (string key)
//		{
//			using (var conn = _connectionGenerator.Generate(key))
//			{
//				var query = string.Format ("select * from {0}", typeof(User).Name);
//				var results = conn.Query<User> (query);
//				return results.FirstOrDefault ();
//			}
//		}

//		public virtual OperationResult Save(string key, User user)
//		{
//			using (var conn = _connectionGenerator.Generate(key))
//			{
//				var query = string.Format("DELETE from {0} ", typeof(User).Name);
//				int affected = conn.ExecuteScalar(query);

//				var isSaved = conn.Insert(user) > 0;
//				return isSaved ? new OperationResult(true, string.Empty)
//					: new OperationResult(false, Constants.ErrorMessages.SavingFailed);
//			}
//		}

//		protected void DeleteExistingUserRecords (string key)
//		{
//			using (var conn = _connectionGenerator.Generate(key))
//			{
//				var query = string.Format("DELETE from {0} ", typeof(User).Name);
//				int affected = conn.ExecuteScalar(query);
//			}
//		}

//		public bool RemoveDb()
//		{
//			return _connectionGenerator.Reset();
//		}
//	}
//}

