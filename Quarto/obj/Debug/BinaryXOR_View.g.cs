﻿#pragma checksum "..\..\BinaryXOR_View.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A14EFFA0D0D223ABDBC56074E6B1B5AD20DFB648"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Quarto;
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


namespace Quarto {
    
    
    /// <summary>
    /// BinaryXOR_View
    /// </summary>
    public partial class BinaryXOR_View : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\BinaryXOR_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt1;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\BinaryXOR_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt2;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\BinaryXOR_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkRow;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\BinaryXOR_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkColumn;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\BinaryXOR_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkDiagonal;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\BinaryXOR_View.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCalc;
        
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
            System.Uri resourceLocater = new System.Uri("/Quarto;component/binaryxor_view.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\BinaryXOR_View.xaml"
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
            this.txt1 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.txt2 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.chkRow = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 4:
            this.chkColumn = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.chkDiagonal = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 6:
            this.btnCalc = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\BinaryXOR_View.xaml"
            this.btnCalc.Click += new System.Windows.RoutedEventHandler(this.btnCalc_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

