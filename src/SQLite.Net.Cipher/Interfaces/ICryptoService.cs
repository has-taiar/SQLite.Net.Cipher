namespace SQLite.Net.Cipher.Interfaces
{
	/// <summary>
	/// An interface of the crypto service. 
	/// The SQLite.Net.Cipher uses this service for all encryption/decryption tasks. 
	/// </summary>
	public interface ICryptoService
	{
		 string Encrypt(string data, string key, string iv);
		 string Decrypt(string encryptedData, string key, string iv);
		 string ComputeHash(string input);
		 string GenerateRandomKey(int length);
	}
}

