using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    [Serializable()]
    class Class
    {
        private int _classID;
        private String _className;

        public int ClassID
        {
            get { return _classID; }
            set { _classID = value; }
        }

        public String ClassName
        {
            get { return _className; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid ClassName");
                _className = value; 
            }
        }

        public override String ToString()
        {
            return "Class: " + ClassName.ToString();
        }

        public Class(int ClassID, String ClassName)
        {
            this.ClassID = ClassID;
            this.ClassName = ClassName;
        }
    }

    [Serializable()]
    class ClassManager
    {
        private int _classID;
        private int _professorNumMec;

        public int ClassID
        {
            get { return _classID; }
            set { _classID = value; }
        }

        public int ProfessorNumMec
        {
            get { return _professorNumMec; }
            set
            {
                if (value > 100000)
                    throw new Exception("Invalid NumMec");
                _professorNumMec = value;
            }
        }

        public override String ToString()
        {
            return "Manager: " + ProfessorNumMec.ToString() + " of class " + ClassID.ToString();
        }

        public ClassManager(int ClassID, int ProfessorNumMec)
        {
            this.ClassID = ClassID;
            this.ProfessorNumMec = ProfessorNumMec;
        }

    }
}
