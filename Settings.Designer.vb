<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnSaveClose = New System.Windows.Forms.Button()
        Me.txtPercentUpThreshold = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtPercentDownThreshold = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtAccountNumber = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'btnSaveClose
        '
        Me.btnSaveClose.Location = New System.Drawing.Point(135, 227)
        Me.btnSaveClose.Name = "btnSaveClose"
        Me.btnSaveClose.Size = New System.Drawing.Size(97, 23)
        Me.btnSaveClose.TabIndex = 0
        Me.btnSaveClose.Text = "Save and Close"
        Me.btnSaveClose.UseVisualStyleBackColor = True
        '
        'txtPercentUpThreshold
        '
        Me.txtPercentUpThreshold.Location = New System.Drawing.Point(251, 66)
        Me.txtPercentUpThreshold.Name = "txtPercentUpThreshold"
        Me.txtPercentUpThreshold.Size = New System.Drawing.Size(52, 20)
        Me.txtPercentUpThreshold.TabIndex = 1
        Me.txtPercentUpThreshold.Text = "10"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(309, 70)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(21, 16)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "%"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 69)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(186, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Percent Equity  UP Threshold to Alert:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 96)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(206, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Percent Equity  DOWN Threshold to Alert:"
        '
        'txtPercentDownThreshold
        '
        Me.txtPercentDownThreshold.Location = New System.Drawing.Point(251, 89)
        Me.txtPercentDownThreshold.Name = "txtPercentDownThreshold"
        Me.txtPercentDownThreshold.Size = New System.Drawing.Size(52, 20)
        Me.txtPercentDownThreshold.TabIndex = 5
        Me.txtPercentDownThreshold.Text = "10"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(309, 93)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(21, 16)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "%"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 43)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(90, 13)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "Account Number:"
        '
        'txtAccountNumber
        '
        Me.txtAccountNumber.Location = New System.Drawing.Point(251, 43)
        Me.txtAccountNumber.Name = "txtAccountNumber"
        Me.txtAccountNumber.Size = New System.Drawing.Size(89, 20)
        Me.txtAccountNumber.TabIndex = 8
        '
        'frmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(352, 262)
        Me.Controls.Add(Me.txtAccountNumber)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtPercentDownThreshold)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtPercentUpThreshold)
        Me.Controls.Add(Me.btnSaveClose)
        Me.Name = "frmSettings"
        Me.Text = "Settings"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnSaveClose As System.Windows.Forms.Button
    Friend WithEvents txtPercentUpThreshold As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtPercentDownThreshold As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtAccountNumber As System.Windows.Forms.TextBox
End Class
