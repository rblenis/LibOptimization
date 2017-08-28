Imports LibOptimization2.Util
Imports LibOptimization2.MathUtil

Namespace Optimization.DerivativeFree
    ''' <summary>
    ''' Nelder Mead Method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Also known as "Down hill simplex" or "simplex method".
    '''  -Implementation according to the original paper.
    ''' 
    ''' Reffrence:
    ''' J.A.Nelder and R.Mead, "A Simplex Method for Function Minimization", The Computer Journal vol.7, 308–313 (1965)
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class NelderMead : Inherits absOptimization
#Region "Member(for Nelder-Mead parameters)"
        'MEMO:Default parameter Iteration:5,000, EPS:0.000001

        ''' <summary>Refrection coeffcient(default:1.0)</summary>
        Public ReadOnly Refrection As Double = 1.0

        ''' <summary>Expantion coeffcient(default:2.0)</summary>
        Public ReadOnly Expantion As Double = 2.0

        ''' <summary>Contraction coeffcient(default:0.5)</summary>
        Public ReadOnly Contraction As Double = 0.5

        ''' <summary>Shrink coeffcient(default:2.0)</summary>
        Public ReadOnly Shrink As Double = 2.0
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Init() As Boolean
            'Check number of variable
            If ObjectiveFunction.NumberOfVariable < 2 Then
                ErrorManage.SetError(ErrorManage.ErrorType.ERR_INIT, "NumberOfVariable is 1")
                Return False
            End If

            MyBase.PopulationSize = ObjectiveFunction.NumberOfVariable + 1
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
                Dim centroid As clsPoint = Me.GetCentroid(_populations)

                '1st Refrection
                Dim refrection As clsPoint = Me.CalcRefrection(WorstPoint, centroid, Me.Refrection)

                'Simplex Operators - Refrection, Expantion, Constratction, (Shrink)
                If refrection.Eval < BestPoint.Eval Then
                    Dim expantion As clsPoint = Me.CalcExpantion(refrection, centroid, Me.Expantion) 'Fig. 1 Flow diagram is constratction??
                    If expantion.Eval < BestPoint.Eval Then
                        WorstPoint = expantion
                    Else
                        WorstPoint = refrection
                    End If
                Else
                    If refrection.Eval > Worst2ndPoint.Eval Then
                        If refrection.Eval > WorstPoint.Eval Then
                            'nop
                        Else
                            WorstPoint = refrection
                        End If
                        'Contraction
                        Dim contraction As clsPoint = Me.CalcContraction(WorstPoint, centroid, Me.Contraction)
                        If contraction.Eval > WorstPoint.Eval Then
                            WorstPoint = contraction
                        Else
                            'Shrink
                            Me.CalcShrink(Me.Shrink)
                        End If
                    Else
                        WorstPoint = refrection
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
        Private Function GetCentroid(ByVal ai_vertexs As List(Of clsPoint)) As clsPoint
            Dim ret As New clsPoint(ai_vertexs(0))

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
        ''' Refrection
        ''' </summary>
        ''' <param name="ai_tgt">Target vertex</param>
        ''' <param name="ai_base">Base vertex</param>
        ''' <param name="ai_coeff">Expantion coeffcient. Recommned value 1.0</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' xr = (1 + alpha)¯x - p
        ''' </remarks>
        Private Function CalcRefrection(ByVal ai_tgt As clsPoint, ByVal ai_base As clsPoint,
                                        Optional ByVal ai_coeff As Double = 1.0) As clsPoint
            Dim ret As New clsPoint(ObjectiveFunction)

            Dim numVar As Integer = ai_base.Count
            For i As Integer = 0 To numVar - 1
                Dim temp As Double = -ai_coeff * ai_tgt(i) + (1 + ai_coeff) * ai_base(i)
                ret(i) = temp
            Next
            ret.ReEvaluate()

            'limit result
            LimitSolutionSpace(ret)

            Return ret
        End Function

        ''' <summary>
        ''' Expantion
        ''' </summary>
        ''' <param name="ai_tgt">Target vertex</param>
        ''' <param name="ai_base">Base vertex</param>
        ''' <param name="ai_coeff">Expantion coeffcient. Recommned value 2.0</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' xe = gamma * p + (1 - gamma)¯x
        ''' </remarks>
        Private Function CalcExpantion(ByVal ai_tgt As clsPoint, ByVal ai_base As clsPoint,
                                       Optional ByVal ai_coeff As Double = 2.0) As clsPoint
            Dim ret As New clsPoint(ObjectiveFunction)

            Dim numVar As Integer = ai_base.Count
            For i As Integer = 0 To numVar - 1
                Dim temp As Double = ai_coeff * ai_tgt(i) + (1 - ai_coeff) * ai_base(i)
                ret(i) = temp
            Next
            ret.ReEvaluate()

            'limit result
            LimitSolutionSpace(ret)

            Return ret
        End Function

        ''' <summary>
        ''' Contraction
        ''' </summary>
        ''' <param name="ai_tgt">Target vertex</param>
        ''' <param name="ai_base">Base vertex</param>
        ''' <param name="ai_coeff">Constraction coeffcient. Recommned value 0.5</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' xc = beta * p + (1 - beta)¯x
        ''' </remarks>
        Private Function CalcContraction(ByVal ai_tgt As clsPoint, ByVal ai_base As clsPoint,
                                         Optional ByVal ai_coeff As Double = 0.5) As clsPoint
            Dim ret As New clsPoint(ObjectiveFunction)

            Dim numVar As Integer = ai_base.Count
            For i As Integer = 0 To numVar - 1
                Dim temp As Double = -ai_coeff * ai_tgt(i) + (1 + ai_coeff) * ai_base(i)
                ret(i) = temp
            Next
            ret.ReEvaluate()

            'limit result
            LimitSolutionSpace(ret)

            Return ret
        End Function

        ''' <summary>
        ''' Shrink(All point replace)
        ''' </summary>
        ''' <param name="ai_coeff">Shrink coeffcient.</param>
        ''' <remarks>
        ''' </remarks>
        Private Sub CalcShrink(Optional ByVal ai_coeff As Double = 2.0)
            Dim numVar As Integer = _populations(0).Count

            Dim tempBestPoint As New clsPoint(BestPoint)
            For i As Integer = 0 To _populations.Count - 1
                For j As Integer = 0 To numVar - 1
                    Dim temp As Double = (tempBestPoint(j) + _populations(i)(j)) / ai_coeff
                    _populations(i)(j) = temp
                Next
                _populations(i).ReEvaluate()

                'limit result
                LimitSolutionSpace(_populations(i))
            Next
        End Sub
#End Region

#Region "Property(Private)"
        Private Property BestPoint() As clsPoint
            Get
                Return _populations(0)
            End Get
            Set(ByVal value As clsPoint)
                _populations(0) = value
            End Set
        End Property

        Private Property WorstPoint() As clsPoint
            Get
                Return _populations(_populations.Count - 1)
            End Get
            Set(ByVal value As clsPoint)
                _populations(_populations.Count - 1) = value
            End Set
        End Property

        Private Property Worst2ndPoint() As clsPoint
            Get
                Return _populations(_populations.Count - 2)
            End Get
            Set(ByVal value As clsPoint)
                _populations(_populations.Count - 2) = value
            End Set
        End Property
#End Region
    End Class

End Namespace