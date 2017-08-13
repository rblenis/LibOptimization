Namespace Util
    ''' <summary>
    ''' Error manage class(Singleton)
    ''' </summary>
    Public Class clsError
        Private Shared _errors As New List(Of clsDetailError)

        Public Enum ErrorType
            ERR_INIT
            ERR_ITERATION
            ERR_EXCEPTION
            ERR_UNKNOWN
        End Enum

        Private Class clsDetailError
            Public Property ErrorType As ErrorType
            Public Property Message As String

            Public Sub New()
                'nop
            End Sub

            Public Sub New(ByVal eType As ErrorType, ByVal mes As String)
                ErrorType = eType
                Message = mes
            End Sub

            Public Overrides Function ToString() As String
                Dim retStr As String = String.Empty
                retStr = String.Format("ErrorType={0}{1}Message={2}", ErrorType, Environment.NewLine, Message)
                Return retStr
            End Function
        End Class

        ''' <summary>
        ''' Clear error
        ''' </summary>
        Public Shared Sub Clear()
            If _errors Is Nothing Then
                Return
            End If

            _errors.Clear()
        End Sub

        ''' <summary>
        ''' Set error
        ''' </summary>
        ''' <param name="eType"></param>
        ''' <param name="message"></param>
        Public Shared Sub SetError(ByVal eType As ErrorType, Optional ByVal message As String = "")
            If _errors Is Nothing Then
                _errors = New List(Of clsDetailError)
                _errors.Add(New clsDetailError(eType, message))
                Return
            End If
        End Sub

        Public Shared Function IsRecentError() As Boolean
            If _errors.Count = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Shared Function GetRecentError() As String
            Return _errors(_errors.Count - 1).ToString
        End Function

        Public Shared Function GetRecentErrorType() As ErrorType
            Return _errors(_errors.Count - 1).ErrorType
        End Function
    End Class
End Namespace
