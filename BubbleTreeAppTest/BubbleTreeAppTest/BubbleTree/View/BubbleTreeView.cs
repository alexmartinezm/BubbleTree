using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BubbleTreeAppTest.BubbleTree.NodeTypes;
using BubbleTreeAppTest.BubbleTree.ViewModel;
using Xamarin.Forms;

namespace BubbleTreeAppTest.BubbleTree.View
{
    public class BubbleTreeView<T> : ContentPage where T : ITreeElement
    {
        private StackLayout Container;
        private StackLayout SearchContainer;
        private Grid BubbleContainer;
       
        
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public Entry SearchEntry = new Entry()
        {
            //TODO: Style
           // Style = GespolStyles.Entries(),
            Text = "",
            TextColor = Color.White,
            Placeholder = "Buscando en la raiz...",
            PlaceholderColor = Color.Silver
        };

        public ToolbarItem GoToParentToolbarItem;

        public BubbleNodes<T> BubbleNodes;

        private BaseNode<T> _currentSearchFilterNode;

        public BaseNode<T> CurrentSearchFilterNode
        {
            get { return _currentSearchFilterNode; }
            set
            {
                _currentSearchFilterNode = value;

                SearchEntry.TextChanged -= OnTextChangedDoSearch();
                SearchEntry.Text = "";
                SearchEntry.TextChanged += OnTextChangedDoSearch();

                SearchEntry.Placeholder = _currentSearchFilterNode != null ? $"Buscando en...{_currentSearchFilterNode.Data?.Description}" : "Buscando en la raiz...";
                
                RefreshNodeFilter();
               
                if (_currentSearchFilterNode != null && !ToolbarItems.Contains(GoToParentToolbarItem)) ToolbarItems.Add(GoToParentToolbarItem);
            }
        }

        private void RefreshNodeFilter()
        {
            if (_currentSearchFilterNode == null)
            {
                ToolbarItems.Remove(GoToParentToolbarItem);
                Container.Children.Remove(BubbleContainer);
                BubbleContainer = new Grid();
                PrepareBubbleContainer(BubbleNodes.RawList.OfType<RootNode<T>>().Count(), 3);
                Container.Children.Add(BubbleContainer);
            }
            else if (_currentSearchFilterNode is InternalNode<T>)
            {
                Container.Children.Remove(BubbleContainer);
                BubbleContainer = new Grid();
                PrepareBubbleContainer(((InternalNode<T>)_currentSearchFilterNode).Children.Count, 3, ((InternalNode<T>)_currentSearchFilterNode).Children);
                Container.Children.Add(BubbleContainer);
            }
            else if (_currentSearchFilterNode is RootNode<T>)
            {
                Container.Children.Remove(BubbleContainer);
                BubbleContainer = new Grid();
                PrepareBubbleContainer(((RootNode<T>)_currentSearchFilterNode).Children.Count, 3, ((RootNode<T>)_currentSearchFilterNode).Children);
                Container.Children.Add(BubbleContainer);
            }
        }

        public BubbleTreeView(List<T> sourceItems)
        {
            var scrollContainer = new ScrollView();

            scrollContainer.Scrolled += (sender, args) =>
            {
                var scrollSpace = scrollContainer.ContentSize.Height - scrollContainer.Height - 20;
                if (scrollSpace <= args.ScrollY)
                {
                    Container.Children.Add(new Label() {TextColor = Color.White,Text = "Another Element!"});
                    Container.Children.Add(new Label() { TextColor = Color.White, Text = "Another Element2!" });
                    Container.Children.Add(new Label() { TextColor = Color.White, Text = "Another Element3!" });
                }
            };

            Container = new StackLayout();
            scrollContainer.Content = Container;
            SearchContainer = new StackLayout();
            BubbleContainer = new Grid();

            Container.Children.Add(SearchContainer);
            Container.Children.Add(BubbleContainer);

            BubbleNodes = NodeFactory<T>.CreateFromSource(sourceItems, node => node.Data.Description, orderDescending: true);
            SearchEntry.TextChanged += OnTextChangedDoSearch();

            ToolbarItems.Add(new ToolbarItem("To Root", "ToRootIcon.png", () => CurrentSearchFilterNode = null) { Text = "Raiz" });
            GoToParentToolbarItem = new ToolbarItem("To parent", "ToParentIcon.png", () =>
            {
                if (CurrentSearchFilterNode is RootNode<T>)
                {
                    CurrentSearchFilterNode = null;
                }
                if (CurrentSearchFilterNode is InternalNode<T>)
                {
                    CurrentSearchFilterNode = ((InternalNode<T>)CurrentSearchFilterNode).Parent;
                }
                if (CurrentSearchFilterNode is LeafNode<T>)
                {
                    CurrentSearchFilterNode = ((LeafNode<T>)CurrentSearchFilterNode).Parent;
                }

            })
            { Text = "Subir" };

            Content = scrollContainer;
            SearchContainer.Children.Add(SearchEntry);

            var rootNodesCount = BubbleNodes.RawList.OfType<RootNode<T>>().Count();
            PrepareBubbleContainer(rootNodesCount, 3);

        }

        private EventHandler<TextChangedEventArgs> OnTextChangedDoSearch()
        {
            return (sender, args) =>
            {
                Interlocked.Exchange(ref cancellationTokenSource, new CancellationTokenSource()).Cancel();
                Task.Delay(TimeSpan.FromMilliseconds(125), cancellationTokenSource.Token).ContinueWith(delegate
                {
                    SearchInBubbleTree(displayLimit: 15);
                },
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.FromCurrentSynchronizationContext()
                    );

            };
        }

        private void SearchInBubbleTree(int displayLimit)
        {
            IEnumerable<BaseNode<T>> searchResultNodes;

            Container.Children.Remove(BubbleContainer);
            BubbleContainer = new Grid();
            if (SearchEntry.Text == "" && _currentSearchFilterNode == null)
            {
                PrepareBubbleContainer(BubbleNodes.RawList.OfType<RootNode<T>>().Count(), 3);
            }
            else
            {

                if (_currentSearchFilterNode == null)
                {
                    searchResultNodes =
                        BubbleNodes.RawList.Where(
                            node => node.Data.Description != null && node.Data.Description.ToUpper().Contains(SearchEntry.Text.ToUpper()))
                            .Take(displayLimit).Reverse();
                }
                else
                {
                    var allChildren = BubbleNodes.GetAllChildrenRaw(CurrentSearchFilterNode);
                    searchResultNodes = allChildren.Where(
                        node => node.Data.Description != null && node.Data.Description.ToUpper().Contains(SearchEntry.Text.ToUpper()))
                        .Take(displayLimit).Reverse();
                }
                PrepareBubbleContainer(searchResultNodes.Count(), 3, searchResultNodes);
            }

            Container.Children.Add(BubbleContainer);


        }

        private void PrepareBubbleContainer(int nodesCount, int numberOfColums, IEnumerable<BaseNode<T>> nodes)
        {
            int nRows = (int) Math.Ceiling((float) nodesCount/numberOfColums);
            for (var i = 0; i < nRows; i++)
            {
                BubbleContainer.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
            }

            for (var i = 0; i < (nodesCount < numberOfColums ? nodesCount : numberOfColums); i++)
            {
                BubbleContainer.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            PopulateBubbleContainer(nRows, numberOfColums, nodes);

        }

        private void PrepareBubbleContainer(int nodesCount, int numberOfColums)
        {
            int nRows = (int) Math.Ceiling((float) nodesCount/numberOfColums);
            for (var i = 0; i < nRows; i++)
            {
                BubbleContainer.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
            }

            for (var i = 0; i < (nodesCount < numberOfColums ? nodesCount : numberOfColums); i++)
            {
                BubbleContainer.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            PopulateBubbleContainer(nRows, numberOfColums);

        }

        /// <summary>
        /// Populates the bubble container with Root nodes.
        /// </summary>
        /// <param name="numberOfRows">The number of rows that bubble container have.</param>
        /// <param name="numberOfColums">The number of columns that bubble container have.</param>
        private void PopulateBubbleContainer(int numberOfRows, int numberOfColums)
        {
            var rootNodes = BubbleNodes.RawList.OfType<RootNode<T>>();
            int rowIterator = 0;
            int columnIterator = 0;
            
            foreach (var node in rootNodes.OrderBy(node => node.Data.Description))
            {
                BubbleContainer.Children.Add(new Bubble<RootNode<T>, T>(node, new Button()
                {
                    //TODO: Style
                    TextColor = Color.White,
                    //Style = GespolStyles.ButtonStyleGrua(),
                    FontSize = 14,
                    Text = node.Data.Description,
                    HeightRequest = 100,
                    WidthRequest = 100,
                    BorderColor = Color.Navy,
                    BorderWidth = 3.0,
                    BorderRadius = 30,
                    BackgroundColor = Color.FromRgb(0, 35, 180),
                    Command = new Command(() => CurrentSearchFilterNode = node)
                }), columnIterator, rowIterator);
                columnIterator++;
                columnIterator %= numberOfColums;
                if (columnIterator == 0)
                {
                    rowIterator++;
                    rowIterator %= numberOfRows;
                }
            }
        }

        private void PopulateBubbleContainer(int numberOfRows, int numberOfColums, IEnumerable<BaseNode<T>> nodes)
        {

            int rowIterator = 0;
            int columnIterator = 0;
            foreach (var node in (SearchEntry.Text == "") ? nodes.OrderBy(node => node.Data.Description) : nodes)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    InsertNode(node, rowIterator, columnIterator).Wait();
                    columnIterator++;
                    columnIterator %= numberOfColums;
                    if (columnIterator == 0)
                    {
                        rowIterator++;
                        rowIterator %= numberOfRows;
                    }
                });
                
            }
        }

        private async Task<bool> InsertNode(BaseNode<T> node, int rowIterator, int columnIterator)
        {
            var bubbleButton = new Button()
            {
                //TODO: Style
                //Style = GespolStyles.ButtonStyleGrua(),
                FontSize = 14,
                Text = node.Data.Description,
                TextColor = Color.White,
                HeightRequest = 100,
                WidthRequest = 100,
                BorderColor = Color.Navy,
                BorderWidth = 3.0,
                BorderRadius = 30,
                BackgroundColor = Color.FromRgb(0, 35, 180),
                Command = new Command(() => CurrentSearchFilterNode = node)
            };
            if (node is RootNode<T>)
            {
                bubbleButton.BackgroundColor = Color.FromRgb(5,5,120);
            }
            else if (node is LeafNode<T>)
            {
                bubbleButton.BackgroundColor = Color.FromRgb(30, 120, 200);
                bubbleButton.BorderRadius = 5;
                bubbleButton.Command = new Command(() =>
                {
                    ((BubbleTreeViewModel<T>)BindingContext).SelectedItem = node.Data;
                    OnPropertyChanged(nameof(BubbleTreeViewModel<T>.SelectedItem));
                    Navigation.PopAsync();
                });
            }
            BubbleContainer.Children.Add(new Bubble<BaseNode<T>, T>(node, bubbleButton), columnIterator, rowIterator);

            return true;

        }

        internal class Bubble<TNode, TEntity> : StackLayout where TNode : BaseNode<TEntity> where TEntity : ITreeElement
        {
            public Button SelectionButton;
            public TNode Node;

            public Bubble(TNode node, Button selectionButton)
            {
                Node = node;
                SelectionButton = selectionButton;

                Padding = new Thickness(1, 1, 1, 1);

                Children.Add(SelectionButton);
                AppearAnimation();

            }

            private async void AppearAnimation()
            {
                var rand = new Random(DateTime.Now.Millisecond);
                await SelectionButton.ScaleTo(0, 0U);
                await SelectionButton.ScaleTo(0, 100U);
                await SelectionButton.FadeTo(0.0f, 0U);
                await SelectionButton.FadeTo(0.0f, 100U);
                await SelectionButton.FadeTo(1.0, 800U);
                await SelectionButton.ScaleTo(1.2, (uint) rand.Next(300, 600), Easing.CubicInOut);
                await SelectionButton.ScaleTo(0.9, 150U, Easing.Linear);
                await SelectionButton.ScaleTo(1.0, 150U, Easing.Linear);
            }

        }
    }
}