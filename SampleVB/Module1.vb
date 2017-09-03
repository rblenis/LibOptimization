Imports LibOptimization
Imports LibOptimization.Optimization
Imports LibOptimization.Util
Imports LibOptimization.BenchmarkFunction

Module Module1
    '------------------------------------------------------------------------------------------------------------------
    'Sample code.
    'LibOptimization is numerical optimization algorithm library for .NET Framework.
    'This library will probably simplify the optimization using C# and VB.Net and other .NET Framework language.
    '------------------------------------------------------------------------------------------------------------------
    Sub Main()
        'Typical use
        With Nothing
            'How to use
            ' 1. You inherit "absObjectiveFunction" class and design objective function.
            ' 2. Choose an optimization method and implement code.
            ' 3. Do optimization!
            ' 4. Get result and evaluate.

            'Instantiation optimization class and set objective function.
            Dim optimization As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
            optimization.ObjectiveFunction = New BenchSphere(2)
            'Initialize starting value
            optimization.Init()
            'Do optimization
            optimization.DoIteration()
            Util.DebugValue(optimization)
        End With

        'set inital position and inital value range
        With Nothing
            Dim optimization As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
            optimization.ObjectiveFunction = New BenchSphere(2)

            'Initial value is generated in the range of 2.5 and 3.5.
            optimization.InitialPosition = {3, 3}
            optimization.InitialValueRange = 0.5

            'init
            optimization.Init()
            If ErrorManage.IsRecentError() = True Then
                Console.WriteLine(ErrorManage.GetRecentError())
                Return
            End If

            'do optimization
            optimization.DoIteration()
            If ErrorManage.IsRecentError() = True Then
                Console.WriteLine(ErrorManage.GetRecentError())
                Return
            End If

            'get result
            Util.DebugValue(optimization)
        End With

        'Set initial point. (Not preparation all algorithms.)
        With Nothing
            Dim optimization As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
            optimization.ObjectiveFunction = New BenchSphere(2)

            'init with initial point
            optimization.Init(New Double() {5, 5})
            If ErrorManage.IsRecentError() = True Then
                Console.WriteLine(ErrorManage.GetRecentError())
                Return
            End If

            'do optimization
            optimization.DoIteration()
            If ErrorManage.IsRecentError() = True Then
                Console.WriteLine(ErrorManage.GetRecentError())
                Return
            End If
        End With

        'use boundary
        With Nothing
            Dim optimization As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
            optimization.ObjectiveFunction = New BenchSphere(2)

            'set bpundary
            optimization.UpperBounds = {1, 1}
            optimization.LowerBounds = {2, 2}
            optimization.UseBounds = True

            'init
            optimization.Init()
            If ErrorManage.IsRecentError() = True Then
                Console.WriteLine(ErrorManage.GetRecentError())
                Return
            End If

            'do optimization
            optimization.DoIteration()
            If ErrorManage.IsRecentError() = True Then
                Console.WriteLine(ErrorManage.GetRecentError())
                Return
            End If

            'get result
            Util.DebugValue(optimization)
        End With

        'fix random seed.
        With Nothing
            Random.XorshiftSingleton.GetInstance.SetDefaultSeed()
            Dim optimization As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
            optimization.Random = New Random.Xorshift()
            optimization.ObjectiveFunction = New BenchSphere(2)
            'init
            optimization.Init()
            'do optimization
            optimization.DoIteration()
            'get result
            Util.DebugValue(optimization)
        End With

        'When you want result every 5 times.
        With Nothing
            Dim optimization As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
            optimization.ObjectiveFunction = New BenchSphere(2)
            optimization.Init()
            Util.DebugValue(optimization)
            While (optimization.DoIteration(5) = False)
                Util.DebugValue(optimization, ai_isOutValue:=False)
            End While
            Util.DebugValue(optimization)
        End With

        'Reuse best point
        With Nothing
            Dim optimization As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
            optimization.ObjectiveFunction = New BenchDeJongFunction3()

            '1st try
            optimization.Init()
            While (optimization.DoIteration(100) = False)
                Util.DebugValue(optimization, ai_isOutValue:=False)
            End While
            Util.DebugValue(optimization)

            '2nd try reuse
            optimization.Init(True)
            While (optimization.DoIteration(100) = False)
                Util.DebugValue(optimization, ai_isOutValue:=False)
            End While
            Util.DebugValue(optimization)
        End With

        'Multi point and MultiThread(Multipoint avoids Local minimum by preparing many values.)
        With Nothing
            'prepare many optimization class.
            Dim multipointNumber As Integer = 30
            Dim listOptimization As New List(Of absOptimization)
            Dim f = New BenchAckley(20)
            For i As Integer = 0 To multipointNumber - 1
                Dim tempOpt As New LibOptimization.Optimization.DerivativeFree.NelderMead()
                tempOpt.ObjectiveFunction = f
                tempOpt.Init()
                listOptimization.Add(tempOpt)
            Next

            'using Parallel.ForEach
            Dim lockObj As New Object()
            Dim best As absOptimization = Nothing
            Threading.Tasks.Parallel.ForEach(listOptimization, Sub(opt As absOptimization)
                                                                   opt.DoIteration()
                                                                   'Swap best result
                                                                   SyncLock lockObj
                                                                       If best Is Nothing Then
                                                                           best = opt
                                                                       ElseIf best.Result.Eval > opt.Result.Eval Then
                                                                           best = opt
                                                                       End If
                                                                   End SyncLock
                                                               End Sub)

            'Check Error
            If ErrorManage.IsRecentError() = True Then
                Return
            End If
            Util.DebugValue(best)
        End With

        'LeastSquaresMethod 最小二乗法
        With Nothing
            'objective function for LeastSquaresMethod
            Dim objectiveFunction = New LeastSquaresMethod()
            objectiveFunction.Init()

            'set optimizer
            Dim opt As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
            opt.ObjectiveFunction = objectiveFunction 'set
            opt.Init()

            Util.DebugValue(opt)
            While (opt.DoIteration(50) = False)
                Util.DebugValue(opt, ai_isOutValue:=False)
            End While
            Util.DebugValue(opt)
        End With
    End Sub
End Module
