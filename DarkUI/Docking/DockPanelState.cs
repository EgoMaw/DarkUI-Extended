using System.Collections.Generic;

namespace DarkUI.Docking
{
    public class DockPanelState
    {
        #region Constructor Region

        public DockPanelState()
        {
            Regions = new List<DockRegionState>();
        }

        #endregion Constructor Region

        #region Property Region

        public List<DockRegionState> Regions { get; set; }

        #endregion Property Region
    }
}