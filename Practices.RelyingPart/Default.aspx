<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Practices.RelyingPart._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Demonstrates dynamic use of metadata.</h1>
        <p>
            This sample shows how to dynamically use WS-Federation Metadata in a ASP.Net Web
                Application. For a more complete explanation of how the sample works, please refer 
                to the readme.html file in the VS solution or to the sample’s <a href="http://code.msdn.microsoft.com/Federation-Metadata-34036040">online description</a>.
        </p>
    </div>

    <div class="row">
        <h3>Your claims</h3>
        <p>
            <asp:GridView ID="ClaimsGridView" AutoGenerateColumns="false" runat="server" CellPadding="3">
                <AlternatingRowStyle BackColor="White" />
                <HeaderStyle BackColor="#7AC0DA" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="Issuer" HeaderText="Issuer" Visible="true" />
                    <asp:BoundField DataField="OriginalIssuer" HeaderText="Original Issuer" Visible="false" />
                    <asp:BoundField DataField="Type" HeaderText="Type" Visible="true" />
                    <asp:BoundField DataField="Value" HeaderText="Value" Visible="true" />
                    <asp:BoundField DataField="ValueType" HeaderText="Value Type" Visible="false" />
                </Columns>
            </asp:GridView>
        </p>
    </div>

</asp:Content>
