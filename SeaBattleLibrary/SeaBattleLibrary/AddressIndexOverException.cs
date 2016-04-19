using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattleLibrary
{
    class IncorrectIndexException: Exception
    {
        public IncorrectIndexException(int start, int end): base("Некорректный индекс. Индекс может быть задан в диапозоне от " + 
            start + " до " + end) {}
    }
}
