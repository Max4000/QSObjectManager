namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    /// <summary>
    /// 
    /// </summary>
    public class NameAndIdAndLastReloadTime 
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name;

        /// <summary>
        /// 
        /// </summary>
        public string Id;

        public string LastReloadTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="lastReloadTime"></param>
        public NameAndIdAndLastReloadTime(string name, string id, string lastReloadTime)
        {
            Name = name;
            Id = id;
            LastReloadTime = lastReloadTime;
        }

        public NameAndIdAndLastReloadTime()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(LastReloadTime)){
                return Name;
            }
            else
            {
                return Name + " " + LastReloadTime;
            }

        }

        public NameAndIdAndLastReloadTime Copy()
        {
            return new NameAndIdAndLastReloadTime(this.Name, this.Id,this.LastReloadTime);
        }
    }
}
