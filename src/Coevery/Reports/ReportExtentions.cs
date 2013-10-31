using Coevery.Reports;
using Coevery.Reports.Services;

public static class ReportExtentions {
    public static void Information(this IReportsCoordinator reportCoordinator, string reportKey, string message) {
        reportCoordinator.Add(reportKey, ReportEntryType.Information, message);
    }
    public static void Warning(this IReportsCoordinator reportCoordinator, string reportKey, string message) {
        reportCoordinator.Add(reportKey, ReportEntryType.Warning, message);
    }
    public static void Error(this IReportsCoordinator reportCoordinator, string reportKey, string message) {
        reportCoordinator.Add(reportKey, ReportEntryType.Error, message);
    }
}

