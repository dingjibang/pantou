using System;
using System.Text.RegularExpressions;

using System.Windows;

using System.Windows.Forms;
using System.Windows.Input;

using GlobalHotKey;

namespace WpfApp1 {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        DateTime today5;
        double totalSeconds, current;
        HotKeyManager hotKeyManager = new HotKeyManager();
        int jyaH, jyaM;
        NotifyIcon ni;

        new System.Windows.Forms.MenuItem version;

        private System.Windows.Forms.ContextMenu contextMenu;

        public MainWindow() {
            InitializeComponent();

            readConfig();

            this.Left = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            int theight = Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom;
            this.Top = SystemParameters.PrimaryScreenHeight - (theight + 2);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();

            var hotKey = hotKeyManager.Register(Key.R, ModifierKeys.Shift | ModifierKeys.Alt);
            hotKeyManager.KeyPressed += reset_del;

            this.ShowInTaskbar = false;

            this.contextMenu = new ContextMenu();
            var exit = new MenuItem();
            var settime = new MenuItem();
            var resettime = new MenuItem();
            version = new MenuItem();


            ni = new NotifyIcon();
            ni.Icon = new System.Drawing.Icon("xixi.ico");
            ni.Visible = true;

            this.contextMenu.MenuItems.AddRange(new MenuItem[] { version, resettime, settime, exit });


            exit.Text = "退出";
            exit.Click += new System.EventHandler(delegate (object sender, EventArgs args) {
                OnExit();
            });


            settime.Text = "设置告辞时间";
            settime.Click += new System.EventHandler(delegate (object sender, EventArgs args) {
                string h = Microsoft.VisualBasic.Interaction.InputBox("几点告辞？（小时，24小时制）", "haha", jyaH + "", 0, 0);
                if (h == null || h.Length == 0 || !Regex.IsMatch(h, @"^\d+$")) return;

                string m = Microsoft.VisualBasic.Interaction.InputBox("几点告辞？（分钟）", "haha", jyaM + "", 0, 0);
                if (m == null || m.Length == 0 || !Regex.IsMatch(m, @"^\d+$")) return;

                jyaH = Convert.ToInt32(h);
                jyaM = Convert.ToInt32(m);
                reset();
            });


            resettime.Text = "重置时间";
            resettime.Click += new System.EventHandler(delegate (object sender, EventArgs args) {
                reset();
            });


            version.Enabled = false;

            ni.ContextMenu = contextMenu;

            reset();

        }

        private void readConfig() {
            try {
                var cfg = System.IO.File.ReadAllText(@"config.cfg").Split(',');
                jyaH = Convert.ToInt32(cfg[0]);
                jyaM = Convert.ToInt32(cfg[1]);
            } catch (Exception) {
                jyaM = 0;
                jyaH = 17;
                saveConfig();
            }
        }

        private void saveConfig() {
            System.IO.File.WriteAllText(@"config.cfg", $"{jyaH},{jyaM}");
        }

        private string versionText() {
            Func<int, string> p = s => s.ToString().Length == 1 ? "0" + s : s.ToString();
            return $"[盼头]版本1.1，告辞时间为每天{p(jyaH)}:{p(jyaM)}";
        }

        private void OnExit() {
            ni.Dispose();
            System.Environment.Exit(1);
        }

        private void reset_del(object sender, KeyPressedEventArgs e) {
            if (e.HotKey.Key == Key.R)
                reset();
        }



        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            current += 0.1;

            double p = current / totalSeconds;
            p *= 100;

            if (p < 0)
                p = 0;
            if (p > 100)
                p = 100;

            CLeft.Width = new GridLength(p, GridUnitType.Star);
            CRight.Width = new GridLength(100 - p, GridUnitType.Star);
        }


        private void reset() {
            saveConfig();
            version.Text = versionText();

            today5 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, jyaH, jyaM, 0);
            var ts = today5.Subtract(DateTime.Now);
            totalSeconds = Convert.ToInt32(ts.TotalSeconds);

            current = 0;
        }


    }


}
