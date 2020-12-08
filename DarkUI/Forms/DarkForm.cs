using DarkUI.Config;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DarkUI.Forms
{
    public class DarkForm : Form
    {
        #region Field Region

        private bool _flatBorder;

        #endregion Field Region

        #region Constructor Region

        public DarkForm()
        {
            BackColor = Colors.GreyBackground;
        }

        #endregion Constructor Region

        #region Property Region

        [Category("Appearance")]
        [Description("Determines whether a single pixel border should be rendered around the form.")]
        [DefaultValue(false)]
        public bool FlatBorder
        {
            get => _flatBorder;
            set
            {
                _flatBorder = value;
                Invalidate();
            }
        }

        #endregion Property Region

        #region Paint Region

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (!_flatBorder)
                return;

            var g = e.Graphics;

            using var p = new Pen(Colors.DarkBorder);
            var modRect = new Rectangle(ClientRectangle.Location, new Size(ClientRectangle.Width - 1, ClientRectangle.Height - 1));
            g.DrawRectangle(p, modRect);
        }

        #endregion Paint Region
    }
}