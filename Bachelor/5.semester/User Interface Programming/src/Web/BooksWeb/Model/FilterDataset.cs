using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;

namespace BooksWeb.Model
{
    public delegate Task<IQueryable<T>> Query<T>();

    public class FilterDataSet<T> : DotvvmViewModelBase
    {
        private Query<T> _query;

        public GridViewDataSet<T> DataSet { get; set; } = new GridViewDataSet<T> { PagingOptions = new PagingOptions { PageSize = 10 } };
        public string Filter { get; set; }
        public int Page { get; set; }

        public override async Task PreRender()
        {
            if (DataSet.IsRefreshRequired)
            {
                var queryable = await _query.Invoke();
                DataSet.LoadFromQueryable(queryable);
                Page = DataSet.PagingOptions.PageIndex;
            }
            await base.PreRender();
        }

        public void RequestFilter()
        {
            DataSet.GoToFirstPage();
            DataSet.RequestRefresh();
        }

        public void RequestRefresh() => DataSet.RequestRefresh();

        public void Set(Query<T> query) => _query = query;
    }
}