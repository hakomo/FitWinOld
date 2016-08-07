using Hakomo.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class TaskWindow : Panel, Modifiable, MouseListener {

        public bool HasDragged = false;

        private List<long> longs = null;
        private MouseAdapter ma;

        public TaskWindow() {
            AutoScroll = true;
            BackColor = F.BackColor;
            Margin = new Padding();
        }

        public void MyKeyUp() {
            if(Controls.Count < 3 || activeIndex == ActiveIndexUp)
                return;
            using(new Redraw(Parent)) {
                ActiveTask.Unhover();
                activeIndex = ActiveIndexUp;
                ActiveTask.Hover();
            }
        }

        public void MyKeyDown() {
            if(Controls.Count < 3 || activeIndex == ActiveIndexDown)
                return;
            using(new Redraw(Parent)) {
                ActiveTask.Unhover();
                activeIndex = ActiveIndexDown;
                ActiveTask.Hover();
            }
        }

        public void MyKeyLeft() {
            if(Controls.Count < 3)
                return;
            using(new Redraw(Parent)) {
                ActiveTask.Unhover();
                activeIndex = (activeIndex + Controls.Count - 2) % (Controls.Count - 1);
                ActiveTask.Hover();
            }
        }

        public void MyKeyRight() {
            if(Controls.Count < 3)
                return;
            using(new Redraw(Parent)) {
                ActiveTask.Unhover();
                activeIndex = (activeIndex + 1) % (Controls.Count - 1);
                ActiveTask.Hover();
            }
        }

        public void Next() {
            if(Controls.Count < 3)
                return;
            ActiveTask.Unhover();
            activeIndex = (activeIndex + 1) % (Controls.Count - 1);
            ActiveTask.Hover();
        }

        public void Hit(Task t) {
            ((FitWin)TopLevelControl).Modify(2, delegate {
                if(t == ActiveTask)
                    return;
                ActiveTask.Unhover();
                activeIndex = Controls.GetChildIndex(t);
                ActiveTask.Hover();
            });
        }

        private void ModifyView() {
            int i;
            Size = F.Task.WindowSize(this);
            for(i = 0; i < Controls.Count - 1; ++i) {
                Controls[i].Location = F.Task.Location(F.WindowWidth(this), Math.Min(Controls.Count - 1, F.Task.Rows), i);
                ((Task)Controls[i]).Modify();
            }
            Controls[i].Top = F.Task.Top(i + F.Task.Rows);
        }

        private List<long> GetLongs(List<IntPtr> hws) {
            List<long> longs = new List<long>();
            foreach(IntPtr hw in hws)
                longs.Add(hw.ToInt64());
            longs.Sort();
            return longs;
        }

        private bool HasModified(List<long> longs) {
            int i;
            if(this.longs == null || longs.Count != this.longs.Count)
                return true;
            for(i = 0; i < longs.Count; ++i) {
                if(longs[i] != this.longs[i])
                    return true;
            }
            return false;
        }

        public void Modify() {
            List<IntPtr> hws = Tasks;
            List<long> longs = GetLongs(hws);
            if(HasModified(longs)) {
                ma = new MouseAdapter();
                ma.From = this;
                ma.To = this;
                this.longs = longs;
                Controls.Clear();
                foreach(IntPtr hw in hws)
                    Controls.Add(new Task(hw, ma));
                Controls.Add(new Control());
                ModifyView();
                if(F.Data.HAHVisible && Controls.Count > 1) {
                    activeIndex = 0;
                    ActiveTask.Hover();
                } else {
                    activeIndex = -1;
                }
            } else {
                ModifyView();
                if(F.Data.HAHVisible && activeIndex == -1 && Controls.Count > 1) {
                    activeIndex = 0;
                    ActiveTask.Hover();
                } else if(!F.Data.HAHVisible && activeIndex != -1) {
                    ActiveTask.Unhover();
                    activeIndex = -1;
                }
            }
        }

        public void Modify(Control c) {
            int i;
            if(!F.Data.HAHTaskEnable || Controls.Count == 1)
                return;
            Point p = F.HAH.Location(c, 0, 0);
            string[] ss = HAH.Task(Controls.Count - 1);
            for(i = 0; i < Controls.Count - 1; ++i) {
                if(Controls[i].Top < 0 || F.Task.WindowHeight < Controls[i].Bottom)
                    continue;
                c.Controls.Add(new HAHLabel {
                    Hitable = (Task)Controls[i],
                    Location = Controls[i].PointToScreen(p),
                    Text = F.HAH.Kind == 0 && F.Data.HAHIsSplit ? ss[i].Substring(1) : ss[i],
                    TransParentText = F.HAH.Kind == 0 && F.Data.HAHIsSplit ? ss[i].Substring(0, 1) : "",
                });
            }
            if(F.HAH.Kind != 0 || !F.Data.HAHIsSplit)
                return;
            c.Controls.Add(new HAHLabel {
                IsHead = true,
                Location = Parent.Controls[0].PointToScreen(p),
                Text = ss[0].Substring(0, 1),
            });
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            if(HasDragged) {
                F.Template.Window.OnMyMouseWheel(e);
            } else {
                base.OnMouseWheel(e);
            }
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

        private int activeIndex = -1;

        public Task ActiveTask {
            get {
                return activeIndex == -1 ? null : (Task)Controls[activeIndex];
            }
        }

        private List<IntPtr> Tasks {
            get {
                List<IntPtr> hws = new List<IntPtr>();
                foreach(IntPtr hw in WinAPI.GetTasks()) {
                    if(hw == TopLevelControl.Handle ? F.Data.SelfVisible :
                            !F.Data.Filters || !F.Data.FilteredClassNames.Contains(WinAPI.GetClassName(hw)))
                        hws.Add(hw);
                }
                return hws;
            }
        }

        private int ActiveIndexUp {
            get {
                return (activeIndex - F.Task.Rows + (Controls.Count - 1) * F.Task.Rows) % (Controls.Count - 1);
            }
        }

        private int ActiveIndexDown {
            get {
                return (activeIndex + F.Task.Rows) % (Controls.Count - 1);
            }
        }
    }
}
