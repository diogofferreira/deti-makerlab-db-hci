using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{

    [Serializable()]
    public class DMLUser
    {
        private int _numMec;
        private String _firstName;
        private String _lastName;
        private String _email;
        private String _passwordHash;
        private String _pathToImage;
        private int _roleID;

        public int NumMec
        {
            get { return _numMec;  }
            set
            {
                if (value > 100000) 
                    throw new Exception("Invalid NumMec");
                _numMec = value;
            }
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
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid password");
                _passwordHash = value;
            }
        }

        public String PathToImage
        {
            get { return _pathToImage; }
            set { _pathToImage = value; }
        }

        public int RoleID
        {
            get { return _roleID; }
            set { _roleID = value; }
        }

        public String FullName
        {
            get { return _firstName + ' ' + _lastName; }
        }

        public override String ToString()
        {
            return FullName + "(" + NumMec.ToString() + ")";
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

        public DMLUser() { }

        public DMLUser(int NumMec, String FirstName, String LastName, 
            String Email, String PasswordHash, String PathToImage)
        {
            this.NumMec = NumMec;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.PasswordHash = PasswordHash;
            this.PathToImage = PathToImage;
        }

    }

    [Serializable()]
    public class Professor : DMLUser
    {
        private String _scientificArea;

        public String ScientificArea
        {
            get { return _scientificArea; }
            set { _scientificArea = value; }
        }

        public override String ToString()
        {
            return "Professor: " + NumMec.ToString();
        }

        public Professor(int NumMec, String FirstName, String LastName,
            String Email, String PasswordHash, String PathToImage, String ScientificArea)
        {
            this.NumMec = NumMec;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.PasswordHash = PasswordHash;
            this.PathToImage = PathToImage;
            this.ScientificArea = ScientificArea;
        }
    }

    [Serializable()]
    public class Student : DMLUser
    {
        private String _course;

        public String Course
        {
            get { return _course; }
            set {
                if (value == null)
                    throw new Exception("Invalid course");
                _course = value; }
        }

        public override String ToString()
        {
            return "Student: " + NumMec.ToString();
        }

        public Student(int NumMec, String FirstName, String LastName,
            String Email, String PasswordHash, String PathToImage, String Course)
        {
            this.NumMec = NumMec;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.PasswordHash = PasswordHash;
            this.PathToImage = PathToImage;
            this.Course = Course;
        }
    }
}