namespace Traces.Common.Constants
{
    public static class TextConstants
    {
        public const string CreateTraceWithoutTitleOrFutureDateErrorMessage =
            "The trace must have a title and a due date in the future to be created.";

        public const string UpdateTraceWithoutTitleOrFutureDateErrorMessageFormat =
            "Trace with id {0} cannot be updated, the replacement must have a title and a due date in the future.";

        public const string TraceCouldNotBeFoundErrorMessageFormat = "The trace with id {0} could not be found.";

        public const string TraceMarkedAsCompletedMessage = "Trace marked as completed successfully.";

        public const string TraceCreatedSuccessfullyMessage = "Trace created successfully.";

        public const string TraceUpdatedSuccessfullyMessage = "Trace updated successfully.";

        public const string TraceDeletedSuccessfullyMessage = "Trace deleted successfully.";

        public const string SuccessHeaderText = "Success";

        public const string ErrorHeaderText = "Oops";

        public const string CreateTraceModalTitle = "Create trace";

        public const string ReplaceTraceModalTitle = "Update trace";

        public const string CreateTraceButtonText = "Create";

        public const string ReplaceTraceButtonText = "Update";
    }
}