Imports LibOptimization.MathUtil

Namespace Optimization.RequireDerivative
    ''' <summary>
    ''' Steepest descent method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Use first derivertive.
    '''  -First order conversion.
    ''' 
    ''' Refference:
    ''' [1]http://dsl4.eee.u-ryukyu.ac.jp/DOCS/nlp/node4.html
    ''' [2]金谷健一, "これならわかる最適化数学－基礎原理から計算手法まで－", 共立出版株式会社 2007 初版第7刷, pp79-84 
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' 
    ''' Memo:
    ''' 最適化で微分を用いて求める手法のことを「勾配法」という。
    ''' 最大値を求めることを山登り法、最小値の場合は最急降下法とよばれる。
    ''' </remarks>
    Public Class SteepestDescent : Inherits absOptimization
#Region "Member(Original parameter for SteepestDescent)"
        ''' <summary>gradient coefficient</summary>
        Public Property ALPHA As Double = 0.2
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize setting and value
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Init() As Boolean
            MyBase.PopulationSize = 1
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
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'Do Iteration
            Dim vector1 As New EasyVector(_populations(0))
            Dim vector2 As New EasyVector(_populations(0))
            Try
                ai_iteration = If(ai_iteration = 0, Iteration - 1, ai_iteration - 1)
                Dim gradient As New EasyVector(ObjectiveFunction.NumberOfVariable)
                For iterate As Integer = 0 To ai_iteration
                    'Calculate Gradient vector
                    gradient.RawVector = ObjectiveFunction.Gradient(vector1)

                    'Check conversion1
                    If gradient.NormL1() < EPS Then
                        Return True
                    End If

                    'Update
                    vector2 = vector1 - ALPHA * gradient

                    'limit solution space
                    LimitSolutionSpace(vector2)

                    'Check conversion2
                    Dim diff = vector2 - vector1
                    vector1 = vector2
                    If diff.NormL2() = 0 Then
                        Return True
                    End If

                    'Check and Update Iteration count
                    If Iteration = _IterationCount Then
                        Return True
                    End If
                    _IterationCount += 1
                Next

                Return False
            Finally
                'return member
                For i As Integer = 0 To vector1.Count - 1
                    _populations(0)(i) = vector1(i)
                Next
                _populations(0).ReEvaluate()
            End Try
        End Function
    End Class
#End Region
End Namespace