namespace FindingDateTimeNow
{
	public static class FindingNewDateTimeConstants
	{
		public const string Description = "Find Usage of DateTime Constructor Without Utc as the DateTimeKind";
		public const string CodeFixDescription = "Change Constructor to DateTimeKind.Utc.";
		public const string DiagnosticId = "FindingNewDateTime";
		public const string FindingDateTimeNowMessage = "Do not use DateTimeKind.Local or DateTimeKind.Unspecified";
		public const string UnspecifiedKindMessage = "You must use a DateTime constuctor that takes a DateTimeKind";
	}
}
