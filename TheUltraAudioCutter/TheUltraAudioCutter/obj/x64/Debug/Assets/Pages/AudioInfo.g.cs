﻿#pragma checksum "C:\Users\User\source\repos\TheUltraAudioCutter\TheUltraAudioCutter\Assets\Pages\AudioInfo.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6E8A89D308FC3F3FB9C047C8906D6A6B"
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
    partial class AudioInfo : 
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
            case 2: // Assets\Pages\AudioInfo.xaml line 12
                {
                    this.AudioInfoGetter = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.AudioInfoGetter).Click += this.AudioInfoGetter_Click;
                }
                break;
            case 3: // Assets\Pages\AudioInfo.xaml line 13
                {
                    this.AudioImage = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 4: // Assets\Pages\AudioInfo.xaml line 14
                {
                    this.TrackName = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 5: // Assets\Pages\AudioInfo.xaml line 15
                {
                    this.AlbumName = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 6: // Assets\Pages\AudioInfo.xaml line 16
                {
                    this.Compositor = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 7: // Assets\Pages\AudioInfo.xaml line 17
                {
                    this.PathMusic = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 8: // Assets\Pages\AudioInfo.xaml line 18
                {
                    this.File_nameMusic = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 9: // Assets\Pages\AudioInfo.xaml line 19
                {
                    this.DurationTrack = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 10: // Assets\Pages\AudioInfo.xaml line 20
                {
                    this.Bitrate = (global::Windows.UI.Xaml.Controls.TextBox)(target);
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

