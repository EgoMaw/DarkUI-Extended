﻿using DarkUI.Config;
using DarkUI.Icons;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DarkUI.Controls
{
    // To make floating toolbox draggable with icon other than "Disabled", add this class' data handler
    // for OnDragEnter event on parent control which will setDragDropEffects to Move.

    [ToolboxItem(false)]
    public class DarkFloatingToolboxContainer
    {
        public DarkFloatingToolbox Toolbox;

        public DarkFloatingToolboxContainer(DarkFloatingToolbox toolbox)
        {
            Toolbox = toolbox;
        }
    }

    public class DarkFloatingToolbox : UserControl
    {
        private Size _dragSnappingMargin = new(10, 10);

        private int _gripMargin = 3;

        private int _gripSize = 12;

        private bool _verticalGrip = true;

        private Rectangle dragBounds;
        private Point dragOffset;
        private bool isDragging;
        private bool positionClamped;

        public DarkFloatingToolbox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            ClampDimensions();
        }

        [Category("Layout")]
        [Description("Determines snapping distance to parent control")]
        public Size SnappingMargin
        {
            get => _dragSnappingMargin;
            set => _dragSnappingMargin = value;
        }

        [Category("Layout")]
        [Description("Determines if control should anchor to nearest parent side")]
        public bool AutoAnchor { get; set; }

        [Category("Layout")]
        [Description("Determines if resize grip is horizontal or vertical.")]
        public bool VerticalGrip
        {
            get => _verticalGrip;
            set
            {
                _verticalGrip = value;
                ClampDimensions();
            }
        }

        [Category("Layout")]
        [Description("Determines resize grip size.")]
        public int GripSize
        {
            get => _gripSize;
            set
            {
                _gripSize = value;
                ClampDimensions();
            }
        }

        [Category("Layout")]
        [Description("Determines margin around grip.")]
        public int GripMargin
        {
            get => _gripMargin;
            set
            {
                _gripMargin = value;
                ClampDimensions();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding => base.Padding;

        public void FixPosition()
        {
            if (!DesignMode && Parent != null)
            {
                var X = Location.X;
                var Y = Location.Y;

                if (X > Parent.Width - Size.Width)
                    X = Parent.Width - Size.Width;
                else if (X < 0)
                    X = 0;

                if (Y > Parent.Height - Size.Height)
                    Y = Parent.Height - Size.Height;
                else if (Y < 0)
                    Y = 0;

                if (!positionClamped && (Location.X != X || Location.Y != Y))
                {
                    positionClamped = true;
                    Location = new Point(X, Y);
                }
                else
                {
                    positionClamped = false;
                }
            }

            SetAnchors();
            Refresh();
        }

        private void ClampDimensions()
        {
            if (_verticalGrip)
                base.Padding = new Padding(0, _gripSize + _gripMargin * 2, 0, 0);
            else
                base.Padding = new Padding(_gripSize + _gripMargin * 2, 0, 0, 0);
            Refresh();
        }

        private Rectangle GetGripBounds()
        {
            if (VerticalGrip)
                return new Rectangle(_gripMargin, _gripMargin, Width - _gripMargin * 2, GripSize);
            return new Rectangle(_gripMargin, _gripMargin, GripSize, Height - _gripMargin * 2);
        }

        private void SetAnchors()
        {
            if (AutoAnchor && Parent != null)
            {
                var newAnchors = AnchorStyles.None;

                if (Location.X + Width / 2 > Parent.Width / 2)
                    newAnchors |= AnchorStyles.Right;
                else
                    newAnchors |= AnchorStyles.Left;

                if (Location.Y + Height / 2 > +Parent.Height / 2)
                    newAnchors |= AnchorStyles.Bottom;
                else
                    newAnchors |= AnchorStyles.Top;

                Anchor = newAnchors;
            }
        }

        private void DragStart(Point offset)
        {
            dragBounds = Parent.ClientRectangle;
            dragBounds.Width -= Width;
            dragBounds.Height -= Height;

            dragOffset = offset;

            isDragging = true;
            DoDragDrop(new DarkFloatingToolboxContainer(this), DragDropEffects.Move);
            SetAnchors();
            isDragging = false;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTTRANSPARENT = -1;

            if (isDragging && m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)HTTRANSPARENT;
            else
                base.WndProc(ref m);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Absorb event
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            // Fill body
            using (var b = new SolidBrush(Colors.GreyBackground))
            {
                g.FillRectangle(b, ClientRectangle);
            }

            // Draw grip
            using (var brush = new TextureBrush(MenuIcons.grip_fill, WrapMode.Tile))
            {
                g.FillRectangle(brush, GetGripBounds());
            }
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            FixPosition();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && GetGripBounds().Contains(new Point(e.X, e.Y)))
                DragStart(e.Location);
        }

        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent)
        {
            base.OnQueryContinueDrag(qcdevent);

            if (Parent.RectangleToScreen(Parent.ClientRectangle).Contains(Cursor.Position))
            {
                var nextLocation = Parent.PointToClient(new Point(Cursor.Position.X - dragOffset.X, Cursor.Position.Y - dragOffset.Y));

                // Snap toolbox to parent border
                nextLocation.Offset(nextLocation.X < _dragSnappingMargin.Width ? -nextLocation.X : 0, nextLocation.Y < _dragSnappingMargin.Height ? -nextLocation.Y : 0);
                nextLocation.Offset(nextLocation.X > dragBounds.Width - _dragSnappingMargin.Width ? -(nextLocation.X - dragBounds.Width) : 0, nextLocation.Y > dragBounds.Height - _dragSnappingMargin.Height ? -(nextLocation.Y - dragBounds.Height) : 0);

                Location = nextLocation;
                Refresh(); // We need to invalidate all controls behind
            }
        }
    }
}