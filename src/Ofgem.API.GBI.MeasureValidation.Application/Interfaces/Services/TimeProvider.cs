namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services
{
    public abstract class TimeProvider
    {
        public abstract DateTimeOffset GetUtcNow();
        public abstract DateTimeOffset GetLocalNow();
        public abstract long GetTimestamp();
        public abstract TimeSpan GetElapsedTime(long startingTimestamp);
        public abstract TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp);
    }
}