﻿#pragma checksum "..\..\BackWin.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "301DEE528A452B495F70266CB8DC35ABEDF6FFBD6B11343293FAAF648A88BE62"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Hardcodet.Wpf.TaskbarNotification;
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
using Twichabot;


namespace Twichabot {
    
    
    /// <summary>
    /// BackWin
    /// </summary>
    public partial class BackWin : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\BackWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Hardcodet.Wpf.TaskbarNotification.TaskbarIcon TaskbarIcon;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\BackWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbTaskbarIcon_Menu_Status;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\BackWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem bTaskbarIcon_Menu_Con;
        
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
            System.Uri resourceLocater = new System.Uri("/Twichabot;component/backwin.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\BackWin.xaml"
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
            this.TaskbarIcon = ((Hardcodet.Wpf.TaskbarNotification.TaskbarIcon)(target));
            
            #line 15 "..\..\BackWin.xaml"
            this.TaskbarIcon.TrayLeftMouseDown += new System.Windows.RoutedEventHandler(this.TaskbarIcon_LeftClick);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 17 "..\..\BackWin.xaml"
            ((System.Windows.Controls.ContextMenu)(target)).IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.TaskbarIcon_ContextMenuVisibleChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 19 "..\..\BackWin.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.TaskbarIcon_LeftClick);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 22 "..\..\BackWin.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.TaskbarIcon_Menu_AppHide_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.tbTaskbarIcon_Menu_Status = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.bTaskbarIcon_Menu_Con = ((System.Windows.Controls.MenuItem)(target));
            
            #line 28 "..\..\BackWin.xaml"
            this.bTaskbarIcon_Menu_Con.Click += new System.Windows.RoutedEventHandler(this.TaskbarIcon_Menu_Con_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 33 "..\..\BackWin.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.TaskbarIcon_Menu_AppStop_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

