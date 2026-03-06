<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
   CodeFile="AdminDashboard.aspx.cs" Inherits="AdminDashboard" Title="Admin Panel | Jyotish" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    /* ===== GLOBAL DASHBOARD STYLES ===== */
    .dashboard-container {
        max-width: 1400px;
        margin: 40px auto;
        padding: 0 30px;
        animation: fadeIn 0.6s ease;
    }
    
    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(20px); }
        to { opacity: 1; transform: translateY(0); }
    }
    
    /* ===== HEADER SECTION ===== */
    .dashboard-header {
        margin-bottom: 50px;
        position: relative;
    }
    
    
    .dashboard-header h1 {
        font-family: 'Cinzel', serif;
        font-size: 2.2rem;
        font-weight: 700;
        color: #c9a84c;
        letter-spacing: 6px;
        text-transform: uppercase;
        margin-bottom: 10px;
        text-shadow: 0 2px 10px rgba(201, 168, 76, 0.2);
        position: relative;
        display: inline-block;
    }
    
    .dashboard-header h1::after {
        content: '';
        position: absolute;
        bottom: -10px;
        left: 0;
        width: 80px;
        height: 2px;
        background: linear-gradient(90deg, #c9a84c, transparent);
    }
    
    .dashboard-header .subtitle {
        font-family: 'Crimson Text', serif;
        font-size: 1rem;
        color: #9a8f7a;
        font-style: italic;
        margin-top: 15px;
        letter-spacing: 1px;
    }
    
    /* ===== STATS CARDS ===== */
    .stats-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
        gap: 24px;
        margin-bottom: 60px;
    }
    
    .stat-card {
        background: linear-gradient(145deg, #0d1428, #0a1020);
        border: 1px solid rgba(201, 168, 76, 0.15);
        border-radius: 12px;
        padding: 28px 20px;
        text-align: center;
        transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        position: relative;
        overflow: hidden;
        box-shadow: 0 10px 30px -10px rgba(0, 0, 0, 0.5);
    }
    
    .stat-card::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        height: 2px;
        background: linear-gradient(90deg, transparent, #c9a84c, transparent);
        transform: translateX(-100%);
        transition: transform 0.6s ease;
    }
    
    .stat-card:hover {
        transform: translateY(-5px);
        border-color: rgba(201, 168, 76, 0.3);
        box-shadow: 0 20px 40px -12px rgba(201, 168, 76, 0.2);
    }
    
    .stat-card:hover::before {
        transform: translateX(100%);
    }
    
    .stat-icon {
        font-size: 2.2rem;
        color: #c9a84c;
        margin-bottom: 15px;
        opacity: 0.9;
    }
    
    .stat-label {
        font-family: 'Cinzel', serif;
        font-size: 0.7rem;
        letter-spacing: 2.5px;
        text-transform: uppercase;
        color: #8a6d2e;
        margin-bottom: 12px;
        font-weight: 400;
    }
    
    .stat-value {
        font-family: 'Cinzel', serif;
        font-size: 2.5rem;
        font-weight: 700;
        color: #c9a84c;
        line-height: 1.2;
        text-shadow: 0 0 20px rgba(201, 168, 76, 0.3);
    }
    
    .stat-unit {
        font-size: 0.9rem;
        color: #9a8f7a;
        margin-top: 5px;
        font-family: 'Crimson Text', serif;
    }
    
    /* ===== SECTION TITLES ===== */
    .section-title {
        font-family: 'Cinzel', serif;
        font-size: 1.1rem;
        font-weight: 600;
        color: #c9a84c;
        letter-spacing: 4px;
        text-transform: uppercase;
        margin: 50px 0 30px;
        position: relative;
        display: inline-block;
        padding-bottom: 12px;
    }
    
    .section-title::after {
        content: '';
        position: absolute;
        bottom: 0;
        left: 0;
        width: 100%;
        height: 1px;
        background: linear-gradient(90deg, #c9a84c, rgba(201, 168, 76, 0.2), transparent);
    }
    
    .section-title span {
        color: #8a6d2e;
        margin-right: 10px;
        font-size: 1.3rem;
    }
    
    /* ===== FORM CARDS ===== */
    .form-card {
        background: linear-gradient(145deg, #0d1428, #0a1020);
        border: 1px solid rgba(201, 168, 76, 0.15);
        border-radius: 16px;
        padding: 40px;
        margin-bottom: 50px;
        box-shadow: 0 15px 35px -10px rgba(0, 0, 0, 0.6);
    }
    
    .form-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 24px;
        margin-bottom: 24px;
    }
    
    .form-group {
        margin-bottom: 20px;
    }
    
    .form-group.full-width {
        grid-column: span 2;
    }
    
    .form-label {
        display: block;
        font-family: 'Cinzel', serif;
        font-size: 0.65rem;
        letter-spacing: 2px;
        text-transform: uppercase;
        color: #8a6d2e;
        margin-bottom: 10px;
        font-weight: 500;
    }
    
    .form-control {
        width: 100%;
        background: rgba(5, 8, 16, 0.8);
        border: 1px solid rgba(201, 168, 76, 0.2);
        border-radius: 8px;
        padding: 14px 18px;
        color: #e8dfc8;
        font-family: 'Crimson Text', serif;
        font-size: 1rem;
        transition: all 0.2s ease;
        outline: none;
    }
    
    .form-control:focus {
        border-color: #c9a84c;
        background: rgba(8, 12, 24, 0.9);
        box-shadow: 0 0 0 3px rgba(201, 168, 76, 0.1);
    }
    
    .form-control::placeholder {
        color: rgba(154, 143, 122, 0.4);
        font-style: italic;
    }
    
    select.form-control {
        cursor: pointer;
    }
    
    textarea.form-control {
        resize: vertical;
        min-height: 120px;
    }
    
    .btn-primary {
        background: linear-gradient(135deg, #8a6d2e, #c9a84c);
        color: #050810;
        font-family: 'Cinzel', serif;
        font-size: 0.8rem;
        font-weight: 700;
        letter-spacing: 2px;
        padding: 16px 40px;
        border: none;
        border-radius: 8px;
        cursor: pointer;
        transition: all 0.3s ease;
        text-transform: uppercase;
        box-shadow: 0 8px 20px -5px rgba(201, 168, 76, 0.3);
    }
    
    .btn-primary:hover {
        transform: translateY(-2px);
        box-shadow: 0 12px 25px -5px rgba(201, 168, 76, 0.5);
        background: linear-gradient(135deg, #9a7d3e, #dabc5c);
    }
    
    .btn-primary:active {
        transform: translateY(0);
    }
    
    /* ===== SEARCH SECTION ===== */
    .search-section {
        background: rgba(13, 20, 40, 0.5);
        border: 1px solid rgba(201, 168, 76, 0.1);
        border-radius: 12px;
        padding: 25px;
        margin-bottom: 30px;
        backdrop-filter: blur(5px);
    }
    
    .search-grid {
        display: grid;
        grid-template-columns: 1fr 1fr auto auto;
        gap: 15px;
        align-items: end;
    }
    
    .btn-search {
        background: #c9a84c;
        color: #050810;
        font-family: 'Cinzel', serif;
        font-size: 0.75rem;
        font-weight: 600;
        letter-spacing: 1px;
        padding: 14px 30px;
        border: none;
        border-radius: 8px;
        cursor: pointer;
        transition: all 0.2s ease;
        min-width: 120px;
    }
    
    .btn-search:hover {
        background: #dabc5c;
        transform: translateY(-1px);
    }
    
    .btn-reset {
        background: transparent;
        color: #9a8f7a;
        font-family: 'Cinzel', serif;
        font-size: 0.75rem;
        letter-spacing: 1px;
        padding: 14px 30px;
        border: 1px solid rgba(201, 168, 76, 0.3);
        border-radius: 8px;
        cursor: pointer;
        transition: all 0.2s ease;
    }
    
    .btn-reset:hover {
        border-color: #c9a84c;
        color: #c9a84c;
        background: rgba(201, 168, 76, 0.05);
    }
    
    /* ===== TABLE STYLES ===== */
    .table-container {
        background: linear-gradient(145deg, #0d1428, #0a1020);
        border: 1px solid rgba(201, 168, 76, 0.15);
        border-radius: 16px;
        padding: 5px;
        overflow: hidden;
        margin-bottom: 40px;
        box-shadow: 0 15px 35px -10px rgba(0, 0, 0, 0.6);
    }
    
    .modern-table {
        width: 100%;
        border-collapse: collapse;
        font-size: 0.9rem;
    }
    
    .modern-table th {
        background: #0d1428;
        color: #c9a84c;
        font-family: 'Cinzel', serif;
        font-size: 0.7rem;
        letter-spacing: 2px;
        text-transform: uppercase;
        padding: 20px 15px;
        border-bottom: 2px solid rgba(201, 168, 76, 0.3);
        text-align: left;
    }
    
    .modern-table td {
        padding: 16px 15px;
        color: #e8dfc8;
        font-family: 'Crimson Text', serif;
        border-bottom: 1px solid rgba(201, 168, 76, 0.1);
    }
    
    .modern-table tr:hover td {
        background: rgba(201, 168, 76, 0.05);
    }
    
    .badge {
        display: inline-block;
        padding: 4px 10px;
        border-radius: 20px;
        font-size: 0.7rem;
        font-family: 'Cinzel', serif;
        letter-spacing: 1px;
    }
    
    .badge-original {
        background: rgba(201, 168, 76, 0.15);
        color: #c9a84c;
        border: 1px solid rgba(201, 168, 76, 0.3);
    }
    
    .badge-shuffled {
        background: rgba(80, 180, 120, 0.15);
        color: #7ecba0;
        border: 1px solid rgba(80, 180, 120, 0.3);
    }
    
    /* ===== ACTION BUTTONS ===== */
    .action-buttons {
        display: flex;
        gap: 8px;
    }
    
    .btn-edit, .btn-delete, .btn-save, .btn-cancel {
        padding: 6px 12px;
        border: none;
        border-radius: 6px;
        font-family: 'Cinzel', serif;
        font-size: 0.65rem;
        letter-spacing: 1px;
        cursor: pointer;
        transition: all 0.2s ease;
        text-decoration: none;
        display: inline-block;
    }
    
    .btn-edit {
        background: rgba(201, 168, 76, 0.15);
        color: #c9a84c;
        border: 1px solid rgba(201, 168, 76, 0.3);
    }
    
    .btn-edit:hover {
        background: rgba(201, 168, 76, 0.25);
        transform: translateY(-1px);
    }
    
    .btn-delete {
        background: rgba(220, 100, 100, 0.15);
        color: #e07070;
        border: 1px solid rgba(220, 100, 100, 0.3);
    }
    
    .btn-delete:hover {
        background: rgba(220, 100, 100, 0.25);
        transform: translateY(-1px);
    }
    
    .btn-save {
        background: rgba(80, 180, 120, 0.15);
        color: #7ecba0;
        border: 1px solid rgba(80, 180, 120, 0.3);
    }
    
    .btn-save:hover {
        background: rgba(80, 180, 120, 0.25);
    }
    
    .btn-cancel {
        background: transparent;
        color: #9a8f7a;
        border: 1px solid rgba(154, 143, 122, 0.3);
    }
    
    .btn-cancel:hover {
        border-color: #e07070;
        color: #e07070;
    }
    
    /* ===== EDIT MODE STYLES ===== */
    .edit-control {
        background: #050810;
        border: 1px solid #c9a84c;
        color: #e8dfc8;
        padding: 8px 10px;
        border-radius: 6px;
        font-family: 'Crimson Text', serif;
        width: 100%;
    }
    
    /* ===== MESSAGE STYLES ===== */
    .alert {
        padding: 16px 22px;
        border-radius: 10px;
        margin-bottom: 25px;
        font-family: 'Crimson Text', serif;
        font-size: 1rem;
        border-left: 4px solid;
        animation: slideIn 0.3s ease;
    }
    
    @keyframes slideIn {
        from { opacity: 0; transform: translateX(-10px); }
        to { opacity: 1; transform: translateX(0); }
    }
    
    .alert-success {
        background: rgba(80, 180, 120, 0.1);
        border-color: #7ecba0;
        color: #7ecba0;
    }
    
    .alert-error {
        background: rgba(220, 100, 100, 0.1);
        border-color: #e07070;
        color: #e07070;
    }
    
    /* ===== RESPONSIVE ===== */
    @media (max-width: 768px) {
        .dashboard-container {
            padding: 0 15px;
        }
        
        .stats-grid {
            grid-template-columns: 1fr;
        }
        
        .form-grid {
            grid-template-columns: 1fr;
        }
        
        .form-group.full-width {
            grid-column: span 1;
        }
        
        .search-grid {
            grid-template-columns: 1fr;
        }
        
        .modern-table {
            font-size: 0.8rem;
        }
        
        .modern-table th, .modern-table td {
            padding: 12px 8px;
        }
        
        .action-buttons {
            flex-direction: column;
        }
    }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="dashboard-container">

    <!-- Header -->
    <div class="dashboard-header">
        <h1>✦ ADMIN PANEL ✦</h1>
        <div class="subtitle">Celestial Management Console</div>
    </div>

    <!-- Stats Cards -->
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-icon">👥</div>
            <div class="stat-label">TOTAL USERS</div>
            <div class="stat-value"><asp:Label ID="lblTotalUsers" runat="server" /></div>
            <div class="stat-unit">registered members</div>
        </div>
        
        <div class="stat-card">
            <div class="stat-icon">📊</div>
            <div class="stat-label">TODAY'S LOGINS</div>
            <div class="stat-value"><asp:Label ID="lblTodayLogins" runat="server" /></div>
            <div class="stat-unit">active sessions</div>
        </div>
        
        <div class="stat-card">
            <div class="stat-icon">✨</div>
            <div class="stat-label">NEW REGISTRATIONS</div>
            <div class="stat-value"><asp:Label ID="lblTodayReg" runat="server" /></div>
            <div class="stat-unit">today only</div>
        </div>
        
        <div class="stat-card">
            <div class="stat-icon">🔮</div>
            <div class="stat-label">HOROSCOPES READ</div>
            <div class="stat-value"><asp:Label ID="lblHoroscopeCount" runat="server" /></div>
            <div class="stat-unit">today's views</div>
        </div>
    </div>

    <!-- Message Panel -->
    <asp:Panel ID="pnlMsg" runat="server" Visible="false">
        <div id="divMsg" runat="server" class="alert"></div>
    </asp:Panel>

    <!-- Add/Edit Horoscope Section -->
    <div class="section-title">
        <span>✧</span> DAILY HOROSCOPE MANAGEMENT
    </div>
    
    <div class="form-card">
        <div class="form-grid">
            <div class="form-group">
                <label class="form-label">🌙 RASHI (ZODIAC SIGN)</label>
                <asp:DropDownList ID="ddlRashi" runat="server" CssClass="form-control" />
            </div>
            
            <div class="form-group">
                <label class="form-label">📅 DATE</label>
                <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            
            <div class="form-group full-width">
                <label class="form-label">📜 PREDICTION TEXT</label>
                <asp:TextBox ID="txtPrediction" runat="server" TextMode="MultiLine" Rows="4" 
                    CssClass="form-control" placeholder="Enter today's prediction..." />
            </div>
            
            <div class="form-group">
                <label class="form-label">🔢 LUCKY NUMBER</label>
                <asp:TextBox ID="txtLuckyNumber" runat="server" MaxLength="10" 
                    CssClass="form-control" placeholder="e.g., 7" />
            </div>
            
            <div class="form-group">
                <label class="form-label">🎨 LUCKY COLOR</label>
                <asp:TextBox ID="txtLuckyColor" runat="server" MaxLength="20" 
                    CssClass="form-control" placeholder="e.g., Golden" />
            </div>
        </div>
        
        <div style="text-align: right; margin-top: 20px;">
            <asp:Button ID="btnSave" runat="server" Text="✧ SAVE HOROSCOPE ✧"
                OnClick="btnSave_Click" CssClass="btn-primary" />
        </div>
    </div>

    <!-- Horoscope Management Section -->
    <div class="section-title">
        <span>✧</span> HOROSCOPE LIBRARY
    </div>
    
    <!-- Search Section -->
    <div class="search-section">
        <div class="search-grid">
            <div>
                <label class="form-label">SEARCH BY DATE</label>
                <asp:TextBox ID="txtSearchDate" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            
            <div>
                <label class="form-label">FILTER BY RASHI</label>
                <asp:DropDownList ID="ddlSearchRashi" runat="server" CssClass="form-control" />
            </div>
            
            <asp:Button ID="btnSearch" runat="server" Text="🔍 SEARCH" OnClick="btnSearch_Click" CssClass="btn-search" />
            <asp:Button ID="btnReset" runat="server" Text="⟲ RESET" OnClick="btnReset_Click" CssClass="btn-reset" />
        </div>
    </div>
    
    <!-- Horoscopes Grid -->
    <div class="table-container">
        <asp:GridView ID="gvHoroscopes" runat="server" AutoGenerateColumns="false" 
            DataKeyNames="HoroscopeId"
            OnRowEditing="gvHoroscopes_RowEditing"
            OnRowUpdating="gvHoroscopes_RowUpdating"
            OnRowCancelingEdit="gvHoroscopes_RowCancelingEdit"
            OnRowDeleting="gvHoroscopes_RowDeleting"
            CssClass="modern-table"
            GridLines="None">
            
            <Columns>
                <asp:BoundField DataField="HoroscopeId" HeaderText="ID" ReadOnly="True" ItemStyle-Width="60px" />
                
                <asp:TemplateField HeaderText="RASHI">
                    <ItemTemplate>
                        <%# Eval("RashiEnglishName") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlEditRashi" runat="server" 
                            SelectedValue='<%# Bind("RashiId") %>'
                            OnDataBinding="ddlEditRashi_DataBinding"
                            CssClass="edit-control" />
                    </EditItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="DATE">
                    <ItemTemplate>
                        <%# Eval("PredictionDate", "{0:dd MMM yyyy}") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditDate" runat="server" TextMode="Date" 
                            Text='<%# Bind("PredictionDate", "{0:yyyy-MM-dd}") %>'
                            CssClass="edit-control" />
                    </EditItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="PREDICTION">
                    <ItemTemplate>
                        <%# Eval("PredictionText").ToString().Length > 40 ? 
                            Eval("PredictionText").ToString().Substring(0,40) + "..." : 
                            Eval("PredictionText") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditPrediction" runat="server" TextMode="MultiLine" Rows="2"
                            Text='<%# Bind("PredictionText") %>' Width="200px" CssClass="edit-control" />
                    </EditItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="NO." ItemStyle-Width="50px">
                    <ItemTemplate>
                        <%# Eval("LuckyNumber") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditNumber" runat="server" Text='<%# Bind("LuckyNumber") %>' 
                            Width="40px" CssClass="edit-control" />
                    </EditItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="COLOR">
                    <ItemTemplate>
                        <span style="display:inline-block; width:12px; height:12px; border-radius:50%; 
                            background-color:<%# Eval("LuckyColor").ToString().ToLower() %>; 
                            margin-right:6px; border:1px solid rgba(255,255,255,0.2);">
                        </span>
                        <%# Eval("LuckyColor") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditColor" runat="server" Text='<%# Bind("LuckyColor") %>' 
                            Width="80px" CssClass="edit-control" />
                    </EditItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="TYPE" ItemStyle-Width="100px">
                    <ItemTemplate>
                        <span class='badge <%# (Eval("IsShuffled") != DBNull.Value && Convert.ToBoolean(Eval("IsShuffled"))) ? "badge-shuffled" : "badge-original" %>'>
                            <%# (Eval("IsShuffled") != DBNull.Value && Convert.ToBoolean(Eval("IsShuffled"))) ? "Shuffled" : " Original" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="ACTIONS" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <div class="action-buttons">
                            <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" 
                                CssClass="btn-edit" Text=" Edit" />
                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" 
                                CssClass="btn-delete" Text="Delete" 
                                OnClientClick="return confirm('Are you sure you want to delete this horoscope?');" />
                        </div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <div class="action-buttons">
                            <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" 
                                CssClass="btn-save" Text="Save" />
                            <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" 
                                CssClass="btn-cancel" Text="Cancel" />
                        </div>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Users Section -->
    <div class="section-title">
        <span>✧</span> REGISTERED USERS
    </div>
    
    <div class="table-container">
        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false"
            CssClass="modern-table"
            GridLines="None">
            <Columns>
                <asp:BoundField DataField="UserId" HeaderText="ID" ItemStyle-Width="60px" />
                <asp:BoundField DataField="FullName" HeaderText="FULL NAME" />
                <asp:BoundField DataField="Email" HeaderText="EMAIL" />
                <asp:BoundField DataField="CreatedDate" HeaderText="JOINED" DataFormatString="{0:dd MMM yyyy}" />
                <asp:BoundField DataField="LastLoginDate" HeaderText="LAST LOGIN" DataFormatString="{0:dd MMM yyyy}" NullDisplayText="Never" />
                <asp:TemplateField HeaderText="STATUS" ItemStyle-Width="80px">
                    <ItemTemplate>
                        <span class='badge <%# Convert.ToBoolean(Eval("IsActive")) ? "badge-original" : "badge-shuffled" %>'>
                            <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "⭕ Inactive" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</div>
</asp:Content>