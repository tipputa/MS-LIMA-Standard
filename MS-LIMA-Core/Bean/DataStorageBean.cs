using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Metabolomics.Core;
using Metabolomics.MsLima.Reader;
using Metabolomics.MsLima.Model;

namespace Metabolomics.MsLima.Bean
{
    public class DataStorageBean
    {
        #region Properties
        public List<MassSpectrum> RawLibraryFile { get; set; }
        public List<CompoundBean> CompoundList { get; set; }

        public string FilePath { get; set; }
        public string OriginalFilePath { get; set; }
        public LibraryFileFormat FileFormat { get; set; }


        #endregion

        public DataStorageBean() { }

        public void SetLibrary(string filePath, CompoundGroupingKey key)
        {
            FilePath = filePath;
            ReadLibraryFile();
            CompoundList = CompoundGroupUtility.CreateCompoundList(RawLibraryFile, key);
        }

        public void SetMassBankLibrary(string filePath, CompoundGroupingKey key)
        {
            FilePath = filePath;
            this.FileFormat = LibraryFileFormat.MassBank;
            this.RawLibraryFile = ReadMassBankFile.ReadAsMsSpectra(FilePath);
            CompoundList = CompoundGroupUtility.CreateCompoundList(RawLibraryFile, key);
        }



        public void ReadLibraryFile()
        {
            var extention = Path.GetExtension(FilePath).ToLower();
            if (extention == ".mgf")
            {
                this.FileFormat = LibraryFileFormat.Mgf;
                this.RawLibraryFile = ReadMgfFile.ReadAsMsSpectra(FilePath);
            }
            else if (extention == ".msp")
            {
                this.FileFormat = LibraryFileFormat.Msp;
                this.RawLibraryFile = ReadMspFile.ReadAsMsSpectra(FilePath);
            }
            else if (extention == ".txt")
            {
                this.FileFormat = LibraryFileFormat.Text;
                this.RawLibraryFile = ReadMassBankFile.ReadAsMsSpectra(FilePath);
            }
            else
            {
                this.FileFormat = LibraryFileFormat.Text;
                this.RawLibraryFile = ReadMassBankFile.ReadAsMsSpectra(FilePath);
            }
        }        
    }
}
