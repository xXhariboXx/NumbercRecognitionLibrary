using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Input;

namespace WKiRO.MainApplication
{
    public class DrawingWindowViewModel : INotifyPropertyChanged
    {
        public DrawingWindowViewModel()
        {
            _model = new DrawingWindowModel();
        }
        public bool SondeApplicationActive
        {
            get { return _model.SondeApplicationActive; }
            set
            {
                if (value == true)
                {
                    DistributionSequenceActive = false;
                    BarrFeaturesActive = false;
                }

                _model.SondeApplicationActive = value;
                OnPropertyChanged("SondeApplicationActive");
            }
        }

        public bool DistributionSequenceActive
        {
            get { return _model.DistributionSequenceActive; }
            set
            {
                if (value == true)
                {
                    SondeApplicationActive = false;
                    BarrFeaturesActive = false;
                }

                _model.SondeApplicationActive = value;
                OnPropertyChanged("DistributionSequenceActive");
            }
        }

        public bool BarrFeaturesActive
        {
            get { return _model.BarrFeaturesActive; }
            set
            {
                if (value == true)
                {
                    SondeApplicationActive = false;
                    DistributionSequenceActive = false;
                }

                _model.SondeApplicationActive = value;
                OnPropertyChanged("BarrFeaturesActive");
            }
        }

        public StrokeCollection Strokes
        {
            get { return _model.Strokes; }
            set
            {
                _strokes = value;
                OnPropertyChanged("Strokes");
            }
        }

        public string RecognitionOutput
        {
            get { return _recognitionOutput; }
            set
            {
                _recognitionOutput = value;
                OnPropertyChanged("RecognitionOutput");
            }
        }

        public ICommand ClearInkButtonCommand
        {
            get
            {
                if (_clearInkButtonCommand == null)
                {
                    _clearInkButtonCommand = new ClearInkButtonCommand(this);
                }
                return _clearInkButtonCommand;
            }
        }

        public int CanvasWidth { get; internal set; }
        public int CanvasHeight { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ICommand _clearInkButtonCommand;

        private DrawingWindowModel _model;
        private StrokeCollection _strokes;

        private string _recognitionOutput;
    }
}
