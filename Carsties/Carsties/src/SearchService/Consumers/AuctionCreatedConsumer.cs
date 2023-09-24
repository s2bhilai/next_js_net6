﻿using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("--> Consuming auction created: " + context.Message.Id);

            var item = mapper.Map<Item>(context.Message);

            if (item.Model == "foo") throw new ArgumentException("cannot sell cars with name of foo");

            await item.SaveAsync();
        }
    }
}
