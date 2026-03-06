using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Text;

public partial class Kundali : Page
{
    private string cs = ConfigurationManager.ConnectionStrings["AstrologyDB"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserId"] == null)
        {
            Response.Redirect("Login.aspx");
            return;
        }
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        try
        {
            string fullName = txtFullName.Text.Trim();
            string birthPlace = txtBirthPlace.Text.Trim();
            string gender = ddlGender.SelectedValue;

            DateTime birthDate = DateTime.Parse(txtBirthDate.Text);
            TimeSpan birthTime = TimeSpan.Parse(txtBirthTime.Text);
            DateTime localDT = birthDate.Date + birthTime;

            double lat = ParseDoubleSafe(hfLatitude.Value, 28.6139);
            double lon = ParseDoubleSafe(hfLongitude.Value, 77.2090);
            string tz = string.IsNullOrEmpty(hfTimezone.Value) ? "Asia/Kolkata" : hfTimezone.Value;

            double utcOffsetHours = GetUtcOffset(tz, localDT);
            DateTime utcDT = localDT.AddHours(-utcOffsetHours);

            double jd = DateTimeToJulianDay(utcDT);
            double ayanamsha = LahiriAyanamsha(jd);

            List<PlanetData> planets = CalculatePlanetPositions(jd, ayanamsha);

            double lagnaLongitude = CalculateLagna(jd, lat, lon, ayanamsha);
            int lagnaRashi = (int)(lagnaLongitude / 30.0) + 1;
            if (lagnaRashi > 12) lagnaRashi = 12;
            if (lagnaRashi < 1) lagnaRashi = 1;
            double lagnaInSign = lagnaLongitude % 30.0;

            int i2;
            for (i2 = 0; i2 < planets.Count; i2++)
                planets[i2].House = CalculateHouse(planets[i2].Longitude, lagnaLongitude);

            PlanetData moonP = GetPlanet(planets, "Moon");
            double moonLon = (moonP != null) ? moonP.Longitude : 0.0;
            int nakshatraIndex = (int)(moonLon / (360.0 / 27.0));
            double nakshatraDeg = moonLon % (360.0 / 27.0);
            int pada = (int)(nakshatraDeg / (360.0 / 108.0)) + 1;

            DashaResult dashaInfo = CalculateVimshottariDasha(moonLon, birthDate);
            List<YogaDosh> yogas = DetectYogas(planets, lagnaRashi);
            List<YogaDosh> doshas = DetectDoshas(planets, lagnaRashi);
            string predictions = GeneratePredictions(planets, lagnaRashi, nakshatraIndex);
            string chartJson = BuildChartJson(planets, lagnaRashi);

            PopulateUI(fullName, birthDate, birthTime, birthPlace, lat, lon,
                       planets, lagnaLongitude, lagnaRashi, lagnaInSign,
                       nakshatraIndex, pada, dashaInfo, yogas, doshas,
                       predictions, chartJson, utcOffsetHours);

            int birthId = SaveBirthDetails(fullName, birthDate, birthTime, birthPlace,
                                           lat, lon, tz, gender);
            if (birthId > 0)
                SaveKundali(birthId, lagnaRashi, planets, nakshatraIndex, pada, dashaInfo, chartJson);

            kundaliResult.Style["display"] = "block";
        }
        catch (Exception ex)
        {
            pnlMsg.Visible = true;
            divMsg.InnerHtml = "Kuch galat hua: " + ex.Message;
        }
    }

    protected void btnSaveKundali_Click(object sender, EventArgs e)
    {
        ShowSuccessMsg("Kundali already saved!");
    }

    // =========================================================
    //  JULIAN DAY NUMBER
    // =========================================================
    private double DateTimeToJulianDay(DateTime dt)
    {
        int y = dt.Year;
        int m = dt.Month;
        int d = dt.Day;
        double h = dt.Hour + dt.Minute / 60.0 + dt.Second / 3600.0;
        if (m <= 2) { y--; m += 12; }
        int A = y / 100;
        int B = 2 - A + A / 4;
        return Math.Floor(365.25 * (y + 4716))
             + Math.Floor(30.6001 * (m + 1))
             + d + h / 24.0 + B - 1524.5;
    }

    // =========================================================
    //  LAHIRI AYANAMSHA
    // =========================================================
    private double LahiriAyanamsha(double jd)
    {
        double base1900 = 22.460905;
        double ratePerYear = 50.2674 / 3600.0;
        double yearsFrom1900 = (jd - 2415020.9132) / 365.25;
        return base1900 + ratePerYear * yearsFrom1900;
    }

    // =========================================================
    //  PLANET POSITIONS (VSOP87 simplified)
    // =========================================================
    private List<PlanetData> CalculatePlanetPositions(double jd, double ayanamsha)
    {
        double T = (jd - 2451545.0) / 36525.0;
        List<PlanetData> list = new List<PlanetData>();
        list.Add(MakePlanet("Sun", "\u2609", NormalizeDeg(SunLongitude(T) - ayanamsha)));
        list.Add(MakePlanet("Moon", "\u263d", NormalizeDeg(MoonLongitude(T) - ayanamsha)));
        list.Add(MakePlanet("Mars", "\u2642", NormalizeDeg(MarsLongitude(T) - ayanamsha)));
        list.Add(MakePlanet("Mercury", "\u263f", NormalizeDeg(MercuryLongitude(T) - ayanamsha)));
        list.Add(MakePlanet("Jupiter", "\u2643", NormalizeDeg(JupiterLongitude(T) - ayanamsha)));
        list.Add(MakePlanet("Venus", "\u2640", NormalizeDeg(VenusLongitude(T) - ayanamsha)));
        list.Add(MakePlanet("Saturn", "\u2644", NormalizeDeg(SaturnLongitude(T) - ayanamsha)));
        double rahuLon = NormalizeDeg(RahuLongitude(T) - ayanamsha);
        double ketuLon = NormalizeDeg(rahuLon + 180.0);
        list.Add(MakePlanet("Rahu", "\u260a", rahuLon));
        list.Add(MakePlanet("Ketu", "\u260b", ketuLon));
        return list;
    }

    private double SunLongitude(double T)
    {
        double L0 = 280.46646 + 36000.76983 * T + 0.0003032 * T * T;
        double M = NormalizeDeg(357.52911 + 35999.05029 * T - 0.0001537 * T * T);
        double Mr = DegToRad(M);
        double C = (1.914602 - 0.004817 * T - 0.000014 * T * T) * Math.Sin(Mr)
                  + (0.019993 - 0.000101 * T) * Math.Sin(2.0 * Mr)
                  + 0.000289 * Math.Sin(3.0 * Mr);
        return NormalizeDeg(L0 + C);
    }

    private double MoonLongitude(double T)
    {
        double L0 = 218.3164477 + 481267.88123421 * T;
        double M = DegToRad(NormalizeDeg(357.5291092 + 35999.0502909 * T));
        double MP = DegToRad(NormalizeDeg(134.9633964 + 477198.8675055 * T));
        double D = DegToRad(NormalizeDeg(297.8501921 + 445267.1114034 * T));
        double F = DegToRad(NormalizeDeg(93.2720950 + 483202.0175233 * T));
        double lon = L0
            + 6.288774 * Math.Sin(MP)
            + 1.274027 * Math.Sin(2.0 * D - MP)
            + 0.658314 * Math.Sin(2.0 * D)
            + 0.213618 * Math.Sin(2.0 * MP)
            - 0.185116 * Math.Sin(M)
            - 0.114332 * Math.Sin(2.0 * F)
            + 0.058793 * Math.Sin(2.0 * D - 2.0 * MP)
            + 0.057066 * Math.Sin(2.0 * D - M - MP)
            + 0.053322 * Math.Sin(2.0 * D + MP)
            + 0.045758 * Math.Sin(2.0 * D - M)
            + 0.041775 * Math.Sin(MP - M)
            + 0.034725 * Math.Sin(D)
            + 0.030333 * Math.Sin(2.0 * D + M)
            + 0.015327 * Math.Sin(2.0 * D - 2.0 * F);
        return NormalizeDeg(lon);
    }

    private double MarsLongitude(double T)
    {
        double L = 355.433 + 19141.6964471 * T;
        double M = DegToRad(NormalizeDeg(19.3730 + 19140.30268 * T));
        return NormalizeDeg(L + 10.6912 * Math.Sin(M) + 0.6228 * Math.Sin(2.0 * M) + 0.0503 * Math.Sin(3.0 * M));
    }

    private double MercuryLongitude(double T)
    {
        double L = 252.2509 + 149474.0722 * T;
        double M = DegToRad(NormalizeDeg(174.7948 + 149472.5153 * T));
        return NormalizeDeg(L + 23.4400 * Math.Sin(M) + 2.9818 * Math.Sin(2.0 * M)
             + 0.5255 * Math.Sin(3.0 * M) + 0.1058 * Math.Sin(4.0 * M));
    }

    private double JupiterLongitude(double T)
    {
        double L = 34.3515 + 3034.9057 * T;
        double M = DegToRad(NormalizeDeg(20.9 + 3034.9057 * T));
        return NormalizeDeg(L + 5.5549 * Math.Sin(M) + 0.1683 * Math.Sin(2.0 * M) + 0.0071 * Math.Sin(3.0 * M));
    }

    private double VenusLongitude(double T)
    {
        double L = 181.9798 + 58519.2130 * T;
        double M = DegToRad(NormalizeDeg(212.3005 + 58517.8039 * T));
        return NormalizeDeg(L + 0.7758 * Math.Sin(M) + 0.0033 * Math.Sin(2.0 * M));
    }

    private double SaturnLongitude(double T)
    {
        double L = 50.0774 + 1222.1138 * T;
        double M = DegToRad(NormalizeDeg(317.9065 + 1221.5515 * T));
        return NormalizeDeg(L + 6.3585 * Math.Sin(M) + 0.2204 * Math.Sin(2.0 * M) + 0.0106 * Math.Sin(3.0 * M));
    }

    private double RahuLongitude(double T)
    {
        return NormalizeDeg(125.0445479 - 1934.1362608 * T + 0.0020754 * T * T);
    }

    // =========================================================
    //  LAGNA
    // =========================================================
    private double CalculateLagna(double jd, double lat, double lon, double ayanamsha)
    {
        double T = (jd - 2451545.0) / 36525.0;
        double theta = NormalizeDeg(100.4606184 + 36000.7700536 * T + 0.000387933 * T * T);
        double lst = NormalizeDeg(theta + lon);
        double eps = 23.439291111 - 0.013004167 * T;
        double epsR = DegToRad(eps);
        double lstR = DegToRad(lst);
        double latR = DegToRad(lat);
        double y = -Math.Cos(lstR);
        double x = Math.Sin(epsR) * Math.Tan(latR) + Math.Cos(epsR) * Math.Sin(lstR);
        double asc = RadToDeg(Math.Atan2(y, x));
        return NormalizeDeg(asc - ayanamsha);
    }

    private int CalculateHouse(double planetLon, double lagnaLon)
    {
        double diff = NormalizeDeg(planetLon - lagnaLon);
        int h = (int)(diff / 30.0) + 1;
        if (h > 12) h = 12;
        if (h < 1) h = 1;
        return h;
    }

    // =========================================================
    //  VIMSHOTTARI DASHA
    // =========================================================
    private static readonly string[] DashaOrder = new string[] { "Ketu", "Venus", "Sun", "Moon", "Mars", "Rahu", "Jupiter", "Saturn", "Mercury" };
    private static readonly int[] DashaYears = new int[] { 7, 20, 6, 10, 7, 18, 16, 19, 17 };

    private DashaResult CalculateVimshottariDasha(double moonLon, DateTime birthDate)
    {
        string[] nakLords = new string[] {
            "Ketu","Venus","Sun","Moon","Mars","Rahu","Jupiter","Saturn","Mercury",
            "Ketu","Venus","Sun","Moon","Mars","Rahu","Jupiter","Saturn","Mercury",
            "Ketu","Venus","Sun","Moon","Mars","Rahu","Jupiter","Saturn","Mercury"
        };
        int nakIdx = (int)(moonLon / (360.0 / 27.0));
        double degInNak = moonLon % (360.0 / 27.0);
        double nakSize = 360.0 / 27.0;
        string birthLord = nakLords[nakIdx];
        int lordIdx = Array.IndexOf(DashaOrder, birthLord);
        double balance = DashaYears[lordIdx] * (1.0 - degInNak / nakSize);

        DashaResult result = new DashaResult();
        result.BirthLord = birthLord;
        result.DashaBalance = balance;
        result.Periods = new List<DashaPeriod>();

        DateTime cursor = birthDate;
        for (int i = 0; i < 9; i++)
        {
            int idx = (lordIdx + i) % 9;
            double yrs = (i == 0) ? balance : (double)DashaYears[idx];
            DashaPeriod dp = new DashaPeriod();
            dp.Planet = DashaOrder[idx];
            dp.Years = yrs;
            dp.From = cursor;
            dp.To = cursor.AddDays(yrs * 365.25);
            result.Periods.Add(dp);
            cursor = dp.To;
        }

        DateTime now = DateTime.Now;
        int pi;
        for (pi = 0; pi < result.Periods.Count; pi++)
        {
            DashaPeriod p = result.Periods[pi];
            if (now >= p.From && now < p.To)
            {
                result.CurrentMahadasha = p.Planet;
                result.RemainingYears = (p.To - now).TotalDays / 365.25;
                // Antardasha
                int mdIdx = Array.IndexOf(DashaOrder, p.Planet);
                DateTime adCursor = p.From;
                int j;
                for (j = 0; j < 9; j++)
                {
                    int adIdx = (mdIdx + j) % 9;
                    double adYrs = DashaYears[adIdx] * (DashaYears[mdIdx] / 120.0);
                    DateTime adEnd = adCursor.AddDays(adYrs * 365.25);
                    if (now >= adCursor && now < adEnd)
                    {
                        result.CurrentAntardasha = DashaOrder[adIdx];
                        break;
                    }
                    adCursor = adEnd;
                }
                break;
            }
        }

        if (result.CurrentMahadasha == null)
        {
            result.CurrentMahadasha = result.Periods[0].Planet;
            result.CurrentAntardasha = result.Periods[0].Planet;
            result.RemainingYears = result.Periods[0].Years;
        }
        return result;
    }

    // =========================================================
    //  YOGA DETECTION
    // =========================================================
    private List<YogaDosh> DetectYogas(List<PlanetData> planets, int lagnaRashi)
    {
        List<YogaDosh> list = new List<YogaDosh>();
        PlanetData sunP = GetPlanet(planets, "Sun");
        PlanetData moonP = GetPlanet(planets, "Moon");
        PlanetData marsP = GetPlanet(planets, "Mars");
        PlanetData jupP = GetPlanet(planets, "Jupiter");
        PlanetData satP = GetPlanet(planets, "Saturn");
        PlanetData mercP = GetPlanet(planets, "Mercury");
        PlanetData venP = GetPlanet(planets, "Venus");

        if (jupP != null && moonP != null)
        {
            int diff = Math.Abs(jupP.House - moonP.House);
            if (diff == 0 || diff == 3 || diff == 6 || diff == 9)
                list.Add(NewYD("Gaj Kesari Yoga", "raj", "Budhi, yashas aur samriddhi pradaayak"));
        }
        if (sunP != null && mercP != null && sunP.House == mercP.House)
            list.Add(NewYD("Budhaditya Yoga", "raj", "Tejasvi buddhi, naam aur yash"));
        if (moonP != null && marsP != null && moonP.House == marsP.House)
            list.Add(NewYD("Chandra-Mangal Yoga", "yoga", "Dhan labh aur sahas ka yoga"));
        if (jupP != null && (jupP.Sign == 4 || jupP.Sign == 9 || jupP.Sign == 12))
            list.Add(NewYD("Hamsa Yoga", "raj", "Gyaan, dharma aur prasiddhi"));
        if (venP != null && (venP.Sign == 2 || venP.Sign == 7 || venP.Sign == 12))
            if (venP.House == 1 || venP.House == 4 || venP.House == 7 || venP.House == 10)
                list.Add(NewYD("Malavya Yoga", "raj", "Vaibhav, sundar vyaktitva, bhog-vilas"));
        if (marsP != null && (marsP.Sign == 1 || marsP.Sign == 8 || marsP.Sign == 10))
            if (marsP.House == 1 || marsP.House == 4 || marsP.House == 7 || marsP.House == 10)
                list.Add(NewYD("Ruchaka Yoga", "raj", "Veerata, shakti, netritva"));
        if (jupP != null && (jupP.House == 9 || jupP.House == 10))
            list.Add(NewYD("Dharma-Karma Yoga", "raj", "Uchcha pad, samman aur dharm mein ruchi"));
        if (satP != null && (satP.Sign == 7 || satP.Sign == 10 || satP.Sign == 11))
            if (satP.House == 1 || satP.House == 4 || satP.House == 7 || satP.House == 10)
                list.Add(NewYD("Shasha Yoga", "raj", "Neeti, parishram se uchcha pad"));

        if (list.Count == 0)
            list.Add(NewYD("Saral Yoga", "yoga", "Graha balanced avastha mein hain"));
        return list;
    }

    // =========================================================
    //  DOSH DETECTION
    // =========================================================
    private List<YogaDosh> DetectDoshas(List<PlanetData> planets, int lagnaRashi)
    {
        List<YogaDosh> list = new List<YogaDosh>();
        PlanetData marsP = GetPlanet(planets, "Mars");
        PlanetData rahuP = GetPlanet(planets, "Rahu");
        PlanetData satP = GetPlanet(planets, "Saturn");
        PlanetData sunP = GetPlanet(planets, "Sun");
        PlanetData moonP = GetPlanet(planets, "Moon");

        if (marsP != null && (marsP.House == 1 || marsP.House == 4 ||
            marsP.House == 7 || marsP.House == 8 || marsP.House == 12))
            list.Add(NewYD("Mangal Dosh", "dosh", "Vivah mein saavdhani zaroori, Mars 1/4/7/8/12 mein"));

        if (rahuP != null)
        {
            bool ksd = true;
            double rahuLon = rahuP.Longitude;
            PlanetData ketuP = GetPlanet(planets, "Ketu");
            if (ketuP != null)
            {
                double ketuLon = ketuP.Longitude;
                int ki;
                for (ki = 0; ki < planets.Count; ki++)
                {
                    PlanetData pl = planets[ki];
                    if (pl.Name == "Rahu" || pl.Name == "Ketu") continue;
                    double dv = NormalizeDeg(pl.Longitude - rahuLon);
                    double dk = NormalizeDeg(ketuLon - rahuLon);
                    if (dv > dk) { ksd = false; break; }
                }
                if (ksd)
                    list.Add(NewYD("Kaal Sarp Dosh", "dosh", "Jeevan mein baadhaayein, Rahu-Ketu ke beech sab grah"));
            }
        }

        if (moonP != null && satP != null)
        {
            int diff = Math.Abs(moonP.Sign - satP.Sign);
            if (diff <= 1 || diff >= 11)
                list.Add(NewYD("Shani Sade Sati", "dosh", "7.5 varsh ka Shani prabhav, saavdhani zaroori"));
        }
        if (sunP != null && rahuP != null)
            if (sunP.House == 9 || rahuP.House == 9)
                list.Add(NewYD("Pitra Dosh", "dosh", "Pitru rinmukt hone ke liye upay karein"));
        if (sunP != null && rahuP != null && sunP.House == rahuP.House)
            list.Add(NewYD("Surya Grahan Yoga", "dosh", "Aatma-samman mein baadha sambhav"));
        if (moonP != null && rahuP != null && moonP.House == rahuP.House)
            list.Add(NewYD("Chandra Grahan Yoga", "dosh", "Maan aur sehat mein sawdhaani"));

        if (list.Count == 0)
            list.Add(NewYD("Koi Bada Dosh Nahi", "ok", "Kundali dosh-mukt prateet hoti hai"));
        return list;
    }

    // =========================================================
    //  PREDICTIONS
    // =========================================================
    private string GeneratePredictions(List<PlanetData> planets, int lagnaRashi, int nakshatraIndex)
    {
        string[] rashiNames = new string[] {
            "Mesh (Aries)","Vrishabha (Taurus)","Mithun (Gemini)","Kark (Cancer)",
            "Simha (Leo)","Kanya (Virgo)","Tula (Libra)","Vrischik (Scorpio)",
            "Dhanu (Sagittarius)","Makar (Capricorn)","Kumbha (Aquarius)","Meen (Pisces)"
        };
        string[] rashiDesc = new string[] {
            "Sahas, urja aur netritva ka swami. Aap ek jaanbaaz aur aagekaar vyakti hain.",
            "Dhairya, sthirta aur bhautik sukh. Aap practical aur mehnat se kamaane wale hain.",
            "Buddhimatta, sanchar aur curiosity. Har vishay mein gehri ruchi rakhte hain.",
            "Sanvedansheel, parivar-premi aur gehri feelings wale vyakti hain.",
            "Gaurav, netritva aur sat-bhavana. Drama aur dil-khush personality ke dhani hain.",
            "Vishleshan, seva aur shuddhi. Detail-oriented aur hardworking hain.",
            "Santulan, nyay aur saundary. Partnership-oriented aur diplomatic hain.",
            "Intense, khoj-wali prakriti. Gehre rahasya ke khojee aur transformative hain.",
            "Darshan, swatantrata aur dharma. Philosophical aur adventure-premi hain.",
            "Anushasan, mahtvakanksha. Karm aur responsibility ko uttam maante hain.",
            "Vaicharik, manavtavaadi. Innovative thinker hain, samaj ke liye sochte hain.",
            "Bhavuk, adhyatmik aur daani. Deeply intuitive aur compassionate hain."
        };
        string[] nakshatraNames = new string[] {
            "Ashwini","Bharani","Krittika","Rohini","Mrigashira","Ardra",
            "Punarvasu","Pushya","Ashlesha","Magha","Purva Phalguni","Uttara Phalguni",
            "Hasta","Chitra","Swati","Vishakha","Anuradha","Jyeshtha",
            "Mula","Purva Ashadha","Uttara Ashadha","Shravana","Dhanishta",
            "Shatabhisha","Purva Bhadrapada","Uttara Bhadrapada","Revati"
        };

        string lagnaName = (lagnaRashi >= 1 && lagnaRashi <= 12) ? rashiNames[lagnaRashi - 1] : "";
        string lagnaDesc = (lagnaRashi >= 1 && lagnaRashi <= 12) ? rashiDesc[lagnaRashi - 1] : "";
        string nakName = (nakshatraIndex >= 0 && nakshatraIndex < 27) ? nakshatraNames[nakshatraIndex] : "";

        StringBuilder sb = new StringBuilder();

        // Personality
        sb.Append("<div class='pred-section'>");
        sb.Append("<div class='pred-heading'>Vyaktitva (Personality)</div>");
        sb.Append("<p class='pred-text'><b>Lagna: " + lagnaName + ".</b> " + lagnaDesc + "</p>");
        sb.Append("</div>");

        // Career
        int tenth_count = 0;
        int ti;
        for (ti = 0; ti < planets.Count; ti++) if (planets[ti].House == 10) tenth_count++;
        sb.Append("<div class='pred-section'>");
        sb.Append("<div class='pred-heading'>Vyavsay (Career)</div>");
        if (tenth_count > 0)
        {
            StringBuilder tn = new StringBuilder();
            for (ti = 0; ti < planets.Count; ti++)
            {
                if (planets[ti].House == 10)
                {
                    if (tn.Length > 0) tn.Append(", ");
                    tn.Append(planets[ti].Name);
                }
            }
            sb.Append("<p class='pred-text'>Daswe bhav mein <b>" + tn.ToString() + "</b> hai/hain. ");
            for (ti = 0; ti < planets.Count; ti++)
            {
                if (planets[ti].House != 10) continue;
                switch (planets[ti].Name)
                {
                    case "Sun": sb.Append("Sarkar, rajneeti aur prashasan mein saflata. "); break;
                    case "Moon": sb.Append("Janta se jude vyavsay, nursing, hospitality mein unnati. "); break;
                    case "Mars": sb.Append("Engineering, fauji, police ya sports mein naam milega. "); break;
                    case "Mercury": sb.Append("Media, lekhan, vyapar ya IT mein saflata. "); break;
                    case "Jupiter": sb.Append("Shiksha, dharm, law ya banking mein uchcha pad. "); break;
                    case "Venus": sb.Append("Kala, sangeet, fashion ya entertainment mein unnati. "); break;
                    case "Saturn": sb.Append("Mehnat se dheere-dheere unnati, service sector mein. "); break;
                    case "Rahu": sb.Append("Innovative field ya videsh mein naam-daam milega. "); break;
                    case "Ketu": sb.Append("Research, adhyatma ya technical field mein unnati. "); break;
                }
            }
            sb.Append("</p>");
        }
        else
        {
            sb.Append("<p class='pred-text'>Karma bhav mein koi grah nahi. Mehnat se saflata nishchit hai.</p>");
        }
        sb.Append("</div>");

        // Wealth
        PlanetData jupP = GetPlanet(planets, "Jupiter");
        sb.Append("<div class='pred-section'>");
        sb.Append("<div class='pred-heading'>Dhan (Wealth)</div>");
        sb.Append("<p class='pred-text'>");
        if (jupP != null && (jupP.House == 2 || jupP.House == 5 || jupP.House == 11))
            sb.Append("Guru ki sthiti dhan ke liye shubh hai. Jeevan mein aarthik samriddhi ki sambhavana. ");
        else
            sb.Append("Dhan ke liye niyamit mehnat aur saving zaroori hai. ");
        sb.Append("Dwitiya aur Ekadash bhav ka vishleshan karein.</p>");
        sb.Append("</div>");

        // Health
        int sixth_count = 0;
        int si2;
        for (si2 = 0; si2 < planets.Count; si2++) if (planets[si2].House == 6) sixth_count++;
        sb.Append("<div class='pred-section'>");
        sb.Append("<div class='pred-heading'>Swasthya (Health)</div>");
        sb.Append("<p class='pred-text'>");
        if (sixth_count > 0)
            sb.Append("Chhatha bhav graha se yukta hai. Swasthya par vishesh dhyan dena zaroori. Regular exercise aur sattvic diet apnaayein. ");
        else
            sb.Append("Swasthya generally theek rehta hai. Preventive care apnaate rahein. ");
        sb.Append("Nakshatra " + nakName + " ke anusaar manasik shanti bhi swasthya mein badi bhaagidaar hai.</p>");
        sb.Append("</div>");

        // Marriage
        PlanetData venP = GetPlanet(planets, "Venus");
        sb.Append("<div class='pred-section'>");
        sb.Append("<div class='pred-heading'>Vivah (Marriage)</div>");
        sb.Append("<p class='pred-text'>");
        if (venP != null && (venP.House == 7 || venP.House == 1 || venP.House == 5))
            sb.Append("Shukra ki sthiti vivah ke liye uttam hai. Prem aur sajhedari mein sukh milega. ");
        StringBuilder sv = new StringBuilder();
        int vi;
        for (vi = 0; vi < planets.Count; vi++)
        {
            if (planets[vi].House == 7)
            {
                if (sv.Length > 0) sv.Append(", ");
                sv.Append(planets[vi].Name);
            }
        }
        if (sv.Length > 0)
            sb.Append("Saptam bhav mein " + sv.ToString() + " hai. Yeh jeevan-saathi ke swabhav ko prabhavit karta hai. ");
        else
            sb.Append("Saptam bhav swatantra hai. Vivah jeevan sukhamay rehne ki sambhavana achchi hai. ");
        sb.Append("Rishte mein vishwas aur samman zaroor rakhein.</p>");
        sb.Append("</div>");

        return sb.ToString();
    }

    // =========================================================
    //  BUILD CHART JSON
    // =========================================================
    private string BuildChartJson(List<PlanetData> planets, int lagnaRashi)
    {
        List<object> data = new List<object>();
        int h;
        for (h = 1; h <= 12; h++)
        {
            int signNum = ((lagnaRashi - 1 + h - 1) % 12) + 1;
            int pi;
            for (pi = 0; pi < planets.Count; pi++)
            {
                if (planets[pi].House == h)
                    data.Add(new { house = h, sym = planets[pi].Symbol, signNum = signNum, name = planets[pi].Name });
            }
            data.Add(new { house = h, sym = "", signNum = signNum, name = "_sign" });
        }
        return new JavaScriptSerializer().Serialize(data);
    }

    // =========================================================
    //  POPULATE UI
    // =========================================================
    private void PopulateUI(string fullName, DateTime birthDate, TimeSpan birthTime,
        string birthPlace, double lat, double lon,
        List<PlanetData> planets, double lagnaLon, int lagnaRashi, double lagnaInSign,
        int nakshatraIndex, int pada, DashaResult dashaInfo,
        List<YogaDosh> yogas, List<YogaDosh> doshas, string predictions,
        string chartJson, double utcOffset)
    {
        string[] rashiNames = new string[] {
            "Mesh","Vrishabha","Mithun","Kark","Simha","Kanya",
            "Tula","Vrischik","Dhanu","Makar","Kumbha","Meen"
        };
        string[] rashiEng = new string[] {
            "Aries","Taurus","Gemini","Cancer","Leo","Virgo",
            "Libra","Scorpio","Sagittarius","Capricorn","Aquarius","Pisces"
        };
        string[] rashiSym = new string[] {
            "\u2648","\u2649","\u264a","\u264b","\u264c","\u264d",
            "\u264e","\u264f","\u2650","\u2651","\u2652","\u2653"
        };
        string[] rashiLords = new string[] {
            "Mars","Venus","Mercury","Moon","Sun","Mercury",
            "Venus","Mars","Jupiter","Saturn","Saturn","Jupiter"
        };
        string[] nakNames = new string[] {
            "Ashwini","Bharani","Krittika","Rohini","Mrigashira","Ardra",
            "Punarvasu","Pushya","Ashlesha","Magha","Purva Phalguni","Uttara Phalguni",
            "Hasta","Chitra","Swati","Vishakha","Anuradha","Jyeshtha",
            "Mula","Purva Ashadha","Uttara Ashadha","Shravana","Dhanishta",
            "Shatabhisha","Purva Bhadrapada","Uttara Bhadrapada","Revati"
        };
        string[] nakLords = new string[] {
            "Ketu","Venus","Sun","Moon","Mars","Rahu","Jupiter","Saturn","Mercury",
            "Ketu","Venus","Sun","Moon","Mars","Rahu","Jupiter","Saturn","Mercury",
            "Ketu","Venus","Sun","Moon","Mars","Rahu","Jupiter","Saturn","Mercury"
        };
        string[] nakDeities = new string[] {
            "Ashwini Kumaras","Yama","Agni","Brahma","Soma","Rudra",
            "Aditi","Brihaspati","Nagas","Pitris","Bhaga","Aryaman",
            "Savitr","Tvastar","Vayu","Indragni","Mitra","Indra",
            "Nirriti","Apah","Vishvadevas","Vishnu","Vasus","Varuna",
            "Ajaikapada","Ahirbudhnya","Pushan"
        };
        string[] ganas = new string[] {
            "Deva","Manushya","Rakshasa","Deva","Deva","Manushya","Deva","Deva","Rakshasa",
            "Rakshasa","Manushya","Manushya","Deva","Rakshasa","Deva","Rakshasa","Deva","Rakshasa",
            "Rakshasa","Manushya","Manushya","Deva","Rakshasa","Deva","Manushya","Manushya","Deva"
        };
        string[] yonis = new string[] {
            "Ashwa","Gaja","Mesha","Sarpa","Sarpa","Shwan","Marjar","Mesha","Marjar",
            "Mushak","Go","Go","Mahish","Vyaghra","Mahish","Vyaghra","Mriga","Mriga",
            "Shwan","Vanar","Nakul","Vanar","Simha","Ashwa","Simha","Gaja","Gaja"
        };
        string[] nadis = new string[] {
            "Adi","Madhya","Antya","Antya","Adi","Madhya","Adi","Madhya","Antya",
            "Antya","Adi","Madhya","Antya","Adi","Madhya","Antya","Adi","Madhya",
            "Antya","Adi","Madhya","Antya","Adi","Madhya","Antya","Adi","Madhya"
        };

        int li = lagnaRashi - 1;

        lblPersonName.Text = fullName.ToUpper();
        lblBirthInfo.Text = birthDate.ToString("dd MMMM yyyy") + " at "
                           + birthTime.Hours.ToString("D2") + ":" + birthTime.Minutes.ToString("D2")
                           + " \u2022 " + birthPlace;
        lblPlaceCoord.Text = "Lat: " + lat.ToString("F4") + "\u00b0  |  Lon: "
                           + lon.ToString("F4") + "\u00b0  |  UTC+" + utcOffset.ToString("F1");

        lblLagnaSymbol.Text = rashiSym[li];
        lblLagnaName.Text = rashiNames[li] + " (" + rashiEng[li] + ")";
        lblLagnaSub.Text = lagnaInSign.ToString("F2") + "\u00b0 in sign  |  Lord: " + rashiLords[li];

        PlanetData moonP = GetPlanet(planets, "Moon");
        if (moonP != null)
        {
            int ms = moonP.Sign - 1;
            lblMoonSymbol.Text = rashiSym[ms];
            lblMoonRashi.Text = rashiNames[ms] + " (" + rashiEng[ms] + ")";
            lblMoonSub.Text = (moonP.Longitude % 30.0).ToString("F2") + "\u00b0  |  Lord: " + rashiLords[ms];
        }

        PlanetData sunP = GetPlanet(planets, "Sun");
        if (sunP != null)
        {
            int ss = sunP.Sign - 1;
            lblSunSymbol.Text = rashiSym[ss];
            lblSunRashi.Text = rashiNames[ss] + " (" + rashiEng[ss] + ")";
            lblSunSub.Text = (sunP.Longitude % 30.0).ToString("F2") + "\u00b0  |  Lord: " + rashiLords[ss];
        }

        // Planet rows
        List<object> pRows = new List<object>();
        int pi2;
        for (pi2 = 0; pi2 < planets.Count; pi2++)
        {
            PlanetData p = planets[pi2];
            int si = p.Sign - 1;
            pRows.Add(new
            {
                Symbol = p.Symbol,
                PlanetName = p.Name,
                SignName = rashiNames[si],
                SignNum = p.Sign,
                House = p.House,
                Degree = (p.Longitude % 30.0).ToString("F2")
            });
        }
        rptPlanets.DataSource = pRows;
        rptPlanets.DataBind();

        // Nakshatra
        int ni = nakshatraIndex;
        lblNakshatra.Text = (ni >= 0 && ni < 27) ? nakNames[ni] : "--";
        lblNakshatraLord.Text = (ni >= 0 && ni < 27) ? nakLords[ni] : "--";
        lblPada.Text = pada.ToString();
        lblRashiLord.Text = (moonP != null) ? rashiLords[moonP.Sign - 1] : "--";
        lblDeity.Text = (ni >= 0 && ni < 27) ? nakDeities[ni] : "--";
        lblGan.Text = (ni >= 0 && ni < 27) ? ganas[ni] : "--";
        lblYoni.Text = (ni >= 0 && ni < 27) ? yonis[ni] : "--";
        lblNadi.Text = (ni >= 0 && ni < 27) ? nadis[ni] : "--";

        // Dasha
        lblCurrentDasha.Text = (dashaInfo.CurrentMahadasha != null) ? dashaInfo.CurrentMahadasha : "--";
        lblCurrentAntardasha.Text = (dashaInfo.CurrentAntardasha != null) ? dashaInfo.CurrentAntardasha : "--";
        lblDashaRemaining.Text = dashaInfo.RemainingYears.ToString("F1");

        List<object> dRows = new List<object>();
        int di;
        for (di = 0; di < dashaInfo.Periods.Count; di++)
        {
            DashaPeriod dp = dashaInfo.Periods[di];
            int barW = (int)Math.Min(100.0, dp.Years / 20.0 * 100.0);
            dRows.Add(new
            {
                Planet = dp.Planet,
                From = dp.From.Year.ToString(),
                To = dp.To.Year.ToString(),
                Years = dp.Years.ToString("F1"),
                BarWidth = barW
            });
        }
        rptDasha.DataSource = dRows;
        rptDasha.DataBind();

        // Yogas
        StringBuilder yh = new StringBuilder();
        int yi;
        for (yi = 0; yi < yogas.Count; yi++)
        {
            string cls = (yogas[yi].Type == "raj") ? "yoga-tag raj"
                       : (yogas[yi].Type == "dosh") ? "yoga-tag dosh" : "yoga-tag";
            yh.Append("<span class='" + cls + "' title='" + yogas[yi].Desc + "'>" + yogas[yi].Name + "</span> ");
        }
        divYogas.InnerHtml = yh.ToString();

        // Doshas
        StringBuilder dh = new StringBuilder();
        int doshi;
        for (doshi = 0; doshi < doshas.Count; doshi++)
        {
            string cls = (doshas[doshi].Type == "ok") ? "yoga-tag" : "yoga-tag dosh";
            dh.Append("<span class='" + cls + "'>" + doshas[doshi].Name + "</span>");
            dh.Append("<p style='color:#9a8f7a;font-size:0.78rem;margin:2px 0 8px 4px;'>" + doshas[doshi].Desc + "</p>");
        }
        divDoshas.InnerHtml = dh.ToString();

        divPredictions.InnerHtml = predictions;
        hfChartData.Value = chartJson;
    }

    // =========================================================
    //  DATABASE
    // =========================================================
    private int SaveBirthDetails(string fullName, DateTime birthDate, TimeSpan birthTime,
                                  string birthPlace, double lat, double lon, string tz, string gender)
    {
        try
        {
            int userId = (int)Session["UserId"];
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand chk = new SqlCommand(
                    "SELECT BirthId FROM UserBirthDetails WHERE UserId=@Uid AND BirthDate=@BD AND BirthTime=@BT", con);
                chk.Parameters.AddWithValue("@Uid", userId);
                chk.Parameters.AddWithValue("@BD", birthDate.Date);
                chk.Parameters.AddWithValue("@BT", birthTime);
                object ex = chk.ExecuteScalar();
                if (ex != null) return Convert.ToInt32(ex);

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO UserBirthDetails (UserId,FullName,BirthDate,BirthTime,BirthPlace,Latitude,Longitude,TimeZone,Gender,CreatedDate) " +
                    "OUTPUT INSERTED.BirthId " +
                    "VALUES (@Uid,@Name,@BD,@BT,@BP,@Lat,@Lon,@TZ,@Gender,GETDATE())", con);
                cmd.Parameters.AddWithValue("@Uid", userId);
                cmd.Parameters.AddWithValue("@Name", fullName);
                cmd.Parameters.AddWithValue("@BD", birthDate.Date);
                cmd.Parameters.AddWithValue("@BT", birthTime);
                cmd.Parameters.AddWithValue("@BP", birthPlace);
                cmd.Parameters.AddWithValue("@Lat", lat);
                cmd.Parameters.AddWithValue("@Lon", lon);
                cmd.Parameters.AddWithValue("@TZ", tz);
                cmd.Parameters.AddWithValue("@Gender", gender);
                return (int)cmd.ExecuteScalar();
            }
        }
        catch { return -1; }
    }

    private void SaveKundali(int birthId, int lagnaRashi, List<PlanetData> planets,
                              int moonNakshatra, int pada, DashaResult dasha, string chartJson)
    {
        try
        {
            int userId = (int)Session["UserId"];
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql =
                    "IF EXISTS (SELECT 1 FROM Kundali WHERE BirthId=@BId AND UserId=@Uid) " +
                    "    UPDATE Kundali SET Lagna=@Lagna,SunHouse=@SH,MoonHouse=@MH,MarsHouse=@MaH," +
                    "    MercuryHouse=@McH,JupiterHouse=@JH,VenusHouse=@VH,SaturnHouse=@SaH,RahuHouse=@RaH,KetuHouse=@KH," +
                    "    SunSign=@SS,MoonSign=@MS,MarsSign=@MaS,MercurySign=@McS,JupiterSign=@JS,VenusSign=@VS," +
                    "    SaturnSign=@SaS,RahuSign=@RaS,KetuSign=@KS,MoonNakshatra=@MN,MoonNakshatraPada=@MP," +
                    "    CurrentMahadasha=@CMD,CurrentAntardasha=@CAD,MahadashaRemaining=@MDR,ChartData=@CD,GenerationDate=GETDATE() " +
                    "    WHERE BirthId=@BId AND UserId=@Uid " +
                    "ELSE " +
                    "    INSERT INTO Kundali (BirthId,UserId,Lagna,SunHouse,MoonHouse,MarsHouse,MercuryHouse,JupiterHouse,VenusHouse,SaturnHouse,RahuHouse,KetuHouse," +
                    "    SunSign,MoonSign,MarsSign,MercurySign,JupiterSign,VenusSign,SaturnSign,RahuSign,KetuSign," +
                    "    MoonNakshatra,MoonNakshatraPada,CurrentMahadasha,CurrentAntardasha,MahadashaRemaining,ChartData) " +
                    "    VALUES (@BId,@Uid,@Lagna,@SH,@MH,@MaH,@McH,@JH,@VH,@SaH,@RaH,@KH," +
                    "    @SS,@MS,@MaS,@McS,@JS,@VS,@SaS,@RaS,@KS,@MN,@MP,@CMD,@CAD,@MDR,@CD)";

                SqlCommand cmd = new SqlCommand(sql, con);
                AddP(cmd, "@BId", birthId);
                AddP(cmd, "@Uid", userId);
                AddP(cmd, "@Lagna", lagnaRashi);
                AddP(cmd, "@SH", HouseOf(planets, "Sun"));
                AddP(cmd, "@MH", HouseOf(planets, "Moon"));
                AddP(cmd, "@MaH", HouseOf(planets, "Mars"));
                AddP(cmd, "@McH", HouseOf(planets, "Mercury"));
                AddP(cmd, "@JH", HouseOf(planets, "Jupiter"));
                AddP(cmd, "@VH", HouseOf(planets, "Venus"));
                AddP(cmd, "@SaH", HouseOf(planets, "Saturn"));
                AddP(cmd, "@RaH", HouseOf(planets, "Rahu"));
                AddP(cmd, "@KH", HouseOf(planets, "Ketu"));
                AddP(cmd, "@SS", SignOf(planets, "Sun"));
                AddP(cmd, "@MS", SignOf(planets, "Moon"));
                AddP(cmd, "@MaS", SignOf(planets, "Mars"));
                AddP(cmd, "@McS", SignOf(planets, "Mercury"));
                AddP(cmd, "@JS", SignOf(planets, "Jupiter"));
                AddP(cmd, "@VS", SignOf(planets, "Venus"));
                AddP(cmd, "@SaS", SignOf(planets, "Saturn"));
                AddP(cmd, "@RaS", SignOf(planets, "Rahu"));
                AddP(cmd, "@KS", SignOf(planets, "Ketu"));
                AddP(cmd, "@MN", moonNakshatra + 1);
                AddP(cmd, "@MP", pada);
                AddP(cmd, "@CMD", (dasha.CurrentMahadasha != null) ? dasha.CurrentMahadasha : "");
                AddP(cmd, "@CAD", (dasha.CurrentAntardasha != null) ? dasha.CurrentAntardasha : "");
                AddP(cmd, "@MDR", (decimal)Math.Round(dasha.RemainingYears, 2));
                AddP(cmd, "@CD", chartJson);
                cmd.ExecuteNonQuery();
            }
        }
        catch { }
    }

    // =========================================================
    //  HELPERS
    // =========================================================
    private PlanetData MakePlanet(string name, string sym, double lon)
    {
        int sign = (int)(lon / 30.0) + 1;
        if (sign > 12) sign = 12;
        if (sign < 1) sign = 1;
        PlanetData pd = new PlanetData();
        pd.Name = name;
        pd.Symbol = sym;
        pd.Longitude = lon;
        pd.Sign = sign;
        return pd;
    }

    private PlanetData GetPlanet(List<PlanetData> list, string name)
    {
        int i;
        for (i = 0; i < list.Count; i++)
            if (list[i].Name == name) return list[i];
        return null;
    }

    private int HouseOf(List<PlanetData> list, string name)
    {
        PlanetData p = GetPlanet(list, name);
        return (p != null) ? p.House : 0;
    }

    private int SignOf(List<PlanetData> list, string name)
    {
        PlanetData p = GetPlanet(list, name);
        return (p != null) ? p.Sign : 0;
    }

    private double NormalizeDeg(double d)
    {
        d = d % 360.0;
        if (d < 0.0) d += 360.0;
        return d;
    }

    private double DegToRad(double d) { return d * Math.PI / 180.0; }
    private double RadToDeg(double r) { return r * 180.0 / Math.PI; }

    private double ParseDoubleSafe(string s, double def)
    {
        double v;
        return double.TryParse(s, out v) ? v : def;
    }

    private double GetUtcOffset(string tz, DateTime dt)
    {
        bool summer = dt.Month >= 4 && dt.Month <= 10;
        if (tz == "Asia/Kolkata") return 5.5;
        if (tz == "Asia/Karachi") return 5.0;
        if (tz == "Asia/Kathmandu") return 5.75;
        if (tz == "Asia/Dhaka") return 6.0;
        if (tz == "Asia/Colombo") return 5.5;
        if (tz == "Asia/Dubai") return 4.0;
        if (tz == "Asia/Singapore") return 8.0;
        if (tz == "Asia/Hong_Kong") return 8.0;
        if (tz == "Asia/Tokyo") return 9.0;
        if (tz == "Asia/Shanghai") return 8.0;
        if (tz == "Asia/Bangkok") return 7.0;
        if (tz == "Asia/Jakarta") return 7.0;
        if (tz == "Asia/Kabul") return 4.5;
        if (tz == "Asia/Tehran") return 3.5;
        if (tz == "Asia/Baghdad") return 3.0;
        if (tz == "Asia/Riyadh") return 3.0;
        if (tz == "Europe/London") return summer ? 1.0 : 0.0;
        if (tz == "Europe/Paris") return summer ? 2.0 : 1.0;
        if (tz == "Europe/Berlin") return summer ? 2.0 : 1.0;
        if (tz == "Europe/Moscow") return 3.0;
        if (tz == "America/New_York") return summer ? -4.0 : -5.0;
        if (tz == "America/Chicago") return summer ? -5.0 : -6.0;
        if (tz == "America/Denver") return summer ? -6.0 : -7.0;
        if (tz == "America/Los_Angeles") return summer ? -7.0 : -8.0;
        if (tz == "America/Sao_Paulo") return -3.0;
        if (tz == "Australia/Sydney") return 10.0;
        if (tz == "Pacific/Auckland") return 12.0;
        if (tz == "Africa/Cairo") return 2.0;
        if (tz == "Africa/Lagos") return 1.0;
        if (tz == "Africa/Johannesburg") return 2.0;
        return 5.5;
    }

    private YogaDosh NewYD(string name, string type, string desc)
    {
        YogaDosh yd = new YogaDosh();
        yd.Name = name;
        yd.Type = type;
        yd.Desc = desc;
        return yd;
    }

    private void AddP(SqlCommand cmd, string pname, object val)
    {
        cmd.Parameters.AddWithValue(pname, (val != null) ? val : (object)DBNull.Value);
    }

    private void ShowSuccessMsg(string msg)
    {
        pnlMsg.Visible = true;
        divMsg.InnerHtml = msg;
        divMsg.Style["background"] = "rgba(80,180,120,0.1)";
        divMsg.Style["border-color"] = "rgba(80,180,120,0.3)";
        divMsg.Style["color"] = "#7ecba0";
    }
}

// ============================================================
//  DATA CLASSES
// ============================================================
public class PlanetData
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public double Longitude { get; set; }
    public int Sign { get; set; }
    public int House { get; set; }
}

public class DashaPeriod
{
    public string Planet { get; set; }
    public double Years { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class DashaResult
{
    public string BirthLord { get; set; }
    public double DashaBalance { get; set; }
    public List<DashaPeriod> Periods { get; set; }
    public string CurrentMahadasha { get; set; }
    public string CurrentAntardasha { get; set; }
    public double RemainingYears { get; set; }
}

public class YogaDosh
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Desc { get; set; }
}