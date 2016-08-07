using Hakomo.Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class TemplatePadding : FlowLayoutPanel, MouseListener {

        private static readonly ContextMenu cm;

        private MouseAdapter ma = new MouseAdapter();

        static TemplatePadding() {
            cm = new ContextMenu(new MenuItem[] {
                new MenuItem("テンプレートを削除(&D)", (s, e) => {
                    Control c = ((MenuItem)s).GetContextMenu().SourceControl.Parent;
                    ((TemplateWindow)c.Parent).Remove(c);
                }),
            });
        }

        public TemplatePadding() {
            ma.From = this;
            ma.To = this;
            AutoSize = true;
            BackColor = F.BorderColor;
            ContextMenu = cm;
            Margin = new Padding();
            Padding = new Padding(F.BorderWidth);
        }

        public TemplatePadding(Split s, MouseAdapter ma)
            : this() {
            ma.From = this;
            Controls.Add(new Template(s, this.ma, ma));
        }

        public TemplatePadding(SplitPanel sp, MouseAdapter ma)
            : this(new Split(sp), ma) {
        }

        public void RemoveFrom(MouseAdapter ma) {
            ma.RemoveFrom(this);
            ((Template)Controls[0]).RemoveFrom(ma);
        }

        public void OnMyEnter(object s, EventArgs e) {
        }

        public void OnMyLeave(object s, EventArgs e) {
        }

        public void OnMyClick(object s, MouseEventArgs e) {
            if(!F.Data.EditorVisible)
                return;
            Editor ep = new Editor((Template)Controls[0]);
            if(ep.Equals(F.Editor.Root))
                return;
            Action.Push(new EditorAction());
            ep.MyResize(F.Editor.Size);
            ep.Replace();
        }

        private TemplateMargin GetHoverTemplateMargin(Point p) {
            Control c = TopLevelControl;
            for(; c != null && !(c is TemplateMargin); c = c.GetChildAtPoint(c.PointToClient(p)))
                ;
            return c as TemplateMargin;
        }

        private static bool IsLeft(Point p, TemplateMargin tm) {
            return (new Rectangle(0, 0, tm.Width / 2, tm.Height)).Contains(tm.PointToClient(p));
        }

        private Rectangle GetHoverRectangle(Point p) {
            TemplateMargin tm = GetHoverTemplateMargin(p);
            if(tm == null)
                return new Rectangle();
            return new Rectangle((IsLeft(p, tm) ? tm.Left : tm.Right) - F.BorderWidth / 2, tm.Top, F.BorderWidth, tm.Height);
        }

        public void OnMyDrag(object s, MouseEventArgs e) {
            Cursor = Cursors.Cross;
            Rectangle r = GetHoverRectangle(((Control)s).PointToScreen(e.Location));
            if(!ma.IsBegin && r == ((TemplateWindow)Parent.Parent).HoverRectangle)
                return;
            using(new Redraw(Parent.Parent)) {
                ((TemplateWindow)Parent.Parent).HoverRectangle = r;
                HoverColor = F.TemplateHoverBorderColor;
            }
        }

        private void Sort(Point p) {
            TemplateMargin tm = GetHoverTemplateMargin(p);
            if(tm == null)
                return;
            int ix = Parent.Parent.Controls.GetChildIndex(Parent),
                jx = Parent.Parent.Controls.GetChildIndex(tm) + (IsLeft(p, tm) ? 0 : 1);
            if(jx < ix) {
                Action.Push(new TemplateMoveAction(ix, jx));
                Parent.Parent.Controls.SetChildIndex(Parent, jx);
            } else if(ix + 1 < jx) {
                Action.Push(new TemplateMoveAction(ix, jx - 1));
                Parent.Parent.Controls.SetChildIndex(Parent, jx - 1);
            }
        }

        public void OnMyDrop(object s, MouseEventArgs e) {
            Cursor = Cursors.Default;
            using(new Redraw(Parent.Parent)) {
                Sort(((Control)s).PointToScreen(e.Location));
                ((TemplateWindow)Parent.Parent).HoverRectangle = new Rectangle();
                HoverColor = F.BorderColor;
            }
        }

        private Color HoverColor {
            set {
                BackColor = value;
                ((Template)Controls[0]).HoverColor = value;
            }
        }
    }
}
