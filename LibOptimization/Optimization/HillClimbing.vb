Imports LibOptimization.Util

Namespace Optimization.DerivativeFree
    ''' <summary>
    ''' Hill-Climbing algorithm
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Randomized algorithm for optimization.
    ''' 
    ''' Reffrence:
    ''' https://en.wikipedia.org/wiki/Hill_climbing
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class HillClimbing : Inherits LibOptimization.Optimization.absOptimization
#Region "Member(Original parameter for Simulated Annealing)"
        ''' <summary>range of neighbor search</summary>
        Public Property NeighborRange As Double = 0.1

        ''' <summary>range of neighbor search</summary>
        Public Property NeighborSize As Integer = 50
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize setting and value
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Init(Optional ByVal anyPoint() As Double = Nothing, Optional ByVal isReuseBestResult As Boolean = False) As Boolean
            If MyBase.Init(anyPoint, isReuseBestResult) = False Then
                Return False
            End If

            'leave best result
            Dim best = _populations(0).Copy()
            _populations.Clear()
            _populations.Add(best)

            Return True
        End Function

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'Do Iteration
            Dim count = GetRemainingIterationCount(ai_iteration)
            For iterate As Integer = 0 To count - 1
                'Update Iteration count
                _IterationCount += 1

                'neighbor function
                Dim nextPoint = Neighbor(_populations(0))

                'limit solution space
                LimitSolutionSpace(nextPoint)

                'evaluate
                If _populations(0).Eval > nextPoint.Eval Then
                    _populations(0) = nextPoint
                End If
            Next

            'iteration count check
            If GetRemainingIterationCount(ai_iteration) = 0 Then
                Return True
            Else
                Return False
            End If
        End Function
#End Region

#Region "Private"
        ''' <summary>
        ''' Neighbor function for local search
        ''' </summary>
        ''' <param name="base"></param>
        ''' <returns></returns>
        Private Function Neighbor(ByVal base As Point) As Point
            Dim ret As New List(Of Point)
            For k As Integer = 0 To Me.NeighborSize - 1
                Dim temp As New Point(base)
                For i As Integer = 0 To temp.Count - 1
                    Dim tempNeighbor = Math.Abs(2.0 * NeighborRange) * MyBase.Random.NextDouble() - NeighborRange
                    temp(i) += tempNeighbor
                Next
                temp.ReEvaluate()
                ret.Add(temp)
            Next
            ret.Sort()

            Return ret(0)
        End Function
#End Region

    End Class
End Namespace
