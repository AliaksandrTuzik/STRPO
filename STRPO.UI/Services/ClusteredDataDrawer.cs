using STRPO.UI.Models;
using STRPO.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace STRPO.UI.Services
{
    class ClusteredDataDrawer: ViewModel.IDrawer
    {
        private readonly Canvas _canvas;

        private double _size => Math.Min(_canvas.Width, _canvas.Height);

        internal ClusteredDataDrawer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void Draw(Tree clusterisedData)
        {
            Draw(clusterisedData, new Point(_size / 2, _size / 2), _size / 4, 0);

            _canvas.InvalidateArrange();
        }

        private void Draw(Tree clusterisedData, Point point, double radius, double angle, Color? clusterColor = null)
        {
            _canvas.Children.Clear();

            //DrawCircle(point, 3, GetColorFromAngle(0));
            //var col = GetColorFromAngle(angle);
            //col.A = 100;
            //DrawCircle(point, 2 * radius, col);

            if (clusterisedData?.Children?.Any() != true)
            {
                DrawCircle(point, 2, /*clusterColor ?? */GetColorFromAngle(angle) /*Colors.Green*/, clusterisedData);

                return;
            }

            var allGrandChildren = Math.Max(1, clusterisedData.Children.Select(child => Math.Max(1, child?.Children?.Count() ?? 1)).Sum());

            var _angle = .0;

            foreach (var tree in clusterisedData.Children)
            {
                double halfAngle = (Math.Max(1, tree?.Children?.Count() ?? 1)) * Math.PI / allGrandChildren;

                _angle += halfAngle;

                double possibleRadius = radius * Math.Sqrt(2 * (1 - Math.Cos(halfAngle)));
                //Task.Run(() =>
                //{
                    Draw(
                        tree,
                        new Point(radius * Math.Cos(_angle) + point.X, radius * Math.Sin(_angle) + point.Y),
                        Math.Min(possibleRadius, radius) * 2 / 3,
                        angle + _angle / 10,
                        tree.IsCluster ? GetColorFromAngle(_angle) : clusterColor
                    );
                //});

                _angle += halfAngle;
            }
        }

        private static Color GetColorFromAngle(double angle)
        {
            return Color.FromArgb(
                255,
                Convert.ToByte(Convert.ToInt32(Math.Round(255 * Math.Sin(angle) + 255)) % 255),
                Convert.ToByte(Convert.ToInt32(Math.Round(255 * Math.Sin(angle + Math.PI / 3) + 255)) % 255),
                Convert.ToByte(Convert.ToInt32(Math.Round(255 * Math.Sin(angle + 2 * Math.PI / 3) + 255)) % 255)
            );
        }

        private void DrawCircle(Point point, double radius, Color color, Tree tree)
        {
            var t = new DispatcherTimer();
            t.Tick += (s, e) =>
            {
                var dot = new Ellipse
                {
                    Width = 2 * radius,
                    Height = 2 * radius,
                    Stroke = new SolidColorBrush(color),
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(color)
                };

                if (tree?.Value != null)
                {
                    var properties = tree.Value.GetType().GetProperties();

                    var pairs = properties.Select(
                        property =>
                        ((
                            Key: property.Name,
                            Value: property.GetValue(tree.Value)?.ToString()
                        ))
                    );

                    //var markBrush = new SolidColorBrush((Color)properties.First(pr => pr.PropertyType == typeof(Color)).GetValue(tree.Value));
                    //dot.Fill = markBrush;
                    //dot.Stroke = markBrush;

                    var panel = new StackPanel();

                    foreach (var pair in pairs)
                    {
                        panel.Children.Add(new TextBlock { Text = $"{pair.Key}: {pair.Value}" });
                    }

                    dot.ContextFlyout = new Flyout
                    {
                        Content = panel
                    };
                }

                dot.SetValue(Canvas.LeftProperty, point.X - radius);
                    dot.SetValue(Canvas.TopProperty, point.Y - radius);

                _canvas.Children.Add(dot);

                t.Stop();
            };

            t.Start();
        }
    }
}
