namespace Serialization.ObjectModel
{
    internal abstract class Root
    {
        public Byte wrapper;

        protected Int16 nameLength;
        protected string name;
        protected Int32 size;

        public Root()
        {
            this.name = "unknown";
            this.wrapper = 0;
            this.nameLength = 0;
            this.size = /* nameLength */ sizeof(Int16) 
                + /* wrapper */ sizeof(Byte) 
                + /* size */ sizeof(Int32);
        }

        public Int32 GetSize() { return size; }

        public void SetName(string name)
        {
            this.name = name;
            nameLength = (Int16)name.Length;
            size += nameLength;
        }

        public string GetName() { return name; }

        public virtual void Pack(ref IList<Byte> buffer, ref Int16 iterator) { }
    }
}
