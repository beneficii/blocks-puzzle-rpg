using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyToolkit
{
    public interface IHasInfo
    {
        public string GetTitle();
        public string GetDescription();
        public List<string> GetTooltips();
    }
}
