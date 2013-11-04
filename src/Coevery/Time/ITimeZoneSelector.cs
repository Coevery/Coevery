using System.Web;

namespace Coevery.Time {
    public interface ITimeZoneSelector : IDependency {
        TimeZoneSelectorResult GetTimeZone(HttpContextBase context);
    }
}
