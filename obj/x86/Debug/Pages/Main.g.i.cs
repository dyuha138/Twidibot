﻿#pragma checksum "..\..\..\..\Pages\Main.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "7A10664E913CEFF8BFFF378E654F7A84948A31903E5B62E118E0B3FBC8CA3209"
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
using Twichabot.Pages;


namespace Twichabot.Pages {
    
    
    /// <summary>
    /// Main
    /// </summary>
    public partial class Main : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 33 "..\..\..\..\Pages\Main.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lChat;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\Pages\Main.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox eChatMsgSend;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\Pages\Main.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bChatStart;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Pages\Main.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bChatStop;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Pages\Main.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lError;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\..\Pages\Main.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lStatus;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\..\Pages\Main.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lChannel;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\Pages\Main.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lBotLogin;
        
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
            System.Uri resourceLocater = new System.Uri("/Twichabot;component/pages/main.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Pages\Main.xaml"
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
            this.lChat = ((System.Windows.Controls.ListBox)(target));
            return;
            case 2:
            this.eChatMsgSend = ((System.Windows.Controls.TextBox)(target));
            
            #line 34 "..\..\..\..\Pages\Main.xaml"
            this.eChatMsgSend.DragEnter += new System.Windows.DragEventHandler(this.ChatMsgSend);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 35 "..\..\..\..\Pages\Main.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ChatMsgSend);
            
            #line default
            #line hidden
            return;
            case 4:
            this.bChatStart = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\..\..\Pages\Main.xaml"
            this.bChatStart.Click += new System.Windows.RoutedEventHandler(this.bChatStart_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.bChatStop = ((System.Windows.Controls.Button)(target));
            
            #line 37 "..\..\..\..\Pages\Main.xaml"
            this.bChatStop.Click += new System.Windows.RoutedEventHandler(this.bChatStop_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.lError = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.lStatus = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.lChannel = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.lBotLogin = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
