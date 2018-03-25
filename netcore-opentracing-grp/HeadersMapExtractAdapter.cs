using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using OpenTracing.Propagation;

namespace NetCore.OpenTracing.GRPC
{
    class HeadersMapExtractAdapter : ITextMap
    {
        private readonly IDictionary<string, string> dict = new Dictionary<string, string>();
        public HeadersMapExtractAdapter(Metadata headers)
        {
            foreach (var kvp in headers)
            {
                if (kvp.IsBinary)
                    dict.Add(kvp.Key, Encoding.UTF8.GetString(kvp.ValueBytes));
                else
                    dict.Add(kvp.Key, kvp.Value);
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public void Set(string key, string value)
        {
            throw new InvalidOperationException("HeadersMapExtractAdapter should only be used with Tracer.extract()");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }
}
