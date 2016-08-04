
using System;

// Implement this and clone the object from memory before encrypting and saving to database
// or the object in memory will be encrypted too
namespace SQLite.Net.Cipher.Interfaces
{
	public interface ICloneable
	{
		Object Clone();
	}
}

