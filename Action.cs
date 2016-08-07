using Hakomo.Library;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FitWinN {

    class EditorAction : Action {

        private readonly Split s = new Split(F.Editor.Root);

        protected override void Undo() {
            Editor e = new Editor(s);
            e.MyResize(F.Editor.Size);
            e.Replace();
        }
    }

    class FilterAction : Action {

        private readonly HashSet<string> hs = new HashSet<string>(F.Data.FilteredClassNames);
        private readonly FitWin f;

        public FilterAction(FitWin f) {
            this.f = f;
        }

        protected override void Undo() {
            F.Data.FilteredClassNames = hs;
            f.Modify(3);
        }
    }

    class PlacementAction : Action {

        private readonly IntPtr hw;
        private readonly WinAPI.Placement p;

        public PlacementAction(IntPtr hw, Control c) {
            this.hw = c.TopLevelControl.Handle;
            p = new WinAPI.Placement(hw);
        }

        protected override void Undo() {
            p.Restore();
            WinAPI.SetForegroundWindow(hw);
        }
    }

    class TemplateAddAction : Action {

        protected override void Undo() {
            F.Template.Window.RemoveAt();
        }
    }

    class TemplateDeleteAction : Action {

        private readonly int ix;
        private readonly Split s;

        public TemplateDeleteAction(Control c) {
            ix = F.Template.Window.Controls.GetChildIndex(c);
            s = new Split((Template)c.Controls[0].Controls[0]);
        }

        protected override void Undo() {
            F.Template.Window.Add(s, ix);
        }
    }

    class TemplateMoveAction : Action {

        private readonly int ix, jx;

        public TemplateMoveAction(int ix, int jx) {
            this.ix = ix;
            this.jx = jx;
        }

        protected override void Undo() {
            F.Template.Window.Sort(ix, jx);
        }
    }
}
