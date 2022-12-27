using FenceHostServer;
using Grpc.Core;

namespace fence_maui.Controls
{
    public class Cursor : ContentView
    {
        public Cursor( IAsyncStreamReader<CursorLocation> reader, double topOffset, double leftOffset, double factor )
        {
            Task.Run( async () =>
            {
                while( await reader.MoveNext( CancellationToken.None ) )
                {
                    // Console.WriteLine( reader.Current );
                    Dispatcher.Dispatch( () =>
                    {
                        AbsoluteLayout.SetLayoutBounds(
                            this,
                            new Rect(
                                ( reader.Current.X - leftOffset ) * factor - ( ELLIPSE_WIDTH / 2 ),
                                ( reader.Current.Y - topOffset ) * factor - ( ELLIPSE_HEIGHT / 2 ),
                                ELLIPSE_WIDTH,
                                ELLIPSE_HEIGHT ) );
                    } );
                }
            } );
        }

        private const double ELLIPSE_WIDTH = 10;
        private const double ELLIPSE_HEIGHT = 10;
    }
}