Imports LibOptimization.Util

Namespace Optimization.DerivativeFree.ParticleSwarmOptmization
    ''' <summary>
    ''' Basic Particle Swarm Optmization
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Swarm Intelligence algorithm.
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]James Kennedy and Russell Eberhart, "Particle Swarm Optimization", Proceedings of IEEE the International Conference on Neural Networks，1995
    ''' [2]Y. Shi and Russell Eberhart, "A Modified Particle Swarm Optimizer", Proceedings of Congress on Evolu-tionary Computation, 79-73., 1998
    ''' [3]R. C. Eberhart and Y. Shi, "Comparing inertia weights and constriction factors in particle swarm optimization", In Proceedings of the Congress on Evolutionary Computation, vol. 1, pp. 84–88, IEEE, La Jolla, Calif, USA, July 2000.
    ''' </remarks>
    Public Class PSO : Inherits absOptimization
#Region "Member"
        ''' <summary>Swarm Size(Default:100)</summary>
        Public Property SwarmSize As Integer = 100

        ''' <summary>Inertia weight. Weigth=1.0(orignal paper 1995), Weight=0.729(Default setting)</summary>
        Public Property Weight As Double = 0.729

        ''' <summary>velocity coefficient(affected by personal best). C1 = C2 = 2.0 (orignal paper 1995), C1 = C2 = 1.49445(Default setting)</summary>
        Public Property C1 As Double = 1.49445

        ''' <summary>velocity coefficient(affected by global best). C1 = C2 = 2.0 (orignal paper 1995), C1 = C2 = 1.49445(Default setting)</summary>
        Public Property C2 As Double = 1.49445
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

            'update velocity and best positon
            For i As Integer = 0 To _populations.Count - 1
                'update velocity for each variable
                Dim v(ObjectiveFunction.NumberOfVariable - 1) As Double
                For j As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                    v(j) = Util.Util.GenRandomRange(Random, -Me.InitialValueRange, Me.InitialValueRange)
                Next
                _populations(i).temp1 = DirectCast(v, Object)
                _populations(i).temp2 = DirectCast(_populations(i).Copy(), Object)
            Next

            Return True
        End Function

        ''' <summary>
        ''' Do optimize
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'get global best
            Dim gBest As Point = Result()

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

                'PSO process
                Dim pBest As Point = Nothing
                For Each particle In _populations
                    'replace personal best
                    pBest = DirectCast(particle.temp2, Point)
                    If particle.Eval < pBest.Eval Then
                        particle.temp2 = DirectCast(particle.Copy(), Point)

                        'replace global best
                        If pBest.Eval < gBest.Eval Then
                            gBest = pBest.Copy()
                        End If
                    End If

                    'update a velocity 
                    Dim v = DirectCast(particle.temp1, Double())
                    For i As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                        Dim r1 = Random.NextDouble()
                        Dim r2 = Random.NextDouble()
                        pBest = DirectCast(particle.temp2, Point)
                        v(i) = Me.Weight * v(i) + C1 * r1 * (pBest(i) - particle(i)) + C2 * r2 * (gBest(i) - particle(i))

                        'update a position using velocity
                        particle(i) = particle(i) + v(i)
                    Next
                    particle.temp1 = DirectCast(v, Object)
                    particle.ReEvaluate()
                    'limit
                    LimitSolutionSpace(particle)
                Next
            Next

            Return (GetRemainingIterationCount(ai_iteration) = 0)
        End Function
#End Region

#Region "Private"
#End Region
    End Class
End Namespace
