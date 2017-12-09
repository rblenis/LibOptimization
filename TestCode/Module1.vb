Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util
Imports LibOptimization.ErrorManage

Module Module1

    Sub Main()

        With Nothing
            'While True
            '    Dim optimization As New Optimization.RequireDerivative.SteepestDescent()
            '    optimization.ObjectiveFunction = New BenchSphereNumericalDerivertive(2)
            '    optimization.UseCriterion = True
            '    optimization.Iteration = 15000
            '    'optimization.AlternationStrategy = LibOptimization.Optimization.DerivativeFree.ReadlCodedGA.RealGAUNDX.EnumAlternatioType.MGG
            '    optimization.Init({100000000000.0, 10})
            '    Util.DebugValue(optimization, ai_isOutValue:=False)
            '    While (optimization.DoIteration(1) = False)
            '        Util.DebugValue(optimization, ai_isOutValue:=False)
            '    End While
            '    Util.DebugValue(optimization)
            'End While

            Dim optimization As New Optimization.DerivativeFree.EvolutionStrategy.ES_1_1()
            optimization.ObjectiveFunction = New BenchRosenblock(10)
            optimization.Iteration = 50000
            optimization.Init()
            Util.DebugValue(optimization, ai_isOutValue:=False)
            While (optimization.DoIteration(50) = False)
                Util.DebugValue(optimization, ai_isOutValue:=False)
            End While
            Util.DebugValue(optimization)
        End With
    End Sub

End Module
