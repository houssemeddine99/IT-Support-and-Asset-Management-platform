<%@ Page Title="Assets" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="List.aspx.vb" Inherits="ITSupportAssetManagement.Web.AssetListPage" %>
<asp:Content ID="AssetListContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading ticket-page-heading"><div><p class="eyebrow">Asset management</p><h1>Equipment inventory</h1><p>Track ownership, location, condition, and warranty coverage.</p></div><a class="button button-primary" href="Create.aspx"><i class="bi bi-plus-lg"></i> Register asset</a></div>
    <asp:Panel ID="SuccessPanel" runat="server" CssClass="page-alert success" Visible="false"><i class="bi bi-check-circle"></i><span>Asset <asp:Literal ID="CreatedAssetTag" runat="server" /> was registered successfully.</span></asp:Panel>
    <asp:Panel ID="ErrorPanel" runat="server" CssClass="page-alert error" Visible="false"><i class="bi bi-exclamation-circle"></i><asp:Literal ID="ErrorMessage" runat="server" /></asp:Panel>
    <section class="panel tickets-workspace">
        <div class="ticket-toolbar"><div class="filter-search"><i class="bi bi-search"></i><asp:TextBox ID="SearchInput" runat="server" MaxLength="140" placeholder="Search asset tag, serial, manufacturer, or model" /></div><asp:DropDownList ID="CategoryFilter" runat="server" /><asp:DropDownList ID="StatusFilter" runat="server"><asp:ListItem Value="">All statuses</asp:ListItem><asp:ListItem>Available</asp:ListItem><asp:ListItem>Assigned</asp:ListItem><asp:ListItem Value="InMaintenance">In maintenance</asp:ListItem><asp:ListItem>Retired</asp:ListItem><asp:ListItem>Lost</asp:ListItem></asp:DropDownList><asp:Button ID="FilterButton" runat="server" Text="Apply filters" CssClass="button button-secondary" /></div>
        <div class="asset-card-grid"><asp:Repeater ID="AssetRepeater" runat="server"><ItemTemplate>
            <article class="asset-card">
                <div class="asset-card-top">
                    <span class="asset-type-icon"><i class="bi bi-pc-display-horizontal"></i></span>
                    <span class='<%# Convert.ToString(Eval("StatusCssClass")) %>'><i></i><%#: Convert.ToString(Eval("Status")).Replace("InMaintenance", "In maintenance") %></span>
                </div>
                <a class="asset-card-title" href='<%# "Details.aspx?id=" & Convert.ToString(Eval("AssetId")) %>'><strong><%#: Eval("DisplayName") %></strong><span><%#: Eval("AssetTag") %></span></a>
                <dl>
                    <div><dt>Category</dt><dd><%#: Eval("CategoryName") %></dd></div>
                    <div><dt>Serial number</dt><dd><%#: DisplayOrDefault(Eval("SerialNumber"), "Not recorded") %></dd></div>
                    <div><dt>Location</dt><dd><%#: DisplayOrDefault(Eval("Location"), "Unspecified") %></dd></div>
                    <div><dt>Assigned to</dt><dd><%#: DisplayOrDefault(Eval("AssignedToName"), "Unassigned") %></dd></div>
                </dl>
                <a class="asset-open" href='<%# "Details.aspx?id=" & Convert.ToString(Eval("AssetId")) %>'>View asset <i class="bi bi-arrow-right"></i></a>
            </article>
        </ItemTemplate></asp:Repeater></div>
        <asp:Panel ID="EmptyPanel" runat="server" CssClass="empty-state" Visible="false"><span><i class="bi bi-laptop"></i></span><h2>No assets found</h2><p>Change the filters or register your first company asset.</p><a class="button button-primary" href="Create.aspx">Register asset</a></asp:Panel>
        <div class="table-footer"><span><asp:Literal ID="ResultCount" runat="server" /> assets</span><div class="pager"><asp:LinkButton ID="PreviousButton" runat="server" CssClass="pager-button" CausesValidation="false"><i class="bi bi-chevron-left"></i> Previous</asp:LinkButton><span><asp:Literal ID="PageText" runat="server" /></span><asp:LinkButton ID="NextButton" runat="server" CssClass="pager-button" CausesValidation="false">Next <i class="bi bi-chevron-right"></i></asp:LinkButton></div><small>Most recently registered first</small></div>
    </section>
</asp:Content>
