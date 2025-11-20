using System;
using System.Collections.Generic;


namespace FXnRXn
{
    [Serializable]
    public class SkillAttributeEntity 
    {
        public int SkillID;
        public string SkillName;
        public List<SkillData> Data = new List<SkillData>();
    }
}

