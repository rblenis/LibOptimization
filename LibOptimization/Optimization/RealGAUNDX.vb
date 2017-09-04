Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization.DerivativeFree.ReadlCodedGA
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' UNDX(Unimodal Normal Distribution Crossover)
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Alternation of generation algorithm is MGG
    ''' 
    ''' Refference:
    ''' [1]小野功，佐藤浩，小林重信, "単峰性正規分布交叉UNDXを用いた実数値GAによる関数最適化"，人工知能学会誌，Vol. 14，No. 6，pp. 1146-1155 (1999)
    ''' [2]北野 宏明 (編集), 遺伝的アルゴリズム 4, 産業図書出版株式会社, 2000年 初版, p261
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class RealGAUNDX : Inherits absOptimization
#Region "Member(Coefficient of GA)"
        ''' <summary>Children size(Default:100, When Adaptation is True, PopulationSize * 0.75)</summary>
        Public Property ChildrenSize As Integer = 100

        ''' <summary>Alpha(Default:0.5)</summary>
        Public Property ALPHA As Double = 0.5

        ''' <summary>Beta(Default:0.35)</summary>
        Public Property BETA As Double = 0.35
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

                'select parent
                Dim randIndex As List(Of Integer) = Util.Util.RandomPermutaion(MyBase._populations.Count)
                Dim p1Index As Integer = randIndex(0)
                Dim p2Index As Integer = randIndex(1)
                Dim p1 = MyBase._populations(p1Index)
                Dim p2 = MyBase._populations(p2Index)
                Dim p3 = MyBase._populations(randIndex(2)) 'for d2

                'cross over UNDX
                Dim children = UNDX(p1, p2, p3)

                'AlternationStrategy MGG
                children.Add(p1)
                children.Add(p2)
                children.Sort()
                MyBase._populations(p1Index) = children(0) 'replace elite
                children.RemoveAt(0)
                Dim rIndex = SelectRoulette(children, True)
                MyBase._populations(p2Index) = children(rIndex) 'replace

                'Check and Update Iteration count
                If Iteration = _IterationCount Then
                    Return True
                End If
                _IterationCount += 1
            Next

            Return False
        End Function

        ''' <summary>
        ''' RouletteWheel Selection 
        ''' </summary>
        ''' <param name="ai_chidren"></param>
        ''' <param name="isForMinimize"></param>
        ''' <returns>index</returns>
        ''' <remarks></remarks>
        Private Function SelectRoulette(ByVal ai_chidren As List(Of Point), ByVal isForMinimize As Boolean) As Integer
            If isForMinimize = True Then
                Dim tempSum As Double = 0.0
                For Each c In ai_chidren
                    tempSum += Math.Abs(c.Eval)
                Next
                Dim tempList As New List(Of Double)(ai_chidren.Count)
                Dim newTempSum As Double = 0.0
                For i As Integer = 0 To ai_chidren.Count - 1
                    Dim temp = tempSum - ai_chidren(i).Eval
                    tempList.Add(temp)
                    newTempSum += temp
                Next
                'select
                Dim r = Random.NextDouble()
                Dim cumulativeRatio As Double = 0.0
                For i As Integer = 0 To ai_chidren.Count - 1
                    cumulativeRatio += tempList(i) / newTempSum
                    If cumulativeRatio > r Then
                        Return i
                    End If
                Next
            Else
                Dim tempSum As Double = 0.0
                For Each c In ai_chidren
                    tempSum += c.Eval
                Next
                'select
                Dim r = Random.NextDouble()
                Dim cumulativeRatio As Double = 0.0
                For i As Integer = 0 To ai_chidren.Count - 1
                    cumulativeRatio += ai_chidren(i).Eval / tempSum
                    If cumulativeRatio > r Then
                        Return i
                    End If
                Next
            End If

            Return 0
        End Function
#End Region

#Region "Private"
        ''' <summary>
        ''' Calc Triangle Area using Heron formula
        ''' </summary>
        ''' <param name="lengthA"></param>
        ''' <param name="lengthB"></param>
        ''' <param name="lengthC"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalcTriangleArea(ByVal lengthA As Double, ByVal lengthB As Double, ByVal lengthC As Double) As Double
            '    p3
            '   /| 
            '  / |
            ' ----
            ' p1  p2
            Dim s = (lengthA + lengthB + lengthC) / 2.0
            Dim area = Math.Sqrt(s * (s - lengthA) * (s - lengthB) * (s - lengthC))
            Return area
        End Function

        ''' <summary>
        ''' UNDX CrossOver
        ''' </summary>
        ''' <param name="p1"></param>
        ''' <param name="p2"></param>
        ''' <param name="p3"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function UNDX(ByVal p1 As Point, ByVal p2 As Point, ByVal p3 As Point) As List(Of Point)
            'calc d
            Dim diffVectorP2P1 = p1 - p2
            Dim length = diffVectorP2P1.NormL2()
            Dim areaTriangle As Double = CalcTriangleArea(length, (p3 - p2).NormL2(), (p3 - p1).NormL2())
            Dim d2 = 2.0 * areaTriangle / length 'S=1/2 * h * a -> h = 2.0 * S / a

            'UNDX
            Dim children As New List(Of Point)(ChildrenSize)
            Dim g = (p1 + p2) / 2.0
            Dim sd1 = (ALPHA * length) ^ 2
            Dim sd2 = (BETA * d2 / Math.Sqrt(ObjectiveFunction.NumberOfVariable)) ^ 2
            Dim e = diffVectorP2P1 / length
            Dim t = New EasyVector(ObjectiveFunction.NumberOfVariable)
            For genChild As Integer = 0 To CInt(ChildrenSize / 2 - 1)
                For i As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                    t(i) = Util.Util.NormRand(0, sd2)
                Next
                t = t - (t.InnerProduct(e)) * e

                'child
                Dim child1(ObjectiveFunction.NumberOfVariable - 1) As Double
                Dim child2(ObjectiveFunction.NumberOfVariable - 1) As Double
                Dim ndRand = Util.Util.NormRand(0, sd1)
                For i As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                    Dim temp = t(i) + ndRand * e(i)
                    child1(i) = g(i) + temp
                    child2(i) = g(i) - temp
                Next

                'overflow check
                Dim temp1 = New Point(ObjectiveFunction, child1)
                If Util.Util.CheckOverflow(temp1) = True Then
                    For i As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                        'temp1(i) = Util.Util.NormRand(g(i), 0.1)
                        temp1(i) = Util.Util.GenRandomRange(Random, -InitialValueRange, InitialValueRange)
                    Next
                End If
                Dim temp2 = New Point(ObjectiveFunction, child2)
                If Util.Util.CheckOverflow(temp2) = True Then
                    For i As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                        temp2(i) = Util.Util.GenRandomRange(Random, -InitialValueRange, InitialValueRange)
                    Next
                End If

                'limit solution space
                LimitSolutionSpace(temp1)
                LimitSolutionSpace(temp2)

                children.Add(temp1)
                children.Add(temp2)
            Next
            Return children
        End Function
#End Region
    End Class
End Namespace
