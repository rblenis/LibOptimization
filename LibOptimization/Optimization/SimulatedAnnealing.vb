Imports LibOptimization.Util

Namespace Optimization.DerivativeFree
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
    Public Class SimulatedAnnealing : Inherits LibOptimization.Optimization.absOptimization
#Region "Member(Original parameter for Simulated Annealing)"
        ''' <summary>cooling ratio</summary>
        Public Property CoolingRatio As Double = 0.99

        ''' <summary>range of neighbor search</summary>
        Public Property NeighborRange As Double = 0.1

        ''' <summary>start temperature</summary>
        Public Property Temperature As Double = 1000.0

        ''' <summary>stop temperature</summary>
        Public Property StopTemperature As Double = 0.00000001
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

            Return True
        End Function

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'Do Iteration
            Dim _bestPoint = _populations(0).Copy()
            Try
                Dim count = GetRemainingIterationCount(ai_iteration)
                For iterate As Integer = 0 To count - 1
                    'Update Iteration count
                    _IterationCount += 1

                    'neighbor function
                    Dim temp As Point = Neighbor(_populations(0))

                    'limit solution space
                    LimitSolutionSpace(temp)

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
                    Temperature *= CoolingRatio
                    If Temperature < StopTemperature Then
                        Return True 'stop iteration
                    End If

                    'reserve best
                    If _populations(0).Eval < _bestPoint.Eval Then
                        _bestPoint = _populations(0).Copy()
                    End If
                Next

                If GetRemainingIterationCount(ai_iteration) = 0 Then
                    Return True 'stop iteration
                Else
                    Return False
                End If
            Finally
                'return member
                _populations(0) = _bestPoint
            End Try
        End Function
#End Region

#Region "Private"
        ''' <summary>
        ''' Neighbor function for local search
        ''' </summary>
        ''' <param name="base"></param>
        ''' <returns></returns>
        Private Function Neighbor(ByVal base As Point) As Point
            Dim temp As New Point(base)
            For i As Integer = 0 To temp.Count - 1
                Dim tempNeighbor = Math.Abs(2.0 * NeighborRange) * MyBase.Random.NextDouble() - NeighborRange
                temp(i) += tempNeighbor
            Next
            temp.ReEvaluate()

            Return temp
        End Function
#End Region

    End Class
End Namespace
