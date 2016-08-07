using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FitWinN {

    class HAHS {

        public readonly HAH Template, TemplateWindow, Task, Multi;

        public HAHS(HAH template, HAH templateWindow, HAH task, HAH multi) {
            Template = template;
            TemplateWindow = templateWindow;
            Task = task;
            Multi = multi;
        }

        public HAHS(string[] template, string[] templateWindow, string[] task, string[] multi)
            : this(new HAH(template), new HAH(templateWindow), new HAH(task), new HAH(multi)) {
        }
    }

    class HAH {

        private static readonly HAHS[] hahss = new HAHS[2];

        private readonly string[] a;

        static HAH() {
            string asdfjkl = "ASDFJKL", lkjfdsa = "LKJFDSA";
            string[] template = new string[] { lkjfdsa, asdfjkl }, templateWindow = new string[] { asdfjkl, lkjfdsa };
            hahss[0] = new HAHS(template, templateWindow,
                new string[] { "T", asdfjkl, lkjfdsa }, new string[] { "N", asdfjkl, lkjfdsa });
            hahss[1] = new HAHS(template, templateWindow,
                new string[] { "QWERUIO", "OIUREWQ" }, new string[] { "12347890", "09874321" });
        }

        public HAH(string[] a) {
            this.a = a;
        }

        public string[] Strs(int n) {
            int i, j;
            int pi = 1;
            for(i = 0; pi * a[i % a.Length].Length < n; ++i)
                pi *= a[i % a.Length].Length;
            int[] b = new int[i + 1];
            string[] c = new string[n];
            for(i = 0; i < n; ++i) {
                StringBuilder sb = new StringBuilder();
                for(j = 0; j < b.Length; ++j) {
                    if(j == 0 || n >= pi * 2 || n % pi > i || i >= pi)
                        sb.Append(a[j % a.Length][b[j]]);
                }
                c[i] = sb.ToString();
                for(j = 0; j < b.Length; ++j) {
                    b[j] = (b[j] + 1) % a[j % a.Length].Length;
                    if(b[j] != 0)
                        break;
                }
            }
            return c;
        }

        public static string[] Template(int n) {
            if(n == 1)
                return new string[] { "" };
            return hahss[F.HAH.Kind].Template.Strs(n);
        }

        public static string[] TemplateWindow(int n) {
            return hahss[F.HAH.Kind].TemplateWindow.Strs(n);
        }

        public static string[] Task(int n) {
            return hahss[F.HAH.Kind].Task.Strs(n);
        }

        public static string[] Multi(int n) {
            return hahss[F.HAH.Kind].Multi.Strs(n);
        }
    }

    partial class FitWin {

        private static readonly Dictionary<Keys, System.Action> dic = new Dictionary<Keys, System.Action>();

        private void InitHAH() {
            int i;
            dic[Keys.Space] = delegate {
                if(F.Data.MultiVisible)
                    F.Multi.Window.Next();
            };
            dic[Keys.Space | Keys.Shift] = delegate {
                if(F.Data.MultiVisible)
                    F.Multi.Window.Back();
            };
            dic[Keys.OemSemicolon] = delegate {
                if(TaskWindow.ActiveTask != null)
                    TaskWindow.ActiveTask.PopupContext();
            };
            dic[Keys.Oemplus] = dic[Keys.OemSemicolon];
            dic[Keys.Apps] = dic[Keys.OemSemicolon];
            dic[Keys.Back] = delegate {
                for(i = 0; i < HAHControl.Controls.Count - 1; ++i)
                    ((HAHLabel)HAHControl.Controls[i]).Back();
            };
            dic[Keys.Tab] = TaskWindow.MyKeyRight;
            dic[Keys.Tab | Keys.Shift] = TaskWindow.MyKeyLeft;
            dic[Keys.Enter] = delegate {
                if(TaskWindow.ActiveTask != null)
                    TaskWindow.ActiveTask.OnMyClick(null, null);
            };
            dic[Keys.Up] = TaskWindow.MyKeyUp;
            dic[Keys.Down] = TaskWindow.MyKeyDown;
            dic[Keys.Left] = TaskWindow.MyKeyLeft;
            dic[Keys.Right] = TaskWindow.MyKeyRight;
            foreach(Keys k in new Keys[] { Keys.A, Keys.S, Keys.D, Keys.F, Keys.J, Keys.K,  Keys.L,
                Keys.Q, Keys.W, Keys.E, Keys.R, Keys.U, Keys.I, Keys.O, Keys.T, Keys.N,
                Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D7, Keys.D8, Keys.D9, Keys.D0 }) {
                char c = Convert.ToChar(k);
                dic[k] = delegate {
                    for(i = 0; i < HAHControl.Controls.Count - 1 &&
                        !((HAHLabel)HAHControl.Controls[i]).EnableKey(c); ++i)
                        ;
                    if(i >= HAHControl.Controls.Count - 1)
                        return;
                    for(i = 0; i < HAHControl.Controls.Count - 1 &&
                        !((HAHLabel)HAHControl.Controls[i]).Key(c); ++i)
                        ;
                };
            }
        }

        protected override bool ProcessDialogKey(Keys k) {
            if(k == Keys.Escape) {
                Esc();
            } else if(F.Data.HAHVisible && dic.ContainsKey(k)) {
                dic[k]();
            }
            return base.ProcessDialogKey(k);
        }
    }
}
