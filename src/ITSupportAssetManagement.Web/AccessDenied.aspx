<%@ Page Title="Access denied" Language="VB" MasterPageFile="~/Site.Master" %>
<asp:Content ID="AccessDeniedContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="panel access-denied"><span><i class="bi bi-shield-lock"></i></span><p class="eyebrow">Permission required</p><h1>This area is not available for your role.</h1><p>Your account is signed in correctly, but this action requires additional operational permissions.</p><div><a class="button button-primary" href="<%= ResolveUrl("~/Default.aspx") %>"><i class="bi bi-grid"></i> Return to overview</a><a class="button button-secondary" href="<%= ResolveUrl("~/Account/Profile.aspx") %>"><i class="bi bi-person"></i> My account</a></div></section>
</asp:Content>
