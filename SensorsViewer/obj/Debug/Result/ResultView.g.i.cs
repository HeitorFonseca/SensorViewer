﻿#pragma checksum "..\..\..\Result\ResultView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "36106EEDC4F6328C5BD86E5883D890670F37E559"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GalaSoft.MvvmLight.Command;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using MahApps.Metro.Controls;
using SensorsViewer.Result;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SensorsViewer.Result {
    
    
    /// <summary>
    /// ResultView
    /// </summary>
    public partial class ResultView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\Result\ResultView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HelixToolkit.Wpf.HelixViewport3D viewPort3d;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Result\ResultView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HelixToolkit.Wpf.SharpDX.Viewport3DX ViewPort3DX;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Result\ResultView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HelixToolkit.Wpf.SharpDX.PointGeometryModel3D points;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\Result\ResultView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MahApps.Metro.Controls.ToggleSwitch enabledSwitch;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SensorsViewer;component/result/resultview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Result\ResultView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.viewPort3d = ((HelixToolkit.Wpf.HelixViewport3D)(target));
            return;
            case 2:
            this.ViewPort3DX = ((HelixToolkit.Wpf.SharpDX.Viewport3DX)(target));
            return;
            case 3:
            this.points = ((HelixToolkit.Wpf.SharpDX.PointGeometryModel3D)(target));
            return;
            case 4:
            this.enabledSwitch = ((MahApps.Metro.Controls.ToggleSwitch)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

