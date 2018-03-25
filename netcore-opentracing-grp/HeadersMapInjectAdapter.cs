using Grpc.Core;
using OpenTracing.Propagation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetCore.OpenTracing.GRPC
{
    class HeadersMapInjectAdapter : ITextMap
    {
        private readonly Metadata dict;
        public HeadersMapInjectAdapter(Metadata headers)
        {
            dict = headers;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new InvalidOperationException("HeadersMapInjectAdapter should only be used with Tracer.inject()");
        }

        public void Set(string key, string value)
        {
            //TODO: check for dups
            dict.Add(key, value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new InvalidOperationException("HeadersMapInjectAdapter should only be used with Tracer.inject()");
        }
    }
}
