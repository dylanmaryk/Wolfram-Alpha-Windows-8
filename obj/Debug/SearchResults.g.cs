﻿

#pragma checksum "C:\Users\Dylan\Documents\Visual Studio 2012\Projects\ModernAlpha\SearchResults.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9D22E9E1834C04AEC8A0190C981B28D3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModernAlpha
{
    partial class SearchResults : global::ModernAlpha.Common.LayoutAwarePage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 27 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.resultsPanel_Tapped;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 53 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.GoBack;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 57 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).KeyDown += this.textBoxSearch_KeyDown;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 58 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.assumptionsBox_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 112 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.Controls.AppBar)(target)).Closed += this.appBar_Closed;
                 #line default
                 #line hidden
                #line 112 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.Controls.AppBar)(target)).Opened += this.appBar_Opened;
                 #line default
                 #line hidden
                #line 112 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.StartLayoutUpdates;
                 #line default
                 #line hidden
                #line 112 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Unloaded += this.StopLayoutUpdates;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 120 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.viewOnline_Click;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 115 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.copyImage_Click;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 116 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.copyText_Click;
                 #line default
                 #line hidden
                break;
            case 9:
                #line 117 "..\..\SearchResults.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.saveImage_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


