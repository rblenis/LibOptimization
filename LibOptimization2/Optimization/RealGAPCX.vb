Imports LibOptimization2.Util
Imports LibOptimization2.MathUtil

Namespace Optimization.DerivativeFree.ReadlCodedGA
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' Parent Centric Recombination(PCX)
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Cross over algorithm is PCX.
    '''  -Alternation of generation algorithm is G3.
    ''' 
    ''' Refference:
    ''' [1]Kalyanmoy Deb, Dhiraj Joshi and Ashish Anand, "Real-Coded Evolutionary Algorithms with Parent-Centric Recombination", KanGAL Report No. 2001003
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class RealGAPCX : Inherits absOptimization
#Region "Member(Coefficient of GA)"
        ''' <summary>Children size(Default:3)</summary>
        Public Property ChildrenSize As Integer = 3

        ''' <summary>Randomize parameter Eta(Default:0.1)</summary>
        Public Property Eta As Double = 0.1

        ''' <summary>Randomize parameter Zeta(Default:0.1)</summary>
        Public Property Zeta As Double = 0.1
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
                    If clsUtil.IsCriterion(EPS, MyBase._populations(0).Eval, MyBase._populations(MyBase._criterionIndex).Eval) Then
                        Return True
                    End If
                End If

                'Parent Centric Recombination with G3(Genelized Generation Gap)
                Dim selectParentsIndex As List(Of Integer) = Nothing
                Dim selectParents As List(Of clsPoint) = Nothing
                SelectParentsForG3(3, selectParentsIndex, selectParents) 'include best result

                'check length 0
                Dim flag = clsUtil.IsExistZeroLength(selectParents)

                'Crossover
                If flag = False Then
                    'PCX Crossover
                    Dim newPopulation As List(Of clsPoint) = CrossoverPCX(selectParents, ChildrenSize, 3)

                    'Replace(by G3)
                    Dim replaceParent As Integer = 2
                    Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(selectParentsIndex.Count)
                    For i As Integer = 0 To replaceParent - 1
                        Dim parentIndex As Integer = selectParentsIndex(randIndex(i))
                        newPopulation.Add(MyBase._populations(parentIndex))
                    Next
                    'sort by eval
                    newPopulation.Sort()

                    'replace
                    For i As Integer = 0 To replaceParent - 1
                        Dim parentIndex As Integer = selectParentsIndex(randIndex(i))
                        MyBase._populations(parentIndex) = newPopulation(i)
                    Next
                End If

                'Check and Update Iteration count
                If Iteration = _IterationCount Then
                    Return True
                End If
                _IterationCount += 1
            Next

            Return False
        End Function

#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Select parent for G3(Genelized Generation Gap)
        ''' </summary>
        ''' <param name="ai_pickN"></param>
        ''' <param name="ao_parentIndex"></param>
        ''' <param name="ao_retParents"></param>
        ''' <remarks></remarks>
        Private Sub SelectParentsForG3(ByVal ai_pickN As Integer, ByRef ao_parentIndex As List(Of Integer), ByRef ao_retParents As List(Of clsPoint))
            'generate random permutation array without best parent index
            Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(MyBase._populations.Count, 0)

            'generate random permutation with best parent index
            ao_parentIndex = New List(Of Integer)(ai_pickN)
            ao_retParents = New List(Of clsPoint)(ai_pickN)
            Dim insertBestParentPosition As Integer = Random.Next(0, ai_pickN)
            For i As Integer = 0 To ai_pickN - 1
                If i = insertBestParentPosition Then
                    ao_parentIndex.Add(0)
                    ao_retParents.Add(MyBase._populations(0))
                Else
                    ao_parentIndex.Add(randIndex(i))
                    ao_retParents.Add(MyBase._populations(randIndex(i)))
                End If
            Next
        End Sub

        ''' <summary>
        ''' Crossover PCX
        ''' </summary>
        ''' <param name="ai_parents"></param>
        ''' <param name="ai_childrenSize"></param>
        ''' <param name="ai_pickParentNo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CrossoverPCX(ByVal ai_parents As List(Of clsPoint), ByVal ai_childrenSize As Integer, ByVal ai_pickParentNo As Integer) As List(Of clsPoint)
            Dim retPopulation As New List(Of clsPoint)

            'PCX
            For pNo As Integer = 0 To ai_childrenSize - 1
                Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(ai_parents.Count)
                Dim parentsPoint As New List(Of clsPoint)(ai_pickParentNo)
                For i As Integer = 0 To ai_pickParentNo - 1
                    parentsPoint.Add(ai_parents(randIndex(i)))
                Next

                'calc g
                Dim g As New clsEasyVector(ObjectiveFunction.NumberOfVariable)
                For i As Integer = 0 To ai_pickParentNo - 1
                    g += parentsPoint(i)
                Next
                g /= ai_pickParentNo

                'calc D
                Dim d As clsEasyVector = g - parentsPoint(0)
                Dim dist As Double = d.NormL2()
                'If dist < EPS Then
                '    Console.WriteLine("very near! g {0}", dist)
                'End If

                Dim diff As New List(Of clsEasyVector)
                For i As Integer = 1 To ai_pickParentNo - 1
                    diff.Add(New clsEasyVector(ObjectiveFunction.NumberOfVariable))
                    diff(i - 1) = parentsPoint(i) - parentsPoint(0)
                    'If diff(i - 1).NormL2 < EPS Then
                    '    Console.WriteLine("very near! {0}", diff(i - 1).NormL2)
                    'End If
                Next

                'orthogonal directions -> Vector D
                Dim DD As New clsEasyVector(ObjectiveFunction.NumberOfVariable)
                For i As Integer = 0 To ai_pickParentNo - 2
                    Dim temp1 As Double = diff(i).InnerProduct(d)
                    Dim temp2 As Double = temp1 / (diff(i).NormL2 * dist)
                    Dim temp3 = 1.0 - Math.Pow(temp2, 2.0)
                    Dim temp4 = Math.Sqrt(temp3)
                    'overflow check. temp3 may be very small number.
                    If Double.IsNaN(temp4) = True Then
                        DD(i) = 0
                    Else
                        DD(i) = diff(i).NormL2 * temp4
                    End If
                Next

                'Average vector D
                Dim meanD As Double = DD.Average()
                Dim tempV1 As New clsEasyVector(ObjectiveFunction.NumberOfVariable)
                For i As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                    tempV1(i) = clsUtil.NormRand(0.0, meanD * Eta) 'original
                Next
                Dim tempInnerP As Double = tempV1.InnerProduct(d)
                Dim tempNRand As Double = clsUtil.NormRand(0.0, Zeta)
                For i As Integer = 0 To ObjectiveFunction.NumberOfVariable - 1
                    tempV1(i) = tempV1(i) - tempInnerP * DD(i) / Math.Pow(dist, 2.0)
                    tempV1(i) += tempNRand * d(i)
                Next

                'add population
                Dim tempChild As New clsPoint(ObjectiveFunction, parentsPoint(0) + tempV1)

                'limit solution space
                LimitSolutionSpace(tempChild)

                retPopulation.Add(tempChild)
            Next

            Return retPopulation
        End Function
#End Region
    End Class
End Namespace
