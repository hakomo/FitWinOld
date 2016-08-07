using Hakomo.Library;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FitWinN {

    partial class FitWin {

        private MainMenu MainMenu {
            get {
                return new MainMenu(new MenuItem[] {
                    new MenuItem("編集(&E)", new MenuItem[] {
                        Action.UndoMenuItem,
                        new MenuItem("-"),
                        new MenuItem("エディタをクリア", delegate {
                            if(!F.Data.EditorVisible)
                                return;
                            Editor e = new Editor { Size = F.Editor.Size };
                            if(e.Equals(F.Editor.Root))
                                return;
                            Action.Push(new EditorAction());
                            e.Replace();
                        }),
                        new MenuItem("エディタを追加", delegate {
                            F.Template.Window.Add();
                        }),
                        new MenuItem("-"),
                        new MenuItem("ウィンドウの配置を保存(&S)", delegate {
                            try {
                                Process.Start(Application.StartupPath + "\\tool\\restowa\\restowa_save");
                            } catch {
                            }
                        }, Shortcut.CtrlS),
                        new MenuItem("ウィンドウの配置を復元(&R)", delegate {
                            try {
                                Process.Start(Application.StartupPath + "\\tool\\restowa\\restowa_restore");
                            } catch {
                            }
                        }, Shortcut.CtrlR),
                    }),
                    ViewMenu,
                    OptionMenu,
                    new MenuItem("ヘルプ(&H)", new MenuItem[] {
                        new MenuItem("オンラインマニュアル...", delegate {
                            Process.Start("http://hakomo.github.io/fitwin/");
                        }),
                        new MenuItem("バグの報告・要望・感想など...", delegate {
                            Process.Start("https://docs.google.com/forms/d/1y261eyYnYuB5LwlV9NHfPYD_DaU48GlPGwQdp_iWnWQ/viewform?entry.1558357097=Fit+Win&entry.187745740&entry.1950332360");
                        }),
                        new MenuItem(F.Text + " について(&A)...", delegate {
                            using(Form f = new Form {
                                BackColor = F.BackColor,
                                FormBorderStyle = FormBorderStyle.FixedDialog,
                                MaximizeBox = false,
                                MinimizeBox = false,
                                ShowInTaskbar = false,
                                Size = new Size(320, 160),
                                StartPosition = FormStartPosition.CenterParent,
                                Text = F.Text + " について",
                            }) {
                                f.Controls.Add(new Label {
                                    AutoSize = true,
                                    Font = new Font(SystemInformation.MenuFont.FontFamily, 10),
                                    Location = new Point(8, 8),
                                    Text = F.Text + " " + F.Version + "\n(C) 2014 hakomo\n\nEmail address : hakomof@gmail.com\nTwitter : @hakomof",
                                });
                                f.ShowDialog(this);
                            }
                        }),
                    }),
                });
            }
        }

        private MenuItem ViewMenu {
            get {
                MenuItem viewMenu = new MenuItem("表示(&V)", new MenuItem[] {
                    new MenuItem("&Hit a Hint", delegate {
                        F.Data.HAHVisible = !F.Data.HAHVisible;
                        Modify(3);
                    }, Shortcut.CtrlF),
                    new MenuItem("-"),
                    new MenuItem("マルチモニター", delegate {
                        System.Action show = delegate {
                            F.Data.MultiVisible = true;
                            Modify(3, delegate {
                                Util.Add(ParentControl, new Frame(F.Multi.Window, "マルチモニター"), 0);
                            });
                        }, hide = delegate {
                            F.Data.MultiVisible = false;
                            Modify(3, delegate {
                                ParentControl.Controls.RemoveAt(0);
                            });
                        };
                        if(F.Data.MultiVisible) {
                            Action.Push(new Action(show));
                            hide();
                        } else {
                            Action.Push(new Action(hide));
                            show();
                        }
                    }),
                    new MenuItem("エディタ", delegate {
                        System.Action show = delegate {
                            F.Data.EditorVisible = true;
                            Modify(3, delegate {
                                Util.Add(ParentControl, new Frame(new EditorWindow(), "エディタ"), F.Data.MultiVisible ? 1 : 0);
                            });
                        }, hide = delegate {
                            F.Data.EditorVisible = false;
                            Modify(3, delegate {
                                ParentControl.Controls.RemoveAt(F.Data.MultiVisible ? 1 : 0);
                            });
                        };
                        if(F.Data.EditorVisible) {
                            Action.Push(new Action(show));
                            hide();
                        } else {
                            Action.Push(new Action(hide));
                            show();
                        }
                    }),
                    new MenuItem("-"),
                    MultiMenu,
                    EditorMenu,
                    TaskMenu,
                    TemplateMenu,
                });
                viewMenu.Popup += delegate {
                    viewMenu.MenuItems[0].Checked = F.Data.HAHVisible;
                    viewMenu.MenuItems[2].Checked = F.Data.MultiVisible;
                    viewMenu.MenuItems[3].Checked = F.Data.EditorVisible;
                };
                return viewMenu;
            }
        }

        private MenuItem MultiMenu {
            get {
                MenuItem editorMenu = new MenuItem("マルチモニター", new MenuItem[] {
                    new RadioMenuItem("大きさ: 中", delegate {
                        F.Data.MultiHeight = 60;
                        Modify(3);
                    }),
                    new RadioMenuItem("大きさ: 大", delegate {
                        F.Data.MultiHeight = 120;
                        Modify(3);
                    }),
                });
                editorMenu.Popup += delegate {
                    int i;
                    for(i = 0; i < 2; ++i)
                        editorMenu.MenuItems[i].Checked = (i + 1) * 60 == F.Multi.Height;
                };
                return editorMenu;
            }
        }

        private MenuItem EditorMenu {
            get {
                MenuItem editorMenu = new MenuItem("エディタ", new MenuItem[] {
                    new RadioMenuItem("大きさ: 中", delegate {
                        F.Data.EditorHeight = 200;
                        Modify(3);
                    }),
                    new RadioMenuItem("大きさ: 大", delegate {
                        F.Data.EditorHeight = 250;
                        Modify(3);
                    }),
                    new MenuItem("-"),
                    new RadioMenuItem("ボーダーの幅: 小", delegate {
                        F.Data.EditorBorderWidth = 6;
                        Modify(3);
                    }),
                    new RadioMenuItem("ボーダーの幅: 中", delegate {
                        F.Data.EditorBorderWidth = 8;
                        Modify(3);
                    }),
                    new RadioMenuItem("ボーダーの幅: 大", delegate {
                        F.Data.EditorBorderWidth = 10;
                        Modify(3);
                    }),
                });
                editorMenu.Popup += delegate {
                    int i;
                    for(i = 0; i < 2; ++i)
                        editorMenu.MenuItems[i].Checked = i * 50 + 200 == F.Editor.Height;
                    for(i = 0; i < 3; ++i)
                        editorMenu.MenuItems[i + 3].Checked = i * 2 + 6 == F.Editor.BorderWidth;
                };
                return editorMenu;
            }
        }

        private MenuItem TaskMenu {
            get {
                int i;
                MenuItem taskMenu = new MenuItem("アプリの一覧", new MenuItem[] {
                    new MenuItem("自身を表示", delegate {
                        F.Data.SelfVisible = !F.Data.SelfVisible;
                        Modify(3);
                    }),
                    new MenuItem("-"),
                });
                for(i = 1; i <= 3; ++i) {
                    int p = i;
                    taskMenu.MenuItems.Add(new RadioMenuItem("行数: " + i, delegate {
                        F.Data.TaskLines = p;
                        Modify(3);
                    }));
                }
                taskMenu.Popup += delegate {
                    taskMenu.MenuItems[0].Checked = F.Data.SelfVisible;
                    for(i = 1; i <= 3; ++i)
                        taskMenu.MenuItems[i + 1].Checked = i == F.Task.Lines;
                };
                return taskMenu;
            }
        }

        private MenuItem TemplateMenu {
            get {
                MenuItem templateMenu = new MenuItem("テンプレート", new MenuItem[] {
                    new RadioMenuItem("大きさ: 中", delegate {
                        F.Data.TemplateArea = 128 * 64;
                        Modify(3);
                    }),
                    new RadioMenuItem("大きさ: 大", delegate {
                        F.Data.TemplateArea = 160 * 80;
                        Modify(3);
                    }),
                });
                templateMenu.Popup += delegate {
                    int i;
                    for(i = 0; i < 2; ++i)
                        templateMenu.MenuItems[i].Checked = F.Template.Area == ((i * 32) + 128) * ((i * 16) + 64);
                };
                return templateMenu;
            }
        }

        private MenuItem OptionMenu {
            get {
                MenuItem optionMenu = new MenuItem("オプション(&O)", new MenuItem[] {
                    new MenuItem("常駐", delegate {
                        F.Data.Resident = !F.Data.Resident;
                        ni.Visible = F.Data.Resident;
                    }),
                    new MenuItem("スタートアップ", delegate {
                        if(File.Exists(F.StartupPath)) {
                            try {
                                File.Delete(F.StartupPath);
                            } catch(UnauthorizedAccessException) {
                            }
                        } else {
                            var s = (new IWshRuntimeLibrary.WshShell()).CreateShortcut(F.StartupPath);
                            s.IconLocation = Application.ExecutablePath + ",0";
                            s.TargetPath = Application.ExecutablePath;
                            s.WindowStyle = 7;
                            try {
                                s.Save();
                            } catch(UnauthorizedAccessException) {
                            } finally {
                                Marshal.ReleaseComObject(s);
                            }
                        }
                    }),
                    new MenuItem("-"),
                    new MenuItem("フィルター(&F)", delegate {
                        F.Data.Filters = !F.Data.Filters;
                        Modify(3);
                    }),
                    new MenuItem("フィルターをクリア", delegate {
                        Action.Push(new FilterAction(this));
                        F.Data.FilteredClassNames.Clear();
                        Modify(3);
                    }),
                    new MenuItem("-"),
                    new MenuItem("&Hit a Hint...", delegate {
                        using(HAHOption f = new HAHOption())
                            f.ShowDialog(this);
                    }),
                });
                optionMenu.Popup += delegate {
                    optionMenu.MenuItems[0].Checked = F.Data.Resident;
                    optionMenu.MenuItems[1].Checked = File.Exists(F.StartupPath);
                    optionMenu.MenuItems[3].Checked = F.Data.Filters;
                };
                return optionMenu;
            }
        }
    }
}
