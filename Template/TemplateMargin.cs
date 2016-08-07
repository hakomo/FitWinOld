using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class TemplateMargin : FlowLayoutPanel {

        public TemplateMargin() {
            AutoSize = true;
            BackColor = Color.Transparent;
            Margin = new Padding();
            Padding = F.Template.Padding;
        }

        public TemplateMargin(Split s, MouseAdapter ma)
            : this() {
            ma.From = this;
            Controls.Add(new TemplatePadding(s, ma));
        }

        public TemplateMargin(SplitPanel sp, MouseAdapter ma)
            : this(new Split(sp), ma) {
        }

        public void RemoveFrom(MouseAdapter ma) {
            ma.RemoveFrom(this);
            ((TemplatePadding)Controls[0]).RemoveFrom(ma);
        }
    }
}
