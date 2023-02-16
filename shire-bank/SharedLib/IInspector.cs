using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    internal interface IInspector
    {
        string GetFullSummary();
        void StartInspection();
        void FinishInspection();
    }
}
