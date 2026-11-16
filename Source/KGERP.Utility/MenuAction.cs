namespace KGERP.Utility
{
    /// <summary>
    /// Actions that can be permitted per menu per user.
    /// Maps to CompanyUserMenu columns: IsView, IsAdd, IsUpdate, IsDelete.
    /// </summary>
    public enum MenuAction
    {
        View = 1,
        Add = 2,
        Edit = 3,   // maps to the IsUpdate column
        Delete = 4
    }
}

