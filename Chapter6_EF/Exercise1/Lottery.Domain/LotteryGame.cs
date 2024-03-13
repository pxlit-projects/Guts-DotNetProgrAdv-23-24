﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Domain
{
    public class LotteryGame
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int NumberOfNumbersInADraw { get; set; }
        public int MaximumNumber { get; set; }

        public ICollection<Draw> Draws { get; set; }
        
    }
}