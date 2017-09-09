Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization.DerivativeFree
    ''' <summary>
    ''' Firefly Algorithm
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -for Mulitimodal optimization
    ''' 
    ''' Refference:
    ''' [1]X. S. Yang, “Firefly algorithms for multimodal optimization,” in Proceedings of the 5th International Conference on Stochastic Algorithms: Foundation and Applications (SAGA '09), vol. 5792 of Lecture notes in Computer Science, pp. 169–178, 2009.
    ''' [2]Firefly Algorithm - http://www.mathworks.com/matlabcentral/fileexchange/29693-firefly-algorithm
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class Firefly : Inherits absOptimization
#Region "Member"
        ''' <summary>attractiveness base</summary>
        Public Property Beta0 As Double = 1.0

        ''' <summary>light absorption coefficient(Default:1.0)</summary>
        Public Property Gamma As Double = 1.0

        ''' <summary>randomization parameter</summary>
        Public Property Alpha As Double = 0.2
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

            'update intensity
            For i As Integer = 0 To _populations.Count - 1
                _populations(i).temp1 = DirectCast(1.0 / (_populations(i).Eval + 0.0001), Object)
            Next

            Return True
        End Function

        ''' <summary>
        ''' Do Iteration
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
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

                'FireFly - original
                For i As Integer = 0 To Me.PopulationSize - 1
                    For j As Integer = 0 To Me.PopulationSize - 1
                        'Move firefly
                        Dim intensity1 = DirectCast(_populations(i).temp1, Double)
                        Dim intensity2 = DirectCast(_populations(j).temp1, Double)
                        If intensity1 < intensity2 Then
                            Dim r As Double = (_populations(i) - _populations(j)).NormL1()
                            Dim beta As Double = Me.Beta0 * Math.Exp(-Me.Gamma * r * r) 'attractiveness
                            For k As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                                Dim newPos As Double = _populations(i)(k)
                                newPos += beta * (_populations(j)(k) - _populations(i)(k)) 'attraction
                                newPos += Me.Alpha * (Random.NextDouble() - 0.5) 'random search
                                _populations(i)(k) = newPos
                            Next k
                            'limit solution space
                            LimitSolutionSpace(_populations(i))

                            'update intensity
                            _populations(i).temp1 = DirectCast(1.0 / (_populations(i).Eval + 0.0001), Object)
                        End If
                    Next
                Next
            Next

            Return (GetRemainingIterationCount(ai_iteration) = 0)
        End Function
#End Region

    End Class
End Namespace
