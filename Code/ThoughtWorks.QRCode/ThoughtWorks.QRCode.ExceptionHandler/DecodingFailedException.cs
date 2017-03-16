using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[Serializable]
	public class DecodingFailedException : ArgumentException
	{
		internal string message = null;
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public DecodingFailedException(string message)
		{
			this.message = message;
		}
	}
}
