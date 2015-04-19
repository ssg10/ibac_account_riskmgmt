<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Dim ChartArea2 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend2 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.AxTws1 = New AxTWSLib.AxTws()
        Me.lbErrorAndLog = New System.Windows.Forms.ListBox()
        Me.lblConnected = New System.Windows.Forms.Label()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.btnDisconnect = New System.Windows.Forms.Button()
        Me.chtEquity = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.lbData = New System.Windows.Forms.ListBox()
        Me.dsAccountBalance = New System.Data.DataSet()
        Me.AccountNetLiq = New System.Data.DataTable()
        Me.DataColumn1 = New System.Data.DataColumn()
        Me.DataColumn2 = New System.Data.DataColumn()
        Me.DataColumn3 = New System.Data.DataColumn()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.btnStartAccSummary = New System.Windows.Forms.Button()
        Me.tmrCollectAccSummary = New System.Windows.Forms.Timer(Me.components)
        Me.btnAccUpdate = New System.Windows.Forms.Button()
        CType(Me.AxTws1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.chtEquity, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsAccountBalance, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AccountNetLiq, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'AxTws1
        '
        Me.AxTws1.Enabled = True
        Me.AxTws1.Location = New System.Drawing.Point(601, 472)
        Me.AxTws1.Name = "AxTws1"
        Me.AxTws1.OcxState = CType(resources.GetObject("AxTws1.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxTws1.Size = New System.Drawing.Size(100, 50)
        Me.AxTws1.TabIndex = 0
        '
        'lbErrorAndLog
        '
        Me.lbErrorAndLog.FormattingEnabled = True
        Me.lbErrorAndLog.Location = New System.Drawing.Point(12, 461)
        Me.lbErrorAndLog.Name = "lbErrorAndLog"
        Me.lbErrorAndLog.Size = New System.Drawing.Size(478, 95)
        Me.lbErrorAndLog.TabIndex = 1
        '
        'lblConnected
        '
        Me.lblConnected.AutoSize = True
        Me.lblConnected.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblConnected.Location = New System.Drawing.Point(512, 18)
        Me.lblConnected.Name = "lblConnected"
        Me.lblConnected.Size = New System.Drawing.Size(189, 24)
        Me.lblConnected.TabIndex = 2
        Me.lblConnected.Text = "NOT CONNECTED"
        '
        'btnConnect
        '
        Me.btnConnect.Location = New System.Drawing.Point(29, 18)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(133, 37)
        Me.btnConnect.TabIndex = 3
        Me.btnConnect.Text = "Connect to IB"
        Me.btnConnect.UseVisualStyleBackColor = True
        '
        'btnDisconnect
        '
        Me.btnDisconnect.Location = New System.Drawing.Point(226, 18)
        Me.btnDisconnect.Name = "btnDisconnect"
        Me.btnDisconnect.Size = New System.Drawing.Size(136, 37)
        Me.btnDisconnect.TabIndex = 4
        Me.btnDisconnect.Text = "Disconnect"
        Me.btnDisconnect.UseVisualStyleBackColor = True
        '
        'chtEquity
        '
        ChartArea2.Name = "ChartArea1"
        Me.chtEquity.ChartAreas.Add(ChartArea2)
        Legend2.Name = "Legend1"
        Me.chtEquity.Legends.Add(Legend2)
        Me.chtEquity.Location = New System.Drawing.Point(12, 82)
        Me.chtEquity.Name = "chtEquity"
        Series2.ChartArea = "ChartArea1"
        Series2.Legend = "Legend1"
        Series2.Name = "Series1"
        Me.chtEquity.Series.Add(Series2)
        Me.chtEquity.Size = New System.Drawing.Size(478, 354)
        Me.chtEquity.TabIndex = 5
        Me.chtEquity.Text = "Chart1"
        '
        'lbData
        '
        Me.lbData.FormattingEnabled = True
        Me.lbData.Location = New System.Drawing.Point(516, 435)
        Me.lbData.Name = "lbData"
        Me.lbData.Size = New System.Drawing.Size(306, 121)
        Me.lbData.TabIndex = 6
        '
        'dsAccountBalance
        '
        Me.dsAccountBalance.DataSetName = "NewDataSet"
        Me.dsAccountBalance.Tables.AddRange(New System.Data.DataTable() {Me.AccountNetLiq})
        '
        'AccountNetLiq
        '
        Me.AccountNetLiq.Columns.AddRange(New System.Data.DataColumn() {Me.DataColumn1, Me.DataColumn2, Me.DataColumn3})
        Me.AccountNetLiq.TableName = "AccountNetLiq"
        '
        'DataColumn1
        '
        Me.DataColumn1.ColumnName = "Date"
        '
        'DataColumn2
        '
        Me.DataColumn2.ColumnName = "Time"
        '
        'DataColumn3
        '
        Me.DataColumn3.ColumnName = "NetLiq"
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(516, 59)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(306, 264)
        Me.DataGridView1.TabIndex = 7
        '
        'btnStartAccSummary
        '
        Me.btnStartAccSummary.Location = New System.Drawing.Point(516, 361)
        Me.btnStartAccSummary.Name = "btnStartAccSummary"
        Me.btnStartAccSummary.Size = New System.Drawing.Size(122, 31)
        Me.btnStartAccSummary.TabIndex = 8
        Me.btnStartAccSummary.Text = "Start Acc Summary"
        Me.btnStartAccSummary.UseVisualStyleBackColor = True
        '
        'tmrCollectAccSummary
        '
        Me.tmrCollectAccSummary.Interval = 5000
        '
        'btnAccUpdate
        '
        Me.btnAccUpdate.Location = New System.Drawing.Point(664, 363)
        Me.btnAccUpdate.Name = "btnAccUpdate"
        Me.btnAccUpdate.Size = New System.Drawing.Size(111, 29)
        Me.btnAccUpdate.TabIndex = 9
        Me.btnAccUpdate.Text = "Start Acc Update"
        Me.btnAccUpdate.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(861, 568)
        Me.Controls.Add(Me.btnAccUpdate)
        Me.Controls.Add(Me.btnStartAccSummary)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.lbData)
        Me.Controls.Add(Me.chtEquity)
        Me.Controls.Add(Me.btnDisconnect)
        Me.Controls.Add(Me.btnConnect)
        Me.Controls.Add(Me.lblConnected)
        Me.Controls.Add(Me.lbErrorAndLog)
        Me.Controls.Add(Me.AxTws1)
        Me.Name = "frmMain"
        Me.Text = "IBac - IB Account Risk Management"
        CType(Me.AxTws1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.chtEquity, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsAccountBalance, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AccountNetLiq, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents AxTws1 As AxTWSLib.AxTws
    Friend WithEvents lbErrorAndLog As System.Windows.Forms.ListBox
    Friend WithEvents lblConnected As System.Windows.Forms.Label
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents btnDisconnect As System.Windows.Forms.Button
    Friend WithEvents chtEquity As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents lbData As System.Windows.Forms.ListBox
    Friend WithEvents dsAccountBalance As System.Data.DataSet
    Friend WithEvents AccountNetLiq As System.Data.DataTable
    Friend WithEvents DataColumn1 As System.Data.DataColumn
    Friend WithEvents DataColumn2 As System.Data.DataColumn
    Friend WithEvents DataColumn3 As System.Data.DataColumn
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents btnStartAccSummary As System.Windows.Forms.Button
    Friend WithEvents tmrCollectAccSummary As System.Windows.Forms.Timer
    Friend WithEvents btnAccUpdate As System.Windows.Forms.Button

End Class
