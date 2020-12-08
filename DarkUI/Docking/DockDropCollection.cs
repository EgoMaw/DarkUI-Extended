namespace DarkUI.Docking
{
    internal class DockDropCollection
    {
        #region Constructor Region

        internal DockDropCollection(DarkDockPanel dockPanel, DarkDockGroup group)
        {
            DropArea = new DockDropArea(dockPanel, group, DockInsertType.None);
            InsertBeforeArea = new DockDropArea(dockPanel, group, DockInsertType.Before);
            InsertAfterArea = new DockDropArea(dockPanel, group, DockInsertType.After);
        }

        #endregion Constructor Region

        #region Property Region

        internal DockDropArea DropArea { get; }

        internal DockDropArea InsertBeforeArea { get; }

        internal DockDropArea InsertAfterArea { get; }

        #endregion Property Region
    }
}