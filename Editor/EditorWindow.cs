using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class EditorWindow : FlowLayoutPanel, Modifiable {

        public EditorWindow() {
            BackColor = F.BackColor;
            Margin = new Padding();

            Panel p = new Panel {
                AutoSize = true,
                Margin = new Padding(),
            };
            Controls.Add(p);

            p.Controls.Add(F.Editor.Root);
        }

        public void Modify() {
            F.Editor.Root.MyResize(F.Editor.Size);
            Size = F.Editor.WindowSize(this);
            Padding = new Padding((F.WindowWidth(this) - F.Editor.Width) / 2, 0, 0, 0);
        }

        public void Modify(Control c) {
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
            using(SolidBrush sb = new SolidBrush(F.BorderColor)) {
                e.Graphics.FillRectangles(sb, new Rectangle[] {
                    new Rectangle(Controls[0].Left - F.BorderWidth, F.BorderWidth, F.BorderWidth, Height - F.BorderWidth * 2),
                    new Rectangle(Controls[0].Right, F.BorderWidth, F.BorderWidth, Height - F.BorderWidth * 2)
                });
            }
        }
    }
}
