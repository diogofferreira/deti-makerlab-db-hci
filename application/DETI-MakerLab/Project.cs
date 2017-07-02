using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    [Serializable()]
    public class Project
    {
        private int _projectID;
        private String _projectName;
        private String _projectDescription;
        private String _miniDescription;
        private Class _class;
        private String _className;
        private List<DMLUser> _workers = new List<DMLUser>();


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
            set
            {
                _projectDescription = value;
                _miniDescription = value.Length > 30 ? value.Substring(0, 30) + "..." : value;
            }
        }

        public String MiniDescription
        {
            get { return _miniDescription; }
            set { _miniDescription = value; }
        }

        public Class ProjectClass
        {
            get { return _class; }
            set { _class = value; }
        }

        public String ClassName
        {
            get { return _className; }
            set { _className = value; }
        }

        public List<DMLUser> Workers
        {
            get { return _workers; }
        }

        public bool hasWorker(DMLUser user)
        {
            foreach (DMLUser worker in _workers)
                if (worker.NumMec == user.NumMec)
                    return true;
            return false;
        }

        public int workerChanges(int numMec, int roleID)
        {
            foreach (DMLUser worker in _workers)
            {
                if (worker.NumMec == numMec)
                {
                    if (worker.RoleID == roleID)
                        return 0;
                    else
                        return 1;
                }
            }
            return 2;
        }

        public void addWorker(DMLUser worker)
        {
            if (worker == null)
                throw new Exception("Trying to add invalid user to project!");
            _workers.Add(worker);
        }

        public bool removeWorker(DMLUser worker)
        {
            if (_workers.Contains(worker))
            {
                _workers.Remove(worker);
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return ProjectName;
        }

        public Project(int ProjectID, String ProjectName, String ProjectDescription)
        {
            this.ProjectID = ProjectID;
            this.ProjectName = ProjectName;
            this.ProjectDescription = ProjectDescription;
            this.ProjectClass = null;
        }

        public Project(int ProjectID, String ProjectName, String ProjectDescription, Class ProjectClass)
        {
            this.ProjectID = ProjectID;
            this.ProjectName = ProjectName;
            this.ProjectDescription = ProjectDescription;
            this.ProjectClass = ProjectClass;
        }
    }

    [Serializable()]
    public class Role
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
