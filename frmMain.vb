Imports System.IO

Public Class frmMain
    Public g_accounts As String() = {"U129661", "U1465027"}
    Dim g_connecting_inprogress As Boolean = False

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
End Class
