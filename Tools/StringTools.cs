using System;
using System.Text;

namespace TF.Tools
{
    public static class StringTools
    {
        public static string ArrayToString(Array array)
        {
            if (array == null)
            {
                return "[ NULL ]";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            foreach (var item in array)
            {
                if (item != null)
                {
                    sb.Append(item.ToString());
                }
                else
                {
                    sb.Append("NULL");
                }

                sb.Append(" , ");
            }

            sb[sb.Length-1] = ']';
            return sb.ToString();

        }
    }
}