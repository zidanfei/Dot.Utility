using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[Serializable]
	public class InvalidDataBlockException : ArgumentException
	{
		internal string message = null;
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public InvalidDataBlockException(string message)
		{
			this.message = message;
		}
	}
}
