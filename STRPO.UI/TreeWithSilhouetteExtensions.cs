using STRPO.Clusterisation.Models;

namespace STRPO.UI
{
    public static class TreeWithSilhouetteExtensions
    {
        public static TreeWithSilhouette AddEmptyNodes(this TreeWithSilhouette tree, int childNodes)
        {
            for (int i = 0; i < childNodes; i++)
            {
                tree.Children.Add(new TreeWithSilhouette());
            }

            return tree;
        }

        public static TreeWithSilhouette AddParentNodes(this TreeWithSilhouette tree, int parentNodes)
        {
            var result = tree;
            for (int i = 0; i < parentNodes; i++)
            {
                var temp = new TreeWithSilhouette();
                temp.Children.Add(result);
                result = temp;
            }

            return result;
        }

        public static TreeWithSilhouette AddChildren(this TreeWithSilhouette tree, params TreeWithSilhouette[] children)
        {
            foreach(var child in children)
            {
                tree.Children.Add(child);
            }

            return tree;
        }
    }
}
