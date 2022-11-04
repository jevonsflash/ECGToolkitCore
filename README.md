# C# ECG Toolkit 2.5 .Net6

This is a project based on C# ECG Toolkit 2.5, It's a modernisation of the original project, designed to run on Linux. Net6 environment, and the ECGView tool rewritten with WPF.



Since `System.Drawing.Common` will not be supported in.NET 6 and later, the cross-platform features will be affected. [(See Official Documentation)](https://learn.microsoft.com/zh-cn/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only), Drawing library uses [ImageSharp. Drawing](https://github.com/SixLabors/ImageSharp.Drawing) instead.



The main transformations are as follows:



- Based on .NET 6

- Drawing system library changed from `System.Drawing.Common` to `ImageSharp.Drawing`

- Rewritten ECG Viewer WPF

- MVVM design mode

- Simple Ioc



The model and directory structure remained the same as the original project, as shown in the original README documentation below



# C# ECG Toolkit 2.5
### ECG Toolkit support for: SCP-ECG, DICOM, HL7 aECG, ISHNE & MUSE-XML

## Description
C# ECG Toolkit is an open source software toolkit to convert, view and print electrocardiograms. The toolkit is developed using C# .NET 2.0 (code also supports 1.1, 3.5 and 4.0). Support for ECG formats: SCP-ECG, DICOM, HL7 aECG, ISHNE and MUSE-XML.

## Features
- ECG Viewer
- Caliper
- Converter
- SCP ECG
- HL7 aECG
- DICOM
- ISHNE
- MUSE-XML
- PDF
- OmronECG

> Migrating from SourceForge [CVS repository](http://ecgtoolkit-cs.sourceforge.net/), 2016-6-12.  
> Merge all commits prior to 2019-12-2。[Ecgtoolkit-cs](https://sourceforge.net/p/ecgtoolkit-cs/git/ci/master/tree/) project may be revived, go to sourceforge for the latest source code， 2019-12-2。
