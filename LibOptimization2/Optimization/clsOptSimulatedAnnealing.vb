Imports LibOptimization2.Util

Namespace Optimization
    ''' <summary>
    ''' Simulated Annealing
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Randomized algorithm for optimization.
    ''' 
    ''' Reffrence:
    ''' http://ja.wikipedia.org/wiki/%E7%84%BC%E3%81%8D%E3%81%AA%E3%81%BE%E3%81%97%E6%B3%95
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptSimulatedAnnealing : Inherits LibOptimization2.Optimization.absOptimization
#Region "Member(Original parameter for Simulated Annealing)"
        ''' <summary>cooling ratio</summary>
        Public Property CoolingRatio As Double = 0.9995

        ''' <summary>range of neighbor search</summary>
        Public Property NeighborRange As Double = 0.1

        ''' <summary>start temperature</summary>
        Public Property Temperature As Double = 1000.0

        ''' <summary>end temperature</summary>
        Public Property EndTemperature As Double = 10.0

        ''' <summary>reserve best point</summary>
        Private _bestPoint As clsPoint = Nothing
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize setting and value
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Init() As Boolean
            MyBase.PopulationSize = 1
            MyBase.UseAdaptivePopulationSize = False
            MyBase.UseCriterion = False

            If MyBase.Init() = False Then
                Return False
            End If

            Me._bestPoint = New clsPoint(_populations(0))

            Return True
        End Function

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            If ai_iteration = 0 Then
                ai_iteration = Me.Iteration - 1
            End If

            'Do Iteration
            Try
                ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
                For iterate As Integer = 0 To ai_iteration
                    'neighbor function
                    Dim temp As New clsPoint(_populations(0))
                    For i As Integer = 0 To temp.Count - 1
                        Dim tempNeighbor = Math.Abs(2.0 * NeighborRange) * MyBase.Random.NextDouble() - NeighborRange
                        temp(i) += tempNeighbor
                    Next
                    temp.ReEvaluate()

                    'transition
                    Dim evalNow As Double = _populations(0).Eval
                    Dim evalNew As Double = temp.Eval
                    Dim r1 As Double = 0.0
                    Dim r2 = MyBase.Random.NextDouble()
                    If evalNew < evalNow Then
                        r1 = 1.0
                    Else
                        Dim delta = evalNow - evalNew
                        r1 = Math.Exp(delta / Temperature)
                    End If
                    If r1 >= r2 Then
                        _populations(0) = temp 'exchange
                    End If

                    'cooling
                    If Me.Temperature > EndTemperature Then
                        Me.Temperature *= Me.CoolingRatio
                    End If

                    'reserve best
                    If _populations(0).Eval < Me._bestPoint.Eval Then
                        Me._bestPoint = _populations(0).Copy()
                    End If

                    'Check and Update Iteration count
                    If Iteration = _IterationCount Then
                        Return True
                    End If
                    _IterationCount += 1
                Next

                Return False
            Finally

            End Try
        End Function

        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Dim ret As New List(Of clsPoint)
                ret.Add(_bestPoint)
                Return ret
            End Get
        End Property

        Public Overrides ReadOnly Property BestResult As clsPoint
            Get
                Return _bestPoint
            End Get
        End Property

        Public Function DebugNowPoint() As clsPoint
            Return _populations(0)
        End Function

        Public Sub DebugSAParametes()
            Console.WriteLine("{0}", Temperature)
        End Sub
#End Region
    End Class
End Namespace
