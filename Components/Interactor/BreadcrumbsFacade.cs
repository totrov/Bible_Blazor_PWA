using MudBlazor;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public class BreadcrumbsFacade
    {
        public Action OnUpdateRequired;
        public string Background { get => _background; }
        private void UpdateRequired() => OnUpdateRequired?.Invoke();
        private List<BreadcrumbItem> items;
        private Dictionary<int, Action> actions;
        private Dictionary<BreadcrumbRecord, BreadcrumbItem> breadcrumbMap;
        private int nextKey;
        private string _background;

        public BreadcrumbsFacade(string background)
        {
            items = new List<BreadcrumbItem>();
            actions = new Dictionary<int, Action>();
            breadcrumbMap = new Dictionary<BreadcrumbRecord, BreadcrumbItem>();
            nextKey = 0;
            _background = background;
        }
        public void SetBreadcrumbs(IEnumerable<BreadcrumbRecord> breadcrumbRecords)
        {
            if (breadcrumbRecords == null || breadcrumbRecords.Count() == 0)
                return;
            items.Clear();
            actions.Clear();
            nextKey = 0;
            foreach (var breadcrumbRecord in breadcrumbRecords)
            {
                var breadcrumbItem = new BreadcrumbItem(breadcrumbRecord.Text, nextKey.ToString(), false, breadcrumbRecord.Icon);
                breadcrumbMap.Add(breadcrumbRecord, breadcrumbItem);

                items.Add(breadcrumbItem);
                actions[nextKey] = breadcrumbRecord.Action;
                breadcrumbRecord.OnTextChanged += (newValue, breadcrumb) =>
                {
                    int index = items.IndexOf(breadcrumbMap[breadcrumb]);
                    items.Remove(breadcrumbMap[breadcrumb]);
                    breadcrumbMap[breadcrumb] = new BreadcrumbItem(
                        breadcrumb.Text, 
                        breadcrumbMap[breadcrumb].Href,
                        breadcrumbMap[breadcrumb].Disabled,
                        breadcrumb.Icon);
                    items.Insert(index, breadcrumbMap[breadcrumb]);
                    UpdateRequired();
                };
                nextKey++;
            }
        }

        public Action GetAction(string key) => actions[int.Parse(key)];
        public List<BreadcrumbItem> GetBreadcrumbs()
        {
            if (items == null || items.Count() == 0)
                return null;

            var last = items.Last();
            
            items.Remove(last);
            var keyForMap = breadcrumbMap.First(x => x.Value == last).Key;
            breadcrumbMap.Remove(keyForMap);

            var newItem = new BreadcrumbItem(last.Text, last.Href, true, last.Icon);
            breadcrumbMap.Add(keyForMap, newItem);
            items.Add(newItem);

            return items;
        }
        public class BreadcrumbRecord
        {
            private string text;

            public event Action<string, BreadcrumbRecord> OnTextChanged;
            void TextChanged(string newValue) => OnTextChanged?.Invoke(newValue, this);
            public string Text { get => text; set { text = value; TextChanged(value); } }
            public Action Action { get; set; }
            public string Icon { get; set; }
        }
    }
}
