using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    [Serializable()]
    class Delivery
    {
        private int _deliveryID;
        private int _projectID;
        private int _userID;
        private DateTime _delDate;
        private List<int> _resources = new List<int>();

        public int DeliveryID
        {
            get { return _deliveryID; }
            set { _deliveryID = value; }
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

        public DateTime DelDate
        {
            get { return _delDate; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid ReqDate");
                _delDate = value;
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
            return "Requisition " + DeliveryID.ToString();
        }

        public Delivery(int DeliveryID, int ProjectID, int UserID, DateTime DelDate)
        {
            this.DeliveryID = DeliveryID;
            this.ProjectID = ProjectID;
            this.UserID = UserID;
            this.DelDate = DelDate;
        }
    }
}
