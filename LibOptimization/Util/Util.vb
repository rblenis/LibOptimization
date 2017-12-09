Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.Util.Random

Namespace Util
    ''' <summary>
    ''' common use
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Util
        Private Shared _callCount As UInt32 = 0
        Private Shared lock As New Object

        ''' <summary>
        ''' for random seed
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GlobalCounter() As UInt32
            While (True)
                SyncLock (lock)
                    Util._callCount = Util._callCount + CType(1, UInt32)
                    Return Util._callCount
                End SyncLock
                System.Threading.Thread.Sleep(50)
            End While

            Return Util._callCount '警告抑止のため
        End Function

        ''' <summary>
        ''' reset random seed
        ''' </summary>
        Public Shared Sub ResetGlobalCounter()
            _callCount = 0
        End Sub

        ''' <summary>
        ''' Normal Distribution
        ''' </summary>
        ''' <param name="oRand">random object</param>
        ''' <param name="ai_ave">Average</param>
        ''' <param name="ai_sigma2">Varianse s^2</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' using Box-Muller method
        ''' </remarks>
        Public Shared Function NormRand(ByVal oRand As System.Random,
                                        Optional ByVal ai_ave As Double = 0,
                                        Optional ByVal ai_sigma2 As Double = 1) As Double
            Dim x As Double = oRand.NextDouble()
            Dim y As Double = oRand.NextDouble()

            Dim c As Double = Math.Sqrt(-2.0 * Math.Log(x))
            If (0.5 - XorshiftSingleton.GetInstance().NextDouble() > 0.0) Then
                Return c * Math.Sin(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            Else
                Return c * Math.Cos(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            End If
        End Function

        ''' <summary>
        ''' Normal Distribution
        ''' </summary>
        ''' <param name="ai_ave">Average</param>
        ''' <param name="ai_sigma2">Varianse s^2</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' using Box-Muller method
        ''' </remarks>
        Public Shared Function NormRand(Optional ByVal ai_ave As Double = 0,
                                        Optional ByVal ai_sigma2 As Double = 1) As Double
            Dim x As Double = XorshiftSingleton.GetInstance().NextDouble()
            Dim y As Double = XorshiftSingleton.GetInstance().NextDouble()

            Dim c As Double = Math.Sqrt(-2.0 * Math.Log(x))
            If (0.5 - XorshiftSingleton.GetInstance().NextDouble() > 0.0) Then
                Return c * Math.Sin(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            Else
                Return c * Math.Cos(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            End If
        End Function

        ''' <summary>
        ''' Cauchy Distribution
        ''' </summary>
        ''' <param name="ai_mu">default:0</param>
        ''' <param name="ai_gamma">default:1</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' http://www.sat.t.u-tokyo.ac.jp/~omi/random_variables_generation.html#Cauchy
        ''' </remarks>
        Public Shared Function CauchyRand(Optional ByVal ai_mu As Double = 0, Optional ByVal ai_gamma As Double = 1) As Double
            Return ai_mu + ai_gamma * Math.Tan(Math.PI * (XorshiftSingleton.GetInstance().NextDouble() - 0.5))
        End Function

        ''' <summary>
        ''' Generate Random permutation
        ''' </summary>
        ''' <param name="ai_max">0 to ai_max-1</param>
        ''' <param name="ai_removeIndex">RemoveIndex</param>
        ''' <returns></returns>
        Public Shared Function RandomPermutaion(ByVal ai_max As Integer, Optional ByVal ai_removeIndex As Integer = -1) As List(Of Integer)
            Return RandomPermutaion(0, ai_max, ai_removeIndex)
        End Function

        ''' <summary>
        ''' Generate Random permutation with range (ai_min to ai_max-1)
        ''' </summary>
        ''' <param name="ai_min">start value</param>
        ''' <param name="ai_max">ai_max-1</param>
        ''' <param name="ai_removeIndex">RemoveIndex -1 is invalid</param>
        ''' <returns></returns>
        Public Shared Function RandomPermutaion(ByVal ai_min As Integer, ByVal ai_max As Integer, ByVal ai_removeIndex As Integer) As List(Of Integer)
            Dim nLength As Integer = ai_max - ai_min
            If nLength = 0 OrElse nLength < 0 Then
                Return New List(Of Integer)
            End If

            Dim ary As New List(Of Integer)(nLength)
            If ai_removeIndex <= -1 Then
                For ii As Integer = ai_min To ai_max - 1
                    ary.Add(ii)
                Next
            Else
                For ii As Integer = ai_min To ai_max - 1
                    If ai_removeIndex <> ii Then
                        ary.Add(ii)
                    End If
                Next
            End If

            'Fisher–Yates shuffle / フィッシャー - イェーツのシャッフル
            Dim n As Integer = ary.Count
            While n > 1
                n -= 1
                Dim k As Integer = XorshiftSingleton.GetInstance().Next(0, n + 1)
                Dim tmp As Integer = ary(k)
                ary(k) = ary(n)
                ary(n) = tmp
            End While
            Return ary
        End Function

        ''' <summary>
        ''' Pick index
        ''' </summary>
        ''' <param name="idxArray"></param>
        ''' <param name="pickNumber"></param>
        ''' <param name="withoutIndex"></param>
        ''' <returns></returns>
        Public Shared Function PickIndex(ByVal idxArray As List(Of Integer), ByVal pickNumber As Integer, ByVal withoutIndex As Integer) As List(Of Integer)
            Dim ret As New List(Of Integer)
            Dim count As Integer = 0

            For Each idx In idxArray
                If idx = withoutIndex Then
                    Continue For
                End If
                ret.Add(idx)
                If count = pickNumber Then
                    Exit For
                End If
                count += 1
            Next

            Return ret
        End Function

        ''' <summary>
        ''' Create index array
        ''' </summary>
        ''' <param name="ai_max">0 to ai_max-1</param>
        ''' <returns></returns>
        Public Shared Function CreateIndexArray(ByVal ai_max As Integer) As List(Of Integer)
            Dim indexArray As New List(Of Integer)(ai_max)
            For i As Integer = 0 To ai_max - 1
                indexArray.Add(i)
            Next
            Return indexArray
        End Function

        ''' <summary>
        ''' Shuffle index array
        ''' </summary>
        ''' <param name="ary"></param>
        Public Shared Sub ShuffleIndex(ByRef ary As List(Of Integer))
            'Fisher–Yates shuffle / フィッシャー - イェーツのシャッフル
            Dim n As Integer = ary.Count
            While n > 1
                n -= 1
                Dim k As Integer = XorshiftSingleton.GetInstance().Next(0, n + 1)
                Dim tmp As Integer = ary(k)
                ary(k) = ary(n)
                ary(n) = tmp
            End While
        End Sub

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="ai_opt"></param>
        ''' <param name="ai_precision"></param>
        ''' <param name="ai_isOutValue"></param>
        ''' <param name="ai_isOnlyIterationCount"></param>
        ''' <remarks></remarks>
        Public Shared Sub DebugValue(ByVal ai_opt As absOptimization,
                                     Optional ai_precision As Integer = 0,
                                     Optional ai_isOutValue As Boolean = True,
                                     Optional ai_isOnlyIterationCount As Boolean = False)
            If ai_opt Is Nothing Then
                Return
            End If

            If ai_isOnlyIterationCount = True Then
                Console.WriteLine("IterationCount:," & String.Format("{0}", ai_opt.IterationCount()))
                Return
            End If

            If ai_isOutValue = True Then
                Console.WriteLine("TargetFunction:" & ai_opt.ObjectiveFunction().GetType().Name & " Dimension:" & ai_opt.ObjectiveFunction().NumberOfVariable.ToString())
                Console.WriteLine("OptimizeMethod:" & ai_opt.GetType().Name)
                Console.WriteLine("Eval          :" & String.Format("{0}", ai_opt.Result.Eval))
                Console.WriteLine("IterationCount:" & String.Format("{0}", ai_opt.IterationCount()))
                Console.WriteLine("Result        :")
                Dim str As New System.Text.StringBuilder()
                For Each value As Double In ai_opt.Result
                    If ai_precision <= 0 Then
                        str.Append(value.ToString())
                    Else
                        str.Append(value.ToString("F3"))
                    End If
                    str.AppendLine("")
                Next
                Console.WriteLine(str.ToString())
            Else
                Console.WriteLine("Eval          :" & String.Format("{0}", ai_opt.Result.Eval))
            End If
        End Sub

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="ai_results"></param>
        ''' <remarks></remarks>
        Public Shared Sub DebugValue(ByVal ai_results As List(Of Point))
            If ai_results Is Nothing OrElse ai_results.Count = 0 Then
                Return
            End If
            For i As Integer = 0 To ai_results.Count - 1
                Console.WriteLine("Eval          :" & String.Format("{0}", ai_results(i).Eval))
            Next
            Console.WriteLine()
        End Sub

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="bestResult"></param>
        Public Shared Sub DebugValue(ByVal bestResult As Point)
            Console.WriteLine("Eval          :" & String.Format("{0}", bestResult.Eval))
        End Sub

        ''' <summary>
        ''' Check Criterion
        ''' </summary>
        ''' <param name="ai_eps">EPS</param>
        ''' <param name="ai_comparisonA"></param>
        ''' <param name="ai_comparisonB"></param>
        ''' <param name="ai_tiny"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsCriterion(ByVal ai_eps As Double,
                                           ByVal ai_comparisonA As Point, ByVal ai_comparisonB As Point,
                                           Optional ByVal ai_tiny As Double = 0.0000000000001) As Boolean
            Return Util.IsCriterion(ai_eps, ai_comparisonA.Eval, ai_comparisonB.Eval, ai_tiny)
        End Function

        ''' <summary>
        ''' Check Criterion
        ''' </summary>
        ''' <param name="ai_eps">EPS</param>
        ''' <param name="ai_comparisonA"></param>
        ''' <param name="ai_comparisonB"></param>
        ''' <param name="ai_tiny"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Reffrence:
        ''' William H. Press, Saul A. Teukolsky, William T. Vetterling, Brian P. Flannery,
        ''' "NUMRICAL RECIPIES 3rd Edition: The Art of Scientific Computing", Cambridge University Press 2007, pp505-506
        ''' </remarks>
        Public Shared Function IsCriterion(ByVal ai_eps As Double,
                                           ByVal ai_comparisonA As Double, ByVal ai_comparisonB As Double,
                                           Optional ByVal ai_tiny As Double = 0.0000000000001) As Boolean
            'check division by zero
            Dim denominator = (Math.Abs(ai_comparisonB) + Math.Abs(ai_comparisonA)) + ai_tiny
            If denominator = 0 Then
                Return True
            End If

            'check criterion
            Dim temp = 2.0 * Math.Abs(ai_comparisonB - ai_comparisonA) / denominator
            If temp < ai_eps Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Random generator helper
        ''' </summary>
        ''' <param name="oRand"></param>
        ''' <param name="ai_min"></param>
        ''' <param name="ai_max"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GenRandomRange(ByVal oRand As System.Random, ByVal ai_min As Double, ByVal ai_max As Double) As Double
            Return Math.Abs(ai_max - ai_min) * oRand.NextDouble() + ai_min
        End Function

        ''' <summary>
        ''' to csv
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <remarks></remarks>
        Public Shared Sub ToCSV(ByVal arP As Point)
            For Each p In arP
                Console.Write("{0},", p)
            Next
            Console.WriteLine("")
        End Sub

        ''' <summary>
        ''' to csv
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <remarks></remarks>
        Public Shared Sub ToCSV(ByVal arP As List(Of Point))
            For Each p In arP
                Util.ToCSV(p)
            Next
            Console.WriteLine("")
        End Sub

        ''' <summary>
        ''' eval output for debug
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <remarks></remarks>
        Public Shared Sub ToEvalList(ByVal arP As List(Of Point))
            For Each p In arP
                Console.WriteLine("{0}", p.Eval)
            Next
        End Sub

        ''' <summary>
        ''' Eval list
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSortedEvalList(ByVal arP As List(Of Point)) As List(Of Eval)
            Dim sortedEvalList = New List(Of Eval)
            For i = 0 To arP.Count - 1
                sortedEvalList.Add(New Eval(i, arP(i).Eval))
            Next
            sortedEvalList.Sort()
            Return sortedEvalList
        End Function

        ''' <summary>
        ''' Best clsPoint
        ''' </summary>
        ''' <param name="ai_points"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetBestPoint(ByVal ai_points As List(Of Point), Optional ByVal isCopy As Boolean = False) As Point
            If ai_points Is Nothing Then
                Return Nothing
            ElseIf ai_points.Count = 0 Then
                Return Nothing
            ElseIf ai_points.Count = 1 Then
                Return ai_points(0)
            End If

            Dim best = ai_points(0)
            For i As Integer = 1 To ai_points.Count - 1
                If best.Eval > ai_points(i).Eval Then
                    best = ai_points(i)
                End If
            Next

            If isCopy = False Then
                Return best
            Else
                Return best.Copy()
            End If
        End Function

        ''' <summary>
        ''' Get sorted population list by evaluate
        ''' </summary>
        ''' <param name="ai_points"></param>
        ''' <returns></returns>
        Public Shared Function GetSortedResultsByEval(ByVal ai_points As List(Of Point)) As List(Of Point)
            If ai_points Is Nothing Then
                Return Nothing
            ElseIf ai_points.Count = 0 Then
                Return Nothing
            ElseIf ai_points.Count = 1 Then
                Dim temp As New List(Of Point)
                temp.Add(ai_points(0))
                Return temp
            End If

            Dim sortedPoints As New List(Of Point)
            Dim sortedEvalList = Util.GetSortedEvalList(ai_points)
            For i As Integer = 0 To sortedEvalList.Count - 1
                sortedPoints.Add(ai_points(sortedEvalList(i).Index))
            Next
            Return sortedPoints
        End Function

        ''' <summary>
        ''' Select Parent
        ''' </summary>
        ''' <param name="ai_population"></param>
        ''' <param name="ai_parentSize"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SelectParent(ByVal ai_population As List(Of Point), ByVal ai_parentSize As Integer) As List(Of KeyValuePair(Of Integer, Point))
            Dim ret As New List(Of KeyValuePair(Of Integer, Point))

            'Index
            Dim randIndex As List(Of Integer) = Util.RandomPermutaion(ai_population.Count)

            'PickParents
            For i As Integer = 0 To ai_parentSize - 1
                ret.Add(New KeyValuePair(Of Integer, Point)(randIndex(i), ai_population(randIndex(i))))
            Next

            Return ret
        End Function

        ''' <summary>
        ''' calc length from each points
        ''' </summary>
        ''' <param name="points"></param>
        ''' <returns></returns>
        Public Shared Function IsExistZeroLength(ByVal points As List(Of Point)) As Boolean
            Dim isCanCrossover As Boolean = True
            Dim vec As EasyVector = Nothing
            For i As Integer = 0 To points.Count - 2
                vec = points(i) - points(i + 1)
                If vec.NormL1() = 0 Then
                    Return True
                End If
            Next
            vec = points(points.Count - 1) - points(0)
            If vec.NormL1() = 0 Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' Overflow check for debug
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Public Shared Function CheckOverflow(ByVal v As Double) As Boolean
            If Double.IsInfinity(v) = True Then
                Return True
            End If
            If Double.IsNaN(v) = True Then
                Return True
            End If
            If Double.IsNegativeInfinity(v) = True Then
                Return True
            End If
            If Double.IsPositiveInfinity(v) = True Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' Overflow check for debug
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Public Shared Function CheckOverflow(ByVal p As Point) As Boolean
            For Each v In p
                If Double.IsInfinity(v) = True Then
                    Return True
                End If
                If Double.IsNaN(v) = True Then
                    Return True
                End If
                If Double.IsNegativeInfinity(v) = True Then
                    Return True
                End If
                If Double.IsPositiveInfinity(v) = True Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Overflow check for debug
        ''' </summary>
        ''' <param name="listP"></param>
        ''' <returns></returns>
        Public Shared Function CheckOverflow(ByVal listP As List(Of Point)) As Boolean
            For Each temp In listP
                For Each v In temp
                    If Double.IsInfinity(v) = True Then
                        Return True
                    End If
                    If Double.IsNaN(v) = True Then
                        Return True
                    End If
                    If Double.IsNegativeInfinity(v) = True Then
                        Return True
                    End If
                    If Double.IsPositiveInfinity(v) = True Then
                        Return True
                    End If
                Next
            Next

            Return False
        End Function

        ''' <summary>
        ''' Set initial point
        ''' </summary>
        ''' <param name="population"></param>
        ''' <param name="initialPosition"></param>
        ''' <param name="density"></param>
        Public Shared Sub SetInitialPoint(ByVal population As List(Of Point), ByVal initialPosition() As Double, ByVal density As Double)
            If population IsNot Nothing AndAlso population.Count > 0 Then
                Dim func = population(0).GetFunc()
                If initialPosition IsNot Nothing AndAlso initialPosition.Length = func.NumberOfVariable Then
                    Dim index As Integer = CInt(population.Count * density)
                    If index < 1 Then
                        index = 1
                    End If
                    If index >= CInt(population.Count * 0.9) Then
                        index = CInt(population.Count * 0.8)
                    End If
                    For i As Integer = 0 To index - 1
                        For j As Integer = 0 To func.NumberOfVariable - 1
                            population(i)(j) = initialPosition(j)
                        Next
                        population(i).ReEvaluate()
                    Next
                End If
            End If
        End Sub
    End Class
End Namespace