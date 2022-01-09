﻿using LightWiki.Domain.Models;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace LightWiki.Shared.Query
{
    public class ApplicationSieveProcessor : SieveProcessor
    {
        public ApplicationSieveProcessor(
            IOptions<SieveOptions> options,
            ISieveCustomSortMethods customSortMethods,
            ISieveCustomFilterMethods customFilterMethods)
            : base(options, customSortMethods, customFilterMethods)
        {
        }

        protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
        {
            mapper.Property<Article>(p => p.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<Article>(p => p.UserId)
                .CanFilter();

            mapper.Property<Group>(p => p.Name)
                .CanSort()
                .CanFilter();

            return mapper;
        }
    }
}