<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Register.aspx.cs" Inherits="Register" Title="Jyotish - Register" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
  .auth-wrap {
    min-height: calc(100vh - 72px);
    display:flex; align-items:center; justify-content:center;
    padding: 40px 20px;
    animation: fadeUp .7s ease both;
  }
  @keyframes fadeUp {
    from { opacity:0; transform:translateY(18px); }
    to   { opacity:1; transform:translateY(0); }
  }
  .auth-box {
    background: linear-gradient(160deg, rgba(13,20,40,.97), rgba(11,16,35,.97));
    border: 1px solid rgba(201,168,76,.22);
    border-radius:4px; padding:48px 44px;
    width:100%; max-width:420px; position:relative;
  }
  .auth-box::before {
    content:'';
    position:absolute; top:0; left:0; right:0; height:2px;
    background: linear-gradient(90deg, transparent, var(--gold), transparent);
  }
  .auth-icon  { text-align:center; font-size:2.2rem; margin-bottom:6px; }
  .auth-title { font-family:'Cinzel',serif; font-size:1.4rem; font-weight:700; color:var(--gold); text-align:center; letter-spacing:3px; margin-bottom:4px; }
  .auth-sub   { font-size:.9rem; color:var(--text-dim); text-align:center; margin-bottom:26px; font-style:italic; }
  .line       { width:80px; height:1px; background:linear-gradient(90deg,transparent,var(--gold-dim),transparent); margin:0 auto 26px; }

  .form-group { margin-bottom:18px; }
  .form-label {
    display:block; font-family:'Cinzel',serif; font-size:.63rem;
    letter-spacing:2px; text-transform:uppercase;
    color:var(--gold-dim); margin-bottom:7px;
  }
  .form-group input[type=text],
  .form-group input[type=email],
  .form-group input[type=password] {
    width:100%; background:rgba(5,8,16,.7);
    border:1px solid rgba(201,168,76,.2); border-radius:2px;
    padding:12px 15px; color:var(--text);
    font-family:'Crimson Text',serif; font-size:1rem;
    transition:all .25s; outline:none;
  }
  .form-group input:focus {
    border-color:var(--gold-dim);
    background:rgba(10,15,30,.9);
    box-shadow:0 0 0 3px rgba(201,168,76,.08);
  }
  .form-group input::placeholder { color:rgba(154,143,122,.4); }

  /* Validator messages */
  .form-group span {
    font-size:.78rem; color:var(--error);
    margin-top:4px; font-style:italic; display:block;
  }

  /* Server message */
  .msg {
    padding:12px 15px; border-radius:2px;
    font-size:.92rem; margin-bottom:16px; border:1px solid;
  }
  .msg-ok  { background:rgba(80,180,120,.1); border-color:rgba(80,180,120,.3); color:var(--success); }
  .msg-err { background:rgba(220,100,100,.1); border-color:rgba(220,100,100,.3); color:var(--error); }

  .switch { text-align:center; margin-top:20px; font-size:.9rem; color:var(--text-dim); font-style:italic; }
  .switch a { color:var(--gold); text-decoration:none; font-family:'Cinzel',serif; font-size:.72rem; letter-spacing:1px; font-style:normal; }
  .switch a:hover { color:var(--gold-lt); }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

<div class="auth-wrap">
  <div class="auth-box">

    <div class="auth-icon">&#10022;</div>
    <h2 class="auth-title">REGISTER</h2>
    <p class="auth-sub">Create your account</p>
    <div class="line"></div>

    <!-- Server message -->
    <asp:Panel ID="pnlMsg" runat="server" Visible="false">
      <div id="divMsg" runat="server" class="msg"></div>
    </asp:Panel>

    <!-- Full Name -->
    <div class="form-group">
      <label class="form-label" for="txtFullName">Full Name</label>
      <asp:TextBox ID="txtFullName" runat="server" TextMode="SingleLine"
        placeholder="Your full name" MaxLength="100" />
      <asp:RequiredFieldValidator ID="rfvName" runat="server"
        ControlToValidate="txtFullName" Display="Dynamic"
        ErrorMessage="Name required hai." />
    </div>

    <!-- Email -->
    <div class="form-group">
      <label class="form-label" for="txtEmail">Email Address</label>
      <asp:TextBox ID="txtEmail" runat="server" TextMode="Email"
        placeholder="your@email.com" MaxLength="100" />
      <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
        ControlToValidate="txtEmail" Display="Dynamic"
        ErrorMessage="Email required hai." />
      <asp:RegularExpressionValidator ID="revEmail" runat="server"
        ControlToValidate="txtEmail" Display="Dynamic"
        ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
        ErrorMessage="Valid email daalo." />
    </div>

    <!-- Password -->
    <div class="form-group">
      <label class="form-label" for="txtPassword">Password</label>
      <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
        placeholder="Min. 6 characters" MaxLength="255" />
      <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
        ControlToValidate="txtPassword" Display="Dynamic"
        ErrorMessage="Password required hai." />
      <asp:RegularExpressionValidator ID="revPassword" runat="server"
        ControlToValidate="txtPassword" Display="Dynamic"
        ValidationExpression=".{6,}"
        ErrorMessage="Password kam se kam 6 characters ka hona chahiye." />
    </div>

    <!-- Confirm Password -->
    <div class="form-group">
      <label class="form-label" for="txtConfirm">Confirm Password</label>
      <asp:TextBox ID="txtConfirm" runat="server" TextMode="Password"
        placeholder="Repeat password" MaxLength="255" />
      <asp:RequiredFieldValidator ID="rfvConfirm" runat="server"
        ControlToValidate="txtConfirm" Display="Dynamic"
        ErrorMessage="Confirm password required hai." />
      <asp:CompareValidator ID="cvPassword" runat="server"
        ControlToValidate="txtConfirm" ControlToCompare="txtPassword"
        Display="Dynamic"
        ErrorMessage="Passwords match nahi kar rahe." />
    </div>

    <!-- Submit -->
    <asp:Button ID="btnRegister" runat="server" Text="Create Account"
      CssClass="btn btn-gold btn-full" OnClick="btnRegister_Click" />

    <div class="switch" style="margin-top:20px;">
      Already have an account? &nbsp;<a href="Login.aspx">Sign In</a>
    </div>
    <div class="switch" style="margin-top:10px;">
      <a href="Default.aspx">&larr; Back to Home</a>
    </div>

  </div>
</div>

</asp:Content>
