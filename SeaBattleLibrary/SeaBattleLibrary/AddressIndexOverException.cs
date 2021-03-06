﻿using System;

namespace SeaBattleLibrary
{
    class IncorrectIndexException: Exception
    {
        public IncorrectIndexException(int start, int end): base("Некорректный индекс. Индекс может быть задан в диапозоне от " + 
            start + " до " + end) {}
    }
}
