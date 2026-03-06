<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
   CodeFile="Kundali.aspx.cs" Inherits="Kundali" Title="Generate Kundali" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
  .kundali-wrap { max-width:1100px; margin:50px auto; padding:0 20px; }

  /* ---- Section Title ---- */
  .sec-title {
    font-family:'Cinzel',serif; color:#c9a84c; letter-spacing:3px;
    font-size:1.4rem; margin-bottom:4px; text-align:center;
  }
  .sec-sub {
    color:#9a8f7a; font-family:'Cinzel',serif; font-size:0.68rem;
    letter-spacing:2px; margin-bottom:40px; text-align:center;
  }

  /* ---- Form Card ---- */
  .form-card {
    background:linear-gradient(145deg,#0d1428,#111a33);
    border:1px solid rgba(201,168,76,0.2); border-radius:6px;
    padding:36px 40px; margin-bottom:30px;
  }
  .form-grid {
    display:grid; grid-template-columns:1fr 1fr; gap:22px;
  }
  .form-grid.three { grid-template-columns:1fr 1fr 1fr; }
  .form-group { display:flex; flex-direction:column; gap:6px; }
  .form-group label {
    font-family:'Cinzel',serif; font-size:0.62rem; letter-spacing:2px;
    color:#8a6d2e; text-transform:uppercase;
  }
  .form-group input, .form-group select {
    background:rgba(5,8,16,0.6); border:1px solid rgba(201,168,76,0.2);
    color:#e8dfc8; padding:10px 14px; font-family:'Crimson Text',serif;
    font-size:0.95rem; border-radius:3px; outline:none; width:100%;
    transition:border-color .2s;
  }
  .form-group input:focus, .form-group select:focus {
    border-color:rgba(201,168,76,0.6);
    box-shadow:0 0 12px rgba(201,168,76,0.1);
  }
  .form-group select option { background:#0d1428; }

  /* Location autocomplete */
  #locationSuggestions {
    position:absolute; z-index:999; background:#0d1428;
    border:1px solid rgba(201,168,76,0.3); border-top:none;
    width:100%; max-height:200px; overflow-y:auto; border-radius:0 0 3px 3px;
  }
  #locationSuggestions div {
    padding:8px 14px; cursor:pointer; font-family:'Crimson Text',serif;
    font-size:0.9rem; color:#e8dfc8; border-bottom:1px solid rgba(201,168,76,0.05);
    transition:background .15s;
  }
  #locationSuggestions div:hover { background:rgba(201,168,76,0.12); color:#c9a84c; }
  .loc-wrap { position:relative; }

  /* ---- Divider ---- */
  .divider {
    border:none; border-top:1px solid rgba(201,168,76,0.12);
    margin:28px 0;
  }

  /* ---- Message ---- */
  .msg-box {
    padding:12px 18px; border-radius:3px; margin-bottom:20px;
    font-family:'Cinzel',serif; font-size:0.75rem; letter-spacing:1.5px;
    display:none;
  }

  /* ===============================================================
     KUNDALI RESULT
  ================================================================ */
  .kundali-result {
    display:none;
    animation:fadeIn .5s ease;
  }
  @keyframes fadeIn { from{opacity:0;transform:translateY(20px)} to{opacity:1;transform:translateY(0)} }

  .result-header {
    background:linear-gradient(135deg,rgba(201,168,76,0.08),rgba(107,141,214,0.06));
    border:1px solid rgba(201,168,76,0.25); border-radius:6px;
    padding:30px 36px; margin-bottom:24px; text-align:center;
  }
  .result-header .person-name {
    font-family:'Cinzel',serif; font-size:1.6rem; color:#c9a84c;
    letter-spacing:4px; margin-bottom:6px;
  }
  .result-header .birth-info {
    color:#9a8f7a; font-size:0.85rem; letter-spacing:1px;
  }

  /* ---- Grid Layout ---- */
  .result-grid {
    display:grid; grid-template-columns:1fr 1fr; gap:20px; margin-bottom:20px;
  }
  .result-grid.three { grid-template-columns:1fr 1fr 1fr; }
  .result-grid.full { grid-template-columns:1fr; }

  /* ---- Cards ---- */
  .r-card {
    background:#0d1428; border:1px solid rgba(201,168,76,0.15);
    border-radius:5px; padding:22px 26px;
  }
  .r-card-title {
    font-family:'Cinzel',serif; font-size:0.65rem; letter-spacing:2.5px;
    color:#8a6d2e; text-transform:uppercase; margin-bottom:14px;
    padding-bottom:8px; border-bottom:1px solid rgba(201,168,76,0.1);
  }

  /* ---- Rashi / Lagna badge ---- */
  .rashi-badge {
    display:inline-flex; align-items:center; gap:10px;
    background:rgba(201,168,76,0.08); border:1px solid rgba(201,168,76,0.2);
    border-radius:4px; padding:10px 18px;
  }
  .rashi-badge .sym { font-size:1.6rem; }
  .rashi-badge .info .name {
    font-family:'Cinzel',serif; color:#c9a84c; font-size:0.95rem;
  }
  .rashi-badge .info .sub { color:#9a8f7a; font-size:0.78rem; }

  /* ---- Planet Table ---- */
  .planet-table { width:100%; border-collapse:collapse; }
  .planet-table th {
    font-family:'Cinzel',serif; font-size:0.58rem; letter-spacing:2px;
    color:#8a6d2e; text-transform:uppercase; padding:6px 8px;
    border-bottom:1px solid rgba(201,168,76,0.12); text-align:left;
  }
  .planet-table td {
    padding:8px 8px; color:#e8dfc8; font-size:0.88rem;
    border-bottom:1px solid rgba(201,168,76,0.06);
  }
  .planet-table tr:last-child td { border-bottom:none; }
  .planet-table .planet-sym { font-size:1.1rem; margin-right:5px; }
  .planet-table .sign-num {
    display:inline-block; background:rgba(201,168,76,0.1);
    color:#c9a84c; font-family:'Cinzel',serif; font-size:0.7rem;
    padding:2px 6px; border-radius:2px; margin-left:4px;
  }

  /* ---- North Indian Chart ---- */
  .chart-container {
    position:relative; width:340px; height:340px;
    margin:0 auto;
  }
  .chart-svg { width:100%; height:100%; }

  /* ---- Dasha Table ---- */
  .dasha-row { display:flex; align-items:center; gap:12px; margin-bottom:10px; }
  .dasha-planet-dot {
    width:10px; height:10px; border-radius:50%;
    background:linear-gradient(135deg,#8a6d2e,#c9a84c); flex-shrink:0;
  }
  .dasha-info { flex:1; }
  .dasha-name { font-family:'Cinzel',serif; font-size:0.82rem; color:#c9a84c; }
  .dasha-years { color:#9a8f7a; font-size:0.75rem; }
  .dasha-bar-wrap {
    height:4px; background:rgba(201,168,76,0.1);
    border-radius:2px; margin-top:4px; overflow:hidden;
  }
  .dasha-bar {
    height:100%; border-radius:2px;
    background:linear-gradient(90deg,#8a6d2e,#c9a84c);
    transition:width .8s ease;
  }
  .dasha-active {
    border:1px solid rgba(201,168,76,0.3);
    border-radius:4px; padding:6px 10px;
    background:rgba(201,168,76,0.05);
  }

  /* ---- Yogas ---- */
  .yoga-tag {
    display:inline-block; margin:4px;
    background:rgba(107,141,214,0.1); border:1px solid rgba(107,141,214,0.25);
    color:#a0b4e8; font-family:'Cinzel',serif; font-size:0.65rem;
    letter-spacing:1px; padding:5px 12px; border-radius:2px;
  }
  .yoga-tag.raj { background:rgba(201,168,76,0.1); border-color:rgba(201,168,76,0.3); color:#c9a84c; }
  .yoga-tag.dosh { background:rgba(220,100,100,0.08); border-color:rgba(220,100,100,0.25); color:#e07070; }

  /* ---- Nakshatra detail ---- */
  .nakshatra-detail {
    display:grid; grid-template-columns:repeat(auto-fit,minmax(130px,1fr)); gap:12px;
  }
  .nak-item {
    background:rgba(201,168,76,0.04); border:1px solid rgba(201,168,76,0.1);
    border-radius:3px; padding:12px;
  }
  .nak-item .nak-label { font-family:'Cinzel',serif; font-size:0.58rem; letter-spacing:2px; color:#8a6d2e; margin-bottom:4px; }
  .nak-item .nak-val { color:#c9a84c; font-size:0.9rem; }

  /* ---- Predictions ---- */
  .pred-section { margin-bottom:16px; }
  .pred-heading {
    font-family:'Cinzel',serif; font-size:0.7rem; letter-spacing:2px;
    color:#c9a84c; margin-bottom:6px; display:flex; align-items:center; gap:8px;
  }
  .pred-heading::after {
    content:''; flex:1; height:1px; background:rgba(201,168,76,0.1);
  }
  .pred-text { color:#c8bfa8; font-size:0.92rem; line-height:1.7; }

  /* ---- Print / Save ---- */
  .action-bar {
    display:flex; gap:12px; justify-content:center; margin-top:30px; flex-wrap:wrap;
  }

  @media(max-width:700px) {
    .form-grid, .form-grid.three, .result-grid, .result-grid.three { grid-template-columns:1fr; }
    .form-card { padding:24px 18px; }
    .chart-container { width:260px; height:260px; }
  }

  @media print {
    .navbar, footer, .action-bar, .form-card, .sec-title, .sec-sub { display:none!important; }
    .kundali-result { display:block!important; }
    body { background:#fff; color:#000; }
    .r-card { border:1px solid #ccc; }
    .r-card-title { color:#555; }
    .result-header .person-name { color:#8a4a00; }
  }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="kundali-wrap">

  <h2 class="sec-title">&#9789; Kundali Nirman &#9789;</h2>
  <p class="sec-sub">VEDIC BIRTH CHART GENERATOR — JANM PATRIKA</p>

  <!-- ===== FORM ===== -->
  <div class="form-card">
    <div class="form-grid" style="margin-bottom:22px;">
      <div class="form-group">
        <label>Poora Naam (Full Name)</label>
        <asp:TextBox ID="txtFullName" runat="server" placeholder="e.g. Rahul Sharma" />
        <asp:RequiredFieldValidator ControlToValidate="txtFullName" runat="server"
          Display="Dynamic" ForeColor="#e07070" Font-Size="Small"
          ErrorMessage="Naam zaroori hai" />
      </div>
      <div class="form-group">
        <label>Ling (Gender)</label>
        <asp:DropDownList ID="ddlGender" runat="server">
          <asp:ListItem Value="Male">Male (Purush)</asp:ListItem>
          <asp:ListItem Value="Female">Female (Stri)</asp:ListItem>
          <asp:ListItem Value="Other">Other</asp:ListItem>
        </asp:DropDownList>
      </div>
    </div>

    <div class="form-grid three" style="margin-bottom:22px;">
      <div class="form-group">
        <label>Janm Tithi (Birth Date)</label>
        <asp:TextBox ID="txtBirthDate" runat="server" TextMode="Date" />
        <asp:RequiredFieldValidator ControlToValidate="txtBirthDate" runat="server"
          Display="Dynamic" ForeColor="#e07070" Font-Size="Small"
          ErrorMessage="Janm tithi zaroori hai" />
      </div>
      <div class="form-group">
        <label>Janm Samay (Birth Time)</label>
        <asp:TextBox ID="txtBirthTime" runat="server" TextMode="Time" />
        <asp:RequiredFieldValidator ControlToValidate="txtBirthTime" runat="server"
          Display="Dynamic" ForeColor="#e07070" Font-Size="Small"
          ErrorMessage="Janm samay zaroori hai" />
      </div>
      <div class="form-group">
        <label>Janm Sthan (Birth Place)</label>
        <div class="loc-wrap">
          <asp:TextBox ID="txtBirthPlace" runat="server"
            placeholder="Shahar ka naam likho..." autocomplete="off"
            onkeyup="searchLocation(this.value)" />
          <div id="locationSuggestions"></div>
        </div>
        <!-- Hidden fields for lat/lng/timezone -->
        <asp:HiddenField ID="hfLatitude"  runat="server" />
        <asp:HiddenField ID="hfLongitude" runat="server" />
        <asp:HiddenField ID="hfTimezone"  runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="txtBirthPlace" runat="server"
          Display="Dynamic" ForeColor="#e07070" Font-Size="Small"
          ErrorMessage="Janm sthan zaroori hai" />
      </div>
    </div>

    <!-- Message -->
    <asp:Panel ID="pnlMsg" runat="server" Visible="false">
      <div id="divMsg" runat="server"
           style="padding:12px 18px;border-radius:3px;margin-bottom:16px;
                  font-family:'Cinzel',serif;font-size:0.75rem;letter-spacing:1.5px;
                  border:1px solid rgba(220,100,100,0.3);background:rgba(220,100,100,0.08);color:#e07070;">
      </div>
    </asp:Panel>

    <div style="text-align:center;margin-top:10px;">
      <asp:Button ID="btnGenerate" runat="server" Text="✦ KUNDALI BANAO ✦"
        CssClass="btn btn-gold" OnClick="btnGenerate_Click" />
    </div>
  </div>

  <!-- ===== RESULT ===== -->
  <div class="kundali-result" id="kundaliResult" runat="server">

    <!-- Header -->
    <div class="result-header">
      <div class="person-name">
        <asp:Label ID="lblPersonName" runat="server" />
      </div>
      <div class="birth-info">
        <asp:Label ID="lblBirthInfo" runat="server" />
      </div>
      <div style="margin-top:10px;">
        <asp:Label ID="lblPlaceCoord" runat="server"
          style="color:#8a6d2e;font-family:'Cinzel',serif;font-size:0.62rem;letter-spacing:1.5px;" />
      </div>
    </div>

    <!-- Row 1: Lagna + Moon Sign + Sun Sign -->
    <div class="result-grid three">
      <div class="r-card">
        <div class="r-card-title">&#9906; Lagna (Ascendant)</div>
        <div class="rashi-badge">
          <span class="sym"><asp:Label ID="lblLagnaSymbol" runat="server" /></span>
          <div class="info">
            <div class="name"><asp:Label ID="lblLagnaName" runat="server" /></div>
            <div class="sub"><asp:Label ID="lblLagnaSub" runat="server" /></div>
          </div>
        </div>
      </div>
      <div class="r-card">
        <div class="r-card-title">☽ Chandra Rashi (Moon Sign)</div>
        <div class="rashi-badge">
          <span class="sym"><asp:Label ID="lblMoonSymbol" runat="server" /></span>
          <div class="info">
            <div class="name"><asp:Label ID="lblMoonRashi" runat="server" /></div>
            <div class="sub"><asp:Label ID="lblMoonSub" runat="server" /></div>
          </div>
        </div>
      </div>
      <div class="r-card">
        <div class="r-card-title">☉ Surya Rashi (Sun Sign)</div>
        <div class="rashi-badge">
          <span class="sym"><asp:Label ID="lblSunSymbol" runat="server" /></span>
          <div class="info">
            <div class="name"><asp:Label ID="lblSunRashi" runat="server" /></div>
            <div class="sub"><asp:Label ID="lblSunSub" runat="server" /></div>
          </div>
        </div>
      </div>
    </div>

    <div style="height:20px;"></div>

    <!-- Row 2: North Indian Chart + Planet Positions -->
    <div class="result-grid">
      <div class="r-card">
        <div class="r-card-title">&#9963; North Indian Birth Chart (Kundali)</div>
        <div class="chart-container">
          <svg id="kundaliSvg" class="chart-svg" viewBox="0 0 340 340"
               xmlns="http://www.w3.org/2000/svg"></svg>
        </div>
        <div style="text-align:center;margin-top:8px;">
          <asp:HiddenField ID="hfChartData" runat="server" />
        </div>
      </div>
      <div class="r-card">
        <div class="r-card-title">&#9654; Graha Sthiti (Planet Positions)</div>
        <table class="planet-table">
          <tr>
            <th>Graha</th>
            <th>Rashi</th>
            <th>Bhav</th>
            <th>Degree</th>
          </tr>
          <asp:Repeater ID="rptPlanets" runat="server">
            <ItemTemplate>
              <tr>
                <td>
                  <span class="planet-sym"><%# Eval("Symbol") %></span>
                  <%# Eval("PlanetName") %>
                </td>
                <td><%# Eval("SignName") %><span class="sign-num"><%# Eval("SignNum") %></span></td>
                <td><%# Eval("House") %></td>
                <td style="color:#8a6d2e;"><%# Eval("Degree") %>°</td>
              </tr>
            </ItemTemplate>
          </asp:Repeater>
        </table>
      </div>
    </div>

    <div style="height:20px;"></div>

    <!-- Row 3: Nakshatra + Dasha -->
    <div class="result-grid">
      <div class="r-card">
        <div class="r-card-title">&#10022; Nakshatra Vivaran</div>
        <div class="nakshatra-detail">
          <div class="nak-item">
            <div class="nak-label">Janm Nakshatra</div>
            <div class="nak-val"><asp:Label ID="lblNakshatra" runat="server" /></div>
          </div>
          <div class="nak-item">
            <div class="nak-label">Nakshatra Lord</div>
            <div class="nak-val"><asp:Label ID="lblNakshatraLord" runat="server" /></div>
          </div>
          <div class="nak-item">
            <div class="nak-label">Pada</div>
            <div class="nak-val"><asp:Label ID="lblPada" runat="server" /></div>
          </div>
          <div class="nak-item">
            <div class="nak-label">Rashi Lord</div>
            <div class="nak-val"><asp:Label ID="lblRashiLord" runat="server" /></div>
          </div>
          <div class="nak-item">
            <div class="nak-label">Nakshatra Deity</div>
            <div class="nak-val"><asp:Label ID="lblDeity" runat="server" /></div>
          </div>
          <div class="nak-item">
            <div class="nak-label">Gan</div>
            <div class="nak-val"><asp:Label ID="lblGan" runat="server" /></div>
          </div>
          <div class="nak-item">
            <div class="nak-label">Yoni</div>
            <div class="nak-val"><asp:Label ID="lblYoni" runat="server" /></div>
          </div>
          <div class="nak-item">
            <div class="nak-label">Nadi</div>
            <div class="nak-val"><asp:Label ID="lblNadi" runat="server" /></div>
          </div>
        </div>
      </div>
      <div class="r-card">
        <div class="r-card-title">&#9651; Vimshottari Dasha</div>
        <div id="dashaCurrentBox" runat="server" class="dasha-active" style="margin-bottom:14px;">
          <div style="font-family:'Cinzel',serif;font-size:0.6rem;letter-spacing:2px;color:#8a6d2e;margin-bottom:4px;">
            CHALU MAHADASHA
          </div>
          <asp:Label ID="lblCurrentDasha" runat="server"
            style="font-family:'Cinzel',serif;font-size:1rem;color:#c9a84c;" />
          <div style="color:#9a8f7a;font-size:0.78rem;margin-top:2px;">
            Antardasha: <asp:Label ID="lblCurrentAntardasha" runat="server" style="color:#c9a84c;" />
          </div>
          <div style="color:#9a8f7a;font-size:0.75rem;margin-top:2px;">
            Remaining: <asp:Label ID="lblDashaRemaining" runat="server" style="color:#c9a84c;" /> years
          </div>
        </div>
        <asp:Repeater ID="rptDasha" runat="server">
          <ItemTemplate>
            <div class="dasha-row">
              <div class="dasha-planet-dot"></div>
              <div class="dasha-info">
                <div class="dasha-name"><%# Eval("Planet") %> Mahadasha</div>
                <div class="dasha-years"><%# Eval("From") %> — <%# Eval("To") %> (<%# Eval("Years") %> yr)</div>
                <div class="dasha-bar-wrap">
                  <div class="dasha-bar" style="width:<%# Eval("BarWidth") %>%;"></div>
                </div>
              </div>
            </div>
          </ItemTemplate>
        </asp:Repeater>
      </div>
    </div>

    <div style="height:20px;"></div>

    <!-- Row 4: Yogas + Doshas -->
    <div class="result-grid">
      <div class="r-card">
        <div class="r-card-title">&#9655; Raj Yoga & Shubh Yoga</div>
        <div id="divYogas" runat="server"></div>
      </div>
      <div class="r-card">
        <div class="r-card-title">&#9651; Dosh Vivaran</div>
        <div id="divDoshas" runat="server"></div>
      </div>
    </div>

    <div style="height:20px;"></div>

    <!-- Row 5: Predictions -->
    <div class="r-card">
      <div class="r-card-title">&#10022; Jyotish Fal (Predictions)</div>
      <div id="divPredictions" runat="server"></div>
    </div>

    <!-- Action Bar -->
    <div class="action-bar">
      <button onclick="window.print()" class="btn btn-outline">&#9112; Print Kundali</button>
      <asp:Button ID="btnSaveKundali" runat="server" Text="✦ Save Kundali"
        CssClass="btn btn-gold" OnClick="btnSaveKundali_Click" />
      <button onclick="document.getElementById('kundaliResult').style.display='none';window.scrollTo(0,0);"
        class="btn btn-outline">&#8635; Naya Banao</button>
    </div>

  </div><!-- /kundali-result -->

</div><!-- /kundali-wrap -->

<script>
// ============================================================
// 1. LOCATION AUTOCOMPLETE — OpenStreetMap Nominatim (free)
// ============================================================
let locationTimeout;
function searchLocation(val) {
    clearTimeout(locationTimeout);
    const box = document.getElementById('locationSuggestions');
    if (val.length < 3) { box.innerHTML = ''; return; }
    locationTimeout = setTimeout(() => {
        fetch(`https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(val)}&format=json&addressdetails=1&limit=8&countrycodes=in,pk,np,bd,lk,us,gb,ca,au,de,fr,ae,sg,nz`)
            .then(r => r.json()).then(data => {
                box.innerHTML = '';
                data.forEach(p => {
                    const div = document.createElement('div');
                    const shortName = p.display_name.split(',').slice(0,3).join(', ');
                    div.textContent = shortName;
                    div.title = p.display_name;
                    div.onclick = () => {
                        document.getElementById('<%= txtBirthPlace.ClientID %>').value = shortName;
                        document.getElementById('<%= hfLatitude.ClientID %>').value  = p.lat;
                        document.getElementById('<%= hfLongitude.ClientID %>').value = p.lon;
                        // Compute UTC offset from lon (rough estimate; server will refine)
                        const tz = getTimezone(parseFloat(p.lat), parseFloat(p.lon));
                        document.getElementById('<%= hfTimezone.ClientID %>').value = tz;
                        box.innerHTML = '';
                    };
                    box.appendChild(div);
                });
            });
    }, 350);
}

function getTimezone(lat, lon) {
    // India: UTC+5:30
    if (lat >= 8 && lat <= 36 && lon >= 68 && lon <= 98) return 'Asia/Kolkata';
    // Pakistan
    if (lat >= 23 && lat <= 37 && lon >= 61 && lon <= 77) return 'Asia/Karachi';
    // Nepal
    if (lat >= 26 && lat <= 31 && lon >= 80 && lon <= 89) return 'Asia/Kathmandu';
    // Bangladesh
    if (lat >= 20 && lat <= 27 && lon >= 88 && lon <= 93) return 'Asia/Dhaka';
    // UAE
    if (lon >= 51 && lon <= 57) return 'Asia/Dubai';
    // UK
    if (lon >= -5 && lon <= 2 && lat >= 49 && lat <= 60) return 'Europe/London';
    // US (rough)
    if (lon <= -115) return 'America/Los_Angeles';
    if (lon <= -100) return 'America/Denver';
    if (lon <= -85)  return 'America/Chicago';
    if (lon <= -65)  return 'America/New_York';
    // Default IST
    return 'Asia/Kolkata';
}

document.addEventListener('click', e => {
    if (!e.target.closest('.loc-wrap')) {
        document.getElementById('locationSuggestions').innerHTML = '';
    }
});

// ============================================================
// 2. DRAW NORTH INDIAN KUNDALI CHART (SVG)
// ============================================================
function drawKundali(chartDataJson) {
    if (!chartDataJson) return;
    let houses;
    try { houses = JSON.parse(chartDataJson); } catch(e) { return; }

    const svg = document.getElementById('kundaliSvg');
    if (!svg) return;
    svg.innerHTML = '';

    const S = 340, C = S/2;
    const gold = 'rgba(201,168,76,0.6)';
    const goldFill = 'rgba(201,168,76,0.04)';
    const goldText = '#c9a84c';
    const dimText = '#9a8f7a';

    function line(x1,y1,x2,y2) {
        const el = document.createElementNS('http://www.w3.org/2000/svg','line');
        el.setAttribute('x1',x1); el.setAttribute('y1',y1);
        el.setAttribute('x2',x2); el.setAttribute('y2',y2);
        el.setAttribute('stroke',gold); el.setAttribute('stroke-width','1');
        svg.appendChild(el); return el;
    }
    function rect(x,y,w,h,fill) {
        const el = document.createElementNS('http://www.w3.org/2000/svg','rect');
        el.setAttribute('x',x); el.setAttribute('y',y);
        el.setAttribute('w',w); el.setAttribute('height',h); el.setAttribute('width',w);
        el.setAttribute('fill',fill||goldFill);
        el.setAttribute('stroke',gold); el.setAttribute('stroke-width','0.5');
        svg.appendChild(el); return el;
    }
    function poly(pts, fill) {
        const el = document.createElementNS('http://www.w3.org/2000/svg','polygon');
        el.setAttribute('points', pts.map(p=>p.join(',')).join(' '));
        el.setAttribute('fill', fill||goldFill);
        el.setAttribute('stroke',gold); el.setAttribute('stroke-width','1');
        svg.appendChild(el); return el;
    }
    function text(x,y,t,sz,col,anchor) {
        const el = document.createElementNS('http://www.w3.org/2000/svg','text');
        el.setAttribute('x',x); el.setAttribute('y',y);
        el.setAttribute('font-size',sz||9);
        el.setAttribute('fill',col||dimText);
        el.setAttribute('text-anchor',anchor||'middle');
        el.setAttribute('font-family','Cinzel,serif');
        el.textContent = t;
        svg.appendChild(el); return el;
    }

    const M = 10; // margin
    const W = S - 2*M;

    // Outer square
    rect(M, M, W, W, 'transparent');

    // Diagonals (house dividers)
    line(M, M, C, C);
    line(M+W, M, C, C);
    line(M, M+W, C, C);
    line(M+W, M+W, C, C);

    // Cross lines
    line(C, M, C, M+W);
    line(M, C, M+W, C);

    // Inner square (rotated 45°)
    const Q = W/4;
    line(C, M+Q, M+Q, C);
    line(C, M+Q, M+W-Q, C);
    line(C, M+W-Q, M+Q, C);
    line(C, M+W-Q, M+W-Q, C);

    // House number positions (North Indian fixed layout)
    // House 1 = top-center diamond, going clockwise
    const housePositions = [
        {cx:C, cy:M+Q*0.7},          // H1  top diamond
        {cx:M+Q*0.5, cy:M+Q*0.5},    // H2  top-left triangle
        {cx:M+Q*0.5, cy:C},           // H3  left-top triangle
        {cx:M+Q*0.5, cy:C+Q*1.5},    // H4  left diamond (actually bottom left area)
        {cx:M+Q*0.5, cy:M+W-Q*0.5},  // H5  bottom-left triangle
        {cx:C,        cy:M+W-Q*0.7}, // H6  bottom diamond
        {cx:M+W-Q*0.5, cy:M+W-Q*0.5},// H7 bottom-right triangle
        {cx:M+W-Q*0.5, cy:C},         // H8  right-bottom triangle
        {cx:M+W-Q*0.5, cy:M+Q*0.5},  // H9  right-top triangle
        {cx:C,         cy:M+Q*0.5+20},// H10 center-top area (already used)
        {cx:M+W-Q*0.5, cy:C-Q},       // H11
        {cx:C+Q*0.5,  cy:M+Q*0.5},    // H12
    ];

    // North Indian chart: fixed house positions
    // House numbers are fixed, signs rotate. Let's use proper positions.
    // Positions for 12 houses in North Indian chart:
    const ni = [
        // H1 top-center
        { pts:[[C-Q,M+Q],[C+Q,M+Q],[C,M+Q*2],[C,M]], label:{x:C, y:M+Q+16} },
        // H2 top-left
        { pts:[[M,M],[C-Q,M+Q],[C,M]], label:{x:M+Q*0.7, y:M+Q*0.7} },
        // H3 left-top
        { pts:[[M,M],[M+Q*2,C],[C-Q,M+Q]], label:{x:M+Q*0.6, y:C-Q*0.6} },
        // H4 left-center
        { pts:[[M,M+Q*2],[M+Q*2,C],[M,C+Q*2]], label:{x:M+Q*0.5, y:C} },
        // H5 left-bottom
        { pts:[[M,M+W],[M+Q*2,C+Q*2],[C-Q,C+Q]], label:{x:M+Q*0.6, y:C+Q*1.6} },
        // H6 bottom-center
        { pts:[[M,M+W],[C-Q,C+Q],[C+Q,C+Q],[C,M+W]], label:{x:C, y:M+W-Q*0.7} },
        // H7 bottom-right
        { pts:[[C,M+W],[C+Q,C+Q],[M+W,M+W]], label:{x:M+W-Q*0.7, y:M+W-Q*0.7} },
        // H8 right-bottom
        { pts:[[M+W,M+W],[M+W-Q*2,C+Q*2],[C+Q,C+Q]], label:{x:M+W-Q*0.6, y:C+Q*1.6} },
        // H9 right-center
        { pts:[[M+W,M+Q*2],[C+Q,M+Q],[M+W-Q*2,C],[M+W,C+Q*2]], label:{x:M+W-Q*0.5, y:C} },
        // H10 right-top
        { pts:[[M+W,M],[C+Q,M+Q],[M+W-Q*2,C]], label:{x:M+W-Q*0.6, y:C-Q*1.6} },
        // H11 top-right
        { pts:[[M+W,M],[C,M],[C+Q,M+Q]], label:{x:M+W-Q*0.7, y:M+Q*0.7} },
        // H12 top-center-right (inner)
        { pts:[[C,M],[C+Q,M+Q],[C,M+Q*2],[C-Q,M+Q]], label:{x:C, y:M+Q+15} },
    ];

    // Draw each house cell & label
    ni.forEach((h, i) => {
        // Draw polygon
        poly(h.pts, goldFill);
        // House number (small, dim)
        text(h.label.x, h.label.y - 8, String(i+1), 7, 'rgba(201,168,76,0.3)', 'middle');
        // Planet text from chartData
        const houseNum = i + 1;
        const planetsInHouse = houses.filter(p => p.house === houseNum);
        if (planetsInHouse.length > 0) {
            const str = planetsInHouse.map(p=>p.sym).join(' ');
            text(h.label.x, h.label.y + 4, str, 9, goldText, 'middle');
        }
        // Sign number
        const sign = houses.find(p => p.house === houseNum);
        if (sign && sign.signNum) {
            text(h.label.x, h.label.y + 14, '('+sign.signNum+')', 7, dimText, 'middle');
        }
    });

    // Center label
    text(C, C-5, 'KUNDALI', 8, 'rgba(201,168,76,0.4)', 'middle');
}

// Auto-render chart when page loads (if result is visible)
window.addEventListener('load', function() {
    const hf = document.getElementById('<%= hfChartData.ClientID %>');
    if (hf && hf.value) {
        const rd = document.getElementById('kundaliResult');
        if (rd) rd.style.display = 'block';
        drawKundali(hf.value);
    }
});
</script>
</asp:Content>
