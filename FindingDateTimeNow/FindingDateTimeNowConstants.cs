namespace FindingDateTimeNow
{
	public static class FindingDateTimeNowConstants
	{
		public const string Category = "Usage";
		public const string CodeFixDescription = "Change Now to UtcNow.";
		public const string DiagnosticId = "FindingDateTimeNow";
		public const string Message = "Do not use DateTime.Now";
		public const string Title = "Find Usage of DateTime.Now";
	}
}
