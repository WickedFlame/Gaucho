﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Dashboard
{
    public class HtmlHelper
    {
        private readonly RazorPage _page;

        public HtmlHelper(RazorPage page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            _page = page;
        }

        public NonEscapedString RenderPartial(RazorPage partialPage)
        {
            partialPage.Assign(_page);
            return new NonEscapedString(partialPage.ToString());
        }
    }
}
