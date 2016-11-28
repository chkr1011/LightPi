using System;
using LightPi.OrchestratorFirmware.Core;
using LightPi.Protocol;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Schemas;

namespace LightPi.OrchestratorFirmware.Rest
{
    [RestController(InstanceCreationType.Singleton)]
    internal class StateController
    {
        private readonly Engine _engine;

        public StateController(Engine engine)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            _engine = engine;
        }

        [UriFormat("/Ping")]
        public GetResponse GetPing()
        {
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/State")]
        public GetResponse GetState()
        {
            var state = _engine.GetState();

            var response = new
            {
                BinaryState = BitConverter.ToString(state),
                LongState = BitConverter.ToUInt64(state, 0)
            };

            return new GetResponse(GetResponse.ResponseStatus.OK, response);
        }

        [UriFormat("/State/{state}")]
        public PutResponse SetState([FromContent] long state)
        {
            var stateValue = BitConverter.GetBytes(state);
            _engine.EnqueueState(stateValue);

            return new PutResponse(PutResponse.ResponseStatus.OK);
        }

        [UriFormat("/TurnOff")]
        public PostResponse TurnOff()
        {
            _engine.EnqueueState(new byte[LightPiProtocol.StateLength]);
            return new PostResponse(PostResponse.ResponseStatus.Created);
        }

        [UriFormat("/TurnOn")]
        public PostResponse TurnOn()
        {
            var state = new byte[LightPiProtocol.StateLength];
            for (var i = 0; i < state.Length; i++)
            {
                state[i] = 0xFF;
            }

            _engine.EnqueueState(state);
            return new PostResponse(PostResponse.ResponseStatus.Created);
        }
    }
}
