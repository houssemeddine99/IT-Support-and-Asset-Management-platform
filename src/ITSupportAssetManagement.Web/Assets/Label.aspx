<%@ Page Title="Print asset label" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Label.aspx.vb" Inherits="ITSupportAssetManagement.Web.AssetLabelPage" %>
<asp:Content ID="AssetLabelContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="NotFoundPanel" runat="server" CssClass="panel empty-state" Visible="false">
        <span><i class="bi bi-search"></i></span><h2>Asset not found</h2><p>The requested equipment record does not exist.</p>
        <a class="button button-primary" href="List.aspx">Return to inventory</a>
    </asp:Panel>
    <asp:Panel ID="LabelPanel" runat="server" Visible="false">
        <div class="page-heading print-hidden"><div><a class="back-link" id="AssetDetailsLink" runat="server"><i class="bi bi-arrow-left"></i> Back to asset</a><h1>Asset QR label</h1><p>Print, cut, and attach this identification label to the equipment.</p></div><div class="heading-actions"><button type="button" class="button button-primary" onclick="window.print()"><i class="bi bi-printer"></i> Print label</button></div></div>
        <div class="asset-label-stage">
            <article class="asset-qr-label">
                <header><img src="../Content/Images/draexlmaier-logo.png" alt="DRÄXLMAIER" /><span>SILIANA IT HUB</span></header>
                <div class="asset-qr-body"><img id="QrImage" runat="server" class="asset-qr-image" alt="Asset QR code" /><div><small>ASSET TAG</small><strong><asp:Literal ID="AssetTag" runat="server" /></strong><h2><asp:Literal ID="AssetName" runat="server" /></h2><p><i class="bi bi-geo-alt"></i> <asp:Literal ID="Location" runat="server" /></p></div></div>
                <footer>Scan to open the live asset record</footer>
            </article>
        </div>
    </asp:Panel>
</asp:Content>
