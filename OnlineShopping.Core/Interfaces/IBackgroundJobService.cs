namespace OnlineShopping.Core.Interfaces;

public interface IBackgroundJobService
{
    void ScheduleMonthlyReports();
    Task SendMonthlyReportsJob();
}