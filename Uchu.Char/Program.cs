﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.Char
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("Expected 2 argument.");

            if (!Guid.TryParse(args[0], out var id))
                throw new ArgumentException($"{args[0]} is not a valid GUID");

            ServerSpecification specification;

            await using (var ctx = new UchuContext())
            {
                specification = await ctx.Specifications.FirstOrDefaultAsync(c => c.Id == id);

                if (specification == default)
                    throw new ArgumentException($"{args[0]} is not a valid server specification ID");
            }

            var server = new Server(specification.Id, args[1]);

            await server.StartAsync();

            await using (var ctx = new UchuContext())
            {
                specification = await ctx.Specifications.FirstAsync(c => c.Id == id);

                ctx.Specifications.Remove(specification);

                await ctx.SaveChangesAsync();
            }
        }
    }
}