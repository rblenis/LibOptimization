using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOptimization.Optimization;
using LibOptimization.Util;
using LibOptimization.ErrorManage;

namespace SampleCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Typical use
            {
                //Target Function
                var func = new RosenBrock();

                //Set Function
                var optimization = new LibOptimization.Optimization.DerivativeFree.NelderMead();
                optimization.ObjectiveFunction = func;
                optimization.Init();

                //Initialize starting value
                optimization.Init();
                if (ErrorManage.IsRecentError() == true)
                {
                    Console.WriteLine(ErrorManage.GetRecentError());
                    return;
                }

                //Do optimization
                optimization.DoIteration();
                if (ErrorManage.IsRecentError() == true)
                {
                    Console.WriteLine(ErrorManage.GetRecentError());
                    return;
                }

                //Get reuslt
                Console.WriteLine(optimization.Result.ToString());
            }
        }
    }
}
