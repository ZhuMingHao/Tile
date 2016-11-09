using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Diagnostics;




// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TitleLocationMap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            //  AddLocalMapTileSource();
            // LoadMap();
            AddMap();
        }
        private void AddLocalMapTileSource()
        {
            MapZoomLevelRange range;
            range.Min = 4;
            range.Max = 7;
            LocalMapTileDataSource dataSource = new LocalMapTileDataSource("/tiles/4/1/1.png");
            MapTileSource tileSource = new MapTileSource(dataSource);
            tileSource.ZoomLevelRange = range;
            MyMapControl.TileSources.Add(tileSource);
            string path = ApplicationData.Current.LocalFolder.Path;
        }

        private void LoadMap()
        {
            CustomMapTileDataSource customDataSource = new CustomMapTileDataSource();
            // Attach a handler for the BitmapRequested event.
            customDataSource.BitmapRequested += customDataSource_BitmapRequestedAsync;
            MapTileSource customTileSource = new MapTileSource(customDataSource);
            MyMapControl.TileSources.Add(customTileSource);
        }

        // Handle the BitmapRequested event.
        private async void customDataSource_BitmapRequestedAsync(
            CustomMapTileDataSource sender,
            MapTileBitmapRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            args.Request.PixelData = await CreateBitmapAsStreamAsync();
            deferral.Complete();
        }

        // Create the custom tiles.
        // This example creates red tiles that are partially opaque.
        private async Task<RandomAccessStreamReference> CreateBitmapAsStreamAsync()
        {
            int pixelHeight = 256;
            int pixelWidth = 256;
            int bpp = 4;

            byte[] bytes = new byte[pixelHeight * pixelWidth * bpp];

            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int pixelIndex = y * pixelWidth + x;
                    int byteIndex = pixelIndex * bpp;

                    // Set the current pixel bytes.
                    bytes[byteIndex] = 0xff;        // Red
                    bytes[byteIndex + 1] = 0x00;    // Green
                    bytes[byteIndex + 2] = 0x00;    // Blue
                    bytes[byteIndex + 3] = 0x80;    // Alpha (0xff = fully opaque)
                }
            }

            // Create RandomAccessStream from byte array.
            InMemoryRandomAccessStream randomAccessStream =
                new InMemoryRandomAccessStream();
            IOutputStream outputStream = randomAccessStream.GetOutputStreamAt(0);
            DataWriter writer = new DataWriter(outputStream);
            writer.WriteBytes(bytes);
            await writer.StoreAsync();
            await writer.FlushAsync();
            return RandomAccessStreamReference.CreateFromStream(randomAccessStream);
        }

        private void AddMap()
        {
            var httpTileDataSource = new HttpMapTileDataSource();
            // Attach a handler for the UriRequested event.
            httpTileDataSource.UriRequested += HandleUriRequestAsync;
            MapTileSource httpTileSource = new MapTileSource(httpTileDataSource);
            MyMapControl.TileSources.Add(httpTileSource);

        }

        private async void HandleUriRequestAsync(HttpMapTileDataSource sender,
           MapTileUriRequestedEventArgs args)
        {
            // Get a deferral to do something asynchronously.
            // Omit this line if you don't have to do something asynchronously.
            var deferral = args.Request.GetDeferral();
            // Get the custom Uri.
            var uri = await GetCustomUriAsync(args.X, args.Y, args.ZoomLevel);
            //Debug.Write(args.X+"---" + args.Y + "---" + args.ZoomLevel);
            // Specify the Uri in the Uri property of the MapTileUriRequest.
            args.Request.Uri = uri;
            // Notify the app that the custom Uri is ready.
            // Omit this line also if you don't have to do something asynchronously.
            deferral.Complete();
        }
        // Create the custom Uri.
        private async Task<Uri> GetCustomUriAsync(int x, int y, int zoomLevel)
        {
            string[] j = { "http://online0.map.bdimg.com/tile/",
            "http://online1.map.bdimg.com/tile/",
            "http://online2.map.bdimg.com/tile/",
            "http://online3.map.bdimg.com/tile/",
            "http://online4.map.bdimg.com/tile/" };
            string udt = "20161103";
            string style = "pl";
            string scaler = "1";
            var cM = j[Math.Abs(x + y) % j.Length] + "?qt=tile&x="
        + (x+"").Replace("","M") + "&y="
        + (y+"" ).Replace("","M")+ "&z=" + zoomLevel + "&styles=" + style + "&scaler=" + scaler
        + "&udt=" + udt;
            return await Task.Run(() => new Uri(cM));
        }
    }
}
