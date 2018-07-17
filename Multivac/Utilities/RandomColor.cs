using System;

namespace Multivac.Utilities
{
    public class RandomColor
    {
        public static uint NoGrays()
        {   // returns a random int value representing a color
            double H = RNG.RandomNumber(0, 359);
            double S = (RNG.RandomNumber(40, 100) / 100.0);
            double V = (RNG.RandomNumber(80, 100) / 100.0); //1.0

            return (uint)HSVtoRGBint(H, S, V);

        } // end

        public static uint AllColors()
        {
            return (uint)RNG.RandomNumber(0, 16777215);
        }

        public static int HSVtoRGBint(double H, double S, double V)
        {
            int RGBint;
            byte red, green, blue;
            double r1, g1, b1;

            double C = V * S;
            double X = C * (1 - Math.Abs((H / 60) % 2 - 1));
            double m = V - C;

            if (0 <= H && H < 60)
            {
                r1 = C;
                g1 = X;
                b1 = 0;
            }
            else if (60 <= H && H < 120)
            {
                r1 = X;
                g1 = C;
                b1 = 0;
            }
            else if (120 <= H && H < 180)
            {
                r1 = 0;
                g1 = C;
                b1 = X;
            }
            else if (180 <= H && H < 240)
            {
                r1 = 0;
                g1 = X;
                b1 = C;
            }
            else if (240 <= H && H < 300)
            {
                r1 = X;
                g1 = 0;
                b1 = C;
            }
            else if (300 <= H && H < 360)
            {
                r1 = C;
                g1 = 0;
                b1 = X;
            }
            else
            {
                r1 = 0;
                g1 = 0;
                b1 = 0;
            }

            red = (byte) Math.Round((r1 + m) * 255);
            green = (byte) Math.Round((g1 + m) * 255);
            blue = (byte) Math.Round((b1 + m) * 255);

            RGBint = ((red * 65536) + (green * 256) + blue);

            return RGBint;
        }
    }
}
