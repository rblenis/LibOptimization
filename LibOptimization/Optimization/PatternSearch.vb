Imports LibOptimization.Util

Namespace Optimization.DerivativeFree
    ''' <summary>
    ''' Hooke-Jeeves Pattern Search Method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Reffrence:
    ''' Hooke, R. and Jeeves, T.A., ""Direct search" solution of numerical and statistical problems", Journal of the Association for Computing Machinery (ACM) 8 (2), pp212–229.
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class PatternSearch : Inherits absOptimization
#Region "Member(Coefficient of pattern search)"
        ''' <summary>step length(Default:0.6)</summary>
        Private ReadOnly StepLength As Double = 0.6

        ''' <summary>shrink parameter(Default:2.0)</summary>
        Private ReadOnly Shrink As Double = 2.0

        ''' <summary>current step length</summary>
        Private m_stepLength As Double = 0.6
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize setting and value
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Init(Optional ByVal anyPoint() As Double = Nothing, Optional ByVal isReuseBestResult As Boolean = False) As Boolean
            MyBase.PopulationSize = 1
            MyBase.UseAdaptivePopulationSize = False

            If MyBase.Init() = False Then
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Iteration - 1, ai_iteration - 1)

            Dim current = New Point(_populations(0))
            Try
                For iterate As Integer = 0 To ai_iteration
                    'MakeExploratoryMoves
                    Dim exp As Point = Me.MakeExploratoryMoves(current, Me.m_stepLength)

                    If exp.Eval < current.Eval Then
                        'Replace basepoint
                        Dim previousBasePoint As Point = current
                        current = exp

                        'MakePatternMove and MakeExploratoryMoves
                        Dim temp As Point = Me.MakePatternMove(previousBasePoint, current)
                        Dim expUsingPatternMove = Me.MakeExploratoryMoves(temp, Me.m_stepLength)
                        If expUsingPatternMove.Eval < current.Eval Then
                            current = expUsingPatternMove
                        End If

                        'limit
                        LimitSolutionSpace(current)
                    Else
                        'Check conversion
                        If Me.m_stepLength < EPS Then
                            Return True
                        End If

                        'Shrink Step
                        Me.m_stepLength /= Me.Shrink
                    End If

                    'Check and Update Iteration count
                    If Iteration = _IterationCount Then
                        Return True
                    End If
                    _IterationCount += 1
                Next

                Return False
            Finally
                'return
                _populations(0) = current
            End Try
        End Function

        ''' <summary>
        ''' Exploratory Move
        ''' </summary>
        ''' <param name="ai_base">Base point</param>
        ''' <param name="ai_stepLength">Step</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MakeExploratoryMoves(ByVal ai_base As Point, ByVal ai_stepLength As Double) As Point
            Dim explorePoint As New List(Of Point)
            For i As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                Dim tempPlus As New Point(ai_base)
                tempPlus(i) += ai_stepLength
                tempPlus.ReEvaluate()
                explorePoint.Add(tempPlus)

                Dim tempMinus As New Point(ai_base)
                tempMinus(i) -= ai_stepLength
                tempMinus.ReEvaluate()
                explorePoint.Add(tempMinus)
            Next
            explorePoint.Sort()

            If explorePoint(0).Eval < ai_base.Eval Then
                Return explorePoint(0)
            Else
                Return New Point(ai_base)
            End If
        End Function

        ''' <summary>
        ''' Pattern Move
        ''' </summary>
        ''' <param name="ai_previousBasePoint"></param>
        ''' <param name="ai_base"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function MakePatternMove(ByVal ai_previousBasePoint As Point, ByVal ai_base As Point) As Point
            Dim ret As New Point(ai_base)
            For i As Integer = 0 To ai_base.Count - 1
                ret(i) = 2.0 * ai_base(i) - ai_previousBasePoint(i)
            Next
            ret.ReEvaluate()

            Return ret
        End Function
#End Region
    End Class
End Namespace
