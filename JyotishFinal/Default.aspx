<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Default" Title="Aura - Home" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
  /* ---- HERO ---- */
  .hero {
    text-align: center;
    padding: 80px 20px 50px;
    animation: fadeUp .8s ease both;
  }
  @keyframes fadeUp {
    from { opacity:0; transform:translateY(22px); }
    to   { opacity:1; transform:translateY(0); }
  }
  .hero-icon {
    font-size: 5rem; display: block;
    animation: float 4s ease-in-out infinite;
    filter: drop-shadow(0 0 20px rgba(201,168,76,.45));
    margin-bottom: 18px;
  }
  @keyframes float {
    0%,100% { transform:translateY(0); }
    50%      { transform:translateY(-12px); }
  }
  .hero h1 {
    font-family: 'Cinzel', serif;
    font-size: clamp(2rem,5vw,3.4rem);
    font-weight: 700; color: var(--gold);
    letter-spacing: 5px;
    text-shadow: 0 0 30px rgba(201,168,76,.3);
  }
  .hero h2 {
    font-family: 'Noto Sans Devanagari', sans-serif;
    font-size: clamp(.9rem,2.5vw,1.25rem);
    color: var(--text-dim); font-weight: 300;
    margin-top: 10px; letter-spacing: 2px;
  }
  .divider {
    width:120px; height:1px;
    background: linear-gradient(90deg, transparent, var(--gold), transparent);
    margin: 24px auto;
  }
  .hero p {
    font-size:1.1rem; color:var(--text-dim);
    max-width:460px; margin:0 auto 32px;
    line-height:1.75; font-style:italic;
  }

  /* ---- SECTION LABEL ---- */
  .section-label {
    font-family:'Cinzel',serif; font-size:.7rem;
    letter-spacing:4px; text-transform:uppercase;
    color:var(--gold-dim); text-align:center;
    margin-bottom:26px;
  }

  /* ---- RASHI GRID ---- */
  .rashi-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(130px,1fr));
    gap: 14px;
    max-width: 900px; margin: 0 auto 60px;
    padding: 0 20px;
  }
  .rashi-card {
    background: linear-gradient(145deg, var(--surface), var(--surface2));
    border: 1px solid rgba(201,168,76,.12);
    border-radius: 4px; padding:20px 10px;
    text-align:center; cursor:pointer;
    transition: all .3s ease;
  }
  .rashi-card:hover {
    border-color: rgba(201,168,76,.5);
    transform: translateY(-4px);
    box-shadow: 0 10px 28px rgba(0,0,0,.45), 0 0 16px rgba(201,168,76,.1);
    background: linear-gradient(145deg, #0f1b35, #152040);
  }
  .rashi-symbol {
    font-size:2rem; display:block; margin-bottom:8px;
    filter: drop-shadow(0 0 8px rgba(201,168,76,.3));
  }
  .rashi-en {
    font-family:'Cinzel',serif; font-size:.68rem;
    letter-spacing:1.5px; color:var(--gold);
    display:block; text-transform:uppercase;
  }
  .rashi-hi {
    font-family:'Noto Sans Devanagari',sans-serif;
    font-size:.85rem; color:var(--text-dim);
    display:block; margin-top:3px;
  }

  /* ---- TEASER BOX ---- */
  .teaser-wrap { max-width:660px; margin:0 auto 80px; padding:0 20px; }
  .teaser-box {
    background: linear-gradient(145deg, rgba(13,20,40,.95), rgba(17,26,51,.95));
    border: 1px solid rgba(201,168,76,.22);
    border-radius:4px; padding:36px 40px; text-align:center;
    position:relative;
  }
  .teaser-box::before {
    content:'&#10022;';
    position:absolute; top:-11px; left:50%; transform:translateX(-50%);
    background:var(--deep); padding:0 14px;
    color:var(--gold); font-size:1rem;
  }
  .teaser-box h3 {
    font-family:'Cinzel',serif; font-size:1.05rem;
    color:var(--gold); letter-spacing:2px; margin-bottom:12px;
  }
  .teaser-box p {
    color:var(--text-dim); font-style:italic;
    line-height:1.75; margin-bottom:22px; font-size:1rem;
  }
  .teaser-hint {
    color:var(--text-dim); font-style:italic; font-size:.9rem;
  }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

  <!-- HERO -->
  <div class="hero">
    <span class="hero-icon">&#128302;</span>
    <h1>Aura</h1>
    <h2>Vedic Astrology</h2>
    <div class="divider"></div>
    <p>Stars hold secrets. Let the cosmos guide your path through ancient wisdom and celestial insight.</p>
    <% if (Session["UserId"] == null) { %>
      <a class="btn btn-gold" href="Register.aspx"
         style="font-size:.82rem;padding:13px 36px;letter-spacing:2px;">
        Begin Your Journey
      </a>
    <% } else { %>
      <p style="color:var(--gold);font-family:'Cinzel',serif;letter-spacing:2px;font-size:.9rem;">
        &#10022; &nbsp; Welcome back, <%: Session["UserName"] %>! &nbsp; &#10022;
      </p>
    <% } %>
  </div>

  <!-- RASHI GRID -->
  <p class="section-label">&#10022; &nbsp; The Twelve Rashis &nbsp; &#10022;</p>

  <div class="rashi-grid">
    <asp:Repeater ID="rptRashis" runat="server">
      <ItemTemplate>
        <div class="rashi-card" onclick="rashiClick(<%# Eval("RashiId") %>)">
          <span class="rashi-symbol"><%# Eval("Symbol") %></span>
          <span class="rashi-en"><%# Eval("RashiEnglishName") %></span>
          <span class="rashi-hi"><%# Eval("RashiName") %></span>
        </div>
      </ItemTemplate>
    </asp:Repeater>
  </div>

  <!-- TEASER -->
  <div class="teaser-wrap">
    <div class="teaser-box">
      <h3>&#10022; &nbsp; Aaj Ka Rashifal &nbsp; &#10022;</h3>
      <% if (Session["UserId"] == null) { %>
        <p>Your daily horoscope awaits — personalized predictions, lucky numbers, and cosmic colors for each rashi. Login to unlock your reading.</p>
        <a class="btn btn-gold" href="Login.aspx" style="font-size:.78rem;padding:12px 30px;">
          Login to Read
        </a>
      <% } else { %>
        <p class="teaser-hint">Click on your rashi card above to read today's horoscope.</p>
      <% } %>
    </div>
  </div>

  <script type="text/javascript">
    function rashiClick(id) {
      <% if (Session["UserId"] != null) { %>
        alert('Rashi #' + id + ' horoscope — coming soon!');
      <% } else { %>
        window.location.href = 'Login.aspx';
      <% } %>
    }
  </script>

</asp:Content>
