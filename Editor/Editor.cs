using Hakomo.Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class Editor : SplitPanel {

        private static readonly ContextMenu cm;

        static Editor() {
            cm = new ContextMenu(new MenuItem[] {
                new MenuItem("水平２分割(&S)", GetEventHandler(2)),
                new MenuItem("水平３分割(&D)", GetEventHandler(3)),
                new MenuItem("水平４分割(&F)", GetEventHandler(4)),
                new MenuItem("-"),
                new MenuItem("垂直２分割(&W)", GetEventHandler(2, FlowDirection.TopDown)),
                new MenuItem("垂直３分割(&E)", GetEventHandler(3, FlowDirection.TopDown)),
                new MenuItem("垂直４分割(&R)", GetEventHandler(4, FlowDirection.TopDown)),
            });
            cm.Popup += (s, e) => {
                int i;
                Editor ep = (Editor)((ContextMenu)s).SourceControl;
                for(i = 2; i <= 4; ++i) {
                    cm.MenuItems[i - 2].Enabled = (ep.Width - ep.BorderWidth * (i - 1)) / i >= F.Editor.MinWidth;
                    cm.MenuItems[i + 2].Enabled = (ep.Height - ep.BorderWidth * (i - 1)) / i >= F.Editor.MinWidth;
                }
            };
        }

        public Editor() {
            BackColor = F.BackColor;
            ContextMenu = cm;
            Margin = new Padding();
        }

        public Editor(Split s)
            : this() {
            int i;
            FlowDirection = s.F;
            Ratio = s.R;
            if(s.S == null)
                return;
            for(i = 0; i < s.S.Length; ++i) {
                Controls.Add(new Editor(s.S[i]));
                if(i < s.S.Length - 1)
                    Controls.Add(new EditorBorder { FlowDirection = FlowDirection });
            }
        }

        public Editor(SplitPanel sp)
            : this(new Split(sp)) {
        }

        private static EventHandler GetEventHandler(int n, FlowDirection fd = FlowDirection.LeftToRight) {
            return (s, e) => ((Editor)((MenuItem)s).GetContextMenu().SourceControl).Split(n, fd);
        }

        public void Replace() {
            using(new Redraw(F.Editor.Root.Parent)) {
                F.Editor.Root.Parent.Controls.Add(this);
                F.Editor.Root.Parent.Controls.Remove(F.Editor.Root);
            }
            F.Editor.Root = this;
        }

        private void SplitParent(int n) {
            int i;
            int ix = Parent.Controls.GetChildIndex(this);
            if(FlowDirection == FlowDirection.LeftToRight) {
                int dot = Width - BorderWidth * (n - 1);
                for(i = 0; i < n; ++i) {
                    Util.Add(Parent, new Editor {
                        Ratio = Ratio * dot / n / Width,
                        Size = new Size(dot / n + (i < dot % n ? 1 : 0), Height),
                    }, ix + i * 2);
                    if(i < n - 1)
                        Util.Add(Parent, new EditorBorder(), ix + i * 2 + 1);
                }
            } else {
                int dot = Height - BorderWidth * (n - 1);
                for(i = 0; i < n; ++i) {
                    Util.Add(Parent, new Editor {
                        Ratio = Ratio * dot / n / Height,
                        Size = new Size(Width, dot / n + (i < dot % n ? 1 : 0)),
                    }, ix + i * 2);
                    if(i < n - 1)
                        Util.Add(Parent, new EditorBorder { FlowDirection = FlowDirection.TopDown }, ix + i * 2 + 1);
                }
            }
            Parent.Controls.Remove(this);
        }

        private void Split(int n, FlowDirection fd) {
            int i;
            Action.Push(new EditorAction());
            FlowDirection = fd;
            using(new Redraw(Parent)) {
                if(Parent is Editor && ((Editor)Parent).FlowDirection == FlowDirection) {
                    SplitParent(n);
                } else if(FlowDirection == FlowDirection.LeftToRight) {
                    int dot = Width - BorderWidth * (n - 1);
                    for(i = 0; i < n; ++i) {
                        Controls.Add(new Editor {
                            Size = new Size(dot / n + (i < dot % n ? 1 : 0), Height),
                        });
                        if(i < n - 1)
                            Controls.Add(new EditorBorder());
                    }
                } else {
                    int dot = Height - BorderWidth * (n - 1);
                    for(i = 0; i < n; ++i) {
                        Controls.Add(new Editor {
                            Size = new Size(Width, dot / n + (i < dot % n ? 1 : 0)),
                        });
                        if(i < n - 1)
                            Controls.Add(new EditorBorder { FlowDirection = FlowDirection.TopDown });
                    }
                }
            }
        }

        protected override int BorderWidth {
            get {
                return F.Editor.BorderWidth;
            }
        }
    }
}
