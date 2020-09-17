using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using FolderClean.Application.Common;
using FolderClean.Wpf.Handlers;
using FolderClean.Wpf.ViewAction;
using Microsoft.Win32;

namespace FolderClean.Wpf.ViewModel
{
    public class MainViewModel : BaseViewModel<MainViewAction>
    {
        #region Const

        public const string ServiceName = "Folder Clean Service";

        #endregion
        #region Private Members
        private string _sourceFolder;
        private string _destinationFolder;
        #endregion

        #region Public Members

        public string SourceFolder
        {
            get => _sourceFolder;
            set
            {
                _sourceFolder = value;
                OnPropertyChanged();
            }
        }

        public string DestinationFolder
        {
            get => _destinationFolder;
            set
            {
                _destinationFolder = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Protected Methods

        protected override async Task PerformAction(MainViewAction action)
        {
            switch (action)
            {
                case MainViewAction.Start:
                    StartService(ServiceName);
                    break;
                case MainViewAction.Stop:
                    StopService(ServiceName);
                    break;
                case MainViewAction.Install:
                    InstallService(ServiceName);
                    break;
                case MainViewAction.Delete:
                    DeleteService(ServiceName);
                    break;
            }
        }

        private void DeleteService(string name)
        {
            try
            {
                if (!ServiceHandler.ServiceIsInstalled(name))
                {
                    MessageBox.Show("Service is not installed");
                    return;
                }
                ServiceHandler.Uninstall(name);
                MessageBox.Show("Service was uninstalled");
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Error : " + e.Message);
            }
        }

        private void InstallService(string name)
        {
            try
            {
                if (!Directory.Exists(SourceFolder))
                {
                    MessageBox.Show("Invalid Source Directory");
                    return;
                }
                if (!Directory.Exists(DestinationFolder))
                {
                    MessageBox.Show("Invalid Destination Directory");
                    return;
                }
                if (ServiceHandler.ServiceIsInstalled(name))
                {
                    MessageBox.Show("Service is already installed");
                    return;
                }
                var dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                dialog.Filter = "Service Files | *.exe";
                if (dialog.ShowDialog().GetValueOrDefault(false))
                {
                    var filePath = dialog.FileName;
                    var fileInfo = new FileInfo(filePath);
                    var folder = fileInfo.Directory?.FullName;
                    File.WriteAllText(folder + "/config.json",JsonSerializer.Serialize(new ConfigApp()
                    {
                        DestinationFolder = DestinationFolder,
                        SourceFolder = SourceFolder
                    }));
                    ServiceHandler.InstallAndStart(name, name,filePath);
                    MessageBox.Show("Service was installed");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error : " + e.Message);
            }
        }

        private void StopService(string name)
        {
            try
            {
                if (!ServiceHandler.ServiceIsInstalled(name))
                {
                    MessageBox.Show("Service is not installed");
                    return;
                }
                ServiceHandler.StopService(name);
                MessageBox.Show("Service was Stopped");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error : " + e.Message);
            }
        }

        private void StartService(string name)
        {
            try
            {
                if (!ServiceHandler.ServiceIsInstalled(name))
                {
                    MessageBox.Show("Service is not installed");
                    return;
                }
                ServiceHandler.StartService(name);
                MessageBox.Show("Service was Started");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error : " + e.Message);
            }
        }

        #endregion
    }
}