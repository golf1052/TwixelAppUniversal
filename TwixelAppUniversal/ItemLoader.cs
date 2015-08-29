using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace TwixelAppUniversal
{
    public class ItemLoader
    {
        bool currentlyLoading;
        bool endOfList;
        public int Offset { get; private set; }

        Func<Task> loadFunction;
        ScrollViewer scrollViewer;
        ProgressBar progressBar;

        public ItemLoader(Func<Task> loadFunc, ScrollViewer scrollViewer, ProgressBar progressBar)
        {
            currentlyLoading = false;
            endOfList = false;
            Offset = 0;
            loadFunction = loadFunc;
            this.scrollViewer = scrollViewer;
            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            this.progressBar = progressBar;
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (scrollViewer.ScrollableHeight == scrollViewer.VerticalOffset)
            {
                if (!currentlyLoading && !endOfList)
                {
                    await loadFunction.Invoke();
                }
            }
        }
        
        public void StartLoading()
        {
            currentlyLoading = true;
            HelperMethods.EnableIndeterminateProgressBar(progressBar);
        }

        public bool CheckForEnd<T>(IEnumerable<T> items)
        {
            HelperMethods.DisableIndeterminateProgressBar(progressBar);
            if (items.Count() == 0)
            {
                endOfList = true;
                currentlyLoading = false;
                return true;
            }
            return false;
        }

        public void EndLoading(int offset)
        {
            Offset += offset;
            currentlyLoading = false;
            HelperMethods.DisableIndeterminateProgressBar(progressBar);
        }
    }
}
