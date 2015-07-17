namespace SQLite.Net.Cipher.Model
{
	public class OperationResult
	{
		public OperationResult (bool successful, string message)
		{
			IsSuccessful = successful;
			Message = message;
		}

		public bool IsSuccessful {get;set;}
		public string Message {get;set;}

	}
}

