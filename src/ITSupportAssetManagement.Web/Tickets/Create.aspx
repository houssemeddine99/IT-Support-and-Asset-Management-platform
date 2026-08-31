<%@ Page Title="New ticket" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Create.aspx.vb" Inherits="ITSupportAssetManagement.Web.TicketCreatePage" %>
<asp:Content ID="CreateTicketContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading ticket-page-heading">
        <div><a class="back-link" href="List.aspx"><i class="bi bi-arrow-left"></i> Back to tickets</a><h1>Create a support ticket</h1><p>Give the support team enough information to resolve the request quickly.</p></div>
    </div>
    <asp:Panel ID="ErrorPanel" runat="server" CssClass="page-alert error" Visible="false"><i class="bi bi-exclamation-circle"></i><asp:Literal ID="ErrorMessage" runat="server" /></asp:Panel>
    <div class="ticket-form-grid">
        <section class="panel form-panel">
            <div class="form-section"><div class="section-number">1</div><div class="section-copy"><h2>Request details</h2><p>Describe what you need help with.</p></div></div>
            <div class="form-body">
                <div class="field"><label for="TitleInput">Title <em>Required</em></label><asp:TextBox ID="TitleInput" runat="server" MaxLength="180" placeholder="Example: Unable to connect to the company VPN" /><asp:RequiredFieldValidator ID="TitleRequired" runat="server" ControlToValidate="TitleInput" ErrorMessage="Enter a short title." CssClass="field-error" Display="Dynamic" /></div>
                <div class="field"><label for="DescriptionInput">Description <em>Required</em></label><asp:TextBox ID="DescriptionInput" runat="server" TextMode="MultiLine" Rows="8" MaxLength="5000" placeholder="What happened? When did it begin? What have you already tried?" /><div class="field-help"><span>Do not include passwords or confidential information.</span><span id="DescriptionCount">0 / 5000</span></div><asp:RequiredFieldValidator ID="DescriptionRequired" runat="server" ControlToValidate="DescriptionInput" ErrorMessage="Describe the issue." CssClass="field-error" Display="Dynamic" /></div>
            </div>
        </section>
        <aside class="form-sidebar">
            <section class="panel form-panel compact">
                <div class="form-section"><div class="section-number">2</div><div class="section-copy"><h2>Classification</h2><p>Help us route the request.</p></div></div>
                <div class="form-body">
                    <div class="field"><label for="CategoryInput">Category <em>Required</em></label><asp:DropDownList ID="CategoryInput" runat="server" /><asp:RequiredFieldValidator ID="CategoryRequired" runat="server" ControlToValidate="CategoryInput" InitialValue="" ErrorMessage="Select a category." CssClass="field-error" Display="Dynamic" /></div>
                    <div class="field"><label for="PriorityInput">Priority</label><asp:DropDownList ID="PriorityInput" runat="server"><asp:ListItem Value="Low">Low - Minor inconvenience</asp:ListItem><asp:ListItem Value="Medium" Selected="True">Medium - Work is affected</asp:ListItem><asp:ListItem Value="High">High - Work is blocked</asp:ListItem><asp:ListItem Value="Critical">Critical - Major outage</asp:ListItem></asp:DropDownList></div>
                    <div class="field"><label for="AssetInput">Related asset <span>Optional</span></label><asp:DropDownList ID="AssetInput" runat="server" /></div>
                </div>
            </section>
            <div class="form-actions"><a class="button button-secondary" href="List.aspx">Cancel</a><asp:Button ID="CreateButton" runat="server" Text="Create ticket" CssClass="button button-primary" /></div>
            <div class="response-note"><i class="bi bi-clock-history"></i><div><strong>Expected response</strong><span>Critical tickets are reviewed immediately. Other requests are reviewed within four business hours.</span></div></div>
        </aside>
    </div>
    <script>
        (function(){var box=document.getElementById('<%= DescriptionInput.ClientID %>'),count=document.getElementById('DescriptionCount');if(!box||!count)return;function update(){count.textContent=box.value.length+' / 5000';}box.addEventListener('input',update);update();}());
    </script>
</asp:Content>
