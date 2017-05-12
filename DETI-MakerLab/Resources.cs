using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    [Serializable()]
    class Resources
    {
        private int _resourceID;
        private Staff _creator;

        public int ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }
        }

        public Staff Creator
        {
            get { return _creator; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid Staff");
                _creator = value;
            }
        }

        public override string ToString()
        {
            return "Resource " + ResourceID.ToString();
        }

        public Resources() { }

        public Resources(int ResourceID, Staff Creator)
        {
            this.ResourceID = ResourceID;
            this.Creator = Creator;
        }
    }

    [Serializable()]
    class ElectronicResources
    {
        private String _productName;
        private String _manufactor;
        private String _model;
        private String _description;
        private Staff _creator;
        private String _pathToImage;

        public String ProductName
        {
            get { return _productName; }
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid ProductName");
                _productName = value;
            }
        }

        public String Manufactor
        {
            get { return _manufactor; }
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid Manufactor");
                _manufactor = value;
            }
        }

        public String Model
        {
            get { return _model; }
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid Model");
                _model = value;
            }
        }

        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Staff Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }

        public String PathToImage
        {
            get { return _pathToImage; }
            set { _pathToImage = value; }
        }

        public override string ToString()
        {
            return "Eletronic Resource: " + ProductName + " - " + Manufactor + " - " + Model;
        }

        public ElectronicResources(String ProductName, String Manufactor, String Model,
            String Description, Staff Creator, String PathToImage)
        {
            this.ProductName = ProductName;
            this.Manufactor = Manufactor;
            this.Model = Model;
            this.Creator = Creator;
            this.PathToImage = PathToImage;
        }
    }

    [Serializable()]
    class ElectronicUnit : Resources
    {
        private ElectronicResources _model;
        private String _supplier;

        public ElectronicResources Model
        {
            get { return _model; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid model");
                _model = value;
            }
        }

        public String Supplier
        {
            get { return _supplier; }
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid Supplier");
                _supplier = value;
            }
        }

        public ElectronicUnit(int ResourceID, ElectronicResources Model, String Supplier)
        {
            this.ResourceID = ResourceID;
            this.Model = Model;
            this.Supplier = Supplier;
        }

    }

    [Serializable()]
    class Kit : Resources
    {
        private String _description;

        public String Description
        {
            get { return _description; }
            set{ _description = value; }
        }

        public Kit(int ResourceID, String Description)
        {
            this.ResourceID = ResourceID;
            this.Description = Description;
        }
    }

}
