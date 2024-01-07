using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace astronomy
{
    internal class DailyScheduleCalculator
    {
        public StringBuilder stringOutput = new();
        public int Day {  get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Duration { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int TimeZone {  get; set; }

        public DailyScheduleCalculator(int day, int month, int year, int duration, double longitude, double latitude, int timeZone = 0)
        {
            Day = day;
            Month = month;
            Year = year;
            Duration = duration;
            Longitude = longitude;
            Latitude = latitude;
            TimeZone = timeZone;
        }

        public double Frac(double x)
        {
            var decimalPart = x - Math.Floor(x);
            return decimalPart < 0 ? decimalPart + 1 : decimalPart;
        }

        public double Round(double num, double dp) {
            return Math.Round(num * Math.Pow(10, dp)) / Math.Pow(10, dp);
        }

        public double Ipart(double x) {
            if (x > 0) return Math.Floor(x);
            return Math.Ceiling(x);
        }

        public double Range(double x) {
            double b = x / 360;
            double a = 360 * (b - Ipart(b));
            return a < 0 ? a + 360 : a;
        }

        public string HrsMin(double hours) {
            var hrs = Math.Floor(hours * 60 + 0.5) / 60.0;
            var h = Math.Floor(hrs);
            var m = Math.Floor(60 * (hrs - h) + 0.5);
            var dum = h * 100 + m;

            var str = "";
            if (dum < 1000) str = "0" + dum.ToString();
            if (dum < 100) str = "0" + dum.ToString();
            if (dum < 10) str = "0" + dum.ToString();

            return str;
        }

        public double Mjd()
        {
            if (Month <= 2)
            {
                Month += 12;
                Year -= 1;
            }

            double a = 10000.0 * Year + 100.0 * Month + Day;

            double b;
            if (a <= 15821004.1)
            {
                b = -2 * Math.Floor((Year + 4716.0) / 4) - 1179;
            }
            else
            {
                b = Math.Floor(Year / 400.0) - Math.Floor(Year / 100.0) + Math.Floor(Year / 4.0);
            }
            a = 365.0 * Year - 679004.0;

            return a + b + Math.Floor(30.6001 * (Month + 1)) + Day + 0.0 / 24.0;
        }

        public double CalDat(double mjd)
        {
            var jd = mjd + 2400000.5;
            var jd0 = Math.Floor(jd + 0.5);

            double c;
            if (jd0 < 2299161.0) c = jd0 + 1524.0;
            else
            {
                var b = Math.Floor((jd0 - 1867216.25) / 36524.25);
                c = jd0 + (b - Math.Floor(b / 4)) + 1525.0;
            }
            var d = Math.Floor((c - 122.1) / 365.25);
            var e = 365.0 * d + Math.Floor(d / 4);
            var f = Math.Floor((c - e) / 30.6001);
            var day = Math.Floor(c - e + 0.5) - Math.Floor(30.6001 * f);
            var month = f - 1 - 12 * Math.Floor(f / 14);
            var year = d - 4715 - Math.Floor((7 + month) / 10);
            var hour = 24.0 * (jd + 0.5 - jd0);
            hour = Convert.ToDouble(HrsMin(hour));

            return Round(year * 10000.0 + month * 100.0 + day + hour / 10000, 4);
        }

        public double[] Quad(double ym, double yz, double yp)
        {
            var a = 0.5 * (ym + yp) - yz;
            var b = 0.5 * (yp - ym);
            var c = yz;
            var xe = -b / (2 * a);
            var ye = (a * xe + b) * xe + c;
            var dis = b * b - 4.0 * a * c;

            double z1 = 0, z2 = 0;
            int nz = 0;
            if (dis > 0)
            {
                var dx = (0.5 * Math.Sqrt(dis)) / Math.Abs(a);
                z1 = xe - dx;
                z2 = xe + dx;
                if (Math.Abs(z1) <= 1.0) nz += 1;
                if (Math.Abs(z2) <= 1.0) nz += 1;
                if (z1 < -1.0) z1 = z2;
            }

            return new double[] { nz, z1, z2, xe, ye };
        }

        public double Lmst(double mjd)
        {
            var d = mjd - 51544.5;
            var t = d / 36525.0;
            var lst = Range(
              280.46061837 +
                360.98564736629 * d +
                0.000387933 * t * t -
                (t * t * t) / 38710000
            );
            return lst / 15.0 + Longitude / 15;
        }

        public double[] MiniSun(double t)
        {
            var p2 = 6.283185307;
            var coseps = 0.91748;
            var sineps = 0.39778;
            double[] sunEq = new double[2];

            double M = p2 * Frac(0.993133 + 99.997361 * t);
            double DL = 6893.0 * Math.Sin(M) + 72.0 * Math.Sin(2 * M);
            double L = p2 * Frac(0.7859453 + M / p2 + (6191.2 * t + DL) / 1296000);
            double SL = Math.Sin(L);
            double X = Math.Cos(L);

            double Y = coseps * SL;
            double Z = sineps * SL;

            double RHO = Math.Sqrt(1 - Z * Z);
            double dec = (360.0 / p2) * Math.Atan(Z / RHO);
            double ra = (48.0 / p2) * Math.Atan(Y / (X + RHO));
            if (ra < 0) ra += 24;

            sunEq[0] = dec;
            sunEq[1] = ra;

            return sunEq;
        }

        public double[] MiniMoon(double t)
        {
            var p2 = 6.283185307;
            var arc = 206264.8062;
            var coseps = 0.91748;
            var sineps = 0.39778;

            var moonEq = new double[2];

            var L0 = Frac(0.606433 + 1336.855225 * t);
            var L = p2 * Frac(0.374897 + 1325.55241 * t);
            var LS = p2 * Frac(0.993133 + 99.997361 * t);
            var D = p2 * Frac(0.827361 + 1236.853086 * t);
            var F = p2 * Frac(0.259086 + 1342.227825 * t);

            var DL = 22640 * Math.Sin(L);
            DL += -4586 * Math.Sin(L - 2 * D);
            DL += +2370 * Math.Sin(2 * D);
            DL += +769 * Math.Sin(2 * L);
            DL += -668 * Math.Sin(LS);
            DL += -412 * Math.Sin(2 * F);
            DL += -212 * Math.Sin(2 * L - 2 * D);
            DL += -206 * Math.Sin(L + LS - 2 * D);
            DL += +192 * Math.Sin(L + 2 * D);
            DL += -165 * Math.Sin(LS - 2 * D);
            DL += -125 * Math.Sin(D);
            DL += -110 * Math.Sin(L + LS);
            DL += +148 * Math.Sin(L - LS);
            DL += -55 * Math.Sin(2 * F - 2 * D);

            var S = F + (DL + 412 * Math.Sin(2 * F) + 541 * Math.Sin(LS)) / arc;
            var H = F - 2 * D;

            var N = -526 * Math.Sin(H);
            N += +44 * Math.Sin(L + H);
            N += -31 * Math.Sin(-L + H);
            N += -23 * Math.Sin(LS + H);
            N += +11 * Math.Sin(-LS + H);
            N += -25 * Math.Sin(-2 * L + F);
            N += +21 * Math.Sin(-L + F);

            var LMoon = p2 * Frac(L0 + DL / 1296000);
            var BMoon = (18520.0 * Math.Sin(S) + N) / arc;
            var CB = Math.Cos(BMoon);
            var X = CB * Math.Cos(LMoon);
            var V = CB * Math.Sin(LMoon);
            var W = Math.Sin(BMoon);
            var Y = coseps * V - sineps * W;
            var Z = sineps * V + coseps * W;
            
            var RHO = Math.Sqrt(1.0 - Z * Z);
            var dec = (360.0 / p2) * Math.Atan(Z / RHO);
            var ra = (48.0 / p2) * Math.Atan(Y / (X + RHO));
            if (ra < 0) ra += 24;

            moonEq[0] = dec;
            moonEq[1] = ra;

            return moonEq;
        }

        public double SinAlt(double iobj, double mjd0, double hour, double cglat, double sglat)
        {
            var rads = 0.0174532925;
            double[] objPos;

            var mjd = mjd0 + hour / 24.0;
            var t = (mjd - 51544.5) / 36525.0;
            if (iobj == 1)
                objPos = MiniMoon(t);
            else
                objPos = MiniSun(t);

            Console.WriteLine("ObjPos: " + objPos[0] + " " + objPos[1]);
            var dec = objPos[0];
            var ra = objPos[1];

            var tau = 15.0 * (Lmst(mjd) - ra);

            var salt = sglat * Math.Sin(rads * dec) + cglat * Math.Cos(rads * dec) * Math.Cos(rads * tau);
            return salt;
        }

        public string FindSunAndTwiEventsForDate(double mjd)
        {
            var rads = 0.0174532925;
            var sinho = new double[4];
            var always_up = " ****";
            var always_down = " ....";
            var outstring = "";

            sinho[0] = Math.Sin(rads * -0.833); // sun
            sinho[1] = Math.Sin(rads * -6.0); // civic dusk
            sinho[2] = Math.Sin(rads * -12.0); // nautical dusk
            sinho[3] = Math.Sin(rads * -9.0); // METEOR

            var sglat = Math.Sin(rads * Latitude);
            var cglat = Math.Cos(rads * Latitude);
            var date = mjd - TimeZone / 24;

            for (int j = 0; j < 4; j++)
            {
                var rise = false;
                var sett = false;
                var above = false;
                var hour = 1.0;
                var ym = SinAlt(2, date, hour - 1.0, cglat, sglat) - sinho[j];
                if (ym > 0.0) above = true;

                double utrise = 0.0, utset = 0.0;
                while (hour < 25 && (sett == false || rise == false))
                {
                    var yz = SinAlt(2, date, hour, cglat, sglat) - sinho[j];
                    var yp = SinAlt(2, date, hour + 1.0, cglat, sglat) - sinho[j];
                    var quadout = Quad(ym, yz, yp);
                    var nz = quadout[0];
                    var z1 = quadout[1];
                    var z2 = quadout[2];
                    var xe = quadout[3];
                    var ye = quadout[4];

                    if (nz == 1)
                    {
                        if (ym < 0.0)
                        {
                            utrise = hour + z1;
                            rise = true;
                        }
                        else
                        {
                            utset = hour + z1;
                            sett = true;
                        }
                    }

                    if (nz == 2)
                    {
                        if (ye < 0.0)
                        {
                            utrise = hour + z2;
                            utset = hour + z1;
                        }
                        else
                        {
                            utrise = hour + z1;
                            utset = hour + z2;
                        }
                    }

                    ym = yp;
                    hour += 2.0;
                }

                if (rise == true || sett == true)
                {
                    if (rise == true) outstring += " " + HrsMin(utrise);
                    else outstring += " ----";
                    if (sett == true) outstring += " " + HrsMin(utset);
                    else outstring += " ----";
                }
                else
                {
                    if (above == true) outstring += always_up + always_up;
                    else outstring += always_down + always_down;
                }
            }

            return outstring;
        }

        public string FindMoonriseSet(double mjd)
        {
            var rads = 0.0174532925;
            var always_up = " ****";
            var always_down = " ....";
            var outstring = "";

            var sinho = Math.Sin((rads * 8) / 60);
            var sglat = Math.Sin(rads * Latitude);
            var cglat = Math.Cos(rads * Latitude);
            var date = mjd - TimeZone / 24;
            var rise = false;
            var sett = false;
            var above = false;
            var hour = 1.0;
            var ym = SinAlt(1, date, hour - 1.0, cglat, sglat) - sinho;
            if (ym > 0.0) above = true;

            double utrise = 0.0, utset = 0.0;
            while (hour < 25 && (sett == false || rise == false))
            {
                var yz = SinAlt(1, date, hour, cglat, sglat) - sinho;
                var yp = SinAlt(1, date, hour + 1.0, cglat, sglat) - sinho;
                var quadout = Quad(ym, yz, yp);
                var nz = quadout[0];
                var z1 = quadout[1];
                var z2 = quadout[2];
                var xe = quadout[3];
                var ye = quadout[4];

                if (nz == 1)
                {
                    if (ym < 0.0)
                    {
                        utrise = hour + z1;
                        rise = true;
                    }
                    else
                    {
                        utset = hour + z1;
                        sett = true;
                    }
                }

                if (nz == 2)
                {
                    if (ye < 0.0)
                    {
                        utrise = hour + z2;
                        utset = hour + z1;
                    }
                    else
                    {
                        utrise = hour + z1;
                        utset = hour + z2;
                    }
                }

                ym = yp;
                hour += 2.0;
            }

            if (rise == true || sett == true)
            {
                if (rise == true) outstring += " " + HrsMin(utrise);
                else outstring += " ----";
                if (sett == true) outstring += " " + HrsMin(utset);
                else outstring += " ----";
            }
            else
            {
                if (above == true) outstring += always_up + always_up;
                else outstring += always_down + always_down;
            }

            return outstring;
        }

    }
}
