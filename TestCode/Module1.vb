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
            Dim optimization As New Optimization.DerivativeFree.ReadlCodedGA.RealGAREX()
            optimization.ObjectiveFunction = New BenchSphere(2)
            'optimization.UseCriterion = True
            'optimization.CriterionRatio = 0.3 'default 0.7
            optimization.UseCriterion = False
            optimization.Init()
            optimization.DoIteration()
            Util.DebugValue(optimization)
        End With
    End Sub

End Module
