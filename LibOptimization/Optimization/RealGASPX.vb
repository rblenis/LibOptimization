Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization.DerivativeFree.ReadlCodedGA
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' SPX(Simplex Crossover) + JGG
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Cross over algorithm is SPX(Simplex Crossover).
    '''  -Alternation of generation algorithm is JGG.
    ''' 
    ''' Refference:
    ''' 樋口 隆英, 筒井 茂義, 山村 雅幸, "実数値GAにおけるシンプレクス交叉", 人工知能学会論文誌Vol. 16 (2001) No. 1 pp.147-155
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class RealGASPX : Inherits absOptimization
#Region "Member(Coefficient of GA)"
        ''' <summary>Children size(Default:100, When Adaptation is True, PopulationSize * 0.75)</summary>
        Public Property ChildrenSize As Integer = 100
#End Region

#Region "Public"
        ''' <summary>
        ''' Init optimizer
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Init(Optional ByVal anyPoint() As Double = Nothing, Optional ByVal isReuseBestResult As Boolean = False) As Boolean
            'Init
            Dim flg = MyBase.Init()

            'When Adaptation flag is True, ChildrenSize is also to set adaptation
            If UseAdaptivePopulationSize = True Then
                ChildrenSize = CInt(PopulationSize * 0.75)
            End If

            Return flg
        End Function

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Sort Evaluate
                MyBase._populations.Sort()

                'check criterion
                If MyBase.UseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If Util.Util.IsCriterion(EPS, MyBase._populations(0).Eval, MyBase._populations(MyBase._criterionIndex).Eval) Then
                        Return True
                    End If
                End If

                'SPX with JGG
                'Parent is n+1
                Dim parents As List(Of KeyValuePair(Of Integer, Point)) = Util.Util.SelectParent(MyBase._populations, ObjectiveFunction.NumberOfVariable + 1)

                'Crossover
                Dim children As List(Of Point) = CrossOverSPX(ChildrenSize, parents)

                'Replace
                Dim index As Integer = 0
                For Each p As KeyValuePair(Of Integer, Point) In parents
                    MyBase._populations(p.Key) = children(index)
                    index += 1
                Next

                'Check and Update Iteration count
                If Iteration = _IterationCount Then
                    Return True
                End If
                _IterationCount += 1
            Next

            Return False
        End Function

        ''' <summary>
        ''' Simplex Crossover
        ''' </summary>
        ''' <param name="ai_childSize"></param>
        ''' <param name="ai_parents"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CrossOverSPX(ByVal ai_childSize As Integer,
                                      ByVal ai_parents As List(Of KeyValuePair(Of Integer, Point))) As List(Of Point)
            'Calc Centroid
            Dim xg As New EasyVector(ObjectiveFunction.NumberOfVariable)
            For Each p As KeyValuePair(Of Integer, Point) In ai_parents
                xg += p.Value
            Next
            xg /= ai_parents.Count 'sum(xi)/(n+k)

            'SPX
            Dim retChilds As New List(Of Point)
            Dim alpha As Double = Math.Sqrt(ObjectiveFunction.NumberOfVariable + 2) 'expantion rate
            For i As Integer = 0 To ai_childSize - 1
                Dim cVector As New List(Of EasyVector)
                Dim pVector As New List(Of EasyVector)
                Dim k As Integer = 0
                For Each xi As KeyValuePair(Of Integer, Point) In ai_parents
                    pVector.Add(xg + alpha * (xi.Value - xg))

                    If k = 0 Then
                        cVector.Add(New EasyVector(ObjectiveFunction.NumberOfVariable)) 'all zero
                    Else
                        Dim rk As Double = MyBase.Random.NextDouble() ^ (1 / k)
                        Dim pos = rk * (pVector(k - 1) - pVector(k) + cVector(k - 1))
                        cVector.Add(pos)
                    End If
                    k += 1
                Next
                Dim tempChild = New Point(ObjectiveFunction, pVector(pVector.Count - 1) + cVector(cVector.Count - 1))

                'limit solution space
                LimitSolutionSpace(tempChild)

                retChilds.Add(tempChild)
            Next
            retChilds.Sort()

            Return retChilds
        End Function
#End Region
    End Class
End Namespace
