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
        private int _projectID;
        private int _userID;
        private DateTime _reqDate;
        private List<int> _resources = new List<int>();

        public int RequisitionID
        {
            get { return _requisitionID; }
            set { _requisitionID = value; }
        }

        public int ProjectID
        {
            get { return _projectID; }
            set { _projectID = value; }
        }

        public int UserID
        {
            get { return _userID; }
            set
            {
                if (value > 100000)
                    throw new Exception("Invalid NumMec");
                _userID = value;
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

        public Requisition(int RequisitionID, int ProjectID, int UserID, DateTime ReqDate)
        {
            this.RequisitionID = RequisitionID;
            this.ProjectID = ProjectID;
            this.UserID = UserID;
            this.ReqDate = ReqDate;
        }
    }
}
