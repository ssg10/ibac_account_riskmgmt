Imports System.IO
Imports System.Timers

Public Class frmMain
    Public g_accounts As String() = {"U129661", "U1465027"}
    Dim g_connecting_inprogress As Boolean = False
    Dim g_onetime_call_account As Boolean = False

    Dim IBCONNECTION_NUMBER As Integer = 69 ' Must be unique number or IB will not connect us



    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

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
            tmrCollectAccSummary.Start()
        Else
            MsgBox("Make sure you are connected to TWS")
        End If



    End Sub

    Private Sub tmrCollectAccSummary_Tick(sender As Object, e As EventArgs) Handles tmrCollectAccSummary.Tick
        'Every 5 secs this fires
        'If market hours, then divide minute % 5. if mod is = 0, it is 5th minute (to collect 5 minutes data)
        ' Mark date/time, acc value
        ' Write to file

        Dim market_starttime As DateTime = #12:00:00 AM#
        Dim market_endtime As DateTime = #1:00:00 PM#

        'Console.WriteLine(TimeOfDay)
        'Test if it is market hours.
        If TimeOfDay > market_starttime Then
            If (TimeOfDay.Minute Mod 5) = 0 And Not g_onetime_call_account Then
                g_onetime_call_account = True
                Console.WriteLine(TimeOfDay & " write file")
                Call AxTws1.reqAccountSummary(1, "All", "AccruedCash,BuyingPower,NetLiquidation")

            End If

        End If


    End Sub

    Private Sub AxTws1_accountSummary(sender As Object, e As AxTWSLib._DTwsEvents_accountSummaryEvent) Handles AxTws1.accountSummary

        Dim filename As String


        If e.account = g_accounts(0) Then

            filename = "c:\" & g_accounts(0) & "acc_equity_5min.csv"
            If e.tag = "NetLiquidation" Then
                Using accFile As StreamWriter = New StreamWriter(filename, True)
                    accFile.WriteLine(Now.ToString & "," & e.value)
                End Using
                lbData.Items.Add(g_accounts(0) & "," & e.value)
                lbData.TopIndex = lbData.Items.Count - 1

            End If
        End If


        If e.account = g_accounts(1) Then
            filename = "c:\" & g_accounts(1) & "acc_equity_5min.csv"
            If e.tag = "NetLiquidation" Then
                Using accFile As StreamWriter = New StreamWriter(filename, True)
                    accFile.WriteLine(Now.ToString & "," & e.value)
                End Using
                lbData.Items.Add(g_accounts(1) & "," & e.value)
                lbData.TopIndex = lbData.Items.Count - 1

            End If
        End If


        'Call AxTws1.cancelAccountSummary(1)

    End Sub

    Private Sub AxTws1_accountSummaryEnd(sender As Object, e As AxTWSLib._DTwsEvents_accountSummaryEndEvent) Handles AxTws1.accountSummaryEnd
        lbData.Items.Add("accountSummaryEnd")
        lbData.TopIndex = lbData.Items.Count - 1
        Call AxTws1.cancelAccountSummary(1)
    End Sub

    Private Sub btnAccUpdate_Click(sender As Object, e As EventArgs) Handles btnAccUpdate.Click
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
End Class
