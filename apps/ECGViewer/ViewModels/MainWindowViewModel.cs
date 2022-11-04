using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECGConversion;
using ECGViewer.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text;
using System.Collections;
using ECGConversion.ECGSignals;
using System.IO;
using System.Threading;
using ECGConversion.ECGGlobalMeasurements;
using ECGConversion.ECGDiagnostic;
using ECGViewer.Extensions;
using ECGViewer.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace ECGViewer.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private UnknownECGReader _ECGReader = null;
        private Signals _CurrentSignal = null;

        private ECGDraw.ECGDrawType _DrawType = ECGDraw.ECGDrawType.Regular;

        private double _BottomCutoff = double.NaN;
        private double _TopCutoff = double.NaN;
        private int _Zoom = 1;
        private OpenFileDialog openECGFileDialog;
        private SaveFileDialog saveECGFileDialog;


        public MainWindowViewModel()
        {

            this.openECGFileDialog = new OpenFileDialog();
            this.saveECGFileDialog = new SaveFileDialog();

            this.menuGridNone = ECGDraw.DisplayGrid == ECGDraw.GridType.None;
            this.menuGridOne = ECGDraw.DisplayGrid == ECGDraw.GridType.OneMillimeters;
            this.menuGridFive = ECGDraw.DisplayGrid == ECGDraw.GridType.FiveMillimeters;

            this.MenuLeadFormatRegular=true;
            this.menuFilterNone=true;
            this.menuGain2=true;
            this.menuCaliperOff=true;
            this.menuDisplayInfo=true;

            MenuZoomInCommand=new RelayCommand(MenuZoomInAction);
            MenuZoomOutCommand=new RelayCommand(MenuZoomOutAction);
            MenuOpenFileCommand=new RelayCommand(MenuOpenFileAction);
            MenuSaveFileCommand=new RelayCommand(MenuSaveFileAction, () => IsFileLoaded);
            MenuCloseCommand=new RelayCommand(MenuCloseAction, () => IsFileLoaded);
            MenuAddPluginFileCommand=new RelayCommand(MenuAddPluginFileAction);

            MenuLeadFormatRegularCommand=new RelayCommand(MenuLeadFormatRegularAction, () => (_CurrentECGDrawType & ECGDraw.ECGDrawType.Regular) != 0);
            MenuLeadFormatFourXThreeCommand=new RelayCommand(MenuLeadFormatFourXThreeAction, () => (_CurrentECGDrawType & ECGDraw.ECGDrawType.ThreeXFour) != 0);
            MenuLeadFormatFourXThreePlusOneCommand=new RelayCommand(MenuLeadFormatFourXThreePlusOneAction, () => (_CurrentECGDrawType & ECGDraw.ECGDrawType.ThreeXFourPlusOne) != 0);
            MenuLeadFormatFourXThreePlusThreeCommand=new RelayCommand(MenuLeadFormatFourXThreePlusThreeAction, () => (_CurrentECGDrawType & ECGDraw.ECGDrawType.ThreeXFourPlusThree) != 0);
            MenuLeadFormatSixXTwoCommand=new RelayCommand(MenuLeadFormatSixXTwoAction, () => (_CurrentECGDrawType & ECGDraw.ECGDrawType.SixXTwo) != 0);
            MenuLeadFormatMedianCommand=new RelayCommand(MenuLeadFormatMedianAction, () => (_CurrentECGDrawType & ECGDraw.ECGDrawType.Median) != 0);
            MenuGain1Command=new RelayCommand(MenuGain1Action);
            MenuGain2Command=new RelayCommand(MenuGain2Action);
            MenuGain3Command=new RelayCommand(MenuGain3Action);
            MenuGain4Command=new RelayCommand(MenuGain4Action);

            MenuAnnonymizeCommand=new RelayCommand(MenuAnnonymizeAction);
            MenuDisplayInfoCommand=new RelayCommand(MenuDisplayInfoAction);
            MenuGridOneCommand=new RelayCommand(MenuGridOneAction);
            MenuGridFiveCommand=new RelayCommand(MenuGridFiveAction);
            MenuGridNoneCommand=new RelayCommand(MenuGridNoneAction);

            MenuColor1Command=new RelayCommand(MenuColor1Action);
            MenuColor2Command=new RelayCommand(MenuColor2Action);
            MenuColor3Command=new RelayCommand(MenuColor3Action);
            MenuColor4Command=new RelayCommand(MenuColor4Action);
            MenuCaliperOffCommand=new RelayCommand(MenuCaliperOffAction);
            MenuCaliperDurationCommand=new RelayCommand(MenuCaliperDurationAction);
            MenuCaliperBothCommand=new RelayCommand(MenuCaliperBothAction);
            MenuFilterNoneCommand=new RelayCommand(MenuFilterNoneAction);
            MenuFilter40HzCommand=new RelayCommand(MenuFilter40HzAction);
            MenuFilterMuscleCommand=new RelayCommand(MenuFilterMuscleAction);

            MenuViewCommand=new RelayCommand(MenuViewAction, () => IsFileLoaded);

            this.PropertyChanged+=MainWindowViewModel_PropertyChanged;


        }

        private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName==nameof(RenderHeight) && RenderHeight>0)
            {
                _DrawBuffer=null;
                this.Refresh();
            }
            else if (e.PropertyName==nameof(RenderWidth) && RenderWidth>0 && RenderHeight>0)
            {

                _DrawBuffer=null;
                this.Refresh();

            }
            else if (e.PropertyName==nameof(CurrentECG))
            {
                OnPropertyChanged(nameof(IsFileLoaded));
                OnPropertyChanged(nameof(FileLoadedControlVisiblity));
            }

        }

        ECGDraw.ECGDrawType _CurrentECGDrawType => ECGDraw.PossibleDrawTypes(_CurrentSignal);


        private bool menuOpen;

        public bool MenuOpen
        {
            get { return menuOpen; }
            set
            {
                menuOpen = value;
                OnPropertyChanged();
            }
        }


        private bool menuOpenFile;

        public bool MenuOpenFile
        {
            get { return menuOpenFile; }
            set { menuOpenFile = value; OnPropertyChanged(); }
        }

        private bool menuClose;

        public bool MenuClose
        {
            get { return menuClose; }
            set { menuClose = value; OnPropertyChanged(); }
        }

        private bool menuSave;
        public bool MenuSave
        {
            get { return menuSave; }
            set { menuSave = value; OnPropertyChanged(); }
        }

        private bool menuSaveFile;
        public bool MenuSaveFile
        {
            get { return menuSaveFile; }
            set { menuSaveFile = value; OnPropertyChanged(); }
        }

        private bool menuPlugin;
        public bool MenuPlugin
        {
            get { return menuPlugin; }
            set { menuPlugin = value; OnPropertyChanged(); }

        }

        private bool menuAddPluginFile;
        public bool MenuAddPluginFile
        {
            get { return menuAddPluginFile; }
            set { menuAddPluginFile = value; OnPropertyChanged(); }
        }


        private bool menuView;
        public bool MenuView
        {
            get { return menuView; }
            set { menuView = value; OnPropertyChanged(); }
        }
        private bool menuLeadFormat;
        public bool MenuLeadFormat
        {
            get { return menuLeadFormat; }
            set { menuLeadFormat = value; OnPropertyChanged(); }
        }
        private bool menuLeadFormatRegular;
        public bool MenuLeadFormatRegular
        {
            get { return menuLeadFormatRegular; }
            set { menuLeadFormatRegular = value; OnPropertyChanged(); }
        }
        private bool menuLeadFormatThreeXFour;
        public bool MenuLeadFormatThreeXFour
        {
            get { return menuLeadFormatThreeXFour; }
            set { menuLeadFormatThreeXFour = value; OnPropertyChanged(); }
        }
        private bool menuLeadFormatThreeXFourPlusOne;
        public bool MenuLeadFormatThreeXFourPlusOne
        {
            get { return menuLeadFormatThreeXFourPlusOne; }
            set { menuLeadFormatThreeXFourPlusOne = value; OnPropertyChanged(); }
        }
        private bool menuLeadFormatThreeXFourPlusThree;
        public bool MenuLeadFormatThreeXFourPlusThree
        {
            get { return menuLeadFormatThreeXFourPlusThree; }
            set { menuLeadFormatThreeXFourPlusThree = value; OnPropertyChanged(); }
        }
        private bool menuLeadFormatSixXTwo;
        public bool MenuLeadFormatSixXTwo
        {
            get { return menuLeadFormatSixXTwo; }
            set { menuLeadFormatSixXTwo = value; OnPropertyChanged(); }
        }
        private bool menuLeadFormatMedian;
        public bool MenuLeadFormatMedian
        {
            get { return menuLeadFormatMedian; }
            set { menuLeadFormatMedian = value; OnPropertyChanged(); }
        }
        private bool menuGain;
        public bool MenuGain
        {
            get { return menuGain; }
            set { menuGain = value; OnPropertyChanged(); }
        }
        private bool menuGain4;
        public bool MenuGain4
        {
            get { return menuGain4; }
            set { menuGain4 = value; OnPropertyChanged(); }
        }
        private bool menuGain3;
        public bool MenuGain3
        {
            get { return menuGain3; }
            set { menuGain3 = value; OnPropertyChanged(); }
        }
        private bool menuGain2;
        public bool MenuGain2
        {
            get { return menuGain2; }
            set { menuGain2 = value; OnPropertyChanged(); }
        }
        private bool menuGain1;
        public bool MenuGain1
        {
            get { return menuGain1; }
            set { menuGain1 = value; OnPropertyChanged(); }
        }

        private bool menuAnnonymize;
        public bool MenuAnnonymize
        {
            get { return menuAnnonymize; }
            set { menuAnnonymize = value; OnPropertyChanged(); }
        }
        private bool menuDisplayInfo;
        public bool MenuDisplayInfo
        {
            get { return menuDisplayInfo; }
            set { menuDisplayInfo = value; OnPropertyChanged(); }
        }
        private bool menuGridType;
        public bool MenuGridType
        {
            get { return menuGridType; }
            set { menuGridType = value; OnPropertyChanged(); }
        }
        private bool menuGridFive;
        public bool MenuGridFive
        {
            get { return menuGridFive; }
            set { menuGridFive = value; OnPropertyChanged(); }
        }
        private bool menuGridOne;
        public bool MenuGridOne
        {
            get { return menuGridOne; }
            set { menuGridOne = value; OnPropertyChanged(); }
        }
        private bool menuGridNone;
        public bool MenuGridNone
        {
            get { return menuGridNone; }
            set { menuGridNone = value; OnPropertyChanged(); }
        }
        private bool menuColor;
        public bool MenuColor
        {
            get { return menuColor; }
            set { menuColor = value; OnPropertyChanged(); }
        }
        private bool menuColor1;
        public bool MenuColor1
        {
            get { return menuColor1; }
            set { menuColor1 = value; OnPropertyChanged(); }
        }
        private bool menuColor2;
        public bool MenuColor2
        {
            get { return menuColor2; }
            set { menuColor2 = value; OnPropertyChanged(); }
        }
        private bool menuColor3;
        public bool MenuColor3
        {
            get { return menuColor3; }
            set { menuColor3 = value; OnPropertyChanged(); }
        }
        private bool menuColor4;
        public bool MenuColor4
        {
            get { return menuColor4; }
            set { menuColor4 = value; OnPropertyChanged(); }
        }
        private bool menuZoom;
        public bool MenuZoom
        {
            get { return menuZoom; }
            set { menuZoom = value; OnPropertyChanged(); }
        }
        private bool menuZoomOut;
        public bool MenuZoomOut
        {
            get { return menuZoomOut; }
            set { menuZoomOut = value; OnPropertyChanged(); }
        }
        private bool menuZoomIn;
        public bool MenuZoomIn
        {
            get { return menuZoomIn; }
            set { menuZoomIn = value; OnPropertyChanged(); }
        }
        private bool menuCaliper;
        public bool MenuCaliper
        {
            get { return menuCaliper; }
            set { menuCaliper = value; OnPropertyChanged(); }
        }
        private bool menuCaliperOff;
        public bool MenuCaliperOff
        {
            get { return menuCaliperOff; }
            set { menuCaliperOff = value; OnPropertyChanged(); }
        }
        private bool menuCaliperDuration;
        public bool MenuCaliperDuration
        {
            get { return menuCaliperDuration; }
            set { menuCaliperDuration = value; OnPropertyChanged(); }
        }
        private bool menuCaliperBoth;
        public bool MenuCaliperBoth
        {
            get { return menuCaliperBoth; }
            set { menuCaliperBoth = value; OnPropertyChanged(); }
        }
        private bool menuFilter;
        public bool MenuFilter
        {
            get { return menuFilter; }
            set { menuFilter = value; OnPropertyChanged(); }
        }
        private bool menuFilterNone;
        public bool MenuFilterNone
        {
            get { return menuFilterNone; }
            set { menuFilterNone = value; OnPropertyChanged(); }
        }
        private bool menuFilter40Hz;
        public bool MenuFilter40Hz
        {
            get { return menuFilter40Hz; }
            set { menuFilter40Hz = value; OnPropertyChanged(); }
        }
        private bool menuFilterMuscle;
        public bool MenuFilterMuscle
        {
            get { return menuFilterMuscle; }
            set { menuFilterMuscle = value; OnPropertyChanged(); }
        }




        private float _Gain = 10f;


        public float Gain
        {
            get
            {
                lock (this)
                {
                    return _Gain;
                }
            }
            set
            {
                lock (this)
                {
                    if (value == 40f)
                    {
                        _Gain = value;

                        menuGain4 = true;
                        menuGain3 = false;
                        menuGain2 = false;
                        menuGain1 = false;
                    }
                    else if (value == 20f)
                    {
                        _Gain = value;

                        menuGain4 = false;
                        menuGain3 = true;
                        menuGain2 = false;
                        menuGain1 = false;
                    }
                    else if (value == 10f)
                    {
                        _Gain = value;

                        menuGain4 = false;
                        menuGain3 = false;
                        menuGain2 = true;
                        menuGain1 = false;
                    }
                    else if (value == 5f)
                    {
                        _Gain = value;

                        menuGain4 = false;
                        menuGain3 = false;
                        menuGain2 = false;
                        menuGain1 = true;
                    }
                }
            }
        }

        private string _statusBarText;

        public string StatusBarText
        {
            get { return _statusBarText; }
            set
            {
                _statusBarText = value;
                OnPropertyChanged();
            }
        }

        private string _labelPatient;
        public string LabelPatient
        {
            get { return _labelPatient; }
            set
            {
                _labelPatient = value;
                OnPropertyChanged();
            }
        }
        private string _labelPatientSecond;
        public string LabelPatientSecond
        {
            get { return _labelPatientSecond; }
            set
            {
                _labelPatientSecond = value;
                OnPropertyChanged();
            }
        }
        private string _labelTime;
        public string LabelTime
        {
            get { return _labelTime; }
            set
            {
                _labelTime = value;
                OnPropertyChanged();
            }
        }
        private string _labelDiagnostic;
        public string LabelDiagnostic
        {
            get { return _labelDiagnostic; }
            set
            {
                _labelDiagnostic = value;
                OnPropertyChanged();
            }
        }

        private IECGFormat _CurrentECG = null;
        public IECGFormat CurrentECG
        {
            get
            {
                lock (this)
                {
                    return _CurrentECG;
                }
            }
            set
            {
                lock (this)
                {
                    _Zoom = 1;

                    if ((_CurrentECG != null)
                    &&  (_CurrentECG != value))
                        _CurrentECG.Dispose();

                    if (value == null)
                    {
                        if (_CurrentECG != null)
                            _CurrentECG.Dispose();

                        _CurrentECG = null;
                        _CurrentSignal = null;
                    }
                    else
                    {


                        Gain = 10f;
                        _CurrentECG = value;

                        if (_CurrentECG.Signals.getSignals(out _CurrentSignal) != 0)
                        {
                            this.StatusBarText = "Failed to get signal!";

                            _CurrentECG.Dispose();
                            _CurrentECG = null;
                        }
                        else
                        {
                            if (_CurrentSignal != null)
                            {
                                for (int i = 0, e = _CurrentSignal.NrLeads; i < e; i++)
                                {
                                    ECGTool.NormalizeSignal(_CurrentSignal[i].Rhythm, _CurrentSignal.RhythmSamplesPerSecond);
                                }
                            }

                            Signals sig = _CurrentSignal.CalculateTwelveLeads();
                            if (sig == null)
                                sig = _CurrentSignal.CalculateFifteenLeads();

                            if (sig != null)
                                _CurrentSignal = sig;

                            if (_CurrentSignal.IsBuffered)
                            {
                                BufferedSignals bs = _CurrentSignal.AsBufferedSignals;
                                bs.LoadSignal(bs.RealRhythmStart, bs.RealRhythmStart + 60 * bs.RhythmSamplesPerSecond);
                            }
                            else
                            {
                                int start, end;
                                _CurrentSignal.CalculateStartAndEnd(out start, out end);

                            }
                        }

                        var dt = this._CurrentECGDrawType;
                        MenuLeadFormatRegularCommand.NotifyCanExecuteChanged();
                        MenuLeadFormatFourXThreeCommand.NotifyCanExecuteChanged();
                        MenuLeadFormatFourXThreePlusOneCommand.NotifyCanExecuteChanged();
                        MenuLeadFormatFourXThreePlusThreeCommand.NotifyCanExecuteChanged();
                        MenuLeadFormatSixXTwoCommand.NotifyCanExecuteChanged();
                        MenuLeadFormatMedianCommand.NotifyCanExecuteChanged();

                        var menuLeadFormatRegularIsEnabled = (dt & ECGDraw.ECGDrawType.Regular) != 0;
                        var menuLeadFormatThreeXFourIsEnabled = (dt & ECGDraw.ECGDrawType.ThreeXFour) != 0;
                        var menuLeadFormatThreeXFourPlusOneIsEnabled = (dt & ECGDraw.ECGDrawType.ThreeXFourPlusOne) != 0;
                        var menuLeadFormatThreeXFourPlusThreeIsEnabled = (dt & ECGDraw.ECGDrawType.ThreeXFourPlusThree) != 0;
                        var menuLeadFormatSixXTwoIsEnabled = (dt & ECGDraw.ECGDrawType.SixXTwo) != 0;
                        var menuLeadFormatMedianIsEnabled = (dt & ECGDraw.ECGDrawType.Median) != 0;

                        if ((menuLeadFormatThreeXFour && !menuLeadFormatThreeXFourIsEnabled)
                        ||  (menuLeadFormatThreeXFourPlusOne && !menuLeadFormatThreeXFourPlusOneIsEnabled)
                        ||  (menuLeadFormatThreeXFourPlusThree && !menuLeadFormatThreeXFourPlusThreeIsEnabled)
                        ||  (menuLeadFormatSixXTwo && !menuLeadFormatSixXTwoIsEnabled)
                        ||  (menuLeadFormatMedian && !menuLeadFormatMedianIsEnabled))
                        {
                            CheckLeadFormat(ECGDraw.ECGDrawType.Regular, false);
                        }
                    }

                    _DrawBuffer = null;

                    OnPropertyChanged();
                }
            }
        }

        private Image<Rgba32> DrawBuffer
        {
            get
            {
                lock (this)
                {
                    return _DrawBuffer;
                }
            }
            set
            {
                lock (this)
                {
                    if (_DrawBuffer != null)
                        _DrawBuffer.Dispose();

                    _DrawBuffer = value;
                    OnPropertyChanged();
                }
            }
        }
        public Image<Rgba32> _DrawBuffer = null;

        public ImageSharpImageSource<Rgba32>? DrawBufferSource => DrawBuffer ==null ? null : new ImageSharpImageSource<Rgba32>(DrawBuffer);

        private int _renderWidth;

        public int RenderWidth
        {
            get { return _renderWidth; }
            set
            {
                _renderWidth = value;
                OnPropertyChanged();

            }
        }
        private int _renderHeight;
        public int RenderHeight
        {
            get { return _renderHeight; }
            set
            {
                _renderHeight = value;
                OnPropertyChanged();

            }
        }

        public void Refresh()
        {
            lock (this)
            {
                TopInfo(_CurrentECG);

                var w = this.RenderWidth;
                var h = this.RenderHeight;

                if (_CurrentECG == null)
                {
                    var image = new Image<Rgba32>((int)h, (int)w);

                    image.Mutate(i => i.Fill(SixLabors.ImageSharp.Color.White));
                    _DrawBuffer = image;
                    return;
                }

                if (DrawBuffer == null)
                {


                    int n = 0;
                    int[,] s = { { 782, 492 }, { 1042, 657 }, { 1302, 822 } };

                    for (; n < s.GetLength(0); n++)
                        if ((s[n, 0] > w)
                        ||  (s[n, 1] > h))
                            break;

                    n+=2;

                    // zoom mode on
                    if (_Zoom > 1)
                    {
                        n *= _Zoom;
                        w *= _Zoom;
                        h *= _Zoom;

                        int start, end;

                        if (_DrawType != ECGConversion.ECGDraw.ECGDrawType.Regular)
                        {
                            start = n * 5 * 33 + 1;
                            end = n * 5 * 52 + 1;
                        }
                        else
                        {
                            _CurrentSignal.CalculateStartAndEnd(out start, out end);

                            end = (((end - start) * 25 * n) / _CurrentSignal.RhythmSamplesPerSecond) + 1 + (n * 5);
                            start = int.MaxValue;
                        }

                        if (w > end)
                            w = end;

                        if (w < this.RenderWidth)
                            w = this.RenderWidth;

                        if (h > start)
                            h = start;

                        if (h < this.RenderHeight)
                            h = this.RenderHeight;
                    }

                    _DrawBuffer = new Image<Rgba32>((int)w, (int)h);

                    Signals drawSignal = _CurrentSignal;
                    int nTime = 0;
                    ECGDraw.DpiX = ECGDraw.DpiY = 25.4f * n;



                    if (drawSignal != null)
                    {
                        if (!double.IsNaN(_BottomCutoff))
                        {
                            if (!double.IsNaN(_TopCutoff))
                            {
                                drawSignal = drawSignal.ApplyBandpassFilter(_BottomCutoff, _TopCutoff);
                            }
                            else
                            {
                                drawSignal = drawSignal.ApplyHighpassFilter(_BottomCutoff);
                            }
                        }
                        else if (!double.IsNaN(_TopCutoff))
                        {
                            drawSignal = drawSignal.ApplyLowpassFilter(_TopCutoff);
                        }
                    }

                    int oldSPS = _CurrentSignal.RhythmSamplesPerSecond;

                    DrawBuffer.Mutate(i =>
                    {
                        ECGDraw.DrawECG(i, drawSignal, _DrawType, nTime, 25.0f, _Gain);
                    });

                    OnPropertyChanged(nameof(this.DrawBufferSource));


                }

            }
        }
        public bool IsFileLoaded => this.CurrentECG != null;
        public Visibility FileLoadedControlVisiblity => IsFileLoaded ? Visibility.Visible : Visibility.Collapsed;


        private void MenuViewAction()
        {
        }
        public void TopInfo(IECGFormat format)
        {



            this.MenuCloseCommand.NotifyCanExecuteChanged();
            this.MenuSaveFileCommand.NotifyCanExecuteChanged();
            this.MenuViewCommand.NotifyCanExecuteChanged();

            if ((format == null)
            ||  (format.Demographics == null))
            {
                this.LabelPatient = "";
                this.LabelPatientSecond = "";
                this.LabelTime = "";
                this.LabelDiagnostic = "";
            }
            else
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("Name:       ");

                    if ((format.Demographics.FirstName != null)
                    &&  (format.Demographics.FirstName.Length != 0))
                    {
                        sb.Append(format.Demographics.FirstName);
                        sb.Append(" ");
                    }

                    sb.Append(format.Demographics.LastName);

                    if ((format.Demographics.SecondLastName != null)
                    &&  (format.Demographics.SecondLastName.Length != 0))
                    {
                        sb.Append('-');
                        sb.Append(format.Demographics.SecondLastName);
                    }

                    sb.Append('\n');
                    sb.Append("Patient ID: ");
                    sb.Append(format.Demographics.PatientID);

                    GlobalMeasurements gms;

                    if ((format.GlobalMeasurements != null)
                    &&  (format.GlobalMeasurements.getGlobalMeasurements(out gms) == 0)
                    &&  (gms.measurment != null)
                    &&  (gms.measurment.Length > 0)
                    &&  (gms.measurment[0] != null))
                    {
                        int ventRate = (gms.VentRate == GlobalMeasurement.NoValue) ? 0 : (int)gms.VentRate,
                            PRint = (gms.PRint == GlobalMeasurement.NoValue) ? 0 : (int)gms.measurment[0].PRint,
                            QRSdur = (gms.QRSdur == GlobalMeasurement.NoValue) ? 0 : (int)gms.measurment[0].QRSdur,
                            QT = (gms.QTdur == GlobalMeasurement.NoValue) ? 0 : (int)gms.measurment[0].QTdur,
                            QTc = (gms.QTc == GlobalMeasurement.NoValue) ? 0 : (int)gms.QTc;

                        sb.Append("\n\nVent rate:      ");
                        PrintValue(sb, ventRate, 3);
                        sb.Append(" BPM");

                        sb.Append("\nPR int:         ");
                        PrintValue(sb, PRint, 3);
                        sb.Append(" ms");

                        sb.Append("\nQRS dur:        ");
                        PrintValue(sb, QRSdur, 3);
                        sb.Append(" ms");

                        sb.Append("\nQT\\QTc:     ");
                        PrintValue(sb, QT, 3);
                        sb.Append('/');
                        PrintValue(sb, QTc, 3);
                        sb.Append(" ms");

                        sb.Append("\nP-R-T axes: ");
                        sb.Append((gms.measurment[0].Paxis != GlobalMeasurement.NoAxisValue) ? gms.measurment[0].Paxis.ToString() : "999");
                        sb.Append(' ');
                        sb.Append((gms.measurment[0].QRSaxis != GlobalMeasurement.NoAxisValue) ? gms.measurment[0].QRSaxis.ToString() : "999");
                        sb.Append(' ');
                        sb.Append((gms.measurment[0].Taxis != GlobalMeasurement.NoAxisValue) ? gms.measurment[0].Taxis.ToString() : "999");
                    }

                    this.LabelPatient = sb.ToString();

                    sb = new StringBuilder();

                    sb.Append("DOB:  ");

                    ECGConversion.ECGDemographics.Date birthDate = format.Demographics.PatientBirthDate;
                    if (birthDate != null)
                    {
                        sb.Append(birthDate.Day.ToString("00"));
                        sb.Append(birthDate.Month.ToString("00"));
                        sb.Append(birthDate.Year.ToString("0000"));
                    }

                    sb.Append("\nAge:  ");

                    ushort ageVal;
                    ECGConversion.ECGDemographics.AgeDefinition ad;

                    if (format.Demographics.getPatientAge(out ageVal, out ad) == 0)
                    {
                        sb.Append(ageVal);

                        if (ad != ECGConversion.ECGDemographics.AgeDefinition.Years)
                        {
                            sb.Append(" ");
                            sb.Append(ad.ToString());
                        }
                    }
                    else
                        sb.Append("0");

                    sb.Append("\nGen:  ");
                    if (format.Demographics.Gender != ECGConversion.ECGDemographics.Sex.Null)
                        sb.Append(format.Demographics.Gender.ToString());
                    sb.Append("\nDep:  ");
                    sb.Append(format.Demographics.AcqDepartment);

                    this.LabelPatientSecond = sb.ToString();

                    DateTime dt = format.Demographics.TimeAcquisition;

                    this.LabelTime = (dt.Year > 1000) ? dt.ToString("dd/MM/yyyy HH:mm:ss") : "Time Unknown";

                    Statements stat;

                    if ((format.Diagnostics != null)
                    &&  (format.Diagnostics.getDiagnosticStatements(out stat) == 0))
                    {
                        if ((stat.statement != null)
                        &&  (stat.statement.Length > 0))
                        {
                            sb = new StringBuilder();

                            foreach (string temp in stat.statement)
                            {
                                sb.Append(temp);
                                sb.Append("\r\n");
                            }

                            string temp2 = stat.statement[stat.statement.Length-1];

                            if ((temp2 != null)
                            &&  !temp2.StartsWith("confirmed by", StringComparison.InvariantCultureIgnoreCase)
                            &&  !temp2.StartsWith("interpreted by", StringComparison.InvariantCultureIgnoreCase)
                            &&  !temp2.StartsWith("reviewed by", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if ((format.Demographics.OverreadingPhysician != null)
                                &&  (format.Demographics.OverreadingPhysician.Length != 0))
                                {
                                    if (stat.confirmed)
                                        sb.Append("Confirmed by ");
                                    else if (stat.interpreted)
                                        sb.Append("Interpreted by ");
                                    else
                                        sb.Append("Reviewed by ");

                                    sb.Append(format.Demographics.OverreadingPhysician);

                                }
                                else
                                    sb.Append("UNCONFIRMED AUTOMATED INTERPRETATION");
                            }

                            this.LabelDiagnostic = sb.ToString();
                        }
                    }
                    else
                    {
                        this.LabelDiagnostic = "";
                    }
                }
                catch
                {
                    this.LabelPatient = "";
                    this.LabelPatientSecond = "";
                    this.LabelTime = "";
                    this.LabelDiagnostic = "";

                    this.StatusBarText = "Open failed (due to an exception)!";

                    CurrentECG = null;
                }
            }
        }

        private static void PrintValue(StringBuilder sb, int val, int len)
        {
            int temp = sb.Length;
            sb.Append(val.ToString());
            if ((sb.Length - temp) < len)
                sb.Append(' ', len - (sb.Length - temp));
        }


        private void MenuZoomInAction()
        {
            ZoomIn(
    RenderWidth >> 1,
    RenderHeight >> 1);
        }

        private void MenuZoomOutAction()
        {
            ZoomOut();
        }

        private void ZoomOut()
        {
            if (_Zoom > 1)
            {

                _Zoom >>= 1;

                DrawBuffer = null;
                Refresh();
            }
        }

        private void ZoomIn(int x, int y)
        {
            if (_Zoom < 4)
            {

                _Zoom <<= 1;



                DrawBuffer = null;
                Refresh();
            }
            else
            {
                _Zoom = 1;
                DrawBuffer = null;
                Refresh();
            }
        }




        private void MenuOpenFileAction()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Any ECG File (*.*)|*.*");

                int i = 0;

                System.Collections.ArrayList supportedList = new ArrayList();

                foreach (string format in ECGConverter.Instance.getSupportedFormatsList())
                {
                    string extension = ECGConverter.Instance.getExtension(i);

                    if (ECGConverter.Instance.hasUnknownReaderSupport(i++))
                    {
                        supportedList.Add(format);

                        sb.Append('|');

                        sb.Append(format);
                        sb.Append(" File");

                        if (extension == null)
                            extension = "ecg";

                        sb.Append(" (*.");
                        sb.Append(extension);
                        sb.Append(")|*.");
                        sb.Append(extension);
                    }
                }

                saveECGFileDialog.Filter = sb.ToString();

                openECGFileDialog.Title = "Open ECG";
                openECGFileDialog.Filter = sb.ToString();
                var dr = this.openECGFileDialog.ShowDialog();

                if ((dr == true)
                &&  File.Exists(this.openECGFileDialog.FileName))
                {
                    IECGFormat format = null;

                    if (openECGFileDialog.FilterIndex > 1)
                    {
                        string fmt = (string)supportedList[openECGFileDialog.FilterIndex - 2];
                        IECGReader reader = ECGConverter.Instance.getReader(fmt);
                        ECGConfig cfg = ECGConverter.Instance.getConfig(fmt);

                        if (cfg != null)
                        {
                            return;
                        }

                        format = reader.Read(this.openECGFileDialog.FileName, 0, cfg);
                    }
                    else
                    {
                        if (_ECGReader == null)
                            _ECGReader = new UnknownECGReader();

                        format = _ECGReader.Read(this.openECGFileDialog.FileName);
                    }

                    if (format != null)
                    {
                        CurrentECG = format;

                        if (CurrentECG != null)
                        {
                            this.StatusBarText = "Opened file!";
                            this.StatusBarText = "Opened file!";

                        }
                    }
                    else
                    {
                        CurrentECG = null;

                        this.StatusBarText = "Failed to open file!";
                    }
                }
                else
                {
                    this.StatusBarText = "";
                }

                this.Refresh();
            }
            catch (Exception ex)
            {
                CurrentECG = null;

                MessageBox.Show(App.Current.MainWindow, ex.ToString(), ex.Message, MessageBoxButton.OK);
            }
        }

        private void MenuSaveFileAction()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Current Format (*.*)|*.*");

                int i = 0;

                string[] supportedList = ECGConverter.Instance.getSupportedFormatsList();

                foreach (string format in supportedList)
                {
                    string extension = ECGConverter.Instance.getExtension(i++);

                    sb.Append('|');

                    sb.Append(format);
                    sb.Append(" File");

                    if (extension == null)
                        extension = "ecg";

                    sb.Append(" (*.");
                    sb.Append(extension);
                    sb.Append(")|*.");
                    sb.Append(extension);
                }

                saveECGFileDialog.Title = "Save ECG";
                saveECGFileDialog.Filter = sb.ToString();
                saveECGFileDialog.OverwritePrompt = true;
                var dr = saveECGFileDialog.ShowDialog();

                if (dr == true)
                {
                    int index = saveECGFileDialog.FilterIndex - 2;

                    IECGFormat writeFile = CurrentECG;

                    if (index >= 0)
                    {
                        ECGConfig cfg = ECGConverter.Instance.getConfig(index);

                        if (cfg != null)
                        {
                            return;
                        }

                        try
                        {
                            if (CurrentECG.GetType() != ECGConverter.Instance.getType(index))
                            {
                                if (((ECGConverter.Instance.Convert(CurrentECG, index, cfg, out writeFile) != 0)
                                ||  !writeFile.Works())
                                &&  (writeFile != null))
                                {
                                    writeFile.Dispose();
                                    writeFile = null;
                                }
                            }
                        }
                        catch
                        {
                            if (writeFile != null)
                            {
                                writeFile.Dispose();
                                writeFile = null;
                            }
                        }

                        if (writeFile == null)
                        {
                            MessageBox.Show(App.Current.MainWindow, "Converting of file has failed!", "Converting failed!", MessageBoxButton.OK);

                            return;
                        }
                    }

                    ECGWriter.Write(writeFile, saveECGFileDialog.FileName, true);

                    if (writeFile != CurrentECG)
                        writeFile.Dispose();

                    if (ECGWriter.getLastError() != 0)
                    {
                        MessageBox.Show(App.Current.MainWindow, ECGWriter.getLastErrorMessage(), "Writing of file failed!", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(App.Current.MainWindow, ex.ToString(), ex.Message, MessageBoxButton.OK);
            }
        }



        private void MenuCloseAction()
        {
            try
            {
                if (CurrentECG != null)
                {
                    CurrentECG.Dispose();
                    CurrentECG = null;

                    this.StatusBarText = "";

                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(App.Current.MainWindow, ex.ToString(), ex.Message, MessageBoxButton.OK);
            }
        }

        private void MenuAddPluginFileAction()
        {
            try
            {
                openECGFileDialog.Title = "Open Plugin";
                openECGFileDialog.Filter = "Assembly file (*.dll)|*.dll";
                var dr = openECGFileDialog.ShowDialog();

                if (dr == true)
                {
                    if (ECGConverter.AddPlugin(openECGFileDialog.FileName) != 0)
                        MessageBox.Show(App.Current.MainWindow, "Selected plugin file is not supported!", "Unsupported plugin!", MessageBoxButton.OK);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(App.Current.MainWindow, ex.ToString(), ex.Message, MessageBoxButton.OK);
            }
        }


        private void MenuLeadFormatRegularAction()
        {
            CheckLeadFormat(ECGDraw.ECGDrawType.Regular, true);
        }

        private void MenuLeadFormatFourXThreeAction()
        {
            CheckLeadFormat(ECGDraw.ECGDrawType.ThreeXFour, true);
        }

        private void MenuLeadFormatFourXThreePlusOneAction()
        {
            CheckLeadFormat(ECGDraw.ECGDrawType.ThreeXFourPlusOne, true);
        }

        private void MenuLeadFormatFourXThreePlusThreeAction()
        {
            CheckLeadFormat(ECGDraw.ECGDrawType.ThreeXFourPlusThree, true);
        }

        private void MenuLeadFormatSixXTwoAction()
        {
            CheckLeadFormat(ECGDraw.ECGDrawType.SixXTwo, true);
        }

        private void MenuLeadFormatMedianAction()
        {
            CheckLeadFormat(ECGDraw.ECGDrawType.Median, true);
        }

        private void CheckLeadFormat(ECGDraw.ECGDrawType lt, bool refresh)
        {
            if (lt != _DrawType)
            {
                menuLeadFormatRegular = false;
                menuLeadFormatSixXTwo = false;
                menuLeadFormatThreeXFour = false;
                menuLeadFormatThreeXFourPlusOne = false;
                menuLeadFormatThreeXFourPlusThree = false;
                menuLeadFormatMedian = false;

                switch (lt)
                {
                    case ECGDraw.ECGDrawType.Regular:
                        menuLeadFormatRegular = true;
                        break;
                    case ECGDraw.ECGDrawType.SixXTwo:
                        menuLeadFormatSixXTwo = true;
                        break;
                    case ECGDraw.ECGDrawType.ThreeXFour:
                        menuLeadFormatThreeXFour = true;
                        break;
                    case ECGDraw.ECGDrawType.ThreeXFourPlusOne:
                        menuLeadFormatThreeXFourPlusOne = true;
                        break;
                    case ECGDraw.ECGDrawType.ThreeXFourPlusThree:
                        menuLeadFormatThreeXFourPlusThree = true;
                        break;
                    case ECGDraw.ECGDrawType.Median:
                        menuLeadFormatMedian = true;
                        break;
                    default:
                        menuLeadFormatRegular = true;

                        lt = ECGDraw.ECGDrawType.Regular;
                        break;
                }

                _DrawType = lt;

                if (refresh)
                {
                    DrawBuffer = null;

                    this.Refresh();
                }
            }
        }

        private void MenuGain1Action()
        {
            if (Gain != 5f)
            {
                Gain = 5f;

                DrawBuffer = null;
            }

            this.Refresh();
        }

        private void MenuGain2Action()
        {
            if (Gain != 10f)
            {
                Gain = 10f;

                DrawBuffer = null;
            }

            this.Refresh();
        }

        private void MenuGain3Action()
        {
            if (Gain != 20f)
            {
                Gain = 20f;

                DrawBuffer = null;
            }

            this.Refresh();
        }

        private void MenuGain4Action()
        {
            if (Gain != 40f)
            {
                Gain = 40f;

                DrawBuffer = null;
            }

            this.Refresh();
        }

        private void ECGTimeScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            lock (this)
            {
                _DrawBuffer = null;
            }

            this.Refresh();
        }


        private void MenuAnnonymizeAction()
        {
            lock (this)
            {
                _CurrentECG.Anonymous();
                _DrawBuffer = null;
            }

            this.Refresh();
        }

        private void MenuDisplayInfoAction()
        {
            ECGDraw.DisplayInfo = menuDisplayInfo = !menuDisplayInfo;

            lock (this)
            {
                _DrawBuffer = null;
            }

            this.Refresh();
        }

        private void MenuGridOneAction()
        {
            menuGridNone = false;
            menuGridOne = true;
            menuGridFive = false;

            ECGDraw.DisplayGrid = ECGDraw.GridType.OneMillimeters;

            lock (this)
            {
                _DrawBuffer = null;
            }

            this.Refresh();
        }

        private void MenuGridFiveAction()
        {
            menuGridNone = false;
            menuGridOne = false;
            menuGridFive = true;

            ECGDraw.DisplayGrid = ECGDraw.GridType.FiveMillimeters;

            lock (this)
            {
                _DrawBuffer = null;
            }

            this.Refresh();
        }

        private void MenuGridNoneAction()
        {
            menuGridNone = true;
            menuGridOne = false;
            menuGridFive = false;

            ECGDraw.DisplayGrid = ECGDraw.GridType.None;

            lock (this)
            {
                _DrawBuffer = null;
            }

            this.Refresh();
        }




        private void LoadUrlDelayed(object obj)
        {
            if ((obj != null)
            &&  (obj is string))
            {
                System.Diagnostics.Process process = null;

                try
                {
                    Thread.Sleep(1500);

                    process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = "rundll32.exe";
                    process.StartInfo.Arguments = "url.dll,FileProtocolHandler " + (string)obj;
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                    process.WaitForExit(5000);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error!", MessageBoxButton.OK);
                }
                finally
                {
                    if (process != null)
                        process.Dispose();
                }
            }
        }

        private void SetColors(int kind)
        {

        }


        private void MenuColor1Action()
        {
            SetColors(0);
        }

        private void MenuColor2Action()
        {
            SetColors(1);
        }

        private void MenuColor3Action()
        {
            SetColors(2);
        }

        private void MenuColor4Action()
        {
            SetColors(3);
        }

        private void MenuCaliperOffAction()
        {
            menuCaliperOff = true;
            menuCaliperDuration = false;
            menuCaliperBoth = false;
        }

        private void MenuCaliperDurationAction()
        {
            menuCaliperOff = false;
            menuCaliperDuration = true;
            menuCaliperBoth = false;
        }

        private void MenuCaliperBothAction()
        {
            menuCaliperOff = false;
            menuCaliperDuration = false;
            menuCaliperBoth = true;
        }




        private void MenuFilterNoneAction()
        {
            menuFilterNone = true;
            menuFilter40Hz = false;
            menuFilterMuscle = false;

            _BottomCutoff = double.NaN;
            _TopCutoff = double.NaN;

            DrawBuffer = null;
            this.Refresh();
        }



        private void MenuFilter40HzAction()
        {
            menuFilterNone = false;
            menuFilter40Hz = true;
            menuFilterMuscle = false;

            _BottomCutoff = 0.05;
            _TopCutoff = 40.0;

            DrawBuffer = null;
            this.Refresh();
        }



        private void MenuFilterMuscleAction()
        {
            menuFilterNone = false;
            menuFilter40Hz = false;
            menuFilterMuscle = true;

            _BottomCutoff = 0.05;
            _TopCutoff = 35.0;

            DrawBuffer = null;
            this.Refresh();
        }

        public RelayCommand MenuZoomInCommand { get; set; }
        public RelayCommand MenuZoomOutCommand { get; set; }
        public RelayCommand MenuOpenFileCommand { get; set; }
        public RelayCommand MenuSaveFileCommand { get; set; }
        public RelayCommand MenuViewCommand { get; set; }
        public RelayCommand MenuCloseCommand { get; set; }
        public RelayCommand MenuAddPluginFileCommand { get; set; }

        public RelayCommand MenuLeadFormatRegularCommand { get; set; }
        public RelayCommand MenuLeadFormatFourXThreeCommand { get; set; }
        public RelayCommand MenuLeadFormatFourXThreePlusOneCommand { get; set; }
        public RelayCommand MenuLeadFormatFourXThreePlusThreeCommand { get; set; }
        public RelayCommand MenuLeadFormatSixXTwoCommand { get; set; }
        public RelayCommand MenuLeadFormatMedianCommand { get; set; }
        public RelayCommand MenuGain1Command { get; set; }
        public RelayCommand MenuGain2Command { get; set; }
        public RelayCommand MenuGain3Command { get; set; }
        public RelayCommand MenuGain4Command { get; set; }

        public RelayCommand MenuAnnonymizeCommand { get; set; }
        public RelayCommand MenuDisplayInfoCommand { get; set; }
        public RelayCommand MenuGridOneCommand { get; set; }
        public RelayCommand MenuGridFiveCommand { get; set; }
        public RelayCommand MenuGridNoneCommand { get; set; }

        public RelayCommand MenuColor1Command { get; set; }
        public RelayCommand MenuColor2Command { get; set; }
        public RelayCommand MenuColor3Command { get; set; }
        public RelayCommand MenuColor4Command { get; set; }
        public RelayCommand MenuCaliperOffCommand { get; set; }
        public RelayCommand MenuCaliperDurationCommand { get; set; }
        public RelayCommand MenuCaliperBothCommand { get; set; }
        public RelayCommand MenuFilterNoneCommand { get; set; }
        public RelayCommand MenuFilter40HzCommand { get; set; }
        public RelayCommand MenuFilterMuscleCommand { get; set; }

    }
}
