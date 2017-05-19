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
        private String _passwordHash;
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

        public String PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = value; }
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

        public bool verifyPassword(String password)
        {
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(PasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }

        public static String hashPassword(String password)
        {
            if (password == null | String.IsNullOrEmpty(password))
                throw new Exception("Invalid password");
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        public override String ToString()
        {
            return "Staff:" + _employeeNum.ToString();
        }

        public Staff() { }

        public Staff(int EmployeeNum, String FirstName, String LastName,
            String Email, String PasswordHash, String PathToImage)
        {
            this.EmployeeNum = EmployeeNum;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.PasswordHash = PasswordHash;
            this.PathToImage = PathToImage;
        }
    }
}
