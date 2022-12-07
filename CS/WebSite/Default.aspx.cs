using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using DevExpress.Web;

public partial class _Default : System.Web.UI.Page {
    protected void Page_Init(object sender, EventArgs e) {
        if (Session["dataSource"] == null)
            Session["dataSource"] = GetDataSource();
        ASPxGridView1.DataSource = Session["dataSource"];
        ASPxGridView1.DataBind();

        if (Session["dataSourceSelectedValues"] != null) {
            ASPxGridView2.DataSource = Session["dataSourceSelectedValues"];
            ASPxGridView2.DataBind();
        }
    }
    protected void txtLB_Init(object sender, EventArgs e) {
        ASPxTextBox textBox = (ASPxTextBox)sender;
        GridViewDataItemTemplateContainer templateContainer = (GridViewDataItemTemplateContainer)textBox.NamingContainer;
        textBox.ClientInstanceName = string.Format("textBox_{0}", templateContainer.VisibleIndex);
        textBox.ClientSideEvents.TextChanged = string.Format("function(s, e) {{ OnTextChanged(s, e, {0}); }}", templateContainer.VisibleIndex);

        Dictionary<object, int> lowerBoundStorage = Session["lowerBoundStorage"] as Dictionary<object, int>;
        if (lowerBoundStorage != null) {
            object key = templateContainer.KeyValue;
            if (lowerBoundStorage.ContainsKey(key))
                textBox.Value = lowerBoundStorage[key];
        }
    }
    protected void ASPxGridView1_CustomUnboundColumnData(object sender, DevExpress.Web.ASPxGridViewColumnDataEventArgs e) {
        if (e.Column.FieldName == "LowerBound") {
            Dictionary<object, int> lowerBoundStorage = Session["lowerBoundStorage"] as Dictionary<object, int>;
            if (lowerBoundStorage == null) {
                lowerBoundStorage = new Dictionary<object, int>();
                Session["lowerBoundStorage"] = lowerBoundStorage;
            }
            object key = e.GetListSourceFieldValue(e.ListSourceRowIndex, "CategoryID");
            if (lowerBoundStorage.ContainsKey(key))
                e.Value = lowerBoundStorage[key];
            else
                e.Value = 0;
        }
    }
    private DataTable GetDataSource() {
        DataTable dataTable;

        using (OleDbConnection connection = new OleDbConnection()) {
            connection.ConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MapPath("~/App_Data/nwind.mdb"));

            dataTable = new DataTable();

            OleDbDataAdapter adapter = new OleDbDataAdapter(string.Empty, connection);
            adapter.SelectCommand.CommandText = "SELECT [CategoryID], [CategoryName], [Description] FROM [Categories]";
            adapter.Fill(dataTable);
        }

        return dataTable;
    }
    protected void ASPxGridView2_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e) {
        ASPxGridView g2 = (ASPxGridView)sender;
        DataTable dataSourceSelectedValues = new DataTable();

        dataSourceSelectedValues.Columns.Add(new DataColumn("CategoryID"));
        dataSourceSelectedValues.Columns.Add(new DataColumn("CategoryName"));
        dataSourceSelectedValues.Columns.Add(new DataColumn("Description"));
        dataSourceSelectedValues.Columns.Add(new DataColumn("LowerBound"));

        dataSourceSelectedValues.PrimaryKey = new DataColumn[] { dataSourceSelectedValues.Columns["CategoryID"] };
        int indexSelectedValues = 0;

        Dictionary<object, int> lowerBoundStorage = Session["lowerBoundStorage"] as Dictionary<object, int>;
        if (lowerBoundStorage == null)
            lowerBoundStorage = new Dictionary<object, int>();
        for (int i = 0; i < ASPxGridView1.VisibleRowCount; i++) {
            ASPxTextBox txtLowerBound = (ASPxTextBox)ASPxGridView1.FindRowCellTemplateControl(i, (GridViewDataColumn)ASPxGridView1.Columns["LowerBound"], "txtLB");
            int lowerBound = 0;
            object key = ASPxGridView1.GetRowValues(i, "CategoryID");
            if (txtLowerBound != null) {
                lowerBound = int.Parse(txtLowerBound.Text.Trim());
                if (!lowerBoundStorage.ContainsKey(key))
                    lowerBoundStorage.Add(key, lowerBound);
                else
                    lowerBoundStorage[key] = lowerBound;
            } else {
                if (lowerBoundStorage.ContainsKey(key))
                    lowerBound = lowerBoundStorage[key];
            }
            
            if (ASPxGridView1.Selection.IsRowSelected(i)) {
                dataSourceSelectedValues.ImportRow(ASPxGridView1.GetDataRow(i));
                DataRow dataRow = dataSourceSelectedValues.Rows[indexSelectedValues];
                dataRow["LowerBound"] = lowerBound;
                indexSelectedValues++;
            }
        }
        Session["dataSourceSelectedValues"] = dataSourceSelectedValues;
        Session["lowerBoundStorage"] = lowerBoundStorage;

        g2.DataSource = Session["dataSourceSelectedValues"];
        g2.DataBind();
    }
    protected void ASPxCallback1_Callback(object source, DevExpress.Web.CallbackEventArgs e) {
        string[] parameters = e.Parameter.Split('|');
        int visibleIndex = int.Parse(parameters[0]);
        int lowerBound = int.Parse(parameters[1]);

        Dictionary<object, int> lowerBoundStorage = Session["lowerBoundStorage"] as Dictionary<object, int>;
        if (lowerBoundStorage == null)
            lowerBoundStorage = new Dictionary<object, int>();

        object key = ASPxGridView1.GetRowValues(visibleIndex, "CategoryID");
        if (!lowerBoundStorage.ContainsKey(key))
            lowerBoundStorage.Add(key, lowerBound);
        else
            lowerBoundStorage[key] = lowerBound;
    }
}