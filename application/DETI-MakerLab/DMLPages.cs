using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    public interface DMLPages
    {
        // Method that checks if the page fields are empty or not
        // Ãnd force user to check fields loss
        bool isEmpty();
    }
}
