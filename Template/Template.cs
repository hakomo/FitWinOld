using System;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class Template : SplitPanel, Hitable {

        public Template(Split s, MouseAdapter ma, MouseAdapter mb) {
            int i;
            ma.From = this;
            mb.From = this;
            BackColor = F.BackColor;
            FlowDirection = s.F;
            Margin = new Padding();
            Ratio = s.R;
            if(s.S == null)
                return;
            for(i = 0; i < s.S.Length; ++i) {
                Controls.Add(new Template(s.S[i], ma, mb));
                if(i < s.S.Length - 1)
                    Controls.Add(new TemplateBorder(ma, mb) { FlowDirection = FlowDirection });
            }
        }

        public Template(SplitPanel sp, MouseAdapter ma, MouseAdapter mb)
            : this(new Split(sp), ma, mb) {
        }

        public void RemoveFrom(MouseAdapter ma) {
            int i;
            ma.RemoveFrom(this);
            for(i = 0; i < Controls.Count; i += 2)
                ((Template)Controls[i]).RemoveFrom(ma);
            for(i = 1; i < Controls.Count; i += 2)
                ((TemplateBorder)Controls[i]).RemoveFrom(ma);
        }

        public int Modify(Control c, string h, string[] ss, int ix) {
            int i;
            if(Controls.Count == 0) {
                Size s = TextRenderer.MeasureText((F.Data.HAHTemplateIsSplit ? "" : h) + ss[ix], F.HAH.Font);
                Point p = F.HAH.Location(c, (Width - s.Width) / 2, (Height - s.Height) / 2);
                c.Controls.Add(new HAHLabel {
                    Hitable = this,
                    Location = PointToScreen(p),
                    Text = (F.Data.HAHTemplateIsSplit ? "" : h) + ss[ix],
                    TransParentText = F.Data.HAHTemplateIsSplit ? h : "",
                });
                return ix + 1;
            } else {
                for(i = 0; i < Controls.Count; i += 2)
                    ix = ((Template)Controls[i]).Modify(c, h, ss, ix);
                return ix;
            }
        }

        public void Hit() {
            ((FitWin)TopLevelControl).Modify(2, delegate {
                ((FitWin)TopLevelControl).TaskWindow.ActiveTask.Hit(this);
                if(F.Data.HAHAutoNext)
                    ((FitWin)TopLevelControl).TaskWindow.Next();
            });
        }

        public int RecCount {
            get {
                int i;
                int sm = 0;
                for(i = 0; i < Controls.Count; i += 2)
                    sm += ((Template)Controls[i]).RecCount;
                return Math.Max(1, sm);
            }
        }

        public Color HoverColor {
            set {
                int i;
                for(i = 0; i < Controls.Count; i += 2)
                    ((Template)Controls[i]).HoverColor = value;
                for(i = 1; i < Controls.Count; i += 2)
                    Controls[i].BackColor = value;
            }
        }

        protected override int BorderWidth {
            get {
                return F.BorderWidth;
            }
        }
    }
}
