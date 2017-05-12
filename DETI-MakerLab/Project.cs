using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    [Serializable()]
    class Project
    {
        private int _projectID;
        private String _projectName;
        private String _projectDescription;
        private int _classID;
        private String _className;
        private List<int[]> _workers = new List<int[]>();

        public int ProjectID
        {
            get { return _projectID; }
            set { _projectID = value; }
        }

        public String ProjectName
        {
            get { return _projectName; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid ProjectName");
                _projectName = value;
            }
        }

        public String ProjectDescription
        {
            get { return _projectDescription; }
            set { _projectDescription = value; }
        }

        public int ClassID
        {
            get { return _classID; }
            set { _classID = value; }
        }

        public String ClassName
        {
            get { return _className; }
            set { _className = value; }
        }

        public List<int[]> Workers
        {
            get { return _workers; }
        }

        public void addWorker(int UserID, int RoleID)
        {
            int[] worker = { UserID, RoleID };
            _workers.Add(worker);
        }

        public bool removeWorker(int UserID, int RoleID)
        {
            int[] worker = { UserID, RoleID };
            if (_workers.Contains(worker))
            {
                _workers.Remove(worker);
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return "Project " + ProjectName;
        }

        public Project(int ProjectID, String ProjectName, String ProjectDescription)
        {
            this.ProjectID = ProjectID;
            this.ProjectName = ProjectName;
            this.ProjectDescription = ProjectDescription;
            this.ClassID = -1;
        }

        public Project(int ProjectID, String ProjectName, String ProjectDescription, int ClassID)
        {
            this.ProjectID = ProjectID;
            this.ProjectName = ProjectName;
            this.ProjectDescription = ProjectDescription;
            this.ClassID = ClassID;
        }

        public Project(int ProjectID, String ProjectName, String ProjectDescription, String ClassName)
        {
            this.ProjectID = ProjectID;
            this.ProjectName = ProjectName;
            this.ProjectDescription = ProjectDescription;
            this.ClassName = ClassName;
        }
    }

    [Serializable()]
    class Role
    {
        private int _roleID;
        private String _roleDescription;

        public int RoleID
        {
            get { return _roleID; }
            set { _roleID = value; }
        }

        public String RoleDescription
        {
            get { return _roleDescription; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid ClassName");
                _roleDescription = value;
            }
        }

        public override String ToString()
        {
            return "Role: " + RoleDescription.ToString();
        }

        public Role(int RoleID, String RoleDescription)
        {
            this.RoleID = RoleID;
            this.RoleDescription = RoleDescription;
        }
    }
}
