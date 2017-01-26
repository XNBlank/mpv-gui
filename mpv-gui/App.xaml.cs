﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace mpv_gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        private void OnExit(object sender, ExitEventArgs e)
        {
            mpv_gui.Properties.Settings.Default.Save();
        }

    }
}
