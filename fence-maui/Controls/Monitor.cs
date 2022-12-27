namespace fence_maui.Controls
{
    public class Monitor : ContentView
    {
        public Monitor( FenceHostServer.Monitor monitor )
        {
            Width = monitor.Width;
            Height = monitor.Height;
            Top = monitor.Top;
            Left = monitor.Left;
        }

        public double Width
        {
            get => mWidth;
            set
            {
                mWidth = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => mHeight;
            set
            {
                mHeight = value;
                OnPropertyChanged();
            }
        }

        public double Top
        {
            get => mTop;
            set
            {
                mTop = value;
                OnPropertyChanged();
            }
        }

        public double Left
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