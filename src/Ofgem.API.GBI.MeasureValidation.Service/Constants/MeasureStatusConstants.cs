namespace Ofgem.API.GBI.MeasureValidation.Service.Constants
{
    public static class MeasureStatusConstants
    {
        public const int NotifiedIncomplete = 1;
        public const int MeasureAwaitingVerification = 2;
        public const int NotifiedPending = 3;
        public const int OnHold = 4;
        public const int BeingAssessed = 5;
        public const int WithSupplier = 6;
        public const int InternalQuery = 7;
        public const int Approved = 8;
        public const int Rejected = 9;
    }
}
