Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.Collections.Generic
Imports DevExpress.Web.ASPxGridView
Imports DevExpress.Web.ASPxEditors

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
		If Session("dataSource") Is Nothing Then
			Session("dataSource") = GetDataSource()
		End If
		ASPxGridView1.DataSource = Session("dataSource")
		ASPxGridView1.DataBind()

		If Session("dataSourceSelectedValues") IsNot Nothing Then
			ASPxGridView2.DataSource = Session("dataSourceSelectedValues")
			ASPxGridView2.DataBind()
		End If
	End Sub
	Protected Sub txtLB_Init(ByVal sender As Object, ByVal e As EventArgs)
		Dim textBox As ASPxTextBox = CType(sender, ASPxTextBox)
		Dim templateContainer As GridViewDataItemTemplateContainer = CType(textBox.NamingContainer, GridViewDataItemTemplateContainer)
		textBox.ClientInstanceName = String.Format("textBox_{0}", templateContainer.VisibleIndex)
		textBox.ClientSideEvents.TextChanged = String.Format("function(s, e) {{ OnTextChanged(s, e, {0}); }}", templateContainer.VisibleIndex)

		Dim lowerBoundStorage As Dictionary(Of Object, Integer) = TryCast(Session("lowerBoundStorage"), Dictionary(Of Object, Integer))
		If lowerBoundStorage IsNot Nothing Then
			Dim key As Object = templateContainer.KeyValue
			If lowerBoundStorage.ContainsKey(key) Then
				textBox.Value = lowerBoundStorage(key)
			End If
		End If
	End Sub
	Protected Sub ASPxGridView1_CustomUnboundColumnData(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridView.ASPxGridViewColumnDataEventArgs)
		If e.Column.FieldName = "LowerBound" Then
			Dim lowerBoundStorage As Dictionary(Of Object, Integer) = TryCast(Session("lowerBoundStorage"), Dictionary(Of Object, Integer))
			If lowerBoundStorage Is Nothing Then
				lowerBoundStorage = New Dictionary(Of Object, Integer)()
				Session("lowerBoundStorage") = lowerBoundStorage
			End If
			Dim key As Object = e.GetListSourceFieldValue(e.ListSourceRowIndex, "CategoryID")
			If lowerBoundStorage.ContainsKey(key) Then
				e.Value = lowerBoundStorage(key)
			Else
				e.Value = 0
			End If
		End If
	End Sub
	Private Function GetDataSource() As DataTable
		Dim dataTable As DataTable

		Using connection As New OleDbConnection()
			connection.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MapPath("~/App_Data/nwind.mdb"))

			dataTable = New DataTable()

			Dim adapter As New OleDbDataAdapter(String.Empty, connection)
			adapter.SelectCommand.CommandText = "SELECT [CategoryID], [CategoryName], [Description] FROM [Categories]"
			adapter.Fill(dataTable)
		End Using

		Return dataTable
	End Function
	Protected Sub ASPxGridView2_CustomCallback(ByVal sender As Object, ByVal e As ASPxGridViewCustomCallbackEventArgs)
		Dim g2 As ASPxGridView = CType(sender, ASPxGridView)
		Dim dataSourceSelectedValues As New DataTable()

		dataSourceSelectedValues.Columns.Add(New DataColumn("CategoryID"))
		dataSourceSelectedValues.Columns.Add(New DataColumn("CategoryName"))
		dataSourceSelectedValues.Columns.Add(New DataColumn("Description"))
		dataSourceSelectedValues.Columns.Add(New DataColumn("LowerBound"))

		dataSourceSelectedValues.PrimaryKey = New DataColumn() { dataSourceSelectedValues.Columns("CategoryID") }
		Dim indexSelectedValues As Integer = 0

		Dim lowerBoundStorage As Dictionary(Of Object, Integer) = TryCast(Session("lowerBoundStorage"), Dictionary(Of Object, Integer))
		If lowerBoundStorage Is Nothing Then
			lowerBoundStorage = New Dictionary(Of Object, Integer)()
		End If
		For i As Integer = 0 To ASPxGridView1.VisibleRowCount - 1
			Dim txtLowerBound As ASPxTextBox = CType(ASPxGridView1.FindRowCellTemplateControl(i, CType(ASPxGridView1.Columns("LowerBound"), GridViewDataColumn), "txtLB"), ASPxTextBox)
			Dim lowerBound As Integer = 0
			Dim key As Object = ASPxGridView1.GetRowValues(i, "CategoryID")
			If txtLowerBound IsNot Nothing Then
				lowerBound = Integer.Parse(txtLowerBound.Text.Trim())
				If (Not lowerBoundStorage.ContainsKey(key)) Then
					lowerBoundStorage.Add(key, lowerBound)
				Else
					lowerBoundStorage(key) = lowerBound
				End If
			Else
				If lowerBoundStorage.ContainsKey(key) Then
					lowerBound = lowerBoundStorage(key)
				End If
			End If

			If ASPxGridView1.Selection.IsRowSelected(i) Then
				dataSourceSelectedValues.ImportRow(ASPxGridView1.GetDataRow(i))
				Dim dataRow As DataRow = dataSourceSelectedValues.Rows(indexSelectedValues)
				dataRow("LowerBound") = lowerBound
				indexSelectedValues += 1
			End If
		Next i
		Session("dataSourceSelectedValues") = dataSourceSelectedValues
		Session("lowerBoundStorage") = lowerBoundStorage

		g2.DataSource = Session("dataSourceSelectedValues")
		g2.DataBind()
	End Sub
	Protected Sub ASPxCallback1_Callback(ByVal source As Object, ByVal e As DevExpress.Web.ASPxCallback.CallbackEventArgs)
		Dim parameters() As String = e.Parameter.Split("|"c)
		Dim visibleIndex As Integer = Integer.Parse(parameters(0))
		Dim lowerBound As Integer = Integer.Parse(parameters(1))

		Dim lowerBoundStorage As Dictionary(Of Object, Integer) = TryCast(Session("lowerBoundStorage"), Dictionary(Of Object, Integer))
		If lowerBoundStorage Is Nothing Then
			lowerBoundStorage = New Dictionary(Of Object, Integer)()
		End If

		Dim key As Object = ASPxGridView1.GetRowValues(visibleIndex, "CategoryID")
		If (Not lowerBoundStorage.ContainsKey(key)) Then
			lowerBoundStorage.Add(key, lowerBound)
		Else
			lowerBoundStorage(key) = lowerBound
		End If
	End Sub
End Class