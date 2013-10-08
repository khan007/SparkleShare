//   SparkleShare, a collaboration and sharing tool.
//   Copyright (C) 2010  Hylke Bons (hylkebons@gmail.com)
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with this program. If not, see (http://www.gnu.org/licenses/).


using System;
using System.Diagnostics;

using Gtk;
using Mono.Unix;

namespace SparkleShare {

    public class SparkleAbout : Window {

        public SparkleAboutController Controller = new SparkleAboutController ();

        private Label updates;


        public SparkleAbout () : base ("")
        {
            SetSizeRequest (600, 260);
            Resizable      = false;
            BorderWidth    = 0;
            IconName       = "folder-sparkleshare";
            WindowPosition = WindowPosition.Center;
            Title          = "About SparkleShare";

            CssProvider css_provider = new CssProvider ();
            string image_path        = new string [] { SparkleUI.AssetsPath, "pixmaps", "about.png" }.Combine ();
            css_provider.LoadFromData ("GtkWindow {" +
                "background-image: url('" + image_path + "');" +
                "background-repeat: no-repeat;" +
                "background-position: left bottom;" +
                "}");
            
            StyleContext.AddProvider (css_provider, 800);

            CreateAbout ();

            DeleteEvent += delegate (object o, DeleteEventArgs args) {
                Controller.WindowClosed ();
                args.RetVal = true;
            };
            
            KeyPressEvent += delegate (object o, KeyPressEventArgs args) {
                if (args.Event.Key == Gdk.Key.Escape ||
                    (args.Event.State == Gdk.ModifierType.ControlMask && args.Event.Key == Gdk.Key.w)) {
                    
                    Controller.WindowClosed ();
                }
            };

            Controller.HideWindowEvent += delegate {
                Application.Invoke (delegate { Hide (); });
            };

            Controller.ShowWindowEvent += delegate {
                Application.Invoke (delegate {
                    ShowAll ();
                    Present ();
                });
            };

            Controller.UpdateLabelEvent += delegate (string text) {
                Application.Invoke (delegate {
                    this.updates.Markup = String.Format ("<span font_size='small' fgcolor='#8cc4ff'>{0}</span>", text);
                    this.updates.ShowAll ();
                });
            };
        }


        private void CreateAbout ()
        {
            Label version = new Label () {
                Markup = string.Format ("<span font_size='small' fgcolor='white'>version {0}</span>",
                    Controller.RunningVersion),
                Xalign = 0,
                Xpad = 0
            };

            this.updates = new Label () {
                Markup = "<span font_size='small' fgcolor='#a8bbcf'>Checking for updates...</span>",
                Xalign = 0,
                Xpad = 0
            };

            Label copyright = new Label () {
                Markup = "<span font_size='small' fgcolor='white'>" +
                         "Copyright © 2010–" + DateTime.Now.Year + " " +
                         "Hylke Bons and others." +
                         "</span>",
                Xalign = 0,
                Xpad   = 0
            };

            Label license = new Label () {
                LineWrap     = true,
                LineWrapMode = Pango.WrapMode.Word,
                Markup       = "<span font_size='small' fgcolor='white'>" +
                               "SparkleShare is Open Source software. You are free to use, modify, " +
                               "and redistribute it under the GNU General Public License version 3 or later." +
                               "</span>",
                Wrap         = true,
                Xalign       = 0
            };

            VBox layout_vertical = new VBox (false, 0);            
            HBox links_layout = new HBox (false, 16);
            
            SparkleLink website_link        = new SparkleLink ("Website", Controller.WebsiteLinkAddress);
            SparkleLink credits_link        = new SparkleLink ("Credits", Controller.CreditsLinkAddress);
            SparkleLink report_problem_link = new SparkleLink ("Report a problem", Controller.ReportProblemLinkAddress);
            SparkleLink debug_log_link = new SparkleLink ("Debug log", Controller.DebugLogLinkAddress);
            
            links_layout.PackStart (website_link, false, false, 0);
            links_layout.PackStart (credits_link, false, false, 0);
            links_layout.PackStart (report_problem_link, false, false, 0);
            links_layout.PackStart (debug_log_link, false, false, 0);

            layout_vertical.PackStart (new Label (""), true, true, 0);            
            layout_vertical.PackStart (version, false, false, 0);
            layout_vertical.PackStart (this.updates, false, false, 0);
            layout_vertical.PackStart (copyright, false, false, 6);
            layout_vertical.PackStart (license, false, false, 6);
            layout_vertical.PackStart (links_layout, false, false, 16);
            
            HBox layout_horizontal = new HBox (false, 0);
            layout_horizontal.PackStart (new Label (""), false, false, 149);
            layout_horizontal.PackStart (layout_vertical, false, false, 0);

            Add (layout_horizontal);
        }
    }
    
    
    public class SparkleLink : EventBox {
        
        public SparkleLink (string text, string url)
        {
            VisibleWindow = false;
            
            Label label = new Label () {
                Markup = "<span size='small' fgcolor='#729fcf' underline='single'>" + text + "</span>"
            };
            
            EnterNotifyEvent += delegate { Window.Cursor = new Gdk.Cursor (Gdk.CursorType.Hand1); };
            LeaveNotifyEvent += delegate { Window.Cursor = new Gdk.Cursor (Gdk.CursorType.Arrow); };
            ButtonPressEvent += delegate { Program.Controller.OpenWebsite (url); };
            
            Add (label);
        }
    }
}
