# C# ECG Toolkit 2.5 .Net6

这是基于C# ECG Toolkit 2.5 二次开发的项目，对原项目经行了现代化改造，旨在能运行于Linux的.Net6环境，并且对ECGView这个工具用WPF技术重写了

由于在System.Drawing.Common在.net 6以及更高版本将不受支持，跨平台特性将受影响，[官方文档](https://learn.microsoft.com/zh-cn/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only)， 绘制库使用[ImageSharp.Drawing](https://github.com/SixLabors/ImageSharp.Drawing)代替

主要改造如下：

- 基于.Net 6 
- 绘制系统库由System.Drawing.Common变更为ImageSharp.Drawing
- 重写的 ECG Viewer WPF
- MVVM 设计模式
- 简单的Ioc

模型和目录结构将和原始项目保持一致，以下是原始README文档



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
