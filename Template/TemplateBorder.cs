using System.Windows.Forms;

namespace FitWinN {

    class TemplateBorder : SplitBorder {

        public TemplateBorder(MouseAdapter ma, MouseAdapter mb)
            : base() {
            ma.From = this;
            mb.From = this;
        }

        protected override int BorderWidth {
            get {
                return F.BorderWidth;
            }
        }

        public void RemoveFrom(MouseAdapter ma) {
            ma.RemoveFrom(this);
        }

        public override FlowDirection FlowDirection {
            get {
                return flowDirection;
            }
            set {
                flowDirection = value;
                if(FlowDirection == FlowDirection.LeftToRight) {
                    Width = BorderWidth;
                } else {
                    Height = BorderWidth;
                }
            }
        }
    }
}
