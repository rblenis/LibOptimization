Imports LibOptimization2.Util

Namespace Optimization.DerivativeFree
    ''' <summary>
    ''' Differential Evolution Algorithm
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -similar to GA algorithm.
    ''' 
    ''' Memo:
    '''  Notation of DE
    '''   DE/x/y/z
    '''    x: pick parent strategy(rand or best)
    '''    y: number of difference vector
    '''    z: crossover scheme
    '''       ex.Binomial -> bin
    ''' 
    ''' Refference:
    ''' [1]Storn, R., Price, K., "Differential Evolution – A Simple and Efficient Heuristic for Global Optimization over Continuous Spaces", Journal of Global Optimization 11: 341–359.
    ''' [2]Price, K. and Storn, R., "Minimizing the Real Functions of the ICEC’96 contest by Differential Evolution", IEEE International Conference on Evolutionary Computation (ICEC’96), may 1996, pp. 842–844.
    ''' [3]Sk. Minhazul Islam, Swagatam Das, "An Adaptive Differential Evolution Algorithm With Novel Mutation and Crossover Strategies for Global Numerical Optimization", IEEE TRANSACTIONS ON SYSTEMS, MAN, AND CYBERNETICS—PART B: CYBERNETICS, VOL. 42, NO. 2, APRIL 2012, pp482-500.
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class DifferentialEvolution : Inherits absOptimization
#Region "Member(DE parameters)"
        ''' <summary>Differential weight(Scaling factor)(Default:0.5)</summary>
        Public Property F As Double = 0.5

        ''' <summary>Differential weight(Scaling factor)(Default:0.5) for CurrentToBest, randToBest</summary>
        Public Property Fdash As Double = 0.5

        ''' <summary>Cross over ratio(Default:0.9)</summary>
        Public Property CrossOverRatio As Double = 0.9

        ''' <summary>Evolution Strategy</summary>
        Public Property DEStrategy As EnumDEStrategyType = EnumDEStrategyType.DE_rand_1_bin

        ''' <summary>Enum Differential Evolution Strategy</summary>
        Public Enum EnumDEStrategyType
            ''' <summary>DE/rand/1/bin 強い大域検索</summary>
            DE_rand_1_bin
            ''' <summary>DE/rand/2/bin 強い大域検索</summary>
            DE_rand_2_bin
            ''' <summary>DE/best/1/bin 強い局所検索</summary>
            DE_best_1_bin
            ''' <summary>DE/best/2/bin 強い局所検索</summary>
            DE_best_2_bin
            ''' <summary>DE/current/1/bin 弱い大域検索</summary>
            DE_current_1_bin
            ''' <summary>DE/currentToBest/1/bin 弱い局所検索</summary>
            DE_current_to_Best_1_bin
            ''' <summary>DE/randToBest/1/bin 大域・局所検索</summary>
            DE_rand_to_Best_1_bin
        End Enum
#End Region

#Region "Public"
        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Sort Evaluate
                MyBase._populations.Sort()

                'check criterion
                If MyBase.UseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(EPS, MyBase._populations(0).Eval, MyBase._populations(MyBase._criterionIndex).Eval) Then
                        Return True
                    End If
                End If

                'reserve best value
                Dim best = _populations(0).Copy()

                'DE
                For i As Integer = 0 To PopulationSize - 1
                    'pick different parent without i
                    Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(_populations.Count, i)
                    Dim xi = _populations(i)
                    Dim p1 As clsPoint = _populations(randIndex(0))
                    Dim p2 As clsPoint = _populations(randIndex(1))
                    Dim p3 As clsPoint = _populations(randIndex(2))
                    Dim p4 As clsPoint = _populations(randIndex(3))
                    Dim p5 As clsPoint = _populations(randIndex(4))

                    'Mutation and Crossover
                    Dim child = New clsPoint(ObjectiveFunction)
                    Dim j = Random.Next() Mod ObjectiveFunction.NumberOfVariable
                    Dim D = ObjectiveFunction.NumberOfVariable - 1
                    If Me.DEStrategy = EnumDEStrategyType.DE_rand_1_bin Then
                        'DE/rand/1/bin
                        For k = 0 To ObjectiveFunction.NumberOfVariable - 1
                            If Random.NextDouble() < CrossOverRatio OrElse k = D Then
                                child(j) = p1(j) + F * (p2(j) - p3(j))
                            Else
                                child(j) = xi(k)
                            End If
                            j = (j + 1) Mod ObjectiveFunction.NumberOfVariable 'next
                        Next
                    ElseIf Me.DEStrategy = EnumDEStrategyType.DE_rand_2_bin Then
                        'DE/rand/2/bin
                        For k = 0 To ObjectiveFunction.NumberOfVariable - 1
                            If Random.NextDouble() < CrossOverRatio OrElse k = D Then
                                child(j) = p1(j) + F * (p2(j) + p3(j) - p4(j) - p5(j))
                            Else
                                child(j) = xi(k)
                            End If
                            j = (j + 1) Mod ObjectiveFunction.NumberOfVariable 'next
                        Next
                    ElseIf Me.DEStrategy = EnumDEStrategyType.DE_best_1_bin Then
                        'DE/best/1/bin
                        For k = 0 To ObjectiveFunction.NumberOfVariable - 1
                            If Random.NextDouble() < CrossOverRatio OrElse k = D Then
                                child(j) = best(j) + F * (p1(j) - p2(j))
                            Else
                                child(j) = xi(k)
                            End If
                            j = (j + 1) Mod ObjectiveFunction.NumberOfVariable 'next
                        Next
                    ElseIf Me.DEStrategy = EnumDEStrategyType.DE_best_2_bin Then
                        'DE/best/2/bin
                        For k = 0 To ObjectiveFunction.NumberOfVariable - 1
                            If Random.NextDouble() < CrossOverRatio OrElse k = D Then
                                child(j) = best(j) + F * (p1(j) + p2(j) - p3(j) - p4(j))
                            Else
                                child(j) = xi(k)
                            End If
                            j = (j + 1) Mod ObjectiveFunction.NumberOfVariable 'next
                        Next
                    ElseIf Me.DEStrategy = EnumDEStrategyType.DE_current_1_bin Then
                        'DE/current-to(target-to)/1/bin
                        For k = 0 To ObjectiveFunction.NumberOfVariable - 1
                            If Random.NextDouble() < CrossOverRatio OrElse k = D Then
                                child(j) = xi(j) + F * (p2(j) - p3(j))
                            Else
                                child(j) = xi(k)
                            End If
                            j = (j + 1) Mod ObjectiveFunction.NumberOfVariable 'next
                        Next
                    ElseIf Me.DEStrategy = EnumDEStrategyType.DE_current_to_Best_1_bin Then
                        'DE/current-to-best/1/bin
                        For k = 0 To ObjectiveFunction.NumberOfVariable - 1
                            If Random.NextDouble() < CrossOverRatio OrElse k = D Then
                                child(j) = xi(j) + Fdash * (best(j) - p1(j)) + F * (p2(j) - p3(j))
                            Else
                                child(j) = xi(k)
                            End If
                            j = (j + 1) Mod ObjectiveFunction.NumberOfVariable 'next
                        Next
                    ElseIf Me.DEStrategy = EnumDEStrategyType.DE_rand_to_Best_1_bin Then
                        'DE/rand-to-best/1/bin
                        For k = 0 To ObjectiveFunction.NumberOfVariable - 1
                            If Random.NextDouble() < CrossOverRatio OrElse k = D Then
                                child(j) = p1(j) + Fdash * (best(j) - p1(j)) + F * (p2(j) - p3(j))
                            Else
                                child(j) = xi(k)
                            End If
                            j = (j + 1) Mod ObjectiveFunction.NumberOfVariable 'next
                        Next
                    End If
                    child.ReEvaluate() 'Evaluate child

                    'Limit solution space
                    LimitSolutionSpace(child)

                    'Survive
                    If child.Eval < _populations(i).Eval Then
                        _populations(i) = child
                    End If

                    'Current best
                    If child.Eval < best.Eval Then
                        best = child
                    End If
                Next 'end population for

                'Check and Update Iteration count
                If Iteration = _IterationCount Then
                    Return True
                End If
                _IterationCount += 1
            Next

            Return False
        End Function
#End Region
    End Class
End Namespace
