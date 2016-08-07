using Hakomo.Library;
using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace FitWinN {

    partial class FitWin : Form {

        private bool isBegined = false;

        public FitWin() {

            F.Data = Data.File;

            SetBounds();

            Icon = Properties.Resources.icon;
            MaximizeBox = false;
            Menu = MainMenu;
            Text = F.Text;

            FlowLayoutPanel p = new FlowLayoutPanel {
                AutoSize = true,
                BackColor = F.BorderColor,
                FlowDirection = FlowDirection.TopDown,
                Margin = new Padding(),
                Padding = new Padding(F.BorderWidth, F.BorderWidth, F.BorderWidth, 0),
            };

            F.Multi.Window = new MultiWindow();
            if(F.Data.MultiVisible)
                p.Controls.Add(new Frame(F.Multi.Window, "マルチモニター"));
            F.Editor.Root = new Editor();
            if(F.Data.EditorVisible)
                p.Controls.Add(new Frame(new EditorWindow(), "エディタ"));
            p.Controls.Add(new Frame(new TaskWindow()));
            F.Template.Window = new TemplateWindow();
            p.Controls.Add(new Frame(F.Template.Window, "テンプレート"));

            Panel q = new Panel { Dock = DockStyle.Fill };
            q.Controls.Add(p);

            Controls.Add(q);

            InitHAH();

            CheckVersion();
        }

        private async void CheckVersion() {
            using(WebClient wc = new WebClient()) {
                try {
                    if(await wc.DownloadStringTaskAsync("http://hakomo.github.io/fitwin/version") != F.Version)
                        Text = F.Text + " - 新しいバージョンが公開されています";
                } catch(WebException) {
                }
            }
        }

        private void SetBounds() {
            StartPosition = FormStartPosition.Manual;
            DesktopBounds = F.Data.Bounds;
            foreach(Screen s in Screen.AllScreens) {
                if(s.WorkingArea.Contains(Location))
                    return;
            }
            DesktopLocation = new Point(400, 0);
            F.Data.Bounds.Location = DesktopLocation;
        }

        public void Modify(int k, System.Action a = null) {
            using(new Redraw((k & 2) == 2 ? HAHControl : ParentControl)) {
                if(a != null)
                    a();
                if((k & 1) == 1) {
                    foreach(Control c in ParentControl.Controls)
                        ((Modifiable)c.Controls[1]).Modify();
                }
                if((k & 2) == 2 && (F.Data.HAHVisible || HAHControl.Controls.Count != 1)) {
                    Control c = ParentControl;
                    HAHControl.Controls.Clear();
                    if(F.Data.HAHVisible) {
                        foreach(Control d in c.Controls)
                            ((Modifiable)d.Controls[1]).Modify(HAHControl);
                    }
                    HAHControl.Controls.Add(c);
                }
            }
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            Modify(1);
            if(isBegined)
                return;
            isBegined = true;
            if(F.Data.HAHVisible)
                Modify(3);
            InitResident();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if(WindowState == FormWindowState.Minimized) {
                OnMyMinimize();
            } else if(Controls.Count != 0) {
                if(WindowState == FormWindowState.Normal)
                    F.Data.Bounds = DesktopBounds;
                Modify(3);
            }
        }

        protected override void OnResizeEnd(EventArgs e) {
            base.OnResizeEnd(e);
            if(WindowState == FormWindowState.Normal)
                F.Data.Bounds = DesktopBounds;
            Modify(1);
        }

        protected override void OnFormClosed(FormClosedEventArgs e) {
            base.OnFormClosed(e);
            if(F.Multi.Window.ActiveMulti != null)
                F.Data.MultiBounds = F.Multi.Window.WorkingArea;
            F.Data.Splits = F.Template.Window.Splits;
            Data.File = F.Data;
        }

        public TaskWindow TaskWindow {
            get {
                return (TaskWindow)ParentControl.Controls[(F.Data.MultiVisible ? 1 : 0) +
                    (F.Data.EditorVisible ? 1 : 0)].Controls[1];
            }
        }

        private Control HAHControl {
            get {
                return Controls[0];
            }
        }

        private Control ParentControl {
            get {
                return HAHControl.Controls[HAHControl.Controls.Count - 1];
            }
        }

        [STAThread]
        private static void Main() {
            Util.Run<FitWin>("9287b2ab-3d79-47d1-904e-4a028a2d8c10", delegate {
                Keyboard.Input(new Keys[] { Keys.ControlKey, Keys.Menu, Keys.ShiftKey }, Keys.F17);
            });
        }
    }
}
