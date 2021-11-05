using System.Collections;
using System.Text;

namespace Tsb.Security.Web.licence.KeyStores
{
    public class KeyStorePart2
    {
        readonly Hashtable _parts = new Hashtable
        {
            { 1, Encoding.UTF8.GetBytes("/qXfsGHFfQdKwKJgUZ+afKjFqjn7d55/IuFOyJaOQi/HqDO9J2slv/wcVNjW8qnZmxsimv10iQs+LyhdKycJpijcJW/f8yPN9xXVZE/FSCXWrnktfPRnpD/EbzmmDut3TOQOQ==</Modulus><Exponent>AAEAAQ==</Exponent></RSAKeyValue>") },
            {2, Encoding.UTF8.GetBytes("3IZFqoGFPFOowARGPcppvtkRNopV5Sb9YsKwiY4NGkxcFeLZW4FDXSJlJCfBZn0LAYjpJJXdfdcXUZtOGAaf0+3miR4L4HoCcUpDbcCdZM7kU03kEmrq8w+5lMZdltm/7A/8iX3eVMQ516xi1sygPZJ1MAna0=</Q><DP>OUUFjApz16J03vcmz3VJw+lz6WDZiQd5FBplLD8FLFMX0rQDdaDmPVyOkXqdmDmGfq9gMe/") }, 
            {3, Encoding.UTF8.GetBytes("IITWtio0XjEvdTMYp/GvfFi4T30WUaEVfwkiJPyYWDZIbVMdy496JZziwwH5SQpH/4Gei+aa621oS/+B9ias4OLwd00OR4ALVn0E=</DP><DQ>TePbpHFV/h4fTvv4B21tBx7YCVKYW2eixsLndxgYc1Hov34nackfpp7PtJ+Z/") }, 
            {4,Encoding.UTF8.GetBytes("E0MrnDdeknjOCNr7rMQUpoA7NVgIgDIAfxZJCUeitk8BGXXWn1KJK6b3wTOQkbBZi0WFtJqnjMkvBigaClb59fKJmALEC3oST5r1kHR49oZDxE=</DQ><InverseQ>VHJTObi/HT0sgvLqkiNExsSbi5rhozasV1HEANQSSysSPB9C/MtzSg7N4YWeTWk7/1QNeZ8BT66EzCHJIr/")},
        };

        public byte[] this[int key]
        {
            get { return (byte[])_parts[key]; }
        }
    }
}