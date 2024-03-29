﻿using DarkUI.Config;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DarkUI.Controls
{
    public enum DarkProgressBarMode
    {
        NoText,
        Percentage,
        XOfN
    }

    public class DarkProgressBar : ProgressBar
    {
        private static readonly StringFormat _textAlignment = new()
        { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        private DarkProgressBarMode _textMode = DarkProgressBarMode.Percentage;

        public DarkProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        [Category("Appearance")]
        [DefaultValue(DarkProgressBarMode.Percentage)]
        public DarkProgressBarMode TextMode
        {
            get => _textMode;
            set
            {
                if (value == _textMode)
                    return;
                _textMode = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        public new Font Font
        {
            get => base.Font;
            set => base.Font = value;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Prevent progress bar flickering
                return cp;
            }
        }

        // Paint does not work for progressbars for some reason.
        // This is a workaround:
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x000F)
            {
                var percentage = (Value - Minimum) / (float)(Maximum - Minimum);

                using var g = CreateGraphics();
                var rect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

                g.Clear(Colors.MediumBackground);

                using (var b = new SolidBrush(Colors.LighterBackground))
                {
                    g.FillRectangle(b, new Rectangle(rect.Left + 2, rect.Top + 2, (int)((rect.Width - 4) * percentage), rect.Height - 4));
                }

                using (var p = new Pen(Colors.GreySelection))
                {
                    g.DrawRectangle(p, new Rectangle(rect.Left, rect.Top, rect.Width - 1, rect.Height - 1));
                }

                if (_textMode != DarkProgressBarMode.NoText)
                {
                    using var b = new SolidBrush(Colors.LightText);
                    switch (_textMode)
                    {
                        case DarkProgressBarMode.Percentage:
                            g.DrawString(float.IsNaN(percentage) ? "N/A" : Math.Round(percentage * 100) + "%", Font, b, ClientRectangle, _textAlignment);
                            break;

                        case DarkProgressBarMode.XOfN:
                            g.DrawString((Minimum == 0 ? Value + 1 : Value) + " / " + (Minimum == 0 ? Maximum + 1 : Maximum), Font, b, ClientRectangle, _textAlignment);
                            break;

                        default:
                            throw new NotImplementedException("Text mode: " + _textMode);
                    }
                }
            }
        }
    }
}