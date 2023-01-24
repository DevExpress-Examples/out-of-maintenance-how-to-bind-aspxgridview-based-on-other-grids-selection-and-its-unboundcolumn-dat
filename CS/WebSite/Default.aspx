<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.14.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.14.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dxwgv" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.14.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dxe" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" language="javascript">
        function OnTextChanged(s, e, visibleIndex) {
            callback.PerformCallback(visibleIndex + '|' + s.GetValue().toString())
        }

        function OnCallbackComplete(s, e) {
            grid2.PerformCallback('');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <dx:ASPxCallback ID="ASPxCallback1" runat="server" 
            ClientInstanceName="callback" OnCallback="ASPxCallback1_Callback">
            <ClientSideEvents CallbackComplete="OnCallbackComplete" />
        </dx:ASPxCallback>
        <dxwgv:ASPxGridView ID="ASPxGridView1" runat="server" KeyFieldName="CategoryID" ClientInstanceName="grid1"
            OnCustomUnboundColumnData="ASPxGridView1_CustomUnboundColumnData">
            <Columns>
                <dxwgv:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0">
                </dxwgv:GridViewCommandColumn>
                <dxwgv:GridViewDataTextColumn FieldName="CategoryID" VisibleIndex="1">
                </dxwgv:GridViewDataTextColumn>
                <dxwgv:GridViewDataTextColumn FieldName="CategoryName" VisibleIndex="2">
                </dxwgv:GridViewDataTextColumn>
                <dxwgv:GridViewDataTextColumn FieldName="Description" VisibleIndex="3">
                </dxwgv:GridViewDataTextColumn>
                <dxwgv:GridViewDataTextColumn FieldName="LowerBound" VisibleIndex="4" UnboundType="Integer">
                    <DataItemTemplate>
                        <dxe:ASPxTextBox ID="txtLB" runat="server" Text="0" Width="60px" OnInit="txtLB_Init">
                        </dxe:ASPxTextBox>
                    </DataItemTemplate>
                </dxwgv:GridViewDataTextColumn>
            </Columns>
            <ClientSideEvents SelectionChanged="function(s, e) {
                if (!callback.InCallback())
                    grid2.PerformCallback('');
            }" />
            <SettingsBehavior AllowMultiSelection="true" />
            <SettingsPager PageSize="5"></SettingsPager>
        </dxwgv:ASPxGridView>
        <br />
        <dxwgv:ASPxGridView ID="ASPxGridView2" runat="server" KeyFieldName="CategoryID" ClientInstanceName="grid2"
            OnCustomCallback="ASPxGridView2_CustomCallback">
            <Columns>
                <dxwgv:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0">
                </dxwgv:GridViewCommandColumn>
                <dxwgv:GridViewDataTextColumn FieldName="CategoryID" VisibleIndex="1">
                </dxwgv:GridViewDataTextColumn>
                <dxwgv:GridViewDataTextColumn FieldName="CategoryName" VisibleIndex="2">
                </dxwgv:GridViewDataTextColumn>
                <dxwgv:GridViewDataTextColumn FieldName="Description" VisibleIndex="3">
                </dxwgv:GridViewDataTextColumn>
                <dxwgv:GridViewDataTextColumn FieldName="LowerBound" VisibleIndex="4">
                </dxwgv:GridViewDataTextColumn>
            </Columns>
        </dxwgv:ASPxGridView>
    </div>
    </form>
</body>
</html>