namespace Traces.Common.Enums
{
    public enum TraceState
    {
        Active = 0, // The task has been created, and is still due to be completed.
        Completed = 2, // The task has been marked as completed. It is 2 for a reason, there used to be an extra state in between.
    }
}