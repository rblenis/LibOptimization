Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization.DerivativeFree
    ''' <summary>
    ''' Nelder Mead Method wikipedia ver
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Also known as "Down hill simplex" or "simplex method".
    ''' 
    ''' Reffrence:
    ''' http://ja.wikipedia.org/wiki/Nelder-Mead%E6%B3%95
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class NelderMeadWiki : Inherits absOptimization
#Region "Member(for Nelder-Mead parameters)"
        ''' <summary>Refrection coeffcient(default:1.0)</summary>
        Public ReadOnly Refrection As Double = 1.0

        ''' <summary>Expantion coeffcient(default:2.0)</summary>
        Public ReadOnly Expantion As Double = 2.0

        ''' <summary>Contraction coeffcient(default:-0.5)</summary>
        Public ReadOnly Contraction As Double = -0.5

        ''' <summary>Shrink coeffcient(default:0.5)</summary>
        Public ReadOnly Shrink As Double = 0.5
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Init(Optional ByVal anyPoint() As Double = Nothing, Optional ByVal isReuseBestResult As Boolean = False) As Boolean
            'Check number of variable
            If ObjectiveFunction.NumberOfVariable < 2 Then
                ErrorManage.ErrorManage.SetError(ErrorManage.ErrorManage.ErrorType.ERR_INIT, "NumberOfVariable is 1")
                Return False
            End If

            MyBase.PopulationSize = ObjectiveFunction.NumberOfVariable + 1
            MyBase.UseAdaptivePopulationSize = False
            MyBase.UseCriterion = False

            If MyBase.Init(anyPoint, isReuseBestResult) = False Then
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
            'Do Iteration
            Dim count = GetRemainingIterationCount(ai_iteration)
            For iterate As Integer = 0 To count - 1
                'Update Iteration count
                _IterationCount += 1

                'Check criterion
                _populations.Sort()
                If Util.Util.IsCriterion(Me.EPS, _populations(0).Eval, _populations(_populations.Count - 1).Eval) Then
                    Return True
                End If

                'Calc centroid
                Dim centroid = Me.GetCentroid(_populations)

                'Reflection
                Dim refrection = Me.ModifySimplex(Me.WorstPoint, centroid, Me.Refrection)
                If BestPoint.Eval <= refrection.Eval AndAlso refrection.Eval < Worst2ndPoint.Eval Then
                    WorstPoint = refrection
                ElseIf refrection.Eval < BestPoint.Eval Then
                    'Expantion
                    Dim expantion = Me.ModifySimplex(Me.WorstPoint, centroid, Me.Expantion)
                    If expantion.Eval < refrection.Eval Then
                        WorstPoint = expantion
                    Else
                        WorstPoint = refrection
                    End If
                Else
                    'Contraction
                    Dim contraction = Me.ModifySimplex(WorstPoint, centroid, Me.Contraction)
                    If contraction.Eval < WorstPoint.Eval Then
                        WorstPoint = contraction
                    Else
                        'Reduction(Shrink) BestPoint以外を縮小
                        Me.CalcShrink(Me.Shrink)
                    End If
                End If
            Next

            If GetRemainingIterationCount(ai_iteration) = 0 Then
                Return True 'stop iteration
            Else
                Return False
            End If
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Calc Centroid
        ''' </summary>
        ''' <param name="ai_vertexs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetCentroid(ByVal ai_vertexs As List(Of Point)) As Point
            Dim ret As New Point(ai_vertexs(0))

            Dim numVar As Integer = ai_vertexs(0).Count
            For i As Integer = 0 To numVar - 1
                Dim temp As Double = 0.0
                For numVertex As Integer = 0 To ai_vertexs.Count - 2 'Except Worst
                    temp += ai_vertexs(numVertex)(i)
                Next
                ret(i) = temp / (ai_vertexs.Count - 1)
            Next
            ret.ReEvaluate()

            'limit result
            LimitSolutionSpace(ret)

            Return ret
        End Function

        ''' <summary>
        ''' Simplex
        ''' </summary>
        ''' <param name="ai_tgt">Target vertex</param>
        ''' <param name="ai_base">Base vertex</param>
        ''' <param name="ai_coeff">Coeffcient</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Function ModifySimplex(ByVal ai_tgt As Point, ByVal ai_base As Point, ByVal ai_coeff As Double) As Point
            Dim ret As New Point(ObjectiveFunction)
            For i As Integer = 0 To ret.Count - 1
                Dim temp As Double = ai_base(i) + ai_coeff * (ai_base(i) - ai_tgt(i))
                ret(i) = temp
            Next
            ret.ReEvaluate()

            'limit result
            LimitSolutionSpace(ret)

            Return ret
        End Function

        ''' <summary>
        ''' Shrink(Except best point)
        ''' </summary>
        ''' <param name="ai_coeff">Shrink coeffcient</param>
        ''' <remarks>
        ''' </remarks>
        Private Sub CalcShrink(ByVal ai_coeff As Double)
            For i As Integer = 1 To _populations.Count - 1 'expect BestPoint
                For j As Integer = 0 To _populations(0).Count - 1
                    Dim temp = BestPoint(j) + ai_coeff * (_populations(i)(j) - BestPoint(j))
                    _populations(i)(j) = temp
                Next

                'limit result
                LimitSolutionSpace(_populations(i))

                _populations(i).ReEvaluate()
            Next
        End Sub
#End Region

#Region "Property(Private)"
        Private Property BestPoint() As Point
            Get
                Return _populations(0)
            End Get
            Set(ByVal value As Point)
                _populations(0) = value
            End Set
        End Property

        Private Property WorstPoint() As Point
            Get
                Return _populations(_populations.Count - 1)
            End Get
            Set(ByVal value As Point)
                _populations(_populations.Count - 1) = value
            End Set
        End Property

        Private Property Worst2ndPoint() As Point
            Get
                Return _populations(_populations.Count - 2)
            End Get
            Set(ByVal value As Point)
                _populations(_populations.Count - 2) = value
            End Set
        End Property
#End Region
    End Class

End Namespace