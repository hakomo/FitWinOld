using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    public abstract class SplitPanel : FlowLayoutPanel {

        public Rectangle WindowBounds {
            get {
                return Root.GetWindowBounds(this, F.WorkingArea);
            }
        }

        public void MyResize(Size size) {
            int i;
            Size = size;
            if(Controls.Count == 0)
                return;
            if(FlowDirection == FlowDirection.LeftToRight) {
                List<int> dots = GetDots(Width - BorderWidth * (Controls.Count / 2));
                for(i = 0; i < Controls.Count; i += 2)
                    ((SplitPanel)Controls[i]).MyResize(new Size(dots[i / 2], Height));
                for(i = 1; i < Controls.Count; i += 2)
                    Controls[i].Height = Height;
            } else {
                List<int> dots = GetDots(Height - BorderWidth * (Controls.Count / 2));
                for(i = 0; i < Controls.Count; i += 2)
                    ((SplitPanel)Controls[i]).MyResize(new Size(Width, dots[i / 2]));
                for(i = 1; i < Controls.Count; i += 2)
                    Controls[i].Width = Width;
            }
            for(i = 1; i < Controls.Count; i += 2)
                ((SplitBorder)Controls[i]).MyResize();
        }

        private List<int> GetDots(int dot) {
            int i;
            double sm = dot / Ratios;
            List<int> dots = new List<int>();
            for(i = 0; i < Controls.Count; i += 2) {
                dots.Add((int)(sm * ((SplitPanel)Controls[i]).Ratio));
                dot -= dots[i / 2];
            }
            for(i = 0; i < dot; ++i)
                ++dots[i];
            return dots;
        }

        private Rectangle GetWindowBounds(SplitPanel sp, Rectangle r) {
            int i;
            if(sp == this)
                return r;
            if(Controls.Count == 0)
                return new Rectangle();
            if(FlowDirection == FlowDirection.LeftToRight) {
                List<int> dots = GetDots(r.Width);
                for(i = 0; i < Controls.Count; i += 2) {
                    r.Width = dots[i / 2];
                    Rectangle s = ((SplitPanel)Controls[i]).GetWindowBounds(sp, r);
                    if(s != new Rectangle())
                        return s;
                    r.X += dots[i / 2];
                }
            } else {
                List<int> dots = GetDots(r.Height);
                for(i = 0; i < Controls.Count; i += 2) {
                    r.Height = dots[i / 2];
                    Rectangle s = ((SplitPanel)Controls[i]).GetWindowBounds(sp, r);
                    if(s != new Rectangle())
                        return s;
                    r.Y += dots[i / 2];
                }
            }
            return new Rectangle();
        }

        public bool Equals(SplitPanel sp) {
            return (new Split(this)).Equals(new Split(sp));
        }

        protected abstract int BorderWidth {
            get;
        }

        public double Ratio = 1;

        protected double Ratios {
            get {
                int i;
                double sm = 0;
                for(i = 0; i < Controls.Count; i += 2)
                    sm += ((SplitPanel)Controls[i]).Ratio;
                return sm;
            }
        }

        public SplitPanel Root {
            get {
                SplitPanel sp = this;
                for(; sp.Parent is SplitPanel; sp = (SplitPanel)sp.Parent)
                    ;
                return sp;
            }
        }
    }
}
