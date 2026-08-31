<%@ Page Title="Sign in" Language="VB" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="ITSupportAssetManagement.Web.LoginPage" %>
<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Sign in - NexaDesk</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&amp;family=Manrope:wght@700;800&amp;display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
    <link href="Content/auth.css" rel="stylesheet" />
</head>
<body>
    <form id="LoginForm" runat="server" class="auth-shell">
        <section class="auth-showcase">
            <a class="auth-brand" href="Login.aspx"><span><i class="bi bi-command"></i></span><strong>NexaDesk</strong></a>
            <div class="showcase-copy"><span class="showcase-pill"><i class="bi bi-stars"></i> IT operations, simplified</span><h1>Everything your IT team needs, in one place.</h1><p>Resolve requests faster, keep every asset visible, and plan maintenance before problems interrupt the business.</p></div>
            <div class="showcase-stats"><div><strong>3.2h</strong><span>Average resolution</span></div><div><strong>92%</strong><span>Healthy assets</span></div><div><strong>99.8%</strong><span>Service uptime</span></div></div>
        </section>
        <section class="auth-form-panel">
            <div class="auth-card">
                <div class="mobile-brand"><span><i class="bi bi-command"></i></span><strong>NexaDesk</strong></div>
                <p class="eyebrow">Welcome back</p><h2>Sign in to your workspace</h2><p class="auth-intro">Enter your account details to continue.</p>
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

