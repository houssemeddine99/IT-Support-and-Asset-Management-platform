<%@ Page Title="Home" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="ITSupportAssetManagement.Web.HomePage" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="hero-panel p-5 rounded-4">
        <span class="badge text-bg-primary mb-3">VB.NET Web Platform</span>
        <h1 class="display-5 fw-bold">IT Support and Asset Management</h1>
        <p class="lead mb-4">Manage support tickets, company equipment, and computer maintenance from one platform.</p>
        <div class="row g-3">
            <div class="col-md-4"><div class="module-card"><h2>Support tickets</h2><p>Report, assign, prioritize, and resolve IT requests.</p></div></div>
            <div class="col-md-4"><div class="module-card"><h2>Equipment</h2><p>Track assets, assignments, availability, and history.</p></div></div>
            <div class="col-md-4"><div class="module-card"><h2>Maintenance</h2><p>Record diagnostics, interventions, parts, and status.</p></div></div>
        </div>
    </section>
</asp:Content>

