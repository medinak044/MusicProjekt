namespace MP_API.Helpers;

// Populate a separate SQL table containing these string values
public class ProjectStatuses
{
    public enum ProjectStatusEnum
    {
        NotStarted = 0,
        Started = 1,
        OnHold = 2,
        Completed = 3,
        Canceled = 4,
        UnderRevision = 5,
    }
}
