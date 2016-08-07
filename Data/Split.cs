using System.Windows.Forms;

namespace FitWinN {

    public class Split {

        public double R;
        public FlowDirection F;
        public Split[] S;

        public Split() {
        }

        public Split(SplitPanel sp) {
            int i;
            R = sp.Ratio;
            F = sp.FlowDirection;
            if(sp.Controls.Count == 0)
                return;
            S = new Split[sp.Controls.Count / 2 + 1];
            for(i = 0; i < sp.Controls.Count; i += 2)
                S[i / 2] = new Split((SplitPanel)sp.Controls[i]);
        }

        public bool Equals(Split s) {
            int i;
            if(R != s.R || F != s.F)
                return false;
            if(S == null || s.S == null)
                return S == s.S;
            if(S.Length != s.S.Length)
                return false;
            for(i = 0; i < S.Length; ++i) {
                if(!S[i].Equals(s.S[i]))
                    return false;
            }
            return true;
        }
    }
}
