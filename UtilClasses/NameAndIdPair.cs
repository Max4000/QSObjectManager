namespace UtilClasses
{
    /// <summary>
    /// 
    /// </summary>
    public class NameAndIdPair 
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name;

        /// <summary>
        /// 
        /// </summary>
        public string Id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public NameAndIdPair(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public NameAndIdPair()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        public NameAndIdPair Copy()
        {
            return new NameAndIdPair(this.Name, this.Id);
        }
    }
}
