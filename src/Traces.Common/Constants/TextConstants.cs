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

        public const string EditTraceModalTitle = "Update trace";

        public const string CreateTraceButtonText = "Create";

        public const string EditTraceButtonText = "Update";

        public const string UpcomingTracesText = "Upcoming tasks";

        public const string OverdueTracesTitleText = "Overdue";

        public const string TracesAddButtonText = "Add";

        public const string UpcomingTracesEmptyText =
            "There are currently no upcoming tasks. You can create new ones with the Add button.";

        public const string TracesDeleteButtonText = "Delete";

        public const string TracesTableTitleText = "Title";

        public const string TracesTableDescriptionText = "Description";

        public const string TracesTableDueDateText = "Due date";

        public const string TracesTablePropertyText = "Property";

        public const string TracesTableReservationText = "Reservation";

        public const string TracesLoadedUntilTextFormat = "Tasks loaded until {0}";

        public const string TracesLoadMoreButtonTextFormat = "Load tasks for the next {0} days";

        public const string TracesCompleteButtonText = "Mark as complete";

        public const string TracesEditButtonText = "Edit";

        public const string ApaleoSetupLoadingText = "Loading...";

        public const string ApaleoSetupLoadingTitle = "Setting things up...";

        public const string ApaleoSetupLoadingMessage =
            "We are getting everything ready for you. This will only take a few seconds...";

        public const string ApaleoSetupErrorTitle = "Oops... That should not have happened.";

        public const string ApaleoSetupErrorMessage =
            "We apologize but we experienced an issue while trying to create your integrations, we have already been notified and one of our lions is looking into it. You can wait for a minute and try to set them up again by clicking the button below.";

        public const string ApaleoSetupSuccessTitle = "All set up";

        public const string ApaleoSetupSuccessMessage =
            "Everything is ready for you to start using the app. To start you can go to the apaleo page by clicking the button below.";

        public const string ApaleoSetupButtonNavigateToApaleoText = "Go to apaleo app";

        public const string ApaleoSetupButtonTryAgainText = "Try again";

        public const string ApaleoOneMessageItemIncomplete =
            "The requested item is incommplete. Creating is not possible.";

        public const string FetchingDataFromApaleoForTracesErrorMessage =
            "There was an issue creating this trace. Unfortunately there was an issue fetching the data from the apaleo services.";

        public const string NoReservationIdProvidedErrorMessage =
            "There was no reservationId provided while trying to create a trace from the reservation flow";

        public const string DateIntervalErrorMessage =
            "The provided date interval is invalid. The end of the interval must be greater than the beginning";
    }
}