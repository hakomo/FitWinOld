using Hakomo.Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class Task : Label, MouseListener, Hitable {

        private static readonly ContextMenu cm;

        private readonly IntPtr hw;
        private readonly MouseAdapter ma = new MouseAdapter();
        private string caption;
        private SplitPanel sp = null;

        static Task() {
            cm = new ContextMenu(new MenuItem[] {
                new MenuItem("元のサイズに戻す(&R)", GetEventHandler(WinAPI.Restore)),
                new MenuItem("最小化(&N)", GetEventHandler(WinAPI.Minimize)),
                new MenuItem("最大化(&X)", GetEventHandler(WinAPI.Maximize)),
                new MenuItem("-"),
                new MenuItem("閉じる(&C)", (s, e) => {
                    Task t = (Task)((MenuItem)s).GetContextMenu().SourceControl;
                    WinAPI.Close(t.hw);
                    ((FitWin)t.TopLevelControl).Modify(3);
                }),
                new MenuItem("-"),
                new MenuItem("フィルター(&F)", (s, e) => {
                    Task t = (Task)((MenuItem)s).GetContextMenu().SourceControl;
                    Action.Push(new FilterAction((FitWin)t.TopLevelControl));
                    string cn = WinAPI.GetClassName(t.hw);
                    if(F.Data.FilteredClassNames.Contains(cn)) {
                        F.Data.FilteredClassNames.Remove(cn);
                    } else {
                        F.Data.FilteredClassNames.Add(cn);
                    }
                    ((FitWin)t.TopLevelControl).Modify(3);
                }),
            });
            cm.Popup += (s, e) => {
                IntPtr hw = ((Task)((ContextMenu)s).SourceControl).hw;
                cm.MenuItems[0].Enabled = WinAPI.IsIconic(hw) || WinAPI.IsZoomed(hw);
                cm.MenuItems[1].Enabled = !WinAPI.IsIconic(hw) && WinAPI.CanMinimize(hw);
                cm.MenuItems[2].Enabled = !WinAPI.IsZoomed(hw) && WinAPI.CanMaximize(hw);
                cm.MenuItems[6].Checked = F.Data.FilteredClassNames.Contains(WinAPI.GetClassName(hw));
            };
        }

        public Task(IntPtr hw, MouseAdapter ma) {
            this.hw = hw;
            this.ma.From = this;
            this.ma.To = this;
            ma.From = this;

            ContextMenu = cm;
            Size = F.Task.Size;
        }

        private static EventHandler GetEventHandler(Action<IntPtr> a) {
            return (s, e) => {
                Task t = (Task)((MenuItem)s).GetContextMenu().SourceControl;
                Action.Push(new PlacementAction(t.hw, t));
                a(t.hw);
                WinAPI.SetForegroundWindow(t.TopLevelControl.Handle);
            };
        }

        public void PopupContext() {
            cm.Show(this, new Point(F.Task.Width / 2, F.Task.Width / 2));
        }

        public void Hover() {
            BackColor = F.HoverColor;
            Parent.Parent.Controls[0].Text = caption;
        }

        public void Unhover() {
            if(!ma.IsHover)
                BackColor = Color.Transparent;
            if(((TaskWindow)Parent).ActiveTask != null)
                Parent.Parent.Controls[0].Text = ((TaskWindow)Parent).ActiveTask.caption;
        }

        public void Modify() {
            caption = WinAPI.GetCaption(hw);
            if(TextRenderer.MeasureText(caption, SystemInformation.MenuFont).Width > F.Task.Width * 7) {
                caption = caption.Substring(0, Util.BinarySearch(0, caption.Length, (m) => {
                    return TextRenderer.MeasureText(caption.Substring(0, m) + "...", SystemInformation.MenuFont).Width <= F.Task.Width * 7;
                })) + "...";
            }
            if(Image != null)
                Image.Dispose();
            if(hw == TopLevelControl.Handle) {
                using(Icon icon = ((FitWin)TopLevelControl).Icon)
                    Image = icon.ToBitmap();
            } else {
                using(Icon icon = WinAPI.GetIcon(hw))
                    Image = icon.ToBitmap();
            }
        }

        public void Hit() {
            ((TaskWindow)Parent).Hit(this);
        }

        public void Hit(SplitPanel sp) {
            Action.Push(new PlacementAction(hw, this));
            if(sp != sp.Root) {
                WinAPI.Move(hw, hw == TopLevelControl.Handle ? IntPtr.Zero : TopLevelControl.Handle, sp.WindowBounds);
            } else if(F.Data.MultiVisible && Screen.AllScreens.Length > 1) {
                WinAPI.Move(hw, hw == TopLevelControl.Handle ? IntPtr.Zero : TopLevelControl.Handle, F.WorkingArea.Location);
                if(WinAPI.CanMaximize(hw))
                    WinAPI.Maximize(hw);
            } else if(WinAPI.CanMaximize(hw) && !WinAPI.IsZoomed(hw)) {
                WinAPI.Maximize(hw);
            } else {
                WinAPI.BeforeActivate(hw, TopLevelControl.Handle);
            }
            WinAPI.SetForegroundWindow(TopLevelControl.Handle);
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
            if(!ma.IsHover && this != ((TaskWindow)Parent).ActiveTask)
                return;
            using(Pen p = new Pen(F.HoverBorderColor))
                e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
        }

        public void OnMyEnter(object s, EventArgs e) {
            if(Parent == null || this == ((TaskWindow)Parent).ActiveTask)
                return;
            using(new Redraw(Parent.Parent))
                Hover();
        }

        public void OnMyLeave(object s, EventArgs e) {
            if(Parent == null || this == ((TaskWindow)Parent).ActiveTask)
                return;
            using(new Redraw(Parent.Parent))
                Unhover();
        }

        public void OnMyClick(object s, MouseEventArgs e) {
            Action.Push(new PlacementAction(hw, this));
            WinAPI.BeforeActivate(hw, TopLevelControl.Handle);
        }

        private Control GetHoverControl(Point p) {
            Control c = TopLevelControl;
            for(; ; ) {
                Control d = c.GetChildAtPoint(c.PointToClient(p));
                if(d == null)
                    break;
                c = d;
            }
            return c;
        }

        public void OnMyDrag(object s, MouseEventArgs e) {
            if(Parent == null)
                return;
            Cursor = Cursors.Cross;
            ((TaskWindow)Parent).HasDragged = true;
            Control c = GetHoverControl(PointToScreen(e.Location));
            if(sp == c)
                return;
            if(sp != null)
                sp.BackColor = F.BackColor;
            if(c is SplitPanel) {
                sp = (SplitPanel)c;
                sp.BackColor = F.HoverColor;
            } else if(sp != null) {
                sp = null;
            }
        }

        public void OnMyDrop(object s, MouseEventArgs e) {
            if(Parent == null)
                return;
            Cursor = Cursors.Default;
            ((TaskWindow)Parent).HasDragged = false;
            if(sp == null)
                return;
            Hit(sp);
            sp.BackColor = F.BackColor;
            sp = null;
        }
    }
}
