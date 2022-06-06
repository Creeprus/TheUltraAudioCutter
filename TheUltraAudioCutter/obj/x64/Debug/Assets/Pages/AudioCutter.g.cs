﻿#pragma checksum "C:\Users\User\source\repos\TheUltraAudioCutter\TheUltraAudioCutter\Assets\Pages\AudioCutter.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A4D81C38693001D8A03AEA5C1E7FC5AA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheUltraAudioCutter.Assets.Pages
{
    partial class AudioCutter : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // Assets\Pages\AudioCutter.xaml line 63
                {
                    this.LoadDummy = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 3: // Assets\Pages\AudioCutter.xaml line 55
                {
                    this.Player = (global::Windows.UI.Xaml.Controls.MediaElement)(target);
                    ((global::Windows.UI.Xaml.Controls.MediaElement)this.Player).PointerMoved += this.Player_PointerMoved;
                    ((global::Windows.UI.Xaml.Controls.MediaElement)this.Player).MediaOpened += this.Player_MediaOpened;
                }
                break;
            case 4: // Assets\Pages\AudioCutter.xaml line 61
                {
                    this.CurrentProgressTrack = (global::Windows.UI.Xaml.Controls.ProgressBar)(target);
                }
                break;
            case 5: // Assets\Pages\AudioCutter.xaml line 44
                {
                    this.CopyBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.CopyBtn).Click += this.Copy_Click;
                }
                break;
            case 6: // Assets\Pages\AudioCutter.xaml line 46
                {
                    this.CutBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.CutBtn).Click += this.Cut_Click;
                }
                break;
            case 7: // Assets\Pages\AudioCutter.xaml line 48
                {
                    this.PasteBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.PasteBtn).Click += this.Paste_Click;
                }
                break;
            case 8: // Assets\Pages\AudioCutter.xaml line 50
                {
                    this.DeleteBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.DeleteBtn).Click += this.Delete_Click;
                }
                break;
            case 9: // Assets\Pages\AudioCutter.xaml line 52
                {
                    this.SaveBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.SaveBtn).Click += this.SaveBtn_Click;
                }
                break;
            case 10: // Assets\Pages\AudioCutter.xaml line 33
                {
                    this.AudioEditorControl = (global::TheUltraAudioCutter.Assets.Pages.GraphicsEditor)(target);
                }
                break;
            case 11: // Assets\Pages\AudioCutter.xaml line 25
                {
                    this.OpenFileBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.OpenFileBtn).Click += this.LoadAudioFile;
                }
                break;
            case 12: // Assets\Pages\AudioCutter.xaml line 28
                {
                    this.ChangeFolder_Btn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.ChangeFolder_Btn).Click += this.ChangeFolder_Btn_Click;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

