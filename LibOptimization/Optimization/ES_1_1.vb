Imports LibOptimization.Util

Namespace Optimization.DerivativeFree.EvolutionStrategy
    ''' <summary>
    ''' Evolution Strategy (1+1)-ES without Criterion
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]進化戦略 https://ja.wikipedia.org/wiki/%E9%80%B2%E5%8C%96%E6%88%A6%E7%95%A5
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class ES_1_1 : Inherits LibOptimization.Optimization.absOptimization
#Region "Member(Original parameter for Simulated Annealing)"
        ''' <summary>range of neighbor search</summary>
        Public Property NeighborRange As Double = 0.1

        ''' <summary>range of neighbor search</summary>
        Public Property NeighborSize As Integer = 50

        ''' <summary>update ratio C(Schwefel 0.85)</summary>
        Public Property C As Double = 0.85

        ''' <summary>recent result for Criterion</summary>
        Private _recentResult As Double = 0.0

        ''' <summary>variance</summary>
        Private _variance As Double = 0.0

        ''' <summary>recent Mutate success history for 1/5 rule</summary>
        Private _successMutate As New Queue(Of Integer)
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize setting and value
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Init(Optional ByVal anyPoint() As Double = Nothing, Optional ByVal isReuseBestResult As Boolean = False) As Boolean
            MyBase.PopulationSize = 1
            MyBase.UseAdaptivePopulationSize = False
            MyBase.UseCriterion = False

            If MyBase.Init(anyPoint, isReuseBestResult) = False Then
                Return False
            End If

            'specify ES
            _successMutate.Clear()
            For i As Integer = 0 To (Me.ObjectiveFunction.NumberOfVariable * 10) - 1
                _successMutate.Enqueue(0)
            Next
            'init variance
            _variance = Util.Util.GenRandomRange(MyBase.Random, 0.1, 5)

            Return True
        End Function

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'for 1/5 rule
            Dim n = MyBase.ObjectiveFunction.NumberOfVariable * 10.0

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'check criterion
                If MyBase.UseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If Util.Util.IsCriterion(EPS, MyBase._populations(0).Eval, MyBase._populations(MyBase._criterionIndex).Eval) Then
                        Return True
                    End If
                End If

                '-------------------------------------------------
                'ES
                '-------------------------------------------------
                'mutate
                Dim child = _populations(0).Copy()
                For i As Integer = 0 To _populations(0).Count - 1
                    Dim temp = Util.Util.NormRand(MyBase.Random, 0, _variance)
                    child(i) = _populations(0)(i) + temp
                Next
                child.ReEvaluate()

                'check best
                If child.Eval < _populations(0).Eval Then
                    _recentResult = _populations(0).Eval
                    _populations(0) = child
                    _successMutate.Enqueue(1)
                Else
                    _successMutate.Enqueue(0)
                End If
                _successMutate.Dequeue()

                '1/5 rule
                Dim successCount = _successMutate.Sum()
                Dim updateSuccessRatio = successCount / n
                If updateSuccessRatio < 0.2 Then
                    _variance = _variance * C
                Else
                    _variance = _variance / C
                End If

                'Check and Update Iteration count
                If Iteration = _IterationCount Then
                    Return True
                End If
                _IterationCount += 1
            Next

            Return False
        End Function
#End Region

#Region "Private"

#End Region

    End Class
End Namespace
