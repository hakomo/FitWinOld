using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class MultiWindow : Panel, Modifiable {

        private List<Rectangle> rectangles = new List<Rectangle>();

        public MultiWindow() {
            BackColor = F.BackColor;
            Margin = new Padding();
        }

        public void Next() {
            if(Controls.Count < 2)
                return;
            ((FitWin)TopLevelControl).Modify(3, delegate {
                ActiveMulti.BackColor = F.BackColor;
                activeIndex = (activeIndex + 1) % Controls.Count;
                ActiveMulti.BackColor = F.HoverColor;
            });
        }

        public void Back() {
            if(Controls.Count < 2)
                return;
            ((FitWin)TopLevelControl).Modify(3, delegate {
                ActiveMulti.BackColor = F.BackColor;
                activeIndex = (activeIndex + Controls.Count - 1) % Controls.Count;
                ActiveMulti.BackColor = F.HoverColor;
            });
        }

        public void Hit(Multi m) {
            ((FitWin)TopLevelControl).Modify(3, delegate {
                if(m == ActiveMulti)
                    return;
                ActiveMulti.BackColor = F.BackColor;
                activeIndex = Controls.GetChildIndex(m);
                ActiveMulti.BackColor = F.HoverColor;
            });
        }

        private bool HasModified(List<Rectangle> rectangles) {
            int i;
            if(rectangles.Count != this.rectangles.Count)
                return true;
            for(i = 0; i < rectangles.Count; ++i) {
                if(rectangles[i] != this.rectangles[i])
                    return true;
            }
            return false;
        }

        public void Modify() {
            int i;
            List<Rectangle> rectangles = new List<Rectangle>(), boundss = new List<Rectangle>();
            Rectangle r = new Rectangle();
            foreach(Screen s in Screen.AllScreens) {
                rectangles.Add(s.WorkingArea);
                boundss.Add(s.Bounds);
                r.X = Math.Min(r.X, s.Bounds.Left);
                r.Y = Math.Min(r.Y, s.Bounds.Top);
                r.Width = Math.Max(r.Width, s.Bounds.Right);
                r.Height = Math.Max(r.Height, s.Bounds.Bottom);
            }
            r.Size = new Size(r.Width - r.X, r.Height - r.Y);
            if(HasModified(rectangles)) {
                if(activeIndex != -1)
                    F.Data.MultiBounds = WorkingArea;
                this.rectangles = rectangles;
                Controls.Clear();
                for(i = 0; i < rectangles.Count; ++i)
                    Controls.Add(new Multi());
                activeIndex = Math.Max(0, rectangles.IndexOf(F.Data.MultiBounds));
                ActiveMulti.BackColor = F.HoverColor;
            }
            Size = F.Multi.WindowSize(this);
            if((double)(r.Width + F.BorderWidth) / (r.Height + F.BorderWidth) < (double)Width / Height) {
                double p = (double)(Height - F.BorderWidth) / r.Height;
                for(i = 0; i < Controls.Count; ++i) {
                    Point q = new Point((int)((boundss[i].Left - r.X) * p) + F.BorderWidth, (int)((boundss[i].Top - r.Y) * p) + F.BorderWidth);
                    Controls[i].Bounds = new Rectangle(q.X + (Width - (int)((r.Width + F.BorderWidth) * p)) / 2, q.Y,
                        (int)((boundss[i].Right - r.X) * p) - q.X, (int)((boundss[i].Bottom - r.Y) * p) - q.Y);
                }
            } else {
                double p = (double)(Width - F.BorderWidth) / r.Width;
                for(i = 0; i < Controls.Count; ++i) {
                    Point q = new Point((int)((boundss[i].Left - r.X) * p) + F.BorderWidth, (int)((boundss[i].Top - r.Y) * p) + F.BorderWidth);
                    Controls[i].Bounds = new Rectangle(q.X, q.Y + (Height - (int)((r.Height + F.BorderWidth) * p)) / 2,
                        (int)((boundss[i].Right - r.X) * p) - q.X, (int)((boundss[i].Bottom - r.Y) * p) - q.Y);
                }
            }
        }

        public void Modify(Control c) {
            int i;
            if(!F.Data.MultiVisible || !F.Data.HAHMultiEnable)
                return;
            string[] ss = HAH.Multi(Controls.Count);
            for(i = 0; i < Controls.Count; ++i) {
                Size s = TextRenderer.MeasureText(F.HAH.Kind == 0 && F.Data.HAHIsSplit ? ss[i].Substring(1) : ss[i], F.HAH.Font);
                Point p = F.HAH.Location(c, (Controls[i].Width - s.Width) / 2, (Controls[i].Height - s.Height) / 2);
                c.Controls.Add(new HAHLabel {
                    Hitable = (Multi)Controls[i],
                    Location = Controls[i].PointToScreen(p),
                    Text = F.HAH.Kind == 0 && F.Data.HAHIsSplit ? ss[i].Substring(1) : ss[i],
                    TransParentText = F.HAH.Kind == 0 && F.Data.HAHIsSplit ? ss[i].Substring(0, 1) : "",
                });
            }
            if(F.HAH.Kind != 0 || !F.Data.HAHIsSplit)
                return;
            Point q = F.HAH.Location(c, 0, 0);
            c.Controls.Add(new HAHLabel {
                IsHead = true,
                Location = Parent.Controls[0].PointToScreen(q),
                Text = ss[0].Substring(0, 1),
            });
        }

        private int activeIndex = -1;

        public Multi ActiveMulti {
            get {
                return activeIndex == -1 ? null : (Multi)Controls[activeIndex];
            }
        }

        public Rectangle WorkingArea {
            get {
                return rectangles[activeIndex];
            }
        }
    }
}
