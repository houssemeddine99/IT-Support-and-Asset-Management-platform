<%@ Page Title="Sign in" Language="VB" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="ITSupportAssetManagement.Web.LoginPage" %>
<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Sign in - Siliana IT Hub</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&amp;family=Manrope:wght@700;800&amp;display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
    <link href="Content/auth.css?v=20260901.4" rel="stylesheet" />
</head>
<body>
    <form id="LoginForm" runat="server" class="auth-shell">
        <section class="auth-showcase">
            <a class="auth-brand" href="Login.aspx" aria-label="DRÄXLMAIER Siliana IT Hub"><img src="Content/Images/draexlmaier-logo.png" alt="DRÄXLMAIER" /><small>Siliana IT Hub</small></a>
            <div class="showcase-copy"><span class="showcase-kicker">Digital workplace · Siliana plant</span><h1>IT operations,<br />connected.</h1><p>A single workspace for service requests, equipment lifecycle, and preventive maintenance.</p><div class="capability-list"><span><i class="bi bi-inbox"></i> Service desk</span><span><i class="bi bi-laptop"></i> Asset control</span><span><i class="bi bi-tools"></i> Maintenance</span></div></div>
            <div class="showcase-footer"><span class="status-dot"></span><span><strong>Siliana IT Hub</strong><small>Internal operations platform</small></span></div>
        </section>
        <section class="auth-form-panel">
            <div class="auth-card">
                <div class="mobile-brand"><img src="Content/Images/draexlmaier-logo.png" alt="DRÄXLMAIER" /><small>Siliana IT Hub</small></div>
                <p class="eyebrow">Secure employee access</p><h2>Welcome back</h2><p class="auth-intro">Sign in with your company account to continue.</p>
                <asp:Panel ID="ErrorPanel" runat="server" CssClass="auth-alert" Visible="false"><i class="bi bi-exclamation-circle"></i><asp:Literal ID="ErrorMessage" runat="server" /></asp:Panel>
                <div class="field"><label for="EmailInput">Email address</label><div class="input-wrap"><i class="bi bi-envelope"></i><asp:TextBox ID="EmailInput" runat="server" TextMode="Email" MaxLength="254" autocomplete="email" placeholder="name@company.com" /></div><asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="EmailInput" ErrorMessage="Email is required." CssClass="field-error" Display="Dynamic" /></div>
                <div class="field"><div class="label-row"><label for="PasswordInput">Password</label><a href="#forgot">Forgot password?</a></div><div class="input-wrap"><i class="bi bi-lock"></i><asp:TextBox ID="PasswordInput" runat="server" TextMode="Password" MaxLength="128" autocomplete="current-password" placeholder="Enter your password" /></div><asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="PasswordInput" ErrorMessage="Password is required." CssClass="field-error" Display="Dynamic" /></div>
                <label class="remember"><asp:CheckBox ID="RememberInput" runat="server" /> <span>Keep me signed in</span></label>
                <asp:Button ID="LoginButton" runat="server" Text="Sign in" CssClass="auth-button" />
                <p class="auth-footnote"><i class="bi bi-shield-check"></i> Protected with secure password hashing and encrypted sessions.</p>
            </div>
        </section>
    </form>
</body>
</html>
