using OpenTracing.Tag;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.OpenTracing.GRPC
{
    public static class GRPCTags
    {
        public static readonly StringTag GrpcMethodName = new StringTag("grpc.method_name");
        public static readonly StringTag GrpcMethodType = new StringTag("grpc.method_type");
        public static readonly StringTag GrpcAuthority = new StringTag("grpc.authority");
        public static readonly StringTag GrpcCallOptions = new StringTag("grpc.call_options");
        public static readonly StringTag GrpcCallAttributes = new StringTag("grpc.call_attributes");
        public static readonly StringTag GrpcCompressor = new StringTag("grpc.compressor");
        public static readonly IntTag GrpcDeadline = new IntTag("grpc.deadline_millis");
        public static readonly StringTag GrpcHeaders = new StringTag("grpc.headers");
        /* span.setTag("grpc.call_options", callOptions.toString());

                    break;
                case COMPRESSOR:
                    if (callOptions.getCompressor() == null) {
                        span.setTag("grpc.compressor", "null");
                    } else {
                        span.setTag("grpc.compressor", callOptions.getCompressor());
                    }
                    break;
                case DEADLINE:
                    if (callOptions.getDeadline() == null) {
                        span.setTag("grpc.deadline_millis", "null");
                    } else {
                        span.setTag("grpc.deadline_millis", callOptions.getDeadline().timeRemaining(TimeUnit.MILLISECONDS));
                    }
                    break;
                case METHOD_NAME:
                    span.setTag("grpc.method_name", method.getFullMethodName());
                    break;
                case METHOD_TYPE:
                    if (method.getType() == null) {
                        span.setTag("grpc.method_type", "null");
                    } else {
                        span.setTag("grpc.method_type", method.getType().toString());*/


        /*   switch (attr) {
                        case METHOD_TYPE:
                            span.setTag("grpc.method_type", call.getMethodDescriptor().getType().toString());
                            break;
                        case CALL_ATTRIBUTES:
                            span.setTag("grpc.call_attributes", call.getAttributes().toString());
                            break;
                        case HEADERS:
                            span.setTag("grpc.headers", headers.toString());*/
    }
}
