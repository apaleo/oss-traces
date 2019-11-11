namespace Traces.Common.Enums
{
    public enum TraceStateEnum
    {
        Active = 0, // The task has been created, and is still due to be completed.
        Obsolete = 1, // The task has exceeded its due date/time.
        Completed = 2, // The task has been marked as completed.
    }
}