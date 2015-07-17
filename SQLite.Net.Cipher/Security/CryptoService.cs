using System;
using System.Text;
using PCLCrypto;
using SQLite.Net.Cipher.Interfaces;

namespace SQLite.Net.Cipher.Security
{
	public class CryptoService : ICryptoService
	{
		public CryptoService ()
		{
		}

		#region ICryptoService implementation

		public string ComputeHash (string input)
		{
			var hasher = PCLCrypto.WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
	        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
	        byte[] hash = hasher.HashData(inputBytes);
	        var hashedAsString = Convert.ToBase64String(hash);

	        return hashedAsString;
		}

		public string GenerateRandomKey(int length)
		{
			byte[] buffer = PCLCrypto.WinRTCrypto.CryptographicBuffer.GenerateRandom((uint)length);
			var key = Convert.ToBase64String(buffer);
			return key;
		}

		#endregion

		public string Encrypt(string dataText, string keyText, string ivText)
		{
			byte[] data = Encoding.UTF8.GetBytes(dataText);
			byte[] iv = string.IsNullOrEmpty(ivText) ?  null : Encoding.UTF8.GetBytes(ivText); 

			var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);

			var keyMaterial = CreateKeyMaterial(keyText, SaltText, EncryptionKeyLength, Iterations);
			var key = provider.CreateSymmetricKey(keyMaterial);

			byte[] cipherText = WinRTCrypto.CryptographicEngine.Encrypt(key, data, iv);

			var encryptedText = Convert.ToBase64String(cipherText);

			return encryptedText;
		}

		public string Decrypt(string dataText, string keyText, string ivText)
		{
			byte[] data = Convert.FromBase64String(dataText);
			byte[] iv = string.IsNullOrEmpty(ivText) ? null : Encoding.UTF8.GetBytes(ivText);

			var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);

			var keyMaterial = CreateKeyMaterial(keyText, SaltText, EncryptionKeyLength, Iterations);
			var key = provider.CreateSymmetricKey(keyMaterial);

			byte[] plainText = WinRTCrypto.CryptographicEngine.Decrypt(key, data, iv);
			
			var decrypted = Encoding.UTF8.GetString(plainText, 0, plainText.Length);

			return decrypted;
		}

		byte[] CreateKeyMaterial(string keySeed, string saltText, int keyLengthInBytes = 16, int iterations = 5000)
		{
			byte[] salt = Encoding.UTF8.GetBytes(saltText);
			byte[] key = NetFxCrypto.DeriveBytes.GetBytes(keySeed, salt, iterations, keyLengthInBytes);
			return key;
		}

		private const string SaltText = "MY-TEMP-SALT";
		private const int Iterations = 5000;
		private const int EncryptionKeyLength = 16;
	}
}

