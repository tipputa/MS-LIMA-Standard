using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.ComponentModel;
using Metabolomics.Core;
using Metabolomics.MsLima.Model;
using Metabolomics.MsLima.Bean;
using ChartDrawing;
using Microsoft.Win32;
using Metabolomics.MsLima.Exporter;

namespace Metabolomics.MsLima.ViewModel
{
    class SaveChartDrawingVM : ViewModelBase
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }
        public int DpiX { get; set; } = 300;
        public int DpiY { get; set; } = 300;
        public string FilePath { get; set; }
        public bool IsPngChecked { get; set; } = false;
        public bool IsEmfChecked { get; set; } = true;
        public bool IsArticleFormat { get; set; } = true;
        public DrawVisual DrawVisual { get; set; }
        public SaveChartDrawingVM(DrawVisual dv)
        {
            this.DrawVisual = dv;
            this.Width = dv.Area.Width;
            this.Height = dv.Area.Height;
            this.MinX = dv.MinX;
            this.MaxX = dv.MaxX;
            this.MinY = dv.MinY;
            this.MaxY = dv.MaxY;
        }

        #region Delegate Commands
        private DelegateCommand fileSelect;
        private DelegateCommand exportCommand;
        private DelegateCommand cancelCommand;

        public DelegateCommand FileSelect {
            get {
                return fileSelect ?? new DelegateCommand(x =>
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.FileName = "*";
                    sfd.Title = "Save file dialog";
                    sfd.RestoreDirectory = true;

                    if (sfd.ShowDialog() == true)
                    {
                        this.FilePath = sfd.FileName;
                        OnPropertyChanged(nameof(FilePath));
                    }
                });
            }
        }

        public DelegateCommand ExportCommand { get {
                return exportCommand ?? new DelegateCommand(x =>
                {
                    if (string.IsNullOrEmpty(FilePath))
                    {
                        MessageBox.Show("Please select export file path");
                        return;
                    }
                    if (!IsPngChecked && !IsEmfChecked)
                    {
                        MessageBox.Show("Please select at least one file format");
                        return;
                    }
                    if (IsPngChecked)
                        ExportDrawVisual.SaveAsPng(FilePath + ".png", DrawVisual, MinX, MaxX, MinY, MaxY, Width, Height, DpiX, DpiY, IsArticleFormat);
                    if (IsEmfChecked)
                        ExportDrawVisual.SaveAsEmf(FilePath + ".emf", DrawVisual, MinX, MaxX, MinY, MaxY, Width, Height, IsArticleFormat);

                    ((SaveChartDrawing)x).Close();
                });
            }
        }
        public DelegateCommand CancelCommand {
            get {
                return cancelCommand ?? new DelegateCommand(x => ((SaveChartDrawing)x).Close());
            }
        }

        #endregion
    }
}
