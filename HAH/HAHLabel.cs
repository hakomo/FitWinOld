using System.Windows.Forms;

namespace FitWinN {

    class HAHLabel : Label {

        public bool IsHead = false;
        public string TransParentText = "";
        public Hitable Hitable = null;

        private int enableIx = 0, disableIx = 0;

        public HAHLabel() {
            AutoSize = true;
            BackColor = F.HAHColor;
            Font = F.HAH.Font;
        }

        public void Back() {
            if(!Visible) {
                --disableIx;
                if(disableIx == 0)
                    Visible = true;
            } else if(enableIx != 0) {
                --enableIx;
            }
        }

        public bool EnableKey(char k, bool isHead = false) {
            if(!Visible) {
                return false;
            } else if(enableIx < TransParentText.Length) {
                return k == TransParentText[enableIx];
            } else if(enableIx < TransParentText.Length + Text.Length) {
                return k == Text[enableIx - TransParentText.Length];
            }
            return isHead;
        }

        public bool Key(char k) {
            if(EnableKey(k, IsHead)) {
                ++enableIx;
                if(!IsHead && enableIx == Text.Length + TransParentText.Length) {
                    if(((Control)Hitable).TopLevelControl is FitWin) {
                        Hitable.Hit();
                    } else {
                        ((FitWin)TopLevelControl).Modify(2);
                    }
                    return true;
                }
            } else {
                if(Visible)
                    Visible = false;
                ++disableIx;
            }
            return false;
        }
    }
}
