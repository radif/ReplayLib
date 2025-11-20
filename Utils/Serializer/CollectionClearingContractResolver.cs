using System;
using System.Collections;
using Newtonsoft.Json.Serialization;

namespace Replay.Utils
{
    public class CollectionClearingContractResolver : DefaultContractResolver
    {
        protected override JsonArrayContract CreateArrayContract(Type objectType)
        {
            var c = base.CreateArrayContract(objectType);
            c.OnDeserializingCallbacks.Add((obj, streamingContext) =>
            {
                var list = obj as IList;
                if (list != null && !list.IsReadOnly)
                    list.Clear();
            });
            return c;
        }
    }
}