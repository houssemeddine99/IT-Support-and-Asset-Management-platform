<%@ Page Title="My account" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Profile.aspx.vb" Inherits="ITSupportAssetManagement.Web.AccountProfilePage" %>
<asp:Content ID="AccountContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading account-heading"><div><p class="eyebrow"><i class="bi bi-shield-lock"></i> Personal security</p><h1>My account</h1><p>Review your workplace identity and protect access to the Siliana IT Hub.</p></div></div>
    <asp:Panel ID="RequiredPasswordPanel" runat="server" CssClass="page-alert warning" Visible="false"><i class="bi bi-shield-exclamation"></i><span>Your administrator issued a temporary password. Create your own password before continuing.</span></asp:Panel><asp:Panel ID="ErrorPanel" runat="server" CssClass="page-alert error" Visible="false"><i class="bi bi-exclamation-circle"></i><asp:Literal ID="ErrorMessage" runat="server" /></asp:Panel>
    <div class="account-grid">
        <aside class="panel account-card">
            <div class="account-identity"><span class="account-avatar"><asp:Literal ID="InitialsText" runat="server" /></span><div><small>Authenticated account</small><h2><asp:Literal ID="DisplayNameText" runat="server" /></h2><span><asp:Literal ID="RoleText" runat="server" /></span></div></div>
            <dl class="account-details">
                <div><dt>Work email</dt><dd><asp:Literal ID="EmailText" runat="server" /></dd></div>
                <div><dt>Employee code</dt><dd><asp:Literal ID="EmployeeCodeText" runat="server" /></dd></div>
                <div><dt>Department</dt><dd><asp:Literal ID="DepartmentText" runat="server" /></dd></div>
                <div><dt>Phone</dt><dd><asp:Literal ID="PhoneText" runat="server" /></dd></div>
            </dl>
            <p class="account-note"><i class="bi bi-info-circle"></i> Contact an administrator to change identity, role, or contact information.</p>
        </aside>
        <section class="panel account-security">
            <div class="panel-heading"><div><h2>Change password</h2><p>Your session will close after the password is updated.</p></div><span class="settings-icon violet"><i class="bi bi-key"></i></span></div>
            <div class="form-body">
                <div class="field"><label for="CurrentPasswordInput">Current password <em>Required</em></label><asp:TextBox ID="CurrentPasswordInput" runat="server" TextMode="Password" MaxLength="128" autocomplete="current-password" /><asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPasswordInput" ErrorMessage="Enter your current password." CssClass="field-error" Display="Dynamic" /></div>
                <div class="field"><label for="NewPasswordInput">New password <em>Required</em></label><asp:TextBox ID="NewPasswordInput" runat="server" TextMode="Password" MaxLength="128" autocomplete="new-password" /><span class="field-help">At least 12 characters with uppercase, lowercase, number, and symbol.</span><asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPasswordInput" ErrorMessage="Enter a new password." CssClass="field-error" Display="Dynamic" /></div>
                <div class="field"><label for="ConfirmPasswordInput">Confirm new password <em>Required</em></label><asp:TextBox ID="ConfirmPasswordInput" runat="server" TextMode="Password" MaxLength="128" autocomplete="new-password" /><asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPasswordInput" ErrorMessage="Confirm the new password." CssClass="field-error" Display="Dynamic" /><asp:CompareValidator ID="PasswordMatch" runat="server" ControlToValidate="ConfirmPasswordInput" ControlToCompare="NewPasswordInput" ErrorMessage="The new passwords do not match." CssClass="field-error" Display="Dynamic" /></div>
            </div>
            <div class="edit-actions"><a class="button button-secondary" href="../Default.aspx">Cancel</a><asp:Button ID="ChangePasswordButton" runat="server" Text="Update password" CssClass="button button-primary" /></div>
        </section>
    </div>
</asp:Content>
