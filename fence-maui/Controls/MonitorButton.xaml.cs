using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fence_maui.Controls
{
    public partial class MonitorButton : Frame
    {
        public MonitorButton( FenceHostServer.Monitor monitor )
        {
            MonitorWidth = monitor.Width;
            MonitorHeight = monitor.Height;
            MonitorTop = monitor.Top;
            MonitorLeft = monitor.Left;

            InitializeComponent();
        }

        public double MonitorWidth
        {
            get => mWidth;
            set
            {
                mWidth = value;
                OnPropertyChanged();
            }
        }

        public double MonitorHeight
        {
            get => mHeight;
            set
            {
                mHeight = value;
                OnPropertyChanged();
            }
        }

        public double MonitorTop
        {
            get => mTop;
            set
            {
                mTop = value;
                OnPropertyChanged();
            }
        }

        public double MonitorLeft
        {
            get => mLeft;
            set
            {
                mLeft = value;
                OnPropertyChanged();
            }
        }

        private double mWidth;
        private double mHeight;
        private double mTop;
        private double mLeft;
    }
}