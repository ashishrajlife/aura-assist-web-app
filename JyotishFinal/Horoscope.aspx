<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" 
   CodeFile="Horoscope.aspx.cs" Inherits="Horoscope" Title="Aaj Ka Rashifal" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div style="max-width:700px;margin:60px auto;padding:0 20px;text-align:center;">

  <asp:Panel ID="pnlHoroscope" runat="server" Visible="false">
    <div style="font-size:4rem;margin-bottom:16px;">
      <asp:Label ID="lblSymbol" runat="server" />
    </div>
    <h2 style="font-family:'Cinzel',serif;color:#c9a84c;font-size:1.8rem;letter-spacing:3px;">
      <asp:Label ID="lblRashiName" runat="server" />
    </h2>
    <p style="color:#9a8f7a;font-family:'Cinzel',serif;font-size:0.75rem;letter-spacing:2px;margin:6px 0 30px;">
      <asp:Label ID="lblDate" runat="server" />
    </p>

    <div style="background:linear-gradient(145deg,#0d1428,#111a33);border:1px solid rgba(201,168,76,0.25);border-radius:4px;padding:36px 40px;margin-bottom:24px;position:relative;">
      <p style="font-size:1.15rem;line-height:1.85;color:#e8dfc8;font-style:italic;">
        <asp:Label ID="lblPrediction" runat="server" />
      </p>
    </div>

    <div style="display:flex;gap:16px;justify-content:center;flex-wrap:wrap;">
      <div style="background:#0d1428;border:1px solid rgba(201,168,76,0.2);border-radius:4px;padding:20px 30px;min-width:140px;">
        <p style="font-family:'Cinzel',serif;font-size:0.6rem;letter-spacing:2px;color:#8a6d2e;margin-bottom:8px;">LUCKY NUMBER</p>
        <p style="font-family:'Cinzel',serif;font-size:1.8rem;color:#c9a84c;">
          <asp:Label ID="lblLuckyNumber" runat="server" />
        </p>
      </div>
      <div style="background:#0d1428;border:1px solid rgba(201,168,76,0.2);border-radius:4px;padding:20px 30px;min-width:140px;">
        <p style="font-family:'Cinzel',serif;font-size:0.6rem;letter-spacing:2px;color:#8a6d2e;margin-bottom:8px;">LUCKY COLOR</p>
        <p style="font-family:'Cinzel',serif;font-size:1.3rem;color:#c9a84c;">
          <asp:Label ID="lblLuckyColor" runat="server" />
        </p>
      </div>
    </div>
  </asp:Panel>

  <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
    <p style="color:#9a8f7a;font-style:italic;font-size:1.1rem;">Aaj ka horoscope abhi available nahi hai. Baad mein aayein.</p>
  </asp:Panel>

  <div style="margin-top:36px;">
    <a href="UserDashboard.aspx" style="font-family:'Cinzel',serif;font-size:0.7rem;letter-spacing:2px;color:#8a6d2e;text-decoration:none;">
      &#8592; Wapas Jaayein
    </a>
  </div>
</div>
</asp:Content>