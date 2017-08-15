Imports LibOptimization2.MathUtil

Namespace Optimization.RequireDerivative
    ''' <summary>
    ''' Newton Method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Use second derivertive.
    '''  -Second order conversion.
    ''' 
    ''' Refference:
    ''' 金谷健一, "これならわかる最適化数学－基礎原理から計算手法まで－", 共立出版株式会社 2007 初版第7刷, pp79-84 
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' 
    ''' Memo:
    ''' 最適化で微分を用いて求める手法のことを「勾配法」という。
    ''' 最大値を求めることを山登り法、最小値の場合は最急降下法とよばれる。
    ''' </remarks>
    Public Class NewtonMethod : Inherits absOptimization
#Region "Member"
        ''' <summary>hessian matrix coefficient(Default:1.0)</summary>
        Public Property ALPHA As Double = 1.0
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
            Dim vector1 As New clsEasyVector(_populations(0))
            Dim vector2 As New clsEasyVector(_populations(0))
            Try
                ai_iteration = If(ai_iteration = 0, Iteration - 1, ai_iteration - 1)
                Dim gradient As New clsEasyVector(ObjectiveFunction.NumberOfVariable, clsEasyVector.VectorDirection.COL)
                Dim h As New clsEasyMatrix()
                For iterate As Integer = 0 To ai_iteration
                    'Calculate Gradient vector
                    gradient.RawVector = ObjectiveFunction.Gradient(vector1)

                    'Check criterion (gradient base)
                    If gradient.NormL1() < EPS Then
                        Return True
                    End If

                    'Calculate Hessian matrix
                    h.RawMatrix = ObjectiveFunction.Hessian(vector1)

                    'Update
                    vector2 = vector1 - ALPHA * h.Inverse() * gradient 'H^-1 calulate heavy...

                    'limit solution space
                    LimitSolutionSpace(vector2)

                    'Check conversion2
                    Dim diff = vector2 - vector1
                    vector1 = vector2
                    If diff.NormL2() = 0 Then
                        Return True
                    End If

                    'Check and Update Iteration count
                    If Iteration = MyBase._IterationCount Then
                        Return True
                    End If
                    MyBase._IterationCount += 1
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