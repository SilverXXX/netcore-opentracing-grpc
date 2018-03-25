using Grpc.Core;
using Grpc.Core.Interceptors;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCore.OpenTracing.GRPC
{
    public class TracingInterceptor : Interceptor
    {
        private readonly ITracer tracer;

        public TracingInterceptor(ITracer tracer)
        {
            this.tracer = tracer;
        }

        private ISpan BuildClientSpan<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context) where TRequest : class where TResponse: class {

            var spanBuilder = tracer.BuildSpan(context.Method.FullName)  //TODO: make an OperationNameConstructor configurable like in java impl?
                .IgnoreActiveSpan()
                .WithTag(Tags.SpanKind.Key, Tags.SpanKindClient)
                .WithTag(Tags.PeerHostname.Key, context.Host)
                .WithTag(Tags.PeerService.Key, context.Method.ServiceName)
                .WithTag(GRPCTags.GrpcMethodName.Key, context.Method.FullName)
                .WithTag(GRPCTags.GrpcMethodType.Key, context.Method.Type.ToString()); //TODO: Check this...

            if (context.Options.Deadline.HasValue)
                spanBuilder.WithTag(GRPCTags.GrpcDeadline.Key, (DateTime.Now - context.Options.Deadline).Value.Milliseconds);

            ISpanContext spanContext = null;

            if (context.Options.Headers != null)
            {
                // just in case if span context was injected manually to props in basicPublish
                spanContext = tracer.Extract(BuiltinFormats.TextMap,
                     new HeadersMapExtractAdapter(context.Options.Headers));
            }

            if (spanContext == null)
            {
                var parentSpan = tracer.ActiveSpan;
                if (parentSpan != null)
                {
                    spanContext = parentSpan.Context;
                }
            }

            if (spanContext != null)
            {
                spanBuilder.AsChildOf(spanContext);
            }

            var span = spanBuilder.Start();

            SpanDecorator.OnRequest(span);

            return span;
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var span = BuildClientSpan<TRequest, TResponse>(context);
            var res = base.AsyncClientStreamingCall(context, continuation);

            //TODO: How to close here? continue is right?

            res.ResponseHeadersAsync.ContinueWith((r) => {
                if (r.IsCanceled)
                {
                    span.Log("canceled");
                } else
                if (r.IsFaulted)
                {
                    span.Log("faulted"); //add r.Excpetion? //must use error as event log see https://github.com/opentracing/specification/blob/master/semantic_conventions.md#log-fields-table
                }
                else { 
                    span.Log(new Dictionary<string, object> { { "completed", res.GetStatus().StatusCode } });
                }
                span.Finish();
            }); 

            return res;
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncDuplexStreamingCall(context, continuation);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncServerStreamingCall(request, context, continuation);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncUnaryCall(request, context, continuation);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var span = BuildClientSpan<TRequest, TResponse>(context);
            TResponse res;
            try
            {
                res = base.BlockingUnaryCall(request, context, continuation);
            }
            finally
            {
                span.Finish();
            }
            //TODO: Errors?
            return res;
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return base.ClientStreamingServerHandler(requestStream, context, continuation);
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            return base.UnaryServerHandler(request, context, continuation);
        }
    }
}
