﻿#pragma checksum "..\..\StationsMeasure.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0A6A8430BEADD664A7DCDF35C85E6260"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using AutomatedMicroTriangulation;
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
using TSControl;


namespace AutomatedMicroTriangulation {
    
    
    /// <summary>
    /// StationsMeasure
    /// </summary>
    public partial class StationsMeasure : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\StationsMeasure.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbStationMeasureTarget1;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\StationsMeasure.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbStationMeasureTarget2;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\StationsMeasure.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbStationMeasureTarget3;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\StationsMeasure.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel stpStationMeasureConPanel;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\StationsMeasure.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbStationMeasureCOM;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\StationsMeasure.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnStationMeasureMeasure;
        
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
            System.Uri resourceLocater = new System.Uri("/AutomatedMicroTriangulation;component/stationsmeasure.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\StationsMeasure.xaml"
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
            this.cmbStationMeasureTarget1 = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.cmbStationMeasureTarget2 = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.cmbStationMeasureTarget3 = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.stpStationMeasureConPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 5:
            this.cmbStationMeasureCOM = ((System.Windows.Controls.ComboBox)(target));
            
            #line 43 "..\..\StationsMeasure.xaml"
            this.cmbStationMeasureCOM.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cmbStationMeasureCOM_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnStationMeasureMeasure = ((System.Windows.Controls.Button)(target));
            
            #line 47 "..\..\StationsMeasure.xaml"
            this.btnStationMeasureMeasure.Click += new System.Windows.RoutedEventHandler(this.btnStationMeasureMeasure_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

