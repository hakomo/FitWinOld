using Hakomo.Library;
using System;
using System.Windows.Forms;

namespace FitWinN {

    partial class FitWin {

        private NotifyIcon ni;

        private void InitResident() {
            ni = new NotifyIcon {
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("ウィンドウを表示", (s, e) => MyShow()),
                    new MenuItem("閉じる(&C)", (s, e) => Application.Exit()),
                }),
                Icon = Properties.Resources.icon,
                Text = F.Text,
                Visible = F.Data.Resident,
            };
            ni.DoubleClick += (s, e) => MyShow();
            Application.ApplicationExit += (s, e) => ni.Dispose();

            WinAPI.RegisterHotKey(Handle, 9, 7, Convert.ToByte(Keys.F17));
        }

        private void Esc() {
            if(F.Data.Resident) {
                Visible = false;
            } else {
                Close();
            }
        }

        private void OnMyMinimize() {
            if(F.Data.Resident)
                Visible = false;
        }

        private void MyShow() {
            Visible = true;
            WinAPI.Activate(Handle);
            if(F.Data.HAHVisible)
                Modify(3);
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            if(m.Msg == 0x312)
                MyShow();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            if(!F.Data.Resident || e.CloseReason != CloseReason.UserClosing)
                return;
            e.Cancel = true;
            Visible = false;
        }
    }
}
