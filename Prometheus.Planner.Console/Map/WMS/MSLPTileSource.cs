using Esri.ArcGISRuntime.Geometry;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;
using Prometeo.Planner.Console.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Prometeo.Planner.Console.Map
{
    public class MSLPTileSource : Microsoft.Maps.MapControl.WPF.TileSource
    {
        static bool _mapServerNotResponding;
        string _url;

        public MSLPTileSource()
        {
            _url = ConfigurationManager.AppSettings["mslpProviderUrl"].ToString();
            this.DirectImage = new ImageCallback(TileRender);
        }

        public Uri GetUriPath(int x, int y, int zoomLevel)
        {
            var uri = new Uri(String.Format(_url, XYZoomToBBox(x, y, zoomLevel)), UriKind.Absolute);
            Trace.WriteLine(uri.ToString());
            return uri;
        }

        public BitmapImage TileRender(long x, long y, int zoomLevel)
        {
            var bbox = XYZoomToBBox((int) x, (int) y, zoomLevel);
            if (string.IsNullOrEmpty(bbox))
                return CreateTransparentTile();

            if (!_mapServerNotResponding)
            {
                var stream = RESTTools.DownloadImage(new Uri(String.Format(_url, bbox)), null);

                if (stream != null)
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = stream;
                    bi.EndInit();

                    return bi;
                }

                _mapServerNotResponding = true;
            }

            return CreateTransparentTile();
        }

        private BitmapImage CreateTransparentTile()
        {
            int width = 256;
            int height = 256;

            var source = BitmapSource.Create(width, height,
                                            96, 96,
                                            PixelFormats.Indexed1,
                                            new BitmapPalette(new List<Color>(){
                                                Colors.Transparent
                                            }),
                                            new byte[width * height],
                                            width);

            var image = new BitmapImage();

            var memoryStream = new MemoryStream();

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;

            image.BeginInit();
            image.StreamSource = memoryStream;
            image.EndInit();

            return image;
        }

        public string XYZoomToBBox(int x, int y, int zoom)
        {
            MapPoint pp1 = FromTileXYZToMercator(x, y + 1, zoom);
            MapPoint pp2 = FromTileXYZToMercator(x + 1, y, zoom);

            if (pp1 == null || pp2 == null)
                return null;

            int TILE_HEIGHT = 256, TILE_WIDTH = 256;
            // From the grid position and zoom, work out the min and max Latitude / Longitude values of this tile
            double W = (float)(x * TILE_WIDTH) * 360 / (float)(TILE_WIDTH * Math.Pow(2, zoom)) - 180;
            double N = (float)Math.Asin((Math.Exp((0.5 - (y * TILE_HEIGHT) / (TILE_HEIGHT) / Math.Pow(2, zoom)) * 4 * Math.PI) - 1) / (Math.Exp((0.5 - (y * TILE_HEIGHT) / 256 / Math.Pow(2, zoom)) * 4 * Math.PI) + 1)) * 180 / (float)Math.PI;
            double E = (float)((x + 1) * TILE_WIDTH) * 360 / (float)(TILE_WIDTH * Math.Pow(2, zoom)) - 180;
            double S = (float)Math.Asin((Math.Exp((0.5 - ((y + 1) * TILE_HEIGHT) / (TILE_HEIGHT) / Math.Pow(2, zoom)) * 4 * Math.PI) - 1) / (Math.Exp((0.5 - ((y + 1) * TILE_HEIGHT) / 256 / Math.Pow(2, zoom)) * 4 * Math.PI) + 1)) * 180 / (float)Math.PI;

            //MapPoint p1 = new MapPoint(W, E, SpatialReferences.Wgs84);
            //MapPoint p2 = new MapPoint(S, N, SpatialReferences.Wgs84);

            //MapPoint pp1 = (MapPoint)GeometryEngine.Project(p1, SpatialReferences.WebMercator);
            //MapPoint pp2 = (MapPoint)GeometryEngine.Project(p2, SpatialReferences.WebMercator);

            string[] bounds = new string[] { pp1.X.ToString(), pp1.Y.ToString(), pp2.X.ToString(), pp2.Y.ToString() };

            //string[] bounds = new string[] { W.ToString(), S.ToString(), E.ToString(), N.ToString() };
            // Return a comma-separated string of the bounding coordinates
            return string.Join(",", bounds);
        }

        private MapPoint FromTileXYZToMercator(int x, int y, int zoom)
        {
            var quadkey = new QuadKey(x, y, zoom);
            if (quadkey.Key == null)
                return null;

            int tileX, tileY, levelOfDetail, pixelX, pixelY;
            double lat, longit;

            QuadKeyToTileXY(quadkey.Key, out tileX, out tileY, out levelOfDetail);
            TileXYToPixelXY(tileX, tileY, out pixelX, out pixelY);
            PixelXYToLatLong(pixelX, pixelY, levelOfDetail, out lat, out longit);

            var p1 = new MapPoint(longit, lat, SpatialReferences.Wgs84);
            return (MapPoint)GeometryEngine.Project(p1, SpatialReferences.WebMercator);
        }

        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        /// <summary>
        /// Determines the map width and height (in pixels) at a specified level
        /// of detail.
        /// </summary>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The map width and height in pixels.</returns>
        public static uint MapSize(int levelOfDetail)
        {
            return (uint)256 << levelOfDetail;
        }

        public static void QuadKeyToTileXY(string quadKey, out int tileX, out int tileY, out int levelOfDetail)
        {
            tileX = tileY = 0;
            levelOfDetail = quadKey.Length;
            for (int i = levelOfDetail; i > 0; i--)
            {
                int mask = 1 << (i - 1);
                switch (quadKey[levelOfDetail - i])
                {
                    case '0':
                        break;

                    case '1':
                        tileX |= mask;
                        break;

                    case '2':
                        tileY |= mask;
                        break;

                    case '3':
                        tileX |= mask;
                        tileY |= mask;
                        break;

                    default:
                        throw new ArgumentException("Invalid QuadKey digit sequence.");
                }
            }
        }

        /// <summary>
        /// Converts tile XY coordinates into pixel XY coordinates of the upper-left pixel
        /// of the specified tile.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="pixelX">Output parameter receiving the pixel X coordinate.</param>
        /// <param name="pixelY">Output parameter receiving the pixel Y coordinate.</param>
        public static void TileXYToPixelXY(int tileX, int tileY, out int pixelX, out int pixelY)
        {
            pixelX = tileX * 256;
            pixelY = tileY * 256;
        }

        /// <summary>
        /// Converts a pixel from pixel XY coordinates at a specified level of detail
        /// into latitude/longitude WGS-84 coordinates (in degrees).
        /// </summary>
        /// <param name="pixelX">X coordinate of the point, in pixels.</param>
        /// <param name="pixelY">Y coordinates of the point, in pixels.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <param name="latitude">Output parameter receiving the latitude in degrees.</param>
        /// <param name="longitude">Output parameter receiving the longitude in degrees.</param>
        public static void PixelXYToLatLong(int pixelX, int pixelY, int levelOfDetail, out double latitude, out double longitude)
        {
            double mapSize = MapSize(levelOfDetail);
            double x = (Clip(pixelX, 0, mapSize - 1) / mapSize) - 0.5;
            double y = 0.5 - (Clip(pixelY, 0, mapSize - 1) / mapSize);

            latitude = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;
            longitude = 360 * x;
        }
    }
}
