using Hangfire;
using Hangfire.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksLibrary.Hangfire
{
    public static class HangfireInitializeJobs
    {
        public static void Initialize()
        {
            JobStorage.Current = new SqlServerStorage("HangFireConntectionString");

            RecurringJob.AddOrUpdate<IHangfireService>(s => s.DeleteReservationAfterExpiredTime(), Cron.Hourly);

            RecurringJob.AddOrUpdate<IHangfireService>(s => s.LockUserAccountIfTimeToReturnBookExpired(), Cron.Hourly);

            RecurringJob.AddOrUpdate<IHangfireService>(s => s.SendReminderAboutEndingTimeToReturnBook(), Cron.Hourly);
        }
    }
}