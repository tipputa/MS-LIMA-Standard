# MS-LIMA 

[Download the latest release](https://github.com/tipputa/MS-LIMA-Standard//releases/latest)

MS-LIMA is mass spectral library management software.

It supports MS2 peak annotations by MS-FINDER. Please also check [MS-FINDER](http://prime.psc.riken.jp/Metabolomics_Software/MS-FINDER/).

## Acceptable formats
+ MSP formats (.msp; from RIKEN CSRS, MoNA)
+ MGF format (.mgf)
+ MassBank format (.txt)

## How to use
Please import a mass spectral library file from menu.

MS-LIMA automatically makes compound groups based on meta data (InChI, InChIKey, or ShortInChIKey), and investigates the library errors about retention time (large difference), InChIKey, and formaula in each compound group.

Please export the library if you changed in MS-LIMA.

## MainWindow
![MainWindow](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_MS-LIMA.PNG?raw=true)
It consists of mass spectrum view (MS view) and 3 tables, compound table, mass spectra table, and peak table.

If you change selected compound, spectra table and peak informations will be automatically changed.

You can directly change peak tables for curation. 

Consensus peak table automatically made from all spectra in a selected compound group.

If you select any row in peak table, the selected peak will be highlighted in MS view.

#### Filtering by compound name, retention time, molecular weight, and InChIKey
Please use this function for searching and filtering compounds

### Three MS viewers 
You can visually chack mass spectrum one by one or simultaneously.
![Multiple viewer](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_MS-LIMA_2.PNG?raw=true)

Also you can see the consensus spectrum of a selected compound. Peak colors indicates as following:
+ Black: 1 spectrum has this peak
+ Blue: >1 spectrum and <50% of spectra have this peak
+ Red: >50% of spectra have this peak
![Consensus viewer](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_MS-LIMA_3.PNG?raw=true)


## Additional windows
You can open additional windows by clicking Viewer in Menubar
### Meta data of all spectra
![Metadata Window](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_MetaInformation.PNG?raw=true)
You can directly change almost all metadata from this window. 

![Comparative Window](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_ComparativeViewer.PNG?raw=true)
You can compare two spectra using this window. 

Spectrum selected in left table will be shown as upper part, selected in right table will be shown as lower part.

You can import different library from the button.

This window cannot affect the main window if you changed imported library.

## Menubar Utilities
### Remove unannotated peaks
All peaks which have comments will be removed from the library.

If you want to save modified library, please export it.

### Convert precursor m/z as theoretical m/z
In all spectra, precursor m/z value will be comverted as theoretical m/z calculated by formula and adduct type.

If you want to save modified library, please export it.

### Remove all retention time
Retention time will be removed in all spectra.

If you want to save modified library, please export it.

### Calc and save common product ions
MS-LIMA can calculate common product ions in whole library. 

Exported format is as following;

|MedianMz |MedianIntensity |NumPeaks |Percent |Comments |
|--:|--:|--:|--:|--:|
|56.04 |11.6 |151 |18.6 |SMILES NCCC |
|41.03 |8.7 |137 |16.8 |SMILES CCC |
|42.03 |13.7 |126 | 15.5 |SMILES NCC |

### Update SMILES and InChI based on InChIKey
You can update SMILES and InCHi in all spectra based on InChIKey.

Please use following file format. The demo file locates ./demo/InChIKey_InChI_SMILES.txt

|InChIKey |InChI |SMILES |
|--:|--:|--:|
|AAA-AAA-A|InChI=/AA/AAA|CCCC|
|BBB-BBB-B|InChI=/BB/BBB|NCCC|

### Update common meta data
You can update common meta data, such as INSTRUMENT, INSTRUMENTTYPE, MSLEVEL, SPECTRUMTYPE, and so on.

Please use following file format. The demo file locates ./demo/CommonMetaData.txt

|Header|
|---|
|INSTRUMENT: YourSetting|
|INSTRUMENTTYPE: YourSetting|
