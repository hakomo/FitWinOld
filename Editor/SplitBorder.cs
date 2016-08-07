using System.Windows.Forms;

namespace FitWinN {

    abstract class SplitBorder : Control {

        public SplitBorder() {
            BackColor = F.BorderColor;
            Dock = DockStyle.Fill;
            Margin = new Padding();
            FlowDirection = FlowDirection.LeftToRight;
        }

        public SplitBorder(SplitBorder sb)
            : this() {
            FlowDirection = sb.FlowDirection;
        }

        public void MyResize() {
            if(FlowDirection == FlowDirection.LeftToRight) {
                Width = BorderWidth;
            } else {
                Height = BorderWidth;
            }
        }

        protected abstract int BorderWidth {
            get;
        }

        protected FlowDirection flowDirection;

        public abstract FlowDirection FlowDirection {
            get;
            set;
        }
    }
}
