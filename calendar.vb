Public Class calendar
    Private m_selected_date As DateTime
    Private m_ok As Boolean

    Public Property selected_date() As DateTime
        Get
            selected_date = m_selected_date
        End Get
        Set(ByVal Value As DateTime)
            m_selected_date = Value
            'txtReqId.Text = CStr(m_orderId)
        End Set
    End Property

    Public ReadOnly Property ok() As Boolean
        Get
            ok = m_ok
        End Get
    End Property

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        m_ok = True
        Hide()
    End Sub


    Private Sub MonthCalendar1_DateSelected(sender As Object, e As DateRangeEventArgs) Handles MonthCalendar1.DateSelected
        m_selected_date = e.Start.Date
    End Sub

    Private Sub calendar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        m_ok = False
    End Sub
End Class