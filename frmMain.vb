Imports System.IO
Imports System.Timers

Public Class frmMain
    Public g_accounts As String() = {"U129661", "U1465027"}
    Dim g_connecting_inprogress As Boolean = False
    Dim g_onetime_call_account As Boolean = False
    Dim g_is_acc_summary_subscribed As Boolean = False
    Dim g_sec_when_data_collected As Integer = 59
    Dim g_min_when_data_collected As Integer = 59
    Dim g_is_accsummary_api_called As Boolean = False
    Dim g_dump_daily_data_once As Boolean = False
    Dim g_write_once_per_run As Boolean = True
    'Dim MARKET_CLOSE_TIME As DateTime = #1:00:00 PM#
    'Dim MARKET_OPEN_TIME As DateTime = #6:30:00 AM#

    'For test
    Dim MARKET_CLOSE_TIME As DateTime = #2:00:00 AM#
    Dim MARKET_OPEN_TIME As DateTime = #12:30:00 AM#

    Dim IBCONNECTION_NUMBER As Integer = 69 ' Must be unique number or IB will not connect us
    Dim IB_ACC_SUMMARY_REQID As Integer = 1

    Dim dtAccEquity As New DataTable("dtAccEquity")

    Private Sub AxTws1_errMsg(sender As Object, e As AxTWSLib._DTwsEvents_errMsgEvent) Handles AxTws1.errMsg
        lbErrorAndLog.Items.Add(Now.ToString & ":  " & e.errorMsg)
        lbErrorAndLog.TopIndex = lbErrorAndLog.Items.Count - 1


        'Check if marketdata farm is recoqnized, then connection is good
        Dim connection_msg As String

        connection_msg = "Market data farm"

        If g_connecting_inprogress Then

            If e.errorMsg.Contains(connection_msg) Then
                lblConnected.Text = "CONNECTED"
                lblConnected.BackColor = Color.Green
                Call AxTws1.reqCurrentTime()
            End If

            ' Just a flag to let this if then executed once
            g_connecting_inprogress = False
        End If
    End Sub

    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        g_connecting_inprogress = True
        Call AxTws1.connect("", "7496", IBCONNECTION_NUMBER)
        'Wait for errMsg event with "Market data farm ", if string contains that, connection is good

    End Sub

    Private Sub btnDisconnect_Click(sender As Object, e As EventArgs) Handles btnDisconnect.Click
        Call AxTws1.disconnect()
        lblConnected.Text = "DISCONNECTED"
        lblConnected.BackColor = Color.Red
    End Sub

    Private Sub btnStartAccSummary_Click(sender As Object, e As EventArgs) Handles btnStartAccSummary.Click

        If String.Compare(lblConnected.Text, "CONNECTED") = 0 Then  '0 means equal
            If Not g_is_acc_summary_subscribed Then
                tmrCollectAccSummary.Start()
                'Call AxTws1.reqAccountSummary(IB_ACC_SUMMARY_REQID, "All", "AccruedCash,BuyingPower,NetLiquidation")
                g_is_acc_summary_subscribed = True
                btnStartAccSummary.Text = "Stop Acc Summary"
            Else
                tmrCollectAccSummary.Stop()
                AxTws1.cancelAccountSummary(IB_ACC_SUMMARY_REQID)
                g_is_acc_summary_subscribed = False
                btnStartAccSummary.Text = "Start Acc Summary"
            End If

        Else
            MsgBox("Make sure you are connected to TWS")
        End If

    End Sub

    Private Sub tmrCollectAccSummary_Tick(sender As Object, e As EventArgs) Handles tmrCollectAccSummary.Tick
        'Every 5 secs this fires
        'If market hours, then divide minute % 5. if mod is = 0, it is 5th minute (to collect 5 minutes data)
        ' Mark date/time, acc value
        ' Write to file

        '----- Start collecting data during market hours.
        If (TimeOfDay >= MARKET_OPEN_TIME) And (TimeOfDay <= MARKET_CLOSE_TIME) Then

            ' If we are at the next min, reset the flags. Purpose of flags is to write the data once in i-th minute, since timer runs more often
            ' =0 because when current min is 0 = 59+1 will be wrong. (when min rolls back)
            'If (TimeOfDay.Minute = (g_min_when_data_collected + 1)) Or (TimeOfDay.Minute = 0) Then
            If (TimeOfDay.Minute <> g_min_when_data_collected) Then
                Console.WriteLine(Now.ToString & " Timeofday.minute <> " & g_min_when_data_collected & " " & g_sec_when_data_collected)
                g_sec_when_data_collected = 59
                g_min_when_data_collected = 59
                g_write_once_per_run = True
            End If

            ' See if it is (mod) ith minute to grab i min account data
            If ((TimeOfDay.Minute Mod 1) = 0) And (TimeOfDay.Second < g_sec_when_data_collected) Then
                Console.WriteLine(TimeOfDay & " " & TimeOfDay.Second & " " & g_sec_when_data_collected & " write file")
                'Save second data to be compared - used to avoid getting too many reading in 5 min
                g_sec_when_data_collected = TimeOfDay.Second
                g_min_when_data_collected = TimeOfDay.Minute


                Call AxTws1.reqAccountSummary(IB_ACC_SUMMARY_REQID, "All", "NetLiquidation")
                g_is_accsummary_api_called = True

            Else
                Console.WriteLine(TimeOfDay & " " & g_min_when_data_collected & " " & g_sec_when_data_collected & " Else if not mod 1")
                ' add flag so that it is called only if api has been called once
                If g_is_accsummary_api_called Then
                    Console.WriteLine(TimeOfDay & "Call cancelAccountSummary")
                    Call AxTws1.cancelAccountSummary(IB_ACC_SUMMARY_REQID)
                    g_is_accsummary_api_called = False
                End If

            End If

        Else
            ' make sure all flags reset
            Console.WriteLine("Reset flags")
            g_sec_when_data_collected = 59
            g_min_when_data_collected = 59
        End If

        '----- If now is after market hours by a bit, then we collect the end of day account data
        If (TimeOfDay > MARKET_CLOSE_TIME) And (TimeOfDay < MARKET_CLOSE_TIME.AddMinutes(3)) And Not g_dump_daily_data_once Then

            Console.WriteLine(Now & " Write EOD data")

            Call AxTws1.reqAccountSummary(IB_ACC_SUMMARY_REQID, "All", "NetLiquidation")  'assume data req comes back faster than timer fires (1 sec or so)

        ElseIf (TimeOfDay > MARKET_CLOSE_TIME.AddMinutes(15)) And (TimeOfDay < MARKET_CLOSE_TIME.AddMinutes(30)) Then

            ' Reset the flag so that next day's closing price can be recorded again (anytime between 1:00pm to 1:15pm, hit once using flag)
            g_dump_daily_data_once = False

        End If

    End Sub

    Private Sub AxTws1_accountSummary(sender As Object, e As AxTWSLib._DTwsEvents_accountSummaryEvent) Handles AxTws1.accountSummary

        Dim filename As String
        Try
            If (TimeOfDay >= MARKET_OPEN_TIME) And (TimeOfDay <= MARKET_CLOSE_TIME) Then

                Console.WriteLine(Now & " " & e.account)

                If g_write_once_per_run Then

                    If e.account = g_accounts(0) Then

                        filename = "c:\" & g_accounts(0) & "acc_equity_5min.csv"
                        If e.tag = "NetLiquidation" Then
                            Using accFile As StreamWriter = New StreamWriter(filename, True)
                                accFile.WriteLine(Now.ToString & "," & e.value)
                            End Using
                            lbData.Items.Add(Now.ToString & "," & g_accounts(0) & "," & e.value)
                            lbData.TopIndex = lbData.Items.Count - 1

                            dtAccEquity.Rows.Add({Now.ToString, e.value})  ' Parms take array
                            DataGridView1.DataSource = dtAccEquity.DefaultView

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
                            End With

                            chtEquity.ResetAutoValues()

                        End If
                    End If

                    If e.account = g_accounts(1) Then
                        filename = "c:\" & g_accounts(1) & "acc_equity_5min.csv"
                        If e.tag = "NetLiquidation" Then
                            Using accFile As StreamWriter = New StreamWriter(filename, True)
                                accFile.WriteLine(Now.ToString & "," & e.value)
                            End Using
                            lbData.Items.Add(TimeOfDay.ToString & "," & g_accounts(1) & "," & e.value)
                            lbData.TopIndex = lbData.Items.Count - 1
                            g_write_once_per_run = False
                        End If
                    End If
                End If


            End If '==== If TimeOfDay > MARKET_OPEN_TIME Then

            '----- If now is after market hours by a bit, then we collect the end of day account data
            If (TimeOfDay > MARKET_CLOSE_TIME) And (TimeOfDay < MARKET_CLOSE_TIME.AddMinutes(3)) Then

                If e.account = g_accounts(0) Then
                    ' Write file for first account
                    filename = "c:\" & g_accounts(0) & "acc_equity_daily.csv"
                    Using accFile As StreamWriter = New StreamWriter(filename, True)
                        accFile.WriteLine(Now.ToString & "," & e.value)
                    End Using
                    lbData.Items.Add(g_accounts(0) & "," & e.value)
                    lbData.TopIndex = lbData.Items.Count - 1
                End If

                If e.account = g_accounts(1) Then
                    ' Write file for second account
                    filename = "c:\" & g_accounts(1) & "acc_equity_daily.csv"
                    Using accFile As StreamWriter = New StreamWriter(filename, True)
                        accFile.WriteLine(Now.ToString & "," & e.value)
                    End Using
                    lbData.Items.Add(g_accounts(1) & "," & e.value)
                    lbData.TopIndex = lbData.Items.Count - 1
                End If

                Console.WriteLine(Now.ToString & " " & g_dump_daily_data_once & "  Market close and collect data")
                ' set flag back so that timer next fire wont call reqAccountSummary again
                g_dump_daily_data_once = True

                Call AxTws1.cancelAccountSummary(IB_ACC_SUMMARY_REQID)

            End If

        Catch ex As Exception

            MessageBox.Show(ex.ToString)
        End Try

    End Sub

    Private Sub AxTws1_accountSummaryEnd(sender As Object, e As AxTWSLib._DTwsEvents_accountSummaryEndEvent) Handles AxTws1.accountSummaryEnd
        lbData.Items.Add("accountSummaryEnd")
        lbData.TopIndex = lbData.Items.Count - 1
        Call AxTws1.cancelAccountSummary(1)
    End Sub

    Private Sub btnAccUpdate_Click(sender As Object, e As EventArgs) Handles btnAccUpdate.Click
        ' Account update won't work if ninjatrader is connected
        ' Therefore, we are using accountsummary for the meantime, but if TWS is not at second account, then it will return only the main account.
        '
        AxTws1.reqAccountUpdates(1, g_accounts(0))  ' 1 = start receiving, 0 stop
        ' AxTws1.reqAccountUpdates(1, g_accounts(1))

    End Sub

    Private Sub AxTws1_updateAccountValue(sender As Object, e As AxTWSLib._DTwsEvents_updateAccountValueEvent) Handles AxTws1.updateAccountValue

        For i As Integer = 0 To g_accounts.Length - 1

            If e.accountName = g_accounts(i) And e.key = "NetLiquidation" Then
                AxTws1.reqAccountUpdates(0, g_accounts(i))  ' 1 = start receiving, 0 stop

                ' Make sure it is not end of array and access out-of-bound g_accounts array element at i+1
                If i < g_accounts.Length - 1 Then
                    AxTws1.reqAccountUpdates(1, g_accounts(i + 1))
                End If

                lbData.Items.Add(e.accountName & "," & TimeOfDay & "," & e.value)
                lbData.TopIndex = lbData.Items.Count - 1
            End If

        Next

    End Sub

    Private Sub tmrAccUpdate_Tick(sender As Object, e As EventArgs) Handles tmrAccUpdate.Tick
        'Every 5 secs this fires
        'If market hours, then divide minute % 5. if mod is = 0, it is 5th minute (to collect 5 minutes data)
        ' Mark date/time, acc value
        ' Write to file

        ' The issue is accountvalue can 

        Dim market_starttime As DateTime = #12:00:00 AM#
        Dim market_endtime As DateTime = #1:00:00 PM#

        'Console.WriteLine(TimeOfDay)
        'Test if it is market hours.
        If TimeOfDay > market_starttime Then
            If (TimeOfDay.Minute Mod 5) = 0 Then

                ' need this to be called once only
                Console.WriteLine(TimeOfDay & " write file")

                AxTws1.reqAccountUpdates(1, g_accounts(0))



            End If

        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim file As String = "c:\" & g_accounts(0) & "acc_equity_5min.csv"

        Try 'file might throw exception if not exist


            chtEquity.Series(0).Points.Clear()

            chtEquity.DataSource = ""

            dtAccEquity.Clear()

            Using sr As StreamReader = New StreamReader(file)

                While Not sr.EndOfStream
                    Dim dataSplits As String() = sr.ReadLine.Split(",")
                    dtAccEquity.Rows.Add(dataSplits)
                End While
            End Using

            DataGridView1.DataSource = dtAccEquity.DefaultView


            chtEquity.DataSource = dtAccEquity
            chtEquity.DataBind()

            With chtEquity.Series(0)
                .Points.Clear()

                .Points.DataBind(dtAccEquity.DefaultView, "DateTime", "NetLiquidity", Nothing)
                '.XValueMember = "<DateTime>"
                '.YValueMembers = "<NetLiquidity>"
                .ChartType = DataVisualization.Charting.SeriesChartType.Line
            End With

            chtEquity.ResetAutoValues()


        Catch ex As Exception
            MessageBox.Show(ex.ToString)

        End Try

    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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

        cbDaysinchart.SelectedIndex = 0

    End Sub

    Private Sub AxTws1_accountDownloadEnd(sender As Object, e As AxTWSLib._DTwsEvents_accountDownloadEndEvent) Handles AxTws1.accountDownloadEnd
        Console.WriteLine("accountDownlowdEnd")
    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        cbDaysinchart.SelectedItem = 0

    End Sub
End Class
