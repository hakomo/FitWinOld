using Hakomo.Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class TemplateWindow : FlowLayoutPanel, Modifiable, MouseListener {

        public Rectangle HoverRectangle = new Rectangle();

        private readonly MouseAdapter ma = new MouseAdapter();

        public TemplateWindow() {
            ma.From = this;
            ma.To = this;
            AutoScroll = true;
            BackColor = F.BackColor;
            Margin = new Padding();

            if(F.Data.Splits != null) {
                foreach(Split s in F.Data.Splits)
                    Controls.Add(new TemplateMargin(s, ma));
                F.Data.Splits = null;
            }

            Button b = new Button {
                AutoSize = true,
                Font = SystemInformation.MenuFont,
                Margin = F.Template.Padding,
                Padding = F.Template.Padding,
                TabStop = false,
                Text = "エディタを追加",
                UseVisualStyleBackColor = true,
            };
            b.Click += delegate {
                F.Template.Window.Add();
            };
            Controls.Add(b);

            Controls.Add(new Control());
        }

        public void Add() {
            if(!F.Data.EditorVisible)
                return;
            if(Controls.Count - 2 >= 40) {
                MessageBox.Show("テンプレートは４０個までです", F.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Action.Push(new TemplateAddAction());
            TemplateMargin tm = new TemplateMargin(F.Editor.Root, ma);
            Template(tm).MyResize(F.Template.Size);
            using(new Redraw(this))
                Util.Add(this, tm, Controls.Count - 2);
        }

        public void Add(Split s, int ix) {
            TemplateMargin tm = new TemplateMargin(s, ma);
            Template(tm).MyResize(F.Template.Size);
            using(new Redraw(this))
                Util.Add(this, tm, ix);
        }

        public void RemoveAt() {
            RemoveFrom((TemplateMargin)Controls[Controls.Count - 3]);
            using(new Redraw(this))
                Controls.RemoveAt(Controls.Count - 3);
        }

        public void Remove(Control c) {
            Action.Push(new TemplateDeleteAction(c));
            RemoveFrom((TemplateMargin)c);
            using(new Redraw(this))
                Controls.Remove(c);
        }

        public void Sort(int ix, int jx) {
            using(new Redraw(this))
                Controls.SetChildIndex(Controls[jx], ix);
        }

        private void RemoveFrom(TemplateMargin tm) {
            tm.RemoveFrom(ma);
        }

        public void Modify() {
            int i;
            if(Width < F.WindowWidth(this)) {
                Size = F.Template.WindowSize(this);
                Controls[Controls.Count - 1].Margin = F.Template.TailMargin(this);
            } else {
                Controls[Controls.Count - 1].Margin = F.Template.TailMargin(this);
                Size = F.Template.WindowSize(this);
            }
            Padding = F.Template.WindowPadding(this);
            for(i = 0; i < Controls.Count - 2; ++i)
                Template(i).MyResize(F.Template.Size);
        }

        public void Modify(Control c) {
            int i;
            Point p = F.HAH.Location(c, F.Template.BorderWidth, F.Template.BorderHeight - F.HAH.FontHeight);
            string[] ss = HAH.TemplateWindow(Controls.Count - 2);
            for(i = 0; i < Controls.Count - 2; ++i) {
                if(Controls[i].Top + F.Template.BorderHeight - F.HAH.FontHeight < 0 ||
                    F.Template.WindowHeight(c) < Controls[i].Bottom - F.Template.BorderHeight)
                    continue;
                Template(i).Modify(c, ss[i], HAH.Template(Template(i).RecCount), 0);
                if(!F.Data.HAHTemplateIsSplit)
                    continue;
                c.Controls.Add(new HAHLabel {
                    IsHead = true,
                    Location = Controls[i].PointToScreen(p),
                    Text = ss[i],
                });
            }
        }

        public void OnMyMouseWheel(MouseEventArgs e) {
            OnMouseWheel(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
            if(HoverRectangle.Width == 0)
                return;
            using(SolidBrush sb = new SolidBrush(F.TemplateHoverBorderColor))
                e.Graphics.FillRectangle(sb, HoverRectangle);
        }

        protected override Point ScrollToControl(Control c) {
            return AutoScrollPosition;
        }

        public void OnMyEnter(object s, EventArgs e) {
            if(TopLevelControl == Form.ActiveForm)
                Focus();
        }

        public void OnMyLeave(object s, EventArgs e) {
        }

        public void OnMyClick(object s, MouseEventArgs e) {
        }

        public void OnMyDrag(object s, MouseEventArgs e) {
        }

        public void OnMyDrop(object s, MouseEventArgs e) {
        }

        public Split[] Splits {
            get {
                int i;
                Split[] splits = new Split[Controls.Count - 2];
                for(i = 0; i < Controls.Count - 2; ++i)
                    splits[i] = new Split(Template(i));
                return splits;
            }
        }

        private Template Template(int ix) {
            return (Template)Controls[ix].Controls[0].Controls[0];
        }

        private Template Template(TemplateMargin tm) {
            return (Template)tm.Controls[0].Controls[0];
        }
    }
}
