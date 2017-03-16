using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[Serializable]
	public class SymbolNotFoundException : ArgumentException
	{
		internal string message = null;
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public SymbolNotFoundException(string message)
		{
			this.message = message;
		}
	}
}
