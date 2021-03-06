// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/faceService.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace FaceService {
  /// <summary>
  /// The greeting service definition.
  /// </summary>
  public static partial class FaceRecongnise
  {
    static readonly string __ServiceName = "greet.FaceRecongnise";

    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    static readonly grpc::Marshaller<global::FaceService.FaceRequest> __Marshaller_greet_FaceRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::FaceService.FaceRequest.Parser));
    static readonly grpc::Marshaller<global::FaceService.FaceReply> __Marshaller_greet_FaceReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::FaceService.FaceReply.Parser));
    static readonly grpc::Marshaller<global::FaceService.AliveRequest> __Marshaller_greet_AliveRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::FaceService.AliveRequest.Parser));
    static readonly grpc::Marshaller<global::FaceService.AliveReply> __Marshaller_greet_AliveReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::FaceService.AliveReply.Parser));

    static readonly grpc::Method<global::FaceService.FaceRequest, global::FaceService.FaceReply> __Method_RecongnizationByFace = new grpc::Method<global::FaceService.FaceRequest, global::FaceService.FaceReply>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "RecongnizationByFace",
        __Marshaller_greet_FaceRequest,
        __Marshaller_greet_FaceReply);

    static readonly grpc::Method<global::FaceService.AliveRequest, global::FaceService.AliveReply> __Method_CheckAlive = new grpc::Method<global::FaceService.AliveRequest, global::FaceService.AliveReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "CheckAlive",
        __Marshaller_greet_AliveRequest,
        __Marshaller_greet_AliveReply);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::FaceService.FaceServiceReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of FaceRecongnise</summary>
    [grpc::BindServiceMethod(typeof(FaceRecongnise), "BindService")]
    public abstract partial class FaceRecongniseBase
    {
      public virtual global::System.Threading.Tasks.Task RecongnizationByFace(grpc::IAsyncStreamReader<global::FaceService.FaceRequest> requestStream, grpc::IServerStreamWriter<global::FaceService.FaceReply> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::FaceService.AliveReply> CheckAlive(global::FaceService.AliveRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(FaceRecongniseBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_RecongnizationByFace, serviceImpl.RecongnizationByFace)
          .AddMethod(__Method_CheckAlive, serviceImpl.CheckAlive).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, FaceRecongniseBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_RecongnizationByFace, serviceImpl == null ? null : new grpc::DuplexStreamingServerMethod<global::FaceService.FaceRequest, global::FaceService.FaceReply>(serviceImpl.RecongnizationByFace));
      serviceBinder.AddMethod(__Method_CheckAlive, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::FaceService.AliveRequest, global::FaceService.AliveReply>(serviceImpl.CheckAlive));
    }

  }
}
#endregion
