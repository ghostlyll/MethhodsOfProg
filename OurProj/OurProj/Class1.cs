using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurProj
{
    public interface Game
    {
        bool IsPossibleToConstruct();
        void SaveAsXML();
        void SaveAnswers();
        void LoadFromXML();
        void PlayGame();
    }
}
