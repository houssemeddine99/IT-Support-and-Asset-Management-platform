<%@ Page Title="Overview" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="ITSupportAssetManagement.Web.HomePage" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div><p class="eyebrow">Monday, 31 August</p><h1>Good morning, Houssem</h1><p>Here is what is happening across your IT operations today.</p></div>
        <div class="heading-actions"><button class="button button-secondary" type="button"><i class="bi bi-download"></i> Export report</button><a class="button button-primary" href="Tickets/Create.aspx"><i class="bi bi-plus-lg"></i> New ticket</a></div>
    </div>
    <section class="metric-grid" aria-label="Key metrics">
        <article class="metric-card metric-blue"><div class="metric-top"><span class="metric-icon"><i class="bi bi-inbox"></i></span><span class="trend up"><i class="bi bi-arrow-up-right"></i> 8.2%</span></div><p>Open tickets</p><div class="metric-value">24</div><small>12 require attention</small></article>
        <article class="metric-card metric-violet"><div class="metric-top"><span class="metric-icon"><i class="bi bi-laptop"></i></span><span class="trend up"><i class="bi bi-arrow-up-right"></i> 2.4%</span></div><p>Total assets</p><div class="metric-value">312</div><small>286 currently assigned</small></article>
        <article class="metric-card metric-orange"><div class="metric-top"><span class="metric-icon"><i class="bi bi-tools"></i></span><span class="trend down"><i class="bi bi-arrow-down-right"></i> 1.6%</span></div><p>In maintenance</p><div class="metric-value">8</div><small>3 overdue interventions</small></article>
        <article class="metric-card metric-green"><div class="metric-top"><span class="metric-icon"><i class="bi bi-stopwatch"></i></span><span class="trend up"><i class="bi bi-arrow-up-right"></i> 12%</span></div><p>Avg. resolution</p><div class="metric-value">3.2h</div><small>Target is under 4 hours</small></article>
    </section>
    <div class="dashboard-grid">
        <section class="panel ticket-panel" id="tickets">
            <div class="panel-heading"><div><h2>Priority tickets</h2><p>Requests that need your team&rsquo;s attention</p></div><a href="#all-tickets">View all <i class="bi bi-arrow-right"></i></a></div>
            <div class="ticket-list">
                <article class="ticket-row"><span class="priority priority-critical"></span><div class="ticket-main"><div><span class="ticket-id">#INC-2048</span><span class="status status-critical">Critical</span></div><h3>Production server is unreachable</h3><p><i class="bi bi-building"></i> Infrastructure &middot; Reported 18 min ago</p></div><div class="assignee"><span class="avatar avatar-indigo">MK</span><span><strong>Malek K.</strong><small>Infrastructure</small></span></div><button class="more-button" type="button" aria-label="Ticket options"><i class="bi bi-three-dots"></i></button></article>
                <article class="ticket-row"><span class="priority priority-high"></span><div class="ticket-main"><div><span class="ticket-id">#INC-2042</span><span class="status status-high">High</span></div><h3>VPN connection fails after update</h3><p><i class="bi bi-shield-lock"></i> Network &middot; Reported 1h ago</p></div><div class="assignee"><span class="avatar avatar-cyan">SA</span><span><strong>Sarra A.</strong><small>Network</small></span></div><button class="more-button" type="button" aria-label="Ticket options"><i class="bi bi-three-dots"></i></button></article>
                <article class="ticket-row"><span class="priority priority-medium"></span><div class="ticket-main"><div><span class="ticket-id">#REQ-2039</span><span class="status status-medium">Medium</span></div><h3>New employee laptop setup</h3><p><i class="bi bi-person-plus"></i> Hardware &middot; Reported 3h ago</p></div><div class="assignee"><span class="avatar avatar-rose">YA</span><span><strong>Yasmine A.</strong><small>Support</small></span></div><button class="more-button" type="button" aria-label="Ticket options"><i class="bi bi-three-dots"></i></button></article>
                <article class="ticket-row"><span class="priority priority-low"></span><div class="ticket-main"><div><span class="ticket-id">#REQ-2036</span><span class="status status-low">Low</span></div><h3>Adobe license renewal request</h3><p><i class="bi bi-key"></i> Software · Reported yesterday</p></div><div class="assignee"><span class="avatar avatar-amber">HB</span><span><strong>Hamza B.</strong><small>Software</small></span></div><button class="more-button" type="button" aria-label="Ticket options"><i class="bi bi-three-dots"></i></button></article>
            </div>
        </section>
        <section class="panel health-panel" id="assets">
            <div class="panel-heading"><div><h2>Asset health</h2><p>Current fleet condition</p></div><button class="more-button" type="button" aria-label="Asset options"><i class="bi bi-three-dots"></i></button></div>
            <div class="health-chart" role="img" aria-label="92 percent of assets are healthy"><div class="health-ring"><div><strong>92%</strong><span>Healthy</span></div></div></div>
            <div class="health-legend"><div><span class="legend-dot healthy"></span><p><strong>286</strong> Healthy</p><em>91.7%</em></div><div><span class="legend-dot warning"></span><p><strong>18</strong> Needs attention</p><em>5.8%</em></div><div><span class="legend-dot danger"></span><p><strong>8</strong> In maintenance</p><em>2.5%</em></div></div>
            <button class="button button-quiet" type="button">Open asset inventory <i class="bi bi-arrow-right"></i></button>
        </section>
    </div>
    <div class="dashboard-grid lower-grid">
        <section class="panel activity-panel" id="maintenance">
            <div class="panel-heading"><div><h2>Recent activity</h2><p>Latest updates from your IT team</p></div><select aria-label="Activity period"><option>Today</option><option>This week</option></select></div>
            <div class="timeline"><div class="timeline-item"><span class="timeline-icon success"><i class="bi bi-check-lg"></i></span><div><p><strong>Ticket #INC-2028 resolved</strong> by Sarra A.</p><small>Printer connection restored &middot; 12 minutes ago</small></div></div><div class="timeline-item"><span class="timeline-icon info"><i class="bi bi-laptop"></i></span><div><p><strong>Asset LT-0147 assigned</strong> to Anis M.</p><small>Dell Latitude 5440 &middot; 46 minutes ago</small></div></div><div class="timeline-item"><span class="timeline-icon warning"><i class="bi bi-wrench"></i></span><div><p><strong>Maintenance scheduled</strong> by Malek K.</p><small>Server SRV-003 &middot; Today at 18:00</small></div></div></div>
        </section>
        <section class="panel quick-panel">
            <div class="panel-heading"><div><h2>Quick actions</h2><p>Common IT workflows</p></div></div>
            <div class="quick-grid"><button type="button"><span class="quick-icon blue"><i class="bi bi-plus-circle"></i></span><span><strong>Create ticket</strong><small>Log a new request</small></span><i class="bi bi-chevron-right"></i></button><button type="button"><span class="quick-icon violet"><i class="bi bi-upc-scan"></i></span><span><strong>Add asset</strong><small>Register equipment</small></span><i class="bi bi-chevron-right"></i></button><button type="button"><span class="quick-icon orange"><i class="bi bi-calendar2-check"></i></span><span><strong>Plan maintenance</strong><small>Schedule intervention</small></span><i class="bi bi-chevron-right"></i></button><button type="button"><span class="quick-icon green"><i class="bi bi-person-add"></i></span><span><strong>Invite member</strong><small>Add a team user</small></span><i class="bi bi-chevron-right"></i></button></div>
        </section>
    </div>
</asp:Content>
