using System;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class Multi : Control, MouseListener, Hitable {

        private readonly MouseAdapter ma = new MouseAdapter();

        public Multi() {
            ma.From = this;
            ma.To = this;
        }

        public void Hit() {
            ((MultiWindow)Parent).Hit(this);
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
            using(Pen p = new Pen(F.BorderColor, F.BorderWidth))
                e.Graphics.DrawRectangle(p, 1, 1, Width - 2, Height - 2);
        }

        public void OnMyEnter(object s, EventArgs e) {
        }

        public void OnMyLeave(object s, EventArgs e) {
        }

        public void OnMyClick(object s, MouseEventArgs e) {
            Hit();
        }

        public void OnMyDrag(object s, MouseEventArgs e) {
        }

        public void OnMyDrop(object s, MouseEventArgs e) {
        }
    }
}
