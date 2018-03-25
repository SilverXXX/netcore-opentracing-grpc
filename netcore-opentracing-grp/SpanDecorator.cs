using Grpc.Core.Interceptors;
using OpenTracing;
using OpenTracing.Tag;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.OpenTracing.GRPC
{
    static class SpanDecorator
    {
        static readonly String COMPONENT_NAME = "netcore-grpc";

        public static void OnRequest(ISpan span)
        {
            Tags.Component.Set(span, COMPONENT_NAME);
        }

        public static void OnResponse(ISpan span)
        {
            Tags.Component.Set(span, COMPONENT_NAME);
        }
    }
}
