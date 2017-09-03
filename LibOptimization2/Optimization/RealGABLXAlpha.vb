Imports LibOptimization.Util

Namespace Optimization.DerivativeFree.ReadlCodedGA
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' BLX-Alpha + JGG
    ''' </summary>
    ''' <remarks>
    ''' Features:
    ''' -Derivative free optimization algorithm.
    ''' -Alternation of generation algorithm is JGG.
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class RealGABLXAlpha : Inherits absOptimization
#Region "Member(Coefficient of GA)"
        ''' <summary>Children Size(Default:100)</summary>
        Public Property ChildrenSize As Integer = 100

        ''' <summary>Alpha is expantion ratio(Default:0.5)</summary>
        Public Property Alpha As Double = 0.5
#End Region

#Region "Public"
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

                'BLX-alpha cross-over with JGG
                'Pick parent
                Dim randIndex As List(Of Integer) = Util.Util.RandomPermutaion(MyBase._populations.Count)
                Dim p1Index As Integer = randIndex(0)
                Dim p2Index As Integer = randIndex(1)
                Dim p1 = MyBase._populations(p1Index)
                Dim p2 = MyBase._populations(p2Index)

                'cross over
                Dim children As New List(Of clsPoint)(ChildrenSize)
                For numChild As Integer = 0 To ChildrenSize - 1
                    children.Add(New clsPoint(ObjectiveFunction))
                    For i As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                        Dim range As Double = Math.Abs(p1(i) - p2(i))
                        Dim min As Double = 0
                        Dim max As Double = 0
                        If p1(i) > p2(i) Then
                            min = p2(i)
                            max = p1(i)
                        Else
                            min = p1(i)
                            max = p2(i)
                        End If
                        children(numChild)(i) = Util.Util.GenRandomRange(MyBase.Random, min - Alpha * range, max + Alpha * range)
                    Next
                    children(numChild).ReEvaluate()
                Next

                'replace(JGG)
                children.Sort()
                MyBase._populations(p1Index) = children(0) 'first best
                MyBase._populations(p2Index) = children(1) 'second best

                'limit solution space
                LimitSolutionSpace(MyBase._populations(p1Index))
                LimitSolutionSpace(MyBase._populations(p2Index))

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