using Hakomo.Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class EditorBorder : SplitBorder, MouseListener {

        private static readonly ContextMenu cm;

        private readonly MouseAdapter ma = new MouseAdapter();

        static EditorBorder() {
            cm = new ContextMenu(new MenuItem[] {
                new MenuItem("ボーダーを削除(&D)", (s, e) => {
                    ((EditorBorder)((MenuItem)s).GetContextMenu().SourceControl).Delete();
                }),
            });
        }

        public EditorBorder()
            : base() {
            ma.From = this;
            ma.To = this;
            ContextMenu = cm;
        }

        public EditorBorder(SplitBorder sb)
            : base(sb) {
            ma.From = this;
            ma.To = this;
            ContextMenu = cm;
        }

        private void Delete() {
            Action.Push(new EditorAction());
            if(Parent.Controls.Count == 3) {
                using(new Redraw(Parent))
                    Parent.Controls.Clear();
                return;
            }
            int ix = Parent.Controls.GetChildIndex(this);
            Editor e;
            if(FlowDirection == FlowDirection.LeftToRight) {
                e = new Editor {
                    Size = new Size(Parent.Controls[ix - 1].Width + Parent.Controls[ix + 1].Width + BorderWidth, Parent.Height),
                };
                e.Ratio = e.Width * GetRatios(ix);
            } else {
                e = new Editor {
                    Size = new Size(Parent.Width, Parent.Controls[ix - 1].Height + Parent.Controls[ix + 1].Height + BorderWidth),
                };
                e.Ratio = e.Height * GetRatios(ix);
            }
            using(new Redraw(Parent)) {
                Parent.Controls.RemoveAt(ix + 1);
                Parent.Controls.RemoveAt(ix - 1);
                Util.Add(Parent, e, ix - 1);
                Parent.Controls.Remove(this);
            }
        }

        private double GetRatios(int ix) {
            int i;
            double sm = 0;
            for(i = 0; i < Parent.Controls.Count; i += 2) {
                if(i == ix - 1 || i == ix + 1)
                    continue;
                Editor e = (Editor)Parent.Controls[i];
                sm += e.Ratio / (FlowDirection == FlowDirection.LeftToRight ? e.Width : e.Height);
            }
            return sm / (Parent.Controls.Count / 2 - 1);
        }

        public void OnMyEnter(object s, EventArgs e) {
        }

        public void OnMyLeave(object s, EventArgs e) {
        }

        public void OnMyClick(object s, MouseEventArgs e) {
        }

        public void OnMyDrag(object s, MouseEventArgs e) {
            if(ma.IsBegin)
                Action.Push(new EditorAction());
            int ix = Parent.Controls.GetChildIndex(this);
            double ratios = Parent.Controls.Count == 3 ? 1 : GetRatios(ix);
            Editor bf = (Editor)Parent.Controls[ix - 1], af = (Editor)Parent.Controls[ix + 1];
            if(FlowDirection == FlowDirection.LeftToRight) {
                int dot = Math.Min(Math.Max(F.Editor.MinWidth - bf.Width, e.X - ma.Location.X), af.Width - F.Editor.MinWidth);
                if(dot == 0)
                    return;
                using(new Redraw(Parent)) {
                    bf.Width += dot;
                    af.Width -= dot;
                    bf.Ratio = bf.Width * ratios;
                    af.Ratio = af.Width * ratios;
                    bf.MyResize(bf.Size);
                    af.MyResize(af.Size);
                }
            } else {
                int dot = Math.Min(Math.Max(F.Editor.MinWidth - bf.Height, e.Y - ma.Location.Y), af.Height - F.Editor.MinWidth);
                if(dot == 0)
                    return;
                using(new Redraw(Parent)) {
                    bf.Height += dot;
                    af.Height -= dot;
                    bf.Ratio = bf.Height * ratios;
                    af.Ratio = af.Height * ratios;
                    bf.MyResize(bf.Size);
                    af.MyResize(af.Size);
                }
            }
        }

        public void OnMyDrop(object s, MouseEventArgs e) {
        }

        protected override int BorderWidth {
            get {
                return F.Editor.BorderWidth;
            }
        }

        public override FlowDirection FlowDirection {
            get {
                return flowDirection;
            }
            set {
                flowDirection = value;
                if(FlowDirection == FlowDirection.LeftToRight) {
                    Cursor = Cursors.SizeWE;
                    Margin = new Padding(0, F.BorderWidth, 0, F.BorderWidth);
                    Width = BorderWidth;
                } else {
                    Cursor = Cursors.SizeNS;
                    Margin = new Padding(F.BorderWidth, 0, F.BorderWidth, 0);
                    Height = BorderWidth;
                }
            }
        }
    }
}
