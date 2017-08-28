Imports LibOptimization2.Util

Namespace Optimization.DerivativeFree.DifferentialEvolution
    ''' <summary>
    ''' Adaptive Differential Evolution Algorithm - JADE
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -similar to GA algorithm.
    ''' 
    ''' Refference:
    '''  [1]Zhang, Jingqiao, and Arthur C. Sanderson. "JADE: adaptive differential evolution with optional external archive." IEEE Transactions on evolutionary computation 13.5 (2009): 945-958.
    '''  [2]阪井節子, and 高濱徹行. "パラメータの相関を考慮した適応型差分進化アルゴリズム JADE の改良 (不確実性の下での数理モデルとその周辺)." (2015).
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class JADE : Inherits absOptimization
#Region "Member(for JADE Parameters)"
        ''' <summary>Constant raio 0 to 1(Adaptive paramter for muF, muCR)(Default:0.1)</summary>
        Public Property C As Double = 0.1

        ''' <summary>Adapative cross over ratio</summary>
        Private muCR As Double = 0.5

        ''' <summary>Adapative F</summary>
        Private muF As Double = 0.5
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize setting and value
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Init() As Boolean
            'initialize
            If MyBase.Init() = False Then
                Return False
            End If

            'init muF, muCR
            muCR = 0.5
            muF = 0.5

            Return True
        End Function

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'index array for parent selection
            Dim idxArray = Util.Util.CreateIndexArray(Me.PopulationSize)

            'Do Iterate
            Dim count = GetRemainingIterationCount(ai_iteration)
            For iterate As Integer = 0 To count - 1
                'Update Iteration count
                _IterationCount += 1

                'Sort Evaluate
                _populations.Sort()

                'check criterion
                If MyBase.IsCriterion() = True Then
                    Return True
                End If

                'Mutation and Crossover
                Dim sumF As Double = 0.0
                Dim sumFSquare As Double = 0.0
                Dim sumCR As Double = 0.0
                Dim countSuccess As Integer = 0
                For i As Integer = 0 To Me.PopulationSize - 1
                    'update F
                    Dim F As Double = 0.0
                    While True
                        F = Util.Util.CauchyRand(muF, 0.1)
                        If F < 0 Then
                            Continue While
                        End If
                        If F > 1 Then
                            F = 1.0
                        End If
                        Exit While
                    End While

                    'update CR 0 to 1
                    Dim CR As Double = Util.Util.NormRand(muCR, 0.1)
                    If CR < 0 Then
                        CR = 0.0
                    ElseIf CR > 1 Then
                        CR = 1.0
                    End If

                    'Sort Evaluate
                    _populations.Sort()

                    'pick different parent without i
                    'Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(_populations.Count, i)
                    'Dim xi = _populations(i)
                    'Dim p1 As clsPoint = _populations(randIndex(0))
                    'Dim p2 As clsPoint = _populations(randIndex(1))
                    Util.Util.ShuffleIndex(idxArray)
                    Dim pickIndex = Util.Util.PickIndex(idxArray, 3, i)
                    Dim xi As clsPoint = _populations(pickIndex(0))
                    Dim p1 As clsPoint = _populations(pickIndex(1))
                    Dim p2 As clsPoint = _populations(pickIndex(2))

                    'Mutation and Crossover
                    Dim child = New clsPoint(ObjectiveFunction)
                    Dim j = Random.Next() Mod ObjectiveFunction.NumberOfVariable
                    Dim D = ObjectiveFunction.NumberOfVariable - 1

                    'DE/current-to-pbest/1 for JADE Strategy. current 100p% p<-(0,1)
                    Dim p = CInt(Me.PopulationSize * Random.NextDouble()) 'range is 0 to PopulationSize
                    Dim pbest As clsPoint = Nothing
                    If p = 0 Then
                        pbest = _populations(0) 'best
                    ElseIf p >= Me.PopulationSize Then
                        pbest = _populations(PopulationSize - 1) 'worst
                    Else
                        pbest = _populations(Random.Next(0, p))
                    End If

                    'crossover
                    For k = 0 To ObjectiveFunction.NumberOfVariable - 1
                        If Random.NextDouble() < CR OrElse k = D Then
                            child(j) = xi(j) + F * (pbest(j) - xi(j)) + F * (p1(j) - p2(j))
                        Else
                            child(j) = xi(k)
                        End If
                        j = (j + 1) Mod ObjectiveFunction.NumberOfVariable 'next
                    Next
                    child.ReEvaluate() 'Evaluate child

                    'Limit solution space
                    LimitSolutionSpace(child)

                    'Survive
                    If child.Eval < _populations(i).Eval Then
                        'replace
                        _populations(i) = child

                        'for adaptive parameter
                        sumF += F
                        sumFSquare += F ^ 2
                        sumCR += CR
                        countSuccess += 1
                    End If
                Next 'population iteration

                'calc muF, muCR
                If countSuccess > 0 Then
                    muCR = (1 - C) * muCR + C * (sumCR / countSuccess) '(1-c) * muCR + c * meanA(CRs)
                    muF = (1 - C) * muF + C * (sumFSquare / sumF)
                Else
                    muCR = (1 - C) * muCR
                    muF = (1 - C) * muF
                    'Console.WriteLine("muF={0}, muCR={1}", muF, muCR)
                End If
            Next

            Return (GetRemainingIterationCount(ai_iteration) = 0)
        End Function

#End Region

#Region "Private"

#End Region
    End Class
End Namespace
