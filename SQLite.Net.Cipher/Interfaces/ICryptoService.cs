namespace SQLite.Net.Cipher.Interfaces
{
	public interface ICryptoService
	{
		 string Encrypt(string data, string key, string iv);
		 string Decrypt(string encryptedData, string key, string iv);
		 string ComputeHash(string input);
		 string GenerateRandomKey(int length);
	}
}

