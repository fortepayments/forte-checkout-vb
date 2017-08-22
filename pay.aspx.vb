Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Security.Cryptography
Imports ForteCheckoutSimple.ForteLibrary



Public Class pay
    Inherits System.Web.UI.Page
    Private cliSignature As New ForteLibrary.Gateway.Signature()
    Public return_string As String = "Initial"
    Public source As String = ""

    ' Capture the time in Ticks.
    Public utc_time As String = DateTime.Now.Ticks.ToString()
    Public pay_now_single_return_string As String
    Public pay_schedule_amount_return_string As String
    Public pay_range_select_amount_return_string As String
    Public pay_range_select_amount_labels As String
    Public api_loginID As String




    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        cliSignature.api_login_id = "YourAPILoginID" 'Add your API Login ID.
        cliSignature.secure_trans_key = "YourSecureTransKey" 'Add your secure transaction key.


        cliSignature.pay_now_single_payment = cliSignature.api_login_id & "|sale|1.0|10.00|" & utc_time & "|A1234||"
        cliSignature.pay_schedule_amount = cliSignature.api_login_id & "|schedule|1.0|1-9.5;5|" & utc_time & "|A1234|" & cliSignature.customer_token & "|" & cliSignature.payment_token
        cliSignature.pay_range_select_amount = cliSignature.api_login_id & "|sale|1.0|{20,40,60,80,100,0};20-1000|" & utc_time & "|||"
        cliSignature.pay_range_select_amount_labels = cliSignature.api_login_id & "|sale|1.0|{1375.23,1573.66,56.99,0|Total outstanding,Last statement balance,Minimum balance,Specify different amount}|" & utc_time & "|||"
        api_loginID = cliSignature.api_login_id
        pay_now_single_return_string = Gateway.EndPoint(cliSignature, "CREATESIGNATUREPAYSINGLEAMOUNT")
        pay_schedule_amount_return_string = Gateway.EndPoint(cliSignature, "CREATESIGNATURESCHEDULE")
        pay_range_select_amount_return_string = Gateway.EndPoint(cliSignature, "CREATESIGNATURERANGE")
        pay_range_select_amount_labels = Gateway.EndPoint(cliSignature, "CREATESIGNATURERANGELABEL")

    End Sub

End Class


Namespace ForteLibrary

    Public NotInheritable Class Gateway
        Private Sub New()
        End Sub

#Region "CONSTANTS"

        Public Class Signature
            Public Property api_login_id() As String
                Get
                    Return m_api_login_id
                End Get
                Set(value As String)
                    m_api_login_id = value
                End Set
            End Property
            Private m_api_login_id As String
            Public Property secure_trans_key() As String
                Get
                    Return m_secure_trans_key
                End Get
                Set(value As String)
                    m_secure_trans_key = value
                End Set
            End Property
            Private m_secure_trans_key As String
            Public Property pay_now_single_payment() As String
                Get
                    Return m_pay_now_single_payment
                End Get
                Set(value As String)
                    m_pay_now_single_payment = value
                End Set
            End Property
            Private m_pay_now_single_payment As String
            Public Property pay_schedule_amount() As String
                Get
                    Return m_pay_schedule_amount
                End Get
                Set(value As String)
                    m_pay_schedule_amount = value
                End Set
            End Property
            Private m_pay_schedule_amount As String
            Public Property pay_range_select_amount() As String
                Get
                    Return m_pay_range_select_amount
                End Get
                Set(value As String)
                    m_pay_range_select_amount = value
                End Set
            End Property
            Private m_pay_range_select_amount As String
            Public Property pay_range_select_amount_labels() As String
                Get
                    Return m_pay_range_select_amount_labels
                End Get
                Set(value As String)
                    m_pay_range_select_amount_labels = value
                End Set
            End Property
            Private m_pay_range_select_amount_labels As String
            Public Property utc_time() As String
                Get
                    Return m_utc_time
                End Get
                Set(value As String)
                    m_utc_time = value
                End Set
            End Property
            Private m_utc_time As String

            Public Property customer_token() As String
                Get
                    Return m_customer_token
                End Get
                Set(value As String)
                    m_customer_token = value
                End Set
            End Property
            Private m_customer_token As String
            Public Property payment_token() As String
                Get
                    Return m_payment_token
                End Get
                Set(value As String)
                    m_payment_token = value
                End Set
            End Property
            Private m_payment_token As String
        End Class

#End Region

        Shared objSign As New Signature()
        Shared myoper As New Operation()
        Shared mySignature As String = "Error"

        Public Shared Function EndPoint(SignClient As Signature, Operation As String) As String
            objSign = SignClient

            Select Case Operation

                Case "CREATESIGNATUREPAYSINGLEAMOUNT"
                    mySignature = myoper.CreateSignature(objSign.pay_now_single_payment, objSign.secure_trans_key)
                    Exit Select

                Case "CREATESIGNATURESCHEDULE"
                    mySignature = myoper.CreateSignature(objSign.pay_schedule_amount, objSign.secure_trans_key)
                    Exit Select

                Case "CREATESIGNATURERANGE"
                    mySignature = myoper.CreateSignature(objSign.pay_range_select_amount, objSign.secure_trans_key)
                    Exit Select

                Case "CREATESIGNATURERANGELABEL"
                    mySignature = myoper.CreateSignature(objSign.pay_range_select_amount_labels, objSign.secure_trans_key)
                    Exit Select
            End Select

            Return mySignature

        End Function

    End Class
    Friend Class Operation

        Public Sub New()
        End Sub
        Friend Function CreateSignature(strSource As String, key As String) As String
            Dim Signature As String = "error"

            Dim encoding As New System.Text.ASCIIEncoding()
            Dim keyByte As Byte() = encoding.GetBytes(key)
            Dim messageBytes As Byte() = encoding.GetBytes(strSource)
            Dim hashmessage As Byte()


            hashmessage = Nothing
            Signature = Nothing
            Dim hmacmd5 As New HMACMD5(keyByte)
            hashmessage = hmacmd5.ComputeHash(messageBytes)
            Signature = ByteToString(hashmessage)

            Return Signature

        End Function

        Public Shared Function ByteToString(buff As Byte()) As String
            Dim sbinary As String = ""

            For i As Integer = 0 To buff.Length - 1
                ' hex format
                sbinary += buff(i).ToString("X2")
            Next
            Return (sbinary)
        End Function

    End Class


End Namespace

