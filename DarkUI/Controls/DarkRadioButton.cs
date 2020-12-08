using DarkUI.Config;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DarkUI.Controls
{
    public class DarkRadioButton : RadioButton
    {
        #region Constructor Region

        public DarkRadioButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
        }

        #endregion Constructor Region

        #region Method Region

        private void SetControlState(DarkControlState controlState)
        {
            if (_controlState != controlState)
            {
                _controlState = controlState;
                Invalidate();
            }
        }

        #endregion Method Region

        #region Paint Region

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var rect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            var size = Consts.RadioButtonSize;

            var textColor = Colors.LightText;
            var borderColor = Colors.LightText;
            var fillColor = Colors.LightestBackground;

            if (Enabled)
            {
                if (Focused)
                {
                    borderColor = Colors.BlueHighlight;
                    fillColor = Colors.BlueSelection;
                }

                if (_controlState == DarkControlState.Hover)
                {
                    borderColor = Colors.BlueHighlight;
                    fillColor = Colors.BlueSelection;
                }
                else if (_controlState == DarkControlState.Pressed)
                {
                    borderColor = Colors.GreyHighlight;
                    fillColor = Colors.GreySelection;
                }
            }
            else
            {
                textColor = Colors.DisabledText;
                borderColor = Colors.GreyHighlight;
                fillColor = Colors.GreySelection;
            }

            using (var b = new SolidBrush(Colors.GreyBackground))
            {
                g.FillRectangle(b, rect);
            }

            g.SmoothingMode = SmoothingMode.HighQuality;

            using (var p = new Pen(borderColor))
            {
                var boxRect = new Rectangle(0, rect.Height / 2 - size / 2, size, size);
                g.DrawEllipse(p, boxRect);
            }

            if (Checked)
            {
                using var b = new SolidBrush(fillColor);
                var boxRect = new Rectangle(3, rect.Height / 2 - (size - 7) / 2 - 1, size - 6, size - 6);
                g.FillEllipse(b, boxRect);
            }

            g.SmoothingMode = SmoothingMode.Default;

            using (var b = new SolidBrush(textColor))
            {
                var stringFormat = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near
                };

                var modRect = new Rectangle(size + 4, 0, rect.Width - size, rect.Height);
                g.DrawString(Text, Font, b, modRect, stringFormat);
            }
        }

        #endregion Paint Region

        #region Field Region

        private DarkControlState _controlState = DarkControlState.Normal;

        private bool _spacePressed;

        #endregion Field Region

        #region Property Region

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Appearance Appearance => base.Appearance;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool AutoEllipsis => base.AutoEllipsis;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image BackgroundImage => base.BackgroundImage;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageLayout BackgroundImageLayout => base.BackgroundImageLayout;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool FlatAppearance => false;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FlatStyle FlatStyle => base.FlatStyle;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image Image => base.Image;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ContentAlignment ImageAlign => base.ImageAlign;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int ImageIndex => base.ImageIndex;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string ImageKey => base.ImageKey;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageList ImageList => base.ImageList;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ContentAlignment TextAlign => base.TextAlign;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TextImageRelation TextImageRelation => base.TextImageRelation;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool UseCompatibleTextRendering => false;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool UseVisualStyleBackColor => false;

        #endregion Property Region

        #region Event Handler Region

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_spacePressed)
                return;

            if (e.Button == MouseButtons.Left)
            {
                if (ClientRectangle.Contains(e.Location))
                    SetControlState(DarkControlState.Pressed);
                else
                    SetControlState(DarkControlState.Hover);
            }
            else
            {
                SetControlState(DarkControlState.Hover);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!ClientRectangle.Contains(e.Location))
                return;

            SetControlState(DarkControlState.Pressed);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_spacePressed)
                return;

            SetControlState(DarkControlState.Normal);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_spacePressed)
                return;

            SetControlState(DarkControlState.Normal);
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);

            if (_spacePressed)
                return;

            var location = Cursor.Position;

            if (!ClientRectangle.Contains(location))
                SetControlState(DarkControlState.Normal);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            _spacePressed = false;

            var location = Cursor.Position;

            if (!ClientRectangle.Contains(location))
                SetControlState(DarkControlState.Normal);
            else
                SetControlState(DarkControlState.Hover);
        }

        #endregion Event Handler Region
    }
}