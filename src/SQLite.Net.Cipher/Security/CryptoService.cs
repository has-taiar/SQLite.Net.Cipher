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

		/// <summary>
		/// Compute a SHA256 hash value of a string
		/// </summary>
		/// <param name="input"></param>
		/// <returns>SHA256 value of the input string</returns>
		public string ComputeHash (string input)
		{
			var hasher = PCLCrypto.WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
	        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
	        byte[] hash = hasher.HashData(inputBytes);
	        var hashedAsString = Convert.ToBase64String(hash);

	        return hashedAsString;
		}

		/// <summary>
		/// Generate a random key
		/// </summary>
		/// <param name="length">Length of the required random key</param>
		/// <returns>random key</returns>
		public string GenerateRandomKey(int length)
		{
			byte[] buffer = PCLCrypto.WinRTCrypto.CryptographicBuffer.GenerateRandom((uint)length);
			var key = Convert.ToBase64String(buffer);
			return key;
		}

		/// <summary>
		/// Encrypts the input dataText using AES algorithm
		/// </summary>
		/// <param name="dataText">the input text to be encrypted</param>
		/// <param name="keyText">the encryption key seed. This can be any text, with any length. </param>
		/// <param name="ivText">The vector value, or null</param>
		/// <returns>a string that represents the base64 of the encrypted input text</returns>
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

		/// <summary>
		/// Decrypts the input dataText using AES algorithm
		/// </summary>
		/// <param name="dataText">the input text (base64) to be decrypted</param>
		/// <param name="keyText">the encryption key seed. This must be the same as the key that was used for encrypting the text. </param>
		/// <param name="ivText">The vector value, or null</param>
		/// <returns>the original text, the result of the decryption.</returns>
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

