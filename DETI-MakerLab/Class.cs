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
        private List<Professor> _managers = new List<Professor>();

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
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid ClassName");
                _className = value; 
            }
        }

        public List<Professor> Managers
        {
            get { return _managers; }
        }

        public bool addManager(Professor manager)
        {
            if (manager == null)
                return false;
            _managers.Add(manager);
            return true;
        }

        public bool removeManager(Professor manager)
        {
            if (_managers.Contains(manager))
            {
                _managers.Remove(manager);
                return true;
            }
            return false;
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
}
