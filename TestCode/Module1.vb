Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util
Imports LibOptimization.ErrorManage

Module Module1

    Sub Main()
        'With Nothing
        '    Dim opt = New LibOptimization.Optimization.DerivativeFree.NelderMead()
        '    opt.Init()
        '    opt.DoIteration()
        '    Util.DebugValue(opt)
        'End With

        'With Nothing
        '    Dim opt = New LibOptimization.Optimization.DerivativeFree.NelderMead()
        '    opt.ObjectiveFunction = New BenchSphere(2)
        '    opt.DoIteration()
        '    Util.DebugValue(opt)
        'End With

        'With Nothing
        '    Dim opt = New LibOptimization.Optimization.DerivativeFree.NelderMead()
        '    opt.DoIteration()
        '    Util.DebugValue(opt)
        'End With

        With Nothing
            Dim optimization As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
            optimization.ObjectiveFunction = New BenchSphere(2)

            'Initial value is generated in the range of -3 to 3.
            optimization.InitialValueRange = 3

            'init
            optimization.Init({1, 1})
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

        With Nothing
            While True
                Dim optimization As New Optimization.DerivativeFree.ReadlCodedGA.RealGAUNDX()
                optimization.ObjectiveFunction = New BenchSphere(2)
                optimization.UseCriterion = False
                optimization.Iteration = 15000
                'optimization.AlternationStrategy = LibOptimization.Optimization.DerivativeFree.ReadlCodedGA.RealGAUNDX.EnumAlternatioType.MGG
                optimization.Init()
                Util.DebugValue(optimization, ai_isOutValue:=False)
                While (optimization.DoIteration(100) = False)
                    Util.DebugValue(optimization, ai_isOutValue:=False)
                End While
                Util.DebugValue(optimization)
            End While
        End With
    End Sub

End Module
