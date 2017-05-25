using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    [Serializable()]
    public class Resources
    {
        private int _resourceID;

        public int ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }
        }

        public override string ToString()
        {
            return "Resource " + ResourceID.ToString();
        }

        public Resources() { }

        public Resources(int ResourceID)
        {
            this.ResourceID = ResourceID;
        }

    }

    [Serializable()]
    public class ElectronicResources : Resources
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
            return ProductName + " " + Model;
        }

        public override bool Equals(object obj)
        {
            if (!typeof(ElectronicResources).IsInstanceOfType(obj))
                return false;
            ElectronicResources o = obj as ElectronicResources;
            return ProductName == o.ProductName &&
                Manufactor == o.Manufactor &&
                Model == o.Model &&
                Description == o.Description;
        }

        public ElectronicResources(String ProductName, String Manufactor, String Model,
            String Description, Staff Creator, String PathToImage)
        {
            this.ProductName = ProductName;
            this.Manufactor = Manufactor;
            this.Model = Model;
            this.Description = Description;
            this.Creator = Creator;
            this.PathToImage = PathToImage;
        }
    }

    [Serializable()]
    public class ElectronicUnit : Resources
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
            set { _supplier = value; }
        }

        public override string ToString()
        {
            return Model.ProductName + " " + Model.Model + " by " + Model.Manufactor + " #" + ResourceID.ToString();
        }

        public ElectronicUnit(int ResourceID, ElectronicResources Model, String Supplier)
        {
            this.ResourceID = ResourceID;
            this.Model = Model;
            this.Supplier = Supplier;
        }

    }

    [Serializable()]
    public class Kit : Resources
    {
        private String _description;
        private List<ElectronicUnit> _units;

        public String Description
        {
            get { return _description; }
            set{ _description = value; }
        }

        public List<ElectronicUnit> Units
        {
            get { return _units; }
        }

        public void addUnit(ElectronicUnit unit)
        {
            _units.Add(unit);
        }

        public override string ToString()
        {
            return Description + " #" + ResourceID.ToString();
        }

        public Kit(int ResourceID, String Description)
        {
            _units = new List<ElectronicUnit>();
            this.ResourceID = ResourceID;
            this.Description = Description;
        }
    }

    [Serializable()]
    public class ListItem
    {
        public ListItem() { }
    }

    [Serializable()]
    public class ResourceItem : ListItem
    {
        private ElectronicResources _resource;
        private List<ElectronicUnit> _units;

        public ElectronicResources Resource
        {
            get { return _resource; }
            set { _resource = value; }
        }

        public List<ElectronicUnit> Units
        {
            get { return _units; }
            set { _units = value; }
        }

        public String MaxUnits
        {
            get { return Units.Count.ToString() + " available"; }
        }

        public void addUnit(ElectronicUnit newUnit)
        {
            if (!Units.Contains(newUnit))
                Units.Add(newUnit);
        }

        public bool requestUnitv1(int unitID)
        {
            foreach (ElectronicUnit unit in Units)
                if (unit.ResourceID == unitID)
                {
                    Units.Remove(unit);
                    return true;
                }
            return false;
        }

        public ElectronicUnit requestUnit()
        {
            ElectronicUnit toReturn = null;
            if (Units.Count > 0)
            {
                toReturn = Units.First();
                Units.Remove(toReturn);
            }
            return toReturn;
        }

        public override string ToString()
        {
            return Resource.ToString();
        }

        public ResourceItem(ElectronicResources Resource)
        {
            this.Resource = Resource;
            this.Units = new List<ElectronicUnit>();
        }
    }

    [Serializable()]
    public class KitItem : ListItem
    {
        private String _kitDescription;
        private List<Kit> _units;

        public String KitDescription
        {
            get { return _kitDescription; }
            set { _kitDescription = value; }
        }

        public List<Kit> Units
        {
            get { return _units; }
            set { _units = value; }
        }

        public String MaxUnits
        {
            get { return Units.Count.ToString() + " available"; }
        }

        public void addUnit(Kit newUnit)
        {
            if (!Units.Contains(newUnit))
                Units.Add(newUnit);
        }

        public Kit requestUnit()
        {
            Kit toReturn = null;
            if (Units.Count > 0)
            {
                toReturn = Units.First();
                Units.Remove(toReturn);
            }
            return toReturn;
        }

        public bool requestUnitv1(int unitID)
        {
            foreach (Kit unit in Units)
                if (unit.ResourceID == unitID)
                {
                    Units.Remove(unit);
                    return true;
                }
            return false;
        }

        public override string ToString()
        {
            return KitDescription;
        }

        public KitItem(String KitDescription)
        {
            this.KitDescription = KitDescription;
            this.Units = new List<Kit>();
        }
    }
}
