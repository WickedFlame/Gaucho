﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gaucho.Dashboard.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    #line 2 "..\..\Pages\LayoutPage.cshtml"
    using System.Reflection;
    
    #line default
    #line hidden
    using System.Text;
    
    #line 3 "..\..\Pages\LayoutPage.cshtml"
    using Gaucho.Dashboard;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    public partial class LayoutPage : RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");




WriteLiteral("\r\n<!DOCTYPE html>\r\n\r\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head" +
">\r\n    <title>");


            
            #line 10 "..\..\Pages\LayoutPage.cshtml"
      Write(DashboardOptions.Title);

            
            #line default
            #line hidden
WriteLiteral(" - ");


            
            #line 10 "..\..\Pages\LayoutPage.cshtml"
                                Write(Title);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <meta ch" +
"arset=\"utf-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-s" +
"cale=1.0\">\r\n    <meta name=\"robots\" content=\"none\">\r\n    ");



WriteLiteral("\r\n");


            
            #line 17 "..\..\Pages\LayoutPage.cshtml"
       var version = GetType().GetTypeInfo().Assembly.GetName().Version; 

            
            #line default
            #line hidden
WriteLiteral("    <link rel=\"stylesheet\" href=\"");


            
            #line 18 "..\..\Pages\LayoutPage.cshtml"
                            Write(Url.To($"/css{version.Major}{version.Minor}{version.Build}"));

            
            #line default
            #line hidden
WriteLiteral("\">\r\n</head>\r\n<body>\r\n    <div class=\"header\">\r\n        <div></div>\r\n        <div " +
"class=\"container\">\r\n            <h1>");


            
            #line 24 "..\..\Pages\LayoutPage.cshtml"
           Write(DashboardOptions.Title);

            
            #line default
            #line hidden
WriteLiteral("</h1>\r\n        </div>\r\n        <div></div>\r\n    </div>\r\n\r\n<div class=\"main-wrappe" +
"r\">\r\n    <div></div>\r\n    <div class=\"container\" style=\"margin-bottom: 20px;\">\r\n" +
"        <h2>");


            
            #line 32 "..\..\Pages\LayoutPage.cshtml"
       Write(Title);

            
            #line default
            #line hidden
WriteLiteral("</h2>\r\n        <div>\r\n            ");


            
            #line 34 "..\..\Pages\LayoutPage.cshtml"
       Write(RenderBody());

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div></div>\r\n</div>\r\n<script>\r\n\tvar gauchoConfi" +
"g = {\r\n\t\tpollUrl: \"");


            
            #line 41 "..\..\Pages\LayoutPage.cshtml"
        Write(Url.To("/metrics"));

            
            #line default
            #line hidden
WriteLiteral("\",\r\n        clearUrl: \"");


            
            #line 42 "..\..\Pages\LayoutPage.cshtml"
               Write(Url.To("/metrics/clear"));

            
            #line default
            #line hidden
WriteLiteral("\",\r\n\t\tpollInterval: 2000\r\n\t}\r\n\r\n</script>\r\n<script src=\"");


            
            #line 47 "..\..\Pages\LayoutPage.cshtml"
        Write(Url.To($"/js{version.Major}{version.Minor}{version.Build}"));

            
            #line default
            #line hidden
WriteLiteral("\" type=\"module\"></script>\r\n</body>\r\n</html>");


        }
    }
}
#pragma warning restore 1591
