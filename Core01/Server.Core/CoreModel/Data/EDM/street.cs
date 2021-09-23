using System;
using System.Collections.Generic;

using Server.Core.Public;
namespace Server.Core.CoreModel
{
    public partial class street : IEntityObject
    {
   
        [System.ComponentModel.DataAnnotations.KeyAttribute]
        public int street_id { get; set; }
        public int village_id { get; set; }
        public int tstreet_id { get; set; }
        public string street_name { get; set; }

        long IEntityObject.Id { get { return street_id; } }
    }
}
