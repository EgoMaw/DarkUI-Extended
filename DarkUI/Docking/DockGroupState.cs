using System.Collections.Generic;

namespace DarkUI.Docking
{
    public class DockGroupState
    {
        #region Constructor Region

        public DockGroupState()
        {
            Contents = new List<string>();
        }

        #endregion Constructor Region

        #region Property Region

        public List<string> Contents { get; set; }

        public string VisibleContent { get; set; }

        #endregion Property Region
    }
}