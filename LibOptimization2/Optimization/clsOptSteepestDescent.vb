Imports LibOptimization2.MathUtil

Namespace Optimization
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
    Public Class clsOptSteepestDescent : Inherits absOptimization
#Region "Member(Original parameter forSteepestDescent)"
        ''' <summary>Rate</summary>
        Public Property ALPHA As Double = 0.3
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
            If ai_iteration = 0 Then
                ai_iteration = Me.Iteration - 1
            End If

            'Do Iteration
            Dim vector As New clsEasyVector(_populations(0))
            Dim gradent As New clsEasyVector(MyBase.ObjectiveFunction.NumberOfVariable)
            ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
            Try
                For iterate As Integer = 0 To ai_iteration
                    'Calculate Gradient vector
                    gradent.RawVector = Me.ObjectiveFunction.Gradient(vector)

                    'Check conversion
                    If gradent.NormL1() < EPS Then
                        Return True
                    End If

                    'Update
                    vector = vector - Me.ALPHA * gradent

                    'Check Iteration count
                    If Iteration = _IterationCount Then
                        Return True
                    End If

                    _IterationCount += 1
                Next

                Return False
            Finally
                'return member
                For i As Integer = 0 To vector.Count - 1
                    _populations(0)(i) = vector(i)
                Next
                _populations(0).ReEvaluate()
            End Try
        End Function
    End Class
#End Region
End Namespace