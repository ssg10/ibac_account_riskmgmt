﻿Imports System.IO
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
    Dim g_write_daily_data_once As Boolean = False
    Dim g_starting_equity As Double = 0
    Public g_pct_equity_up_threshold_alert As Decimal = 8
    Public g_pct_equity_down_threshold_alert As Decimal = 8
    Dim g_user_has_setting As Boolean


    Dim MARKET_OPEN_TIME As DateTime = #6:30:00 AM#
    Dim MARKET_CLOSE_TIME As DateTime = #1:00:00 PM#
    
    ' Dim g_returned_calendar_date As DateTime

    'For test
    'Dim MARKET_OPEN_TIME As DateTime = #11:00:00 PM#
    'Dim MARKET_CLOSE_TIME As DateTime = #11:59:00 PM#


    Dim IBCONNECTION_NUMBER As Integer = 7 ' Must be unique number or IB will not connect us
    Dim IB_ACC_SUMMARY_REQID As Integer = 1

    Dim dtAccEquity As New DataTable("dtAccEquity")
    Dim dtDailyAccEquity As New DataTable("dtDailyAccEquity") ' DataTable for temporary streamread daily data file
    Dim dtPctAccEquity As New DataTable("dtPctAccEquity")     ' DataTable for storing percent equity change

    ' Dim fileNameAndPath As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\SomeFile.txt"

    Private Sub AxTws1_errMsg(sender As Object, e As AxTWSLib._DTwsEvents_errMsgEvent) Handles AxTws1.errMsg
        lbErrorAndLog.Items.Add(Now.ToString & ":  " & e.errorMsg)
        lbErrorAndLog.TopIndex = lbErrorAndLog.Items.Count - 1

        Console.WriteLine("Errormsg: " & e.errorMsg)

        'Check if marketdata farm is recoqnized, then connection is good
        Dim connection_msg As String

        connection_msg = "Market data farm"

        If g_connecting_inprogress Then

            If e.errorMsg.Contains(connection_msg) Then
                lblConnected.Text = "CONNECTED"
                lblConnected.BackColor = Color.Green

                'Run servertime timer
                tmrServerTime.Enabled = True

                btnConnect.Enabled = False
                btnDisconnect.Enabled = True

            End If

            ' Just a flag to let this if then executed once
            g_connecting_inprogress = False
        End If
    End Sub

    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        g_connecting_inprogress = True
        Call AxTws1.connect("", "7496", IBCONNECTION_NUMBER)
    End Sub

    Private Sub btnDisconnect_Click(sender As Object, e As EventArgs) Handles btnDisconnect.Click
        Call AxTws1.disconnect()
        lblConnected.Text = "DISCONNECTED"
        lblConnected.BackColor = Color.Red

        btnConnect.Enabled = True
        btnDisconnect.Enabled = False

        tmrServerTime.Enabled = False
    End Sub

    Private Sub btnStartAccSummary_Click(sender As Object, e As EventArgs) Handles btnStartAccSummary.Click

        If String.Compare(lblConnected.Text, "CONNECTED") = 0 Then  '0 means equal
            If Not g_is_acc_summary_subscribed Then
                tmrCollectAccSummary.Start()

                g_is_acc_summary_subscribed = True
                btnStartAccSummary.Text = "Stop Acc Summary"

                'Pre-populate datatable for chart depending on user selection of num of days to see on chart
                Dim ret As Decimal = PopulateEquityDataTable(CDec(cbDaysinchart.Text))
                If ret < 0 Then
                    lbErrorAndLog.Items.Add("Error when populating dtAccEquity for chart")
                    lbErrorAndLog.TopIndex = lbErrorAndLog.Items.Count - 1
                End If

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
        '      g_dump_Daily_data_once is to make sure reqAccountSummary is called once
        '      g_write_daily_data_once is to make sure the accountsummary event is processed once, since there is a chance it triggers more than 1 event for 1 req call
        If (TimeOfDay > MARKET_CLOSE_TIME.AddMinutes(1)) And (TimeOfDay < MARKET_CLOSE_TIME.AddMinutes(5)) And Not g_dump_daily_data_once Then

            Console.WriteLine(Now & " Write EOD data")

            Call AxTws1.reqAccountSummary(IB_ACC_SUMMARY_REQID, "All", "NetLiquidation")  'assume data req comes back faster than timer fires (1 sec or so)
            g_write_daily_data_once = True
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

                            'Set up chart and plot
                            With chtEquity.ChartAreas(0)
                                .AxisY.Minimum = e.value - 200
                                .AxisY.Maximum = e.value + 200
                            End With

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
                                .BorderWidth = 3
                                .BorderColor = Color.Black
                            End With

                            chtEquity.ResetAutoValues()

                            'Update percent pnl label
                            Dim pnl_pct As Decimal = ((e.value - g_starting_equity) / g_starting_equity) * 100
                            lblDayPct.Text = pnl_pct.ToString("N2")
                            If pnl_pct < 0 Then
                                lblDayPct.ForeColor = Color.Red
                            Else
                                lblDayPct.ForeColor = Color.Green
                            End If

                            'Plot it on percent chart
                            With chtPctEquity.ChartAreas(0)
                                .AxisY.LabelStyle.Format = "{0:0.00}"
                                .AxisY.Minimum = pnl_pct - 10
                                .AxisY.Maximum = pnl_pct + 10
                            End With

                            dtPctAccEquity.Rows.Add({Now.ToString, pnl_pct.ToString("N2")})
                            dgPercent.DataSource = dtPctAccEquity.DefaultView

                            chtPctEquity.Series(0).Points.Clear()
                            chtPctEquity.DataSource = ""
                            chtPctEquity.ResetAutoValues()
                            chtPctEquity.DataSource = dtPctAccEquity
                            chtPctEquity.DataBind()


                            With chtPctEquity.Series(0)
                                .Points.Clear()
                                .Points.DataBind(dtPctAccEquity.DefaultView, "DateTime", "PctPnl", Nothing)
                                .ChartType = DataVisualization.Charting.SeriesChartType.Line
                                .BorderWidth = 3
                                .BorderColor = Color.Black
                            End With
                            chtPctEquity.ResetAutoValues()

                            'Check if pnl pct is higher than threshold to show alert or even close all
                            If pnl_pct > frmSettings.pct_equity_up_threshold_alert Then
                                lblWarning.ForeColor = Color.Red
                                lblWarning.Text = "YOUR TODAY'S GAIN IS OVER " & frmSettings.pct_equity_up_threshold_alert.ToString &
                                    "THRESHOLD. TAKE PROFIT. "
                            ElseIf pnl_pct < frmSettings.pct_equity_down_threshold_alert Then
                                lblWarning.ForeColor = Color.Red
                                lblWarning.Text = "YOUR TODAY'S LOSS EXCEEDS " & frmSettings.pct_equity_up_threshold_alert.ToString &
                                    "THRESHOLD. CLOSE HALF OR ALL. "
                            End If
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
            If (TimeOfDay > MARKET_CLOSE_TIME.AddMinutes(1)) And (TimeOfDay < MARKET_CLOSE_TIME.AddMinutes(3)) And (g_write_daily_data_once) Then

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
                g_write_daily_data_once = False
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

    Private Sub btnIntradayChart_Click(sender As Object, e As EventArgs) Handles btnIntradayChart.Click
        Dim dtTempEquity As New DataTable("dtTempEquity")

        Dim file As String = "c:\" & g_accounts(0) & "acc_equity_5min.csv"

        dtTempEquity.Columns.Add("DateTime")
        dtTempEquity.Columns.Add("NetLiquidity")

        Try 'file might throw exception if not exist among other exceptions
            chtEquity.Series(0).Points.Clear()

            chtEquity.DataSource = ""

            'dtAccEquity.Clear()

            Using sr As StreamReader = New StreamReader(file)
                While Not sr.EndOfStream
                    Dim dataSplits As String() = sr.ReadLine.Split(",")
                    dtTempEquity.Rows.Add(dataSplits)
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
                .BorderWidth = 3
                .BorderColor = Color.Black
            End With

            chtEquity.ResetAutoValues()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)

        End Try

    End Sub


    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblWarning.Text = Now.Date

        'Init datatables
        dtAccEquity.Columns.Add("DateTime")
        dtAccEquity.Columns.Add("NetLiquidity")
        dtDailyAccEquity.Columns.Add("DateTime")
        dtDailyAccEquity.Columns.Add("NetLiquidity")
        dtPctAccEquity.Columns.Add("DateTime")
        dtPctAccEquity.Columns.Add("PctPnl")

        'Set first percent as 0 at first row
        dtPctAccEquity.Rows.Add({Now.ToString, 0})

        chtEquity.Series.Clear()
        chtPctEquity.Series.Clear()

        'Create chart
        chtEquity.Series.Add("AccEquity")
        chtEquity.Name = "AccEquity"
        chtPctEquity.Series.Add("PctPnl")
        chtPctEquity.Name = "PctPnl"

        With chtEquity.ChartAreas(0)
            .AxisX.Title = "Date"
            .AxisY.Title = "$ Equity"
        End With

        With chtPctEquity.ChartAreas(0)
            .AxisX.Title = "Date"
            .AxisY.Title = "P&&L %"
        End With

        'Show first index as default view
        cbDaysinchart.SelectedIndex = 1

        'Gray out disconnect button
        btnDisconnect.Enabled = False

        'Find the last row in daily equity datatable and display on label
        If ReadDailyEquityFile() = 0 Then ' 0 = no error
            g_starting_equity = dtDailyAccEquity.Rows(dtDailyAccEquity.Rows.Count - 1).Item("NetLiquidity")

            lblStartingEquity.Text = g_starting_equity.ToString
        End If

        txtEndDate.Text = Now.Date
        txtStartDate.Text = Now.AddDays(-5).Date

        AccessSettingFile("read")

        lblAccountNum.Text = g_accounts(0)

    End Sub

    Private Sub AxTws1_accountDownloadEnd(sender As Object, e As AxTWSLib._DTwsEvents_accountDownloadEndEvent) Handles AxTws1.accountDownloadEnd
        Console.WriteLine("accountDownlowdEnd")
    End Sub



    Private Sub btnStartDate_Click(sender As Object, e As EventArgs) Handles btnStartDate.Click
        calendar.ShowDialog()

        If calendar.ok() Then
            txtStartDate.Text = calendar.selected_date()
        End If

    End Sub

    Private Sub btnEndDate_Click(sender As Object, e As EventArgs) Handles btnEndDate.Click
        calendar.ShowDialog()

        If calendar.ok Then
            txtEndDate.Text = calendar.selected_date()
        End If
    End Sub

    Private Sub btnOpenChart_Click(sender As Object, e As EventArgs) Handles btnOpenChart.Click
        frmHistoricalChart.chart_timeframe = "intraday"
        frmHistoricalChart.Show()

    End Sub


    Private Sub btnDailyChart_Click(sender As Object, e As EventArgs) Handles btnDailyChart.Click
        frmHistoricalChart.chart_timeframe = "daily"
        frmHistoricalChart.Show()
    End Sub

    Private Function ReadDailyEquityFile() As Integer
        '@ Read daily equity file (acc_equity_daily.csv) and load it to table for use later
        '@ Return:  dtDailyAccEquity

        Dim file As String = "c:\" & g_accounts(0) & "acc_equity_daily.csv"

        Try
            'RELoad daily equity data file and load it to datatable
            dtDailyAccEquity.Clear()

            Using sr As StreamReader = New StreamReader(file)
                While Not sr.EndOfStream
                    Dim dataSplits As String() = sr.ReadLine.Split(",")
                    dtDailyAccEquity.Rows.Add(dataSplits)
                End While
            End Using
            ReadDailyEquityFile = 0
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
            ReadDailyEquityFile = -1
        End Try

    End Function


    Private Sub menuSetting_Click(sender As Object, e As EventArgs) Handles menuSetting.Click
        frmSettings.ShowDialog()

    End Sub

    Sub PlayWarningSound(warningtype As String)
        Try
            Select Case warningtype

                Case "up"
                    My.Computer.Audio.Play("C:\dailyprofithit.wav",
                        AudioPlayMode.WaitToComplete)
                Case "down"
                    My.Computer.Audio.Play("C:\dailystopexceeds.wav",
                        AudioPlayMode.WaitToComplete)

            End Select

            My.Computer.Audio.Stop()
        Catch ex As Exception

            MessageBox.Show(ex.ToString)

        End Try
    End Sub

    Sub PlayAsteriskSound()
        My.Computer.Audio.PlaySystemSound(
            System.Media.SystemSounds.Asterisk)
    End Sub

    Private Sub AxTws1_currentTime(sender As Object, e As AxTWSLib._DTwsEvents_currentTimeEvent) Handles AxTws1.currentTime
        Dim servertime As DateTime = ConvertTimestamp(e.time)

        lblServerTime.Text = servertime.TimeOfDay.ToString
    End Sub

    Private Function PopulateEquityDataTable(num_days As Decimal) As Integer

        Dim file As String = "c:\" & g_accounts(0) & "acc_equity_5min.csv"
        Dim days As Integer = num_days - 1  ' if user asks 2 days, meaning we will load 1 prev day worth of data
        Dim furthest_backdate As Date = Now

        furthest_backdate = furthest_backdate.AddDays(-1 * days)
        Console.WriteLine("PopulateEquityDataTable: furthest day back = " & furthest_backdate.Date)

        Try
            '!!!! CLEAR DATATABLE !!!!!!!
            dtAccEquity.Clear()

            Using sr As StreamReader = New StreamReader(file)
                While Not sr.EndOfStream
                    Dim dataSplits As String() = sr.ReadLine.Split(",")

                    'Only add num_days before
                    If DateTime.Compare(CDate(dataSplits(0)), furthest_backdate) >= 0 Then
                        dtAccEquity.Rows.Add(dataSplits)
                    End If

                End While
            End Using
        Catch ex As Exception

            MessageBox.Show(ex.ToString)
            'Return error
            PopulateEquityDataTable = -1
        End Try

        'Return no error
        PopulateEquityDataTable = 0
    End Function

    Public Function AccessSettingFile(action As String) As Decimal
        'Load program and user settings from file
        ' File Structure:
        '     ibac_settings.txt
        '     <account number>
        '     <equity threshold high>
        '     <equity threshold low>
        '     <user has done init setup>

        Dim filename = "c:\ibac_settings.txt"

        Try
            If action = "read" Then
                Dim lines() As String = IO.File.ReadAllLines(filename)


                'Using sr As StreamReader = New StreamReader(filename)
                'While Not sr.EndOfStream
                'Dim data As String = sr.ReadLine

                For line As Integer = 0 To lines.Length - 1
                    lbErrorAndLog.Items.Add("Read Settings index = " & line.ToString)
                    lbErrorAndLog.Items.Add(lines(line))
                    lbErrorAndLog.TopIndex = lbErrorAndLog.Items.Count - 1

                    Select Case line

                        Case 0
                            g_accounts(0) = lines(line)
                        Case 1
                            g_pct_equity_up_threshold_alert = lines(line)
                        Case 2
                            g_pct_equity_down_threshold_alert = lines(line)

                    End Select

                Next

                'End While
                'End Using

            ElseIf action = "write" Then
                g_pct_equity_up_threshold_alert = frmSettings.pct_equity_up_threshold_alert()
                g_pct_equity_down_threshold_alert = frmSettings.pct_equity_down_threshold_alert()
                g_accounts(0) = frmSettings.account_num()


                Using accFile As StreamWriter = New StreamWriter(filename, False)
                    accFile.WriteLine(g_accounts(0))
                    accFile.WriteLine(g_pct_equity_up_threshold_alert)
                    accFile.WriteLine(g_pct_equity_down_threshold_alert)
                End Using

            End If
        Catch
            MsgBox("This is the first time you use the software. Let us configure the settings first")
            frmSettings.Show()

        End Try

        AccessSettingFile = 0

    End Function

    Private Sub tmrServerTime_Tick(sender As Object, e As EventArgs) Handles tmrServerTime.Tick
        AxTws1.reqCurrentTime()

    End Sub

    Function ConvertTimestamp(ByVal timestamp As Double) As DateTime
        Return New DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp).ToLocalTime
    End Function
End Class
