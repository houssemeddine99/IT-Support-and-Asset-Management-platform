<%@ Page Title="Search" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Index.aspx.vb" Inherits="ITSupportAssetManagement.Web.SearchIndex" %>
<asp:Content ID="SearchContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading search-heading"><div><p class="eyebrow"><i class="bi bi-search"></i> Workspace search</p><h1>Results for “<asp:Literal ID="QueryText" runat="server" />”</h1><p>Tickets, equipment, and people matching your query.</p></div><span class="result-count"><asp:Literal ID="ResultCount" runat="server" /> results</span></div>
    <asp:Panel ID="AlertPanel" runat="server" CssClass="page-alert error" Visible="false"><i class="bi bi-exclamation-circle"></i><asp:Literal ID="AlertText" runat="server" /></asp:Panel>
    <section class="panel search-results-panel">
        <asp:Repeater ID="ResultRepeater" runat="server"><ItemTemplate>
            <a class="global-result" href='<%# ResolveUrl(Convert.ToString(Eval("NavigateUrl"))) %>'>
                <span class='<%# "result-icon " & ResultClass(Eval("EntityType")) %>'><i class='<%# ResultIcon(Eval("EntityType")) %>'></i></span>
                <span class="result-copy"><small><%#: Eval("EntityType") %></small><strong><%#: Eval("Title") %></strong><em><%#: Eval("Subtitle") %></em></span>
                <span class="result-state"><b><%#: Eval("Status") %></b><small><%#: FormatResultDate(Eval("UpdatedAtUtc")) %></small></span>
                <i class="bi bi-arrow-up-right"></i>
            </a>
        </ItemTemplate></asp:Repeater>
        <asp:Panel ID="EmptyPanel" runat="server" CssClass="search-empty" Visible="false"><span><i class="bi bi-search"></i></span><h2>No matching records</h2><p>Try an asset tag, ticket number, employee name, model, or location.</p></asp:Panel>
    </section>
</asp:Content>
