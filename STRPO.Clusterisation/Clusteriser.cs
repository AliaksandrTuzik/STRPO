using STRPO.Clusterisation.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace STRPO.Clusterisation
{
    public class Clusteriser
    {
        public async Task<TreeWithSilhouette> ClusterizeAsync<T>(IEnumerable<T> data)
        {
            var properties = typeof(T).GetProperties().Where(pr => !pr.CustomAttributes.Any()).ToList();

            var stringCollectionProperties = properties.Where(property =>
                property.PropertyType != typeof(string) &&
                property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) &&
                property.PropertyType.GenericTypeArguments.All(type => type == typeof(string))
            );

            var pairs = properties.Except(stringCollectionProperties).Select(
                property =>
                (
                    Key: property.Name,
                    Value: new Func<T, string>(t => property.GetValue(t)?.ToString())
                )
            ).ToList();

            pairs.AddRange(stringCollectionProperties.SelectMany(
                property =>
                {
                    var vals = data
                                ?.SelectMany(d => property.GetValue(d) as IEnumerable<string>)
                                ?.Select(str => str?.ToLowerInvariant())
                                ?.Distinct();

                    return vals?.Select(val => (
                            Key: $"{property.Name}_{val}",
                            Value: new Func<T, string>(t => (property.GetValue(t) as IEnumerable<string>)?.Contains(val).ToString())
                        )
                    );
                }
            ).Take(1000));

            var clusterizationVariants = new List<TreeWithSilhouette>();

            // Shuffle pairs

            var propertiesValues = (await Task.WhenAll(pairs.Select(pair => Task.Run(
                () =>
                {
                    return (
                        Key: pair.Key,
                        GetValue: pair.Value,
                        KnownValues: data.Select(d => pair.Value(d)).Distinct().ToList()
                    );
                })
            ))).Where(pair => pair.KnownValues.Count() > 1).ToList();

            // Questionable
            propertiesValues = propertiesValues.OrderBy(pr => pr.KnownValues.Count).ToList();

            clusterizationVariants.Add(await GetTreeFromDataAsync(data, propertiesValues));

            // Optimize variants

            var variant = clusterizationVariants.Last();

            foreach (var child in variant.Children)
            {
                child.IsCluster = true;
            }

            //ClearOutSmallClusters(variant);

            //OptimizeClusterization(variant);

            var s = clusterizationVariants[0].Children.Average(tree => tree.SilhouetteCoef);

            return clusterizationVariants.OrderBy(tree => tree.SilhouetteCoef).FirstOrDefault();
        }

        private static void ClearOutSmallClusters(TreeWithSilhouette variant)
        {
            bool hasChanged;
            do
            {
                hasChanged = false;


            }
            while (hasChanged);
        }

        private static void OptimizeClusterization(TreeWithSilhouette variant)
        {
            bool hasChanged;
            do
            {
                hasChanged = false;

                var weakClusters = new List<TreeWithSilhouette>();

                foreach (var child in variant.Children)
                {
                    if (child.Children?.Any() == true && child.SilhouetteCoef < child.Children.Average(ch => ch.SilhouetteCoef))
                    {
                        weakClusters.Add(child);
                        hasChanged = true;
                    }
                }

                foreach (var weakCluster in weakClusters)
                {
                    variant.Children.Remove(weakCluster);
                    foreach (var subCluster in weakCluster.Children)
                    {
                        variant.Children.Add(subCluster);
                    }
                }
            }
            while (hasChanged);
        }

        private async Task<TreeWithSilhouette> GetTreeFromDataAsync<T>(IEnumerable<T> data, List<(string Key, Func<T, string> GetValue, List<string> KnownValues)> propertiesValues)
        {
            if (!data.Any())
            {
                return null;
            }

            var result = new TreeWithSilhouette();

            if (propertiesValues.Count == 0)
            {
                foreach (var d in data)
                {
                    result.Children.Add(new TreeWithSilhouette { Value = d });
                }

                return result;
            }

            var property = propertiesValues.First();

            var children = (await Task.WhenAll(
                property.KnownValues.Select(
                    val =>
                    Task.Run(async () => {
                        var nodeData = data.Where(d => property.GetValue(d) == val).ToList();

                        if (!nodeData.Any())
                        {
                            return null;
                        }

                        if (nodeData.Count() == 1)
                        {
                            var res = new TreeWithSilhouette { Value = nodeData.First() };

                            for (int i = 0; i < propertiesValues.Count; i++)
                            {
                                var temp = new TreeWithSilhouette();
                                temp.Children.Add(res);
                                res = temp;
                            }

                            return res;
                        }

                        return await GetTreeFromDataAsync(nodeData, propertiesValues.Skip(1).ToList());
                    })
                )
            )).Where(tree => tree != null);

            while (children.Count() == 1)
            {
                children = children.First()?.Children;
            }

            foreach (var child in children)
            {
                result.Children.Add(child);
            }

            if (!result.Children.Any())
            {
                return null;
            }

            return result;
        }
    }
}
