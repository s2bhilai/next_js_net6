﻿using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        private readonly AuctionDbContext _auctionDbContext;

        public AuctionFinishedConsumer(AuctionDbContext auctionDbContext)
        {
            this._auctionDbContext = auctionDbContext;
        }

        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("--> Consuming auction finished");

            var auction = await _auctionDbContext.Auctions.FindAsync(context.Message.AuctionId);

            if(context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount;
            }

            auction.Status = auction.SoldAmount > auction.ReservePrice ? 
                Entities.Status.Finished : Entities.Status.ReserveNotMet;

            await _auctionDbContext.SaveChangesAsync();
        }
    }
}
