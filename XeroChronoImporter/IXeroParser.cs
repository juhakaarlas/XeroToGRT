using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeroChronoImporter
{
    public interface IXeroParser
    {
        ShotSession? Process(string filePath);
    }
}
