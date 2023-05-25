using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    internal class MoveHistory
    {
        public int rowIndex { get; set; }
        public int columnIndex { get; set; }
        public String letter { get; set; }
        public int weight { get; set; }
    }
}
