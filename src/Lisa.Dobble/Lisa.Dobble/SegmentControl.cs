using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble
{
    public class SegmentControl : ContentView
    {
        private StackLayout _layout;

        private Color _tintColor = Color.Black;
        public Color TintColor
        {
            get { return _tintColor; }
            set
            {
                _tintColor = value;
                if (_layout == null)
                    return;
                _layout.BackgroundColor = _tintColor;

                for (int iBtn = 0; iBtn < _layout.Children.Count; iBtn++)
                {
                    setSelectedState(iBtn, iBtn == _selectedSegment, true);
                }
            }
        }

        public SegmentControl()
            : base()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.Start;
            _layout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(1),
                Spacing = 1
            };
            Padding = new Thickness(0, 0);
            _selectedSegment = 0;
            Content = _layout;
            _clickedCommand = new Command(setSelectedSegment);

        }

        public void AddSegment(string segmentText)
        {
            Button b = new Button()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BorderColor = TintColor,
                BorderRadius = 0,
                BorderWidth = 0,
                Text = segmentText,
                TextColor = TintColor,
                BackgroundColor = Color.White,
                Command = _clickedCommand,
            };
            b.CommandParameter = _layout.Children.Count;
            _layout.BackgroundColor = TintColor;
            _layout.Children.Add(b);
            setSelectedState(_layout.Children.Count - 1, _layout.Children.Count - 1 == _selectedSegment);
        }

        public event EventHandler<int> SelectedSegmentChanged;

        private Command _clickedCommand;
        private void setSelectedSegment(object o)
        {
            int selectedIndex = (int)o;
            SelectedSegment = selectedIndex;
            if (SelectedSegmentChanged != null)
                SelectedSegmentChanged(this, selectedIndex);
        }

        public void SetSegmentText(int iSegment, string segmentText)
        {
            if (iSegment >= _layout.Children.Count || iSegment < 0)
                throw new IndexOutOfRangeException("SetSegmentText: Attempted to change segment text for a segment doesn't exist.");

            ((Button)_layout.Children[iSegment]).Text = segmentText;
        }

        private int _selectedSegment = 0;
        public int SelectedSegment
        {
            get
            {
                return _selectedSegment;
            }
            set
            {
                //reset the original selected segment
                if (value == _selectedSegment)
                    return;

                setSelectedState(_selectedSegment, false);
                _selectedSegment = value;
                if (value < 0 || value >= _layout.Children.Count)
                    return;
                setSelectedState(_selectedSegment, true);

            }
        }

        private void setSelectedState(int indexer, bool isSelected, bool setBorderColor = false)
        {
            if (_layout.Children.Count <= indexer)
                return; //Out of bounds

            Button vw = (Button)_layout.Children[indexer];
            if (isSelected)
            {
                vw.BackgroundColor = TintColor;
                vw.TextColor = Color.White;
            }
            else
            {
                vw.BackgroundColor = Color.White;
                vw.TextColor = TintColor;
            }
            if (setBorderColor)
                vw.BorderColor = TintColor;

        }
    }
}
