using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace MvcPaging
{
	public class Pager
	{
		private ViewContext viewContext;
		private readonly int pageSize;
		private readonly int currentPage;
		private readonly int totalItemCount;
		private readonly RouteValueDictionary linkWithoutPageValuesDictionary;
		private readonly AjaxOptions ajaxOptions;

		public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary, AjaxOptions ajaxOptions)
		{
			this.viewContext = viewContext;
			this.pageSize = pageSize;
			this.currentPage = currentPage;
			this.totalItemCount = totalItemCount;
			this.linkWithoutPageValuesDictionary = valuesDictionary;
			this.ajaxOptions = ajaxOptions;
		}

		public HtmlString RenderHtml()
		{
			var pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
			const int nrOfPagesToDisplay = 10;

			var sb = new StringBuilder();

		   const string li = "<li class=\"{0}\"><a href=\"{1}\">{2}</a>";
		   var dotDotDot = string.Format(li, "disabled", "#", "...");


         sb.AppendLine("<div class=\"pagination\"><ul>");

		   var previousEnabled = currentPage > 1;
         sb.AppendFormat(li, "prev " + (previousEnabled ? "" : "disabled"), GetPageUrl(currentPage - 1), "&larr; Previous");

			var start = 1;
			var end = pageCount;

			if (pageCount > nrOfPagesToDisplay)
			{
				var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
				var below = (currentPage - middle);
				var above = (currentPage + middle);

				if (below < 4)
				{
					above = nrOfPagesToDisplay;
					below = 1;
				}
				else if (above > (pageCount - 4))
				{
					above = pageCount;
					below = (pageCount - nrOfPagesToDisplay);
				}

				start = below;
				end = above;
			}

			if (start > 3)
			{
				sb.AppendFormat(li, "", GetPageUrl(1), "1");
            sb.AppendFormat(li, "", GetPageUrl(2), "2");
			   sb.AppendLine(dotDotDot);
			}
			
			for (var i = start; i <= end; i++)
			{
				if (i == currentPage || (currentPage <= 0 && i == 0))
				{
					sb.AppendFormat(li, "active", "#", i);
				}
				else
				{
               sb.AppendFormat(li, "", GetPageUrl(i), i);
				}
			}
			if (end < (pageCount - 3))
			{
				sb.AppendLine(dotDotDot);
            sb.AppendFormat(li, "", GetPageUrl(pageCount - 1), pageCount - 1);
            sb.AppendFormat(li, "", GetPageUrl(pageCount), pageCount);
			}

		   var isNext = currentPage < pageCount;

		   sb.AppendFormat(li, "next " + (isNext ? "" : "disabled"), GetPageUrl(currentPage + 1), "Next &raquo;");
		   sb.Append("</ul></div>");

			return new HtmlString(sb.ToString());
		}
      
      private string GetPageUrl(int pageNumber)
      {
         var pageLinkValueDictionary = new RouteValueDictionary(linkWithoutPageValuesDictionary) { { "page", pageNumber } };

         // To be sure we get the right route, ensure the controller and action are specified.
         var routeDataValues = viewContext.RequestContext.RouteData.Values;
         if (!pageLinkValueDictionary.ContainsKey("controller") && routeDataValues.ContainsKey("controller"))
         {
            pageLinkValueDictionary.Add("controller", routeDataValues["controller"]);
         }
         if (!pageLinkValueDictionary.ContainsKey("action") && routeDataValues.ContainsKey("action"))
         {
            pageLinkValueDictionary.Add("action", routeDataValues["action"]);
         }

         // 'Render' virtual path.
         var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(viewContext.RequestContext, pageLinkValueDictionary);

         if (virtualPathForArea == null)
            return null;

         return virtualPathForArea.VirtualPath;
      }
	}
}