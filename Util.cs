using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class Caption : Label {

        public Caption(string s = "") {
            BackColor = F.CaptionColor;
            Dock = DockStyle.Fill;
            Font = SystemInformation.MenuFont;
            Height = F.CaptionHeight;
            Margin = new Padding();
            Text = s;
            TextAlign = ContentAlignment.MiddleCenter;
        }
    }

    class Frame : FlowLayoutPanel {

        public Frame(Control window, string caption = "") {
            AutoSize = true;
            FlowDirection = FlowDirection.TopDown;
            Margin = new Padding(0, 0, 0, F.BorderWidth);

            Controls.Add(new Caption(caption));
            Controls.Add(window);
        }
    }

    interface Modifiable {

        void Modify();

        void Modify(Control c);
    }

    interface Hitable {

        void Hit();
    }

    class Action {

        public static readonly MenuItem UndoMenuItem;

        private static readonly List<Action> actions = new List<Action>();

        private readonly System.Action action;

        static Action() {
            UndoMenuItem = new MenuItem("元に戻す(&U)", delegate {
                actions[actions.Count - 1].Undo();
                actions.RemoveAt(actions.Count - 1);
                if(actions.Count == 0)
                    UndoMenuItem.Enabled = false;
            }, Shortcut.CtrlZ);
            UndoMenuItem.Enabled = false;
        }

        public Action() {
        }

        public Action(System.Action action) {
            this.action = action;
        }

        public static void Push(Action a) {
            if(actions.Count >= 100)
                actions.RemoveAt(0);
            actions.Add(a);
            UndoMenuItem.Enabled = true;
        }

        protected virtual void Undo() {
            action();
        }
    }

    class MouseAdapter {

        private bool isHover = false;
        private Point location;
        private enum State { Up, Down, Drag };
        private State state = State.Up;

        private event EventHandler MouseEnter = null, MouseLeave = null;
        private event MouseEventHandler MouseClick = null, MouseDrag = null, MouseDrop = null;

        private void OnEnter(object s, EventArgs e) {
            isHover = true;
            if(MouseEnter != null)
                MouseEnter(s, e);
        }

        private void OnLeave(object s, EventArgs e) {
            isHover = false;
            if(MouseLeave != null)
                MouseLeave(s, e);
        }

        private void OnDown(object s, MouseEventArgs e) {
            if(e.Button != MouseButtons.Left)
                return;
            location = e.Location;
            state = State.Down;
        }

        private int GetDistance(int x, int y) {
            return x * x + y * y;
        }

        private void OnMove(object s, MouseEventArgs e) {
            if(e.Button != MouseButtons.Left || state == State.Up ||
                state == State.Down && GetDistance(e.X - Location.X, e.Y - Location.Y) < 9)
                return;
            if(MouseDrag != null)
                MouseDrag(s, e);
            state = State.Drag;
        }

        private void OnUp(object s, MouseEventArgs e) {
            if(e.Button != MouseButtons.Left)
                return;
            if(state == State.Down) {
                if(MouseClick != null)
                    MouseClick(s, e);
            } else if(state == State.Drag) {
                if(MouseDrop != null)
                    MouseDrop(s, e);
            }
            state = State.Up;
        }

        public void RemoveFrom(Control c) {
            c.MouseEnter -= OnEnter;
            c.MouseLeave -= OnLeave;
            c.MouseDown -= OnDown;
            c.MouseMove -= OnMove;
            c.MouseUp -= OnUp;
        }

        public void RemoveTo(MouseListener ml) {
            MouseEnter -= ml.OnMyEnter;
            MouseLeave -= ml.OnMyLeave;
            MouseClick -= ml.OnMyClick;
            MouseDrag -= ml.OnMyDrag;
            MouseDrop -= ml.OnMyDrop;
        }

        public bool IsBegin {
            get {
                return state == State.Down;
            }
        }

        public bool IsHover {
            get {
                return isHover;
            }
        }

        public Point Location {
            get {
                return location;
            }
        }

        public Control From {
            set {
                value.MouseEnter += OnEnter;
                value.MouseLeave += OnLeave;
                value.MouseDown += OnDown;
                value.MouseMove += OnMove;
                value.MouseUp += OnUp;
            }
        }

        public MouseListener To {
            set {
                MouseEnter += value.OnMyEnter;
                MouseLeave += value.OnMyLeave;
                MouseClick += value.OnMyClick;
                MouseDrag += value.OnMyDrag;
                MouseDrop += value.OnMyDrop;
            }
        }
    }

    interface MouseListener {

        void OnMyEnter(object s, EventArgs e);

        void OnMyLeave(object s, EventArgs e);

        void OnMyClick(object s, MouseEventArgs e);

        void OnMyDrag(object s, MouseEventArgs e);

        void OnMyDrop(object s, MouseEventArgs e);
    }
}
