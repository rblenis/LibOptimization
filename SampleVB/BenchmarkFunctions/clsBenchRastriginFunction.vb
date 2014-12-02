﻿''' <summary>
''' Benchmark function
''' Rastrigin function
''' </summary>
''' <remarks>
''' Minimum:
'''  x = {0,...,0}
''' 
''' Referrence:
''' http://mikilab.doshisha.ac.jp/dia/research/pdga/archive/doc/ga2k_performance.pdf
''' </remarks>
Public Class clsBenchRastriginFunction : Inherits LibOptimization.absObjectiveFunction
    Private dimension As Integer = 0

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal ai_dim As Integer)
        Me.dimension = ai_dim
    End Sub

    ''' <summary>
    ''' Target Function
    ''' </summary>
    ''' <param name="ai_var"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function F(ByVal ai_var As List(Of Double)) As Double
        If ai_var Is Nothing Then
            Return 0
        End If

        If Me.dimension <> ai_var.Count Then
            Return 0
        End If

        Dim A As Double = 10.0
        Dim ret As Double = 0.0
        For i As Integer = 0 To Me.dimension - 1
            ret += ai_var(i) ^ 2 - A * Math.Cos(2 * 3.141592653589 * ai_var(i))
        Next
        ret += A

        Return ret
    End Function

    Public Overloads Function F(ByVal ai_var() As Double) As Double
        Return Me.F(New List(Of Double)(ai_var))
    End Function

    Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
        Return Nothing
    End Function

    Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
        Return Nothing
    End Function

    Public Overrides ReadOnly Property NumberOfVariable As Integer
        Get
            Return Me.dimension
        End Get
    End Property
End Class