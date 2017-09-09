Imports LibOptimization.Optimization
Imports LibOptimization.MathUtil
Imports System.Text

Namespace Optimization
    ''' <summary>
    ''' Point Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Point
        Inherits EasyVector
        Implements IComparable

        ''' <summary>Objective fucntion</summary>
        Private _func As absObjectiveFunction = Nothing

        ''' <summary>Evaluate value</summary>
        Private _evaluateValue As Double = 0.0

        ''' <summary>temp1</summary>
        Public Property temp1 As Object = Nothing

        ''' <summary>temp2</summary>
        Public Property temp2 As Object = Nothing

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            'nop
        End Sub

        ''' <summary>
        ''' copy constructor
        ''' </summary>
        ''' <param name="ai_vertex"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_vertex As Point)
            _func = ai_vertex.GetFunc()
            AddRange(ai_vertex) 'ok
            _evaluateValue = ai_vertex.Eval
        End Sub

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="ai_func"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            _func = ai_func
            AddRange(New Double(ai_func.NumberOfVariable - 1) {}) 'ok
            _evaluateValue = _func.F(Me)
        End Sub

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="ai_func"></param>
        ''' <param name="ai_vars"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, ByVal ai_vars As List(Of Double))
            _func = ai_func
            AddRange(ai_vars) 'ok
            _evaluateValue = _func.F(Me)
        End Sub

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="ai_func"></param>
        ''' <param name="ai_vars"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, ByVal ai_vars() As Double)
            _func = ai_func
            AddRange(ai_vars) 'ok
            _evaluateValue = _func.F(Me)
        End Sub

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="ai_func"></param>
        ''' <param name="ai_dim"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, ByVal ai_dim As Integer)
            _func = ai_func
            AddRange(New Double(ai_dim - 1) {}) 'ok
        End Sub

        ''' <summary>
        ''' Compare(ICompareble)
        ''' </summary>
        ''' <param name="ai_obj"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' larger Me than obj is -1. smaller Me than obj is 1.
        ''' Equal is return to Zero
        ''' </remarks>
        Public Function CompareTo(ByVal ai_obj As Object) As Integer Implements System.IComparable.CompareTo
            'Nothing check
            If ai_obj Is Nothing Then
                Return 1
            End If

            'Type check
            If Not [GetType]() Is ai_obj.GetType() Then
                Throw New ArgumentException("Different type", "obj")
            End If

            'Compare
            Dim mineValue As Double = _evaluateValue
            Dim compareValue As Double = DirectCast(ai_obj, Point).Eval
            If mineValue = compareValue Then
                Return 0
            ElseIf mineValue < compareValue Then
                Return -1
            Else
                Return 1
            End If
        End Function

        ''' <summary>
        ''' Re Evaluate
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ReEvaluate()
            _evaluateValue = _func.F(Me)
        End Sub

        ''' <summary>
        ''' Get Function
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFunc() As absObjectiveFunction
            Return _func
        End Function

        ''' <summary>
        ''' EvaluateValue
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Eval() As Double
            Get
                Return _evaluateValue
            End Get
        End Property

        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <param name="ai_range">-ai_range to ai_range</param>
        ''' <param name="ai_rand">Random object</param>
        ''' <remarks></remarks>
        Public Sub InitValue(ByVal ai_range As Double, ByVal ai_rand As System.Random)
            For i As Integer = 0 To _func.NumberOfVariable - 1
                Add(Math.Abs(2.0 * ai_range) * ai_rand.NextDouble() - ai_range)
            Next
        End Sub

        ''' <summary>
        ''' Copy Point
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Copy() As Point
            Return New Point(Me)
        End Function

        ''' <summary>
        ''' Copy Point
        ''' </summary>
        ''' <param name="ai_point"></param>
        ''' <remarks></remarks>
        Public Sub Copy(ByVal ai_point As Point)
            For i As Integer = 0 To ai_point.Count - 1
                Me(i) = ai_point(i)
            Next
            _evaluateValue = ai_point.Eval
        End Sub

        ''' <summary>
        ''' Override ToString
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            If Me.Count = 0 Then
                Return ""
            End If

            Dim str = New StringBuilder()
            str.Append("Eval:")
            str.Append(vbCrLf)
            str.Append(String.Format("{0}", Eval))
            str.Append(vbCrLf)
            str.Append("Variables:")
            str.Append(vbCrLf)
            For Each var In Me
                str.Append(String.Format("{0}", var))
                str.Append(vbCrLf)
            Next

            Return str.ToString()
        End Function
    End Class
End Namespace