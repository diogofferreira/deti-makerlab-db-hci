using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    [Serializable()]
    class Requisition
    {
        private int _requisitionID;
        private Project _reqProject;
        private DMLUser _user;
        private DateTime _reqDate;
        private List<int> _resources = new List<int>();

        public int RequisitionID
        {
            get { return _requisitionID; }
            set { _requisitionID = value; }
        }

        public Project ReqProject
        {
            get { return _reqProject; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid Project");
                _reqProject = value;
            }
        }

        public DMLUser User
        {
            get { return _user; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid User");
                _user = value;
            }
        }

        public DateTime ReqDate
        {
            get { return _reqDate; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid ReqDate");
                _reqDate = value;
            }
        }

        public List<int> Resources
        {
            get { return _resources; }
        }

        public void addResource(int resourceID)
        {
            Resources.Add(resourceID);
        }

        public override string ToString()
        {
            return "Requisition " + RequisitionID.ToString();
        }

        public Requisition(int RequisitionID, Project ReqProject, DMLUser User, DateTime ReqDate)
        {
            this.RequisitionID = RequisitionID;
            this.ReqProject = ReqProject;
            this.User = User;
            this.ReqDate = ReqDate;
        }
    }
}
