using System;
using DT = System.DateTime;

namespace FindingDateTimeNow.IntegrationTests
{
    public sealed class FindingDateTimeNowTests
    {
        public void DoSomething()
        {
            // These should all fail.
            var dt1 = new DateTime(10000, DateTimeKind.Local);
            var dt2 = new DateTime(10000, DateTimeKind.Unspecified);
            var dt3 = new DT(10000, DateTimeKind.Local);
            var dt4 = new DT(10000, DateTimeKind.Unspecified);
            var dt5 = new DateTime(1000);
            var dt6 = new DT(1000);
            var dt7 = DateTime.Now;
            var dt8 = DT.Now;

            // These should be fine.
            var dt9 = new DateTime(10000, DateTimeKind.Utc);
            var dt10 = new DT(10000, DateTimeKind.Utc);
            var dt11 = DateTime.UtcNow;
            var dt12 = DT.UtcNow;
            var dt13 = this.Now;
        }

        public string Now { get; set; }
    }
}
