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
    <link href="Content/auth.css?v=20260901.6" rel="stylesheet" />
</head>
<body>
    <form id="LoginForm" runat="server" class="auth-shell login-template">
        <section class="auth-showcase">
            <div class="showcase-art" aria-hidden="true"></div>
            <a class="auth-brand" href="Login.aspx" aria-label="DRÄXLMAIER Siliana IT Hub"><img src="Content/Images/draexlmaier-logo.png" alt="DRÄXLMAIER" /><small>Siliana IT Hub</small></a>
            <div class="showcase-copy"><span class="showcase-kicker">Digital workplace · Siliana plant</span><h1>The platform where IT service, assets, and maintenance work together.</h1><p>A trusted internal workspace built for the DRÄXLMAIER Siliana operations team.</p></div>
            <div class="showcase-footer"><span>Copyright &copy; 2026 DRÄXLMAIER Group. Internal use only.</span></div>
        </section>
        <section class="auth-form-panel">
            <div class="auth-card">
                <div class="mobile-brand"><img src="Content/Images/draexlmaier-logo.png" alt="DRÄXLMAIER" /><small>Siliana IT Hub</small></div>
                <p class="eyebrow">Siliana IT Hub</p><h2>Welcome back!</h2><p class="auth-intro">Sign in to your account to continue</p>
                <asp:Panel ID="SuccessPanel" runat="server" CssClass="auth-alert success" Visible="false"><i class="bi bi-check-circle"></i><asp:Literal ID="SuccessMessage" runat="server" /></asp:Panel>
                <asp:Panel ID="ErrorPanel" runat="server" CssClass="auth-alert" Visible="false"><i class="bi bi-exclamation-circle"></i><asp:Literal ID="ErrorMessage" runat="server" /></asp:Panel>
                <div class="social-signin">
                    <button type="button" class="google-btn"><i class="bi bi-google"></i> Continue with Google</button>
                </div>
                <div class="divider"><span>or</span></div>
                <div class="field"><label for="EmailInput">Email address</label><div class="input-wrap"><i class="bi bi-envelope"></i><asp:TextBox ID="EmailInput" runat="server" TextMode="Email" MaxLength="254" autocomplete="email" placeholder="name@company.com" /></div><asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="EmailInput" ErrorMessage="Email is required." CssClass="field-error" Display="Dynamic" /></div>
                <div class="field"><div class="label-row"><label for="PasswordInput">Password</label><a href="#forgot">Forgot password?</a></div><div class="input-wrap"><i class="bi bi-lock"></i><asp:TextBox ID="PasswordInput" runat="server" TextMode="Password" MaxLength="128" autocomplete="current-password" placeholder="Enter your password" /></div><asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="PasswordInput" ErrorMessage="Password is required." CssClass="field-error" Display="Dynamic" /></div>
                <label class="remember"><asp:CheckBox ID="RememberInput" runat="server" /> <span>Keep me signed in</span></label>
                <asp:Button ID="LoginButton" runat="server" Text="Sign in" CssClass="auth-button" />
                <p class="auth-help">Need access? <span>Contact your IT administrator</span></p>
                <p class="auth-footnote"><i class="bi bi-shield-check"></i> Protected with secure password hashing and encrypted sessions.</p>
            </div>
        </section>
    </form>
</body>
</html>
