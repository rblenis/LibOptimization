Imports LibOptimization2.Util

Namespace Optimization
    ''' <summary>
    ''' Abstarct optimization Class rev2
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class absOptimization
#Region "Member"
        ''' <summary>Objective function</summary>
        Public Property ObjectiveFunction As absObjectiveFunction = Nothing

        ''' <summary>Random object</summary>
        Public Property Random As System.Random = New clsRandomXorshift(BitConverter.ToUInt32(BitConverter.GetBytes(Environment.TickCount), 0))

        ''' <summary>Max iteration count(Default:2000)</summary>
        Public Property Iteration As Integer = 2000

        ''' <summary>Iteration count</summary>
        Protected Property _IterationCount As Integer = 0

        ''' <summary>Epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.00000001

        ''' <summary>Population Size(Default:100)</summary>
        Public Property PopulationSize As Integer = 100

        ''' <summary>Initial position</summary>
        Public Property InitialPosition As Double() = Nothing

        ''' <summary>population</summary>
        Protected _populations As New List(Of clsPoint)

        ''' <summary>criterion index</summary>
        Protected _criterionIndex As Integer = 0

        ''' <summary>Use criterion flag</summary>
        Public Property UseCriterion As Boolean = True

        ''' <summary>Use adaptive population flag(population = 50*ln(variable)+15))</summary>
        Public Property UseAdaptivePopulationSize As Boolean = False

        ''' <summary>Use bounds flag</summary>
        Public Property UseBounds As Boolean = False

        ''' <summary>Range of initial value(This parameters to use when generate a variable)</summary>
        Public Property InitialValueRange As Double = 5

        ''' <summary>Upper and lower bound(limit solution space)</summary>
        Public Property Bounds As Double()() = Nothing
#End Region

#Region "Public"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        Public Sub New()
            'nop
        End Sub

        ''' <summary>
        ''' Initialize optimizer
        ''' </summary>
        ''' <remarks></remarks>
        Public Overridable Function Init() As Boolean
            'check
            If ObjectiveFunction Is Nothing Then
                clsError.SetError(clsError.ErrorType.ERR_INIT, "Not exist ObjectiveFunction")
                Return clsError.IsRecentError()
            End If

            'init
            Me._IterationCount = 0
            Me._populations.Clear()

            'error manage reset
            clsError.Clear()

            'Set initialize value
            Try
                'adaptive population size
                If UseAdaptivePopulationSize = True Then
                    Me.PopulationSize = CInt(50 * Math.Log(Me.ObjectiveFunction.NumberOfVariable) + 15)
                End If

                'generate initial position
                Dim temp(Me.ObjectiveFunction.NumberOfVariable - 1) As Double
                For i As Integer = 0 To Me.PopulationSize - 1
                    For j As Integer = 0 To Me.ObjectiveFunction.NumberOfVariable - 1
                        temp(j) = clsUtil.GenRandomRange(Me.Random, -Me.InitialValueRange, Me.InitialValueRange)
                    Next

                    Dim tempPoint = New clsPoint(New clsPoint(Me.ObjectiveFunction, temp))
                    If UseBounds = True Then
                        clsUtil.LimitSolutionSpace(tempPoint, Bounds)
                    End If

                    Me._populations.Add(tempPoint)
                Next

                'calc criterion index
                If UseCriterion = True Then
                    _criterionIndex = CInt(PopulationSize * 0.7)
                End If

                'Sort Evaluate
                Me._populations.Sort()
            Catch ex As Exception
                clsError.SetError(clsError.ErrorType.ERR_INIT, ex.Message)
            End Try

            Return True
        End Function

        ''' <summary>
        ''' Do Iteration
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>true:Stopping Criterion or Iteration count has been reached. false:Do not Stopping Criterion or remain iteration count.</returns>
        ''' <remarks></remarks>
        Public MustOverride Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean

        ''' <summary>
        ''' Result
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property BestResult As clsPoint
            Get
                Return clsUtil.GetBestPoint(_populations, True)
            End Get
        End Property

        ''' <summary>
        ''' All Sorted Results
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Results As List(Of clsPoint)
            Get
                Return clsUtil.GetSortedResultsByEval(_populations)
            End Get
        End Property

        ''' <summary>
        ''' Iteration count 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IterationCount() As Integer
            Get
                Return _IterationCount
            End Get
        End Property

        ''' <summary>
        ''' Reset Iteration count
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ResetIterationCount()
            Me._IterationCount = 0
        End Sub
#End Region
    End Class
End Namespace
