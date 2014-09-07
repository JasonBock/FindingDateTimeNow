namespace FindingDateTimeNow
{
	public static class FindingDateTimeNowConstants
	{
		public const string Description = "Find Usage of DateTime.Now";
		public const string CodeFixDescription = "Change Now to UtcNow.";
		public const string DiagnosticId = "FindingDateTimeNow";
		public const string Message = "Do not use DateTime.Now";
	}
}
