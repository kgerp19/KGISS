 
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientPaymentInformationReport.aspx.cs" Inherits="KGERP.Reports.ClientPaymentInformationReport"%>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" /> 
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server"></asp:ScriptManager>        
        <rsweb:ReportViewer ID="ClientPaymentReportViewer" runat="server" Height="620px" Width="1150px">
        </rsweb:ReportViewer>
    </form>
</body>
</html>
