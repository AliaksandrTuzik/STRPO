using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRPO.Clusterisation.Models
{
    public class TreeWithSilhouette
    {
        private double? _innerRadius;
        private double? _outerRadius;
        private TreeWithSilhouette _parent;

        public object Value { get; set; }

        public ObservableCollection<TreeWithSilhouette> Children { get; }

        internal int DeepChildrenCount => Children?.Any() == true ? Children.Sum(child => child.DeepChildrenCount) : 1;

        internal TreeWithSilhouette Parent
        {
            get => _parent;
            set
            {
                _parent = value;

                ResetOuterRadius();
            }
        }

        internal double InnerRadius
        {
            get
            {
                if (!_innerRadius.HasValue)
                {
                    _innerRadius = CalculateInnerRadius();
                }

                return _innerRadius.Value;
            }
        }

        private void ResetInnerRadius()
        {
            _innerRadius = null;

            Parent?.ResetInnerRadius();
        }

        internal double OuterRadius
        {
            get
            {
                if (!_outerRadius.HasValue)
                {
                    _outerRadius = CalculateOuterRadius();
                }

                return _outerRadius.Value;
            }
        }

        private void ResetOuterRadius()
        {
            _outerRadius = null;

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child?.ResetOuterRadius();
                }
            }
        }

        internal double SilhouetteCoef => Math.Max(OuterRadius, InnerRadius) == 0
            ? 0 : (OuterRadius - InnerRadius) / Math.Max(OuterRadius, InnerRadius);

        public int Depth => (Parent?.Depth ?? -1) + 1;

        public bool IsCluster { get; internal set; }

        public TreeWithSilhouette()
        {
            Children = new ObservableCollection<TreeWithSilhouette>();
            Children.CollectionChanged += OnChildrenCollectionChanged;
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetInnerRadius();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Reset:
                    if (e.NewItems != null)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            (newItem as TreeWithSilhouette).Parent = this;
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            (oldItem as TreeWithSilhouette).Parent = null;
                        }
                    }
                    ResetInnerRadius();
                    break;
            }
        }

        private double CalculateInnerRadius()
        {
            if (Children?.Any() != true)
            {
                return 0;
            }

            if (Children.Count == 1)
            {
                return Children.First().InnerRadius;
            }

            double innerRadius = 0;

            foreach (var child1 in Children)
            {
                foreach (var child2 in Children)
                {
                    if (child1 == child2)
                    {
                        innerRadius += child1.InnerRadius * child1.DeepChildrenCount * (child1.DeepChildrenCount - 1);
                    }
                    else
                    {
                        innerRadius += (child1.DeepChildrenCount * child2.DeepChildrenCount) * 1.0 / (1 << Depth);
                    }
                }
            }

            innerRadius /= DeepChildrenCount * (DeepChildrenCount - 1);

            return innerRadius;
        }

        private double CalculateOuterRadius()
        {
            var parent = Parent;

            if (parent == null)
            {
                return 0;
            }

            while (parent.Children.Count < 2)
            {
                if (parent == null)
                {
                    return 0;
                }

                parent = parent.Parent;
            }

            return 1.0 / (1 << parent.Depth);

            //Ye old way

            //double outerRadius = Parent.OuterRadius * Parent.DeepChildrenCount * GetOuterElementsCount(Parent);

            //foreach (var sibling in Parent.Children)
            //{
            //    if (sibling != this)
            //    {
            //        outerRadius += DeepChildrenCount * sibling.DeepChildrenCount * 1.0 / (1 << Parent.Depth);
            //    }
            //}

            //outerRadius /= DeepChildrenCount * GetOuterElementsCount(this);

            //return outerRadius;
        }

        #region Static methods
        public static List<double> GetAverageSilhouettes(TreeWithSilhouette tree)
        {
            var clusters = new[] { tree };

            var maxDepth = GetMaxDepth(tree);

            var avrSilhouettes = new List<double>();

            avrSilhouettes.Add(clusters.Average(cluster => cluster.SilhouetteCoef));

            for (int i = 1; i < maxDepth; i++)
            {
                clusters = clusters.SelectMany(cluster => cluster.Children ?? Enumerable.Empty<TreeWithSilhouette>()).ToArray();

                avrSilhouettes.Add(clusters.Average(cluster => cluster.SilhouetteCoef));
            }

            return avrSilhouettes;
        }

        private static int GetMaxDepth(TreeWithSilhouette tree)
        {
            return tree.Children?.Any() == true ? tree.Children.Max(child => GetMaxDepth(child)) : tree.Depth;
        }

        private static int GetOuterElementsCount(TreeWithSilhouette tree)
        {
            if (tree.Parent == null)
            {
                return 0;
            }

            var sum = tree.Parent.Children.Select(child => child == tree ? 0 : child.DeepChildrenCount).Sum();

            return sum + GetOuterElementsCount(tree.Parent);
        }
        #endregion Static methods
    }
}
