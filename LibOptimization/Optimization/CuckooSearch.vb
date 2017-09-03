Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization.DerivativeFree
    ''' <summary>
    ''' Standard Cuckoo Search
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]Xin-She Yang, Suash Deb, "Cuckoo search via Lévy flights.", World Congress on Nature and Biologically Inspired Computing (NaBIC 2009). IEEE Publications. pp. 210–214. arXiv:1003.1594v1.
    ''' [2]Cuckoo Search (CS) Algorithm
    '''    http://www.mathworks.com/matlabcentral/fileexchange/29809-cuckoo-search--cs--algorithm
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class CuckooSearch : Inherits absOptimization
#Region "Member"
        'Population Size(Default:25)

        ''' <summary>Discovery rate(Default:0.25)</summary>
        Public Property PA As Double = 0.25

        ''' <summary>Levy flight parameter(Default:1.5)</summary>
        Public Property Beta As Double = 1.5
#End Region

#Region "Public"
        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'levy flight parameter
            Dim sigma = (Me.Gamma(1 + Beta) * Math.Sin(Math.PI * Beta / 2) / (Me.Gamma((1 + Beta) / 2) * Beta * 2 ^ ((Beta - 1) / 2))) ^ (1 / Beta)

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

                'get a cuckoo
                Dim currentBest = _populations(0)
                Dim newNests As New List(Of Point)(Me.PopulationSize)
                For i = 0 To Me.PopulationSize - 1
                    Dim s = _populations(i)
                    Dim newNest = New Point(ObjectiveFunction)
                    'levy flight process
                    For j = 0 To ObjectiveFunction.NumberOfVariable - 1
                        Dim u = Util.Util.NormRand(0) * sigma
                        Dim v = Math.Abs(Util.Util.NormRand(0))
                        Dim tempStep = u / Math.Pow(v, 1 / Beta)
                        Dim stepSize = 0.01 * tempStep * (s(j) - currentBest(j))
                        Dim temp = s(j) + stepSize * Util.Util.NormRand(0)
                        newNest(j) = temp
                    Next

                    'Limit solution space
                    LimitSolutionSpace(newNest)

                    newNests.Add(newNest) 'new nests
                Next

                'replace update solution
                For i = 0 To Me.PopulationSize - 1
                    If newNests(i).Eval < _populations(i).Eval Then
                        _populations(i) = newNests(i)
                    End If
                Next

                'Discovery and randomization
                newNests.Clear()
                Dim randPerm1 = Util.Util.RandomPermutaion(Me.PopulationSize)
                Dim randPerm2 = Util.Util.RandomPermutaion(Me.PopulationSize)
                For i = 0 To Me.PopulationSize - 1
                    Dim newNest = New Point(ObjectiveFunction)
                    For j = 0 To ObjectiveFunction.NumberOfVariable - 1
                        If Random.NextDouble() > Me.PA Then
                            newNest(j) = _populations(i)(j) + Random.NextDouble() * (_populations(randPerm1(i))(j) - _populations(randPerm2(i))(j))
                        Else
                            newNest(j) = _populations(i)(j)
                        End If
                    Next

                    'Limit solution space
                    LimitSolutionSpace(newNest)

                    newNests.Add(newNest)
                Next

                'replace
                For i = 0 To Me.PopulationSize - 1
                    If newNests(i).Eval < _populations(i).Eval Then
                        _populations(i) = newNests(i)
                    End If
                Next
            Next

            Return (GetRemainingIterationCount(ai_iteration) = 0)
        End Function
#End Region

#Region "Private"
        ''' <summary>
        ''' Log Gamma function
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Refference:
        ''' C言語による最新アルゴリズム事典
        ''' </remarks>
        Private Function LogGamma(ByVal x As Double) As Double
            Dim LOG_2PI = 1.8378770664093456
            Dim N = 8

            Dim B0 = 1              '   /* 以下はBernoulli数 */
            Dim B1 = (-1.0 / 2.0)
            Dim B2 = (1.0 / 6.0)
            Dim B4 = (-1.0 / 30.0)
            Dim B6 = (1.0 / 42.0)
            Dim B8 = (-1.0 / 30.0)
            Dim B10 = (5.0 / 66.0)
            Dim B12 = (-691.0 / 2730.0)
            Dim B14 = (7.0 / 6.0)
            Dim B16 = (-3617.0 / 510.0)

            Dim v As Double = 1
            Dim w As Double

            While x < N
                v *= x
                x += 1
            End While
            w = 1 / (x * x)
            Return ((((((((B16 / (16 * 15)) * w + (B14 / (14 * 13))) * w _
                        + (B12 / (12 * 11))) * w + (B10 / (10 * 9))) * w _
                        + (B8 / (8 * 7))) * w + (B6 / (6 * 5))) * w _
                        + (B4 / (4 * 3))) * w + (B2 / (2 * 1))) / x _
                        + 0.5 * LOG_2PI - Math.Log(v) - x + (x - 0.5) * Math.Log(x)
        End Function

        ''' <summary>
        ''' Gamma function
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Refference:
        ''' C言語による最新アルゴリズム事典
        ''' </remarks>
        Private Function Gamma(ByVal x As Double) As Double
            If (x < 0) Then
                Return Math.PI / (Math.Sin(Math.PI * x) * Math.Exp(LogGamma(1 - x)))
            End If
            Return Math.Exp(LogGamma(x))
        End Function
#End Region
    End Class
End Namespace
