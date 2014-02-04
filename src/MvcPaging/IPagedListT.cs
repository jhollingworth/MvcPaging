using System.Collections.Generic;

namespace MvcPaging
{
    public interface IPagedList<T> : IPagedList, IList<T>
	{
	}
}