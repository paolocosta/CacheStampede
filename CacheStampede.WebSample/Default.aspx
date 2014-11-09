<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CacheStampede.WebSample.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h1>ASP.NET Cache</h1>
    Product last retrieval time: <asp:Label id="lblCreationTime" runat="server" />
    <br><br>
    <asp:LinkButton ID="lnkClearCache" runat="server" Text="Clear cache" 
            onclick="lnkClearCache_Click" />  
    </div>
    </form>
</body>
</html>
