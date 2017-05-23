using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    [Serializable()]
    public class Staff
    {
        private int _employeeNum;
        private String _firstName;
        private String _lastName;
        private String _email;
        private String _pathToImage;

        public int EmployeeNum
        {
            get { return _employeeNum; }
            set { _employeeNum = value; }
        }

        public String FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public String LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public String Email
        {
            get { return _email; }
            set
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(value);
                    if (addr.Address != value)
                        throw new Exception("Invalid email");
                    _email = value;
                }
                catch
                {
                    throw new Exception("Invalid email");
                }
            }
        }

        public String PathToImage
        {
            get { return _pathToImage; }
            set { _pathToImage = value; }
        }

        public String FullName
        {
            get { return _firstName + ' ' + _lastName; }
        }

        public override String ToString()
        {
            return "Staff:" + _employeeNum.ToString();
        }

        public Staff() { }

        public Staff(int EmployeeNum, String FirstName, String LastName,
            String Email, String PathToImage)
        {
            this.EmployeeNum = EmployeeNum;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.PathToImage = PathToImage;
        }
    }
}
