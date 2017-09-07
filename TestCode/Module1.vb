Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util

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
