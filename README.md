# MS-LIMA 

[Download the latest release](https://github.com/tipputa/MS-LIMA-Standard//releases/latest)

MS-LIMA is a software for mass spectral library management.

## Acceptable formats
+ MSP formats (.msp; from RIKEN CSRS, MoNA)
+ MGF format (.mgf)
+ MassBank format (.txt)

## How to use
Please import a mass spectral library file from menu.

MS-LIMA automatically makes compound groups based on meta data (InChI, InChIKey, or ShortInChIKey), and investigate the library errors about retention time (large difference), InChIKey, and formaula in each compound group.

Please export the library if you changed.

## MainWindow
![MainWindow](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_MS-LIMA.PNG?raw=true)
It consists of mass spectrum view (MS view) and 3 tables, compound table, mass spectra table, and peak table.

If you change selected compound, spectra table and peak informations are automatically changed.

You can directly change peak tables for curation. 

Consensus Peak table automatically made from all spectra in a selected compound group.

If you select any row in peak table, the selected peak will be highlighted in MS view.

### three MS viewers 
You can visually chack mass spectrum one by one or simultaneously.
![Multiple viewer](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_MS-LIMA_2.PNG?raw=true)

Also you can see the consensus spectrum of a selected compound. Peak colors indicates as following:
Black: 1 spectrum has this peak
Blue: >1 spectrum and <50% of spectra have this peak
Red: >50% of spectra have this peak
![Consensus viewer](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_MS-LIMA_3.PNG?raw=true)

### Filtering by compound name, retention time, molecular weight, and InChIKey
Please use this function for searching and filtering compounds


## Additional windows
![Metadata Window](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_MetaInformation.PNG?raw=true)


![Comparative Window](https://github.com/tipputa/MS-LIMA-Standard/blob/master/MS-LIMA/Pic/190529_ComparativeViewer.PNG?raw=true)



## Menubar Utilities
### 
