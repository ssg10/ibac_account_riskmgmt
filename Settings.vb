Public Class frmSettings

    Dim m_pct_equity_up_threshold_alert As Decimal = 10
    Dim m_pct_equity_down_threshold_alert As Decimal = -10

    Public ReadOnly Property pct_equity_up_threshold_alert() As Decimal
        Get
            pct_equity_up_threshold_alert = m_pct_equity_up_threshold_alert
        End Get
    End Property
    Public ReadOnly Property pct_equity_down_threshold_alert() As Decimal
        Get
            pct_equity_down_threshold_alert = m_pct_equity_down_threshold_alert
        End Get
    End Property

    Private Sub btnSaveClose_Click(sender As Object, e As EventArgs) Handles btnSaveClose.Click
        m_pct_equity_up_threshold_alert = CDec(txtPercentUpThreshold.Text)
        m_pct_equity_down_threshold_alert = CDec(txtPercentDownThreshold.Text)
        Me.Hide()
    End Sub

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtPercentDownThreshold.Text = CStr(m_pct_equity_down_threshold_alert)
        txtPercentUpThreshold.Text = CStr(m_pct_equity_up_threshold_alert)
    End Sub
End Class