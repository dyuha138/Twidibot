﻿#pragma checksum "..\..\..\MainWin.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "469CF0589F611A18BDE28520FDF8F7DFCE96C8BC72A16E3B905B39836DE80B05"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using ControlzEx;
using ControlzEx.Behaviors;
using ControlzEx.Controls;
using ControlzEx.Theming;
using ControlzEx.Windows.Shell;
using MahApps.Metro;
using MahApps.Metro.Accessibility;
using MahApps.Metro.Actions;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.Behaviors;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Converters;
using MahApps.Metro.Markup;
using MahApps.Metro.Theming;
using MahApps.Metro.ValueBoxes;
using MaterialDesignThemes.MahApps;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
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
using Twidibot;


namespace Twidibot {
    
    
    /// <summary>
    /// MainWin
    /// </summary>
    public partial class MainWin : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\MainWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid GridMenu;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\MainWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bMenuChatHstr;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\MainWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bMenuCom;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\MainWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bMenuStatus;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\MainWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bMenuAppSet;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\MainWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Frame FrameV;
        
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
            System.Uri resourceLocater = new System.Uri("/Twidibot;component/mainwin.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWin.xaml"
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
            this.GridMenu = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.bMenuChatHstr = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\..\MainWin.xaml"
            this.bMenuChatHstr.Click += new System.Windows.RoutedEventHandler(this.bMenu_ChatHstr_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.bMenuCom = ((System.Windows.Controls.Button)(target));
            
            #line 37 "..\..\..\MainWin.xaml"
            this.bMenuCom.Click += new System.Windows.RoutedEventHandler(this.bMenu_Com_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.bMenuStatus = ((System.Windows.Controls.Button)(target));
            
            #line 38 "..\..\..\MainWin.xaml"
            this.bMenuStatus.Click += new System.Windows.RoutedEventHandler(this.bMenu_Status_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.bMenuAppSet = ((System.Windows.Controls.Button)(target));
            
            #line 39 "..\..\..\MainWin.xaml"
            this.bMenuAppSet.Click += new System.Windows.RoutedEventHandler(this.bMenu_AppSet_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.FrameV = ((System.Windows.Controls.Frame)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
