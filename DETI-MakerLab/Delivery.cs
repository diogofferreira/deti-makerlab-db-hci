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
        private Project _delProject;
        private DMLUser _user;
        private DateTime _delDate;
        private List<int> _resources = new List<int>();

        public int DeliveryID
        {
            get { return _deliveryID; }
            set { _deliveryID = value; }
        }

        public Project DelProject
        {
            get { return _delProject; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid Project");
                _delProject = value;
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

        public Delivery(int DeliveryID, Project DelProject, DMLUser User, DateTime DelDate)
        {
            this.DeliveryID = DeliveryID;
            this.DelProject = DelProject;
            this.User = User;
            this.DelDate = DelDate;
        }
    }
}
