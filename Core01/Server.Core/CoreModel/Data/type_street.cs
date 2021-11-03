using System;
using System.Collections.Generic;

using Server.Core.Public;
namespace Server.Core.CoreModel
{
    public partial class type_street : IEntityObject
    {
   
        [System.ComponentModel.DataAnnotations.KeyAttribute]
        public int tstreet_id { get; set; }
        public string tstreet_name { get; set; }

        long IEntityObject.Id { get { return tstreet_id; } }
    }
}
