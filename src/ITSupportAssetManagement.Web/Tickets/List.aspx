<%@ Page Title="Tickets" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="List.aspx.vb" Inherits="ITSupportAssetManagement.Web.TicketListPage" %>
<asp:Content ID="TicketContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading ticket-page-heading">
        <div><p class="eyebrow">Service desk</p><h1>Support tickets</h1><p>Track, prioritize, and resolve every IT request.</p></div>
        <a class="button button-primary" href="Create.aspx"><i class="bi bi-plus-lg"></i> New ticket</a>
    </div>

    <asp:Panel ID="SuccessPanel" runat="server" CssClass="page-alert success" Visible="false"><i class="bi bi-check-circle"></i><span>Ticket <asp:Literal ID="CreatedTicketNumber" runat="server" /> was created successfully.</span></asp:Panel>
    <asp:Panel ID="ErrorPanel" runat="server" CssClass="page-alert error" Visible="false"><i class="bi bi-exclamation-circle"></i><asp:Literal ID="ErrorMessage" runat="server" /></asp:Panel>

    <section class="panel tickets-workspace">
        <div class="ticket-toolbar">
            <div class="filter-search"><i class="bi bi-search"></i><asp:TextBox ID="SearchInput" runat="server" MaxLength="180" placeholder="Search by number, title, or requester" /></div>
            <asp:DropDownList ID="StatusFilter" runat="server"><asp:ListItem Value="">All statuses</asp:ListItem><asp:ListItem>Open</asp:ListItem><asp:ListItem>Assigned</asp:ListItem><asp:ListItem Value="InProgress">In progress</asp:ListItem><asp:ListItem>Waiting</asp:ListItem><asp:ListItem>Resolved</asp:ListItem><asp:ListItem>Closed</asp:ListItem></asp:DropDownList>
            <asp:DropDownList ID="PriorityFilter" runat="server"><asp:ListItem Value="">All priorities</asp:ListItem><asp:ListItem>Critical</asp:ListItem><asp:ListItem>High</asp:ListItem><asp:ListItem>Medium</asp:ListItem><asp:ListItem>Low</asp:ListItem></asp:DropDownList>
            <asp:DropDownList ID="SlaFilter" runat="server"><asp:ListItem Value="">All SLA states</asp:ListItem><asp:ListItem Value="attention">Needs attention</asp:ListItem><asp:ListItem Value="overdue">Overdue</asp:ListItem></asp:DropDownList>
            <asp:Button ID="FilterButton" runat="server" Text="Apply filters" CssClass="button button-secondary" />
        </div>
        <div class="ticket-table-wrap">
            <table class="data-table">
                <thead><tr><th>Ticket</th><th>Category</th><th>Priority</th><th>Status</th><th>SLA</th><th>Requester</th><th>Assignee</th><th>Created</th><th></th></tr></thead>
                <tbody>
                    <asp:Repeater ID="TicketRepeater" runat="server"><ItemTemplate>
                        <tr>
                            <td><a class="ticket-cell" href='<%# "Details.aspx?id=" & Convert.ToString(Eval("TicketId")) %>'><strong><%#: Eval("Title") %></strong><span><%#: Eval("TicketNumber") %><%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("AssetTag"))), "", " &middot; " & Server.HtmlEncode(Convert.ToString(Eval("AssetTag")))) %></span></a></td>
                            <td><span class="category-chip"><i class="bi bi-tag"></i><%#: Eval("CategoryName") %></span></td>
                            <td><span class='<%# Eval("PriorityCssClass") %>'><%#: Eval("Priority") %></span></td>
                            <td><span class='<%# Eval("StatusCssClass") %>'><i></i><%#: Convert.ToString(Eval("Status")).Replace("InProgress", "In progress") %></span></td>
                            <td><span class='<%# Eval("SlaCssClass") %>'><i class="bi bi-clock"></i><%#: Eval("SlaLabel") %></span></td>
                            <td><%#: Eval("RequestedByName") %></td><td><%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("AssignedToName"))), "Unassigned", Server.HtmlEncode(Convert.ToString(Eval("AssignedToName")))) %></td>
                            <td><%#: CType(Eval("CreatedAtUtc"), DateTime).ToString("dd MMM yyyy") %></td><td><a class="row-action" href='<%# "Details.aspx?id=" & Convert.ToString(Eval("TicketId")) %>' aria-label="Open ticket"><i class="bi bi-chevron-right"></i></a></td>
                        </tr>
                    </ItemTemplate></asp:Repeater>
                </tbody>
            </table>
            <asp:Panel ID="EmptyPanel" runat="server" CssClass="empty-state" Visible="false"><span><i class="bi bi-inbox"></i></span><h2>No tickets found</h2><p>Try different filters or create the first support ticket.</p><a class="button button-primary" href="Create.aspx">Create ticket</a></asp:Panel>
        </div>
        <div class="table-footer"><span><asp:Literal ID="ResultCount" runat="server" /> tickets</span><small>Sorted by priority and creation date</small></div>
    </section>
</asp:Content>
