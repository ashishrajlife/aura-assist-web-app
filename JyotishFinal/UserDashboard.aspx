<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
   CodeFile="UserDashboard.aspx.cs" Inherits="UserDashboard" Title="My Dashboard" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div style="max-width:960px;margin:50px auto;padding:0 20px;">

  <h2 style="font-family:'Cinzel',serif;color:#c9a84c;letter-spacing:3px;font-size:1.4rem;margin-bottom:4px;">
    Namaste, <asp:Label ID="lblName" runat="server" />
  </h2>
  <p style="color:#9a8f7a;font-family:'Cinzel',serif;font-size:0.7rem;letter-spacing:2px;margin-bottom:40px;">
    &#10022; &nbsp; AAPKA COSMIC DASHBOARD &nbsp; &#10022;
  </p>

  <!-- Stats Row -->
  <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:14px;margin-bottom:36px;">
    <div style="background:#0d1428;border:1px solid rgba(201,168,76,0.2);border-radius:4px;padding:24px;text-align:center;">
      <p style="font-family:'Cinzel',serif;font-size:0.6rem;letter-spacing:2px;color:#8a6d2e;margin-bottom:10px;">MEMBER SINCE</p>
      <p style="color:#c9a84c;font-size:1rem;font-family:'Cinzel',serif;">
        <asp:Label ID="lblJoinDate" runat="server" />
      </p>
    </div>
    <div style="background:#0d1428;border:1px solid rgba(201,168,76,0.2);border-radius:4px;padding:24px;text-align:center;">
      <p style="font-family:'Cinzel',serif;font-size:0.6rem;letter-spacing:2px;color:#8a6d2e;margin-bottom:10px;">LAST LOGIN</p>
      <p style="color:#c9a84c;font-size:1rem;font-family:'Cinzel',serif;">
        <asp:Label ID="lblLastLogin" runat="server" />
      </p>
    </div>
  </div>

  <!-- ✦ KUNDALI CTA CARD ✦ -->
  <div style="background:linear-gradient(135deg,rgba(201,168,76,0.08),rgba(107,141,214,0.06));
              border:1px solid rgba(201,168,76,0.3);border-radius:6px;
              padding:32px 36px;margin-bottom:36px;text-align:center;position:relative;overflow:hidden;">
    <div style="position:absolute;top:-20px;right:-20px;font-size:8rem;opacity:0.04;pointer-events:none;">&#9963;</div>
    <p style="font-family:'Cinzel',serif;font-size:0.65rem;letter-spacing:3px;color:#8a6d2e;margin-bottom:8px;">
      &#10022; VEDIC ASTROLOGY &#10022;
    </p>
    <h3 style="font-family:'Cinzel',serif;color:#c9a84c;font-size:1.3rem;letter-spacing:2px;margin-bottom:10px;">
      Apni Janm Kundali Banao
    </h3>
    <p style="color:#9a8f7a;font-size:0.9rem;max-width:500px;margin:0 auto 22px;">
      Naam, janm tithi, samay aur sthan dalkar apni poori Vedic Kundali pao —
      graha sthiti, nakshatra, dasha, yogas aur jyotish fal sahit.
    </p>
    <a href="Kundali.aspx" class="btn btn-gold"
       style="font-family:'Cinzel',serif;font-size:0.78rem;letter-spacing:2px;
              padding:12px 36px;border-radius:2px;text-decoration:none;
              background:linear-gradient(135deg,#8a6d2e,#c9a84c);color:#050810;font-weight:700;display:inline-block;">
      &#9963; &nbsp; KUNDALI BANAO &nbsp; &#9963;
    </a>
  </div>

  <!-- Rashi Grid -->
  <p style="font-family:'Cinzel',serif;font-size:0.7rem;letter-spacing:4px;color:#8a6d2e;text-align:center;margin-bottom:24px;">
    &#10022; &nbsp; APNI RASHI CHUNEIN &nbsp; &#10022;
  </p>
  <div style="display:grid;grid-template-columns:repeat(auto-fill,minmax(130px,1fr));gap:12px;margin-bottom:50px;">
    <asp:Repeater ID="rptRashis" runat="server">
      <ItemTemplate>
        <div onclick="window.location.href='Horoscope.aspx?rashiId=<%# Eval("RashiId") %>'"
             style="background:linear-gradient(145deg,#0d1428,#111a33);border:1px solid rgba(201,168,76,0.12);
                    border-radius:4px;padding:20px 10px;text-align:center;cursor:pointer;transition:all 0.3s;"
             onmouseover="this.style.borderColor='rgba(201,168,76,0.5)';this.style.transform='translateY(-3px)'"
             onmouseout="this.style.borderColor='rgba(201,168,76,0.12)';this.style.transform='translateY(0)'">
          <span style="font-size:2rem;display:block;margin-bottom:6px;"><%# Eval("Symbol") %></span>
          <span style="font-family:'Cinzel',serif;font-size:0.68rem;letter-spacing:1.5px;color:#c9a84c;display:block;text-transform:uppercase;"><%# Eval("RashiEnglishName") %></span>
        </div>
      </ItemTemplate>
    </asp:Repeater>
  </div>

</div>
</asp:Content>
