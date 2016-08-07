using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class HAHOption : Form {

        public HAHOption() {
            BackColor = F.BackColor;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Size = new Size(400, 300);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Hit a Hint オプション";

            FlowLayoutPanel generalfp = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(8),
            };
            generalfp.Controls.AddRange(new Control[] {
                new CheckBox {
                    AutoSize = true,
                    Checked = F.Data.HAHMultiEnable,
                    Text = "マルチモニターのヒント",
                    Margin = new Padding(),
                }, new CheckBox {
                    AutoSize = true,
                    Checked = F.Data.HAHTaskEnable,
                    Text = "アプリの一覧のヒント",
                    Margin = new Padding(0, 0, 0, 22),
                }, new CheckBox {
                    AutoSize = true,
                    Checked = F.Data.HAHAutoNext,
                    Text = "ウィンドウを移動・サイズ変更したとき、\nアプリの一覧のフォーカスを移動",
                    Margin = new Padding(0, 0, 0, 22),
                }, new CheckBox {
                    AutoSize = true,
                    Checked = F.Data.HAHTemplateIsSplit,
                    Text = "テンプレートのヒントを分離",
                    Margin = new Padding(),
                },
            });

            GroupBox general = new GroupBox {
                Font = SystemInformation.MenuFont,
                Height = 194,
                Margin = new Padding(16, 16, 16, 0),
                Text = "全般",
                Width = 352,
            };
            general.Controls.Add(generalfp);

            Button restore = new Button {
                AutoSize = true,
                Font = SystemInformation.MenuFont,
                Margin = new Padding(0, 0, 8, 0),
                Text = "既定値に戻す",
                Width = 80,
            }, ok = new Button {
                AutoSize = true,
                DialogResult = DialogResult.OK,
                Font = SystemInformation.MenuFont,
                Margin = new Padding(0, 0, 8, 0),
                Text = "OK",
                Width = 80,
            }, cancel = new Button {
                AutoSize = true,
                DialogResult = DialogResult.Cancel,
                Font = SystemInformation.MenuFont,
                Margin = new Padding(0, 0, 8, 0),
                Text = "キャンセル",
                Width = 80,
            };
            restore.Click += delegate {
                ((CheckBox)generalfp.Controls[0]).Checked = true;
                ((CheckBox)generalfp.Controls[1]).Checked = true;
                ((CheckBox)generalfp.Controls[2]).Checked = true;
                ((CheckBox)generalfp.Controls[3]).Checked = false;
            };
            ok.Click += delegate {
                F.Data.HAHMultiEnable = ((CheckBox)generalfp.Controls[0]).Checked;
                F.Data.HAHTaskEnable = ((CheckBox)generalfp.Controls[1]).Checked;
                F.Data.HAHAutoNext = ((CheckBox)generalfp.Controls[2]).Checked;
                F.Data.HAHTemplateIsSplit = ((CheckBox)generalfp.Controls[3]).Checked;
                ((FitWin)Owner).Modify(2);
            };
            AcceptButton = ok;
            CancelButton = cancel;

            FlowLayoutPanel panel = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
            }, bottom = new FlowLayoutPanel {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(104, 0, 0, 0),
            };
            panel.Controls.Add(general);
            bottom.Controls.AddRange(new Control[] { restore, ok, cancel });

            Controls.AddRange(new Control[] { panel, bottom });
        }
    }
}
