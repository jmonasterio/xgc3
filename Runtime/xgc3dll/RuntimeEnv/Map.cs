using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using xgc3.Core;

namespace xgc3.RuntimeEnv
{
    public class Map<T> : Dictionary<string, T> where T : xgc3.Core.Instance
    {
        // Workaround for interface
        // Variance in one direction only so type expressinos are natural
        public IEnumerable<D> Convert<D>() where D : Instance
        {
            Dictionary<string, Instance>.ValueCollection vc = this.Values as Dictionary<string, Instance>.ValueCollection;
            return new EnumerableWrapper<Instance, D>( vc );
        }

        private class EnumerableWrapper<S, D> : IEnumerable<D>
            where D:S
        {
            public EnumerableWrapper(IEnumerable<S> source)
            {
                this.source = source;
            }

            public IEnumerator<D> GetEnumerator()
            {
                return new EnumeratorWrapper(this.source.GetEnumerator());
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            private class EnumeratorWrapper : IEnumerator<D>
            {
                public EnumeratorWrapper(IEnumerator<S> source)
                {
                    this.source = source;
                }

                private IEnumerator<S> source;

                public D Current
                {
                    get { return (D) this.source.Current; }
                }

                public void Dispose()
                {
                    this.source.Dispose();
                }

                object System.Collections.IEnumerator.Current
                {
                    get { return this.source.Current; }
                }

                public bool MoveNext()
                {
                    return this.source.MoveNext();
                }

                public void Reset()
                {
                    this.source.Reset();
                }
            }

            private IEnumerable<S> source;
        }

#if SOMEDAY
        // TODO: Add a wrapper that returns: IEnumerable<KeyValuePair<TKey, TValue>>
        // SO, I can iterate over key value pairs, instead of just values
        public IEnumerable<KeyValuePair<string, D>> ConvertKeyValuePair<D>() where D : Instance
        {
            // TODO
            Dictionary<string, Instance> map = this as Dictionary<string, Instance>;
            return new EnumerableWrapperKeyValuePair<IEnumerable<KeyValuePair<string, Instance>>, IEnumerable<KeyValuePair<string, D>>>(map);
        }

        // TODO
        private class EnumerableWrapperKeyValuePair<S, D> //: IEnumerable<KeyValuePair<string, D>> where D : S
        {
            public EnumerableWrapperKeyValuePair(IEnumerable<KeyValuePair<string, S>> source)
            {
                this.source = source;
            }
        }
#endif


    }

}

    




