Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization.DerivativeFree.ReadlCodedGA
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' REX(Real-coded Ensemble Crossover) + JGG
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Cross over algorithm is REX(Real-coded Ensemble Cross over).
    '''  -Alternation of generation algorithm is JGG.
    ''' 
    ''' Refference:
    ''' 小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class RealGAREX : Inherits absOptimization
#Region "Member(Coefficient of GA)"
        ''' <summary>Parent size for cross over(Default:n+1)</summary>
        Public Property ParentSize As Integer = 100 'REX(phi, n+k) -> n+1<n+k<PopultionSize, phi is random mode. n is number of variable.

        ''' <summary>Children Size(Default:100)</summary>
        Public Property ChildrenSize As Integer = 100

        ''' <summary>REX randomo mode(Default:UNIFORM)</summary>
        Public Property RandomMode As RexRandomMode = RexRandomMode.UNIFORM

        Public Enum RexRandomMode
            UNIFORM
            NORMAL_DIST
        End Enum
#End Region

#Region "Public"
        ''' <summary>
        ''' Init optimizer
        ''' </summary>
        ''' <param name="anyParentsize">parent size(0 is default. n+1)</param>
        ''' <returns></returns>
        Public Overloads Function Init(Optional ByVal anyParentsize As Integer = 0) As Boolean
            'set user parentsize
            If anyParentsize <= 0 Then
                Me.ParentSize = ObjectiveFunction.NumberOfVariable + 1
            Else
                Me.ParentSize = anyParentsize
            End If

            If Me.ParentSize >= PopulationSize Then
                Me.ParentSize = PopulationSize - 1
            End If

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

                'REX with JGG
                Dim parents As List(Of KeyValuePair(Of Integer, Point)) = Util.Util.SelectParent(MyBase._populations, ParentSize)

                'Crossover
                Dim children As List(Of Point) = CrossOverREX(RandomMode, ChildrenSize, parents)

                'Replace
                Dim index As Integer = 0
                For Each p As KeyValuePair(Of Integer, Point) In parents
                    MyBase._populations(p.Key) = children(index) 'replace
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
        ''' REX(Real-coded Ensemble Crossover)
        ''' </summary>
        ''' <param name="ai_randomMode"></param>
        ''' <param name="ai_childNum">ChildNum</param>
        ''' <param name="ai_parents"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' REX(U, n+k) -> U is UniformRandom
        ''' REX(N, n+k) -> N is NormalDistribution
        ''' "n+k" is parents size.
        ''' </remarks>
        Private Function CrossOverREX(ByVal ai_randomMode As RexRandomMode, ByVal ai_childNum As Integer, ByVal ai_parents As List(Of KeyValuePair(Of Integer, Point))) As List(Of Point)
            'Calc Centroid
            Dim xg As New EasyVector(ObjectiveFunction.NumberOfVariable)
            For Each p As KeyValuePair(Of Integer, Point) In ai_parents
                xg += p.Value
            Next
            xg /= ai_parents.Count 'sum(xi)/(n+k)

            'cross over
            Dim retChilds As New List(Of Point)
            Dim uniformRandParam As Double = Math.Sqrt(3 / ai_parents.Count)
            Dim normalDistParam As Double = 1 / ai_parents.Count '???
            For i As Integer = 0 To ai_childNum
                'cross over
                Dim childV As New EasyVector(ObjectiveFunction.NumberOfVariable)
                'sum( rand * (xi-xg) )
                For Each xi As KeyValuePair(Of Integer, Point) In ai_parents
                    'rand parameter
                    Dim randVal As Double = 0.0
                    If ai_randomMode = RexRandomMode.NORMAL_DIST Then
                        randVal = Util.Util.NormRand(0, normalDistParam)
                    Else
                        randVal = Math.Abs(2.0 * uniformRandParam) * MyBase.Random.NextDouble() - MyBase.InitialValueRange
                    End If
                    'rand * (xi-xg)
                    childV += randVal * (xi.Value - xg)
                Next
                'xg + sum( rand * (xi-xg) )
                childV += xg

                'convert clsPoint
                Dim child As New Point(ObjectiveFunction, childV)

                'limit solution space
                LimitSolutionSpace(child)

                retChilds.Add(child)
            Next
            retChilds.Sort()

            Return retChilds
        End Function
#End Region
    End Class
End Namespace
