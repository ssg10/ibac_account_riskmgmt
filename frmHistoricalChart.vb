Imports System.IO

Public Class frmHistoricalChart
    Dim dtAccEquity As New DataTable("dtAccEquity")

    Private m_chart_timeframe As String

    Public Property chart_timeframe() As String
        Get
            chart_timeframe = m_chart_timeframe
        End Get
        Set(ByVal Value As String)
            m_chart_timeframe = Value
        End Set
    End Property

    Private Sub frmHistoricalChart_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim filename As String

        'Init datagridview control
        dtAccEquity.Columns.Add("DateTime")
        dtAccEquity.Columns.Add("NetLiquidity")

        chtEquity.Series.Clear()

        'Create chart
        chtEquity.Series.Add("AccEquity")
        chtEquity.Name = "AccEquity"

        With chtEquity.ChartAreas(0)
            .AxisX.Title = "Date"
            .AxisY.Title = "Units"
            .AxisY.Minimum = 8000
            .AxisY.Maximum = 15000
        End With

        ' Load file
        If m_chart_timeframe = "intraday" Then
            filename = "c:\" & frmMain.g_accounts(0) & "acc_equity_5min.csv"
        ElseIf m_chart_timeframe = "daily" Then
            filename = "c:\" & frmMain.g_accounts(0) & "acc_equity_daily.csv"
        Else
            GoTo ErrorHandling
        End If

        Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(filename)
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.SetDelimiters(",")
            Dim currentRow As String()
            Dim currentDate As DateTime
            Dim equity As Double
            While Not MyReader.EndOfData
                Try
                    currentRow = MyReader.ReadFields()
                    'Dim currentField As String

                    'This will dump, 
                    ' Date Time
                    ' Equity$
                    'For Each currentField In currentRow
                    'Console.WriteLine(currentField)
                    currentDate = currentRow(0)
                    equity = currentRow(1)
                    'Next, if date is in between selected dates, put it in data table for plotting into chart
                    If (DateTime.Compare(CDate(currentDate).Date, CDate(frmMain.txtStartDate.Text).Date) >= 0) And
                            (DateTime.Compare(CDate(currentDate).Date, CDate(frmMain.txtEndDate.Text).Date) <= 0) Then

                        dtAccEquity.Rows.Add({currentDate, equity})  ' Parms take array

                    End If

                Catch ex As Microsoft.VisualBasic.
                            FileIO.MalformedLineException
                    MsgBox("Line " & ex.Message &
                    "is not valid and will be skipped.")
                End Try
            End While
        End Using

        'Set up Chart
        chtEquity.Series(0).Points.Clear()
        chtEquity.DataSource = ""
        chtEquity.ResetAutoValues()
        chtEquity.DataSource = dtAccEquity
        chtEquity.DataBind()

        With chtEquity.Series(0)
            .Points.Clear()

            .Points.DataBind(dtAccEquity.DefaultView, "DateTime", "NetLiquidity", Nothing)
            '.XValueMember = "<DateTime>"
            '.YValueMembers = "<NetLiquidity>"
            .ChartType = DataVisualization.Charting.SeriesChartType.Line
            .BorderWidth = 3
            .BorderColor = Color.Black
        End With


ErrorHandling:
    End Sub

End Class