using System.Drawing;
using System.Windows.Forms;

namespace DarkUI.Forms
{
    internal class DarkTranslucentForm : Form
    {
        #region Constructor Region

        public DarkTranslucentForm(Color backColor, double opacity = 0.6)
        {
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(1, 1);
            ShowInTaskbar = false;
            AllowTransparency = true;
            Opacity = opacity;
            BackColor = backColor;
        }

        #endregion Constructor Region

        #region Property Region

        protected override bool ShowWithoutActivation => true;

        #endregion Property Region
    }
}