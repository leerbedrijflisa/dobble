using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisa.Dobble.Interfaces
{
    public interface IImageResizerService
    {
        byte[] ResizeImage(byte[] imageData, float width, float height);
    }
}
