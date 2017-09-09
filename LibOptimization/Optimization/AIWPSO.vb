﻿Imports LibOptimization.Util

Namespace Optimization.DerivativeFree.ParticleSwarmOptmization
    ''' <summary>
    ''' Particle Swarm Optmization using Adaptive Inertia Weight(AIW-PSO)
    ''' AdaptW
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Swarm Intelligence algorithm.
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]A. Nickabadi, M. M. Ebadzadeh, and R. Safabakhsh, “A novel particle swarm optimization algorithm with adaptive inertia weight,” Applied Soft Computing Journal, vol. 11, no. 4, pp. 3658–3670, 2011.
    ''' </remarks>
    Public Class AIWPSO : Inherits absOptimization
#Region "Member"
        ''' <summary>adaptive inertia weight(Default:1.0)</summary>
        Public Property Weight As Double = 1.0

        ''' <summary>Weight max for adaptive weight(Default:1.0).</summary>
        Public Property WeightMax As Double = 1.0

        ''' <summary>Weight min for adaptive weight(Default:0.0).</summary>
        Public Property WeightMin As Double = 0.0

        ''' <summary>velocity coefficient(affected by personal best)(Default:1.49445)</summary>
        Public Property C1 As Double = 1.49445

        ''' <summary>velocity coefficient(affected by global best)(Default:1.49445)</summary>
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
                    v(j) = Util.Util.GenRandomRange(Random, -InitialValueRange, InitialValueRange)
                Next
                _populations(i).temp1 = DirectCast(v, Object)
                _populations(i).temp2 = DirectCast(_populations(i).Copy(), Object)
            Next

            Weight = 1.0

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
                Dim replaceSuccessCount As Integer = 0
                Dim pBest As Point = Nothing
                For Each particle In _populations
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

                    'replace personal best
                    pBest = DirectCast(particle.temp2, Point)
                    If particle.Eval < pBest.Eval Then
                        particle.temp2 = DirectCast(particle.Copy(), Point)
                        replaceSuccessCount += 1 'for AIWPSO

                        'replace global best
                        If pBest.Eval < gBest.Eval Then
                            gBest = pBest.Copy()
                        End If
                    End If
                Next

                'Inertia Weight Strategie - AIW Adaptive Inertia Weight
                Dim PS = replaceSuccessCount / PopulationSize
                Me.Weight = (Me.WeightMax - Me.WeightMin) * PS - Me.WeightMin
            Next

            Return (GetRemainingIterationCount(ai_iteration) = 0)
        End Function
#End Region

#Region "Private"
#End Region
    End Class
End Namespace
