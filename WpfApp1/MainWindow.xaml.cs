using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlobalHotKey;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        DateTime today5;
        double totalSeconds, current;
        HotKeyManager hotKeyManager = new HotKeyManager();
        int jyaH = 17, jyaM = 0;
        NotifyIcon ni;

        private System.Windows.Forms.ContextMenu contextMenu;

        public MainWindow()
        {
            InitializeComponent();

            this.Left = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            int theight = Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom;
            this.Top = SystemParameters.PrimaryScreenHeight - (theight + 2);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();

            var hotKey = hotKeyManager.Register(Key.R, ModifierKeys.Shift | ModifierKeys.Alt);
            hotKeyManager.KeyPressed += reset_h;

            this.ShowInTaskbar = false;

            this.contextMenu = new System.Windows.Forms.ContextMenu();
            var exit = new System.Windows.Forms.MenuItem();
            var settime = new System.Windows.Forms.MenuItem();
            var resettime = new System.Windows.Forms.MenuItem();
            var version = new System.Windows.Forms.MenuItem();


            ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("xixi.ico");
            ni.Visible = true;

            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { version, resettime, settime, exit });

            
            exit.Text = "退出";
            exit.Click += new System.EventHandler(delegate (object sender, EventArgs args){
                OnExit();
            });

            
            settime.Text = "设置告辞时间";
            settime.Click += new System.EventHandler(delegate (object sender, EventArgs args) {
                string h = Microsoft.VisualBasic.Interaction.InputBox("几点告辞？（小时）", "haha", "17", 0, 0);
                if (h == null || h.Length == 0 || !Regex.IsMatch(h, @"^\d+$")) return;

                string m = Microsoft.VisualBasic.Interaction.InputBox("几点告辞？（分钟）", "haha", "00", 0, 0);
                if (m == null || m.Length == 0 || !Regex.IsMatch(m, @"^\d+$")) return;

                jyaH = Convert.ToInt32(h);
                jyaM = Convert.ToInt32(m);
                reset();
            });

            
            resettime.Text = "重置时间";
            resettime.Click += new System.EventHandler(delegate (object sender, EventArgs args) {
                reset();
            });

            
            version.Text = "[盼头]版本1.1";
            version.Enabled = false;

            ni.ContextMenu = contextMenu;

            reset();

        }

        private void OnExit()
        {
            ni.Dispose();
            System.Environment.Exit(1);
        }

        private void reset_h(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == Key.R)
                reset();
        }



        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
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


        private void reset()
        {
            today5 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, jyaH, jyaM, 0);
            var ts = today5.Subtract(DateTime.Now);
            totalSeconds = Convert.ToInt32(ts.TotalSeconds);

            current = 0;
        }

        
    }


}
