using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble
{
    public class FancyFrame : Frame {

        public event EventHandler OnSwipe;

        public void SendSwipe()
        {
            var handler = OnSwipe;
            if(handler!=null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    
    }
}       
