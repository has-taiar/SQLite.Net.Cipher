namespace SQLite.Net.Cipher.Interfaces
{
	/// <summary>
	/// IModel interface
	/// All objects that need to be stored in the database need to implement this interface. 
	/// The only requirement it has is to provide an Id property. 
	/// </summary>
	public interface IModel
	{
		/// <summary>
		/// Id property that represent the primary key of the object.
		/// </summary>
		string Id { get; set; }
	}
}
