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
    
    #line 3 "..\..\Pages\EventBusPartial.cshtml"
    using System.Collections.Generic;
    
    #line default
    #line hidden
    using System.Linq;
    using System.Text;
    
    #line 2 "..\..\Pages\EventBusPartial.cshtml"
    using Gaucho.Dashboard;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Pages\EventBusPartial.cshtml"
    using Gaucho.Dashboard.Pages;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    public partial class EventBusPartial : RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");





WriteLiteral("\r\n<div>\r\n");


            
            #line 8 "..\..\Pages\EventBusPartial.cshtml"
     foreach (var pipeline in ServerMonitor.GetPipelineMetrics())
    {
        
            
            #line default
            #line hidden
            
            #line 10 "..\..\Pages\EventBusPartial.cshtml"
   Write(Html.RenderPartial(new PipelinePartial(pipeline)));

            
            #line default
            #line hidden
            
            #line 10 "..\..\Pages\EventBusPartial.cshtml"
                                                          
    }

            
            #line default
            #line hidden
WriteLiteral("</div>");


        }
    }
}
#pragma warning restore 1591