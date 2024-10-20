using System.Collections;
using System.Collections.Generic;

namespace Core.GameInitialization
{
    public class InitializationProcessList : IEnumerable<InitializationProcess>
    {
        private List<InitializationProcess> _list = new();

        internal InitializationProcessList() { }

        public InitializationProcess CreateProcess() 
        {
            InitializationProcess result = new();
            _list.Add(result);
            return result;
        }

        public IEnumerator<InitializationProcess> GetEnumerator()
        {
            return ((IEnumerable<InitializationProcess>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
}
