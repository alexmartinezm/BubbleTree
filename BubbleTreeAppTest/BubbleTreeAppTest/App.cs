﻿using System;
using System.Collections.Generic;
using System.Globalization;
using BubbleTreeComponent.ViewModel;
using Xamarin.Forms;

namespace BubbleTreeAppTest
{
    public class App : Application
    {
        public App()
        {

            var defaultBubbleTreeLabel = new Label
            {
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Red

            };
            var defaultBubbleTree = new BubbleTree<BubbleTreeItem>();

            defaultBubbleTree
                .BeginConfiguration()
                    .SetSourceItems(GetBubbleTreeItemList())
                .EndConfiguration();


            var customizedBubbleTreeLabel = new Label
            {
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Red

            };
            var customizedBubbleTree = new BubbleTree<BubbleTreeItem>();

            customizedBubbleTree
                .BeginConfiguration()
                    .SetSourceItems(GetBubbleTreeItemList())
                        .BeginViewConfiguration()
                            .SetBackgroundColor(Color.FromRgb(180, 180, 130))
                            .SetTitle("Bubble Tree Title")
                            .OnSelectedItemChanged(() => customizedBubbleTreeLabel.TextColor = Color.Lime)
                                .BeginToolBarItemsConfiguration()
                                    .SetGoRootIconFile("ToRootIcon.png")
                                    .SetGoUpIconFile("ToParentIcon.png")
                                    .SetGoRootText("Go Root")
                                    .SetGoUpText("Go Up")
                                .EndToolBarItemsConfiguration()
                                .BeginSearchEntryConfiguration()
                                    .SetPlaceHolderColor(Color.FromRgb(20, 20, 20))
                                    .SetTextColor(Color.Black)
                                    .SetBackgroundColor(Color.FromRgb(170, 170, 120))
                                    .SetFontSize(14)
                                    .SetKeyBoard(Keyboard.Text)
                                    .AddCompletedDelegate((sender, args) => {customizedBubbleTreeLabel.TextColor = Color.Blue;})
                                    .SetSearchingOnRootPlaceholderText("Searching on whole tree")
                                    .SetSearchingInPlaceholderText("Searching inside of ")
                                .EndSearchEntryConfiguration()
                                .BeginGridConfiguration()
                                    .SetColumnDisplayCount(3)
                                    .SetBubbleDisplayLimitPerSearch(15)
                                .EndGridConfiguration()
                                .BeginRootNodesConfiguration()
                                    .SetTextColor(Color.White)
                                    .SetFontSize(14)
                                    .SetBorderColor(Color.FromRgb(120, 0, 0))
                                    .SetBorderWidth(3)
                                    .SetBorderRadius(50)
                                    .SetBackgroundColor(Color.FromRgb(60, 0, 0))
                                    .SetImageConfiguration("RootNodeIcon.png", Button.ButtonContentLayout.ImagePosition.Top)
                                .EndRootNodesConfiguration()
                                .BeginInternalNodesConfiguration()
                                    .SetTextColor(Color.White)
                                    .SetFontSize(13)
                                    .SetBorderColor(Color.FromRgb(0, 120, 0))
                                    .SetBorderWidth(3)
                                    .SetBorderRadius(30)
                                    .SetBackgroundColor(Color.FromRgb(0, 60, 0))
                                    .SetImageConfiguration("InternalNodeIcon.png", Button.ButtonContentLayout.ImagePosition.Right)
                                .EndInternalNodesConfiguration()
                                .BeginLeafNodesConfiguration()
                                    .SetTextColor(Color.White)
                                    .SetFontSize(12)
                                    .SetBorderColor(Color.FromRgb(0, 0, 120))
                                    .SetBorderWidth(3)
                                    .SetBorderRadius(5)
                                    .SetBackgroundColor(Color.FromRgb(0, 0, 60))
                                    .SetImageConfiguration("LeafNodeIcon.png", Button.ButtonContentLayout.ImagePosition.Bottom)
                                .EndLeafNodesConfiguration()
                        .EndViewConfiguration()
                .EndConfiguration();





            defaultBubbleTreeLabel.BindingContext = defaultBubbleTree;
            defaultBubbleTreeLabel.SetBinding(Label.TextProperty, nameof(BubbleTree<BubbleTreeItem>.SelectedItem), BindingMode.Default, new BubbleTreeConverter());


            customizedBubbleTreeLabel.BindingContext = customizedBubbleTree;
            customizedBubbleTreeLabel.SetBinding(Label.TextProperty, nameof(BubbleTree<BubbleTreeItem>.SelectedItem),BindingMode.Default,new BubbleTreeConverter());

           


            // The root page of your application
            var content = new ContentPage
            {
                Title = "BubbleTreeAppTest",
               BackgroundColor = Color.Black,
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            FontSize = 30,
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Bubble Tree Component Test!",
                            TextColor = Color.White
                        },

                        defaultBubbleTreeLabel,

                        customizedBubbleTreeLabel
                    }
                }
            };

            defaultBubbleTreeLabel.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () => await content.Navigation.PushAsync(defaultBubbleTree.GetView()))
            });

            customizedBubbleTreeLabel.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () => await content.Navigation.PushAsync(customizedBubbleTree.GetView()))
            });

            MainPage = new NavigationPage(content);
            
        }

        private List<BubbleTreeItem> GetBubbleTreeItemList()
        {
            return new List<BubbleTreeItem>()
            {
                new BubbleTreeItem()
                {
                    Description = "1. Root Node First",
                    ElementId = 1,
                    ParentElementId = null
                },
                new BubbleTreeItem()
                {
                    Description = "2. Root Node Second",
                    ElementId = 2,
                    ParentElementId = null
                },
                new BubbleTreeItem()
                {
                    Description = "3. Root Node Third",
                    ElementId = 3,
                    ParentElementId = null
                },
                new BubbleTreeItem()
                {
                    Description = "4. Root Node Fourth",
                    ElementId = 4,
                    ParentElementId = null
                },
                new BubbleTreeItem()
                {
                    Description = "5. Root Node Fifth",
                    ElementId = 5,
                    ParentElementId = null
                },
                new BubbleTreeItem()
                {
                    Description = "1.1 Internal Node",
                    ElementId = 6,
                    ParentElementId = 1
                },
                new BubbleTreeItem()
                {
                    Description = "1.2. Internal Node",
                    ElementId = 7,
                    ParentElementId = 1
                },
                new BubbleTreeItem()
                {
                    Description = "1.3 Internal Node",
                    ElementId = 8,
                    ParentElementId = 1
                },
                new BubbleTreeItem()
                {
                    Description = "2.1 Internal Node",
                    ElementId = 9,
                    ParentElementId = 2
                },
                new BubbleTreeItem()
                {
                    Description = "2.2 Internal Node",
                    ElementId = 10,
                    ParentElementId = 2
                },
                new BubbleTreeItem()
                {
                    Description = "3.1 Internal Node",
                    ElementId = 11,
                    ParentElementId = 3
                },
                new BubbleTreeItem()
                {
                    Description = "4.1 Internal Node",
                    ElementId = 12,
                    ParentElementId = 4
                },
                new BubbleTreeItem()
                {
                    Description = "5.1 Internal Node",
                    ElementId = 13,
                    ParentElementId = 5
                },
                new BubbleTreeItem()
                {
                    Description = "1.1.1 Leaf Node",
                    ElementId = 14,
                    ParentElementId = 6
                },
                new BubbleTreeItem()
                {
                    Description = "1.2.1 Leaf Node",
                    ElementId = 15,
                    ParentElementId = 7
                },new BubbleTreeItem()
                {
                    Description = "1.3.1 Leaf Node",
                    ElementId = 16,
                    ParentElementId = 8
                },new BubbleTreeItem()
                {
                    Description = "2.1.1 Leaf Node",
                    ElementId = 17,
                    ParentElementId = 9
                },new BubbleTreeItem()
                {
                    Description = "2.2.1 Leaf Node",
                    ElementId = 18,
                    ParentElementId = 10
                },new BubbleTreeItem()
                {
                    Description = "3.1.1 Leaf Node",
                    ElementId = 19,
                    ParentElementId = 11
                },new BubbleTreeItem()
                {
                    Description = "4.1.1 Leaf Node",
                    ElementId = 20,
                    ParentElementId = 12
                },new BubbleTreeItem()
                {
                    Description = "5.1.1 Leaf Node",
                    ElementId = 21,
                    ParentElementId = 13
                },
                
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }

    public class BubbleTreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Click here to select an item on a bubble tree component!";
            return ((BubbleTreeItem) value).Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
