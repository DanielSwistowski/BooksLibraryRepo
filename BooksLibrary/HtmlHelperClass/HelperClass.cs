using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BooksLibrary.HtmlHelperClass
{
    public static class HelperClass
    {
        public static MvcHtmlString BoolToYesOrNo(this HtmlHelper htmlHelper, bool yesNo)
        {
            var text = yesNo ? "Tak" : "Nie";
            return new MvcHtmlString(text);
        }

        public static MvcHtmlString GenderTranstateToPL(this HtmlHelper htmlHelper, BooksLibrary.Business.Models.Gender gender)
        {
            var text = "";

            if (gender == BooksLibrary.Business.Models.Gender.Male)
                text = "Mężczyzna";
            else
                text = "Kobieta";

            return new MvcHtmlString(text);
        }

        public static MvcHtmlString QuantityToYesOrNo(this HtmlHelper htmlHelper, int quantity)
        {
            var text = "";
            if (quantity > 0)
            {
                text = "Tak";
            }
            else
            {
                text = "Nie";
            }

            return new MvcHtmlString(text);
        }
    }
}