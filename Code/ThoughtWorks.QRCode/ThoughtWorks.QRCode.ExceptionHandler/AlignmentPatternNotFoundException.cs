using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	[Serializable]
	public class AlignmentPatternNotFoundException : ArgumentException
	{
		internal string message = null;
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		public AlignmentPatternNotFoundException(string message)
		{
			this.message = message;
		}
	}
}
