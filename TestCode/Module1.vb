Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util

Module Module1

    Sub Main()
        Dim optimization As New Optimization.DerivativeFree.DifferentialEvolution.DE_best_1_bin()
        optimization.ObjectiveFunction = New BenchSphere(2)
        optimization.CriterionRatio = 0.05
        optimization.UseCriterion = True
        optimization.Init()
        optimization.DoIteration()
        Util.DebugValue(optimization)
    End Sub

End Module
