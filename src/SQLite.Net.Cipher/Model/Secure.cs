using System;

namespace SQLite.Net.Cipher.Model
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class Secure : Attribute
	{
	}
}
