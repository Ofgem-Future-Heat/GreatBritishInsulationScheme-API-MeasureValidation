using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;

namespace Ofgem.API.GBI.MeasureValidation.Application.Common
{
    public class TimeProviderImplementation : TimeProvider
    {   
        public TimeProviderImplementation () { }
        public TimeZoneInfo LocalTimeZone => throw new NotImplementedException();

        public long TimestampFrequency => throw new NotImplementedException();

        public override TimeSpan GetElapsedTime(long startingTimestamp)
        {
            throw new NotImplementedException();
        }

        public override TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp)
        {
            throw new NotImplementedException();
        }

        public override DateTimeOffset GetLocalNow()
        {
            return DateTimeOffset.Now;
        }

        public override long GetTimestamp()
        {
            throw new NotImplementedException();
        }

        public override DateTimeOffset GetUtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
