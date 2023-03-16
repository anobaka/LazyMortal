using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrap.Models
{
    public class RefWrapper<T>
    {
        public T Value;
        public RefWrapper(T value) => Value = value;
    }
}
