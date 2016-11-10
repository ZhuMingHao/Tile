using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitleLocationMap
{
    public class TileManager
    {

        public class mercator
        {
            public double me_x;
            public double me_y;

        };
        public class px
        {
            public double px_x;
            public double px_y;
        };
        public class tile
        {
            public int x;
            public int y;
            public int z;

        };
        double TILE_PIXEL_UNIT = 256.0;
        double EARTH_RADIUS = 6370996.81;
        double[] MCBAND = { 12890594.86, 8362377.87, 5591021, 3481989.83, 1678043.12, 0 };
        int[] LLBAND = { 75, 60, 45, 30, 15, 0 };
        private static double[][] LL2MC = {new double[]{-0.0015702102444, 111320.7020616939, 1704480524535203, -10338987376042340, 26112667856603880, -35149669176653700, 26595700718403920, -10725012454188240, 1800819912950474, 82.5}
                                               ,new double[]{0.0008277824516172526, 111320.7020463578, 647795574.6671607, -4082003173.641316, 10774905663.51142, -15171875531.51559, 12053065338.62167, -5124939663.577472, 913311935.9512032, 67.5}
                                               ,new double[]{0.00337398766765, 111320.7020202162, 4481351.045890365, -23393751.19931662, 79682215.47186455, -115964993.2797253, 97236711.15602145, -43661946.33752821, 8477230.501135234, 52.5}
                                               ,new double[]{0.00220636496208, 111320.7020209128, 51751.86112841131, 3796837.749470245, 992013.7397791013, -1221952.21711287, 1340652.697009075, -620943.6990984312, 144416.9293806241, 37.5}
                                               ,new double[]{-0.0003441963504368392, 111320.7020576856, 278.2353980772752, 2485758.690035394, 6070.750963243378, 54821.18345352118, 9540.606633304236, -2710.55326746645, 1405.483844121726, 22.5}
                                               ,new double[]{-0.0003218135878613132, 111320.7020701615, 0.00369383431289, 823725.6402795718, 0.46104986909093, 2351.343141331292, 1.58060784298199, 8.77738589078284, 0.37238884252424, 7.45}};
        private static double[][] MC2LL = {new double[]{1.410526172116255e-8, 0.00000898305509648872, -1.9939833816331, 200.9824383106796, -187.2403703815547, 91.6087516669843, -23.38765649603339, 2.57121317296198, -0.03801003308653, 17337981.2}
                                               ,new double[]{-7.435856389565537e-9, 0.000008983055097726239, -0.78625201886289, 96.32687599759846, -1.85204757529826, -59.36935905485877, 47.40033549296737, -16.50741931063887, 2.28786674699375, 10260144.86}
                                               ,new double[]{-3.030883460898826e-8, 0.00000898305509983578, 0.30071316287616, 59.74293618442277, 7.357984074871, -25.38371002664745, 13.45380521110908, -3.29883767235584, 0.32710905363475, 6856817.37}
                                               ,new double[]{-1.981981304930552e-8, 0.000008983055099779535, 0.03278182852591, 40.31678527705744, 0.65659298677277, -4.44255534477492, 0.85341911805263, 0.12923347998204, -0.04625736007561, 4482777.06}
                                               ,new double[]{3.09191371068437e-9, 0.000008983055096812155, 0.00006995724062, 23.10934304144901, -0.00023663490511, -0.6321817810242, -0.00663494467273, 0.03430082397953, -0.00466043876332, 2555164.4}
                                               ,new double[]{2.890871144776878e-9, 0.000008983055095805407, -3.068298e-8, 7.47137025468032, -0.00000353937994, -0.02145144861037, -0.00001234426596, 0.00010322952773, -0.00000323890364, 826088.5}};


        private mercator convert_coord_to_mercator(double lng, double lat)
        {
            double[] ll2mc = null;
            for (var i = 0; i < LLBAND.Length; i++)
            {
                if (lat >= LLBAND[i])
                {
                    ll2mc = LL2MC[i];
                    break;
                }

            }
            if (ll2mc == null)
            {
                for (var i = LLBAND.Length - 1; i >= 0; i--)
                {
                    if (lat <= -LLBAND[i])
                    {
                        ll2mc = LL2MC[i];
                        break;
                    }

                }
            }

            double x = ll2mc[0] + ll2mc[1] * Math.Abs(lng);
            double a = Math.Abs(lat) / ll2mc[9];
            double y = ll2mc[2] + ll2mc[3] * a + ll2mc[4] * Math.Pow(a, 2) + ll2mc[5] * Math.Pow(a, 3);
            y += ll2mc[6] * Math.Pow(a, 4) + ll2mc[7] * Math.Pow(a, 5) + ll2mc[8] * Math.Pow(a, 6);
            x *= lng > 0 ? 1 : -1;
            y *= lat > 0 ? 1 : -1;
            return new mercator() { me_x = x, me_y = y };

        }

        private px convert_mercator_to_px(double x, double y, double z)
        {
            double x_px = Math.Floor(x * Math.Pow(2, z - 18));
            double y_px = Math.Floor(y * Math.Pow(2, z - 18));
            return new px() { px_x = x_px, px_y = y_px };
        }

        private tile convert_px_to_tile(double x_px, double y_px, int z)
        {
            int x_tile = (int)(Math.Floor(x_px / TILE_PIXEL_UNIT));
            int y_tile = (int)(Math.Floor(y_px / TILE_PIXEL_UNIT));
            return new tile() { x = x_tile, y = y_tile, z = z };

        }
        public tile getTile(double lng, double lat, int z)
        {
            mercator point = convert_coord_to_mercator(lng, lat);
            px px = convert_mercator_to_px(point.me_x, point.me_y, z);
            tile Tile = convert_px_to_tile(px.px_x, px.px_y, z);
            return Tile;
        }

        public tile convert_bingTile_to_baidu(int x, int y, int z)
        {
            var quadkey = tileToQuadkey(x, y, z);
            var merca = BingTest(quadkey);
            tile tie = getTileFromMercator(merca, z);
            return tie;

        }

        public tile getTileFromMercator(mercator me, int z)
        {
            px px = convert_mercator_to_px(me.me_x, me.me_y, z);
            tile Tile = convert_px_to_tile(px.px_x, px.px_y, z);
            return Tile;
        }


        private string tileToQuadkey(int x_tile, int y_tile, int z_tile)
        {

            string index = "";

            for (var z = z_tile; z > 0; z--)
            {

                var b = 0;

                var mask = 1 << (z - 1);

                if ((x_tile & mask) != 0) b++;

                if ((y_tile & mask) != 0) b += 2;

                index += b.ToString();

            }

            return index;

        }
        private mercator BingTest(string qdkey)
        {
            const double EarthRadius = 6378137;
            //string qdkey = "03200320023";
            double x = 0;
            double y = 0;
            double offset = EarthRadius * Math.PI / 2.0;
            Debug.WriteLine("quadkey: {0}", qdkey);
            for (int i = 0; i < qdkey.Length; i++)
            {
                string s = qdkey.Substring(i, 1);
                //  Debug.Write("{0} offset: {1} lod {2}", s, offset, i);
                switch (s)
                {
                    case "0": x -= offset; y += offset; break;
                    case "1": x += offset; y += offset; break;
                    case "2": x -= offset; y -= offset; break;
                    case "3": x += offset; y -= offset; break;
                    default: throw new Exception("bad quadkey");
                }
                offset *= 0.5;
            }

            // compare with expected result
            //double x0 = -9373014;
            //  double y0 = 4011415;
            return new mercator() { me_x = x, me_y = y };
            //Debug.WriteLine("Mercator: x {0}, y {1}", x, y);
            //  Debug.WriteLine("error x {0} y{1}", x - x0, y - y0);
        }

    }
}
