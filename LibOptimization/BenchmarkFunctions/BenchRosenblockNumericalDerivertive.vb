Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Rosenblock function(Banana function)
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  F(0,...,0) = 0
    ''' </remarks>
    Public Class BenchRosenblockNumericalDerivertive : Inherits absObjectiveFunction
        Private dimension As Integer = 0

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <param name="ai_dim">Set dimension</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_dim As Integer)
            If ai_dim <= 1 Then
                Throw New NotImplementedException
            End If
            dimension = ai_dim
        End Sub

        Public Overrides Function F(ByVal x As List(Of Double)) As Double
            If x Is Nothing Then
                Return 0
            End If

            If dimension <> x.Count Then
                Return 0
            End If

            Dim ret As Double = 0.0
            For i As Integer = 0 To dimension - 2
                ret += 100 * (x(i + 1) - x(i) ^ 2) ^ 2 + (x(i) - 1) ^ 2
            Next

            Return ret
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Return Me.NumericDerivertive(ai_var)
        End Function

        Public Overrides Function Hessian(ByVal x As List(Of Double)) As List(Of List(Of Double))
            Dim hesse As New List(Of List(Of Double))
            Dim tempVect(dimension - 1) As Double
            For i As Integer = 0 To dimension - 1
                hesse.Add(New List(Of Double)(tempVect))
            Next

            If dimension = 2 Then
                For i As Integer = 0 To dimension - 1
                    For j As Integer = 0 To dimension - 1
                        If i = j Then
                            If i <> dimension - 1 Then
                                hesse(i)(j) = -400 * (x(i + 1) - x(i) ^ 2) + 800 * x(i) ^ 2 - 2
                            Else
                                hesse(i)(j) = 200
                            End If
                        Else
                            hesse(i)(j) = -400 * x(0)
                        End If
                    Next
                Next
            Else
                For i As Integer = 0 To dimension - 1
                    For j As Integer = 0 To dimension - 1
                        If i = j Then
                            If i = 0 Then
                                hesse(i)(j) = -400 * (x(i + 1) - x(i) ^ 2) + 800 * x(i) ^ 2 - 2
                            ElseIf i = dimension - 1 Then
                                hesse(i)(j) = 200
                            Else
                                hesse(i)(j) = -400 * (x(i + 1) - x(i) ^ 2) + 800 * x(i) ^ 2 + 198
                            End If
                        End If
                        If i = j - 1 Then
                            hesse(i)(j) = -400 * x(i)
                        End If
                        If i - 1 = j Then
                            hesse(i)(j) = -400 * x(j)
                        End If
                    Next
                Next
            End If

            Return hesse
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return dimension
        End Function
    End Class

End Namespace
