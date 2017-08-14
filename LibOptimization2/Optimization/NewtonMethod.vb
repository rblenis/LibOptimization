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
            If ai_iteration = 0 Then
                ai_iteration = Me.Iteration - 1
            End If

            'Do Iteration
            Dim vector As New clsEasyVector(_populations(0))
            Dim gradient As New clsEasyVector(MyBase.ObjectiveFunction.NumberOfVariable, clsEasyVector.VectorDirection.COL)
            Dim h As New clsEasyMatrix()
            Try
                ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
                For iterate As Integer = 0 To ai_iteration
                    'Calculate Gradient vector
                    gradient.RawVector = Me.ObjectiveFunction.Gradient(vector)

                    'Check conversion
                    If gradient.NormL1() < EPS Then
                        Return True
                    End If

                    'Calculate Hessian matrix
                    h.RawMatrix = Me.ObjectiveFunction.Hessian(vector)

                    'Update
                    vector = vector - Me.ALPHA * h.Inverse() * gradient 'H^-1 calulate heavy...

                    'Check and Update Iteration count
                    If Iteration = MyBase._IterationCount Then
                        Return True
                    End If
                    MyBase._IterationCount += 1
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