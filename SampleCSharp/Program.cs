using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOptimization.Optimization;
using LibOptimization.Util;

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
                var opt = new LibOptimization.Optimization.DerivativeFree.NelderMead();
                opt.ObjectiveFunction = func;
                opt.Init();

                //Optimization
                opt.DoIteration();

                //Check Error
                if( ErrorManage.IsRecentError()==true)
                {
                    Console.WriteLine(ErrorManage.GetRecentError());
                    return;
                }
                else
                {
                    //Get Result
                    Util.DebugValue(opt);
                }
            }
        }
    }
}
