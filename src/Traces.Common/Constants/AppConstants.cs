namespace Traces.Common.Constants
{
    public static class AppConstants
    {
        public const string AccountLevelUrlAbsolutePath = "/traces/account";

        public const string ReservationLevelUrlAbsolutePath = "/traces/reservation";

        public const string PropertyLevelUrlAbsolutePath = "/traces/property";

        public const string LogoutUrlAbsolutePath = "/logout";

        public const string SubjectIdQueryParameter = "subjectId";

        public const string AccountCodeQueryParameter = "accountCode";

        public const string ReservationIdQueryParameter = "reservationId";

        public const string PropertyIdQueryParameter = "propertyId";

        public const string IntegrationSourceType = "Public";

        public const long MaxFileSizeInBytes = 5 * 1024 * 1024;
    }
}