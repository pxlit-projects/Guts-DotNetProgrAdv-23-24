using System;
using System.Collections.Generic;
using System.Linq;
using Lottery.AppLogic.Interfaces;
using Lottery.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Infrastructure
{
    internal class DrawRepository : IDrawRepository
    {
        public DrawRepository(LotteryContext context)
        {
        }

        public IList<Draw> Find(int lotteryGameId, DateTime? fromDate, DateTime? untilDate)
        {
            throw new NotImplementedException();
        }

        public void Add(Draw draw)
        {
        }
    }
}