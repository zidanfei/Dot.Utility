using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[Serializable]
	public class FinderPatternNotFoundException : Exception
	{
		internal string message = null;
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public FinderPatternNotFoundException(string message)
		{
			this.message = message;
		}
	}
}
