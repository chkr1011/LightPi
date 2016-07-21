using System;
using System.Windows.Threading;

namespace LightPi.OrchestratorEmulator
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();


            Surface.RegisterBackgroundSprite(@".\Sprites\Background.jpg");

            for (int i = 0; i < 48; i++)
            {
                Surface.RegisterOutputSprite(i, @".\Sprites\0.png");
            }

            var t = new DispatcherTimer(DispatcherPriority.Render);
            t.Interval = TimeSpan.FromMilliseconds(100);
            t.Tick += T_Tick;
            t.Start();
        }

        private bool s;

        private void T_Tick(object sender, EventArgs e)
        {
            Surface.SetOutputState(0, s);

            s = !s;

            Surface.Update();
        }
    }
}
