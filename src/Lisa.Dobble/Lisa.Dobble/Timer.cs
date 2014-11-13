using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble
{
    public class Timer
    {
        public Timer()
        {
            _isContinuous = true;
        }

        public bool IsContinuous
        {
            get
            {
                return _isContinuous;
            }
            set
            {
                if (_callback != null && _callback.IsRunning)
                {
                    _callback.IsContinuous = value;
                }

                _isContinuous = value;
            }
        }

        public bool IsRunning
        {
            get
            {
                return _callback != null && _callback.IsRunning;
            }
        }

        public void Start(int milliseconds)
        {
            _callback = new OneOffTimer(milliseconds);
            _callback.IsContinuous = _isContinuous;
            _callback.Tick += OnTick;
        }

        public void Stop()
        {
            if (_callback != null)
            {
                _callback.IsRunning = false;
            }
        }

        public event EventHandler Tick;

        private void OnTick(object sender, EventArgs e)
        {
            if (Tick != null)
            {
                Tick(this, e);
            }
        }

        private OneOffTimer _callback;
        private bool _isContinuous;

        private class OneOffTimer
        {
            public OneOffTimer(int milliseconds)
            {
                IsRunning = true;
                Device.StartTimer(new TimeSpan(0, 0, 0, 0, milliseconds), OnTick);
            }

            public bool IsRunning { get; set; }
            public bool IsContinuous { get; set; }

            public bool OnTick()
            {
                if (IsRunning)
                {
                    if (Tick != null)
                    {
                        Tick(this, EventArgs.Empty);
                    }

                    IsRunning = IsContinuous;
                    return IsContinuous;
                }

                return false;
            }

            public event EventHandler Tick;
        }
    }
}
