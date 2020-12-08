using DarkUI.Config;
using DarkUI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DarkUI.Docking
{
    public class DarkDockPanel : UserControl
    {
        #region Constructor Region

        public DarkDockPanel()
        {
            Splitters = new List<DarkDockSplitter>();
            DockContentDragFilter = new DockContentDragFilter(this);
            DockResizeFilter = new DockResizeFilter(this);

            Regions = new Dictionary<DarkDockArea, DarkDockRegion>();
            _contents = new List<DarkDockContent>();

            BackColor = Colors.GreyBackground;

            CreateRegions();
        }

        #endregion Constructor Region

        #region Event Region

        public event EventHandler<DockContentEventArgs> ActiveContentChanged;

        public event EventHandler<DockContentEventArgs> ContentAdded;

        public event EventHandler<DockContentEventArgs> ContentRemoved;

        #endregion Event Region

        #region Field Region

        private readonly List<DarkDockContent> _contents;

        private DarkDockContent _activeContent;
        private bool _switchingContent;

        #endregion Field Region

        #region Property Region

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DarkDockContent ActiveContent
        {
            get => _activeContent;
            set
            {
                // Don't let content visibility changes re-trigger event
                if (_switchingContent)
                    return;

                _switchingContent = true;

                _activeContent = value;

                ActiveGroup = _activeContent.DockGroup;
                ActiveRegion = ActiveGroup.DockRegion;

                foreach (var region in Regions.Values)
                    region.Redraw();

                if (ActiveContentChanged != null)
                    ActiveContentChanged(this, new DockContentEventArgs(_activeContent));

                _switchingContent = false;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DarkDockRegion ActiveRegion { get; internal set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DarkDockGroup ActiveGroup { get; internal set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DarkDockContent ActiveDocument => Regions[DarkDockArea.Document].ActiveDocument;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockContentDragFilter DockContentDragFilter { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockResizeFilter DockResizeFilter { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<DarkDockSplitter> Splitters { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MouseButtons MouseButtonState
        {
            get
            {
                var buttonState = MouseButtons;
                return buttonState;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<DarkDockArea, DarkDockRegion> Regions { get; }

        #endregion Property Region

        #region Method Region

        public void AddContent(DarkDockContent dockContent)
        {
            AddContent(dockContent, null);
        }

        public void AddContent(DarkDockContent dockContent, DarkDockGroup dockGroup)
        {
            if (_contents.Contains(dockContent))
                RemoveContent(dockContent);

            dockContent.DockPanel = this;
            _contents.Add(dockContent);

            if (dockGroup != null)
                dockContent.DockArea = dockGroup.DockArea;

            if (dockContent.DockArea == DarkDockArea.None)
                dockContent.DockArea = dockContent.DefaultDockArea;

            var region = Regions[dockContent.DockArea];
            region.AddContent(dockContent, dockGroup);

            if (ContentAdded != null)
                ContentAdded(this, new DockContentEventArgs(dockContent));

            dockContent.Select();
        }

        public void InsertContent(DarkDockContent dockContent, DarkDockGroup dockGroup, DockInsertType insertType)
        {
            if (_contents.Contains(dockContent))
                RemoveContent(dockContent);

            dockContent.DockPanel = this;
            _contents.Add(dockContent);

            dockContent.DockArea = dockGroup.DockArea;

            var region = Regions[dockGroup.DockArea];
            region.InsertContent(dockContent, dockGroup, insertType);

            if (ContentAdded != null)
                ContentAdded(this, new DockContentEventArgs(dockContent));

            dockContent.Select();
        }

        public void RemoveContent(DarkDockContent dockContent)
        {
            if (!_contents.Contains(dockContent))
                return;

            dockContent.DockPanel = null;
            _contents.Remove(dockContent);

            var region = Regions[dockContent.DockArea];
            region.RemoveContent(dockContent);

            if (ContentRemoved != null)
                ContentRemoved(this, new DockContentEventArgs(dockContent));
        }

        public bool ContainsContent(DarkDockContent dockContent)
        {
            return _contents.Contains(dockContent);
        }

        public List<DarkDockContent> GetDocuments()
        {
            return Regions[DarkDockArea.Document].GetContents();
        }

        private void CreateRegions()
        {
            var documentRegion = new DarkDockRegion(this, DarkDockArea.Document);
            Regions.Add(DarkDockArea.Document, documentRegion);

            var leftRegion = new DarkDockRegion(this, DarkDockArea.Left);
            Regions.Add(DarkDockArea.Left, leftRegion);

            var rightRegion = new DarkDockRegion(this, DarkDockArea.Right);
            Regions.Add(DarkDockArea.Right, rightRegion);

            var bottomRegion = new DarkDockRegion(this, DarkDockArea.Bottom);
            Regions.Add(DarkDockArea.Bottom, bottomRegion);

            // Add the regions in this order to force the bottom region to be positioned
            // between the left and right regions properly.
            Controls.Add(documentRegion);
            Controls.Add(bottomRegion);
            Controls.Add(leftRegion);
            Controls.Add(rightRegion);

            // Create tab index for intuitive tabbing order
            documentRegion.TabIndex = 0;
            rightRegion.TabIndex = 1;
            bottomRegion.TabIndex = 2;
            leftRegion.TabIndex = 3;
        }

        public void DragContent(DarkDockContent content)
        {
            DockContentDragFilter.StartDrag(content);
        }

        #endregion Method Region

        #region Serialization Region

        public DockPanelState GetDockPanelState()
        {
            var state = new DockPanelState();

            state.Regions.Add(new DockRegionState(DarkDockArea.Document));
            state.Regions.Add(new DockRegionState(DarkDockArea.Left, Regions[DarkDockArea.Left].Size));
            state.Regions.Add(new DockRegionState(DarkDockArea.Right, Regions[DarkDockArea.Right].Size));
            state.Regions.Add(new DockRegionState(DarkDockArea.Bottom, Regions[DarkDockArea.Bottom].Size));

            var _groupStates = new Dictionary<DarkDockGroup, DockGroupState>();

            var orderedContent = _contents.OrderBy(c => c.Order);
            foreach (var content in orderedContent)
                foreach (var region in state.Regions)
                    if (region.Area == content.DockArea)
                    {
                        DockGroupState groupState;

                        if (_groupStates.ContainsKey(content.DockGroup))
                        {
                            groupState = _groupStates[content.DockGroup];
                        }
                        else
                        {
                            groupState = new DockGroupState();
                            region.Groups.Add(groupState);
                            _groupStates.Add(content.DockGroup, groupState);
                        }

                        groupState.Contents.Add(content.SerializationKey);

                        groupState.VisibleContent = content.DockGroup.VisibleContent.SerializationKey;
                    }

            return state;
        }

        public void RestoreDockPanelState(DockPanelState state, Func<string, DarkDockContent> getContentBySerializationKey)
        {
            foreach (var region in state.Regions)
            {
                switch (region.Area)
                {
                    case DarkDockArea.Left:
                        Regions[DarkDockArea.Left].Size = region.Size;
                        break;

                    case DarkDockArea.Right:
                        Regions[DarkDockArea.Right].Size = region.Size;
                        break;

                    case DarkDockArea.Bottom:
                        Regions[DarkDockArea.Bottom].Size = region.Size;
                        break;
                }

                foreach (var group in region.Groups)
                {
                    DarkDockContent previousContent = null;
                    DarkDockContent visibleContent = null;

                    foreach (var contentKey in group.Contents)
                    {
                        var content = getContentBySerializationKey(contentKey);

                        if (content == null)
                            continue;

                        content.DockArea = region.Area;

                        if (previousContent == null)
                            AddContent(content);
                        else
                            AddContent(content, previousContent.DockGroup);

                        previousContent = content;

                        if (group.VisibleContent == contentKey)
                            visibleContent = content;
                    }

                    if (visibleContent != null)
                        visibleContent.Select();
                }
            }
        }

        #endregion Serialization Region
    }
}