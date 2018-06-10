﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp.Modularity;

namespace Volo.Abp.AspNetCore.Mvc.UI.Bundling
{
    public class BundleContributorCollection
    {
        private readonly List<IBundleContributor> _contributors;

        public BundleContributorCollection()
        {
            _contributors = new List<IBundleContributor>();
        }

        public void Add(IBundleContributor contributor)
        {
            _contributors.Add(contributor);
        }

        public void Add<TContributor>()
            where TContributor : IBundleContributor, new()
        {
            Add(typeof(TContributor));
        }

        public void Add([NotNull] Type contributorType)
        {
            Check.NotNull(contributorType, nameof(contributorType));

            AddWithDependencies(contributorType);
        }

        public IReadOnlyList<IBundleContributor> GetAll()
        {
            return _contributors.ToImmutableList();
        }

        private bool IsAlreadyAdded(Type contributorType)
        {
            return _contributors.Any(c => c.GetType() == contributorType);
        }

        private void AddWithDependencies(Type contributorType)
        {
            if (IsAlreadyAdded(contributorType))
            {
                return;
            }

            var dependsOnAttributes = contributorType
                .GetCustomAttributes(true)
                .OfType<IDependedTypesProvider>()
                .ToList();

            foreach (var dependsOnAttribute in dependsOnAttributes)
            {
                foreach (var dependedType in dependsOnAttribute.GetDependedTypes())
                {
                    AddWithDependencies(dependedType); //Recursive call
                }
            }

            AddInstanceToContributors(contributorType);
        }

        private void AddInstanceToContributors(Type contributorType)
        {
            if (!typeof(IBundleContributor).IsAssignableFrom(contributorType))
            {
                throw new AbpException($"Given {nameof(contributorType)} ({contributorType.AssemblyQualifiedName}) should implement the {typeof(IBundleContributor).AssemblyQualifiedName} interface!");
            }

            try
            {
                _contributors.Add((IBundleContributor)Activator.CreateInstance(contributorType));
            }
            catch (Exception ex)
            {
                throw new AbpException($"Can not instantiate {contributorType.AssemblyQualifiedName}", ex);
            }
        }
    }
}